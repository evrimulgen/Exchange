using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


public interface IApiService
{
    Task<T> GetAsync<T>(string endpoint, string args = null);
}
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private string url;
    
    public ApiService(string url)
    {
        this.url = url;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(url),

        };

        _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    // GET
    public async Task<T> GetAsync<T>(string endpoint, string args = null)
    {
        var response = await _httpClient.GetAsync($"{endpoint}?{args}");

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(response.StatusCode.ToString());

        var result = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(result);
    }

}