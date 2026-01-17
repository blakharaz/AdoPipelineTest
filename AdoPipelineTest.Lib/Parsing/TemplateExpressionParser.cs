using System.Text;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.Parsing;

internal class TemplateExpressionParser
{
    private readonly string _text;
    private int _pos;

    internal TemplateExpressionParser(string text)
    {
        _text = text;
    }

    internal Expression ParseExpression()
    {
        SkipWhitespace();

        if (IsAtEnd())
        {
            return new StringLiteral { Value = string.Empty };
        }

        if (Peek() is '"' or '\'')
        {
            return ParseStringLiteral();
        }

        var identifier = ParseIdentifier();

        SkipWhitespace();

        if (identifier == "parameters" && Match('.'))
        {
            var name = ParseIdentifier();
            return new ParameterExpression { ParameterName = name };
        }

        if (identifier == "variables" && Match('.'))
        {
            var name = ParseIdentifier();
            return new VariableExpression { Name = name };
        }

        if (Match('('))
        {
            var parameters = new List<Expression>();

            SkipWhitespace();

            if (!Match(')'))
            {
                while (true)
                {
                    var parameter = ParseExpression();
                    parameters.Add(parameter);

                    SkipWhitespace();

                    if (Match(')'))
                    {
                        break;
                    }

                    Expect(',');
                    SkipWhitespace();
                }
            }

            return new FunctionExpression
            {
                FunctionName = identifier,
                FunctionParameters = parameters
            };
        }

        return new StringLiteral { Value = identifier };
    }

    private StringLiteral ParseStringLiteral()
    {
        var quote = Next();
        var builder = new StringBuilder();

        while (!IsAtEnd())
        {
            var ch = Next();

            if (ch == quote)
            {
                break;
            }

            if (ch == '\\' && !IsAtEnd())
            {
                var escaped = Next();
                builder.Append(escaped);
                continue;
            }

            builder.Append(ch);
        }

        return new StringLiteral { Value = builder.ToString() };
    }

    private string ParseIdentifier()
    {
        SkipWhitespace();

        var start = _pos;

        while (!IsAtEnd())
        {
            var ch = Peek();

            if (char.IsLetterOrDigit(ch) || ch == '_')
            {
                _pos++;
                continue;
            }

            break;
        }

        return _text[start.._pos];
    }

    private void SkipWhitespace()
    {
        while (!IsAtEnd() && char.IsWhiteSpace(Peek()))
        {
            _pos++;
        }
    }

    private bool Match(char expected)
    {
        if (IsAtEnd() || Peek() != expected)
        {
            return false;
        }

        _pos++;
        return true;
    }

    private void Expect(char expected)
    {
        if (!Match(expected))
        {
            throw new FormatException($"Expected '{expected}' at position {_pos}.");
        }
    }

    private char Peek() => _text[_pos];

    private char Next() => _text[_pos++];

    private bool IsAtEnd() => _pos >= _text.Length;
}
    