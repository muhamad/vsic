namespace VisualStudioInstallerCleaner.Sdk.Abstract;

/// <summary>
/// list all available package names
/// </summary>
public interface IAvailablePackageNames
{
    /// <summary>
    /// get list of available packages in input directory
    /// </summary>
    /// <param name="directoryName">directory name</param>
    /// <returns>a sequence of package names</returns>
    IEnumerable<string> GetAvailablePackageNames(string directoryName);
}
