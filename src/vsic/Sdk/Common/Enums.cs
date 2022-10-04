namespace VisualStudioInstallerCleaner.Sdk.Common;

/// <summary>
/// the processor architecture used by visual studio installer
/// </summary>
public enum ProcessorArchitecture
{
    /// <summary>
    /// neutral code
    /// </summary>
    Neutral,

    /// <summary>
    /// x86 ISA
    /// </summary>
    X86,

    /// <summary>
    /// x64 ISA
    /// </summary>
    X64,

    /// <summary>
    /// CIL byte code
    /// </summary>
    Msil,

    /// <summary>
    /// itanium ISA
    /// </summary>
    IA64
}
