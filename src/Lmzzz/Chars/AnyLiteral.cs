using Lmzzz.Chars.Fluent;
using System.Buffers;

namespace Lmzzz.Chars;

public class AnyLiteral : Parser<TextSpan>
{
    private readonly SearchValues<char> end;
    private readonly bool mustHasEnd;
    private readonly bool canEmpty;
    private readonly char? escape;

    public AnyLiteral(string end, bool mustHasEnd, bool canEmpty, char? escape)
    {
        this.mustHasEnd = mustHasEnd;
        this.canEmpty = canEmpty;
        this.escape = escape;
        if (escape.HasValue)
        {
            end = end + escape.Value;
        }

        this.end = SearchValues.Create(end);
    }

    public override bool Parse(CharParseContext context, ref ParseResult<TextSpan> result)
    {
        context.EnterParser(this);

        var cursor = context.Cursor;
        if (!cursor.Eof)
        {
            if (escape.HasValue)
            {
                var sb = new System.Text.StringBuilder();
                var span = cursor.Span;
                int i = 0;
                var j = 0;
                do
                {
                    var s = span.Slice(j);
                    i = s.IndexOfAny(end);
                    if (i >= 0)
                    {
                        if (s[i] == escape)
                        {
                            if (i + 1 >= s.Length)
                            {
                                context.ExitParser(this);
                                return false;
                            }
                            else if (s[i + 1] == escape)
                            {
                                sb.Append(s.Slice(0, i + 1));
                            }
                            else if (end.Contains(s[i + 1]))
                            {
                                sb.Append(s.Slice(0, i));
                                sb.Append(s[i + 1]);
                            }
                            else
                            {
                                context.ExitParser(this);
                                return false;
                            }

                            j += i + 2;
                        }
                        else
                        {
                            sb.Append(s.Slice(0, i));
                            i += j;
                            break;
                        }
                    }
                    else
                    {
                        i += j;
                        break;
                    }
                    //if (i > 0 && s[i - 1] == escape)
                    //{
                    //    sb.Append(s.Slice(0, i - 1));
                    //    sb.Append(s[i]);
                    //    j += i + 1;
                    //}
                    //else
                    //{
                    //    sb.Append(s.Slice(0, i));
                    //    i += j;
                    //    break;
                    //}
                } while (true);
                if (i == 0 && !canEmpty)
                {
                    context.ExitParser(this);
                    return false;
                }
                if (i >= 0)
                {
                    var start = cursor.Offset;
                    cursor.Advance(i);
                    result.Set(start, cursor.Offset, new TextSpan(sb.ToString(), 0, sb.Length));
                    context.ExitParser(this);
                    return true;
                }

                if (!mustHasEnd)
                {
                    span = cursor.Span;
                    var start = cursor.Offset;
                    cursor.Advance(span.Length);
                    result.Set(start, cursor.Offset, new TextSpan(cursor.Buffer, 0, span.Length));
                    context.ExitParser(this);
                    return true;
                }
            }
            else
            {
                var span = cursor.Span;
                var i = span.IndexOfAny(end);
                if (i == 0 && !canEmpty)
                {
                    context.ExitParser(this);
                    return false;
                }
                if (i >= 0)
                {
                    var start = cursor.Offset;
                    cursor.Advance(i);
                    result.Set(start, cursor.Offset, new TextSpan(cursor.Buffer, start, i));
                    context.ExitParser(this);
                    return true;
                }

                if (!mustHasEnd)
                {
                    var start = cursor.Offset;
                    cursor.Advance(span.Length);
                    result.Set(start, cursor.Offset, new TextSpan(cursor.Buffer, start, span.Length));
                    context.ExitParser(this);
                    return true;
                }
            }
        }
        context.ExitParser(this);
        return false;
    }
}