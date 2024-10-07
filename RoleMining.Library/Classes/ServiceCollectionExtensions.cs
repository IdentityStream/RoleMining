using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RoleMining.Library.Algorithms;
using System.Collections.Generic;

namespace RoleMining.Library.Classes
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRoleMining(this IServiceCollection services)
        {
            services.AddTransient<JaccardIndex>();
            //services.AddValidatorsFromAssemblyContaining<IValidator<IEnumerable<UserAccess>>>();
            services.AddTransient<IValidator<IEnumerable<UserAccess>>, UserAccessValidator>();
            services.AddTransient<IValidator<IEnumerable<UserInRole>>, UserInRoleValidator>();
        }
    }
}
