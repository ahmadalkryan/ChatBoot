using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Dtos.AiDto
{
    public class RequestDto
    {
        [JsonPropertyName("messages")]
        public List<MessageDto>  Messages{ get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }="meta-llama/llama-3.3-70b-instruct:free";
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }= 0.7;
        [JsonPropertyName("max_tokens")]
        public int Max_Tokens { get; set; }= 500;

    }
}
