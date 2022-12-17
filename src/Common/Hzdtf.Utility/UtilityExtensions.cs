using Hzdtf.Utility.AsyncCallbackWrap;
using Hzdtf.Utility.HostConfig;
using Hzdtf.Utility.InterfaceImpl;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Utility.ProcessCall;
using Hzdtf.Utility.Proxy;
using Hzdtf.Utility.RequestResource;
using Hzdtf.Utility.TheOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 辅助扩展类
    /// @ 黄振东
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// 添加默认的辅助服务
        /// </summary>
        /// <param name="service">服务收藏</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddDefaultFoxUCUtility(this IServiceCollection service)
        {
            service.AddSingleton<IAsyncReleaseInt, AsyncReleaseInt>();
            service.AddSingleton<IAsyncReleaseLong, AsyncReleaseLong>();
            service.AddSingleton<IAsyncReleaseString, AsyncReleaseString>();

            service.AddSingleton<IHostConfigReader, HostConfigJsonFile>();

            service.AddSingleton<IInterfaceMapImpl, InterfaceMapImplCache>();
            service.AddSingleton<ILocalization, LocalizationImpl>();
            service.AddSingleton<ICultureLibrary, CultureLibraryCache>();

            service.AddSingleton<IRpcServerListen, RpcServerListen>();
            service.AddSingleton<IMethodCall, MethodCallCache>();
            service.AddSingleton<IProcessCall, StaticProcessCallCache>();
            service.AddSingleton<IRequestResource, RequestResourceCache>();

            service.AddSingleton<ITheOperation, TheOperation>();

            service.AddSingleton<IIdentity<int>, IntId>();
            service.AddSingleton<IIdentity<string>, StringId>();
            service.AddSingleton<IIdentity<long>, SnowflakeId>();
            service.AddSingleton<IIdentity<Guid>, GuidId>();

            service.AddSingleton<IBusinessDispatchProxy, RpcDispatchProxyClient>();

            return service;
        }
    }
}
