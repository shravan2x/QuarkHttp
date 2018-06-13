using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuarkHttp
{
    public class QHttpServer
    {
        private readonly IPAddress _ip;
        private readonly TcpListener _listener;
        private readonly IQHttpRequestHandler _requestHandler;
        private readonly int _port;
        private bool _isActive;

        public QHttpServer(IQHttpRequestHandler requestHandler, int port = 80, IPAddress ip = null)
        {
            _requestHandler = requestHandler;
            _port = port;
            _ip = ip ?? IPAddress.Any;

            _listener = new TcpListener(_ip, _port);
        }

        public void Start()
        {
            Thread listenerThread = new Thread(RunStart);
            listenerThread.Name = "QHttpServer listener thread";
            listenerThread.IsBackground = false;

            listenerThread.Start();
        }

        private void RunStart()
        {
            _isActive = true;

            _listener.Start();
            while (_isActive)
            {
                while (!_listener.Pending())
                {
                    Thread.Sleep(10);

                    if (_isActive)
                        continue;

                    _listener.Stop();
                    return;
                }

                TcpClient s = _listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, _requestHandler);
                Thread thread = new Thread(processor.Process);
                thread.IsBackground = true;
                thread.Start();

                Thread.Sleep(10);
            }
        }

        public void Stop()
        {
            _isActive = false;
        }

        private class HttpProcessor
        {
            private readonly TcpClient _socket;
            private readonly QHttpRequest _request;
            private readonly IQHttpRequestHandler _requestHandler;

            private NetworkStream _stream;

            public HttpProcessor(TcpClient socket, IQHttpRequestHandler requestHandler)
            {
                _socket = socket;
                _requestHandler = requestHandler;
                
                _request = new QHttpRequest();
            }

            public void Process()
            {
                try
                {
                    _stream = _socket.GetStream();
                    _stream.ReadTimeout = 10000;

                    ParseRequestLine();
                    ReadHeaders();

                    if (_request.Method == EQHttpMethod.Post)
                        ReadPostBody();

                    _requestHandler.Handle(_request, new QHttpWriter(_stream));

                    _stream.Close();
                    _socket.Close();
                }
                catch (Exception) { }
            }

            private string ReadLine()
            {
                StringBuilder data = new StringBuilder();

                while (true)
                {
                    int nextChar = _stream.ReadByte();

                    if (nextChar == '\n')
                        break;
                    if (nextChar == '\r')
                        continue;

                    if (nextChar == -1)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    data.Append(Convert.ToChar(nextChar));
                }

                return data.ToString();
            }

            private void ParseRequestLine()
            {
                string requestLine = ReadLine();
                string[] tokens = requestLine.Split(' ');

                if (tokens.Length != 3)
                    throw new Exception("Invalid HTTP Request line");

                _request.Method = (QHttpMethod) Enum.Parse(typeof(QHttpMethod), tokens[0]);
                _request.Url = "http://example.com" + tokens[1];

                switch (tokens[2])
                {
                    case "HTTP/1.1":
                        _request.Version = HttpVersion.Version11;
                        break;

                    case "HTTP/1.0":
                        _request.Version = HttpVersion.Version10;
                        break;

                    default:
                        throw new Exception("Unrecognized HTTP Version");
                }
            }

            private void ReadHeaders()
            {
                string line;
                while ((line = ReadLine()) != "")
                {
                    string key = line.IsolateTo(":"), value = line.IsolateFrom(":");
                    if (value == null)
                        throw new Exception("Invalid HTTP Header line: " + line);

                    string headerKey = key.Trim().ToLower(), headerValue = value.Trim();
                    switch (headerKey)
                    {
                        case "accept":
                            _request.Accept = headerValue;
                            break;

                        case "content-type":
                            _request.ContentType = headerValue;
                            break;

                        case "cookie" :
                            _request.CookieString = headerValue;
                            break;

                        case "user-agent":
                            _request.UserAgent = headerValue;
                            break;

                        default :
                            _request.Headers.Add(headerKey, headerValue);
                            break;
                    }
                }
            }

            private void ReadPostBody()
            {
                if (_request.Headers["content-length"] == null)
                    return;

                int contentLen = Int32.Parse(_request.Headers["content-length"]);
                if (contentLen > 10240)
                    throw new Exception($"POST Content-Length({contentLen}) too big for this simple server");

                MemoryStream ms = new MemoryStream();

                byte[] buf = new byte[4096];
                int toRead = contentLen;
                while (toRead > 0)
                {
                    int numread = _stream.Read(buf, 0, Math.Min(4096, toRead));
                    
                    if (numread == 0)
                        throw new Exception("Client disconnected during post");

                    toRead -= numread;
                    ms.Write(buf, 0, numread);
                }
                
                _request.Payload = new QPayload(ms.ToArray());
            }
        }
    }
}
