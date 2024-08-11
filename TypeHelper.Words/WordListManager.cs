using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelper.Words;

public class WordListManager
{
    private static string ZIP_Dictionary_Path = "";

    private static string WordList_Path = "";

    private IEnumerable<Word> AllWords = new List<Word>();
    private IEnumerable<Word> AllZipCodes = new List<Word>();

    public WordListManager()
    {
        ZIP_Dictionary_Path = System.IO.Path.Combine(Environment.CurrentDirectory, "config", "lists", "zip_dict.csv");
        WordList_Path = System.IO.Path.Combine(Environment.CurrentDirectory, "config", "lists", "");
    }

    private List<Word> LoadAndSortWords(string path, string? category = null)
    {
        return ReadWordsFromCsv(path, category).OrderBy(o => o.Value).ToList();
    }

    private List<Word> CombineAndGroupWords(List<Word> zipDictionary, List<Word> wordList)
    {
        var combinedWords = zipDictionary.Concat(wordList).ToList();

        var groupedWords = zipDictionary
            .Select(o => new Word()
            {
                Value = o.Category,
                Category = o.Explanation,
                Explanation = o.Value
            })
            .GroupBy(w => w.Value)
            .Select(g => new Word
            {
                Value = g.Key,
                Category = g.First().Category,
                Explanation = string.Join(", ", g.Select(w => w.Explanation))
            })
            .ToList();

        return combinedWords.Concat(groupedWords).ToList();
    }

    public void ProcessWords()
    {
        List<Word> zipDictionary = LoadAndSortWords(ZIP_Dictionary_Path);
        List<Word> wordlists = new List<Word>();
        foreach (string filename in System.IO.Directory.GetFiles(WordList_Path))
        {
            if (filename.EndsWith(".csv"))
            {
                string category = System.IO.Path.GetFileName(filename).Replace(".csv", "");
                List<Word> wordlist = LoadAndSortWords(filename, category);
                wordlists.AddRange(wordlist);
            }
        }
        AllZipCodes = zipDictionary;
        AllWords = CombineAndGroupWords(zipDictionary, wordlists);
    }

    public IEnumerable<Word> GetZipCodes()
    {
        return AllZipCodes;
    }

    public IEnumerable<Word> GetWords()
    {
        return AllWords;
    }

    private List<Word> ReadWordsFromCsv(string filePath, string? category = null)
    {
        List<Word> words = new List<Word>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(';');

                if (category == null)
                {
                    if (values.Length != 3)
                    {
                        // Skip lines that don't have exactly 3 columns
                        continue;
                    }

                    var word = new Word
                    {
                        Value = values[0].Trim(),
                        Category = values[1].Trim(),
                        Explanation = values[2].Trim()
                    };

                    words.Add(word);
                }
                else
                {
                    if (values.Length != 2)
                    {
                        // Skip lines that don't have exactly 3 columns
                        continue;
                    }

                    var word = new Word
                    {
                        Value = values[0].Trim(),
                        Category = category,
                        Explanation = values[1].Trim()
                    };

                    words.Add(word);
                }
            }
        }

        return words;
    }
}