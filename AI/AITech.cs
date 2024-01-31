using LLama;
using LLama.Common;
using System.Collections.Concurrent;
using static System.Net.Mime.MediaTypeNames;

namespace LisaCore.AI
{
    public class AITech : IDisposable
    {

        public event EventHandler<AiSpeak> OnAiSpeak;
        public event EventHandler<AiChatMessage> OnChatMessage;

        private readonly InteractiveExecutor _executor;
        private readonly LLamaWeights _model;
        private readonly LLamaContext _context;
        private readonly ConcurrentQueue<AiChatMessage> _inputQueue = new ConcurrentQueue<AiChatMessage>();
        private readonly ConcurrentQueue<string> _infoQueue = new ConcurrentQueue<string>();
        private bool _stop = true;
        private readonly bool _model_loaded;
        private string SystemInstruction = @"You are an adept and intelligent building automation technician named Ember. Ember is an expert at HVAC, DDC controls, Fire Alarm, Access Control, Security, and Digital Video. Ember can remember information set between <<SYS>>info<</SYS>>. Ember use the info to identify issues and help user solve problem. When User ask for help, provides step by step instruction how to fix the problem.";
        public bool IsLoaded => _model_loaded;


        public AITech(string modelPath, string systemInstruction = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(systemInstruction)) SystemInstruction = systemInstruction;
                if (!File.Exists(modelPath))
                {
                    return;
                }
                var parameters = new ModelParams(modelPath)
                {
                    ContextSize = 1024,
                    Seed = 1337,
                    GpuLayerCount = 5
                };
                _model = LLamaWeights.LoadFromFile(parameters);
                _context = _model.CreateContext(parameters);
                if (_context == null)
                    return;
                _executor = new InteractiveExecutor(_context);
                _model_loaded = true;

            } catch (Exception ex) { 
                _model_loaded = false;
                throw new Exception("FailedLoadModelFromFile", ex);
            }
            
        }

        public async Task UserInput(AiChatMessage input)
        {
            await Task.Run(() => _inputQueue.Enqueue(input));
        }
        public async Task InfoInput(string input)
        {
            await Task.Run(() => _infoQueue.Enqueue(input));
        }


        private async Task Run()
        {
            var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" }, MaxTokens = 128 };
            await UserInput(new() { RequestBy = "System", UserInput = $"{SystemInstruction}\r\nUser:" });
            while (!_stop)
            {
                bool processedInput = false;
                if (_inputQueue.TryDequeue(out var message))
                {
                    processedInput = true;
                    var outMessage = "";
                    var prompt = message.UserInput; //firstboot? message.UserInput : string.IsNullOrEmpty(message.SystemInput) ? $@"[INST]{message.UserInput}[/INST]" : $@"[INST]<<SYS>>{message.SystemInput}<</SYS>>{message.UserInput}[/INST]";
                    await foreach (var text in _executor.InferAsync(prompt, inferenceParams))
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            OnAiSpeak?.Invoke(null, new() { ConverstationId = message.ConversationId, Message = text });
                        }
                        outMessage += text;
                        
                    }
                    message.OutputMessage = outMessage;
                    OnChatMessage?.Invoke(null, message);
                    

                }
                else
                {
                    bool delayed = false;
                    if (!processedInput && _infoQueue.TryDequeue(out var info))
                    {
                        await foreach (var text in _executor.InferAsync($@"{info}", inferenceParams))
                        {
                            if (!string.IsNullOrEmpty(text))
                            {
                                OnAiSpeak?.Invoke(null, new() { Message = text });
                            }
                        }

                    }
                    else
                    {
                        await Task.Delay(100); // Prevents tight looping when queue is empty
                        delayed = true;
                    }

                    if (!processedInput && !delayed)
                    {
                        await Task.Delay(100); // Prevents tight looping when queue is empty
                    }
                }
            }
        }

        
        public bool Start()
        {
            if (!_model_loaded) { return false; }
            _stop = false;
            Task.Run(() => Run());
            return true;
        }

        public bool Stop()
        {
            _stop = true;
            return _stop;
        }
        public void Dispose()
        {
            _context.Dispose();
            _model.Dispose();
            
        }
    }
}
