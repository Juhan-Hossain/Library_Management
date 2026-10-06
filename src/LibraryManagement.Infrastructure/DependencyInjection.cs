using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Persistence.Interceptors;
using LibraryManagement.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LibraryDb")
            ?? throw new InvalidOperationException("Connection string 'LibraryDb' is not configured.");

        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<LibraryDbContext>((sp, options) => options
            .UseSqlServer(connectionString)
            .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));

        // One scoped DbContext instance serves both CQRS sides within a request.
        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<LibraryDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<LibraryDbContext>());

        services.AddScoped<ILibraryRepository, LibraryRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();

        return services;
    }
}