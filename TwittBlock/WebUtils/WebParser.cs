using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittBlock.WebUtils
{
    public static class WebParser
    {
        public static ICollection<WebForm> Parse(string html)
        {
            List<WebForm> forms = new List<WebForm>();
            int formStartIndex = 0, formEndIndex;
            do
            {
                string htmlForm = html.Substring("<form", "</form>", out formStartIndex, out formEndIndex, formStartIndex);
                if (formStartIndex == -1)
                    break;

                WebForm form = new WebForm();
                form.Action = htmlForm.Substring("action=\"", "\"");
                form.Method = htmlForm.Substring("method=\"", "\"");

                int searchIndex = 0;
                do
                {
                    int startIndex, endIndex;
                    string input = htmlForm.Substring("<input", ">", out startIndex, out endIndex, searchIndex);
                    if (!string.IsNullOrEmpty(input))
                    {
                        form.Inputs.Add(new WebInput { Name = input.Substring("name=\"", "\""), Value = input.Substring("value=\"", "\"") });
                    }

                    searchIndex = endIndex;
                }
                while (searchIndex != -1);

                forms.Add(form);
                formStartIndex = formEndIndex;
            }
            while (formStartIndex != -1);

            return forms;
        }
    }
}
