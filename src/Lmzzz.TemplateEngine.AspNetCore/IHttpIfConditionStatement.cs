using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public interface IHttpIfConditionStatement
{
    bool EvaluateHttpTemplate(HttpContext context, StringBuilder sb);
}