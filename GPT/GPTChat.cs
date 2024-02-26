﻿using LLama;
using LLama.Common;
using System.Collections.Concurrent;
using System.Text;
using static LLama.Common.ChatHistory;
using static System.Net.Mime.MediaTypeNames;

namespace LisaCore.GPT
{
    public class GPTChat : IDisposable
    {
        public event EventHandler<ChatSpeak>? OnGPTSpeak;
        public event EventHandler<ChatMessage>? OnGPTChatMessage;

        private readonly InteractiveExecutor _executor;
        private readonly LLamaContext _context;
        private readonly ConcurrentQueue<ChatMessage> _inputQueue = new();
        private readonly ConcurrentQueue<string> _infoQueue = new();
        private bool _stop = true;
        private readonly string _chatName;
        private readonly uint _maxTokenLimit;
        private string SystemInstruction => $@"You are an adept and intelligent building automation technician named {_chatName}. {_chatName} is an expert at HVAC, DDC controls, Fire Alarm, Access Control, Security, and Digital Video. {_chatName} can remember information set between <<SYS>>info<</SYS>>. {_chatName} use the info to identify issues and help user solve problem. When User ask for help, provides step by step instruction how to fix the problem.";

        public GPTChat(string chatName, string modelPath, uint maxTokenLimit = 1024)
        {
            _maxTokenLimit = maxTokenLimit;
            _chatName = chatName ?? throw new ArgumentNullException(nameof(chatName));
            if (!File.Exists(modelPath)) throw new FileNotFoundException("Model file not found.", modelPath);

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = _maxTokenLimit,
                Seed = 1337,
                GpuLayerCount = 5
            };
            var model = LLamaWeights.LoadFromFile(parameters);
            _context = model.CreateContext(parameters) ?? throw new InvalidOperationException("Unable to create context from model.");
            _executor = new InteractiveExecutor(_context);
            _maxTokenLimit = maxTokenLimit;
        }

        public Task ChatAsync(ChatMessage input) => EnqueueAsync(_inputQueue, input);

        private async Task EnqueueAsync<T>(ConcurrentQueue<T> queue, T item)
        {
            await Task.Yield(); // Ensure the method is asynchronous
            queue.Enqueue(item);
        }

        private async Task Run()
        {
            var inferenceParams = new InferenceParams { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" }, MaxTokens = 128 };
            await ChatAsync(new ChatMessage { RequestBy = "System", UserInput = $"{SystemInstruction}\r\nUser:" });

            while (!_stop)
            {
                await ProcessInputQueue(inferenceParams);
                await Task.Delay(100);
            }
        }

        private async Task ProcessInputQueue(InferenceParams inferenceParams)
        {
            if (_inputQueue.TryDequeue(out var message))
            {

                var outputMessage = await InferMessageAsync(message, inferenceParams);
                
                message.OutputMessage = outputMessage;
                OnGPTChatMessage?.Invoke(this, message);

            }

        }

        private async Task<string> InferMessageAsync(ChatMessage input, InferenceParams inferenceParams)
        {
            if (string.IsNullOrEmpty(input.UserInput)) { return string.Empty; }
            var outputMessage = new StringBuilder();
            //var userMessage = input.UserInput;
            //var contextMessage = input.Context;
            //int userInputTokenCount = GPT3Tokenizer.Encode(input.UserInput).Count;
            //int maxContextToken = (int)_maxTokenLimit - userInputTokenCount;
            //if (maxContextToken <= 0)
            //{
            //    if (maxContextToken < 0) //don't trim if token is zero.
            //    { 
            //        userMessage = GPT3Tokenizer.TrimTextUpToTokenLimit(input.UserInput, (int)_maxTokenLimit);
            //    } 
            //    contextMessage = string.Empty; 
            //} else if (!string.IsNullOrEmpty(contextMessage))
            //{
            //    contextMessage = GPT3Tokenizer.TrimTextUpToTokenLimit(contextMessage, maxContextToken);
            //}

            var fullPrompt = string.IsNullOrEmpty(input.Context)? input.UserInput : $"{input.Context}\n{input.UserInput}";
            await foreach (var text in _executor.InferAsync(fullPrompt, inferenceParams))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    outputMessage.Append(text);
                    OnGPTSpeak?.Invoke(this, new ChatSpeak {ConverstationId = input.ConversationId, Message = text });
                }
            }
            return outputMessage.ToString();
        }
        

        public bool Start()
        {
            if (_stop)
            {
                _stop = false;
                Task.Run(Run);
                return true;
            }
            return false;
        }

        public bool Stop()
        {
            _stop = true;
            return true;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
