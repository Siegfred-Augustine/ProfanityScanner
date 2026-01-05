using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ProfanityScanner.Models.Classes
{
    public class TrieNode
    {
        public static Dictionary<char,char> letterEquiv = new Dictionary<char,char>{ 
            {'i', 'e'},
            {'e', 'i'},
            {'o', 'u'},
            {'u', 'o'}
        };
        public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>(4);
        public bool isEndOfWord { get; set; }

        public string overallWord { get; set; }
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
            var completedWord = new System.Text.StringBuilder();
            foreach (char c in word)
            {
                char ch = char.ToLowerInvariant(c);

                // If the value in "ch" already exist within the keys of the node
                if (!node.Children.TryGetValue(ch, out TrieNode next))
                {
                    next = new TrieNode();
                    node.Children[ch] = next;
                }

                node = next;
                completedWord.Append(c);
            }
            node.overallWord = completedWord.ToString();
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
            TrieNode lastNode = new TrieNode();

            for (int i = 0; i < n; i++)
            {
                TrieNode node = root;
                char prev = '\0';
                int lastMatch = -1;

                for (int j = i; j < n; j++)
                {
                    // Convert current char to lowercase
                    char c = char.ToLowerInvariant(text[j]);
                    // Convert next char to lowercase
                    // If current char 'c' is within the dictionary of the current node, move to the next node containing 'c' 
                    if (node.Children.TryGetValue(c, out TrieNode next))
                    {
                        node = next;
                    }
                    else if(TrieNode.letterEquiv.TryGetValue(c, out char equivC))
                    {
                        Console.WriteLine("firing");
                        if(node.Children.TryGetValue(equivC, out TrieNode equivNode))
                        {
                          Console.WriteLine("inner if");    
                          node = equivNode;
                        }
                    }
                    // If the current char 'c' is a duplicate of the previous, do nothing (case: "tannnga")
                    else if (prev == c ) 
                    {
                        // Do nothing

                    }
                    else 
                    { 
                        break; 
                    }

                    // Doesn't work for profane words ending in two similar letters (ex: piste ginoo)
                    
                       Console.WriteLine(node.overallWord);
                    if (node.isEndOfWord)
                    {
                        lastMatch = j;
                        lastNode = node;
                    }

                    prev = c;
                }

                if(lastMatch != -1){
                  matches.Add((i, lastMatch));
                  i = lastMatch - 1;
                  Scanner.originalProfane.Add(lastNode.overallWord);
                }
            }

            return matches;
        }
    }
}

