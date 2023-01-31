namespace WorkforceDataApi.Csv;

public interface ITpsCsvProcessor
{
    Task Process(string fileName);
}
