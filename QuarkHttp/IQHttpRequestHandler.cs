namespace QuarkHttp
{
    public interface IQHttpRequestHandler
    {
        void Handle(QHttpRequest request, QHttpWriter httpWriter);
    }
}
