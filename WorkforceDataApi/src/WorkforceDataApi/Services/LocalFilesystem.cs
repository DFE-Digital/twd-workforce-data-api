using System.Diagnostics;

namespace WorkforceDataApi.Services;

public class LocalFilesystem : ILocalFilesystem
{
    public string GetApplicationDataPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    }

    public string CreateDirectory(string path)
    {
        var fullPath = path;

        // If a relative path is specifed make it relative to this application EXE.
        if (!Path.IsPathRooted(path))
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var contentRootPath = Path.GetDirectoryName(pathToExe);
            fullPath = Path.Combine(contentRootPath, path);
        }

        var directory = Directory.CreateDirectory(fullPath);
        return directory.FullName;
    }

    public void Move(string sourceFile, string destinationFile, bool overwrite)
    {
        File.Move(sourceFile, destinationFile, overwrite);
    }
}
