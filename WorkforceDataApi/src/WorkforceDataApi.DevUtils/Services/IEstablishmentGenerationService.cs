using WorkforceDataApi.DevUtils.Models;

namespace WorkforceDataApi.DevUtils.Services;

public interface IEstablishmentGenerationService
{
    EstablishmentSummary Generate();
}
