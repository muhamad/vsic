using VisualStudioInstallerCleaner.Sdk.Common;

namespace VisualStudioInstallerCleaner.Sdk.Abstract;

/// <summary>
/// decompose package name
/// </summary>
public interface IParsePackageName
{
    /// <summary>
    /// parse package name and store the result into <see cref="PackageInfo"/> instance
    /// </summary>
    /// <param name="packageName">a package name</param>
    /// <returns><see cref="PackageInfo"/> instance; null for invalid package format</returns>
    PackageInfo Parse(string packageName);
}
