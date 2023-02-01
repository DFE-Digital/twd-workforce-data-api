using WorkforceDataApi.Models;

namespace WorkforceDataApi.DevUtils.Models;

public class WorkforceData
{
    public required Teacher Teacher { get; init; }

    public required TpsExtractDataItem[] WorkforceDataItems { get; init; }
}
