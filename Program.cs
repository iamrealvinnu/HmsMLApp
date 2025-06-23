using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeCantSpell.Hunspell;

namespace HmsMLApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Set up dependency injection to manage services
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole()) // Add console logging
                .AddSingleton<MlIntentProcessor>() // Register MlIntentProcessor as a singleton
                .AddSingleton<WordList>(sp =>
                {
                    // Get the base directory of the application
                    var baseDirectory = AppContext.BaseDirectory;
                    var dataDirectory = Path.Combine(baseDirectory, "Data"); // Define Data folder path
                    var dictionaryPath = Path.Combine(dataDirectory, "en_US.dic"); // Path to dictionary file
                    var affixPath = Path.Combine(dataDirectory, "en_US.aff"); // Path to affix file

                    // Display file paths for debugging
                    Console.WriteLine($"Base Directory: {baseDirectory}");
                    Console.WriteLine($"Data Directory: {dataDirectory}");
                    Console.WriteLine($"Dictionary Path: {dictionaryPath}");
                    Console.WriteLine($"Affix Path: {affixPath}");

                    // Create Data directory if it doesn't exist
                    if (!Directory.Exists(dataDirectory))
                    {
                        Directory.CreateDirectory(dataDirectory);
                        Console.WriteLine($"Created Data directory at: {dataDirectory}");
                    }

                    // Check if dictionary and affix files exist
                    if (!File.Exists(dictionaryPath) || !File.Exists(affixPath))
                    {
                        throw new FileNotFoundException("Hunspell dictionary or affix file not found in Data folder. Please ensure 'en_US.dic' and 'en_US.aff' are present in " + dataDirectory);
                    }

                    // Load Hunspell word list from files
                    using var dictionaryStream = File.OpenRead(dictionaryPath);
                    using var affixStream = File.OpenRead(affixPath);
                    var wordList = WordList.CreateFromStreams(dictionaryStream, affixStream);
                    Console.WriteLine("WordList initialized successfully.");

                    return wordList;
                })
                .AddSingleton<NerProcessor>() // Register NerProcessor as a singleton
                .BuildServiceProvider();

            // Get logger instance for logging application events
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger?.LogInformation("Starting GHMS Restaurant Bot application...");

            // Initialize and train intent processor
            var mlIntentProcessor = serviceProvider.GetService<MlIntentProcessor>();
            var (metrics, predictionEngine) = mlIntentProcessor.Train();

            // Initialize and train/load NER processor
            var nerProcessor = serviceProvider.GetService<NerProcessor>();
            await nerProcessor.Train();
            await nerProcessor.Load();

            // Create executor for handling responses
            var executor = new Executor(serviceProvider.GetService<ILogger<Executor>>());

            // Store response history using a concurrent dictionary
            var responseHistory = new ConcurrentDictionary<Guid, Executor.ExecutorReponse>();

            // Main loop to accept user input
            while (true)
            {
                Console.WriteLine();
                Console.Write("Enter your question: ");
                var input = Console.ReadLine()?.Trim();

                // Exit condition
                if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
                {
                    break;
                }

                // Predict intent using ML model
                var intentData = new MlIntentProcessor.IntentData { Text = input };
                var prediction = predictionEngine.Predict(intentData);

                // Process prediction if valid
                if (prediction != null && !string.IsNullOrEmpty(prediction.PredictedLabel) && prediction.Score.Max() > 0.49f)
                {
                    var mi = executor.GetType().GetMethod(prediction.PredictedLabel); // Get method based on predicted label
                    if (mi != null)
                    {
                        var output = mi.Invoke(executor, new object[] { input, prediction.PredictedLabel, prediction.Score.Max(), nerProcessor.SearchEntity(input), Array.Empty<string>() }) as Executor.ExecutorReponse;
                        responseHistory.TryAdd(Guid.NewGuid(), output ?? new Executor.ExecutorReponse());
                        Console.WriteLine(output?.Response ?? string.Empty);
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, I didn’t understand that. How can I help you?");
                }
            }
        }
    }
}