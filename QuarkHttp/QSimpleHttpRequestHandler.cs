using System.Net;

namespace QuarkHttp
{
    public class QSimpleHttpRequestHandler : IQHttpRequestHandler
    {
        public delegate QHttpResponse HttpRequestHandler(QHttpRequest request);
        private readonly HttpRequestHandler _requestHandler;

        public QSimpleHttpRequestHandler(HttpRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public void Handle(QHttpRequest request, QHttpWriter httpWriter)
        {
            QHttpResponse response = _requestHandler(request);

            httpWriter.WriteStatus(response.StatusCode, response.StatusDescription, HttpVersion.Version11);
            httpWriter.WriteHeaders(response.Headers);
            if (response.Headers["content-length"] == null)
                httpWriter.WriteHeader("content-length", (response.Payload?.Content?.Length ?? 0).ToString());
            httpWriter.Commit();

            if (response.Payload != null)
                httpWriter.WriteBody(response.Payload.Content, 0, response.Payload.Content.Length);

            httpWriter.Flush();
        }
    }
}
