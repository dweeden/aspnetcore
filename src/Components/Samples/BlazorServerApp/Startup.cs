// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BlazorServerApp.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace BlazorServerApp;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<WeatherForecastService>();

        // added by dweeden
        services.AddHttpContextAccessor();
        services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICorsService corsService,
        ICorsPolicyProvider corsPolicyProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UsePathBase("/XYZ");

        app.UseHttpsRedirection();

        // added by dweeden
        app.UseAntiforgery();
        app.UseCors("CorsPolicy");
        app.UseStaticFiles();
        // new StaticFileOptions
        // {
        //     //RequestPath = "/XYZ",
        //     ServeUnknownFileTypes = true,
        //     OnPrepareResponse = (ctx) =>
        //     {
        //         var policy = corsPolicyProvider
        //             .GetPolicyAsync(ctx.Context, "CorsPolicy")
        //             .ConfigureAwait(false)
        //             .GetAwaiter().GetResult();
        //
        //         var corsResult = corsService.EvaluatePolicy(ctx.Context, policy!);
        //
        //         corsService.ApplyResult(corsResult, ctx.Context.Response);
        //     }
        // });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}

