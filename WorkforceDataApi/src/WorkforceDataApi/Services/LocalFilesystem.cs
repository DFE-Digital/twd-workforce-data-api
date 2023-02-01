using System.Diagnostics;

namespace WorkforceDataApi.Services;

public class LocalFilesystem : ILocalFilesystem
{
    private const string WorkforceDataFolderName = "workforce-data";

    public string GetWorkforceApplicationDataPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            WorkforceDataFolderName);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
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
