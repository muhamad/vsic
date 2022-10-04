namespace VisualStudioInstallerCleaner.Sdk.Common;

/// <summary>
/// package information
/// </summary>
public sealed class PackageInfo
{
    /// <summary>
    /// the package full name
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// the package name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// the package version
    /// </summary>
    public Version Version { get; set;}

    /// <summary>
    /// the architecture to deploy on
    /// </summary>
    public ProcessorArchitecture ProductArchitecture { get; set; } = ProcessorArchitecture.Neutral;

    /// <summary>
    /// the architecture to develop on
    /// </summary>
    public ProcessorArchitecture MachineArchitecture { get; set; } = ProcessorArchitecture.Neutral;

    /// <summary>
    /// the package language
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// other unknown parts from the name
    /// </summary>
    public IReadOnlyList<string> UnknownParts { get; set; }

    /// <summary>
    /// Determine whether this package is not correct
    /// </summary>
    /// <returns>true for package with invalid information; false otherwise</returns>
    public bool IsInvalid()
        => string.IsNullOrWhiteSpace(Name) || Version == null;
}
