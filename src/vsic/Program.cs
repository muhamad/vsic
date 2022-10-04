using System.Linq;
using VisualStudioInstallerCleaner.Sdk.Common;
using VisualStudioInstallerCleaner.Sdk.Services;

namespace VisualStudioInstallerCleaner;

/// <summary>
/// Entry point for the application
/// </summary>
public class Program
{
    /// <summary>
    /// The main function
    /// </summary>
    /// <param name="args">application arguments</param>
    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            PrintHelp();
            return;
        }

        if (!Directory.Exists(args[0]) || !Directory.Exists(args[1]))
        {
            Console.WriteLine("both input directories must exists.");
            return;
        }

        var engine = new CleanerEngine(new AvailablePackageNames(), new ParsePackageName());
        var options = new CleanerEngineOptions
        {
            OnAvailablePackageNames = e => Console.WriteLine("Found Existing: {0} Packages", e.Count()),
            OnInvalidPackage = e => Console.WriteLine("Found Invalid: {0} Packages", e.Count()),
            OnDeprecatedPackages = e => OnDeprecatedPackages(args[0], args[1], e)
        };

        engine.Execute(args[0], options);
    }

    private static void OnDeprecatedPackages(string sourceDir, string destDir, IEnumerable<PackageInfo> packages)
    {
        var all = packages.ToList();

        if (all.Count == 0)
        {
            Console.WriteLine("No deprecated packages found.");
            return;
        }

        Console.WriteLine("Found Deprecated: {0} Packages", all.Count);
        Console.Write("Moving packages: ");

        var (row, column) = (Console.CursorTop, Console.CursorLeft);

        for (var i = 0; i < all.Count; ++i)
        {
            Console.SetCursorPosition(column, row);
            Console.Write($"{i + 1} of {all.Count}");
            Directory.Move(Path.Combine(sourceDir, all[i].FullName), Path.Combine(destDir, all[i].FullName));
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("By Muhammad Aladdin https://github.com/muhamad/vsic");
        Console.WriteLine("\nThis tool work with Visual Studio Installer 2017, 2019 and 2022.");
        Console.WriteLine("The target is to reduce installer packages cache by safely moving deprecated packages out of the cache, when a package exist for same language and architecture multiple times, we keep the one with highest version and move out the other ones to another directory.");
        Console.WriteLine("You can run the download option again on the layout directory to see that nothing is removed.");
        Console.WriteLine("\nusage:");
        Console.WriteLine("    vsic <vs-installer-source> <existing-dir-to-move-deprecated-packages>");
        Console.WriteLine("\nplease make sure that <existing-dir-to-move-deprecated-packages> is empty before starting the process");
    }
}
