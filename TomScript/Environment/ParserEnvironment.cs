namespace TomScript.Environment;

public class ParserEnvironment
{
    private HashSet<string> _keywords = new();
    public IEnumerable<string> Keywords => _keywords;
    public void AddKeywords(IEnumerable<string> keywords)
    {
        foreach (var keyword in keywords)
        {
            if (!_keywords.Contains(keyword))
                _keywords.Add(keyword);
        }
    }

    public bool CheckKeyword(string word)
    {
        return _keywords.Contains(word);
    }
}