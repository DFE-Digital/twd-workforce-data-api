namespace WorkforceDataApi.Services;

public interface ILocalFilesystem
{
    string CreateDirectory(string path);

    Stream CreateFile(string path);
}
