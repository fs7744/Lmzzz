using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.Template.Inner;

public class TemplateEngineParser
{
    public static readonly Parser<IConditionStatement> ConditionParser;
    public static readonly Parser<IStatement> NullValue = IgnoreSeparator(Text("null")).Then<IStatement>(static s => TemplateEngine.Optimize(NullValueStatement.Value)).Name(nameof(NullValue));

    public static readonly Parser<IConditionStatement> BoolValue = IgnoreSeparator(Text("true", true)).Then<IConditionStatement>(static s => TemplateEngine.Optimize(BoolValueStatement.True))
            .Or(IgnoreSeparator(Text("false", true)).Then<IConditionStatement>(static s => TemplateEngine.Optimize(BoolValueStatement.False))).Name(nameof(BoolValue));

    public static readonly Parser<IStatement> StringValue = IgnoreSeparator(String('\'').Or(String())).Then<IStatement>(static s => TemplateEngine.Optimize(new StringValueStatement(s))).Name(nameof(StringValue));
    public static readonly Parser<IStatement> NumberValue = IgnoreSeparator(Decimal()).Then<IStatement>(static s => TemplateEngine.Optimize(new DecimalValueStatement(s))).Name(nameof(NumberValue));
    private static readonly Parser<TextSpan> FieldName = Identifier(Character.SVIdentifierPart, Character.SVIdentifierPart).Or(Between(Char('['), IgnoreSeparator(String('\'').Or(String())), IgnoreSeparator(Char(']'))));
    public static readonly Parser<IStatement> Field = IgnoreSeparator(Separated(Char('.'), FieldName)).Then<IStatement>(static s => TemplateEngine.Optimize(new FieldStatement(s))).Name(nameof(Field));

    public static readonly Parser<char> ParenOpen = IgnoreSeparator(Char('(')).Name(nameof(ParenOpen));
    public static readonly Parser<char> ParenClose = IgnoreSeparator(Char(')')).Name(nameof(ParenClose));
    public static readonly Deferred<IStatement> FunctionExpr = Deferred<IStatement>(nameof(FunctionExpr));
    public static readonly Parser<IStatement> AnyValue = NullValue.Or(BoolValue.Then(static x => x as IStatement)).Or(StringValue).Or(IgnoreSeparator(FunctionExpr)).Or(NumberValue).Or(Field).Name(nameof(AnyValue));

    public static readonly Parser<IConditionStatement> OP = AnyValue
        .And(IgnoreSeparator(Text("==")).Or(IgnoreSeparator(Text("!="))).Or(IgnoreSeparator(Text(">="))).Or(IgnoreSeparator(Text(">"))).Or(IgnoreSeparator(Text("<="))).Or(IgnoreSeparator(Text("<"))))
        .And(AnyValue)
        .Then<IConditionStatement>(static s => TemplateEngine.Optimize(StatementUtils.Create(s.Item2, s.Item1, s.Item3))).Name(nameof(OP));

    public static readonly Deferred<IConditionStatement> Condition = Deferred<IConditionStatement>(nameof(Condition));

    public static readonly Parser<IConditionStatement> GroupExpression = Between(ParenOpen, Condition, ParenClose).Name(nameof(GroupExpression));
    public static readonly Parser<IConditionStatement> NotExpression = Between(IgnoreSeparator(Text("!")).And(ParenOpen), Condition, ParenClose).Then(static s => TemplateEngine.Optimize(StatementUtils.Create("!", s))).Name(nameof(NotExpression));

    public static readonly Parser<IConditionStatement> ConditionValue = OP.Or(IgnoreSeparator(FunctionExpr).Then(static x => x as IConditionStatement)).Or(BoolValue).Name(nameof(ConditionValue));

    public static readonly Parser<IConditionStatement> Primary = NotExpression.Or(GroupExpression).Or(ConditionValue).Name(nameof(Primary));

    public static readonly Parser<IConditionStatement> Conditions = Primary.LeftAssociative(
               (IgnoreSeparator(Text("&&")), static (x, y) => TemplateEngine.Optimize(StatementUtils.Create("&&", x, y))),
               (IgnoreSeparator(Text("and", true)), static (x, y) => TemplateEngine.Optimize(StatementUtils.Create("&&", x, y))),
               (IgnoreSeparator(Text("||")), static (x, y) => TemplateEngine.Optimize(StatementUtils.Create("||", x, y))),
               (IgnoreSeparator(Text("or", true)), static (x, y) => TemplateEngine.Optimize(StatementUtils.Create("||", x, y)))
           ).Name(nameof(Conditions));

    public static readonly Parser<string> SignBegin = IgnoreSeparator(Text("{{"));
    public static readonly Parser<string> SignEnd = IgnoreSeparator(Text("}}"));

    private static readonly Parser<string> _endif = IgnoreSeparator(Text("endif", true));
    private static readonly Parser<string> _if = IgnoreSeparator(Text("if", true));
    private static readonly Parser<string> _elseif = IgnoreSeparator(Text("elseif", true));
    private static readonly Parser<string> _else = IgnoreSeparator(Text("else", true));
    private static readonly Parser<string> _for = IgnoreSeparator(Text("for", true));
    private static readonly Parser<string> _endfor = IgnoreSeparator(Text("endfor", true));

    public static readonly Parser<IStatement> ReplaceStr = Between(Text("{{").AndIf(static context =>
    {
        var s = context.Cursor.Position;
        var r = new ParseResult<string>();
        var rr = true;
        if (_endif.Parse(context, ref r) || _if.Parse(context, ref r) || _elseif.Parse(context, ref r) || _else.Parse(context, ref r) || _for.Parse(context, ref r) || _endfor.Parse(context, ref r))
        {
            context.Cursor.Reset(s);
            rr = false;
        }
        return rr;
    }), AnyValue, SignEnd).Then<IStatement>(static x => TemplateEngine.Optimize(new ReplaceStatement(x)));

    public static readonly Parser<IStatement> OriginStr = AnyBeforeEnd("{{", canEmpty: false, escape: '\\').Then<IStatement>(static x => TemplateEngine.Optimize(new OriginStrStatement(x.Span.ToString())));

    public static readonly Deferred<IStatement> TemplateValue = Deferred<IStatement>(nameof(TemplateValue));

    public static readonly Parser<IStatement> Template = TemplateValue.Eof().Name(nameof(Template));

    public static readonly Parser<IStatement> If = SignBegin.And(_if).And(ParenOpen).And(Conditions).And(ParenClose).And(SignEnd)
        .And(TemplateValue)
        .And(ZeroOrMany(SignBegin.And(_elseif).And(ParenOpen).And(Conditions).And(ParenClose).And(SignEnd).And(TemplateValue)))
        .And(Optional(SignBegin.And(_else).And(SignEnd).And(TemplateValue)))
        .And(SignBegin.And(_endif).And(SignEnd))
        .Then<IStatement>(static x => TemplateEngine.Optimize(new IfStatement()
        {
            If = new IfConditionStatement(x.Item1.Item4, x.Item1.Item7),
            ElseIfs = x.Item2 == null || x.Item2.Count == 0 ? null : x.Item2.Select(static y => new IfConditionStatement(y.Item4, y.Item7)),
            Else = x.Item3.Item2 != null ? x.Item3.Item4 : null,
        }))
        .Name(nameof(If));

    public static readonly Parser<IStatement> For = SignBegin.And(_for).And(ParenOpen).And(IgnoreSeparator(FieldName).And(IgnoreSeparator(Char(','))).And(IgnoreSeparator(FieldName)).And(IgnoreSeparator(Text("in", true)))).And(IgnoreSeparator(FunctionExpr).Or(Field)).And(ParenClose).And(SignEnd)
        .And(TemplateValue)
        .And(SignBegin).And(_endfor).And(SignEnd)
        .Then<IStatement>(static x => TemplateEngine.Optimize(new ForStatement() { ItemName = $"field_{x.Item1.Item4.Item1.Span.ToString()}", IndexName = $"field_{x.Item1.Item4.Item3.Span.ToString()}", Value = x.Item1.Item5, Statement = x.Item2 })).Name(nameof(For));

    static TemplateEngineParser()

    {
        FunctionExpr.Parser = Identifier(Character.SVIdentifierPart, Character.SVIdentifierPart).And(ParenOpen)
            .And(Optional(AnyValue.And(ZeroOrMany(IgnoreSeparator(Char(',')).And(AnyValue))))).And(ParenClose).Then<IStatement>(static x =>
            {
                var args = new List<IStatement>();
                if (x.Item3.Item1 != null)
                {
                    args.Add(x.Item3.Item1);
                }
                if (x.Item3.Item2 != null)
                {
                    args.AddRange(x.Item3.Item2.Select(y => y.Item2));
                }
                var func = new FunctionStatement()
                {
                    Name = x.Item1.Span.ToString(),
                    Arguments = args.Count == 0 ? Array.Empty<IStatement>() : args.ToArray()
                };

                return TemplateEngine.Optimize<IStatement>(func);
            });
        ConditionParser = Condition.Parser = Conditions;
        TemplateValue.Parser = ZeroOrMany(For.Or(If).Or(ReplaceStr).Or(OriginStr)).Then<IStatement>(static x =>
        {
            if (x == null || x.Count == 0) return null;
            var c = x[0];
            var list = new List<IStatement>() { c };
            for (var i = 1; i < x.Count; i++)
            {
                var n = x[i];
                if (c is OriginStrStatement o && n is OriginStrStatement no)
                {
                    o.Text += no.Text;
                    continue;
                }
                list.Add(n);
                c = n;
            }
            return TemplateEngine.Optimize(list.Count == 1 ? list.First() : new ArrayStatement(list.ToArray()));
        });
        ConditionParser = ConditionParser.Eof();
    }
}