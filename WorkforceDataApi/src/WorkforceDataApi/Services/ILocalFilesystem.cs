namespace WorkforceDataApi.Services;

public interface ILocalFilesystem
{
    string GetApplicationDataPath();

    string CreateDirectory(string path);

    void Move(string sourceFile, string destinationFile, bool overwrite);
}
