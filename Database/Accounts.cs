using ModelsNamespace;
using System;
using System.Linq;
using System.Net.Http;

namespace DatabaseNamespace
{
    /// <summary>
    /// Daje nam dostęp do kolekcji Accounts
    /// </summary>
    public class Accounts
    {
        private static readonly HttpClient _client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:3000"),
        };


        public static Account[] GetAccounts()
        {
            HttpResponseMessage response = _client.GetAsync("api/Accounts").Result;
            var result = response.Content.ReadAsAsync<Account[]>().Result;
            return result;
        }
        public static Account[] GetNewsletterSubscribers()
        {
            var result = GetAccounts().Where(x => x.WantsNewsletter).ToArray();
            return result;
        }
        public static void PostAccount(Account account)
        {
            var httpResponseMessage = _client.PostAsJsonAsync("api/Accounts", account).Result;
        }
    }
}
