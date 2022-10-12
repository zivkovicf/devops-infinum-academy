using FluentValidation;
using IA.DevOps.Movies.Common.Validators;
using IA.DevOps.Movies.Contracts.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace IA.DevOps.Movies.Common
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterFluentValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<MovieForm>, MovieValidator>();

            return services;
        }
    }
}
