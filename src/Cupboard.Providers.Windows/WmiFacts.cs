using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using Spectre.Console.Cli;

namespace Cupboard;

internal sealed class WmiFacts : IFactProvider
{
    private static readonly Dictionary<string, string> _properties;

    static WmiFacts()
    {
        _properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Version", "wmi.os.version" },
            { "WindowsDirectory", "wmi.os.windows_dir" },
            { "FreeVirtualMemory", "wmi.os.free_virtual_mem" },
            { "FreePhysicalMemory", "wmi.os.free_physical_mem" },
            { "BuildNumber", "wmi.os.build" },
            { "TotalVirtualMemorySize", "wmi.os.total_virtual_mem" },
            { "TotalVisibleMemorySize", "wmi.os.total_mem" },
        };
    }

    public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
    {
        if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            yield break;
        }

        var keys = string.Join(',', _properties.Keys);
        var searcher = new ManagementObjectSearcher($"SELECT {keys} FROM Win32_OperatingSystem");
        var osDetailsCollection = searcher.Get();
        foreach (var prop in osDetailsCollection)
        {
            foreach (var property in _properties)
            {
                yield return (property.Value, prop[property.Key]);
            }
        }

        if (osDetailsCollection.Count > 0)
        {
            yield return ("wmi.os", true);
        }
    }
}
