using System;

namespace Cupboard
{
    public sealed class WingetPackage : Resource
    {
        public string Package { get; set; }
        public PackageState Ensure { get; set; }
        public bool? IgnoreChecksum { get; set; }
        public string? PackageVersion { get; set; }

        public WingetPackage(string name)
            : base(name)
        {
            Package = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
