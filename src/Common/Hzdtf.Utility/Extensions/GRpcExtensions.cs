using Hzdtf.Utility;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Utility.Utils;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Extensions;
using System.Diagnostics;
using Grpc.Net.ClientFactory;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Net.Security;

namespace Grpc.Net.Client
{
    /// <summary>
    /// GRpc扩展类
    /// @ 黄振东
    /// </summary>
    public static partial class GRpcExtensions
    {
        /// <summary>
        /// 默认套接字Http处理
        /// </summary>
        public static readonly SocketsHttpHandler DefaultSocketsHttpHandler = CreateSocketsHttpHandler();

        /// <summary>
        /// 默认套接字Http tls处理
        /// </summary>
        public static readonly SocketsHttpHandler DefaultSocketsTlsHttpHandler = CreateSocketsHttpHandler(SslProtocols.Tls12);

        /// <summary>
        /// 默认方法配置
        /// </summary>
        public static readonly MethodConfig DefaultMethodConfig = new MethodConfig()
        {
            Names = { MethodName.Default }
        };

        /// <summary>
        /// 创建一个渠道，执行完业务处理方法后，会自动关闭渠道连接
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="action">回调业务处理方法</param>
        /// <param name="exAction">发生异常回调，如果为null，则不会捕获异常</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="options">选项配置</param>
        [Obsolete("此方法因不能复用GRpc通道，性能差，故弃用。请使用GetGRpcClient")]
        public static void CreateChannel(string address, Action<GrpcChannel, Metadata> action, Action<RpcException> exAction = null, Action<ChannelCustomerOptions> customerOptions = null, GrpcChannelOptions options = null)
        {
            var headers = new Metadata();
            var cusOptions = new ChannelCustomerOptions();
            if (customerOptions != null)
            {
                customerOptions(cusOptions);
                if (cusOptions.ComData != null && !string.IsNullOrWhiteSpace(cusOptions.ComData.ClientRemoteIp))
                {
                    headers.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, cusOptions.ComData.ClientRemoteIp);
                }
            }

            var token = cusOptions.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                headers.Add($"{AuthUtil.AUTH_KEY}", token.AddBearerToken());
            }
            var eventId = cusOptions.GetEventId();
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                headers.Add(App.EVENT_ID_KEY, eventId);
            }

            var channel = options == null ? GrpcChannel.ForAddress(address) : GrpcChannel.ForAddress(address, options);
            ExecCallBusiness(address, cusOptions, channel, headers, eventId, action, exAction);
        }

        /// <summary>
        /// 创建Metadata
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>Metadata</returns>
        public static Metadata CreateMetadata(this CommonUseData comData)
        {
            var headers = new Metadata();
            SetMetadata(headers, comData);

            return headers;
        }


        /// <summary>
        /// 设置Metadata
        /// </summary>
        /// <param name="headers">Metadata</param>
        /// <param name="comData">通用数据</param>
        public static void SetMetadata(this Metadata headers, CommonUseData comData)
        {
            if (comData == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(comData.ClientRemoteIp))
            {
                headers.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, comData.ClientRemoteIp);
            }
            var token = comData.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                headers.Add($"{AuthUtil.AUTH_KEY}", token.AddBearerToken());
            }
            var eventId = comData.GetEventId();
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                headers.Add(App.EVENT_ID_KEY, eventId);
            }

            if (!string.IsNullOrWhiteSpace(comData.Controller))
            {
                headers.Add("Controller", comData.Controller);
            }
            if (!string.IsNullOrWhiteSpace(comData.Action))
            {
                headers.Add("Action", comData.Action);
            }
            if (!string.IsNullOrWhiteSpace(comData.Path))
            {
                headers.Add("Path", comData.Path);
            }
            if (!string.IsNullOrWhiteSpace(comData.MenuCode))
            {
                headers.Add("MenuCode", comData.MenuCode);
            }
            if (!comData.FunctionCodes.IsNullOrLength0())
            {
                headers.Add("FunctionCodes", comData.FunctionCodes.ToJsonString());
            }
            if (!string.IsNullOrWhiteSpace(comData.Key))
            {
                headers.Add("Key", comData.Key);
            }
        }

        /// <summary>
        /// 执行回调业务
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="options">自定义配置</param>
        /// <param name="channel">已经创建的渠道</param>
        /// <param name="headers">头</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="action">回调业务动作</param>
        /// <param name="exAction">发生异常回调，如果为null，则不会捕获异常</param>
        private static void ExecCallBusiness(string address, ChannelCustomerOptions options, GrpcChannel channel, Metadata headers, 
            string eventId, Action<GrpcChannel, Metadata> action, Action<RpcException> exAction)
        {
            RpcException rpcEx = null;
            Exception exce = null;
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                action(channel, headers);
                watch.Stop();
            }
            catch (RpcException ex)
            {
                watch.Stop();
                exce = rpcEx = ex;
            }
            catch (Exception ex)
            {
                watch.Stop();
                exce = ex;
            }
            finally
            {
                channel.Dispose();
            }

            if (App.InfoEvent != null)
            {
                App.InfoEvent.RecordAsync($"grpc发起请求地址:{address}.接口:{options.Api}.耗时:{watch.ElapsedMilliseconds}ms",
                    exce, "CreateChannel", eventId, address, options.Api);
            }
            if (rpcEx != null && exAction != null)
            {
                exAction(rpcEx);
            }
            else if (exce != null)
            {
                throw new Exception(exce.Message, exce);
            }
        }

        /// <summary>
        /// 判断请求内容类型是否GRpc
        /// </summary>
        /// <param name="requestContentType">请求内容</param>
        /// <returns>请求内容类型是否GRpc</returns>
        public static bool IsRequestGRpc(string requestContentType)
        {
            if (string.IsNullOrWhiteSpace(requestContentType))
            {
                return false;
            }

            return "application/grpc".Equals(requestContentType.ToLower());
        }

        /// <summary>
        /// 抛出RPC异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">消息</param>
        public static void ThrowRpcException(this Exception ex, string msg)
        {
            var status = new Status(StatusCode.Unknown, msg, ex);
            throw new RpcException(status);
        }

        /// <summary>
        /// 如果返回值是错误则抛出RPC异常
        /// </summary>
        /// <param name="basicReturn">基本返回</param>
        public static void ThrowReturnFailureRpcException(this BasicReturnInfo basicReturn)
        {
            if (basicReturn == null || basicReturn.Success())
            {
                return;
            }

            new BusinessException(basicReturn.Code, basicReturn.Msg, basicReturn.Desc).ThrowRpcException(basicReturn.Msg);
        }

        /// <summary>
        /// 获取GRpc客户端
        /// </summary>
        /// <typeparam name="GRpcClientT">GRpc客户端类型</typeparam>
        /// <param name="action">回调业务处理方法</param>
        /// <param name="exAction">发生异常回调，如果为null，则不会捕获异常</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <returns>GRpc客户端</returns>
        public static GRpcClientT GetGRpcClient<GRpcClientT>(Action<GRpcClientT, Metadata> action, Action<RpcException> exAction = null, Action<ChannelCustomerOptions> customerOptions = null)
            where GRpcClientT : ClientBase<GRpcClientT>
        {
            var client = App.Instance.GetService<GRpcClientT>();
            ExecGRpcClient(client, action, exAction, customerOptions);

            return client;
        }

        /// <summary>
        /// 异步获取GRpc客户端
        /// </summary>
        /// <typeparam name="GRpcClientT">GRpc客户端类型</typeparam>
        /// <param name="func">回调业务处理方法</param>
        /// <param name="exAction">发生异常回调，如果为null，则不会捕获异常</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <returns>GRpc客户端</returns>
        public static async Task<GRpcClientT> GetGRpcClientAsync<GRpcClientT>(Func<GRpcClientT, Metadata, Task> func, Action<RpcException> exAction = null, Action<ChannelCustomerOptions> customerOptions = null)
            where GRpcClientT : ClientBase<GRpcClientT>
        {
            var client = App.Instance.GetService<GRpcClientT>();
            await ExecGRpcClientAsync(client, func, exAction, customerOptions);

            return client;
        }

        /// <summary>
        /// 设置轮询负载均衡策略和静态解析
        /// </summary>
        /// <param name="options">GRpc工厂选项配置</param>
        /// <param name="service">服务</param>
        /// <param name="callbackChannelOptions">回调GRpc渠道选项配置</param>
        /// <param name="credentials">证书</param>
        public static void SetRobinBalancingAndStaticResolver(this GrpcClientFactoryOptions options, string service, Action<GrpcChannelOptions> callbackChannelOptions = null, ChannelCredentials credentials = null)
        {
            options.Address = new Uri($"static:{service}");
            options.ChannelOptionsActions.Add(op =>
            {
                if (credentials == null)
                {
                    credentials = ChannelCredentials.Insecure;
                }

                if (credentials == ChannelCredentials.SecureSsl)
                {
                    op.HttpHandler = DefaultSocketsTlsHttpHandler;
                }
                else
                {
                    op.HttpHandler = DefaultSocketsHttpHandler;
                }
                op.Credentials = credentials;
                op.ServiceConfig = new ServiceConfig 
                {
                    MethodConfigs = { DefaultMethodConfig },
                    LoadBalancingConfigs = { new RoundRobinConfig() } 
                };

                if (callbackChannelOptions != null)
                {
                    callbackChannelOptions(op);
                }
            });
        }

        /// <summary>
        /// 执行GRpc客户端
        /// 需要在App.GetGRpcClient里设置获取GRpc客户端工厂的实现
        /// </summary>
        /// <typeparam name="GRpcClientT">GRpc客户端类型</typeparam>
        /// <param name="client">客户端</param>
        /// <param name="action">回调业务处理方法</param>
        /// <param name="exAction">发生异常回调，如果为null，则不会捕获异常</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <returns>GRpc客户端</returns>
        private static void ExecGRpcClient<GRpcClientT>(GRpcClientT client, Action<GRpcClientT, Metadata> action, Action<RpcException> exAction = null, Action<ChannelCustomerOptions> customerOptions = null)
            where GRpcClientT : ClientBase<GRpcClientT>
        {
            if (action == null)
            {
                return;
            }

            var headers = new Metadata();
            var cusOptions = new ChannelCustomerOptions();
            if (customerOptions != null)
            {
                customerOptions(cusOptions);
                if (cusOptions.ComData != null && !string.IsNullOrWhiteSpace(cusOptions.ComData.ClientRemoteIp))
                {
                    headers.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, cusOptions.ComData.ClientRemoteIp);
                }
            }

            var token = cusOptions.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                headers.Add($"{AuthUtil.AUTH_KEY}", token.AddBearerToken());
            }
            var eventId = cusOptions.GetEventId();
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                headers.Add(App.EVENT_ID_KEY, eventId);
                if (cusOptions.ComData != null && !string.IsNullOrWhiteSpace(cusOptions.ComData.ClientRemoteIp))
                {
                    headers.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, cusOptions.ComData.ClientRemoteIp);
                }
            }

            RpcException rpcEx = null;
            Exception exce = null;
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                action(client, headers);
                watch.Stop();
            }
            catch (RpcException ex)
            {
                watch.Stop();
                exce = rpcEx = ex;
            }
            catch (Exception ex)
            {
                watch.Stop();
                exce = ex;
            }

            if (App.InfoEvent != null)
            {
                App.InfoEvent.RecordAsync($"grpc发起请求.接口:{cusOptions.Api}.耗时:{watch.ElapsedMilliseconds}ms",
                    exce, "GetGRpcClient", eventId, cusOptions.Api);
            }
            if (rpcEx != null && exAction != null)
            {
                exAction(rpcEx);
            }
            else if (exce != null)
            {
                throw new Exception(exce.Message, exce);
            }
        }

        /// <summary>
        /// 异步执行GRpc客户端
        /// 需要在App.GetGRpcClient里设置获取GRpc客户端工厂的实现
        /// </summary>
        /// <typeparam name="GRpcClientT">GRpc客户端类型</typeparam>
        /// <param name="client">客户端</param>
        /// <param name="func">回调业务处理方法</param>
        /// <param name="exAction">发生异常回调，如果为null，则不会捕获异常</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <returns>GRpc客户端</returns>
        private static async Task ExecGRpcClientAsync<GRpcClientT>(GRpcClientT client, Func<GRpcClientT, Metadata, Task> func, Action<RpcException> exAction = null, Action<ChannelCustomerOptions> customerOptions = null)
            where GRpcClientT : ClientBase<GRpcClientT>
        {
            if (func == null)
            {
                return;
            }

            var headers = new Metadata();
            var cusOptions = new ChannelCustomerOptions();
            if (customerOptions != null)
            {
                customerOptions(cusOptions);
                if (cusOptions.ComData != null && !string.IsNullOrWhiteSpace(cusOptions.ComData.ClientRemoteIp))
                {
                    headers.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, cusOptions.ComData.ClientRemoteIp);
                }
            }

            var token = cusOptions.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                headers.Add(AuthUtil.AUTH_KEY, token.AddBearerToken());
            }
            var eventId = cusOptions.GetEventId();
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                headers.Add(App.EVENT_ID_KEY, eventId);
            }

            RpcException rpcEx = null;
            Exception exce = null;
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                await func(client, headers);
                watch.Stop();
            }
            catch (RpcException ex)
            {
                watch.Stop();
                exce = rpcEx = ex;
            }
            catch (Exception ex)
            {
                watch.Stop();
                exce = ex;
            }

            if (App.InfoEvent != null)
            {
                _ = App.InfoEvent.RecordAsync($"grpc发起请求.接口:{cusOptions.Api}.耗时:{watch.ElapsedMilliseconds}ms",
                    exce, "GetGRpcClient", eventId, cusOptions.Api);
            }
            if (rpcEx != null && exAction != null)
            {
                exAction(rpcEx);
            }
            else if (exce != null)
            {
                throw new Exception(exce.Message, exce);
            }
        }

        /// <summary>
        /// 创建套接字http处理
        /// </summary>
        /// <param name="sslProtocols">安全协议，默认为none</param>
        /// <returns>套接字http处理</returns>
        public static SocketsHttpHandler CreateSocketsHttpHandler(SslProtocols sslProtocols = SslProtocols.None)
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true,
            };
            if (sslProtocols != SslProtocols.None)
            {
                handler.SslOptions = new SslClientAuthenticationOptions()
                {
                    EnabledSslProtocols = sslProtocols,
                    RemoteCertificateValidationCallback = (o, c1, c2, ssl) =>
                    {
                        return true;
                    },
                };
            }

            return handler;
        }
    }    
}
