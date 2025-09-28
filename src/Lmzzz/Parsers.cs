using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lmzzz;

public static class Parsers
{
    public static TermParser Ingore(params char[] chars) => new(chars);
}

public class TermParser : Parser<Term>
{
    private readonly HashSet<char> _chars;

    public TermParser(params char[] chars)
    {
        _chars = new HashSet<char>(chars);
    }

    public override Term Parse(ParseContext context)
    {
        var c = context.Current;
        if (c.HasValue && _chars.Contains(c.Value))
        {
            context.Advance();
            return new Term(c.Value.ToString());
        }
        return null;
    }
}