using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace ORACLE_ECOMERCE_2021.Models.Authy
{
    public interface IAuthy
    {
        Task<TokenVerificationResult> verifyPhoneTokenAsync(string phoneNumber, string countryCode, string token);
        Task<string> phoneVerificationRequestAsync(string countryCode, string phoneNumber);
    }

    public class Authy : IAuthy
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpClientFactory ClientFactory;
        private readonly HttpClient client;

        public string message { get; private set; }

        public Authy(IConfiguration config, 
            IHttpClientFactory clientFactory)
        {
            Configuration = config;

            ClientFactory = clientFactory;
            client = ClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.authy.com");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("user-agent", "Twilio Account Security C# Sample");

            // Get Authy API Key from Configuration
            client.DefaultRequestHeaders.Add("X-Authy-API-Key", Configuration["AuthyApiKey"]);
        }

        public async Task<TokenVerificationResult> verifyPhoneTokenAsync(string phoneNumber, string countryCode, string token)
        {
            var result = await client.GetAsync(
                $"/protected/json/phones/verification/check?phone_number={phoneNumber}&country_code={countryCode}&verification_code={token}"
            );

            var message = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return new TokenVerificationResult(message);
            }

            return new TokenVerificationResult(message, false);
        }
        public async Task<string> phoneVerificationRequestAsync(string countryCode, string phoneNumber)
        {
            var result = await client.PostAsync(
                $"/protected/json/phones/verification/start?via=sms&country_code={countryCode}&phone_number={phoneNumber}",
                null
            );

            var content = await result.Content.ReadAsStringAsync();

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsStringAsync();
        }
    }

    public class TokenVerificationResult
    {
        public TokenVerificationResult(string message, bool succeeded = true)
        {
            this.Message = message;
            this.Succeeded = succeeded;
        }

        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}