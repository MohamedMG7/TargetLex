using CommandLine;
using System.Text.RegularExpressions;

namespace TargetLex
{
    internal static class Program
    {
        private static readonly Regex SafeFileName = new(@"^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);

        private static int Main(string[] args)
        {
            var exitCode = 0;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => exitCode = RunOptions(opts))
                .WithNotParsed(_ => exitCode = 2);

            return exitCode;
        }

        private static int RunOptions(Options opts)
        {
            var validationErrors = ValidateOptions(opts);
            if (validationErrors.Count > 0)
            {
                foreach (var error in validationErrors)
                {
                    WriteStatus($"[!] {error}", ConsoleColor.Red);
                }

                return 2;
            }

            if (!opts.NoAnimation)
            {
                RenderBanner();
            }

            var generator = new PasswordPatternGenerator();
            var candidates = generator.Generate(
                opts.TargetName.Trim(),
                opts.TargetNickname.Trim(),
                opts.TargetBirthYear.Trim(),
                NormalizeNumber(opts.TargetBirthMonth),
                NormalizeNumber(opts.TargetBirthDay),
                LeetMode.Parse(opts.LeetMode));

            if (!string.IsNullOrWhiteSpace(opts.CheckPassword))
            {
                return RunPasswordCheck(opts.CheckPassword, candidates);
            }

            var filtered = ApplyFilters(candidates, opts).ToList();
            var outputPath = WriteToTextFile(opts.OutputDirectory, opts.OutputFileName, filtered);

            if (!opts.NoAnimation)
            {
                RenderProgress(filtered.Count);
            }

            if (opts.Preview)
            {
                PrintPreview(filtered);
            }

            Console.WriteLine();
            WriteStatus($"Generated {filtered.Count:N0} unique candidates -> {outputPath}", ConsoleColor.Green);
            return 0;
        }

        private static List<string> ValidateOptions(Options opts)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(opts.TargetName))
            {
                errors.Add("--target-name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(opts.TargetNickname))
            {
                errors.Add("--target-nickname cannot be empty.");
            }

            if (!IsFourDigitYear(opts.TargetBirthYear))
            {
                errors.Add("--birth-year must be a four-digit year.");
            }

            if (!TryParseMonth(opts.TargetBirthMonth, out var month))
            {
                errors.Add("--birth-month must be a number from 1 to 12.");
            }

            if (!TryParseDay(opts.TargetBirthDay, out var day))
            {
                errors.Add("--birth-day must be a number from 1 to 31.");
            }

            if (month > 0 && day > 0 && !DateTime.TryParse($"{opts.TargetBirthYear}-{month:00}-{day:00}", out _))
            {
                errors.Add("--birth-year, --birth-month, and --birth-day must form a valid date.");
            }

            if (!LeetMode.IsValid(opts.LeetMode))
            {
                errors.Add("--leet must be one of: off, basic, advanced.");
            }

            if (opts.MinLength is <= 0)
            {
                errors.Add("--min-length must be greater than zero.");
            }

            if (opts.MaxLength is <= 0)
            {
                errors.Add("--max-length must be greater than zero.");
            }

            if (opts.MinLength.HasValue && opts.MaxLength.HasValue && opts.MinLength > opts.MaxLength)
            {
                errors.Add("--min-length cannot be greater than --max-length.");
            }

            if (opts.MaxResults is <= 0)
            {
                errors.Add("--max-results must be greater than zero.");
            }

            if (opts.CheckPassword is not null && string.IsNullOrWhiteSpace(opts.CheckPassword))
            {
                errors.Add("--check-password cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(opts.OutputFileName) && string.IsNullOrWhiteSpace(opts.CheckPassword))
            {
                errors.Add("--output-file is required when generating a wordlist.");
            }

            if (!string.IsNullOrWhiteSpace(opts.OutputFileName) && !SafeFileName.IsMatch(opts.OutputFileName))
            {
                errors.Add("--output-file may only contain letters, numbers, dots, underscores, and hyphens.");
            }

            if (string.IsNullOrWhiteSpace(opts.OutputDirectory))
            {
                errors.Add("--output-dir cannot be empty.");
            }

            return errors;
        }

        private static int RunPasswordCheck(string password, IReadOnlyList<string> candidates)
        {
            var exactMatches = new HashSet<string>(candidates, StringComparer.Ordinal);

            Console.WriteLine();
            if (exactMatches.Contains(password))
            {
                WriteStatus("[!] Targetable: this password matches a TargetLex generated pattern.", ConsoleColor.Red);
                Console.WriteLine($"Matched candidate: {password}");
                Console.WriteLine("Reason: it can be built from the supplied target details and selected leetspeak mode.");
                return 1;
            }

            WriteStatus("[+] No direct TargetLex pattern match found.", ConsoleColor.Green);
            Console.WriteLine("This is safer against this specific generated pattern set, but still use a long unique password.");
            return 0;
        }

        private static IEnumerable<string> ApplyFilters(IEnumerable<string> candidates, Options opts)
        {
            var query = candidates.Where(candidate => !string.IsNullOrWhiteSpace(candidate));

            if (opts.MinLength.HasValue)
            {
                query = query.Where(candidate => candidate.Length >= opts.MinLength.Value);
            }

            if (opts.MaxLength.HasValue)
            {
                query = query.Where(candidate => candidate.Length <= opts.MaxLength.Value);
            }

            if (opts.MaxResults.HasValue)
            {
                query = query.Take(opts.MaxResults.Value);
            }

            return query;
        }

        private static string WriteToTextFile(string outputDirectory, string outputFileName, List<string> wordlist)
        {
            var outputDir = Path.GetFullPath(outputDirectory);
            Directory.CreateDirectory(outputDir);

            var fileName = outputFileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                ? outputFileName
                : outputFileName + ".txt";

            var outputPath = Path.Combine(outputDir, fileName);
            File.WriteAllLines(outputPath, wordlist);
            return outputPath;
        }

        private static void RenderBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("""
              ______                   __  __              
             /_  __/___ ___________  _/ /_/ /   ___  _  __
              / / / __ `/ ___/ __ `/ / __/ /   / _ \| |/_/
             / / / /_/ / /  / /_/ / / /_/ /___/  __/>  <  
            /_/  \__,_/_/   \__, /  \__/_____/\___/_/|_|  
                            /____/                         
            """);
            Console.ResetColor();
            Console.WriteLine("Targeted wordlist generator");
            Console.WriteLine();
        }

        private static void RenderProgress(int count)
        {
            const int width = 34;
            var frames = new[] { "|", "/", "-", "\\" };

            for (var i = 0; i <= 100; i++)
            {
                var filled = i * width / 100;
                var bar = new string('#', filled) + new string('-', width - filled);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("\r[");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(bar);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("] ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{i,3}% ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{frames[i % frames.Length]} ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"building {count:N0} candidates");
                Thread.Sleep(12);
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        private static void PrintPreview(List<string> candidates)
        {
            Console.WriteLine();
            WriteStatus("Preview:", ConsoleColor.Yellow);
            foreach (var candidate in candidates.Take(20))
            {
                Console.WriteLine($"  {candidate}");
            }
        }

        private static void WriteStatus(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static bool IsFourDigitYear(string value)
        {
            return Regex.IsMatch(value.Trim(), @"^\d{4}$");
        }

        private static bool TryParseMonth(string value, out int month)
        {
            return int.TryParse(value.Trim(), out month) && month is >= 1 and <= 12;
        }

        private static bool TryParseDay(string value, out int day)
        {
            return int.TryParse(value.Trim(), out day) && day is >= 1 and <= 31;
        }

        private static string NormalizeNumber(string value)
        {
            return int.Parse(value.Trim()).ToString("00");
        }
    }

    public enum LeetModeValue
    {
        Off,
        Basic,
        Advanced
    }

    internal static class LeetMode
    {
        public static bool IsValid(string value)
        {
            return Enum.TryParse<LeetModeValue>(Normalize(value), ignoreCase: true, out _);
        }

        public static LeetModeValue Parse(string value)
        {
            return Enum.Parse<LeetModeValue>(Normalize(value), ignoreCase: true);
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "off" => nameof(LeetModeValue.Off),
                "basic" => nameof(LeetModeValue.Basic),
                "advanced" => nameof(LeetModeValue.Advanced),
                _ => value
            };
        }
    }
}
