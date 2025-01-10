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
            List<string> wordlist;
            if (opts.AdvancedLeetspeakOption)
            {
                wordlist = generator.BasicPatterns(opts.Target_Name, opts.Target_Nickname, opts.Target_Birthdate).LeetSpeak(true).GetPatterns();
            } else if (opts.LeetspeakOption) {
                wordlist = generator.BasicPatterns(opts.Target_Name, opts.Target_Nickname, opts.Target_Birthdate).LeetSpeak(false).GetPatterns();
            }
            else {
                wordlist = generator.BasicPatterns(opts.Target_Name, opts.Target_Nickname, opts.Target_Birthdate).GetPatterns();
            }
            
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

        //static void GeneratingEffect(List<string> wordList) {
        //    foreach (string word in wordList)
        //    {
        //        Console.Write("\r" + word + " ");
        //        Thread.Sleep(1);
        //    }
        //    Console.Write("");
        //    Console.Write("\r" + "Wordlist Ready");
        //}

        static void GeneratingEffect(List<string> wordList)
        {
            int maxLength = 0; // Track max length of printed word

            // Display progress by iterating through each word
            foreach (string word in wordList)
            {
                maxLength = Math.Max(maxLength, word.Length); // Update max length
                Console.Write("\r" + word + " "); // " " for the noisy effect 
                Thread.Sleep(1);
            }

            // Final message
            Console.Write("\r" + "Wordlist Ready".PadRight(maxLength)); // Clear remaining characters
        }

    }
}
