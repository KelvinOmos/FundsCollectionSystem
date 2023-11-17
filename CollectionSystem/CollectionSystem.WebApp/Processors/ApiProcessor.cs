using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net;
using RestSharp;

namespace CollectionSystem.WebApp.Processors
{
    public class ApiProcessor
    {
        public async Task<string> PostDatasAsync(object data, string url, string token = null)
        {
            var httpKlientHandler = new HttpClientHandler();

            HttpClient client = new HttpClient();

            //SslPolicyErrors ignoredErrors = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch;
            httpKlientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            //ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
            //    System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            //    System.Security.Cryptography.X509Certificates.X509Chain chain,
            //    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            //    {
            //        return true;
            //    };

            string responseD = "";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token)) { client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token); }

            var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
            //Logger.Log("internal api msg" + resp.Result.ToString());
            //var response = resp.Result;
            if (response.IsSuccessStatusCode)
            {
                responseD = response.Content.ReadAsStringAsync().Result;
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> GetDataAsync(string url, string token = null)
        {
            HttpClient client = new HttpClient();
            string responseD = "";

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token)) { client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token); }

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                responseD = response.Content.ReadAsStringAsync().Result;
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> PostDataAsync(object data, string url, string token = null)
        {

            HttpClient client = new HttpClient();

            string responseD = "";

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token)) { client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token); }

            var response = await client.PostAsync(url, new StringContent(data.ToString(), Encoding.UTF8, "application/json"));
            //var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
            //Logger.Log("internal api msg" + resp.Result.ToString());
            if (response.IsSuccessStatusCode)
            {
                responseD = response.Content.ReadAsStringAsync().Result;
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> Post(string url, string data)
        {
            var options = new RestClientOptions(url)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");            
            request.AddStringBody(data, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }
    }
}
