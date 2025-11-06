namespace Lmzzz.Chars.Fluent;

public delegate bool ParseDelegate<T>(CharParseContext context, ref ParseResult<T> result);

public abstract class Parser<T>
{
    public string Name { get; set; }

    public abstract bool Parse(CharParseContext context, ref ParseResult<T> result);

    public abstract ParseDelegate<T> GetDelegate();

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

    #region Then

    public Parser<U> Then<U>(Func<T, U> conversion) => new Then<T, U>(this, conversion);

    #endregion Then

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}