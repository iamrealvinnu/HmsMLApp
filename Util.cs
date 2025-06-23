using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GHMSRestaurantBot
{
    /// <summary>
    /// Utility class for formatting console output and extending LINQ functionality.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Applies a style to the text (simulated for console output).
        /// </summary>
        /// <param name="text">Text to style.</param>
        /// <param name="style">Style description (e.g., color, font-size).</param>
        /// <returns>Styled text (plain for console).</returns>
        public static string WithStyle(string text, string style = "")
        {
            // In a real UI, apply style; for console, return plain text
            return text;
        }

        /// <summary>
        /// Highlights text for emphasis (simulated for console output).
        /// </summary>
        /// <param name="text">Text to highlight.</param>
        /// <param name="style">Style description (e.g., color, font-size).</param>
        /// <returns>Highlighted text (plain for console).</returns>
        public static string Highlight(string text, string style = "")
        {
            // In a real UI, apply highlight; for console, return plain text
            return text;
        }

        /// <summary>
        /// Dumps an enumerable collection to the console in JSON format.
        /// </summary>
        /// <typeparam name="T">Type of the collection items.</typeparam>
        /// <param name="source">Collection to dump.</param>
        /// <param name="title">Title for the output.</param>
        public static void Dump<T>(this IEnumerable<T> source, string title)
        {
            // Display title and JSON-formatted collection
            Console.WriteLine($"\n{title}:");
            Console.WriteLine(JsonConvert.SerializeObject(source, Formatting.Indented));
        }
    }
}