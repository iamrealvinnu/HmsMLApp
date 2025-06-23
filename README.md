# HmsMLApp

A C# console-based AI assistant for restaurant-related queries, featuring intent classification, named entity recognition (NER), and natural language processing. The app uses ML.NET for intent prediction and Catalyst for NER, supporting custom food and restaurant vocabulary.

## Features

- **Intent Classification:** Uses ML.NET to classify user queries into intents (e.g., Greeting, Menu, Order, Support).
- **Named Entity Recognition:** Extracts entities like food items and names using Catalyst and custom NER patterns.
- **Spell Checking:** Integrates Hunspell for spell correction, including a custom food-related word list.
- **Extensible Responses:** Modular executor methods for different intents (Greeting, Menu, Order, etc.).
- **Interactive Console:** Accepts user input in a loop, predicts intent, extracts entities, and generates responses.

## Project Structure

- `Program.cs`: Entry point. Sets up dependency injection, loads models, and runs the main input loop.
- `MlIntentProcessor.cs`: Handles intent training, prediction, and spell correction using ML.NET and Hunspell.
- `NerProcessor.cs`: Loads NER patterns, builds the NLP pipeline, and extracts entities from user input.
- `Executor.cs`: Contains response logic for each intent, formatting answers based on extracted entities and context.
- `Util.cs`: Utility functions for console output and collection formatting.
- `CustomWordList.cs`: Custom vocabulary for food/restaurant terms to enhance spell checking.
- `Data/`: Contains required data files:
  - `en_US.aff`, `en_US.dic`: Hunspell dictionary files.
  - `ghms-restaurant.json`: Intent patterns and responses.
  - `ghms-restaurant-ner.json`: NER patterns for entity extraction.
  - `mlHmsIntent.zip`, `mlHmsIntent.ner.bin`: Trained ML and NER models.

## How It Works

1. **Startup:**  
   - Loads Hunspell dictionary and affix files.
   - Loads intent and NER data from JSON.
   - Trains or loads ML and NER models.

2. **User Interaction:**  
   - Prompts the user for input.
   - Predicts the intent using the ML model.
   - Extracts entities using the NER pipeline.
   - Invokes the corresponding method in `Executor` to generate a response.
   - Displays the response and logs the interaction.

3. **Extensibility:**  
   - Add new intents or patterns in `ghms-restaurant.json`.
   - Add new NER tags or patterns in `ghms-restaurant-ner.json`.
   - Extend `Executor.cs` with new response logic as needed.

## Example Usage

```
Enter your question: Show me the dosa menu
> Getting dosa

Enter your question: Hi, I want to order idli for John
> Ordering idli for John. Confirm?

Enter your question: Goodbye
> Good night
```

## Requirements

- .NET 7.0 or 8.0 SDK
- Data files in the `Data/` directory (see above)
- [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet), [Catalyst](https://github.com/curiosity-ai/catalyst), [Hunspell](https://github.com/WekanSpell/Hunspell)

## Running the App

1. Ensure all required data files are present in the `Data/` directory.
2. Build the project:
   ```
   dotnet build
   ```
3. Run the app:
   ```
   dotnet run
   ```

## Customization

- **Add new food or restaurant terms:** Edit `CustomWordList.cs`.
- **Add new intents:** Update `ghms-restaurant.json` and retrain.
- **Add new NER patterns:** Update `ghms-restaurant-ner.json` and retrain.

## License
