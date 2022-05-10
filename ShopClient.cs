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
            // You could also do this CreateClient in the base() of ShopCLient if you can't change this class, but then it shouldnt be a singleton!
            using var httpClient = _httpClientFactory.CreateClient("BaseClient");

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

            // Note: You could also do this inprogram.cs with AddHTtpCLient.. You can inject the shoplcientsettingoptions there and set the default uri!
            var url = _shopClientSettingsOptions.Uri;


            // @Ding: You posted this custom cookie code. I am wondering if you need this; if you only are adding cookies to the request that come from the ORIGINAL request,
            // you might be able to get away with using Program.cs and the header propogation logic. Otherwise I think you should use HttpRequestMessage and Headers.Add("Cookie", YOUR_COOKIES_AS_STRING_HERE)

            //var cookies = new CookieCollection();
            //cookies.Add(new Cookie(CookieKeys.SessionId, _cookieService.RequestCookies.SessionId) { Domain = uri.Host });
            //cookies.Add(new Cookie(CookieKeys.Geo, _cookieService.RequestCookies.Geo) { Domain = uri.Host });
                       

            // You could also create the client in the constructor. But then this class should not be a singleton because it wouldnt get disposed! Read this for more info:
            // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory.createclient?view=dotnet-plat-ext-6.0
            using var httpClient = _httpClientFactory.CreateClient("ShopClient");

            await httpClient.PostAsync(url, new StringContent("Whatever content you want here"));
        }
    }
}
