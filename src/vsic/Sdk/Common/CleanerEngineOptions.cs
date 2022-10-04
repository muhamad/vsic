namespace VisualStudioInstallerCleaner.Sdk.Common;

/// <summary>
/// Represent cleaner engine options
/// </summary>
public sealed class CleanerEngineOptions
{
    /// <summary>
    /// Invoked when we get all available package names
    /// </summary>
    public Action<IEnumerable<string>> OnAvailablePackageNames { get; init; } = None;

    /// <summary>
    /// Invoked when the package names parsed into into <see cref="PackageInfo"/>
    /// </summary>
    public Action<IEnumerable<PackageInfo>> OnReadyPackagesInfo { get; init; } = None;

    /// <summary>
    /// Invoked when one or more invalid packages exists
    /// </summary>
    public Action<IEnumerable<PackageInfo>> OnInvalidPackage { get; init; } = None;

    /// <summary>
    /// Invoked with a sequence of deprecated packages for specific package
    /// </summary>
    public Action<IEnumerable<PackageInfo>> OnDeprecatedPackages { get; init; } = None;

    private static void None(IEnumerable<string> arg) {}

    private static void None(IEnumerable<PackageInfo> arg) {}
}
