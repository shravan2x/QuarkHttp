using System;

namespace QuarkHttp
{
    public enum QHttpMethod
    {
        Head,
        Get,
        Post,
        Put,
        Patch,
        Delete,
        Options,
        Trace,
        Connect
    }

    public static class QHttpMethodMethods
    {
        public static string Name(this QHttpMethod method)
        {
            return method.ToString().ToUpper();
        }

        public static QHttpMethod Parse(string methodString)
        {
            foreach (QHttpMethod method in Enum.GetValues(typeof(QHttpMethod)))
                if (methodString.ToUpper() == method.Name())
                    return method;

            throw new Exception("Unknown method type");
        }
    }
}
