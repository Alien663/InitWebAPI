namespace Sample.Service2.Interfaces;

public interface IMyHttpClient
{
    Task GetSample(Dictionary<string, string> queryString);
    Task PostSample();
}
