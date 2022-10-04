using System.Linq;
using System.Threading.Tasks;
using VisualStudioInstallerCleaner.Sdk.Abstract;
using VisualStudioInstallerCleaner.Sdk.Common;

namespace VisualStudioInstallerCleaner.Sdk.Services;

/// <summary>
/// An implementation for <see cref="ICleanerEngine"/>
/// </summary>
public class CleanerEngine : ICleanerEngine
{
    private readonly IAvailablePackageNames availablePackages;
    private readonly IParsePackageName parsePackageName;

    private readonly CleanerEngineOptions defaultOptions = new();

    /// <summary>
    /// Initialize new instance
    /// </summary>
    /// <param name="availablePackages">an <see cref="IAvailablePackageNames"/> object instance</param>
    /// <param name="parsePackageName">an <see cref="IParsePackageName"/> object instance</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CleanerEngine(IAvailablePackageNames availablePackages, IParsePackageName parsePackageName)
    {
        this.availablePackages = availablePackages ?? throw new ArgumentNullException(nameof(availablePackages));
        this.parsePackageName = parsePackageName ?? throw new ArgumentNullException(nameof(parsePackageName));
    }

    /// <inheritdoc />
    public void Execute(string installerSource)
        => Execute(installerSource, defaultOptions);

    /// <inheritdoc />
    public void Execute(string installerSource, CleanerEngineOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (!Directory.Exists(installerSource))
            throw new InvalidOperationException("installer source not exist.");

        var names = availablePackages.GetAvailablePackageNames(installerSource).ToList();
        OnAvailablePackageNames(names, options);

        var packages = names.Select(parsePackageName.Parse).ToList();
        OnReadyPackagesInfo(packages, options);

        var lookup = packages.ToLookup(e => e.IsInvalid());
        OnInvalidPackages(lookup[true], options);

        var deprecatedPackages = GetDeprecatedPackages(lookup[false]);
        OnDeprecatedPackages(deprecatedPackages, options);
    }

    /// <summary>
    /// Execute <see cref="CleanerEngineOptions.OnAvailablePackageNames"/> callback
    /// </summary>
    /// <param name="packageNames">available package names</param>
    /// <param name="options"><see cref="CleanerEngineOptions"/> object instance</param>
    protected virtual void OnAvailablePackageNames(IEnumerable<string> packageNames, CleanerEngineOptions options)
        => Task.Factory.StartNew(() => options.OnAvailablePackageNames(packageNames));

    /// <summary>
    /// Execute <see cref="CleanerEngineOptions.OnReadyPackagesInfo"/> callback
    /// </summary>
    /// <param name="allPackages">a sequence of all available package information</param>
    /// <param name="options"><see cref="CleanerEngineOptions"/> object instance</param>
    protected virtual void OnReadyPackagesInfo(IEnumerable<PackageInfo> allPackages, CleanerEngineOptions options)
        => Task.Factory.StartNew(() => options.OnReadyPackagesInfo(allPackages));

    /// <summary>
    /// Execute <see cref="CleanerEngineOptions.OnInvalidPackage"/> callback
    /// </summary>
    /// <param name="invalidPackages">a sequence of all invalid packages</param>
    /// <param name="options"><see cref="CleanerEngineOptions"/> object instance</param>
    protected virtual void OnInvalidPackages(IEnumerable<PackageInfo> invalidPackages, CleanerEngineOptions options)
    {
        Task.Factory.StartNew(() =>
        {
            var invalid = invalidPackages.ToList();

            if (invalid.Count != 0)
                options.OnInvalidPackage(invalid);
        });
    }

    /// <summary>
    /// Execute <see cref="CleanerEngineOptions.OnDeprecatedPackages"/> callback
    /// </summary>
    /// <param name="deprecatedPackages">a sequence of all deprecated packages</param>
    /// <param name="options"><see cref="CleanerEngineOptions"/> object instance</param>
    protected virtual void OnDeprecatedPackages(IEnumerable<PackageInfo> deprecatedPackages, CleanerEngineOptions options)
        => options.OnDeprecatedPackages(deprecatedPackages);

    /// <summary>
    /// Filter input packages and return all deprecated packages
    /// </summary>
    /// <param name="validPackages">a sequence of all valid packages</param>
    /// <returns>a sequence of deprecated packages</returns>
    protected virtual IEnumerable<PackageInfo> GetDeprecatedPackages(IEnumerable<PackageInfo> validPackages)
    {
        return validPackages
            .GroupBy(e => new {e.Name, e.Language, e.MachineArchitecture, e.ProductArchitecture})
            .Select(e => e.OrderByDescending(p => p.Version).ToList())
            .Where(e => e.Count != 1)
            .SelectMany(e => e.Skip(1));
    }
}
