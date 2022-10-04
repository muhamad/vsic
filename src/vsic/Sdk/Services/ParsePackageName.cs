using VisualStudioInstallerCleaner.Sdk.Abstract;
using VisualStudioInstallerCleaner.Sdk.Common;

namespace VisualStudioInstallerCleaner.Sdk.Services;

using ParserHandler = Func<string, PackageInfo, bool>;

/// <summary>
/// an implementation for <see cref="IParsePackageName"/>
/// </summary>
public class ParsePackageName : IParsePackageName
{
    // the chip is same as machine-arch
    private static readonly string[] KnownArch = {"chip=", "machinearch=", "productarch="};
    private const int MachineArch = 0;
    private const int ProductArch = 1;

    /// <inheritdoc />
    public PackageInfo Parse(string packageName)
    {
        if (packageName == null)
            throw new ArgumentNullException(nameof(packageName));

        var packageInfo = new PackageInfo {FullName = packageName};
        var unknown = new List<string>();

        foreach (var part in SplitPackageName(packageName))
        {
            if (!RunHandlers(part, packageInfo))
                unknown.Add(part);
        }

        packageInfo.UnknownParts = unknown;

        return packageInfo;
    }

    /// <summary>
    /// split package name into parts
    /// </summary>
    /// <param name="packageName">package name</param>
    /// <returns>a list of parts that compose package name</returns>
    protected virtual string[] SplitPackageName(string packageName)
        => packageName.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Get the handlers which parse the package name parts
    /// </summary>
    /// <returns>an array of handlers</returns>
    protected virtual IEnumerable<ParserHandler> GetHandlers()
    {
        yield return ParseName;
        yield return ParseVersion;
        yield return ParseLanguage;
        yield return ParseArchitecture;
    }

    /// <summary>
    /// execute handlers for input value, then update package info if handler value is handled
    /// </summary>
    /// <param name="value">value to parse</param>
    /// <param name="packageInfo">package name to update after parsing the value</param>
    /// <returns>true if value is correctly handled; false if no handler available to parse it</returns>
    protected virtual bool RunHandlers(string value, PackageInfo packageInfo)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (packageInfo == null)
            throw new ArgumentNullException(nameof(packageInfo));

        foreach (var handler in GetHandlers())
        {
            if (handler == null)
                throw new NullReferenceException("a handler returned from GetHandlers can not be null.");

            if (OnHandlerExecuting(handler, value, packageInfo))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Execute handler with input values
    /// </summary>
    /// <param name="handler">handler to execute</param>
    /// <param name="value">package name part</param>
    /// <param name="package">package object to update</param>
    /// <returns>handler result</returns>
    protected virtual bool OnHandlerExecuting(ParserHandler handler, string value, PackageInfo package)
        => handler(value, package);

    /// <summary>
    /// A handler which parse package name part
    /// </summary>
    /// <param name="value">package name part to parse</param>
    /// <param name="packageInfo">the package object to update</param>
    /// <returns>true if section parsed correctly, false if section is a valid for this handler but have errors, null if handler cannot parse this section</returns>
    protected static bool ParseName(string value, PackageInfo packageInfo)
    {
        if (value.Contains("="))
            return false;

        packageInfo.Name = value;
        
        return true;
    }

    /// <summary>
    /// A handler which parse package version part
    /// </summary>
    /// <param name="value">package name part to parse</param>
    /// <param name="packageInfo">the package object to update</param>
    /// <returns>true if section parsed correctly, false if section is a valid for this handler but have errors, null if handler cannot parse this section</returns>
    protected static bool ParseVersion(string value, PackageInfo packageInfo)
    {
        if (!value.StartsWith("version="))
            return false;

        var parts = value.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            return false;

        if (Version.TryParse(parts[1], out var version))
        {
            packageInfo.Version = version;
            return true;
        }

        parts = value.Split('.');

        if (parts.Length > 4) return false;

        for (var i = 0; i < parts.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parts[i]))
                parts[i] = "0";
            else
                parts[i] = parts[i].Trim();
        }

        if (Version.TryParse(string.Join(".", parts), out version))
        {
            packageInfo.Version = version;
            return true;
        }

        return false;
    }

    /// <summary>
    /// A handler which parse language part
    /// </summary>
    /// <param name="value">package name part to parse</param>
    /// <param name="packageInfo">the package object to update</param>
    /// <returns>true if section parsed correctly, false if section is a valid for this handler but have errors, null if handler cannot parse this section</returns>
    protected static bool ParseLanguage(string value, PackageInfo packageInfo)
    {
        if (!value.StartsWith("language="))
            return false;

        packageInfo.Language = value.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries)[1];
        return true;
    }

    /// <summary>
    /// A handler which parse the architecture
    /// </summary>
    /// <param name="value">package name part to parse</param>
    /// <param name="packageInfo">the package object to update</param>
    /// <returns>true if section parsed correctly, false if section is a valid for this handler but have errors, null if handler cannot parse this section</returns>
    protected static bool ParseArchitecture(string value, PackageInfo packageInfo)
    {
        var targetArch = -1;

        for (var i = 0; i < KnownArch.Length; ++i)
        {
            if (value.StartsWith(KnownArch[i]))
            {
                targetArch = i == 2 ? ProductArch : MachineArch;
                break;
            }
        }

        if (targetArch == -1)
            return false;

        var parts = value.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            return false;

        var arch = parts[1].ToLower() switch
        {
            "x64" => ProcessorArchitecture.X64,
            "x86" => ProcessorArchitecture.X86,
            "msil" => ProcessorArchitecture.Msil,
            "ia64" => ProcessorArchitecture.IA64,
            "neutral" => ProcessorArchitecture.Neutral,
            _ => ProcessorArchitecture.Neutral
        };

        if (targetArch == MachineArch)
            packageInfo.MachineArchitecture = arch;
        else
            packageInfo.ProductArchitecture = arch;

        return true;
    }
}
