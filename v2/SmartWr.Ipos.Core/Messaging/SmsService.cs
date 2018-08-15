using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartWr.Ipos.Core.Messaging
{
    public class SmsService
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task<HttpResponseMessage> Send(HttpRequestMessage msg)
        {
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(msg);
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

     
    }
}