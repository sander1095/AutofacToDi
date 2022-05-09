using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutofacToDi
{
    public abstract class BaseClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public virtual void SomeMethod()
        {
            // You could also do this CreateClient in the base() of ShopCLient if you can't change this class
            var httpClient = _httpClientFactory.CreateClient("BaseClient");

            // Do stuff

            return;
        }
    }

    public interface IShopClient
    {
        Task SomethingAsync();
    }

    public class ShopClient : BaseClient, IShopClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShopClientSettings _shopClientSettingsOptions;
        private readonly ISomeOtherDependency _someOtherDependency;

        public ShopClient(
            IHttpClientFactory httpClientFactory,
            IOptions<ShopClientSettings> shopClientSettingsOptions,
            ISomeOtherDependency someOtherDependency) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _someOtherDependency = someOtherDependency;            
            _shopClientSettingsOptions = shopClientSettingsOptions.Value;
        }

        public async Task SomethingAsync()
        {
            // Using other dependencies
            _someOtherDependency.Foo();

            var url = _shopClientSettingsOptions.Uri;

            // TODO: Now we need to pas the cookies to the client.. but watch out with cookies and httpclientfactory:
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0#cookies
            // Our HTTPClient is already created, how do we pass the cookies to it? Or do we need to do this somehow in the httpmessagehandler or something???
            var cookies = new CookieCollection();
            cookies.Add(new Cookie(CookieKeys.SessionId, _cookieService.RequestCookies.SessionId) { Domain = uri.Host });
            cookies.Add(new Cookie(CookieKeys.Geo, _cookieService.RequestCookies.Geo) { Domain = uri.Host });
                       


            // You could also create the client in the constructor. Read this for more info:
            // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory.createclient?view=dotnet-plat-ext-6.0
            var httpClient = _httpClientFactory.CreateClient("ShopClient");

            await httpClient.PostAsync(url, new StringContent("Whatever content you want here"));
        }
    }
}
