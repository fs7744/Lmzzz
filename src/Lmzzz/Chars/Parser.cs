namespace Lmzzz.Chars;

public abstract class Parser<T>
{
    public abstract bool Parse(CharParseContext context, ref ParseResult<T> result);

    public bool TryParse(CharParseContext context, out T value, out ParseException? error)
    {
        error = null;

        try
        {
            var localResult = new ParseResult<T>();

            var success = Parse(context, ref localResult);

            if (success)
            {
                value = localResult.Value;
                return true;
            }
        }
        catch (ParseException e)
        {
            error = e;
        }

        value = default!;
        return false;
    }
}