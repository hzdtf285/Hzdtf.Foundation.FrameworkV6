﻿using Hzdtf.AUC.AspNet;
using Hzdtf.AUC.AspNet.JwtAuthHandler;
using Hzdtf.AUC.Contract.IdentityAuth;
using Hzdtf.AUC.Contract.IdentityAuth.Token;
using Hzdtf.Utility;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Extensions;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Model;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 身份认证扩展类
    /// @ 黄振东
    /// </summary>
    public static class IdentityAuthExtensions
    {
        /// <summary>
        /// 添加身份认证
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <param name="services">服务收藏</param>
        /// <param name="options">选项配置</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddIdentityAuth<IdT>(this IServiceCollection services, Action<IdentityAuthOptions> options = null)
        {
            return services.AddIdentityAuth<IdT, BasicUserInfo<IdT>>(options);
        }

        /// <summary>
        /// 添加身份认证
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <typeparam name="UserT">用户类型</typeparam>
        /// <param name="services">服务收藏</param>
        /// <param name="options">选项配置</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddIdentityAuth<IdT, UserT>(this IServiceCollection services, Action<IdentityAuthOptions> options = null)
            where UserT : BasicUserInfo<IdT>
        {
            var config = new IdentityAuthOptions();
            if (options != null)
            {
                options(config);
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IIdentityAuthReader<IdT, UserT>, IdentityAuthClaimReader<IdT, UserT>>();
            services.AddSingleton<IIdentityAuthContextReader<IdT, UserT>, IdentityAuthClaimReader<IdT, UserT>>();
            services.AddSingleton<ISimpleFactory<HttpContext, CommonUseData>, CommonUseDataFactory<IdT, UserT>>();

            var localOption = config.LocalAuth;
            switch (config.AuthType)
            {
                case IdentityAuthType.COOKIES:
                    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                    {
                        if (!string.IsNullOrWhiteSpace(localOption.LoginPath))
                        {
                            if (localOption.IsRedirectToLogin)
                            {
                                o.Events.OnRedirectToLogin = (context) =>
                                {
                                    return Task.Run(() =>
                                    {
                                        context.Response.Redirect(localOption.LoginPath);
                                    });
                                };
                            }
                            else
                            {
                                o.LoginPath = new PathString(localOption.LoginPath);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(localOption.LogoutPath))
                        {
                            if (localOption.IsRedirectToLogout)
                            {
                                o.Events.OnRedirectToLogout = (context) =>
                                {
                                    return Task.Run(() =>
                                    {
                                        context.Response.Redirect(localOption.LogoutPath);
                                    });
                                };
                            }
                            else
                            {
                                o.LogoutPath = new PathString(localOption.LogoutPath);
                            }
                        }
                    });

                    services.AddSingleton<IIdentityAuth<IdT, UserT>, IdentityCookieAuth<IdT, UserT>>();
                    services.AddSingleton<IIdentityExit, IdentityCookieAuth<IdT, UserT>>();

                    break;

                case IdentityAuthType.JWT:
                    if (config.Config == null)
                    {
                        throw new NullReferenceException("配置属性不能为null");
                    }

                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = AUCUtility.CreateTokenValiParam(config);
                        });

                    services.AddSingleton<IAuthToken, HeaderTokenAuthHandler>();
                    services.AddSingleton<IIdentityTokenAuth, IdentityJwtAuth<IdT, UserT>>();

                    break; 

                case IdentityAuthType.JWT_COOKIE:
                    services.AddSingleton<IAuthToken, CookieTokenAuthHandler>();
                    services.AddAuthentication(options =>
                    {
                        options.AddScheme<CookieTokenAuthHandler>("DefaultJwtCookie", "DefaultJwtCookie");
                        options.DefaultAuthenticateScheme = "DefaultJwtCookie";
                        options.DefaultChallengeScheme = "DefaultJwtCookie";
                    });

                    break;

                case IdentityAuthType.JWT_COOKIE_HEADER:
                    services.AddSingleton<IAuthToken, CookieHeaderTokenAuthHandler>();
                    services.AddSingleton<IIdentityTokenAuth, IdentityJwtAuth<IdT, UserT>>();
                    services.AddAuthentication(options =>
                    {
                        options.AddScheme<CookieHeaderTokenAuthHandler>("DefaultJwtCookieHeader", "DefaultJwtCookieHeader");
                        options.DefaultAuthenticateScheme = "DefaultJwtCookieHeader";
                        options.DefaultChallengeScheme = "DefaultJwtCookieHeader";
                    });

                    break;
            }

            services.AddAuthorization();

            return services;
        }

        /// <summary>
        /// 使用身份认证
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <param name="app">应用生成器</param>
        /// <returns>应用生成器</returns>
        public static IApplicationBuilder UseIdentityAuth<IdT>(this IApplicationBuilder app) => app.UseIdentityAuth<IdT, BasicUserInfo<IdT>>();

        /// <summary>
        /// 使用身份认证
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <typeparam name="UserT">用户类型</typeparam>
        /// <param name="app">应用生成器</param>
        /// <returns>应用生成器</returns>
        public static IApplicationBuilder UseIdentityAuth<IdT, UserT>(this IApplicationBuilder app) where UserT : BasicUserInfo<IdT>
        {
            UserTool<IdT>.GetCurrUserFunc = () =>
            {
                var reader = app.ApplicationServices.GetService<IIdentityAuthReader<IdT, UserT>>();
                if (reader == null)
                {
                    return null;
                }
                var returnInfo = reader.Reader();
                if (returnInfo.Success() && returnInfo.Data != null)
                {
                    return returnInfo.Data;
                }

                if (App.CurrConfig["User:AllowTest"] != null && Convert.ToBoolean(App.CurrConfig["User:AllowTest"]))
                {
                    return UserTool<IdT>.TestUser;
                }

                return null;
            };

            return app;
        }
    }

    /// <summary>
    /// 身份认证选项配置
    /// @ 黄振东
    /// </summary>
    public class IdentityAuthOptions
    {
        /// <summary>
        /// 身份认证类型，默认为Cookies
        /// </summary>
        public IdentityAuthType AuthType
        {
            get;
            set;
        } = IdentityAuthType.COOKIES;

        /// <summary>
        /// 本地认证配置
        /// </summary>
        public LocalAuthOptions LocalAuth
        {
            get;
            set;
        } = new LocalAuthOptions();

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Config
        {
            get;
            set;
        } = App.CurrConfig;
    }

    /// <summary>
    /// 本地认证选项配置
    /// @ 黄振东
    /// </summary>
    public class LocalAuthOptions
    {
        /// <summary>
        /// 登录路径
        /// </summary>
        public string LoginPath
        {
            get;
            set;
        }

        /// <summary>
        /// 登出路径
        /// </summary>
        public string LogoutPath
        {
            get;
            set;
        }

        /// <summary>
        /// 是否重定向登录
        /// </summary>
        public bool IsRedirectToLogin
        {
            get;
            set;
        }

        /// <summary>
        /// 是否重定向登出
        /// </summary>
        public bool IsRedirectToLogout
        {
            get;
            set;
        }
    }
}
