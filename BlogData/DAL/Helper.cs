using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogData.DAL
{
    public static class HelperExtensions
    {
        //extension methods for string
        public static string ToTitleCase(this string str)
        {
            var txtInfo = new CultureInfo("en-US", false).TextInfo;
            return txtInfo.ToTitleCase(str);
        }
    }
}
