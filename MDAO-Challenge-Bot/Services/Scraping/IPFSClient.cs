using Common.Services;
using System.Net.Http.Json;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class IPFSClient : Singleton
{
    private static readonly Uri IPFSApiUrl = new Uri("https://ipfs.io/ipfs/");

    [Inject]
    private readonly HttpClient Client = null!;

    public async Task<T> GetJsonAsync<T>(string key)
    {
        var url = new Uri(IPFSApiUrl, key);
        return await Client.GetFromJsonAsync<T>(url)
            ?? throw new InvalidOperationException($"Expected {nameof(T)} got {null}");
    }
}
