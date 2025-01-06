using CommandLine;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace TargetLex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                 .WithParsed<Options>(opts => RunOptions(opts)); 
        }

        static void RunOptions(Options opts)
        {
            PasswordPatternGenerator generator = new PasswordPatternGenerator();
            List<string> wordlist = generator.BasicPatterns(opts.Target_Name,opts.Target_Nickname,opts.Target_Birthyear,opts.Target_BirthMonth,opts.Target_BirthDay);
            GeneratingEffect(wordlist);
            writeToTextfile(opts.OutputFileName,wordlist);
        }

        static void writeToTextfile(string outputFileName,List<string> wordlist) {
            
            string wordlistPath = Environment.CurrentDirectory;

            string wordlistPathdir = Path.Combine(wordlistPath, "wordlists");

            if (!Directory.Exists(wordlistPathdir))
            {
                Directory.CreateDirectory(wordlistPathdir);
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(wordlistPathdir, outputFileName +".txt"))) { 
                foreach (string word in wordlist) {
                    
                    outputFile.WriteLine(word);

                }
            }

        }

        static void GeneratingEffect(List<string> wordList) {
            foreach (string word in wordList)
            {
                Console.Write("\r" + word + " ");
                Thread.Sleep(10);
            }
            Console.Write("\r" + "Wordlist Ready");
        }

    }
}
