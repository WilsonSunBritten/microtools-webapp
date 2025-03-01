using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace MicrotoolsWebApp.Core.Models
{
    /// <summary>
    /// Represents the UI schema for a microtool
    /// </summary>
    public class UiSchema
    {
        /// <summary>
        /// Gets or sets the components that make up the UI
        /// </summary>
        [YamlMember(Alias = "components")]
        public List<UiComponent> Components { get; set; } = new List<UiComponent>();
    }

    /// <summary>
    /// Represents a UI component in the schema
    /// </summary>
    public class UiComponent
    {
        /// <summary>
        /// Gets or sets the type of the component
        /// </summary>
        [YamlMember(Alias = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the component
        /// </summary>
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the label for the component
        /// </summary>
        [YamlMember(Alias = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the placeholder text for input components
        /// </summary>
        [YamlMember(Alias = "placeholder")]
        public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets whether the component is required
        /// </summary>
        [YamlMember(Alias = "required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the default value for the component
        /// </summary>
        [YamlMember(Alias = "defaultValue")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets additional options for the component
        /// </summary>
        [YamlMember(Alias = "options")]
        public Dictionary<string, object> Options { get; set; } = new Dictionary<string, object>();
    }
}
