using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 注册程序集扩展类
    /// @ 黄振东
    /// </summary>
    public static class RegisterAssemblyExtensions
    {
        /// <summary>
        /// 用DI批量注入接口程序集中对应的实现类
        /// </summary>
        /// <param name="service">服务收藏</param>
        /// <param name="interfaceAssemblyName">接口程序集的名称（不包含文件扩展名）</param>
        /// <param name="implementAssemblyName">实现程序集的名称（不包含文件扩展名）</param>
        /// <param name="lifecycle">生命周期，默认为瞬时</param>
        /// <param name="interceptedClasses">拦截器类型字典，key：拦截器，value：生命周期</param>
        /// <param name="interfacTypeCallback">接口类型回调，key：接口类型；value：是否忽略</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection RegisterAssembly(this IServiceCollection service, string interfaceAssemblyName, string implementAssemblyName, 
            ServiceLifetime lifecycle = ServiceLifetime.Transient, IDictionary<Type, ServiceLifetime> interceptedClasses = null, Func<Type, bool> interfacTypeCallback = null)
        {            
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (string.IsNullOrEmpty(interfaceAssemblyName))
            {
                throw new ArgumentNullException(nameof(interfaceAssemblyName));
            }
            if (string.IsNullOrEmpty(implementAssemblyName))
            {
                throw new ArgumentNullException(nameof(implementAssemblyName));
            }

            var interfaceAssembly = Assembly.Load(interfaceAssemblyName);
            if (interfaceAssembly == null)
            {
                throw new DllNotFoundException($"the dll \"{interfaceAssemblyName}\" not be found");
            }

            var implementAssembly = Assembly.Load(implementAssemblyName);
            if (implementAssembly == null)
            {
                throw new DllNotFoundException($"the dll \"{implementAssemblyName}\" not be found");
            }

            if (!interceptedClasses.IsNullOrCount0())
            {
                service.AddSingleton<IProxyGenerator, ProxyGenerator>();
                service.AddTypeMapLifetimes(interceptedClasses);
            }

            //过滤掉非接口
            var types = interfaceAssembly.GetTypes().Where(t => t.GetTypeInfo().IsInterface);

            foreach (var type in types)
            {
                if (interfacTypeCallback != null && interfacTypeCallback(type))
                {
                    continue;
                }

                //过滤掉抽象类、以及非class
                var implementType = implementAssembly.DefinedTypes
                   .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(b => b.Name == type.Name));
                if (implementType != null)
                {
                    var constrs = implementType.GetConstructors();
                    if (!constrs.IsNullOrLength0() && !constrs.Any(p => p.IsPublic))
                    {
                        continue;
                    }

                    switch (lifecycle)
                    {
                        case ServiceLifetime.Transient:
                            if (interceptedClasses.IsNullOrCount0())
                            {
                                service.AddTransient(type, implementType.AsType());
                            }
                            else
                            {
                                service.AddTransient(type, provider =>
                                {
                                    return provider.GetProxyInstanceFromProvider(implementType.AsType(), interceptedClasses == null ? null : interceptedClasses.Keys);
                                });
                            }

                            break;

                        case ServiceLifetime.Scoped:
                            if (interceptedClasses.IsNullOrCount0())
                            {
                                service.AddScoped(type, implementType.AsType());
                            }
                            else
                            {
                                service.AddScoped(type, provider =>
                                {
                                    return provider.GetProxyInstanceFromProvider(implementType.AsType(), interceptedClasses == null ? null : interceptedClasses.Keys);
                                });
                            }

                            break;

                        case ServiceLifetime.Singleton:
                            if (interceptedClasses.IsNullOrCount0())
                            {
                                service.AddSingleton(type, implementType.AsType());
                            }
                            else
                            {
                                service.AddSingleton(type, provider =>
                                {
                                    return provider.GetProxyInstanceFromProvider(implementType.AsType(), interceptedClasses == null ? null : interceptedClasses.Keys);
                                });
                            }

                            break;

                        default:
                            throw new NotSupportedException($"不支持的生命周期:{lifecycle}");
                    }
                }
            }

            return service;
        }

        /// <summary>
        /// 用DI批量注入批定接口类型的实现类程序集中的实现类
        /// </summary>
        /// <param name="service">服务收藏</param>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="lifecycle">生命周期，默认为瞬时</param>
        /// <param name="interceptedClasses">拦截器类型字典，key：拦截器，value：生命周期</param>
        /// <param name="implClassAssemblys">接口程序集的名称（不包含文件扩展名）</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection RegisterAssemblyWithInterfaceMapImpls(this IServiceCollection service, Type interfaceType,
            ServiceLifetime lifecycle = ServiceLifetime.Transient, IDictionary<Type, ServiceLifetime> interceptedClasses = null,  params string[] implClassAssemblys)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (implClassAssemblys.IsNullOrLength0())
            {
                throw new ArgumentNullException(nameof(implClassAssemblys));
            }

            Assembly[] assemblies = new Assembly[implClassAssemblys.Length];
            for (var i = 0; i < implClassAssemblys.Length; i++)
            {
                assemblies[i] = Assembly.Load(implClassAssemblys[i]);
            }

            var implTypes = ReflectExtensions.GetImplClassType(assemblies, interfaceType);
            if (implTypes.IsNullOrLength0())
            {
                return service;
            }

            if (!interceptedClasses.IsNullOrCount0())
            {
                service.AddSingleton<IProxyGenerator, ProxyGenerator>();
                service.AddTypeMapLifetimes(interceptedClasses);
            }

            foreach (var it in implTypes)
            {
                if (it.IsAbstract)
                {
                    continue;
                }
                var constrs = it.GetConstructors();
                if (!constrs.IsNullOrLength0() && !constrs.Any(p => p.IsPublic))
                {
                    continue;
                }

                switch (lifecycle)
                {
                    case ServiceLifetime.Transient:
                        if (interceptedClasses.IsNullOrCount0())
                        {
                            service.AddTransient(interfaceType, it);
                        }
                        else
                        {
                            service.AddTransient(interfaceType, provider =>
                            {
                                return provider.GetProxyInstanceFromProvider(interfaceType, interceptedClasses == null ? null : interceptedClasses.Keys);
                            });
                        }

                        break;

                    case ServiceLifetime.Scoped:
                        if (interceptedClasses.IsNullOrCount0())
                        {
                            service.AddScoped(interfaceType, it);
                        }
                        else
                        {
                            service.AddScoped(interfaceType, provider =>
                            {
                                return provider.GetProxyInstanceFromProvider(interfaceType, interceptedClasses == null ? null : interceptedClasses.Keys);
                            });
                        }

                        break;

                    case ServiceLifetime.Singleton:
                        if (interceptedClasses.IsNullOrCount0())
                        {
                            service.AddSingleton(interfaceType, it);
                        }
                        else
                        {
                            service.AddSingleton(interfaceType, provider =>
                            {
                                return provider.GetProxyInstanceFromProvider(interfaceType, interceptedClasses == null ? null : interceptedClasses.Keys);
                            });
                        }

                        break;

                    default:
                        throw new NotSupportedException($"不支持的生命周期:{lifecycle}");
                }
            }

            return service;
        }

        /// <summary>
        /// 从服务提供者获取代理实例
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <param name="type">类型</param>
        /// <param name="interceptedClasses">拦截器类型字典集合</param>
        /// <returns>代理实例</returns>
        public static object GetProxyInstanceFromProvider(this IServiceProvider provider, Type type, ICollection<Type> interceptedClasses = null)
        {
            var target = provider.GetService(type);
            if (interceptedClasses.IsNullOrCount0())
            {
                return target;
            }

            var proxy = provider.GetService<IProxyGenerator>();
            var interClassServices = new List<IInterceptor>(interceptedClasses.Count);
            foreach (var interClass in interceptedClasses)
            {
                var interIns = provider.GetService(interClass);
                if (interIns == null)
                {
                    continue;
                }

                interClassServices.Add(interIns as IInterceptor);
            }

            if (interClassServices.Count == 0)
            {
                return target;
            }
            else
            {
                // 选择最多参数的构造器
                var con = type.GetConstructors().OrderByDescending(p => p.GetParameters().Length).Take(1).First();
                var parames = con.GetParameters();
                if (parames.IsNullOrLength0())
                {
                    return proxy.CreateClassProxy(type, interClassServices.ToArray());
                }

                // 构造器参数值数组
                var conValues = new object[parames.Length];
                for (var i = 0; i < parames.Length; i++)
                {
                    conValues[i] = provider.GetService(parames[i].ParameterType);
                }

                return proxy.CreateClassProxy(type, conValues, interClassServices.ToArray());
            }
        }

        /// <summary>
        /// 从服务提供者获取代理实例
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <param name="interceptedClasses">拦截器类型字典集合</param>
        /// <returns>代理实例</returns>
        public static T GetProxyInstanceFromProvider<T>(this IServiceProvider provider, ICollection<Type> interceptedClasses = null) where T : class
        {
            return provider.GetProxyInstanceFromProvider(typeof(T), interceptedClasses) as T;
        }

        /// <summary>
        /// 添加类型映射生命周期
        /// </summary>
        /// <param name="service">服务收藏</param>
        /// <param name="typeMapLifetimes">映射生命周期字典</param>
        /// <returns>服务收藏</returns>
        private static IServiceCollection AddTypeMapLifetimes(this IServiceCollection service, IDictionary<Type, ServiceLifetime> typeMapLifetimes)
        {
            if (typeMapLifetimes.IsNullOrCount0())
            {
                return service;
            }

            foreach (var interClass in typeMapLifetimes)
            {
                switch (interClass.Value)
                {
                    case ServiceLifetime.Transient:
                        service.AddTransient(interClass.Key);

                        break;

                    case ServiceLifetime.Scoped:
                        service.AddScoped(interClass.Key);

                        break;

                    case ServiceLifetime.Singleton:
                        service.AddSingleton(interClass.Key);

                        break;

                    default:
                        throw new NotSupportedException($"不支持的生命周期:{interClass.Value}");
                }
            }

            return service;
        }
    }
}
