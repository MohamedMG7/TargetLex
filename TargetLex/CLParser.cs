using CommandLine;

namespace TargetLex
{
    public class Options {
        [Option('o', "OutputFileName", Required = true, HelpText = "Set Output Name for generated file")]
        public string OutputFileName { get; set; } = null!;

        [Option('n', "Target_Name", Required = true, HelpText = "Set Name Of The Target")]
        public string Target_Name { get; set; } = null!;

        [Option('k', "Target_Nickname", Required = true, HelpText = "Set Nickname Of The Target")]
        public string Target_Nickname { get; set; } = null!;

        [Option('y', "Target_Birthdate", Required = true, HelpText = "Set Birthdate of the target")]
        public string Target_Birthdate { get; set; } = null!;

        [Option("SimpleLeet", Required = false, HelpText = "Add leetspeak variations")]
        public bool LeetspeakOption { get; set; }

        [Option("AdvancedLeet", Required = false, HelpText = "Add leetspeak variations")]
        public bool AdvancedLeetspeakOption { get; set; }

    }
}
    