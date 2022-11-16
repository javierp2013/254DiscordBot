using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace _254DiscordBot
{
    // Modified by: Javier Perez
    // This class is just the response structure for the JSON object returned by API call.
    public class SafeBooruUtil
    {
        public class SafeOBJ
        {
            public string Directory { get; set; }
            public string Hash { get; set; }
            public int Height { get; set; }
            public int Id { get; set; }
            public string Image { get; set; }
            public int Change { get; set; }
            public string Owner { get; set; }
            public int Parent_Id { get; set; }
            public string Rating { get; set; }
            public bool Sample { get; set; }
            public int Sample_height { get; set; }
            public int Sample_width { get; set; }
            public int? Score { get; set; }
            public string Tags { get; set; }
            public int Width { get; set; }
        }
        public static async Task<string> GetJSON(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                //userAgent info
                string info = "TuffyBot/1.0 (by Javier)";
                string type = "application/json";
                client.BaseAddress = new Uri(url);

                //random client things, not super sure if all of it is needed apart from UserAgent stuff.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.UserAgent.Clear();
                //important useragent things, not allowed through without this
                client.DefaultRequestHeaders.UserAgent.ParseAdd(info);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(type));
                HttpResponseMessage response = await client.GetAsync(String.Empty);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    return "failure";
                }
            }
        }


    }
}
