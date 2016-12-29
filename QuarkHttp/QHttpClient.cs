using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace QuarkHttp
{
    public static class QHttpClient
    {
        public static QHttpWrapResponse Send(this QHttpRequest request, bool fetchResponse = true, bool isAllowRedirects = false, int timeout = 100000, IWebProxy proxy = null)
        {
            // Set up the request
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(request.Url);
            webRequest.AllowAutoRedirect = isAllowRedirects;
            webRequest.ProtocolVersion = request.Version;
            webRequest.Method = request.Method.Name();
            webRequest.Accept = request.Accept;
            webRequest.Connection = request.Connection;
            webRequest.ContentType = request.ContentType;
            webRequest.Host = request.Host;
            webRequest.UserAgent = request.UserAgent;
            if (request.Referrer != null)
                webRequest.Referer = request.Referrer;
            webRequest.Timeout = timeout;
            webRequest.KeepAlive = request.KeepAlive;
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            webRequest.Proxy = proxy ?? WebRequest.DefaultWebProxy;

            foreach (string header in request.Headers)
                if (header != "accept" && header != "content-type" && header != "user-agent" && header != "referer" && header != "connection")
                    webRequest.Headers.Add(header, request.Headers[header]);

            // Write the data to the body
            if (request.Payload != null)
            {
                webRequest.ContentLength = request.Payload.Content.Length;

                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    if (!request.Payload.IsStream)
                        requestStream.Write(request.Payload.Content, 0, request.Payload.Content.Length);
                    else
                        using (Stream payloadStream = request.Payload.Stream)
                            payloadStream.CopyTo(requestStream);
                }
            }

            // Get the response
            HttpWebResponse webresponse;
            try
            {
                webresponse = webRequest.GetResponse() as HttpWebResponse;
            }
            catch (Exception exception)
            {
                webresponse = ((WebException) exception).Response as HttpWebResponse;
            }

            return new QHttpWrapResponse(webresponse, fetchResponse);
        }

        public static async Task<QHttpWrapResponse> SendAsync(this QHttpRequest request, bool fetchResponse = true, bool isAllowRedirects = false, int timeout = 100000, IWebProxy proxy = null)
        {
            // Set up the request
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(request.Url);
            webRequest.AllowAutoRedirect = isAllowRedirects;
            webRequest.ProtocolVersion = request.Version;
            webRequest.Method = request.Method.Name();
            webRequest.Accept = request.Accept;
            webRequest.Connection = request.Connection;
            webRequest.ContentType = request.ContentType;
            webRequest.Host = request.Host;
            webRequest.UserAgent = request.UserAgent;
            if (request.Referrer != null)
                webRequest.Referer = request.Referrer;
            webRequest.Timeout = timeout;
            webRequest.KeepAlive = request.KeepAlive;
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            webRequest.Proxy = proxy ?? WebRequest.DefaultWebProxy;

            foreach (string header in request.Headers)
                if (header != "accept" && header != "content-type" && header != "user-agent" && header != "referer" && header != "connection")
                    webRequest.Headers.Add(header, request.Headers[header]);

            // Write the data to the body
            if (request.Payload != null)
            {
                webRequest.ContentLength = request.Payload.Content.Length;

                using (Stream requestStream = await webRequest.GetRequestStreamAsync())
                {
                    if (!request.Payload.IsStream)
                        await requestStream.WriteAsync(request.Payload.Content, 0, request.Payload.Content.Length);
                    else
                        using (Stream payloadStream = request.Payload.Stream)
                            await payloadStream.CopyToAsync(requestStream);
                }
            }

            // Get the response
            HttpWebResponse webresponse;
            try
            {
                webresponse = await webRequest.GetResponseAsync() as HttpWebResponse;
            }
            catch (Exception exception)
            {
                webresponse = ((WebException) exception).Response as HttpWebResponse;
            }

            return new QHttpWrapResponse(webresponse, fetchResponse);
        }

        public static QHttpWrapResponse SendRetry(this QHttpRequest source, Func<QHttpResponse, bool> constraint = null, bool fetchResponse = true, bool isAllowRedirects = false, int timeout = 100000, int retries = 3, int sleep = 100, IWebProxy proxy = null)
        {
            QHttpWrapResponse response = null;

            for (int index = 0; index < retries; index++)
            {
                try
                {
                    response = source.Send(fetchResponse, isAllowRedirects, timeout, proxy);
                    
                    if (response == null)
                        throw new WebException("Error fetching response.");

                    if (constraint != null && !constraint.Invoke(response))
                        throw new HttpResponseConstraintException(response);

                    break;
                }
                catch (Exception)
                {
                    if (index == retries - 1)
                        throw;

                    Thread.Sleep(sleep);
                }
            }

            return response;
        }

        public static async Task<QHttpWrapResponse> SendRetryAsync(this QHttpRequest source, Func<QHttpResponse, bool> constraint = null, bool fetchResponse = true, bool isAllowRedirects = false, int timeout = 100000, int retries = 3, int sleep = 100, IWebProxy proxy = null)
        {
            QHttpWrapResponse response = null;

            for (int index = 0; index < retries; index++)
            {
                try
                {
                    response = await source.SendAsync(fetchResponse, isAllowRedirects, timeout, proxy);

                    if (response == null)
                        throw new WebException("Error fetching response.");

                    if (constraint != null && !constraint.Invoke(response))
                        throw new HttpResponseConstraintException(response);

                    break;
                }
                catch (Exception)
                {
                    if (index == retries - 1)
                        throw;

                    await Task.Delay(sleep);
                }
            }

            return response;
        }

        public class QHttpWrapResponse : QHttpResponse, IDisposable
        {
            private readonly HttpWebResponse _webResponse;

            public QHttpWrapResponse(HttpWebResponse webResponse, bool isFetchResponse)
            {
                if (webResponse == null)
                    throw new NullReferenceException(nameof(webResponse));

                _webResponse = webResponse;
                Headers = _webResponse.Headers;
                StatusCode = _webResponse.StatusCode;
                StatusDescription = _webResponse.StatusDescription;

                if (isFetchResponse)
                    Payload = new QPayload(webResponse.GetResponseStream(), true);
            }

            public Stream ResponseStream => _webResponse.GetResponseStream();

            public void Dispose()
            {
                _webResponse.Dispose();
            }
        }

        public class HttpResponseConstraintException : Exception
        {
            public QHttpResponse Response { get; }

            public HttpResponseConstraintException(QHttpResponse response)
            {
                Response = response;
            }
        }
    }
}
