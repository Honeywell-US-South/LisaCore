using BrickSchema.Net;
using LisaCore.KnowledgeGraph;
using LisaCore.Bot;
using LisaCore.Interpreter;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using LisaCore.MachineLearning.LLM.Intent;
using LisaCore.MachineLearning.OpenNLP.Tools.BERT;
using LisaCore.AI;

namespace LisaCore
{
    public partial class Lisa
    {
        private string? _aiKnowledgeDirectory = null;
        private Chat? _chatlBot = null;
        private CodeProcessor _codeProcessor;
        private ILogger? _logger;
        private Dictionary<string, List<Conversion>> _conversations;
        private Bert? _bert = null;
        private BrickSchemaManager _graph;
        private BehaviorManager _behaviorManager;
        private AlertManager _alertManager;
        private AITech? _aiTech;
        private LLMIntentClassifier _intentClassifer;

        public Lisa(ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
        }

        public Lisa(string aiKnowledgeDirectory, ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
            InitKnowledge(aiKnowledgeDirectory);
            StartGraphManager(aiKnowledgeDirectory);
        }

        public Lisa(string aiKnowledgeDirectory, string bertModelOnnxFilePath, string bertModelVocabularyFilePath, bool useGpu = false, ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
            InitKnowledge(aiKnowledgeDirectory);
            InitNlp(bertModelVocabularyFilePath, bertModelOnnxFilePath, useGpu);
            StartGraphManager(aiKnowledgeDirectory);
        }

        public Lisa(string aiKnowledgeDirectory, string bertModelOnnxFilePath, string bertModelVocabularyFilePath, string nlpContextFilePath, bool useGpu = false, ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
            InitKnowledge(aiKnowledgeDirectory);
            
            InitNlp(bertModelVocabularyFilePath, bertModelOnnxFilePath, nlpContextFilePath, useGpu);
            StartGraphManager(aiKnowledgeDirectory);
        }

        private void StartGraphManager(string graphDir = "")
        {
            if (string.IsNullOrEmpty(graphDir))
            {
                _graph = new BrickSchemaManager();
            } else
            {
                Helpers.SystemIOUtilities.CreateDirectoryIfNotExists(graphDir);
                string brickFile = Path.Combine(graphDir, "graph.json");
                _graph = new BrickSchemaManager(brickFile);
            }
            _behaviorManager = new BehaviorManager();
            _alertManager = new AlertManager(_graph);
        }


        private void InitCodeProcessor()
        {

            _codeProcessor = new CodeProcessor();
            LoadDefaultNameSpaces();
        }


        public void InitKnowledge(string aiKnowledgeDirectory)
        {
            Helpers.SystemIOUtilities.CreateDirectoryIfNotExists(aiKnowledgeDirectory);
            try
            {
                _aiTech = new AITech(Path.Combine(aiKnowledgeDirectory, "mixtral-8x7b-v0.1.Q5_K_M.gguf"));
                _aiTech.OnChatMessage += OnAiChatMessageReceived;
                _aiTech.Start();
            } catch (Exception ex)
            {
                _aiTech = null;
                //throw new Exception("FailedAIInit", ex);
            }
            //_intentClassifer = new LLMIntentClassifier(aiKnowledgeDirectory);
            //_aiKnowledgeDirectory = aiKnowledgeDirectory;
            //_chatlBot = new Chat(_aiKnowledgeDirectory);
            //if (_bert != null)
            //{
            //    _chatlBot?.SetBert(_bert);
            //}
        }

    }
}

