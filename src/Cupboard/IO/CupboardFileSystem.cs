using Spectre.IO;

namespace Cupboard.Internal;

internal sealed class CupboardFileSystem : ICupboardFileSystem
{
    private readonly IFileSystem _fileSystem;

    public IFileProvider File => _fileSystem.File;
    public IDirectoryProvider Directory => _fileSystem.Directory;

    public CupboardFileSystem()
    {
        _fileSystem = new FileSystem();
    }
}