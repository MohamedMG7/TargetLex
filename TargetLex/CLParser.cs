using CommandLine;

namespace TargetLex
{
    public sealed class Options
    {
        [Option('o', "output-file", Required = true, HelpText = "Output file name, without extension unless you want to include one.")]
        public string OutputFileName { get; set; } = string.Empty;

        [Option("output-dir", Required = false, Default = "wordlists", HelpText = "Directory where the generated wordlist will be saved.")]
        public string OutputDirectory { get; set; } = "wordlists";

        [Option('n', "target-name", Required = true, HelpText = "Target first name or primary username fragment.")]
        public string TargetName { get; set; } = string.Empty;

        [Option('k', "target-nickname", Required = true, HelpText = "Target nickname, handle, or secondary username fragment.")]
        public string TargetNickname { get; set; } = string.Empty;

        [Option('y', "birth-year", Required = true, HelpText = "Target birth year, preferably four digits.")]
        public string TargetBirthYear { get; set; } = string.Empty;

        [Option('m', "birth-month", Required = true, HelpText = "Target birth month, 1-12 or 01-12.")]
        public string TargetBirthMonth { get; set; } = string.Empty;

        [Option('d', "birth-day", Required = true, HelpText = "Target birth day, 1-31 or 01-31.")]
        public string TargetBirthDay { get; set; } = string.Empty;

        [Option("leet", Required = false, Default = "off", HelpText = "Leetspeak mode: off, basic, or advanced.")]
        public string LeetMode { get; set; } = "off";

        [Option("min-length", Required = false, HelpText = "Discard candidates shorter than this length.")]
        public int? MinLength { get; set; }

        [Option("max-length", Required = false, HelpText = "Discard candidates longer than this length.")]
        public int? MaxLength { get; set; }

        [Option("max-results", Required = false, HelpText = "Maximum number of candidates to write after filtering.")]
        public int? MaxResults { get; set; }

        [Option("preview", Required = false, Default = false, HelpText = "Print a short preview of generated candidates.")]
        public bool Preview { get; set; }

        [Option("no-animation", Required = false, Default = false, HelpText = "Disable banner and progress animation.")]
        public bool NoAnimation { get; set; }

    }
}
