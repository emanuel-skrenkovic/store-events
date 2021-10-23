using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EventStore.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Store.Catalogue.Application.Product.Command.Create;
using Store.Catalogue.Domain.Product;
using Store.Core.Domain;
using Store.Core.Infrastructure;

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

            services.AddScoped<EventStoreClient>(
                _ => new EventStoreClient(EventStoreClientSettings.Create(Configuration["EventStore:ConnectionString"])));
            
            services.AddScoped<IRepository, EventStoreRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ISerializer, JsonSerializer>();
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