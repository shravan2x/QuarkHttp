using System;
using System.Collections.Specialized;
using System.Linq;

namespace QuarkHttp
{
    internal static class QCollectionUtils
    {
        public static string MakeString(this NameValueCollection paramMap, string assign, string concat, bool urlEncode = false)
        {
            return String.Join(concat, paramMap
                .Cast<string>()
                .Select(x => (!urlEncode ? $"{x}{assign}{paramMap[x]}" : $"{Uri.EscapeDataString(x)}{assign}{Uri.EscapeDataString(paramMap[x])}")));
        }

        public static NameValueCollection MakeCollection(this string paramString, string assign, string concat, bool urlDecode = false)
        {
            NameValueCollection paramMap = new NameValueCollection();

            string[] pairs = paramString.Split(new string[] { concat }, StringSplitOptions.None);
            foreach (string pair in pairs)
            {
                int assignLoc = pair.IndexOf(assign, StringComparison.Ordinal);

                if (assignLoc == -1)
                    continue;

                if (!urlDecode)
                    paramMap[pair.Substring(0, assignLoc)] = pair.Substring(assignLoc + 1);
                else
                    paramMap[Uri.UnescapeDataString(pair.Substring(0, assignLoc))] = Uri.UnescapeDataString(pair.Substring(assignLoc + 1));
            }

            return paramMap;
        }
    }
}
