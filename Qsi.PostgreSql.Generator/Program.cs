using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using CliWrap;
using CliWrap.Buffered;
using CppAst;
using CSharpSyntax;
using CSharpSyntax.Printer;
using LibGit2Sharp;
using Qsi.PostgreSql.Generator.Extensions;
using Qsi.PostgreSql.Generator.Generators;
using Qsi.PostgreSql.Generator.Models;
using Qsi.PostgreSql.Generator.Resources;
using Syntax = CSharpSyntax.Syntax;

namespace Qsi.PostgreSql.Generator
{
    public static class Program
    {
        private const string libpgQueryRepo = "https://github.com/lfittl/libpg_query.git";

        public static void Main()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("Not supported platform");
                return;
            }

            Generate("10-latest");
        }

        private static void Generate(string version)
        {
            var solutionDir = FindSolutionDirectory();

            var config = ResourceManager.FindJsonResource<GenerateConfig>($"Config.{version}.json");
            config.OutputDirectory = config.OutputDirectory?.Replace("$(SolutionDir)", solutionDir);

            try
            {
                // clone libpg_query
                var repoDirectory = InitializeRepository(config, Path.GetFullPath("tmp"));

                // compile
                var compilation = Compile(config, repoDirectory);

                Generate(compilation, config, repoDirectory);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string FindSolutionDirectory()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);

            while (dir != null)
            {
                if (dir.GetFiles("Qsi.sln").Any())
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            return null;
        }

        private static string InitializeRepository(GenerateConfig config, string directory)
        {
            directory = Path.Combine(directory, config.Branch);

            if (!Directory.Exists(directory))
            {
                Repository.Clone(libpgQueryRepo, directory, new CloneOptions
                {
                    BranchName = config.Branch
                });

                foreach (var fetchFile in config.Fetches)
                {
                    var file = Path.Combine(directory, fetchFile.File);
                    var code = new StringBuilder(File.ReadAllText(file));

                    foreach (var fetch in fetchFile.Fetches)
                    {
                        code.Replace(fetch.Find, fetch.Replace);
                    }

                    File.WriteAllText(file, code.ToString());
                }
            }

            return directory;
        }

        private static CppCompilation Compile(GenerateConfig config, string directory)
        {
            Environment.CurrentDirectory = directory;

            Regex[] sourcePatterns = config.CompileConfig.Sources
                .Select(StringExtension.MakeWildcardPattern)
                .ToArray();

            Regex[] sourceExcludePatterns = config.CompileConfig.SourceExcludes
                .Select(StringExtension.MakeWildcardPattern)
                .ToArray();

            List<string> sources = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(directory, f).Replace("\\", "/"))
                .Where(f =>
                {
                    if (!sourcePatterns.Any(p => p.IsMatch(f)))
                        return false;

                    if (sourceExcludePatterns.Any(p => p.IsMatch(f)))
                        return false;

                    return true;
                })
                .ToList();

            var parserOptions = new CppParserOptions
            {
                ParseAsCpp = true,
                ParseMacros = true,
                ParseAttributes = true,
                ParseSystemIncludes = true,
                AdditionalArguments =
                {
                    "--target=x86_64-apple-darwin19.5.0",
                    "-Wall",
                    "-Wno-unused-function",
                    "-Wno-unused-value",
                    "-Wno-unused-variable",
                    "-fno-strict-aliasing",
                    "-fwrapv",
                    "-fPIC",
                    "-Wno-nullability-completeness"
                }
            };

            parserOptions.IncludeFolders.AddRange(GetGnuIncludes());
            parserOptions.IncludeFolders.Add("./");
            parserOptions.IncludeFolders.AddRange(config.CompileConfig.IncludeFolders);

            return CppParser.ParseFiles(sources, parserOptions);
        }

        private static IEnumerable<string> GetGnuIncludes()
        {
            if (!HasCommand("gcc"))
                throw new OperationCanceledException("gcc not found");

            var result = Cli.Wrap("gcc")
                .WithArguments("-v -x c -E -")
                .ExecuteBufferedAsync()
                .Task.Result;

            const string find = "search starts here:";
            var index = result.StandardError.LastIndexOf(find, StringComparison.Ordinal);

            if (index == -1)
                throw new OperationCanceledException("gcc includes not found");

            var includes = result.StandardError[(index + find.Length)..];
            var pattern = new Regex(@"(?:\/[^\/\r\n]+)+");

            return pattern.Matches(includes)
                .Select(m => m.Value)
                .Where(Directory.Exists)
                .ToArray();
        }

        private static bool HasCommand(string command)
        {
            var result = Cli.Wrap("which")
                .WithArguments(command)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync()
                .Task.Result;

            return result.ExitCode == 0;
        }

        private static void Generate(CppCompilation result, GenerateConfig config, string directory)
        {
            if (result.HasErrors)
                throw new OperationCanceledException(result.Diagnostics.ToString());

            Regex[] targets = config.Targets
                .Select(StringExtension.MakeWildcardPattern)
                .ToArray();

            if (Directory.Exists(config.OutputDirectory))
                Directory.Delete(config.OutputDirectory, true);

            Directory.CreateDirectory(config.OutputDirectory);

            var generator = CreateGenerator(config);
            var pathMap = new Dictionary<CppElement, string>();

            IEnumerable<CppElement> cppElements = result.Children()
                .OfType<CppElement>()
                .Where(e =>
                {
                    var file = e.Span.Start.File;

                    if (string.IsNullOrEmpty(file) || !File.Exists(file))
                        return false;

                    var relative = Path.GetRelativePath(directory, file);

                    if (relative.StartsWith("../"))
                        return false;

                    if (!targets.Any(t => t.IsMatch(relative)))
                        return false;

                    pathMap[e] = relative;

                    return true;
                });

            foreach (var element in cppElements)
            {
                IEnumerable<SyntaxNode> csNodes;

                switch (element)
                {
                    case CppEnum cppEnum:
                        csNodes = generator.Generate(cppEnum);
                        break;

                    case CppClass cppClass:
                        csNodes = generator.Generate(cppClass);
                        break;

                    case CppTypedef cppTypedef:
                        csNodes = generator.Generate(cppTypedef);
                        break;

                    default:
                        continue;
                }

                var namespaceSyntax = Syntax.NamespaceDeclaration(config.Namespace);

                var unitSyntax = new CompilationUnitSyntax
                {
                    LeadingTrivia =
                    {
                        Syntax.BlockComment(CreateLeadingComment(element, pathMap[element]))
                    },
                    Members =
                    {
                        namespaceSyntax
                    }
                };

                namespaceSyntax.Members.Clear();

                foreach (var csNode in csNodes)
                {
                    switch (csNode)
                    {
                        case UsingDirectiveSyntax usingDirectiveSyntax:
                            unitSyntax.Usings.Add(usingDirectiveSyntax);
                            break;

                        case MemberDeclarationSyntax memberDeclarationSyntax:
                            namespaceSyntax.Members.Add(memberDeclarationSyntax);
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }

                using var stream = File.Create(Path.Combine(config.OutputDirectory, $"{((ICppMember)element).Name}.cs"));
                using var writer = new StreamWriter(stream);
                using var printer = new SyntaxPrinter(new SyntaxWriter(writer));

                printer.Visit(unitSyntax);
            }
        }

        private static string CreateLeadingComment(CppElement element, string path)
        {
            var builder = new StringBuilder()
                .AppendLine("Generated by QSI")
                .AppendLine()
                .Append(" Date: ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd"))
                .Append(" Span: ")
                .Append($"{element.Span.Start.Line}:{element.Span.Start.Column}")
                .Append(" - ")
                .Append($"{element.Span.End.Line}:{element.Span.End.Column}")
                .AppendLine()
                .Append(" File: ").AppendLine(path)
                .AppendLine();

            return builder.ToString();
        }

        private static ISourceGenerator CreateGenerator(GenerateConfig config)
        {
            switch (config.Branch)
            {
                case "10-latest":
                    return new Pg10Generator(config);
            }

            throw new NotSupportedException();
        }
    }
}
