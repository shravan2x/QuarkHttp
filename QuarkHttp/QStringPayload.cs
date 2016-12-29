using System.Text;

namespace QuarkHttp
{
    public class QStringPayload : QPayload
    {
        public QStringPayload(string chars, Encoding encoding = null) : base(ToBytes(chars, encoding)) { }

        private static byte[] ToBytes(string chars, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            
            return encoding.GetBytes(chars);
        }
    }

    public static class QStringConverter
    {
        public static string AsString(this QPayload payload, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            return encoding.GetString(payload.Content);
        }
    }
}
