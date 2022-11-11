using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace _254DiscordBot
{
    public class safeBooruUtil
    {
        public class safeOBJ
        {
            public string directory { get; set; }
            public string hash { get; set; }
            public int height { get; set; }
            public int id { get; set; }
            public string image { get; set; }
            public int change { get; set; }
            public string owner { get; set; }
            public int parent_id { get; set; }
            public string rating { get; set; }
            public bool sample { get; set; }
            public int sample_height { get; set; }
            public int sample_width { get; set; }
            public int? score { get; set; }
            public string tags { get; set; }
            public int width { get; set; }
        }
        public static async Task<string> getJSON(string url)
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
