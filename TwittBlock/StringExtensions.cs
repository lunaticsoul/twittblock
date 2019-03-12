using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittBlock
{
    public static class StringExtensions
    {
        public static string Substring(this string self, string start, string end, out int startIndex, out int endIndex, int fromIndex = 0)
        {
            startIndex = -1;
            endIndex = -1;

            startIndex = self.IndexOf(start, fromIndex);
            if (startIndex == -1)
                return "";

            startIndex += start.Length;

            endIndex = self.IndexOf(end, startIndex);
            if (endIndex == -1)
                return "";

            return self.Substring(startIndex, endIndex - startIndex);
        }

        public static string Substring(this string self, string start, string end, int fromIndex = 0)
        {
            int startIndex;
            int endIndex;
            return self.Substring(start, end, out startIndex, out endIndex, fromIndex);
        }

        public static string CleanseTwitterMessage(this string message)
        {
            int firstEndTag = message.IndexOf(">") + 1;
            int endMessage = message.IndexOf("</p>", firstEndTag);
            if (endMessage > firstEndTag)
            {
                string innerMessage = message.Substring(firstEndTag, endMessage - firstEndTag);
                innerMessage = innerMessage.Replace("<strong>", "").Replace("</strong>", "");
                innerMessage = innerMessage.CutAll("<a ", "</a>");
                innerMessage = innerMessage.CutAll("<img ", ">");

                return innerMessage;
            }

            return message;
        }

        public static string Cut(this string self, string start, string end, out int endIndex, int fromIndex = 0)
        {
            int startIndex = self.IndexOf(start, fromIndex);
            endIndex = startIndex;
            if (startIndex != -1)
            {
                endIndex = self.IndexOf(end, startIndex);
                if (endIndex != -1)
                {
                    endIndex += end.Length;
                    return self.Substring(0, startIndex) + self.Substring(endIndex, self.Length - endIndex);
                }
            }

            return self;
        }

        public static string CutAll(this string self, string start, string end, int fromIndex = 0)
        {
            int endIndex = 0;
            string cleanseMessage = self;
            do
            {
                cleanseMessage = cleanseMessage.Cut(start, end, out endIndex);
            }
            while (endIndex != -1);

            return cleanseMessage;
        }
    }
}
