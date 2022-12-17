using Hzdtf.Utility.AspNet.Extensions.RoutePermission;
using Hzdtf.Utility.RoutePermission;
using Hzdtf.Utility.UserPermission;
using Hzdtf.Utility.UserPermission.Merchant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 路由权限扩展类
    /// @ 黄振东
    /// </summary>
    public static class RoutePermissionServiceExtensions
    {
        /// <summary>
        /// 添加路由权限服务
        /// </summary>
        /// <param name="services">服务收藏</param>
        /// <param name="options">路由权限选项配置</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddRoutePermission(this IServiceCollection services, Action<RoutePermissionOptions> options = null)
        {
            var routePermissionOptions = new RoutePermissionOptions();
            if (options != null)
            {
                options(routePermissionOptions);
            }

            services.AddSingleton<Microsoft.Extensions.Options.IOptions<RoutePermissionOptions>>(provider =>
            {
                return Microsoft.Extensions.Options.Options.Create<RoutePermissionOptions>(routePermissionOptions);
            });
            if (routePermissionOptions.ConfigReader == null)
            {
                services.AddSingleton<IRoutePermissionConfigReader, RoutePermissionJson>();
            }
            else
            {
                services.AddSingleton(typeof(IRoutePermissionConfigReader), routePermissionOptions.ConfigReader);
            }
            services.AddSingleton<IRoutePermissionReader, RoutePermissionCache>();

            return services;
        }

        /// <summary>
        /// 添加用户路由权限服务
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <typeparam name="UserPermissionImplT">用户权限实现类型</typeparam>
        /// <param name="services">服务收藏</param>
        /// <param name="options">路由权限选项配置</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddUserRoutePermission<IdT, UserPermissionImplT>(this IServiceCollection services, Action<RoutePermissionOptions> options = null)
            where UserPermissionImplT : class, IUserMenuReader<IdT>
        {
            services.AddRoutePermission(options);

            services.AddSingleton<IUserMenuPermission<IdT>, UserMenuLocalCache<IdT>>();
            services.AddSingleton<IUserMenuReader<IdT>, UserPermissionImplT>();

            return services;
        }

        /// <summary>
        /// 添加商户用户路由权限服务
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <typeparam name="MerchantUserPermissionImplT">商户用户权限实现类型</typeparam>
        /// <param name="services">服务收藏</param>
        /// <param name="options">路由权限选项配置</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddMerchantUserRoutePermission<IdT, MerchantUserPermissionImplT>(this IServiceCollection services, Action<RoutePermissionOptions> options = null)
            where MerchantUserPermissionImplT : class, IMerchantUserMenuReader<IdT>
        {
            services.AddRoutePermission(options);

            services.AddSingleton<IMerchantUserMenuPermission<IdT>, MerchantUserMenuLocalCache<IdT>>();
            services.AddSingleton<IMerchantUserMenuReader<IdT>, MerchantUserPermissionImplT>();

            return services;
        }
    }
}
