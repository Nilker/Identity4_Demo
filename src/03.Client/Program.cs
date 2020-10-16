using System;
using System.Net.Http;
using IdentityModel.Client;

namespace _03.Client
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //1、发现server 元数据
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //2、获取 token
            var tokenRep = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });
            if (tokenRep.IsError)
            {
                Console.WriteLine(tokenRep.Error);
                return;
            }

            Console.WriteLine(tokenRep.Json);

            //3、调用 受保护的资源
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenRep.AccessToken);

            var rep = await apiClient.GetAsync("https://localhost:6001/identity");
            if (!rep.IsSuccessStatusCode)
            {
                Console.WriteLine(rep.StatusCode);
            }
            else
            {
                var content = await rep.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }

        }
    }
}
