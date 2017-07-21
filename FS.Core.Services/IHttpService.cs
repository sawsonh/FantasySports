namespace FS.Core.Services
{
    public interface IHttpService
    {
        T Get<T>(string url, params object[] vals);
    }
}