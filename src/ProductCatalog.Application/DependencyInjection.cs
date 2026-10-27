using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Auth;
using ProductCatalog.Application.Auth.Commands;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Models;
using ProductCatalog.Application.Products;
using ProductCatalog.Application.Products.Commands;
using ProductCatalog.Application.Products.Queries;

namespace ProductCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<ICommandHandler<CreateProductCommand, ProductResponse>, CreateProductCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateProductCommand>, UpdateProductCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteProductCommand>, DeleteProductCommandHandler>();
        services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductResponse>, GetProductByIdQueryHandler>();
        services.AddScoped<IQueryHandler<SearchProductsQuery, PagedResult<ProductResponse>>, SearchProductsQueryHandler>();

        services.AddScoped<ICommandHandler<RegisterUserCommand, AuthResponse>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginUserCommand, AuthResponse>, LoginUserCommandHandler>();

        return services;
    }
}
