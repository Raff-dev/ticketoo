using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace projekt
{
    internal class Orders
    {
        const string API_ENDPOINT_ORDERS = "http://localhost:8000/orders/";

        public class Order
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public bool Reduced { get; set; }
        }

        public static async Task OrderTicketAsync(Order order, Action<HttpResponseMessage> callback)
        {
            using (HttpClient client = new HttpClient())
            {
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("ticket", order.Id.ToString()),
                    new KeyValuePair<string, string>("quantity", order.Quantity.ToString()),
                    new KeyValuePair<string, string>("reduced", order.Reduced.ToString())
                });

                HttpResponseMessage response = await client.PostAsync(API_ENDPOINT_ORDERS, formData);
                callback(response);
            }
        }
    }
}
