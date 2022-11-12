using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace _254DiscordBot
{
    public class DanbooruResponse
    {
        public class Image
        {
            public int Id { get; set; }
            public DateTime Created_at { get; set; }
            public int Uploader_id { get; set; }
            public int Score { get; set; }
            public string Source { get; set; }
            public string MD5 { get; set; }
            public DateTime? Last_Comment_Bumped_At { get; set; }
            public string Rating { get; set; }
            public int Image_width { get; set; }
            public int Image_height { get; set; }
            public string Tag_String { get; set; }
            public bool Is_Note_Locked { get; set; }
            public int Fav_Count { get; set; }
            public string File_Ext { get; set; }
            public DateTime? Last_Noted_At { get; set; }
            public bool Is_Rating_Locked { get; set; }
            public int? Parent_Id { get; set; }
            public bool Has_Children { get; set; }
            public int? Approver_Id { get; set; }
            public int Tag_Count_General { get; set; }
            public int Tag_Count_Artist { get; set; }
            public int Tag_Count_Character { get; set; }
            public int Tag_Count_Copyright { get; set; }
            public int File_Size { get; set; }
            public bool Is_Status_Locked { get; set; }
            public string Pool_String { get; set; }
            public int Up_Score { get; set; }
            public int Down_Score { get; set; }
            public bool Is_Pending { get; set; }
            public bool Is_Flagged { get; set; }
            public bool Is_Deleted { get; set; }
            public int Tag_Count { get; set; }
            public DateTime Updated_At { get; set; }
            public bool Is_Banned { get; set; }
            public int? Pixiv_Id { get; set; }
            public DateTime? Last_Commented_At { get; set; }
            public bool Has_Active_Children { get; set; }
            public int Bit_Flags { get; set; }
            public int Tag_Count_Meta { get; set; }
            public bool? Has_Large { get; set; }
            public bool Has_Visible_Children { get; set; }
            public bool Is_Favorited { get; set; }
            public string Tag_String_General { get; set; }
            public string Tag_String_Character { get; set; }
            public string Tag_String_Copyright { get; set; }
            public string Tag_String_Artist { get; set; }
            public string Tag_String_meta { get; set; }
            public string File_Url { get; set; }
            public string Large_File_Url { get; set; }
            public string Preview_File_Url { get; set; }

        }

        public class RootObject
        {
            public List<Image> ImageList { get; set; }

        }
        public static async Task<string> GetJSON(string url)
        {
            using (HttpClient Client = new HttpClient())
            {
                //userAgent info
                string Info = "TuffyBot/1.0 (by Javier)";
                string Type = "application/json";
                Client.BaseAddress = new Uri(url);

                //random client things, not super sure if all of it is needed apart from UserAgent stuff.
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.UserAgent.Clear();
                //important useragent things, not allowed through without this
                Client.DefaultRequestHeaders.UserAgent.ParseAdd(Info);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Type));
                HttpResponseMessage Response = await Client.GetAsync(String.Empty);

                if (Response.IsSuccessStatusCode)
                {
                    string Result = await Response.Content.ReadAsStringAsync();
                    if (Result.Contains("The database timed out running your query"))
                    {
                        return "failure";
                    }
                    return Result;
                }
                else
                {
                    Console.WriteLine(Response.StatusCode);
                    return "failure";
                }
            }
        }
    }
}
