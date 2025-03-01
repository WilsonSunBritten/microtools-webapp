using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MicrotoolsWebApp.Core;

namespace TextTransformer
{
    public class TextTransformerTool : IMicrotool
    {
        public string Id => "text-transformer";

        public string Name => "Text Transformer";

        public string Description => "A simple tool to transform text to different cases (uppercase, lowercase, title case)";

        public Task<object> ExecuteAsync(IDictionary<string, object> parameters)
        {
            Console.WriteLine("Simple log test: " + parameters.ToString());
            
            // Log all parameters
            foreach (var param in parameters)
            {
                Console.WriteLine($"Parameter: {param.Key} = {param.Value}");
            }
            // Get the input text
            if (!parameters.TryGetValue("inputText", out var inputTextObj) || inputTextObj == null)
            {
                throw new ArgumentException("Input text is required");
            }

            string inputText = inputTextObj.ToString();

            // Get the transformation type
            if (!parameters.TryGetValue("transformationType", out var transformationTypeObj) || transformationTypeObj == null)
            {
                Console.WriteLine("transformationType parameter is missing or null");
                transformationTypeObj = "uppercase"; // Default to uppercase if not provided
            }

            string transformationType = transformationTypeObj.ToString().ToLower();
            Console.WriteLine($"Using transformation type: {transformationType}");

            // Transform the text
            string result = transformationType switch
            {
                "uppercase" => inputText.ToUpper(),
                "lowercase" => inputText.ToLower(),
                "titlecase" => ToTitleCase(inputText),
                _ => throw new ArgumentException($"Unknown transformation type: {transformationType}")
            };

            // Return the result
            return Task.FromResult<object>(new
            {
                TransformedText = result
            });
        }

        private string ToTitleCase(string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }
    }
}
