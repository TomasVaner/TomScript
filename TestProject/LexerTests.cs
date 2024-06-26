using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TomScript.Environment;
using TomScript.Lexer;

namespace TestProject;

//[TestFixture(typeof(string), typeof(IEnumerable<Token.TokenTypes>), typeof(IEnumerable<string>))]
[TestFixture]
public class ParsingTest
{
    public struct TokenTestCase
    {
        public string Code;
        public List<string> Keywords;
        public List<Token.TokenTypes> Tokens;

        public TokenTestCase(string code, List<string> keywords, List<Token.TokenTypes> tokens)
        {
            Code = code;
            Keywords = keywords;
            Tokens = tokens;
        }
    }
    
    private static IEnumerable<TokenTestCase> TestVectors()
    {
        yield return new TokenTestCase(
            "",
            new List<string>(),
            new List<Token.TokenTypes> { });
        yield return new TokenTestCase(
            "\"string\"",
            new List<string>(),
            new List<Token.TokenTypes> { Token.TokenTypes.Text });
        yield return new TokenTestCase(
            "'string'",
            new List<string>(),
            new List<Token.TokenTypes> { Token.TokenTypes.Text });
        yield return new TokenTestCase(
            "'st\"ring'",
            new List<string>(),
            new List<Token.TokenTypes> { Token.TokenTypes.Text });
        yield return new TokenTestCase(
            "\"'string''''''\"",
            new List<string>(),
            new List<Token.TokenTypes> { Token.TokenTypes.Text });
        yield return new TokenTestCase(
            "if(true) { do_stuff; }",
            new List<string>{"if", "true"},
            new List<Token.TokenTypes>
            {
                Token.TokenTypes.Keyword, Token.TokenTypes.BracketRoundOpen, Token.TokenTypes.Keyword, Token.TokenTypes.BracketRoundClose,
                Token.TokenTypes.BracketCurlyOpen, Token.TokenTypes.Identifier, Token.TokenTypes.Semicolon,
                Token.TokenTypes.BracketCurlyClose
            });
        
    }

    [TestCaseSource(nameof(TestVectors))]
    public void Test(TokenTestCase test_data)
    {
        ParserEnvironment env= new ();
        env.AddKeywords(test_data.Keywords);
        Lexer lexer = new(test_data.Code, env);
        lexer.Read();
        test_data.Tokens.Add(Token.TokenTypes.EOF);
        Assert.AreEqual(test_data.Tokens.Count, lexer.Tokens.Count);
        
        foreach (var tokens in lexer.Tokens.Zip(test_data.Tokens))
        {
            Assert.AreEqual(tokens.Second, tokens.First.Type);
        }
    }
}