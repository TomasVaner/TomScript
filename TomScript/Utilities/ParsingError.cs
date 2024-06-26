using Microsoft.VisualBasic.FileIO;

namespace TomScript.Utilities;

public class ParsingError
{
    public int Position { get; private set; }
    public int Line { get; private set; }
    public int PositionInLine { get; private set; }
    public string Text { get; private set; }

    public ParsingError(int position, int line, int pos, string text)
    {
        Position = position;
        Line = line;
        PositionInLine = pos;
        Text = text;
    }
}