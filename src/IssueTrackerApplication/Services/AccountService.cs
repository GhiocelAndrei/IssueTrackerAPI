using IssueTracker.Abstractions.Definitions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Text;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Application.Services
{
    public  class AccountService
    {
        private readonly Auth0Settings _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountService(IOptions<Auth0Settings> appSettings, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetToken()
        {
            var requestContent = new
            {
                client_id = _appSettings.ClientId,
                client_secret = _appSettings.ClientSecret,
                audience = _appSettings.Audience,
                grant_type = "client_credentials"
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri($"https://{_appSettings.Domain}/");
                var response = await httpClient.PostAsync("oauth/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var json = JObject.Parse(responseContent);
                        return json["access_token"].ToString();
                    }
                    catch (Exception)
                    {
                        throw new AuthenticationException("The response from the server was not a valid JSON.");
                    }
                }
                else
                {
                    throw new AuthenticationException("Failed to retrieve the access token.");
                }
            }
            catch (HttpRequestException e)
            {
                throw new AuthenticationException($"A network error occurred while attempting to fetch the token : {e.Message}");
            }
        }
    }
}
