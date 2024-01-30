using LisaCore.AI;
using LisaCore.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

//Keep this as LisaCore
namespace LisaCore
{
    /// <summary>
    /// Part of Lisa class
    /// </summary>
    public partial class Lisa
    {
        public event EventHandler<AiSpeak> OnAiSpeak;
        public event EventHandler<AiChatMessage> OnAiChatMessage;

        public async Task<bool> Chat(string userId, string conversationId, string input)
        {
            if (_aiTech != null)
            {
                try
                {
                    AiChatMessage message = new AiChatMessage();
                    message.RequestBy = userId;
                    message.ConversationId = conversationId;
                    message.UserInput = input;
                    _aiTech.UserInput(message);
                    return true;
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }

        public bool UpdateInfo(string input)
        {
            if (_aiTech != null)
            {
                try
                {
                    _aiTech.InfoInput(input);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }


        private void OnAiSpeakReceived(object? sender, AiSpeak message)
        {
            OnAiSpeak?.Invoke(null, message);
        }

        private void OnAiChatMessageReceived(object? sender, AiChatMessage message)
        {
            OnAiChatMessage?.Invoke(null, message);
        }
    }
}
