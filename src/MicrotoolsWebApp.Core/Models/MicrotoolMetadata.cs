using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace MicrotoolsWebApp.Core.Models
{
    /// <summary>
    /// Represents the metadata for a microtool
    /// </summary>
    public class MicrotoolMetadata
    {
        /// <summary>
        /// Gets or sets the unique identifier for the microtool
        /// </summary>
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the microtool
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the microtool
        /// </summary>
        [YamlMember(Alias = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the version of the microtool
        /// </summary>
        [YamlMember(Alias = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the author of the microtool
        /// </summary>
        [YamlMember(Alias = "author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the tags associated with the microtool
        /// </summary>
        [YamlMember(Alias = "tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}
