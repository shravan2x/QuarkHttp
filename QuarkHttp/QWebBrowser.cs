using System;
using System.Net;

namespace QuarkHttp
{
    public class QWebBrowser
    {
        public CookieCollection Cookies { get; }
        public IWebProxy Proxy { get; set; }
        public string UserAgent { get; set; }

        public QWebBrowser()
        {
            Cookies = new CookieCollection();
            Proxy = WebRequest.DefaultWebProxy;
            UserAgent = QUserAgent.Chrome55Windows10;
        }

        public QHttpResponse Execute(QHttpRequest request, bool fetchResponse = true, bool isAllowRedirects = false, int timeout = 100000)
        {
            request.Cookies = Cookies;
            request.UserAgent = UserAgent;

            QHttpResponse response = request.Send(fetchResponse, isAllowRedirects, timeout, Proxy);

            Cookies.Add(response.Cookies);
            return response;
        }

        public QHttpResponse ExecuteRetry(QHttpRequest request, Func<QHttpResponse, bool> constraint = null, bool fetchResponse = true, bool isAllowRedirects = false, int timeout = 100000, int retries = 3, int sleep = 100)
        {
            request.Cookies = Cookies;
            request.UserAgent = UserAgent;

            QHttpResponse response = request.SendRetry(constraint, fetchResponse, isAllowRedirects, timeout, retries, sleep, Proxy);

            Cookies.Add(response.Cookies);
            return response;
        }
    }
}
