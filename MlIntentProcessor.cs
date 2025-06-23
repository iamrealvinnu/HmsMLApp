using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Catalyst;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Text;
using Newtonsoft.Json;
using WeCantSpell.Hunspell;

namespace HmsMLApp
{
    public class MlIntentProcessor
    {
        private readonly WordList _wordList; // Hunspell word list for spell checking
        private readonly List<IntentData> _intentDatas; // List to store intent training data
        private IEnumerable<MlGrmsIntent> _mlIntents; // Store intents from JSON

        public MlIntentProcessor(WordList wordList)
        {
            // Validate and store word list
            _wordList = wordList ?? throw new ArgumentNullException(nameof(wordList));
            var dataFile = Path.Combine(AppContext.BaseDirectory, "Data", "ghms-restaurant.json"); // Path to intent data
            if (!File.Exists(dataFile))
                throw new FileNotFoundException("Intent JSON file not found.", dataFile);
            _intentDatas = PrepareData(LoadIntents(dataFile)).ToList(); // Prepare training data
        }

        public (MulticlassClassificationMetrics, PredictionEngine<IntentData, IntentPrediction>) Train()
        {
            // Create ML context for machine learning operations
            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(_intentDatas ?? Enumerable.Empty<IntentData>()); // Load data into ML

            // Configure text featurization options
            var options = new TextFeaturizingEstimator.Options
            {
                OutputTokensColumnName = "OutputTokens",
                WordFeatureExtractor = new WordBagEstimator.Options { NgramLength = 1, UseAllLengths = true },
                CharFeatureExtractor = new WordBagEstimator.Options { NgramLength = 3, UseAllLengths = true },
                Norm = TextFeaturizingEstimator.NormFunction.L2,
                StopWordsRemoverOptions = new CustomStopWordsRemovingEstimator.Options
                {
                    StopWords = new[] { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "as", "at",
                        "be", "because", "been", "before", "being", "below", "between", "both", "but", "by",
                        "can", "did", "do", "does", "doing", "don", "down", "during",
                        "each", "few", "for", "from", "further",
                        "had", "has", "have", "having", "he", "her", "here", "hers", "herself", "him", "himself", "his", "how",
                        "i", "if", "in", "into", "is", "it", "its", "itself", "just",
                        "me", "more", "most", "my", "myself", "no", "nor", "not", "now",
                        "of", "off", "on", "once", "only", "or", "other", "our", "ours", "ourselves", "out", "over", "own",
                        "s", "same", "she", "should", "so", "some", "such",
                        "t", "than", "that", "the", "their", "theirs", "them", "themselves", "then", "there", "these", "they", "this", "those", "through", "to", "too",
                        "under", "until", "up", "very", "was", "we", "were", "what", "when", "where", "which", "while", "who", "whom", "why", "will", "with",
                        "you", "your", "yours", "yourself", "yourselves" }
                },
                CaseMode = TextNormalizingEstimator.CaseMode.Lower,
                KeepDiacritics = false,
                KeepPunctuations = false,
                KeepNumbers = true
            };

            // Configure trainer options for multiclass classification
            var trainerOptions = new LbfgsMaximumEntropyMulticlassTrainer.Options
            {
                LabelColumnName = "Label",
                FeatureColumnName = "Features",
                MaximumNumberOfIterations = 20000,
                L1Regularization = 0.0001F,
                L2Regularization = 0.0001F,
                OptimizationTolerance = 1e-8f,
                HistorySize = 10,
                EnforceNonNegativity = true
            };
            var trainer = mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(trainerOptions);

            // Build ML pipeline
            var pipeline = mlContext.Transforms.Conversion
                .MapValueToKey("Label")
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", options, "Text"))
                .Append(mlContext.Transforms.Concatenate("Features", "Features"))
                .AppendCacheCheckpoint(mlContext)
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(dataView); // Train the model

            // Validate the model with sample data
            var validationData = mlContext.Data.LoadFromEnumerable(new List<IntentData>
            {
                new IntentData { Label = "Greeting", Text = "Hi" },
                new IntentData { Label = "Goodbye", Text = "bye bye" }
            });
            var scoredModel = model.Transform(validationData);
            var evaluationMetrics = mlContext.MulticlassClassification.Evaluate(scoredModel);

            // Save the trained model
            mlContext.Model.Save(model, dataView.Schema, Path.Combine(AppContext.BaseDirectory, "Data", "mlHmsIntent.zip"));

            var predictionEngine = mlContext.Model.CreatePredictionEngine<IntentData, IntentPrediction>(model); // Create prediction engine

            return (evaluationMetrics, predictionEngine);
        }

        private IEnumerable<MlGrmsIntent> LoadIntents(string filePath)
        {
            // Load intents from JSON file
            using var r = new StreamReader(filePath);
            var json = r.ReadToEnd();
            _mlIntents = JsonConvert.DeserializeObject<List<MlGrmsIntent>>(json) ?? new List<MlGrmsIntent>();
            return _mlIntents;
        }

        private List<IntentData> PrepareData(IEnumerable<MlGrmsIntent> intents) // Changed return type to List
        {
            // Prepare training data from intent patterns
            var data = (intents ?? Enumerable.Empty<MlGrmsIntent>()).SelectMany(intent => intent.Patterns ?? new List<string>(), (intent, pattern) => new IntentData { Text = pattern, Label = intent.Tag }).ToList();
            return CorrectSpelling(CleanData(data)).ToList(); // Ensure List is returned
        }

        private List<IntentData> CleanData(List<IntentData> data) // Changed return type to List
        {
            // Clean and deduplicate training data
            return (data ?? new List<IntentData>())
                .Where(record => !string.IsNullOrWhiteSpace(record.Label) && !string.IsNullOrWhiteSpace(record.Text))
                .GroupBy(record => new { record.Label, record.Text })
                .Select(group => group.First())
                .ToList();
        }

        private List<IntentData> CorrectSpelling(List<IntentData> data) // Changed return type to List
        {
            // Correct spelling in training data using Hunspell
            (data ?? new List<IntentData>()).ForEach(d => d.Text = CorrectText(_wordList, d.Text));
            return data;
        }

        public static string CorrectText(WordList hunspell, string text)
        {
            // Correct spelling of input text
            if (string.IsNullOrEmpty(text) || hunspell == null) return text;
            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]) && !hunspell.Check(words[i].ToLower()))
                {
                    var suggestions = hunspell.Suggest(words[i], new QueryOptions { MaxWords = 3, MaxCharDistance = 3 });
                    if (suggestions.Any())
                    {
                        words[i] = suggestions.First();
                    }
                }
            }
            return string.Join(" ", words);
        }

        public record IntentData
        {
            // Data structure for intent training
            public string? Text { get; set; } // Input text
            public string? Label { get; set; } // Intent label
        }

        public record IntentPrediction
        {
            // Data structure for intent prediction
            public string? PredictedLabel { get; set; } // Predicted intent label
            public float[] Score { get; set; } // Confidence scores
            public float[] Features { get; set; } // Feature values
            public string[] OutputTokens { get; set; } // Tokenized output
        }

        public record MlGrmsIntent
        {
            // Data structure for intent from JSON
            public string Tag { get; set; } = string.Empty; // Intent tag
            public List<string> Patterns { get; set; } = new(); // Patterns for intent
            public List<string> Responses { get; set; } = new(); // Possible responses
            public List<string>? Actions { get; set; } // Optional actions
        }
    }
}