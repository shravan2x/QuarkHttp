using System;
using System.IO;
using System.Text;

namespace QuarkHttp
{
    public class QHttpChunkedStream : Stream
    {
        private const int DefaultChunkSize = 512;
        private static readonly byte[] CrlfBytes = Encoding.ASCII.GetBytes("\r\n");

        private readonly QHttpWriter _httpWriter;
        private readonly byte[] _chunkBuffer;
        private int _chunkBufferPos;

        public QHttpChunkedStream(QHttpWriter httpWriter, int chunkSize = DefaultChunkSize)
        {
            _httpWriter = httpWriter;
            _chunkBuffer = new byte[chunkSize];
            _chunkBufferPos = 0;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public void Write(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            Write(dataBytes, 0, dataBytes.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_chunkBufferPos + count < _chunkBuffer.Length)
            {
                Buffer.BlockCopy(buffer, offset, _chunkBuffer, _chunkBufferPos, count);
                _chunkBufferPos += count;
                return;
            }

            _httpWriter.WriteBody((_chunkBufferPos+count).ToString("X"));
            _httpWriter.WriteBody(CrlfBytes, 0, CrlfBytes.Length);
            _httpWriter.WriteBody(_chunkBuffer, 0, _chunkBufferPos);
            _httpWriter.WriteBody(buffer, offset, count);
            _httpWriter.WriteBody(CrlfBytes, 0, CrlfBytes.Length);

            _chunkBufferPos = 0;
        }

        private void WriteBuffer()
        {
            if (_chunkBufferPos != 0)
            {
                _httpWriter.WriteBody(_chunkBufferPos.ToString("X"));
                _httpWriter.WriteBody(CrlfBytes, 0, CrlfBytes.Length);
                _httpWriter.WriteBody(_chunkBuffer, 0, _chunkBufferPos);
                _httpWriter.WriteBody(CrlfBytes, 0, CrlfBytes.Length);
                _chunkBufferPos = 0;
            }
        }

        public void Commit()
        {
            WriteBuffer();

            _httpWriter.WriteBody(0.ToString());
            _httpWriter.WriteBody(CrlfBytes, 0, CrlfBytes.Length);
            _httpWriter.WriteBody(CrlfBytes, 0, CrlfBytes.Length);
        }

        public override void Flush()
        {
            WriteBuffer();
            _httpWriter.Flush();
        }
    }
}
