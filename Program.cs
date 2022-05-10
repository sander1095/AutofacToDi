using AutofacToDi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<ISomeOtherDependency, SomeOtherDependency>();


// Add the settings
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0#use-di-services-to-configure-options
builder.Services.AddOptions<ShopClientSettings>().Configure((ShopClientSettings settings, IConfiguration configuration) => settings.Uri = configuration["ShopClientSettings:Uri"]);


// To ensure the cookie from the FE is forwarded to the requests made by the ShopClient, we use header propogation:
// Please read and understand this:
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0#header-propagation-middleware-2
// https://stackoverflow.com/a/64157842/3013479
// https://stackoverflow.com/a/62696725/3013479
// Also, be careful and read https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0#cookies and related subjects on httpmessagehandler and cookies being shared!
// It is important to understand htis, because otherwise you might send cookies from previous requests to the NEXT request, possibly leaking PII!


// Note: THIS MIGHT NOT BE REQUIRED; I HAVE NOT TESTED IT.
// If you can disable the code and the ShopClient HttpClient still contains the FE cookie, then you can remove this code!
builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add("Cookie", (headerOptions) =>
    {
        // Note: You will still need to tweak this
        // You can further configure the Cookie header here if you want
        // https://stackoverflow.com/a/62696725/3013479
        return new Microsoft.Extensions.Primitives.StringValues();

    });
});

// We want to forward the Cookie from the incoming request to requests that are made with this HTTPClient so we use header propogation
builder.Services.AddHttpClient("ShopClient", x => { /* Configure the HTTP Client for the ShopClient here. */ })
    .AddHeaderPropagation(x => x.Headers.Add("Cookie"));
    // You could also use stuff like ConfigurePrimaryHttpMessageHandler and inject a cookie service or whatever, but I will let you figure that out on your own



builder.Services.AddHttpClient("BaseClient", x => { /* Configure the HTTP Client for the BaseClient here */ });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseHeaderPropagation();

app.MapRazorPages();

app.Run();
