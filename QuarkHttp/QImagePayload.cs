using System.Drawing;
using System.IO;

namespace QuarkHttp
{
    public class QImagePayload : QPayload
    {
        public QImagePayload(Image image) : base(ToBytes(image)) { }

        private static byte[] ToBytes(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            return ms.ToArray();
        }
    }

    public static class QImageConverter
    {
        public static Image AsImage(this QPayload payload)
        {
            using (MemoryStream ms = new MemoryStream(payload.Content))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
