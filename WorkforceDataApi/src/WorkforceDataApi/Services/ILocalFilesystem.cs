namespace WorkforceDataApi.Services;

public interface ILocalFilesystem
{
    string GetWorkforceApplicationDataPath();

    bool FileExists(string path);

    string CreateDirectory(string path);

    void Move(string sourceFile, string destinationFile, bool overwrite);
}
