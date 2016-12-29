using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace QuarkHttp
{
    internal static class QCollectionUtils
    {
        public static string MakeString(this NameValueCollection paramMap, string assign, string concat, bool urlEncode = false)
        {
            StringBuilder paramStringBuilder = new StringBuilder();

            for (int index = 0; index < paramMap.Count; index++)
            {
                string[] paramValues = paramMap.GetValues(index) ?? new string[1];

                paramStringBuilder.Append(!urlEncode ? $"{paramMap.GetKey(index)}{assign}{paramValues[0]}" : $"{HttpUtility.UrlEncode(paramMap.GetKey(index))}{assign}{HttpUtility.UrlEncode(paramValues[0])}");

                if (index < paramMap.Count - 1)
                    paramStringBuilder.Append(concat);
            }

            return paramStringBuilder.ToString();
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
                    paramMap[HttpUtility.UrlDecode(pair.Substring(0, assignLoc))] = HttpUtility.UrlDecode(pair.Substring(assignLoc + 1));
            }

            return paramMap;
        }
    }
}
