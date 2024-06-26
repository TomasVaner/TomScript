using TomScript.Environment;
using TomScript.Utilities;

namespace TomScript.Lexer;

public class Lexer
{
    public Lexer(string stream, ParserEnvironment environment)
    {
        Stream = stream;
        Environment = environment;
    }

    private int _position = 0;
    private int Position
    {
        get => _position;
        set
        {
            LinePosition += value - _position;
            _position = value;
        }
    }
    private int Line = 0;
    private int LinePosition = 0;
    private readonly string Stream;
    public List<Token> Tokens { get; } = new();
    
    private readonly Dictionary<char, Token.TokenTypes> SymbolTokens = new Dictionary<char, Token.TokenTypes>()
    {
        { '`', Token.TokenTypes.QuoteAcute },
        { '\'', Token.TokenTypes.QuoteSingle },
        { '"', Token.TokenTypes.QuoteDouble },
        { '~', Token.TokenTypes.Tilde },
        { '!', Token.TokenTypes.Exclamation },
        { '@', Token.TokenTypes.At },
        { '#', Token.TokenTypes.Hash },
        { '$', Token.TokenTypes.Dollar },
        { '%', Token.TokenTypes.Percent },
        { '^', Token.TokenTypes.Caret },
        { '&', Token.TokenTypes.Ampersand },
        { '*', Token.TokenTypes.Asterisk },
        { '(', Token.TokenTypes.BracketRoundOpen },
        { ')', Token.TokenTypes.BracketRoundClose },
        { '_', Token.TokenTypes.Underscore },
        { '=', Token.TokenTypes.Equal },
        { ';', Token.TokenTypes.Semicolon },
        { ':', Token.TokenTypes.Colon },
        { '<', Token.TokenTypes.BracketCornerOpen },
        { '>', Token.TokenTypes.BracketCornerClose },
        { '{', Token.TokenTypes.BracketCurlyOpen },
        { '}', Token.TokenTypes.BracketCurlyClose },
        { '[', Token.TokenTypes.BracketSquareOpen },
        { ']', Token.TokenTypes.BracketSquareClose },
        { '.', Token.TokenTypes.Dot },
        { ',', Token.TokenTypes.Comma },
        { '?', Token.TokenTypes.Question },
        { '+', Token.TokenTypes.Plus },
        { '-', Token.TokenTypes.Minus },
        { '\\', Token.TokenTypes.BackSlash },
        { '/', Token.TokenTypes.Slash },
        { '\n', Token.TokenTypes.EOL },
    };

    public void Read()
    {
        Position = 0;
        Line = 0;
        LinePosition = 0;
        Token? token;
        do
        {
            token = GetNextToken();
            if (token is null)
                return;
            switch (token.Type)
            {
                case Token.TokenTypes.QuoteDouble 
                    or Token.TokenTypes.QuoteSingle
                    or Token.TokenTypes.QuoteAcute:
                {
                    var text_token = GetStringToken(token.Type != Token.TokenTypes.QuoteSingle, token.Value[0]);
                    if (text_token != null)
                    {
                        Tokens.Add(text_token);
                    }
                    else
                        return;

                    break;
                }
                case Token.TokenTypes.Identifier when Environment.CheckKeyword(token.Value):
                    token.Type = Token.TokenTypes.Keyword; 
                    Tokens.Add(token);
                    break;
                default:
                    Tokens.Add(token);
                    break;
            }
        } while (token.Type != Token.TokenTypes.EOF);    
    }

    private Token? GetNextToken()
    {
        if (Position == Stream.Length)
            return new Token(Token.TokenTypes.EOF, Stream, Position);
        
        var next_symbol = Peek();
        while (next_symbol is ' ' or '\t' or '\r' or '\n')
        {
            ++Position;
            
            if (next_symbol is '\n')
            {
                ++Line;
            }
            if (Position == Stream.Length)
                return new Token(Token.TokenTypes.EOF, Stream, Position);
            next_symbol = Peek();
        }

        if (SymbolTokens.ContainsKey(next_symbol))
        {
            return GetToken(SymbolTokens[next_symbol]);
        }

        HashSet<char> ident_symbols = new HashSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_");
        string ident = "";
        
        if (ident_symbols.Contains(next_symbol))
        {
            int startPosition = Position;
            while (ident_symbols.Contains((next_symbol)))
            {
                ident += next_symbol;
                ++Position;
                if (Position == Stream.Length)
                    break;
                next_symbol = Peek();
            }
            return new Token(Token.TokenTypes.Identifier, Stream, startPosition, Position - 1);
        }

        _errors.Add(new ParsingError(Position, Line, LinePosition, "L002: unexpected symbol '" + next_symbol + "'"));
        return null;
    }

    private Token GetToken(Token.TokenTypes type, int position = -1, int length = 1)
    {
        if (position < 0)
            position = Position;
        var token = new Token(type, Stream, position, position + length - 1);
        Position = token.EndPosition + 1;
        return token;
    }

    private Char Peek()
    {
        return Position < Stream.Length ? Stream[Position] : '\0';
    }
    
    private string Peek(int count, int skip = 0)
    {
        return Position + count + skip < Stream.Length 
            ? Stream.Substring(Position + skip, count) 
            : string.Empty;
    }


    private Token? GetStringToken(bool escape_symbols, char end_symbol)
    {
        int end_position = Position;
        bool escape_mode = false;
        for (; end_position < Stream.Length; ++end_position)
        {
            if (Stream[end_position] == end_symbol && !escape_mode)
            {
                var token = new Token(Token.TokenTypes.Text, Stream, Position, end_position - 1);
                Position = end_position + 1;
                return token;
            }

            if (escape_symbols && Stream[end_position] == '\\')
            {
                escape_mode = !escape_mode;
                continue;
            }

            if (escape_mode)
            {
                escape_mode = false;
            }
        }
        _errors.Add(new ParsingError(end_position, Line, LinePosition, "L001: End of file in a string literal"));
        return null;
    }

    private readonly List<ParsingError> _errors = new();
    public IEnumerable<ParsingError> Errors => _errors;

    public ParserEnvironment Environment { get; }
}