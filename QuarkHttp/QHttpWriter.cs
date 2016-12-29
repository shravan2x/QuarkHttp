using System;
using System.IO;
using System.Net;
using System.Web;

namespace QuarkHttp
{
    public class QHttpWriter
    {
        private readonly BufferedStream _destBase;
        private readonly StreamWriter _dest;
        private readonly bool _lowerHeaderKeys;

        public QHttpWriter(Stream destination, bool lowerHeaderKeys = false)
        {
            _destBase = new BufferedStream(destination);
            _dest = new StreamWriter(_destBase);
            _lowerHeaderKeys = lowerHeaderKeys;

            _dest.AutoFlush = true;
        }

        public Stream BaseStream => _destBase;

        public void WriteStatus(HttpStatusCode statusCode, string statusDescription = null, Version httpVersion = null)
        {
            Version finalHttpVersion = httpVersion ?? HttpVersion.Version11;
            string finalStatusDescription = statusDescription ?? HttpWorkerRequest.GetStatusDescription((int) statusCode);

            _dest.WriteLine($"HTTP/{finalHttpVersion} {(int) statusCode} {finalStatusDescription}");
        }

        public void WriteHeader(string key, string value)
        {
            string finalKey = _lowerHeaderKeys ? key.ToLower() : key;
            _dest.WriteLine($"{finalKey}: {value}");
        }

        public void WriteHeaders(WebHeaderCollection headerCollection)
        {
            foreach (string key in headerCollection)
            {
                if (key.ToLower() == "set-cookie")
                    foreach (string value in headerCollection.GetValues(key))
                        WriteHeader(key, value);
                else
                    WriteHeader(key, headerCollection[key]);
            }
        }

        public void Commit()
        {
            _dest.WriteLine();
        }

        public void WriteBody(string data)
        {
            _dest.Write(data);
        }

        public void WriteBody(byte[] buffer, int offset, int count)
        {
            _destBase.Write(buffer, offset, count);
        }

        public void Flush()
        {
            _destBase.Flush();
        }
    }
}
