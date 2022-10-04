using VisualStudioInstallerCleaner.Sdk.Common;

namespace VisualStudioInstallerCleaner.Sdk.Abstract;

/// <summary>
/// The engine responsible for cleaning the visual studio installer directory from duplicates
/// </summary>
public interface ICleanerEngine
{
    /// <summary>
    /// execute the engine on the provided installer source
    /// </summary>
    /// <param name="installerSource">the visual studio installer source</param>
    void Execute(string installerSource);

    /// <summary>
    /// execute the engine on the provided installer source and options
    /// </summary>
    /// <param name="installerSource">the visual studio installer source</param>
    /// <param name="options">cleaner engine options</param>
    void Execute(string installerSource, CleanerEngineOptions options);
}
