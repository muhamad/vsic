using System.Linq;
using VisualStudioInstallerCleaner.Sdk.Abstract;

namespace VisualStudioInstallerCleaner.Sdk.Services;

/// <summary>
/// an implementation for <see cref="IAvailablePackageNames"/>
/// </summary>
public class AvailablePackageNames : IAvailablePackageNames
{
    /// <inheritdoc />
    public IEnumerable<string> GetAvailablePackageNames(string directoryName)
    {
        if (directoryName == null)
            throw new NullReferenceException(nameof(directoryName));

        return GetPackagesFromDirectory(directoryName);
    }

    /// <summary>
    /// Get all directory names in input directory name
    /// </summary>
    /// <param name="directoryName">directory name</param>
    /// <returns>a sequence of first level child directory names without path</returns>
    protected virtual IEnumerable<string> GetPackagesFromDirectory(string directoryName)
        => Directory.EnumerateDirectories(directoryName).Select(Path.GetFileName);
}
