# Profanity Scanner

An intelligent profanity filtering web application built with ASP.NET Core MVC that uses advanced text processing algorithms to detect and censor inappropriate language.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-7.0-purple.svg)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-green.svg)

## 🚀 Features

- **Smart Text Detection** - Identifies profane words even with character obfuscation (e.g., `@` for `a`, `4` for `a`, `1` for `i`)
- **Advanced Matching Algorithm** - Handles duplicate letters, character substitutions, and misspellings
- **Real-time Processing** - Instant text analysis and censoring
- **Scan History** - Keeps track of all scans with timestamps
- **Educational Design** - Clean, well-documented code perfect for learning algorithms

## 📋 Table of Contents

- [How It Works](#how-it-works)
- [Usage](#usage)
- [Examples](#examples)
- [Algorithm Details](#algorithm-details)
- [Performance](#performance)
- [Customization](#customization)

### Tech Stack

- **Backend**: ASP.NET Core 8.0, C#
- **Frontend**: Razor Views, Bootstrap
- **Data Structure**: Custom Trie (Prefix Tree) implementation
- **Algorithm**: Recursive pattern matching with multiple strategies


## 🔍 How It Works

### 1. Text Normalization

The scanner first normalizes input text by substituting common obfuscation characters:

- `@` → `a`, `4` → `a`, `1` → `i`, `|` → `i`
- `0` → `o`, `v` → `u`, `$` → `s`, `#` → `h`

### 2. Trie-Based Search

A custom trie data structure stores all profane words for efficient searching:

- O(L) search time where L is word length
- Memory efficient using dynamic dictionaries
- Supports partial matching and word completion

### 3. Recursive Matching Algorithm

The core algorithm uses three strategies to find matches:

```csharp
// Strategy 1: Direct character matching
if (node.Children.TryGetValue(currentChar, out TrieNode directChild))

// Strategy 2: Letter substitution (i↔e, o↔u, v↔u)
if (letterEquiv.TryGetValue(currentChar, out char substitutedChar))

// Strategy 3: Skip duplicate/equivalent characters
if (canSkip)  // textPos > startPos && currentChar == prevChar
```

### 4. Censoring

Found profanities are replaced with asterisks (`*`) while preserving the original text structure.

## 🎯 Usage

1. **Enter Text** - Type or paste text into the input box
2. **Scan** - Click the "Scan" button to process the text
3. **View Results:**
   - **Original Text**: Your input text
   - **Censored Text**: Profane words replaced with asterisks
   - **Detected Words**: List of identified profanities
   - **Scan History**: Previous scans with timestamps

## 🧠 Algorithm Details

### Trie Data Structure

```csharp
public class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; }
    public bool isEndOfWord { get; set; }
    public string overallWord { get; set; }
}
```

### Key Features

- **Handles Obfuscation** - Detects words with character substitutions
- **Duplicate Handling** - Matches words with repeated letters (e.g., "shiiiit")
- **Multiple Strategies** - Tries direct, substitution, and skip approaches
- **Overlap Prevention** - Correctly handles overlapping matches
- **Greedy Matching** - Finds the longest possible profanity matches

## 📊 Performance

**Time Complexity:**
- Trie construction: O(N×L) where N = word count, L = average length
- Text scanning: O(T×S) where T = text length, S = search strategies

**Space Complexity:**
- Trie storage: O(N×L)
- Recursion stack: O(L) depth

## 🔧 Customization

### Adding New Character Substitutions

Edit the dictionary in `Scanner.cs`:

```csharp
private static Dictionary<char, char> dict = new Dictionary<char, char> {
    {'@', 'a'}, {'4', 'a'}, {'8', 'b'}, {'3', 'e'},
    // Add new mappings here
    {'!', 'i'}, {'5', 's'}, {'+', 't'}
};
```

### Adding Letter Equivalents

Edit `letterEquiv` in `Trie.cs`:

```csharp
private Dictionary<char,char> letterEquiv = new Dictionary<char,char>(){
    {'i', 'e'}, {'e', 'i'}, {'o', 'u'}, {'u', 'o'}, {'v', 'u'}
    // Add new equivalents here
};
```

### Modifying the Word List

Edit `Sources/ProfaneWords.txt` with one word per line.

## 🧪 Testing

The algorithm handles various edge cases:

- **Case Insensitivity** - All text is normalized to lowercase
- **Mixed Obfuscation** - Combinations like "sh1t@ss"
- **Boundary Cases** - Empty input, very long text, special characters
- **Overlapping Words** - "assassin" contains both "ass" and "assassin"

## 📚 Learning Resources

This project demonstrates:

- **Data Structures** - Trie implementation and usage
- **Algorithms** - Recursive depth-first search, pattern matching
- **String Processing** - Text normalization and manipulation
- **Web Development** - ASP.NET Core MVC architecture
- **Clean Code** - Separation of concerns, meaningful naming



**Note:** This tool is meant for educational use and basic content filtering. For production environments, consider using established, tested profanity filtering libraries.

## ⭐ Star History

If you find this project helpful, please consider giving it a star!

---

## ❤️ Contributors

- Agustin Rhomer Siegfred S.
- Biñas John Benedict S.
- Boquiren Zyryl R.
- Gatus Mefiel Ann T.
- Pabillo Kenneth D.
- Valderama Gabriel V.
- Yap Richard David D.
