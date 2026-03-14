using Microsoft.Extensions.DependencyInjection;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Application.Services;

namespace Reply.ContactManagement.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContactManagementApplication(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IContactBulkMergeService, ContactBulkMergeService>();
        services.AddScoped<ICustomFieldService, CustomFieldService>();

        return services;
    }
}
