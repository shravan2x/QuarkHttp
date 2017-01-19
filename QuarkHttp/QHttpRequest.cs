using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;

namespace QuarkHttp
{
    public class QHttpRequest
    {
        private const string DefaultUserAgent = QUserAgent.Chrome55Windows10, DefaultAccept = QAccept.Html, DefaultContentType = QContentType.WwwFormUrlEncoded;

        private UriBuilder _url;
        private NameValueCollection _queryParams;
        private WebHeaderCollection _extraHeaders;
        private Version _version;

        public EQHttpMethod Method { get; set; }
        public QPayload Payload { get; set; }
        public bool KeepAlive { get; set; }
        public string Referrer { get; set; }

        private bool _isUrlLastUsed;

        public QHttpRequest()
        {
            UrlBuilder = new UriBuilder();

            Method = EQHttpMethod.Get;
            Headers = new WebHeaderCollection();

            Accept = DefaultAccept;
            ContentType = DefaultContentType;
            UserAgent = DefaultUserAgent;
            Version = HttpVersion.Version11;
            Referrer = null;
            KeepAlive = true;
        }

        public QHttpRequest(string url) : this()
        {
            Url = url;
        }

        public string Url
        {
            get { return UrlBuilder.Uri.ToString(); }

            set
            {
                if (value == null)
                    throw new ArgumentException("Url cannot be null");

                _isUrlLastUsed = true;
                _url = new UriBuilder(value);
            }
        }

        public UriBuilder UrlBuilder
        {
            get
            {
                if (!_isUrlLastUsed)
                    _url.Query = _queryParams.MakeString("=", "&");

                _isUrlLastUsed = true;
                return _url;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Url builder cannot be null");

                _isUrlLastUsed = true;
                _url = value;
            }
        }

        public string Host
        {
            get { return _url.Host; }

            set
            {
                if (value == null)
                    throw new ArgumentException("Host cannot be null");

                _url.Host = value;
            }
        }

        public string Path
        {
            get { return _url.Path; }

            set { _url.Path = value; }
        }

        public string Accept
        {
            get { return Headers["accept"]; }

            set
            {
                if (value == null)
                    throw new ArgumentException("Accept cannot be null");

                Headers["accept"] = value;
            }
        }

        public string Connection
        {
            get { return Headers["connection"]; }

            set
            {
                if (value == null)
                    throw new ArgumentException("Connection cannot be null");

                Headers["connection"] = value;
            }
        }

        public string ContentType
        {
            get { return Headers["content-type"]; }

            set
            {
                if (value == null)
                    throw new ArgumentException("Content type cannot be null");

                Headers["content-type"] = value;
            }
        }

        public string UserAgent
        {
            get { return Headers["user-agent"]; }

            set
            {
                if (value == null)
                    throw new ArgumentException("User agent cannot be null");

                Headers["user-agent"] = value;
            }
        }

        public CookieCollection Cookies
        {
            get { return QCookieJar.MapCookies(CookieString); }

            set { CookieString = QCookieJar.ConcatCookies(value ?? new CookieCollection()); }
        }

        public string CookieString
        {
            get { return Headers["cookie"]; }

            set { Headers["cookie"] = value ?? ""; }
        }

        public WebHeaderCollection Headers
        {
            get { return _extraHeaders; }

            set { _extraHeaders = value ?? new WebHeaderCollection(); }
        }

        public bool IsAjax
        {
            get { return Headers["X-Request"] != null && Headers["X-Requested-With"] != null && Headers["X-Prototype-Version"] != null; }

            set
            {
                if (!value)
                {
                    Headers.Remove("X-Request");
                    Headers.Remove("X-Requested-With");
                    Headers.Remove("X-Prototype-Version");
                }

                Headers["X-Request"] = "JSON";
                Headers["X-Requested-With"] = "XMLHttpRequest";
                Headers["X-Prototype-Version"] = "1.7";
            }
        }

        public byte[] DataBytes
        {
            get { return Payload.Content; }

            set { Payload = new QPayload(value); }
        }

        public string DataString
        {
            set { Payload = new QStringPayload(value); }
        }

        public Image DataImage
        {
            set { Payload = new QImagePayload(value); }
        }

        public Stream DataStream
        {
            set { Payload = new QPayload(value); }
        }

        public NameValueCollection DataParams
        {
            set
            {
                if (value == null)
                    Payload = new QPayload();

                Payload = new QStringPayload(value.MakeString("=", "&", true));
            }
        }

        public string QueryString
        {
            get { return UrlBuilder.Query.Length > 1 ? UrlBuilder.Query.Substring(1) : ""; }

            set
            {
                _isUrlLastUsed = true;
                _url.Query = value ?? "";
            }
        }

        public NameValueCollection QueryParams
        {
            get
            {
                if (_isUrlLastUsed)
                    _queryParams = _url.Query.Length > 1 ? _url.Query.Substring(1).MakeCollection("=", "&") : new NameValueCollection();

                _isUrlLastUsed = false;
                return _queryParams;
            }

            set
            {
                _isUrlLastUsed = false;
                _queryParams = value ?? new NameValueCollection();
            }
        }

        public QMultipartFormData MultipartParams
        {
            set
            {
                ContentType = value.ContentType;
                Payload = new QPayload(value.GetBytes());
            }
        }

        public void AddQueryString(string queryString)
        {
            if (queryString == null)
                return;

            if (_url.Query.Length > 1)
                _url.Query = _url.Query.Substring(1) + "&" + queryString;
            else
                _url.Query = queryString;
        }

        public Version Version
        {
            get { return _version; }

            set
            {
                if (value == null)
                    throw new ArgumentException("Version cannot be null");

                _version = value;
            }
        }
    }
}
