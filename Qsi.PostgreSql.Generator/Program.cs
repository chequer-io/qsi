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
using LibGit2Sharp;
using Qsi.PostgreSql.Generator.Extensions;
using Qsi.PostgreSql.Generator.Models;
using Qsi.PostgreSql.Generator.Resources;

namespace Qsi.PostgreSql.Generator
{
    public static class Program
    {
        private const string libpgQueryRepo = "https://github.com/lfittl/libpg_query.git";

        public static void Main()
        {
            // if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            // {
            //     Console.WriteLine("Not supported platform");
            //     return;
            // }

            var config = ResourceManager.FindJsonResource<GenerateConfig>("Config.10-latest.json");

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

            Parallel.ForEach(cppEnums, GenerateEnum);
            Parallel.ForEach(cppClasses, GenerateClass);
        }

        private static void GenerateEnum(CppEnum cppEnum)
        {
            // TODO: generate
        }

        private static void GenerateClass(CppClass cppClass)
        {
            // TODO: generate
        }
    }
}
