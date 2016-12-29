using System;

namespace QuarkHttp
{
    public enum EQHttpMethod
    {
        Head,
        Get,
        Post
    }

    public static class QHttpMethodMethods
    {
        public static string Name(this EQHttpMethod method)
        {
            return method.ToString().ToUpper();
        }

        public static EQHttpMethod Parse(string methodString)
        {
            foreach (EQHttpMethod method in Enum.GetValues(typeof(EQHttpMethod)))
                if (methodString.ToUpper() == method.Name())
                    return method;

            throw new Exception("Unknown method type");
        }
    }
}
