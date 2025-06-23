using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Sequence;

namespace HmsMLApp
{
    public class Executor
    {
        private readonly ILogger Logger; // Logger for tracking execution

        public Executor(ILogger logger)
        {
            // Initialize logger, throw exception if null
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ExecutorReponse Greeting(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get name entity if available
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            // Set response based on time of day
            string customResponse = DateTime.Now.Hour switch
            {
                < 12 => $"Good morning {name}, how can I help you?",
                < 18 => $"Good afternoon {name}, how can I help you?",
                _ => $"Good evening {name}, how can I help you?"
            };
            return new ExecutorReponse
            {
                Question = question ?? string.Empty, // Store user question
                Prediction = prediction ?? string.Empty, // Store predicted intent
                Response = customResponse, // Set greeting response
                RequestInformation = false, // No additional info needed
                ResponseType = "Greeting", // Type of response
                Score = score, // Confidence score
                Entities = ners ?? new List<(string, string, string)>() // Store entities
            };
        }

        public ExecutorReponse Goodbye(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get name entity if available
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            // Set response based on time of day
            string customResponse = DateTime.Now.Hour switch
            {
                < 12 => $"Have a great morning {name}",
                < 18 => $"Have a great afternoon {name}",
                _ => $"Good night {name}"
            };
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "Goodbye",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse Compliment(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get name entity if available
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = $"Thank you that was nice of you {name}",
                RequestInformation = false,
                ResponseType = "Compliment",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse Criticism(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get name entity if available
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = $"I am sorry you feel that way! {name}",
                RequestInformation = false,
                ResponseType = "Criticism",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse Menu(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item entity if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Yes we do have {food}" : "Getting you the menu of what food we have";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "Menu",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse SearchDosa(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item and name entities if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" for {name}";
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Getting {food}{formattedName}" : "Getting all dosas available";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "SearchDosa",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse SearchIdly(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item and name entities if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" for {name}";
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Getting {food}{formattedName}" : "Getting all idlies available";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "SearchIdly",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse SearchNonvegAppetizer(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item and name entities if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" for {name}";
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Getting {food}{formattedName}" : "Getting all non-veg appetizers available";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "SearchNonvegAppetizer",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse SearchVegAppetizer(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item and name entities if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" for {name}";
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Getting {food}{formattedName}" : "Getting all veg appetizers available";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "SearchVegAppetizer",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse SearchBeverage(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item and name entities if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" for {name}";
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Getting {food}{formattedName}" : "Getting all beverages available";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "SearchBeverage",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse Order(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get food item and name entities if available
            var food = ners?.FirstOrDefault(n => n.EntityType == "FoodItem").EntityValue ?? string.Empty;
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" for {name}";
            // Set response based on food item presence
            var customResponse = !string.IsNullOrEmpty(food) ? $"Ordering {food}{formattedName}. Confirm?" : "Please specify what to order.";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = customResponse,
                RequestInformation = false,
                ResponseType = "Order",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        public ExecutorReponse Support(string question, string prediction, float score, List<(string EntityType, string EntityValue, string Processed)> ners = null, string[] args = null)
        {
            // Get name entity if available
            var name = ners?.FirstOrDefault(n => n.EntityType == "Name").EntityValue ?? string.Empty;
            var formattedName = string.IsNullOrEmpty(name) ? string.Empty : $" {name}";
            return new ExecutorReponse
            {
                Question = question ?? string.Empty,
                Prediction = prediction ?? string.Empty,
                Response = $"SupportInformation{formattedName}",
                RequestInformation = false,
                ResponseType = "Support",
                Score = score,
                Entities = ners ?? new List<(string, string, string)>()
            };
        }

        private static (string StartDate, string EndDate) ParseDate(string input, string culture)
        {
            // Parse date information from input text
            var startDate = string.Empty;
            var endDate = string.Empty;
            var results = DateTimeRecognizer.RecognizeDateTime(input, culture);
            foreach (var result in results)
            {
                var resolutions = result.Resolution["values"] as List<Dictionary<string, string>>;
                if (resolutions != null)
                {
                    foreach (var resolution in resolutions)
                    {
                        if (resolution.TryGetValue("value", out var dateString) || resolution.TryGetValue("start", out dateString))
                        {
                            if (DateTime.TryParse(dateString, null, System.Globalization.DateTimeStyles.None, out var candidate))
                            {
                                startDate = candidate.ToShortDateString();
                                if (resolution.TryGetValue("end", out var endDateString) && DateTime.TryParse(endDateString, null, System.Globalization.DateTimeStyles.None, out var candidateEndDate))
                                {
                                    endDate = candidateEndDate.ToShortDateString();
                                }
                            }
                        }
                    }
                }
            }
            return (startDate, endDate);
        }

        public record ExecutorReponse
        {
            // Record to store response details
            public Guid Id { get; init; } = Guid.NewGuid(); // Unique response ID
            public Guid? ParentId { get; set; } // Optional parent ID
            public string Question { get; init; } = string.Empty; // User question
            public string Prediction { get; init; } = string.Empty; // Predicted intent
            public string Response { get; init; } = string.Empty; // Bot response
            public string ResponseType { get; init; } = string.Empty; // Type of response
            public bool RequestInformation { get; init; } = false; // Flag for additional info
            public float Score { get; init; } = 0.0f; // Confidence score
            public List<(string EntityType, string EntityValue, string Processed)> Entities { get; init; } = new(); // Detected entities
            public List<(string EntityType, string EntityValue, string Processed)> RequestInformationEntities { get; init; } = new(); // Entities for info requests
            public string Tokens { get; set; } = string.Empty; // Tokenized text
            public string Udf1 { get; init; } = string.Empty; // User-defined field 1
            public string Udf2 { get; init; } = string.Empty; // User-defined field 2
            public string Udf3 { get; init; } = string.Empty; // User-defined field 3
            public DateTime RespondedOn { get; } = DateTime.Now; // Response timestamp
        }
    }
}