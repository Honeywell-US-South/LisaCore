using BrickSchema.Net.Classes.Measureable;
using LisaCore.GPT;

//Keep this as LisaCore
namespace LisaCore
{
    /// <summary>
    /// Part of Lisa class
    /// </summary>
    public partial class Lisa
    {
        public event EventHandler<ChatSpeak>? OnChatSpeak;
        public event EventHandler<ChatMessage>? OnChatMessage;

        public async Task<bool> ChatAsync(string userId, string conversationId, string input)
        {
            if (_gptChat != null)
            {
                try
                {
                    ChatMessage message = new ChatMessage();
                    message.RequestBy = userId;
                    message.ConversationId = conversationId;
                    message.UserInput = input;
                    await _gptChat.ChatAsync(message);
                    return true;
                } catch (Exception ex)
                {
                    Console.Out.WriteLineAsync(ex.Message);
                }
            }

            return false;
        }

        
        private void OnGPTSpeakReceived(object? sender, ChatSpeak message)
        {
            
            OnChatSpeak?.Invoke(sender, message);
        }

        private void OnGPTChatMessageReceived(object? sender, ChatMessage message)
        {
            if (message.OutputMessage.EndsWith("User:")) message.OutputMessage = message.OutputMessage.Substring(0, message.OutputMessage.Length - 5);
            OnChatMessage?.Invoke(sender, message);
        }
    }
}
