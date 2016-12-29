using System;
using System.Net;
using System.Text;

namespace QuarkHttp
{
    public static class QCookieJar
    {
        public static CookieCollection MapCookies(string cookieString)
        {
            CookieCollection cookies = new CookieCollection();
            string[] cookieArray = cookieString.Split(';');

            foreach (string curCookie in cookieArray)
            {
                string[] cookieParts = curCookie.Trim().Split('=');
                cookies.Add(new Cookie(cookieParts[0], cookieParts[1]));
            }

            if (cookieString.Length > 0 && cookies.Count == 0)
                throw new Exception("Format incorrect");

            return cookies;
        }

        public static string ConcatCookies(CookieCollection cookies)
        {
            StringBuilder cookieStringBuilder = new StringBuilder();

            for (int index = 0; index < cookies.Count; index++)
            {
                if (cookies[index].Expired)
                    continue;

                string curCookieString = String.Format("{0}={1}", cookies[index].Name, cookies[index].Value);
                cookieStringBuilder.Append(curCookieString);

                if (index < cookies.Count - 1)
                    cookieStringBuilder.Append("; ");
            }

            return cookieStringBuilder.ToString();
        }
    }
}
