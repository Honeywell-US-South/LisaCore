using BrickSchema.Net;
using LisaCore.KnowledgeGraph;
using LisaCore.Bot;
using LisaCore.Interpreter;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using LisaCore.MachineLearning.LLM.Intent;
using LisaCore.MachineLearning.OpenNLP.Tools.BERT;

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
        private LLMIntentClassifier _intentClassifer;

        public Lisa(ILogger? logger = null)
        {
            _logger = logger;
            _graph = new BrickSchemaManager();
            InitCodeProcessor();
        }

        public Lisa(string aiKnowledgeDirectory, ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
            InitKnowledge(aiKnowledgeDirectory);
            _behaviorManager = new BehaviorManager();
        }

        public Lisa(string aiKnowledgeDirectory, string bertModelOnnxFilePath, string bertModelVocabularyFilePath, bool useGpu = false, ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
            InitKnowledge(aiKnowledgeDirectory);
            _behaviorManager = new BehaviorManager();
            InitNlp(bertModelVocabularyFilePath, bertModelOnnxFilePath, useGpu);
        }

        public Lisa(string aiKnowledgeDirectory, string bertModelOnnxFilePath, string bertModelVocabularyFilePath, string nlpContextFilePath, bool useGpu = false, ILogger? logger = null)
        {
            _logger = logger;
            InitCodeProcessor();
            InitKnowledge(aiKnowledgeDirectory);
            _behaviorManager = new BehaviorManager();
            InitNlp(bertModelVocabularyFilePath, bertModelOnnxFilePath, nlpContextFilePath, useGpu);
        }



        private void InitCodeProcessor()
        {

            _codeProcessor = new CodeProcessor();
            LoadDefaultNameSpaces();
        }


        public void InitKnowledge(string aiKnowledgeDirectory)
        {
            Helpers.SystemIOUtilities.CreateDirectoryIfNotExists(aiKnowledgeDirectory);
            string brickFile = Path.Combine(aiKnowledgeDirectory, "graph.json");
            _graph = new BrickSchemaManager(brickFile);
            _intentClassifer = new LLMIntentClassifier(aiKnowledgeDirectory);
            _aiKnowledgeDirectory = aiKnowledgeDirectory;
            _chatlBot = new Chat(_aiKnowledgeDirectory);
            if (_bert != null)
            {
                _chatlBot?.SetBert(_bert);
            }
        }

    }
}

