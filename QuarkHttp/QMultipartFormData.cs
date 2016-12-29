using System;
using System.IO;
using System.Text;

namespace QuarkHttp
{
    public class QMultipartFormData
    {
        private readonly MemoryStream _formDataStream;
        private readonly string _formDataBoundary;

        private bool _isNeedsCsrf;

        public QMultipartFormData()
        {
            _formDataStream = new MemoryStream();
            _formDataBoundary = $"----------{Guid.NewGuid():N}";

            _isNeedsCsrf = false;
        }

        public string ContentType => "multipart/form-data; boundary=" + _formDataBoundary;

        public void Add(string key, string value)
        {
            if (_isNeedsCsrf)
            {
                byte[] csrfBytes = Encoding.UTF8.GetBytes("\r\n");
                _formDataStream.Write(csrfBytes, 0, csrfBytes.Length);
            }
            else
            {
                _isNeedsCsrf = true;
            }

            string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", _formDataBoundary, key, value);
            byte[] postDataBytes = Encoding.UTF8.GetBytes(postData);
            _formDataStream.Write(postDataBytes, 0, postDataBytes.Length);
        }

        public void Add(string key, byte[] value, string fileName = null, string contentType = null)
        {
            if (_isNeedsCsrf)
            {
                byte[] csrfBytes = Encoding.UTF8.GetBytes("\r\n");
                _formDataStream.Write(csrfBytes, 0, csrfBytes.Length);
            }
            else
            {
                _isNeedsCsrf = true;
            }

            string normalizedFileName = fileName ?? key;
            string normalizedContentType = contentType ?? GetContentType(normalizedFileName);

            string postData = $"--{_formDataBoundary}\r\nContent-Disposition: form-data; name=\"{key}\"; filename=\"{normalizedFileName}\"\r\nContent-Type: {normalizedContentType}\r\n\r\n";
            byte[] postDataBytes = Encoding.UTF8.GetBytes(postData);

            _formDataStream.Write(postDataBytes, 0, postDataBytes.Length);
            _formDataStream.Write(value, 0, value.Length);
        }

        private string GetContentType(string fileName)
        {
            int fileExtensionIndex = fileName.LastIndexOf(".", StringComparison.Ordinal);

            if (fileExtensionIndex == -1)
                return "application/octet-stream";

            return QMimeTypeMap.GetMimeType(fileName.Substring(fileExtensionIndex));
        }

        public byte[] GetBytes()
        {
            byte[] footerBytes = Encoding.UTF8.GetBytes("\r\n--" + _formDataBoundary + "--\r\n");
            _formDataStream.Write(footerBytes, 0, footerBytes.Length);
            _formDataStream.Close();

            return _formDataStream.ToArray();
        }
    }
}
