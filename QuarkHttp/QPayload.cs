using System;
using System.IO;

namespace QuarkHttp
{
    public class QPayload : IDisposable
    {
        private byte[] _content;
        private Stream _stream;

        public readonly bool IsStream;

        public QPayload()
        {
            Content = new byte[0];
            IsStream = false;
        }

        public QPayload(byte[] bytes)
        {
            Content = bytes;
            IsStream = false;
        }

        public QPayload(Stream stream, bool readNow = false)
        {
            if (readNow)
            {
                _content = ReadToEnd(stream);
                IsStream = false;
                Dispose();
                return;
            }

            Stream = stream;
            IsStream = true;
        }

        public byte[] Content
        {
            get
            {
                if (_content != null)
                    return _content;

                return Content = ReadToEnd(_stream);
            }

            protected set
            {
                _content = value ?? new byte[0];
            }
        }

        public Stream Stream
        {
            get
            {
                if (_stream != null && _stream.CanRead)
                    return _stream;

                return Stream = new MemoryStream(Content);
            }

            protected set
            {
                _stream = value ?? new MemoryStream(new byte[0]);
            }
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            using (stream)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
