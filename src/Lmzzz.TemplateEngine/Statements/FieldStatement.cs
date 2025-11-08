using Lmzzz.Chars.Fluent;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace Lmzzz.Template.Inner;

public class FieldStatement : IFieldStatement
{
    public IReadOnlyList<string> Names { get; }

    private readonly string key;
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<object, object>>> getters = new();
    private static readonly Func<object, object> Null = _ => null;

    public FieldStatement(IReadOnlyList<string> names)
    {
        Names = names;
        key = $"field_{string.Join(".", Names)}";
    }

    public FieldStatement(IReadOnlyList<TextSpan> names)
    {
        Names = names.Select(i => i.ToString()).ToImmutableList();
        key = $"field_{string.Join(".", Names)}";
    }

    public override string ToString()
    {
        return key;
    }

    public object? Evaluate(TemplateContext context)
    {
        if (context.Cache.TryGetValue(key, out var value)) { return value; }

        object? r;
        if (context.Data is null)
        {
            r = null;
        }
        else
        {
            object c = context.Data;
            foreach (var item in Names)
            {
                var fs = getters.GetOrAdd(item, static k => new ConcurrentDictionary<Type, Func<object, object>>());
                var f = fs.GetOrAdd(c.GetType(), static (t, i) => CreateGetter(t, i), item);
                c = f(c);
                if (c == null)
                    break;
            }
            r = c;
        }

        context.Cache[key] = r;
        return r;
    }

    private static readonly object nullo = null;
    private static readonly MethodInfo GetValue = typeof(System.Array).GetMethod("GetValue", [typeof(int)]);
    private static readonly PropertyInfo Length = typeof(System.Array).GetProperty("Length");

    private static Func<object, object> CreateGetter(Type type, string k)
    {
        if (type.IsGenericType && type.GetInterfaces().Any(x => x == typeof(System.Collections.IDictionary)))
        {
            var ts = type.GetGenericArguments();
            if (ts != null && ts.Length == 2)
            {
                var m = type.GetMethod("TryGetValue");
                if (m != null)
                {
                    if (ts[0] == typeof(string))
                    {
                        var pp = Expression.Parameter(typeof(object), "pp");
                        var c = Expression.Variable(ts[1], "c");
                        var r = Expression.Variable(typeof(object), "r");
                        var mm = Expression.IfThenElse(Expression.Call(Expression.Convert(pp, type), m, Expression.Constant(k), c), Expression.Assign(r, Expression.Convert(c, typeof(object))), Expression.Assign(r, Expression.Constant(nullo)));
                        return Expression.Lambda<Func<object, object>>(Expression.Block(new[] { c, r }, mm, r), pp).Compile();
                    }
                }
            }
        }
        else if (int.TryParse(k, out var i) && i >= 0)
        {
            if (type.BaseType == typeof(System.Array))
            {
                var pp = Expression.Parameter(typeof(object), "pp");
                var a = Expression.Parameter(typeof(Array), "a");
                var r = Expression.Variable(typeof(object), "r");
                var mm = Expression.IfThen(Expression.LessThan(Expression.Constant(i), Expression.Property(a, Length)), Expression.Assign(r, Expression.Call(a, GetValue, Expression.Constant(i))));
                return Expression.Lambda<Func<object, object>>(Expression.Block(new[] { a, r }, Expression.Assign(a, Expression.Convert(pp, typeof(System.Array))), mm, r), pp).Compile();
            }
            else if (type.IsGenericType && type.GetInterfaces().Any(x => x == typeof(System.Collections.ICollection)))
            {
                var l = type.GetProperty("Count");
                var g = type.GetMethod("get_Item");
                if (l != null && g != null)
                {
                    var pp = Expression.Parameter(typeof(object), "pp");
                    var a = Expression.Parameter(type, "a");
                    var r = Expression.Variable(typeof(object), "r");
                    var mm = Expression.IfThen(Expression.LessThan(Expression.Constant(i), Expression.Property(a, l)), Expression.Assign(r, Expression.Convert(Expression.Call(a, g, Expression.Constant(i)), typeof(object))));
                    return Expression.Lambda<Func<object, object>>(Expression.Block(new[] { a, r }, Expression.Assign(a, Expression.Convert(pp, type)), mm, r), pp).Compile();
                }
            }
        }

        var p = type.GetProperty(k);
        if (p != null)
        {
            if (p == null) return Null;

            var pg = p.GetGetMethod();
            if (pg == null) return Null;
            var pp = Expression.Parameter(typeof(object), "pp");
            var m = Expression.Convert(Expression.Call(Expression.Convert(pp, type), pg), typeof(object));
            return Expression.Lambda<Func<object, object>>(m, pp).Compile();
        }
        else
        {
            var f = type.GetField(k);
            if (f != null)
            {
                var pp = Expression.Parameter(typeof(object), "pp");
                var m = Expression.Convert(Expression.Field(Expression.Convert(pp, type), f), typeof(object));
                return Expression.Lambda<Func<object, object>>(m, pp).Compile();
            }
        }

        return Null;
    }
}