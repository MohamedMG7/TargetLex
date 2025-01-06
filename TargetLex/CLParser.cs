using CommandLine;

namespace TargetLex
{
    public class Options {
        [Option('o', "OutputFileName", Required = true, HelpText = "Set Output Name for generated file")]
        public string OutputFileName { get; set; } = null!;

        [Option('n', "Target Name", Required = true, HelpText = "Set Name Of The Target")]
        public string Target_Name { get; set; } = null!;

        [Option('k', "Target Nickname", Required = true, HelpText = "Set Nickname Of The Target")]
        public string Target_Nickname { get; set; } = null!;

        [Option('y', "Target Birthyear", Required = true, HelpText = "Set Birthyaer of the target")]
        public string Target_Birthyear { get; set; } = null!;

        [Option('m', "Target Birthmonth", Required = true, HelpText = "Set Birthmonth of the target")]
        public string Target_BirthMonth { get; set; } = null!;
        
        [Option('d', "Target Birthday", Required = true, HelpText = "Set Birthday of the target")]
        public string Target_BirthDay { get; set; } = null!;


    }
}
    