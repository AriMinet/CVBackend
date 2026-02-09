namespace CVBackend.Shared.Database.Seeders.Interfaces;

/// <summary>
/// Interface for database seeding operations.
/// Follows Dependency Inversion Principle (DIP) - depend on abstractions.
/// </summary>
public interface IDbSeeder
{
    /// <summary>
    /// Seeds the database with initial data if it's empty.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SeedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the database has been seeded.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if data exists, otherwise false.</returns>
    Task<bool> IsSeededAsync(CancellationToken cancellationToken = default);
}
