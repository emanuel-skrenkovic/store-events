using System.Text.Json.Serialization;
using EventStore.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Store.Catalogue.Application.Product.Command.Create;
using Store.Catalogue.Application.Product.Projections.ProductDisplay;
using Store.Catalogue.Domain.Product;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure;
using Store.Core.Infrastructure.EntityFramework;
using Store.Core.Infrastructure.EventStore;

namespace Store.CatalogueManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.IgnoreNullValues = true;
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Store.CatalogueManagement", Version = "v1" }); });

            services.AddMediatR(typeof(ProductCreateCommand));

            services.AddSingleton(_ => new EventStoreClient(EventStoreClientSettings.Create(Configuration["EventStore:ConnectionString"])));
            
            services.AddScoped<IAggregateRepository, EventStoreAggregateRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddSingleton<ISerializer, JsonSerializer>();
            
            services.AddDbContext<StoreCatalogueDbContext>(
                options => options.UseNpgsql(Configuration["Postgres:ConnectionString"], b => b.MigrationsAssembly("Store.Catalogue.Infrastructure")));

            services.AddSingleton(_ => new EventStoreSubscriptionConfiguration
            {
                SubscriptionId = "projections"
            });

            services.AddSingleton<ICheckpointRepository, EventStoreCheckpointRepository>();
            services.AddSingleton<IProjectionManager, ProductDisplayProjectionManager>();
            services.AddSingleton<IProjection<ProductDisplayEntity>, ProductDisplayProjection>();
            services.AddSingleton<IProjectionRunner<ProductDisplayEntity>, EfProjectionRunner<ProductDisplayEntity, StoreCatalogueDbContext>>();
            services.AddHostedService<EventStoreSubscriptionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store.CatalogueManagement v1"));
            }

            app.UseHttpsRedirection();

            app.UsePathBase(new PathString("/catalogue"));
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}