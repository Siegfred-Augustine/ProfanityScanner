using static System.Net.Mime.MediaTypeNames;

namespace ProfanityScanner.Models.Classes
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>(4);
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

                // If the value in "ch" already exist within the children of node, 
                Console.WriteLine(node.Children);
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
                    //Console.WriteLine(word);
                }
            }
        }


        public List<(int start, int end)> FindProfanity(string text)
        {
            List<(int, int)> matches = new();
            int n = text.Length;

            for (int i = 0; i < n; i++)
            {
                TrieNode node = root;
                char prev = '\0';

                for (int j = i; j < n; j++)
                {
                    char c = char.ToLowerInvariant(text[j]);
                    char c2  = ' ';

                    if (j + 1 < n )
                        c2 = char.ToLowerInvariant(text[j + 1]);

                    if (node.Children.TryGetValue(c, out TrieNode next))
                    {
                        node = next;
                    }
                    else if (prev == c) //do nothing
                    {
                    }
                    else if (node.Children.ContainsKey(c2))
                    {
                        continue;
                    }
                    else break;

                    prev = c;

                    if (node.isEndOfWord)
                    {
                        matches.Add((i, j));
                    }
                }
            }

            return matches;
        }
    }
}

