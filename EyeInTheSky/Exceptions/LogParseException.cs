namespace EyeInTheSky.Exceptions
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class LogParseException : Exception
    {
        [JsonProperty]
        public string Log { get; }
        [JsonProperty]
        public string EditFlags { get; }
        [JsonProperty]
        public string Comment { get; }
        [JsonProperty]
        public string LineData { get; }
        [JsonProperty]
        public string Channel { get; }
        
        /// <summary>
        /// Length of the inbound IRC message
        /// </summary>
        public int MessageLength { get; set; }
        public byte[] RawData { get; set; }
        
        public LogParseException(string log, string editFlags, string comment, string data, string channel)
        {
            this.Log = log;
            this.EditFlags = editFlags;
            this.Comment = comment;
            this.LineData = data;
            this.Channel = channel;
        }
    }
}