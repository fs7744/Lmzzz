using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public class HttpTemplateArrayStatement : ArrayStatement, IHttpTemplateStatement
{
    private readonly IHttpTemplateStatement[] statementList;

    public HttpTemplateArrayStatement(IHttpTemplateStatement[] statementList) : base(statementList.Select(static i => i as IStatement).ToArray())
    {
        this.statementList = statementList;
    }

    public void EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        foreach (var item in statementList)
        {
            item.EvaluateHttpTemplate(context, sb);
        }
    }
}