using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public interface IHttpTemplateStatement : IStatement
{
    void EvaluateHttpTemplate(HttpContext context, StringBuilder sb);
}