using static System.Net.Mime.MediaTypeNames;

namespace ProfanityScanner.Models.Classes
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } =
        new Dictionary<char, TrieNode>(4);
        public bool isEndOfWord { get; set; }

    }
    public class Trie
    {
        private readonly TrieNode root = new TrieNode();
        private readonly IWebHostEnvironment _env;
        public Trie(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void Insert(string word)
        {
            TrieNode node = root;

            foreach (char c in word)
            {
                char ch = char.ToLowerInvariant(c);

                if (!node.Children.TryGetValue(ch, out TrieNode next))
                {
                    next = new TrieNode();
                    node.Children[ch] = next;
                }

                node = next;
            }
            node.isEndOfWord = true;
        }

        public void InsertFile(string folder, string file)
        {
            string path = Path.Combine(_env.ContentRootPath, folder, file);

            foreach (string line in File.ReadLines(path))
            {
                string word = line.Trim();

                if (!string.IsNullOrWhiteSpace(word))
                {
                    Insert(word);
                    Console.WriteLine(word);
                }
            }
        }


        public List<(int start, int end)> FindProfanity(string text)
        {
            List<(int, int)> matches = [];
            int n = text.Length;

            for (int i = 0; i < n; i++)
            {
                char cur = char.ToLowerInvariant(text[i]);

                if (!char.IsLetter(cur) || !root.Children.ContainsKey(cur))
                {
                    continue;
                }

                TrieNode node = root;
                int start = i;

                while(i < n && node.Children.TryGetValue(cur, out var nextNode)) {
                    node = nextNode;

                    char next;
                    do {
                        next = (i + 1 >= n) ? '\0' : char.ToLowerInvariant(text[i + 1]);
                        cur = char.ToLowerInvariant(text[i++]);
                    } while(i < n && next == cur && !node.Children.ContainsKey(next)) ;

                    cur = next;
                }

                if (node.isEndOfWord)
                {
                    matches.Add((start, i - 1));
                }

                while (i < n && (!char.IsWhiteSpace(cur) || !char.IsLetter(cur)))
                {
                    cur = char.ToLowerInvariant(text[i++]);
                }
            }

            return matches;
        }
    }
}

