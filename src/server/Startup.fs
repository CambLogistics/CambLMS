open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open WebSharper.AspNetCore
open camblms
open Microsoft.AspNetCore.HttpOverrides

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    // Add services to the container.
    builder.Services
        .AddSitelet(Site.Main)
        .AddAuthentication("WebSharper")
        .AddCookie("WebSharper", fun options -> ())
    |> ignore

    let app = builder.Build()

    // Configure the HTTP request pipeline.
    if not (app.Environment.IsDevelopment()) then
        app
            .UseExceptionHandler("/Error")
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            .UseHsts()
        |> ignore

    app
        //Prepare for reverse proxy
        .UseForwardedHeaders(
            let fho = new ForwardedHeadersOptions()
            fho.ForwardedHeaders <- ForwardedHeaders.XForwardedFor ||| ForwardedHeaders.XForwardedProto
            fho
        )
        .UseAuthentication()
        .UseStaticFiles()
        .UseWebSharper()
    |> ignore

    app.Run()

    0 // Exit code
