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

        public async Task<bool> Chat(string userId, string conversationId, string input)
        {
            if (_gptChat != null)
            {
                try
                {
                    ChatMessage message = new ChatMessage();
                    message.RequestBy = userId;
                    message.ConversationId = conversationId;
                    message.UserInput = input;
                    await _gptChat.Chat(message);
                    return true;
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
            OnChatMessage?.Invoke(sender, message);
        }
    }
}
