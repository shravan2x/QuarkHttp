using QuarkHttp;
using Xunit;

namespace Tests
{
    public static class QHttpRequestFacts
    {
        [Fact]
        public static void TestQueryParams()
        {
            QHttpRequest req = new QHttpRequest("https://google.com");
            req.QueryParams["session"] = "data=";
            Assert.Equal("https://google.com/?session=data%3D", req.Url);
            req.QueryParams["r"] = "3";
            Assert.Equal("https://google.com/?session=data%3D&r=3", req.Url);
            req.QueryParams["r"] = "4";
            Assert.Equal("https://google.com/?session=data%3D&r=4", req.Url);
            req.QueryParams["r"] = "5";
            Assert.Equal("https://google.com/?session=data%3D&r=5", req.Url);
        }
    }
}
