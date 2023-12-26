using LoanService.Api;
using LoanService.Database;
using Microsoft.EntityFrameworkCore;

namespace LoanService;

public static class Endpoints
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/offerings", async (LoanServiceDbContext db, CancellationToken cancellationToken) => await db.LoanOfferings
            .OrderBy(o => o.Name)
            .Select(o => new LoanOfferingDto(o.Name, o.Months, o.InterestRate, o.MaxAmount))
            .ToArrayAsync(cancellationToken));

        return app;
    }
}
