namespace TargetLex
{
    public sealed class PasswordPatternGenerator
    {
        private static readonly char[] SpecialCharacters =
        [
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+',
            '[', ']', '{', '}', '\\', '|', ';', ':', '\'', '"', ',', '<', '>', '.', '/',
            '?', '`', '~'
        ];

        private static readonly Dictionary<char, string[]> BasicLeetspeakMap = new()
        {
            ['a'] = ["4", "@"],
            ['b'] = ["8"],
            ['e'] = ["3"],
            ['g'] = ["6", "9"],
            ['i'] = ["1", "!"],
            ['l'] = ["1", "|"],
            ['o'] = ["0"],
            ['s'] = ["5", "$"],
            ['t'] = ["7", "+"],
            ['z'] = ["2"]
        };

        private static readonly Dictionary<char, string[]> AdvancedLeetspeakMap = new()
        {
            ['a'] = ["4", "@", "^"],
            ['b'] = ["8", "6", "13", "|3"],
            ['c'] = ["(", "{", "[", "<"],
            ['d'] = [")|", "|)", "|}"],
            ['e'] = ["3", "[-"],
            ['f'] = ["|=", "ph"],
            ['g'] = ["6", "9", "&"],
            ['h'] = ["#", "/-/", "|-|"],
            ['i'] = ["1", "!", "|"],
            ['j'] = ["]", "_|", "_/"],
            ['k'] = ["|<", "|{"],
            ['l'] = ["1", "7", "|_", "|"],
            ['m'] = ["/\\/\\", "|\\/|", "^^"],
            ['n'] = ["|\\|", "/\\/"],
            ['o'] = ["0", "()", "[]"],
            ['p'] = ["|>", "|*"],
            ['q'] = ["9", "(_,)", "()_"],
            ['r'] = ["2", "12", "|2"],
            ['s'] = ["5", "$", "z"],
            ['t'] = ["7", "+", "-|-"],
            ['u'] = ["|_|", "(_)"],
            ['v'] = ["\\/", "|/"],
            ['w'] = ["\\/\\/", "vv"],
            ['x'] = ["><", "}{"],
            ['y'] = ["`/", "7", "\\|/"],
            ['z'] = ["2", "7_", "-/_"]
        };

        public IReadOnlyList<string> Generate(
            string name,
            string nickname,
            string birthYear,
            string birthMonth,
            string birthDay,
            LeetModeValue leetMode)
        {
            var wordlist = new HashSet<string>(StringComparer.Ordinal);
            AddBasicPatterns(wordlist, name, nickname, birthYear, birthMonth, birthDay);

            if (leetMode != LeetModeValue.Off)
            {
                var leetMap = leetMode == LeetModeValue.Advanced
                    ? AdvancedLeetspeakMap
                    : BasicLeetspeakMap;

                foreach (var word in wordlist.ToArray())
                {
                    foreach (var variation in GenerateLeetVariations(word, leetMap))
                    {
                        wordlist.Add(variation);
                    }
                }
            }

            return wordlist
                .OrderBy(word => word.Length)
                .ThenBy(word => word, StringComparer.Ordinal)
                .ToList();
        }

        private static void AddBasicPatterns(
            HashSet<string> wordlist,
            string name,
            string nickname,
            string birthYear,
            string birthMonth,
            string birthDay)
        {
            var shortBirthYear = birthYear.Length > 2 ? birthYear[2..] : birthYear;
            var shortBirthMonth = birthMonth.TrimStart('0');
            var shortBirthDay = birthDay.TrimStart('0');
            if (shortBirthMonth.Length == 0) shortBirthMonth = "0";
            if (shortBirthDay.Length == 0) shortBirthDay = "0";

            var names = new[] { name, nickname };
            var dateParts = new[]
            {
                birthYear,
                shortBirthYear,
                birthMonth + birthDay,
                shortBirthMonth + shortBirthDay,
                birthYear + birthMonth + birthDay,
                shortBirthYear + shortBirthMonth + shortBirthDay
            };

            foreach (var targetName in names)
            {
                AddNameShapes(wordlist, targetName);

                foreach (var datePart in dateParts)
                {
                    wordlist.Add(targetName + datePart);
                    wordlist.Add(datePart + targetName);
                    wordlist.Add(Capitalize(targetName) + datePart);
                }

                wordlist.Add(targetName + "123");
                wordlist.Add("123" + targetName);
                wordlist.Add(targetName + "789");
                wordlist.Add("456" + targetName);
                wordlist.Add(targetName + targetName);
                wordlist.Add(ReverseString(targetName) + birthYear);
                wordlist.Add(birthYear + ReverseString(targetName));
                wordlist.Add(MixCase(targetName));
                wordlist.Add(targetName + birthYear.PadLeft(4, '0'));
                wordlist.Add(targetName.PadRight(8, '0') + birthMonth + birthDay);
                wordlist.Add(targetName + "!" + birthYear);
                wordlist.Add("!" + targetName + birthYear);

                foreach (var special in SpecialCharacters)
                {
                    wordlist.Add(targetName + special + birthYear);
                    wordlist.Add(birthYear + special + targetName);
                    wordlist.Add(targetName + special);
                    wordlist.Add(special + targetName);
                    wordlist.Add(targetName + special + birthMonth + birthDay);
                    wordlist.Add(targetName + special + shortBirthYear);
                }
            }

            wordlist.Add(birthYear + birthYear);

            foreach (var season in new[] { "spring", "summer", "fall", "winter" })
            {
                wordlist.Add(season + birthYear);
                wordlist.Add(season + shortBirthYear);
                wordlist.Add(Capitalize(season) + birthYear);
            }
        }

        private static void AddNameShapes(HashSet<string> wordlist, string value)
        {
            wordlist.Add(value);
            wordlist.Add(value.ToUpperInvariant());
            wordlist.Add(value.ToLowerInvariant());
            wordlist.Add(Capitalize(value));
        }

        private static string ReverseString(string input)
        {
            var charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static string MixCase(string input)
        {
            var mixed = new char[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                mixed[i] = i % 2 == 0 ? char.ToUpperInvariant(input[i]) : char.ToLowerInvariant(input[i]);
            }

            return new string(mixed);
        }

        private static string Capitalize(string input)
        {
            return input.Length switch
            {
                0 => input,
                1 => input.ToUpperInvariant(),
                _ => char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant()
            };
        }

        private static IReadOnlyList<string> GenerateLeetVariations(string input, Dictionary<char, string[]> leetMap)
        {
            var results = new HashSet<string>(StringComparer.Ordinal) { input };

            foreach (var (letter, replacements) in leetMap)
            {
                if (!input.Contains(letter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var existing in results.ToArray())
                {
                    foreach (var replacement in replacements)
                    {
                        results.Add(ReplaceIgnoreCase(existing, letter, replacement));
                    }
                }
            }

            return results.ToList();
        }

        private static string ReplaceIgnoreCase(string input, char oldChar, string replacement)
        {
            var result = input;
            result = result.Replace(oldChar.ToString(), replacement, StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }
}
