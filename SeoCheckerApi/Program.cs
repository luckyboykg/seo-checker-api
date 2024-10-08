
using Application.Seo.Queries;
using FluentValidation;
using FluentValidation.AspNetCore;
using SeoCheckerApi.Filters;
using Infrastructure.Bing;
using Infrastructure.Google;
using Infrastructure.MemoryCache;
using static Application.Seo.Queries.GetSeoInfoFromGoogleRequest;
using SeoCheckerApi.Handler;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder);
ConfigureCqrs(builder);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddHttpClient();
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddMemoryCache();

    builder.Services.AddCors(o => o.AddPolicy("SeoCheckerCors", option =>
    {
        option.WithOrigins(builder.Configuration.GetValue<string>("SeoChecker:SpaUrl") ?? "SpaUrl")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    }));

    // Services
    builder.Services.AddScoped<IMemoryCacheService, MemoryCacheService>();
    builder.Services.AddScoped<IGoogleSearchService, GoogleSearchService>();
    builder.Services.AddScoped<IBingSearchService, BingSearchService>();
}

void ConfigureCqrs(WebApplicationBuilder builder)
{
    builder.Services.AddMediatR(p => p.RegisterServicesFromAssembly(typeof(GetSeoInfoFromGoogleRequest).Assembly));
    builder.Services.AddMvc(options => { options.Filters.Add(new BadRequestActionFilter()); });
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<GetSeoInfoFromGoogleRequestValidator>();
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseCors("SeoCheckerCors");

    app.UseHttpsRedirection();

    app.UseAuthorization();
}

void ConfigureEndpoints(WebApplication app)
{
    app.MapControllers();
}