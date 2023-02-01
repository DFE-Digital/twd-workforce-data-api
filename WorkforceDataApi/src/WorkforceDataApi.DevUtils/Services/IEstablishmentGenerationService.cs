using WorkforceDataApi.DevUtils.Models;

namespace WorkforceDataApi.DevUtils.Services;

public interface IEstablishmentGenerationService
{
    void Initialise(string sourceFilename);

    EstablishmentSummary Generate();
}
