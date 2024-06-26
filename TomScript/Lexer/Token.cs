namespace TomScript.Lexer;

public class Token
{
    public enum TokenTypes
    {
        Keyword,
        Identifier,
        Text,
        Plus,
        Minus,
        Asterisk,
        Slash,
        BackSlash,
        Comma,
        Dot,
        Question,
        Underscore,
        Equal,
        Ampersand,
        Caret,
        Percent,
        Dollar,
        Hash,
        At,
        Exclamation,
        Tilde,
        BracketRoundOpen,
        BracketRoundClose,
        BracketCurlyOpen,
        BracketCurlyClose,
        BracketCornerOpen,
        BracketCornerClose,
        BracketSquareOpen,
        BracketSquareClose,
        QuoteDouble,
        QuoteSingle,
        QuoteAcute,
        Semicolon,
        Colon,
        EOL,
        EOF
    }
    
    public TokenTypes Type { get; set; }

    public Token(TokenTypes type, string stream, int startPosition, int endPosition = -1)
    {
        Type = type;
        StartPosition = startPosition;
        EndPosition = endPosition >= 0 ? endPosition : startPosition;
        Value = startPosition < stream.Length
            ? stream.Substring(startPosition, endPosition - startPosition + 1)
            : string.Empty;
    }
    
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public string Value { get; private set; }
}