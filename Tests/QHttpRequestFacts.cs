using QuarkHttp;
using Xunit;

namespace Tests
{
    public class QHttpRequestFacts
    {
        [Fact]
        public void PassingTest()
        {
            QHttpRequest r = new QHttpRequest("https://google.com");
            r.QueryParams["session"] = "data=";
            Assert.Equal("https://google.com/?session=data%3D", r.Url);
            r.QueryParams["r"] = "3";
            Assert.Equal("https://google.com/?session=data%3D&r=3", r.Url);
            r.QueryParams["r"] = "4";
            Assert.Equal("https://google.com/?session=data%3D&r=4", r.Url);
            r.QueryParams["r"] = "5";
            Assert.Equal("https://google.com/?session=data%3D&r=5", r.Url);
        }
    }
}
