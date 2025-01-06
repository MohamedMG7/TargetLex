using System.Xml.Linq;

namespace TargetLex
{
    public class PasswordPatternGenerator
    {

        List<string> WordList;
        List<char> specialCases = new List<char>{
                '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+',
                '[', ']', '{', '}', '\\', '|', ';', ':', '\'', '"', ',', '<', '>', '.', '/',
                '?', '`', '~'
        };

        private readonly Dictionary<char, string[]> BasicLeetspeakMap = new Dictionary<char, string[]>
        {
            {'a', new[] {"4", "@"}},
            {'b', new[] {"8", "|3"}},
            {'c', new[] {"(", "<"}},
            {'d', new[] {"|)", "|}"}},
            {'e', new[] {"3", "€"}},
            {'f', new[] {"|=", "ph"}},
            {'g', new[] {"6", "9"}},
            {'h', new[] {"#", "|-|"}},
            {'i', new[] {"1", "!"}},
            {'j', new[] {"_|", "]"}},
            {'k', new[] {"|<", "|{"}},
            {'l', new[] {"1", "|"}},
            {'m', new[] {"/\\/\\", "|\\/|"}},
            {'n', new[] {"|\\|", "/\\/"}},
            {'o', new[] {"0", "()"}},
            {'p', new[] {"|>", "|*"}},
            {'q', new[] {"9", "(_,)"}},
            {'r', new[] {"2", "|2"}},
            {'s', new[] {"5", "$"}},
            {'t', new[] {"7", "+"}},
            {'u', new[] {"|_|", "(_)"}},
            {'v', new[] {"\\/", "|/"}},
            {'w', new[] {"\\/\\/", "vv"}},
            {'x', new[] {"><", "}{"}},
            {'y', new[] {"¥", "\\|/"}},
            {'z', new[] {"2", "7_"}}
        };

        
        private readonly Dictionary<char, string[]> AdvancedLeetspeakMap = new Dictionary<char, string[]>
        {
            {'a', new[] {"4", "@", "∆", "^", "α", "/\\", "Д", "å"}},
            {'b', new[] {"8", "6", "13", "|3", "ß", "|}", "฿", "|s"}},
            {'c', new[] {"(", "{", "[", "<", "©", "¢", "€", "¢", "₵"}},
            {'d', new[] {")|", "|)", "|}", "|>", "∂", "đ", "Ð", "|]"}},
            {'e', new[] {"3", "€", "£", "[-", "ə", "ε", "ë", "[-", "£"}},
            {'f', new[] {"|=", "ƒ", "|#", "ph", "/=", "ſ", "ʃ"}},
            {'g', new[] {"6", "9", "&", "(_+", "gee", "(?", "ɢ", "ģ"}},
            {'h', new[] {"#", "/-/", "[-]", "]-[", "}-{", "|-|", "╫"}},
            {'i', new[] {"1", "!", "|", "eye", "3y3", "][", "ї", "ỉ"}},
            {'j', new[] {"]", "_|", "_/", "¿", "<|", "_]", "ʝ"}},
            {'k', new[] {"|<", "|{", "|(", "|X", "|c", "￡", "ķ"}},
            {'l', new[] {"1", "7", "|_", "|", "£", "¬", "ł"}},
            {'m', new[] {"/\\/\\", "|\\/|", "em", "|v|", "^^", "nn", "ɱ"}},
            {'n', new[] {"|\\|", "/\\/", "η", "^/", "[\]", "<\\>", "ñ"}},
            {'o', new[] {"0", "()", "oh", "[]", "¤", "Ø", "ö", "ø"}},
            {'p', new[] {"|°", "|>", "|*", "|D", "þ", "|º", "ρ"}},
            {'q', new[] {"(_,)", "9", "()_", "2", "¶", "Ø", "q̊"}},
            {'r', new[] {"2", "|`", "12", "®", "Я", "|2", "ŕ"}},
            {'s', new[] {"5", "$", "z", "§", "ehs", "es", "ŝ"}},
            {'t', new[] {"7", "+", "†", "']['", "✝", "-|-", "ţ"}},
            {'u', new[] {"|_|", "(_)", "v", "µ", "บ", "μ", "û"}},
            {'v', new[] {"\\/", "|/", "\\|", "√", "∨", "̩", "ν"}},
            {'w', new[] {"\\/\\/", "vv", "'//", "\\^/", "\\X/", "\\|/", "ω"}},
            {'x', new[] {"><", "Ж", "}{", "ecks", "×", "?", "χ"}},
            {'y', new[] {"¥", "`/", "Ч", "7", "\\|/", "¤", "ý"}},
            {'z', new[] {"2", "7_", "-/_", "%", "≥", "\"/_", "ž"}}
        };

        public PasswordPatternGenerator()
        {
            WordList = new List<string>();
        }

        public List<string> BasicPatterns(string name, string nickname, string birthyear, string birthmonth, string birthday)
        {
            // Shortened versions of date components
            string shortBirthYear = birthyear.Length > 2 ? birthyear.Substring(2) : birthyear;
            string shortBirthMonth = birthmonth.TrimStart('0');
            string shortBirthDay = birthday.TrimStart('0');

            // 1. Simple Name and Date Combinations
            WordList.Add(name + birthyear);
            WordList.Add(birthyear + name);
            WordList.Add(name + birthmonth + birthday);
            WordList.Add(birthyear + birthmonth + birthday);

            WordList.Add(name + shortBirthYear);
            WordList.Add(shortBirthYear + name);
            WordList.Add(name + shortBirthMonth + shortBirthDay);
            WordList.Add(shortBirthYear + shortBirthMonth + shortBirthDay);

            // Patterns with nickname
            WordList.Add(nickname + birthyear);
            WordList.Add(birthyear + nickname);
            WordList.Add(nickname + birthmonth + birthday);
            WordList.Add(birthyear + birthmonth + nickname);

            WordList.Add(nickname + shortBirthYear);
            WordList.Add(shortBirthYear + nickname);
            WordList.Add(nickname + shortBirthMonth + shortBirthDay);
            WordList.Add(shortBirthYear + shortBirthMonth + nickname);

            // 2. Uppercase and Lowercase Variations
            WordList.Add(name.ToUpper());
            WordList.Add(name.ToLower());
            WordList.Add(name[0].ToString().ToUpper() + name.Substring(1).ToLower());

            WordList.Add(nickname.ToUpper());
            WordList.Add(nickname.ToLower());
            WordList.Add(nickname[0].ToString().ToUpper() + nickname.Substring(1).ToLower());

            // 3. Number Variations
            WordList.Add(name + "123");
            WordList.Add("123" + name);
            WordList.Add(name + birthyear);
            WordList.Add(birthyear + name);

            WordList.Add(nickname + "123");
            WordList.Add("123" + nickname);
            WordList.Add(nickname + birthyear);
            WordList.Add(birthyear + nickname);

            // 4. Symbol Variations
            foreach (char c in specialCases)
            {
                WordList.Add(name + c + birthyear);
                WordList.Add(birthyear + c + name);
                WordList.Add(name + c);
                WordList.Add(c + name);
                WordList.Add(name + c + birthmonth + birthday);
                WordList.Add(name + c + shortBirthYear);

                WordList.Add(nickname + c + birthyear);
                WordList.Add(birthyear + c + nickname);
                WordList.Add(nickname + c);
                WordList.Add(c + nickname);
                WordList.Add(nickname + c + birthmonth + birthday);
                WordList.Add(nickname + c + shortBirthYear);
            }

            // 5. Repeated Words
            WordList.Add(name + name);
            WordList.Add(birthyear + birthyear);

            WordList.Add(nickname + nickname);
            WordList.Add(birthyear + birthyear);

            // 6. Reverse Patterns
            WordList.Add(ReverseString(name) + birthyear);
            WordList.Add(birthyear + ReverseString(name));

            WordList.Add(ReverseString(nickname) + birthyear);
            WordList.Add(birthyear + ReverseString(nickname));

            // 7. Mixed Case Patterns
            WordList.Add(MixCase(name));
            WordList.Add(MixCase(nickname));

            // 8. Padding Numbers
            WordList.Add(name + birthyear.PadLeft(4, '0'));
            WordList.Add(name.PadRight(8, '0') + birthmonth + birthday);

            WordList.Add(nickname + birthyear.PadLeft(4, '0'));
            WordList.Add(nickname.PadRight(8, '0') + birthmonth + birthday);

            // 9. Special Ending and Starting Characters
            WordList.Add(name + "!" + birthyear);
            WordList.Add("!" + name + birthyear);

            WordList.Add(nickname + "!" + birthyear);
            WordList.Add("!" + nickname + birthyear);

            // 10. Numeric Variations
            WordList.Add(name + "789");
            WordList.Add("456" + name);

            WordList.Add(nickname + "789");
            WordList.Add("456" + nickname);

            // 12. Season + Year Combinations
            string[] seasons = { "spring", "summer", "fall", "winter" };
            foreach (string season in seasons)
            {
                WordList.Add(season + birthyear);
                WordList.Add(season + shortBirthYear);
                WordList.Add(season.Capitalize() + birthyear);
            }

            return WordList.Distinct().ToList(); 
        }

        private string ReverseString(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private string MixCase(string input)
        {
            char[] mixed = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                mixed[i] = i % 2 == 0 ? char.ToUpper(input[i]) : char.ToLower(input[i]);
            }
            return new string(mixed);
        }

        private PasswordPatternGenerator LeetSpeak(bool isAdvanced,List<string> Wordlist) {
            List<string> leetwords = new List<string>();
            var map = isAdvanced ? AdvancedLeetspeakMap : BasicLeetspeakMap;

            foreach (string word in WordList) { 
                
            }

            return this;
        }

        private List<string> GenerateLeetVariations(string input, Dictionary<char, string[]> leetMap)
        {
            var results = new List<string> { input };

            foreach (var charMap in leetMap)
            {
                if (input.Contains(charMap.Key))
                {
                    var currentResults = new List<string>(results);
                    foreach (var replacement in charMap.Value)
                    {
                        foreach (var existingString in currentResults)
                        {
                            results.Add(existingString.Replace(charMap.Key.ToString(), replacement));
                        }
                    }
                }
            }

            return results.Distinct().ToList();
        }
    }
}
