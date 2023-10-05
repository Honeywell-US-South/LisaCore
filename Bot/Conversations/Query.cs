using LisaCore.MachineLearning.LLM.Intent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.Bot.Conversations
{
    public class Query
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Input { get; set; }
        // Ignore this from EF Core
        [NotMapped]
        public List<IntentPredicted> Intents { get; set; } = new List<IntentPredicted>();

        // Store serialized Intents
        public string SerializedIntents
        {
            get => JsonConvert.SerializeObject(Intents);
            set => Intents = JsonConvert.DeserializeObject<List<IntentPredicted>>(value);
        }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("Conversation")]
        public string ConversationId { get; set; }
        public ICollection<Result> Results { get; set; } = new List<Result>();

        [NotMapped]
        public bool IsSraiInput { get; set; } = false;
    }
}
