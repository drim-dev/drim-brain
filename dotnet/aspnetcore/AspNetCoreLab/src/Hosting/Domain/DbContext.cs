namespace Domain;

public class DbContext
{
    public Task SaveChanges(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}