using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catalyst;
using Catalyst.Models;
using Mosaik.Core;
using Newtonsoft.Json;

namespace HmsMLApp
{
    public class NerProcessor
    {
        private readonly Pipeline NlpPipeLine; // Pipeline for natural language processing
        private readonly IEnumerable<NerRecord> _nerRecords; // Store NER records from JSON

        public NerProcessor()
        {
            // Define path to NER data file
            var dataFile = Path.Combine(AppContext.BaseDirectory, "Data", "ghms-restaurant-ner.json");
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException("NER data file 'ghms-restaurant-ner.json' not found in Data folder.", dataFile);
            }
            _nerRecords = LoadNerRecords(dataFile); // Load NER records

            try
            {
                // Register English models for NLP
                Catalyst.Models.English.Register();
                NlpPipeLine = Pipeline.For(Language.English); // Initialize pipeline with models
                if (NlpPipeLine == null)
                {
                    throw new InvalidOperationException("Pipeline.For returned null. Ensure Catalyst.Models.English models are installed.");
                }

                // Add custom Spotters for NER based on records
                foreach (var nerRecord in _nerRecords)
                {
                    var spotter = new Spotter(Language.English, 0, nerRecord.Tag, nerRecord.Tag)
                    {
                        Data = { IgnoreCase = true } // Enable case-insensitive matching
                    };
                    spotter.AppendList(nerRecord.Patterns ?? new List<string>()); // Add patterns to Spotter
                    NlpPipeLine.Add(spotter);
                }

                Console.WriteLine("NlpPipeLine initialized successfully with pre-trained models and Spotters.");
            }
            catch (Exception ex)
            {
                // Fallback if model initialization fails
                Console.WriteLine($"Error initializing NlpPipeLine: {ex.Message}. Creating empty pipeline as fallback. Ensure Catalyst.Models.English is installed or place models in {Path.Combine(AppContext.BaseDirectory, "Data")}.");
                NlpPipeLine = new Pipeline();

                // Add Spotters in fallback mode
                foreach (var nerRecord in _nerRecords)
                {
                    var spotter = new Spotter(Language.English, 0, nerRecord.Tag, nerRecord.Tag)
                    {
                        Data = { IgnoreCase = true }
                    };
                    spotter.AppendList(nerRecord.Patterns ?? new List<string>());
                    NlpPipeLine.Add(spotter);
                }
            }
        }

        public async Task Train()
        {
            // Check if pipeline is initialized
            if (NlpPipeLine == null)
            {
                throw new InvalidOperationException("NlpPipeLine is not initialized.");
            }
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "mlHmsIntent.ner.bin"); // Define NER model file path
            using var fileStream = File.OpenWrite(filePath);
            foreach (var nerRecord in _nerRecords ?? Enumerable.Empty<NerRecord>())
            {
                var spotter = new Spotter(Language.English, 0, nerRecord.Tag, nerRecord.Tag)
                {
                    Data = { IgnoreCase = true }
                };
                spotter.AppendList(nerRecord.Patterns ?? new List<string>());
                await spotter.StoreAsync(fileStream); // Save Spotter data
            }
            Console.WriteLine("NER training completed.");
        }

        public async Task Load()
        {
            // Check if pipeline is initialized
            if (NlpPipeLine == null)
            {
                throw new InvalidOperationException("NlpPipeLine is not initialized.");
            }
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "mlHmsIntent.ner.bin");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("NER model file 'mlHmsIntent.ner.bin' not found.", filePath);
            using var fileStream = File.OpenRead(filePath);
            foreach (var nerRecord in _nerRecords ?? Enumerable.Empty<NerRecord>())
            {
                var spotter = new Spotter(Language.English, 0, nerRecord.Tag, nerRecord.Tag)
                {
                    Data = { IgnoreCase = true }
                };
                await spotter.LoadAsync(fileStream); // Load Spotter data
                NlpPipeLine.Add(spotter);
            }
            Console.WriteLine("NER model loaded successfully.");
        }

        private IEnumerable<NerRecord> LoadNerRecords(string filePath)
        {
            // Read and deserialize NER data from JSON file
            using var records = new StreamReader(filePath);
            var json = records.ReadToEnd();
            var nerRecords = JsonConvert.DeserializeObject<List<NerRecord>>(json);
            if (nerRecords == null)
            {
                throw new InvalidDataException("Failed to deserialize 'ghms-restaurant-ner.json'. Check the JSON format.");
            }
            return nerRecords;
        }

        public List<(string EntityType, string EntityValue, string Processed)> SearchEntity(string input)
        {
            // Ensure pipeline is initialized
            if (NlpPipeLine == null)
            {
                throw new InvalidOperationException("NlpPipeLine is not initialized.");
            }
            if (string.IsNullOrEmpty(input))
                return new List<(string, string, string)>();
            var processed = NlpPipeLine.ProcessSingle(new Document(input, Language.English)); // Process input text
            var tokens = string.Join(", ", processed.SelectMany(p => p.Select(o => o.Value))); // Join tokens for debugging
            var pos = string.Join(", ", processed.SelectMany(p => p.Select(o => o.POS))); // Join part-of-speech tags
            return processed.SelectMany(p => p.GetEntities()) // Extract entities
                .Select(e => (EntityType: e.EntityType.Type, EntityValue: e.Value, Processed: $"[{tokens}]-[{pos}]")).ToList();
        }

        public record NerRecord
        {
            // Record to hold NER tag and patterns
            public string Tag { get; set; } = string.Empty; // Entity type (e.g., Name, FoodItem)
            public List<string> Patterns { get; set; } = new(); // List of patterns for the entity
        }
    }
}