using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ProfanityScanner.Models.Classes
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>(4);
        public bool isEndOfWord { get; set; }

        public string overallWord { get; set; }
    }
    public class Trie
    {
        private readonly TrieNode root = new TrieNode();
        private readonly IWebHostEnvironment _env;
        private Dictionary<char,char> letterEquiv = new Dictionary<char,char>(){
          {'i', 'e'},
          {'e', 'i'},
          {'o', 'u'},
          {'u', 'o'}
        };

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
                }
            }
        }

        
         public List<(int start, int end)> FindProfanity(string text)
          {
              List<(int, int)> matches = new();
              int n = text.Length;

              for (int i = 0; i < n; i++)
              {
                  var result = FindLongestMatch(text, i, root, i, '\0');
                  
                  if (result.matchEnd != -1)
                  {
                      matches.Add((i, result.matchEnd));
                      Scanner.originalProfane.Add(result.node.overallWord);
                      i = result.matchEnd; // Skip past the matched word
                  }
              }

              return matches;
          }
      private (int matchEnd, TrieNode node) FindLongestMatch(
          string text, int start, TrieNode node, int pos, char prev)
      {
          // Base case: reached end of text
          if (pos >= text.Length)
              return (-1, null);
          
          char c = char.ToLowerInvariant(text[pos]);
          int bestEnd = -1;
          TrieNode bestNode = null;
          
          // Option 1: Try direct character match
          if (node.Children.TryGetValue(c, out TrieNode directNext))
          {
              // Check if this position completes a word
              if (directNext.isEndOfWord)
              {
                  bestEnd = pos;
                  bestNode = directNext;
              }
              
              // Recursively try to extend the match further
              var result = FindLongestMatch(text, start, directNext, pos + 1, c);
              if (result.matchEnd > bestEnd)
              {
                  bestEnd = result.matchEnd;
                  bestNode = result.node;
              }
          }
          
          // Option 2: Try character substitution (i↔e, o↔u)
          if (letterEquiv.TryGetValue(c, out char equivChar))
          {
              if (node.Children.TryGetValue(equivChar, out TrieNode equivNext))
              {
                  // Check if substitution completes a word
                  if (equivNext.isEndOfWord && pos > bestEnd)
                  {
                      bestEnd = pos;
                      bestNode = equivNext;
                  }
                  
                  // Recursively try to extend with substitution
                  var result = FindLongestMatch(text, start, equivNext, pos + 1, c);
                  if (result.matchEnd > bestEnd)
                  {
                      bestEnd = result.matchEnd;
                      bestNode = result.node;
                  }
              }
          }
          
          // Option 3: Skip duplicate character (e.g., "booo" matches "boo")
          if (prev == c)
          {
              var result = FindLongestMatch(text, start, node, pos + 1, c);
              if (result.matchEnd > bestEnd)
              {
                  bestEnd = result.matchEnd;
                  bestNode = result.node;
              }
          }
          
          // After finding a match, consume any trailing duplicates of the last character
          if (bestEnd != -1 && bestNode != null)
          {
              int extendedEnd = bestEnd;
              char lastChar = char.ToLowerInvariant(text[bestEnd]);
              
              // Keep consuming duplicate characters after the match
              while (extendedEnd + 1 < text.Length && 
                    char.ToLowerInvariant(text[extendedEnd + 1]) == lastChar)
              {
                  extendedEnd++;
              }
              
              bestEnd = extendedEnd;
          }
          
          return (bestEnd, bestNode);
      }
       
    }
}

