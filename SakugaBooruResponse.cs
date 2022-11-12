using System;
using System.Collections.Generic;
using System.Text;

namespace _254DiscordBot
{
    class SakugaBooruResponse
    {
        public int Id { get; set; }
        public string Tags { get; set; }
        public int Created_At { get; set; }
        public int Updated_At { get; set; }
        public int Creator_Id { get; set; }
        public object Approver_Id { get; set; }
        public string Author { get; set; }
        public int Change { get; set; }
        public string Source { get; set; }
        public int Score { get; set; }
        public string MD5 { get; set; }
        public int File_Size { get; set; }
        public string File_Ext { get; set; }
        public string File_Url { get; set; }
        public bool Is_Shown_In_Index { get; set; }
        public string Preview_Url { get; set; }
        public int Preview_Width { get; set; }
        public int Preview_Height { get; set; }
        public int Actual_Preview_Width { get; set; }
        public int Actual_Preview_Height { get; set; }
        public string Sample_Url { get; set; }
        public int Sample_Width { get; set; }
        public int Sample_Height { get; set; }
        public int Sample_File_Size { get; set; }
        public string Jpeg_Url { get; set; }
        public int Jpeg_Width { get; set; }
        public int Jpeg_Height { get; set; }
        public int Jpeg_File_Size { get; set; }
        public string Rating { get; set; }
        public bool Is_Rating_Locked { get; set; }
        public bool Has_Children { get; set; }
        public int? Parent_Id { get; set; }
        public string Status { get; set; }
        public bool Is_Pending { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Is_Held { get; set; }
        public string Frames_Pending_String { get; set; }
        public List<object> Frames_Pending { get; set; }
        public string Frames_String { get; set; }
        public List<object> Frames { get; set; }
        public bool Is_Note_Locked { get; set; }
        public int Last_Noted_At { get; set; }
        public int Last_commented_At { get; set; }
    }
}
