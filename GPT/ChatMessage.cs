using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.GPT
{
    public class ChatMessage
    {
        public string ConversationId { get; set; } = string.Empty;
        public string RequestBy { get; set; } = string.Empty;
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public string SystemInput { get; set; } = string.Empty; //Informaiton about current topic
        public string UserInput { get; set; } = string.Empty;
        public string ResponseBy { get; set; } = string.Empty;
        public string OutputMessage { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
