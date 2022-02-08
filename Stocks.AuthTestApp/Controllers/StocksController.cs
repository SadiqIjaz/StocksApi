using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StockWeb.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace StocksWeb.Controllers
{
    public class TokenResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public class StocksController : Controller
    {
        // use polly retry

        readonly HttpClient httpClient;
        readonly IConfiguration configuration;

        public StocksController(IConfiguration configuration)
        {
            this.configuration = configuration;
            httpClient = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, configuration["Auth:BaseAddress"])
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", configuration["Auth:ClientId"] },
                    { "client_secret", configuration["Auth:ClientSecret"] },
                    { "audience", configuration["Auth:AuthAudience"] },
                    { "grant_type", "client_credentials" }
                })
            };

            HttpResponseMessage response = httpClient.SendAsync(request).Result;
            var responseStream = response.Content.ReadAsStringAsync().Result;

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseStream);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
        }

        // GET: Stock 
        public ActionResult Index()
        {
            HttpResponseMessage response = httpClient.GetAsync(configuration["Auth:AuthAudience"], HttpCompletionOption.ResponseContentRead).Result;

            var stock = JsonConvert.DeserializeObject<List<StockDTO>>(response.Content.ReadAsStringAsync().Result);

            return View(stock);
        }

        // GET: Stock/Details/5
#pragma warning disable IDE0060 // Remove unused parameter
        public ActionResult Details(int id)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var stock = new StockDTO();
            return View(stock);
        }
    }
}
