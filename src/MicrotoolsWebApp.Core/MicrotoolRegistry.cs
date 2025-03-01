using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MicrotoolsWebApp.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MicrotoolsWebApp.Core
{
    /// <summary>
    /// Registry for discovering and loading microtools
    /// </summary>
    public class MicrotoolRegistry
    {
        private readonly IDeserializer _yamlDeserializer;
        private readonly Dictionary<string, MicrotoolInfo> _microtools = new Dictionary<string, MicrotoolInfo>();
        private readonly string _microtoolsDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrotoolRegistry"/> class
        /// </summary>
        /// <param name="microtoolsDirectory">The directory containing the microtools</param>
        public MicrotoolRegistry(string microtoolsDirectory)
        {
            _microtoolsDirectory = microtoolsDirectory ?? throw new ArgumentNullException(nameof(microtoolsDirectory));
            _yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        /// <summary>
        /// Gets all registered microtools
        /// </summary>
        public IEnumerable<MicrotoolInfo> GetAllMicrotools() => _microtools.Values;

        /// <summary>
        /// Gets a microtool by its ID
        /// </summary>
        /// <param name="id">The ID of the microtool</param>
        /// <returns>The microtool info, or null if not found</returns>
        public MicrotoolInfo GetMicrotoolById(string id)
        {
            return _microtools.TryGetValue(id, out var microtool) ? microtool : null;
        }

        /// <summary>
        /// Discovers and loads all microtools in the specified directory
        /// </summary>
        public void DiscoverMicrotools()
        {
            if (!Directory.Exists(_microtoolsDirectory))
            {
                throw new DirectoryNotFoundException($"Microtools directory not found: {_microtoolsDirectory}");
            }

            // Get all subdirectories in the microtools directory
            var microtoolDirectories = Directory.GetDirectories(_microtoolsDirectory);

            foreach (var directory in microtoolDirectories)
            {
                try
                {
                    var metadataPath = Path.Combine(directory, "metadata.yaml");
                    var uiSchemaPath = Path.Combine(directory, "ui-schema.yaml");

                    // Skip if metadata.yaml doesn't exist
                    if (!File.Exists(metadataPath))
                    {
                        Console.WriteLine($"Skipping directory {directory}: metadata.yaml not found");
                        continue;
                    }

                    // Read and deserialize metadata.yaml
                    var metadataYaml = File.ReadAllText(metadataPath);
                    var metadata = _yamlDeserializer.Deserialize<MicrotoolMetadata>(metadataYaml);

                    // Read and deserialize ui-schema.yaml if it exists
                    UiSchema uiSchema = null;
                    if (File.Exists(uiSchemaPath))
                    {
                        var uiSchemaYaml = File.ReadAllText(uiSchemaPath);
                        uiSchema = _yamlDeserializer.Deserialize<UiSchema>(uiSchemaYaml);
                    }

                    // Create microtool info
                    var microtoolInfo = new MicrotoolInfo
                    {
                        Metadata = metadata,
                        UiSchema = uiSchema,
                        DirectoryPath = directory
                    };

                    // Add to registry
                    _microtools[metadata.Id] = microtoolInfo;
                    Console.WriteLine($"Discovered microtool: {metadata.Name} ({metadata.Id})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading microtool from {directory}: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Contains information about a microtool
    /// </summary>
    public class MicrotoolInfo
    {
        /// <summary>
        /// Gets or sets the metadata for the microtool
        /// </summary>
        public MicrotoolMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the UI schema for the microtool
        /// </summary>
        public UiSchema UiSchema { get; set; }

        /// <summary>
        /// Gets or sets the directory path where the microtool is located
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the instance of the microtool
        /// </summary>
        public IMicrotool Instance { get; set; }
    }
}
