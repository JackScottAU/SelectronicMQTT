using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WorkerService1.Selectronic;

namespace ConsoleApp1.Selectronic
{
    public class SelectLiveService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<SelectLiveService> _logger;

        private readonly SelectLiveConfiguration _configuration;

        public SelectLiveService(ILogger<SelectLiveService> logger, IOptions<SelectLiveConfiguration> configuration)
        {
            _logger = logger;

            if(configuration == null || configuration.Value == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _configuration = configuration.Value;

            HttpClientHandler handler = new ()
            {
                CookieContainer = new CookieContainer(),
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task Connect()
        {
            if(string.IsNullOrEmpty(_configuration.EmailAddress) || string.IsNullOrEmpty(_configuration.Password))
            {
                throw new ArgumentNullException("Need username and password.");
            }

            string encoded = "email=" + Uri.EscapeDataString(_configuration.EmailAddress) + "&pwd=" + Uri.EscapeDataString(_configuration.Password);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://select.live/login"),
                Content = new StringContent(encoded, Encoding.UTF8 , "application/x-www-form-urlencoded"),
            };

            var response = await _httpClient.SendAsync(request);

            if(!response.IsSuccessStatusCode)
            {
                throw new ApplicationException("Could not connect.");
            }

            _logger.LogInformation("Connected to select.live service.");
        }

        public async Task<SelectJsonResponse> RawData()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://select.live/dashboard/hfdata/" + _configuration.SystemNumber),
            };

            var response = _httpClient.SendAsync(request).Result;

            string content = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<SelectJsonResponse>(content);

            return data;
        }
    }
}
