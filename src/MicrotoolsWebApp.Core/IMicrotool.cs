using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicrotoolsWebApp.Core
{
    /// <summary>
    /// Interface that all microtools must implement
    /// </summary>
    public interface IMicrotool
    {
        /// <summary>
        /// Gets the unique identifier for the microtool
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the display name of the microtool
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the microtool
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Executes the microtool with the provided parameters
        /// </summary>
        /// <param name="parameters">Dictionary of parameter names and values</param>
        /// <returns>The result of the execution</returns>
        Task<object> ExecuteAsync(IDictionary<string, object> parameters);
    }
}
