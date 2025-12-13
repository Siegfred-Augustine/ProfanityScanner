using ProfanityScanner.Models.Classes;

using static System.Net.Mime.MediaTypeNames;

namespace ProfanityScanner.Models.Classes
{
    public class Scanner
    {
        private static Dictionary<char, char> dict = new Dictionary<char, char> {
            {'@', 'a'},
            {'4', 'a'},
            {'8', 'b'},
            {'3', 'e'},
            {'1', 'i'},
            {'|', 'i'},
            {'0', 'o'},
            {'v', 'u'},
            {'$', 's'},
            {'#', 'h'}
        };

        public string output { get; set;}
        public Scanner()
        {
            output = " ";
        }
        public static string Substitute(string source)
        {
            string text = source.ToLower();
            var output = new System.Text.StringBuilder();

            foreach(char ch in text)
            {
                if(dict.TryGetValue(ch, out char c)){
                    output.Append(c);
                }
                else
                {
                    output.Append(ch);
                }
            }
            return output.ToString();
        }
        public string Censor(string source, List<(int start, int end)> matches)
        {
            if (matches.Count == 0)
                return source;

            char[] chars = source.ToCharArray();

            foreach (var (start, end) in matches)
            {
                for (int i = start; i <= end; i++)
                {
                    chars[i] = '*';
                }
            }

            return new string(chars);
        }

    }
}

