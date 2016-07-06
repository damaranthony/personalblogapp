using System.Globalization;

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
