using System;
using System.Collections.Generic;
using System.Net;

namespace QuarkHttp
{
    public class QHttpResponse
    {
        public QPayload Payload { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public QHttpResponse()
        {
            Headers = new WebHeaderCollection();
        }

        public CookieCollection Cookies => GetAllCookiesFromHeader(Headers["set-cookie"], "");

        private static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
        {
            CookieCollection cc = new CookieCollection();

            if (String.IsNullOrEmpty(strHeader))
                return cc;

            List<string> al = ConvertCookieHeaderToArrayList(strHeader);
            cc = ConvertCookieArraysToCookieCollection(al, strHost);
            return cc;
        }

        private static List<string> ConvertCookieHeaderToArrayList(string strCookHeader)
        {
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            string[] strCookTemp = strCookHeader.Split(',');
            List<string> al = new List<string>();
            int i = 0;
            int n = strCookTemp.Length;
            while (i < n)
            {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                }
                else
                {
                    al.Add(strCookTemp[i]);
                }
                i = i + 1;
            }
            return al;
        }

        private static CookieCollection ConvertCookieArraysToCookieCollection(List<string> al, string strHost)
        {
            CookieCollection cc = new CookieCollection();

            int alcount = al.Count;
            for (int i = 0; i < alcount; i++)
            {
                string strEachCook = al[i];
                string[] strEachCookParts = strEachCook.Split(';');
                int intEachCookPartsCount = strEachCookParts.Length;
                Cookie cookTemp = new Cookie();

                for (int j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        string strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            int firstEqual = strCNameAndCValue.IndexOf("=", StringComparison.Ordinal);
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    string[] nameValuePairTemp;
                    string strPNameAndPValue;
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            nameValuePairTemp = strPNameAndPValue.Split('=');
                            cookTemp.Path = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : "/";
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];

                        if (strPNameAndPValue == string.Empty)
                            continue;

                        nameValuePairTemp = strPNameAndPValue.Split('=');
                        cookTemp.Domain = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : strHost;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }
                cc.Add(cookTemp);
            }
            return cc;
        }
    }
}
