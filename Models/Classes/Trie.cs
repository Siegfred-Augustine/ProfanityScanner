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
          {'u', 'o'},
          {'v', 'u'}
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
            int i = 0;

            while (i < n)
            {
                Console.WriteLine($"\n=== Checking position {i} ===");
                Console.WriteLine($"Char: '{text[i]}', Remaining: '{text.Substring(i)}'");
                
                var result = SearchFromPosition(text, i);
                
                if (result.endPos != -1)
                {
                    string matchedText = text.Substring(i, result.endPos - i + 1);
                    Console.WriteLine($"✓ Found: '{result.word}' from pos {i} to {result.endPos}");
                    Console.WriteLine($"  Matched text: '{matchedText}'");
                    matches.Add((i, result.endPos));
                    Scanner.originalProfane.Add(result.word);
                    
                    // Check if the last character of this match could start a new word
                    char lastChar = char.ToLowerInvariant(text[result.endPos]);
                    int nextPos = result.endPos + 1;
                    
                    // If the last character can start a word from root, check from that position instead
                    if (root.Children.ContainsKey(lastChar))
                    {
                        //this will check if the last character of the current word can be the start of another profane word
                        var testMatch = MatchWord(text, result.endPos, root, result.endPos, '\0');
                        if (testMatch.endPos != -1)
                        {
                            Console.WriteLine($"  Last char '{lastChar}' can start a new word, backtracking to pos {result.endPos}");
                            nextPos = result.endPos; //sets the next checking position as the last character of the current word
                        }
                    }
                    
                    i = nextPos;
                    Console.WriteLine($"  Next check will be at position {i}");
                    if (i < text.Length)
                    {
                        Console.WriteLine($"  Next char: '{text[i]}'");
                    }
                }
                else
                {
                    Console.WriteLine($"✗ No match found at position {i}");
                    i++;
                }
            }
            
            Console.WriteLine($"\n=== Total matches: {matches.Count} ===");

            return matches;
        }

        private (int endPos, string word) SearchFromPosition(string text, int startPos)
        {
            // Try to find the longest profanity match starting at startPos
            var result = MatchWord(text, startPos, root, startPos, '\0');
            
            if (result.endPos == -1)
                return (-1, null);
            
            // Consume trailing duplicates of the last character
            // BUT stop if the next character could start a new word
            int finalEnd = result.endPos;
            char lastChar = char.ToLowerInvariant(text[result.endPos]);
            
            while (finalEnd + 1 < text.Length)
            {
                char nextChar = char.ToLowerInvariant(text[finalEnd + 1]);
                bool isDuplicate = (nextChar == lastChar);
                bool isEquivalent = letterEquiv.TryGetValue(nextChar, out char equiv) && equiv == lastChar;
                
                if (isDuplicate || isEquivalent)
                {
                    // Check if this character could start a new word from the root
                    // If it can, don't consume it as a trailing duplicate
                    if (root.Children.ContainsKey(nextChar))
                    {
                        // Check if consuming this would prevent finding a word
                        // by doing a quick look-ahead
                        var lookAhead = MatchWord(text, finalEnd + 1, root, finalEnd + 1, '\0');
                        if (lookAhead.endPos != -1)
                        {
                            // There's a word starting here, don't consume this character
                            break;
                        }
                    }
                    
                    finalEnd++;
                }
                else
                {
                    break;
                }
            }
            
            Console.WriteLine($"Last Position {finalEnd}"); 
            return (finalEnd, result.word);
        }

        private (int endPos, string word) MatchWord(string text, int textPos, TrieNode node, int startPos, char prevChar)
        {
            if (textPos >= text.Length)
                return (-1, null);
            
            char currentChar = char.ToLowerInvariant(text[textPos]);
            int bestEnd = -1;
            string bestWord = null;
            
            // Strategy 1: Match current character directly
            if (node.Children.TryGetValue(currentChar, out TrieNode directChild))
            {
                if (directChild.isEndOfWord)
                {
                    bestEnd = textPos;
                    bestWord = directChild.overallWord;
                }
                
                var directResult = MatchWord(text, textPos + 1, directChild, startPos, currentChar);
                if (directResult.endPos > bestEnd)
                {
                    bestEnd = directResult.endPos;
                    bestWord = directResult.word;
                }
            }
            
            // Strategy 2: Try letter substitution (i-e, o-u)
            if (letterEquiv.TryGetValue(currentChar, out char substitutedChar))
            {
                if (node.Children.TryGetValue(substitutedChar, out TrieNode substChild))
                {
                    if (substChild.isEndOfWord && textPos > bestEnd)
                    {
                        bestEnd = textPos;
                        bestWord = substChild.overallWord;
                    }
                    
                    var substResult = MatchWord(text, textPos + 1, substChild, startPos, substitutedChar);
                    if (substResult.endPos > bestEnd)
                    {
                        bestEnd = substResult.endPos;
                        bestWord = substResult.word;
                    }
                }
            }
            
            // Strategy 3: Skip duplicate/equivalent character
            // Only skip if we're not at the starting position AND current char is a duplicate
            bool canSkip = textPos > startPos && 
                          (currentChar == prevChar || 
                           (letterEquiv.TryGetValue(currentChar, out char currentEquiv) && currentEquiv == prevChar));
            
            if (canSkip)
            {
                var skipResult = MatchWord(text, textPos + 1, node, startPos, prevChar);
                if (skipResult.endPos > bestEnd)
                {
                    bestEnd = skipResult.endPos;
                    bestWord = skipResult.word;
                }
            }
            return (bestEnd, bestWord);
        } 
               
    } 
   
}



