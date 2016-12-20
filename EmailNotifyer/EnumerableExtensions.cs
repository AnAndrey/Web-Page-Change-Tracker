using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.EmailNotifier
{
    internal static class EnumerableExtensions
    {
        internal static string ToHtmlTableString<T>(this IEnumerable<T> list)
        {
            var result = new StringBuilder();
            result.Append("<table style  > ");

            result.AppendFormat("<th >{0}</th >", "Description");

            foreach (var item in list)
            {
                result.AppendFormat("<tr >");

                result.AppendFormat("<td >{0}</td >", item.ToString());

                result.AppendLine("</tr >");
            }

            result.Append("</table >");
            return result.ToString();
        }
    }
}
