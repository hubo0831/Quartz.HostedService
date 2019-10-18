using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Quartz.Impl;
using Quartz.Spi;

namespace Quartz.HostedService
{
    public static class QuartzHostedServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzHostedService(this IServiceCollection services, IConfiguration config, string sectionName = "Quartz")
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.Configure<QuartzOption>(config.GetSection(sectionName));

            services.AddSingleton<IJobFactory, JobFactory>();

            services.AddSingleton(provider =>
            {
                var option = provider.GetRequiredService<IOptions<QuartzOption>>().Value;
                var sf = new StdSchedulerFactory(option.ToProperties());
                var scheduler = sf.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                return scheduler;
            });

            services.AddHostedService<QuartzHostedService>();

            return services;
        }
    }
}