﻿using BrickSchema.Net;
using LisaCore.KnowledgeGraph;
using LisaCore.Bot;
using LisaCore.Interpreter;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using LisaCore.MachineLearning.LLM.Intent;
using LisaCore.GPT;

namespace LisaCore
{
    public partial class Lisa
    {
        private string? _aiKnowledgeDirectory = null;
        private CodeProcessor _codeProcessor;
        private ILogger? _logger;
        private Dictionary<string, List<Conversion>> _conversations;
        private BrickSchemaManager _graph;
        private BehaviorManager _behaviorManager;
        private AlertManager _alertManager;
        private GPTChat? _gptChat;
        private LLMIntentClassifier _intentClassifer;
        private readonly string _appName;

        public Lisa(string appName, string dataDirectory, ILogger? logger = null, bool loadGraph = true, bool loadCodeProcessor = false, bool loadGPT = false)
        {
            _appName = appName;
            _logger = logger;
            if (loadCodeProcessor) InitCodeProcessor();
            if (loadGPT) InitGPT(Path.Combine(dataDirectory, "GPT"));
            if (loadGraph) InitKnowledgeGraph(dataDirectory);
        }

        private void InitKnowledgeGraph(string graphDir)
        {
            Helpers.SystemIOUtilities.CreateDirectoryIfNotExists(graphDir);
            string brickFile = Path.Combine(graphDir, "graph.json");
            _graph = new BrickSchemaManager(brickFile);

            _behaviorManager = new BehaviorManager();
            _alertManager = new AlertManager(_graph);
        }


        private void InitCodeProcessor()
        {

            _codeProcessor = new CodeProcessor();
            LoadDefaultNameSpaces();
        }


        public void InitGPT(string gptModelDirectory)
        {
            Helpers.SystemIOUtilities.CreateDirectoryIfNotExists(gptModelDirectory);
            try
            {
                _gptChat = new GPTChat(_appName, Path.Combine(gptModelDirectory, "mixtral-8x7b-v0.1.Q5_K_M.gguf"));
                _gptChat.OnGPTChatMessage += OnGPTChatMessageReceived;
                _gptChat.OnGPTSpeak += OnGPTSpeakReceived; ;
                _gptChat.Start();
            } catch (Exception ex)
            {
                _gptChat = null;
                throw new Exception("FailedGPTChatInit", ex);
            }

        }

    }
}

