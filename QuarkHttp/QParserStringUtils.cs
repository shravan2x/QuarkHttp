using System;

namespace QuarkHttp
{
    internal static class QParserStringUtils
    {
        public static string IsolateTo(this string data, string end)
        {
            int loc = data.IndexOf(end, StringComparison.Ordinal);
            if (loc < 0)
                return null;

            return data.Substring(0, loc).Trim();
        }

        public static string IsolateFrom(this string data, string start)
        {
            int loc = data.IndexOf(start, StringComparison.Ordinal) + start.Length;
            if (loc < 0)
                return null;

            return data.Substring(loc).Trim();
        }
    }
}
