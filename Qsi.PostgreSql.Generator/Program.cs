using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            var pathMap = new Dictionary<CppElement, string>();
            var cppEnums = new List<CppEnum>();
            var cppClasses = new List<CppClass>();

            foreach (var element in result.Children().OfType<CppElement>())
            {
                var file = element.Span.Start.File;

                if (string.IsNullOrEmpty(file) || !File.Exists(file))
                    continue;

                var relative = Path.GetRelativePath(directory, file);

                if (relative.StartsWith("../"))
                    continue;

                if (!targets.Any(t => t.IsMatch(relative)))
                    continue;

                pathMap[element] = relative;

                switch (element)
                {
                    case CppEnum cppEnum:
                        cppEnums.Add(cppEnum);
                        break;

                    case CppClass cppClass:
                        cppClasses.Add(cppClass);
                        break;
                }
            }

            if (Directory.Exists(config.OutputDirectory))
                Directory.Delete(config.OutputDirectory, true);

            Directory.CreateDirectory(config.OutputDirectory);

            var generator = CreateGenerator(config);

            var namespaceSyntax = Syntax.NamespaceDeclaration(config.Namespace);

            foreach (var cppEnum in cppEnums)
            {
                using var stream = File.Create(Path.Combine(config.OutputDirectory, $"{cppEnum.Name}.cs"));
                using var writer = new StreamWriter(stream);
                using var printer = new SyntaxPrinter(new SyntaxWriter(writer));

                namespaceSyntax.LeadingTrivia.Clear();
                namespaceSyntax.LeadingTrivia.Add(Syntax.BlockComment(CreateLeadingComment(cppEnum, pathMap[cppEnum])));

                namespaceSyntax.Members.Clear();

                foreach (var csEnum in generator.Generate(cppEnum))
                    namespaceSyntax.Members.Add(csEnum);

                printer.Visit(namespaceSyntax);
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
