using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace MicrotoolsWebApp.Core
{
    /// <summary>
    /// Responsible for loading microtool assemblies and creating instances
    /// </summary>
    public class MicrotoolLoader
    {
        /// <summary>
        /// Loads a microtool from an assembly file
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file</param>
        /// <returns>An instance of the microtool, or null if not found</returns>
        public IMicrotool LoadMicrotool(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException($"Assembly file not found: {assemblyPath}");
            }

            try
            {
                // Load the assembly
                var assemblyLoadContext = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(assemblyPath), true);
                var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblyPath);

                // Find types that implement IMicrotool
                var microtoolType = assembly.GetTypes()
                    .FirstOrDefault(t => typeof(IMicrotool).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                if (microtoolType == null)
                {
                    throw new InvalidOperationException($"No implementation of IMicrotool found in assembly: {assemblyPath}");
                }

                // Create an instance of the microtool
                var microtool = (IMicrotool)Activator.CreateInstance(microtoolType);
                return microtool;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading microtool from {assemblyPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads a microtool from a directory containing a compiled assembly
        /// </summary>
        /// <param name="directory">The directory containing the microtool</param>
        /// <param name="microtoolId">The ID of the microtool</param>
        /// <returns>An instance of the microtool, or null if not found</returns>
        public IMicrotool LoadMicrotoolFromDirectory(string directory, string microtoolId)
        {
            // Look for a DLL with the same name as the microtool ID
            var dllPath = Path.Combine(directory, $"{microtoolId}.dll");
            
            if (File.Exists(dllPath))
            {
                return LoadMicrotool(dllPath);
            }

            // If not found, look for any DLL in the directory
            var dllFiles = Directory.GetFiles(directory, "*.dll");
            
            if (dllFiles.Length > 0)
            {
                return LoadMicrotool(dllFiles[0]);
            }

            throw new FileNotFoundException($"No DLL found in directory: {directory}");
        }
    }
}
