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
        private readonly string _systemNumber;

        private readonly HttpClient _httpClient;

        private readonly string _username;

        private readonly string _password;

        public SelectLiveService(IOptions<SelectLiveConfiguration> configuration)
        {
            _systemNumber = configuration.Value.SystemNumber;
            _username = configuration.Value.EmailAddress;
            _password = configuration.Value.Password;

            HttpClientHandler handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task Connect()
        {
            string encoded = "email=" + Uri.EscapeDataString(_username) + "&pwd=" + Uri.EscapeDataString(_password);

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
        }

        public async Task<SelectJsonResponse> RawData()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://select.live/dashboard/hfdata/" + _systemNumber),
            };

            var response = _httpClient.SendAsync(request).Result;

            string content = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<SelectJsonResponse>(content);

            return data;
        }
    }
}