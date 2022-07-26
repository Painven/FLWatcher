using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlWatcherDesktop.Models;

public interface IPhraseStorage
{
    string[] GetAll();
    void AddPhrase(string phrase);
    void RemovePhrase(string phrase);
}

public class FilePhraseStorage : IPhraseStorage
{
    public string FileName { get; }

    public FilePhraseStorage(string fileName)
    {
        FileName = fileName;
    }

    public string[] GetAll()
    {
        if (File.Exists(FileName))
        {
            return File.ReadAllLines(FileName);
        }
        return Enumerable.Empty<string>().ToArray();
    }

    public void AddPhrase(string phrase)
    {
        File.AppendAllLines(FileName, new string[] { phrase });
    }

    public void RemovePhrase(string phrase)
    {
        var phrases = GetAll();

        if (phrases.Contains(phrase))
        {
            var list = phrases.ToList();
            list.RemoveAll(i => i.Equals(phrase));

            File.WriteAllLines(FileName, list);
        }
    }
}
