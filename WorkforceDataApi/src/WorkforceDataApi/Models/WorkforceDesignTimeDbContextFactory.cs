namespace TrnGeneratorApi.Models;

using Microsoft.EntityFrameworkCore.Design;
using WorkforceDataApi.Models;

public class WorkforceDesignTimeDbContextFactory : IDesignTimeDbContextFactory<WorkforceDbContext>
{
    public WorkforceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<WorkforceDesignTimeDbContextFactory>(optional: true)  // Optional for CI
            .Build();

        return new WorkforceDbContext(configuration);
    }
}
