using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.Helpers
{
    public static class StringHelper
    {
        public static string Cut(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                text = string.Empty;
            else if (text.Length > maxLength)
                text = text.Substring(0, maxLength);

            return text;
        }
    }
}
