using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starter.Infrastructure.Persistance;
using Testcontainers.MsSql;

namespace Starter.WebApi.IntegrationTests.Facts.Factories;

public class StarterWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<StarterDbContext>));

            string connectionString = _dbContainer.GetConnectionString();
            services.AddDbContext<StarterDbContext>(options => options
                .UseSqlServer(connectionString));
        });
    }

    public HttpClient CreateAuthorizedClient()
    {
        HttpClient httpClient = CreateClient();

        // Expiration date is around 2034
        string jwt = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL3N0YXJ0ZXJ3ZWJhcGkuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdGFydGVyd2ViYXBpLmNvbSIsImV4cCI6MjA0NTUwNzU1NiwianRpIjoiN2FkYjM1M2ItMWMyMC00NGVmLWI5MjMtMDU3Mzg2ZTdhZWMyIiwic3ViIjoiNWYxYzNmMTMtZDhlZS00NGMzLTIzNjktMDhkY2Y1ZmQ1MmIxIiwiZW1haWwiOiJqb2huLmRvZUBleGFtcGxlLmNvbSJ9.0DqU6uyHcOw8mZm8CXXwv6t7PRDajU2kwqJrMMCJTEVor8WPs1iOz9Vq-2KZ6Ew3-uKmrEhdG7K2woDokBf-Rg";
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwt);

        return httpClient;
    }

    public StarterDbContext MigrateDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        StarterDbContext dbContext = scope.ServiceProvider.GetRequiredService<StarterDbContext>();
        dbContext.Database.Migrate();

        return dbContext;
    }

    public Task InitializeAsync()
        => _dbContainer.StartAsync();

    public new Task DisposeAsync()
        => _dbContainer.DisposeAsync().AsTask();
}

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true
    };
}