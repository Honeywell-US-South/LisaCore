using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.ML;


namespace LisaCore.MachineLearning.LLM.Intent
{
    public class LLMIntentClassifier
    {
        private const string llmModelName = "LLMIntentClassifierModel";
        private readonly MLContext mlContext;
        private readonly string modelDir;
        private readonly PredictionEnginePool<IntentData, IntentPrediction> predictionEnginePool;
        private readonly IntentDatabaseHandler? intentDatabaseHandler;

        private readonly object syncLock = new object(); // Lock object

        private List<string> _labels = new List<string>();
        public LLMIntentClassifier(string? modelDir = null)
        {
            this.modelDir = modelDir ?? string.Empty;
            string modelPath = string.Empty;
            if (!string.IsNullOrEmpty(modelDir))
            {
                if (!Directory.Exists(modelDir)) { Directory.CreateDirectory(modelDir);}
                modelPath = Path.Combine(modelDir, $"{llmModelName}.zip");
                intentDatabaseHandler = new IntentDatabaseHandler(modelDir);
            }
            
            mlContext = new MLContext();
            var services = new ServiceCollection();
            if (string.IsNullOrEmpty(modelPath) || !File.Exists(modelPath))
            {
                var model = TrainBasic();  // Basic training if no model path is provided
                modelPath = "temp_model.zip";
                mlContext.Model.Save(model, inputSchema: null, filePath: modelPath);
            }
            services.AddPredictionEnginePool<IntentData, IntentPrediction>()
                   .FromFile(modelName: llmModelName, filePath: modelPath, watchForChanges: false);

            var serviceProvider = services.BuildServiceProvider();
            predictionEnginePool = serviceProvider.GetRequiredService<PredictionEnginePool<IntentData, IntentPrediction>>();
        }

        public bool RetrainModel()
        {
            lock (syncLock)
            {
                var data = intentDatabaseHandler?.GetLabeledData();
                if (data?.Count > 0)
                {

                    var trainData = mlContext.Data.LoadFromEnumerable(data.OrderBy(x => x.Label).ToList());
                    _labels.Clear();
                    _labels = data.Select(x => x.Label).Distinct().ToList().OrderBy(s => s).ToList();
                    string modelPath = string.Empty;
                    if (!string.IsNullOrEmpty(modelDir))
                    {
                        if (!Directory.Exists(modelDir)) { Directory.CreateDirectory(modelDir); }
                        modelPath = Path.Combine(modelDir, $"{llmModelName}.zip");
                    }
                    TrainAndSaveModel(trainData, modelPath);
                    return true;
                }
            }
            return false;
        }

        private ITransformer TrainBasic()
        {
            var data = new List<IntentData>
        {
             new IntentData() { Text = "good bye", Label = "End" },
             new IntentData() { Text = "bye", Label = "End" },
             new IntentData() { Text = "see you later", Label = "End" },
             new IntentData() { Text = "I'm done", Label = "End" },

            new IntentData() { Text = "hi", Label = "Greeting" },
            new IntentData() { Text = "hello", Label = "Greeting" },
            new IntentData() { Text = "hello there", Label = "Greeting" },
            new IntentData() { Text = "hey", Label = "Greeting" },
            new IntentData() { Text = "good morning", Label = "Greeting" },
            new IntentData() { Text = "good evening", Label = "Greeting" },

            new IntentData() { Text = "help me please", Label = "Help" },
            new IntentData() { Text = "I need assistance", Label = "Help" },
            new IntentData() { Text = "can you assist me?", Label = "Help" },
            new IntentData() { Text = "I'm lost", Label = "Help" },

            new IntentData() { Text = "tell me a joke", Label = "Entertainment" },
            new IntentData() { Text = "make me laugh", Label = "Entertainment" },
            new IntentData() { Text = "I'm bored", Label = "Entertainment" },

            new IntentData() { Text = "what's the weather?", Label = "Weather" },
            new IntentData() { Text = "is it going to rain?", Label = "Weather" },
            new IntentData() { Text = "how hot is it?", Label = "Weather" }
        };

            var trainData = mlContext.Data.LoadFromEnumerable(data.OrderBy(x => x.Label).ToList());
            _labels.Clear();
            _labels = data.Select(x => x.Label).Distinct().ToList().OrderBy(s=>s).ToList();
            var model = TrainAndSaveModel(trainData);
            return model;
        }

        private ITransformer TrainAndSaveModel(IDataView trainData, string? modelSavePath = null)
        {
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "Text")
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated());

            var model = pipeline.Fit(trainData);
            if (!string.IsNullOrEmpty(modelSavePath))
            {
                mlContext.Model.Save(model, trainData.Schema, modelSavePath);
            }
            return model;
        }

        public List<IntentPredicted> Predict(string text, int take = 3)
        {
            lock (syncLock)
            {
                try
                {
                    var data = new IntentData { Text = text };

                    var prediction = predictionEnginePool.Predict(modelName: llmModelName, data);

                    float[] scores = prediction.Score;

                    // Sort scores and get top 3 indexes and values
                    var results = scores
                                .Select((value, index) => new { Value = value, Index = index })
                                .OrderByDescending(pair => pair.Value)
                                .Take(take)
                                .ToArray();

                    List<IntentPredicted> intents = new List<IntentPredicted>();
                    foreach (var item in results)
                    {
                        intents.Add(new() { Label = _labels[item.Index], Score = item.Value });
                    }

                    return intents;
                }
                catch (Exception ex) { Console.Out.WriteLineAsync(ex.ToString()); }
            }
            return new();
        }

        public List<string> GetLabels()
        {
            var labels = intentDatabaseHandler?.GetLabels();
            return labels??new();
        }

        public bool UpdateLabel(string text, string label)
        {
            var good = intentDatabaseHandler?.UpdateLabel(text, label);
            return good??false;
        }
    }
}
