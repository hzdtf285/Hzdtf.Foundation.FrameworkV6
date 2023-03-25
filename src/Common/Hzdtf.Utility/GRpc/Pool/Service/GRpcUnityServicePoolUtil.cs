using Hzdtf.Utility.Utils;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hzdtf.Utility.GRpc.Pool.Service
{
    /// <summary>
    /// GRpc统计服务池辅助类
    /// @ 黄振东
    /// </summary>
    public static class GRpcUnityServicePoolUtil
    {
        /// <summary>
        /// 异步生成资源值
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="path">路径</param>
        /// <param name="tag">标签</param>
        /// <returns>生成资源值</returns>
        public static async Task<GrpcChannel> BuilderAsync(string serviceName, string path = null, string tag = null)
        {
            var pool = App.GetServiceFromInstance<IGRpcUnityServicePool>();
            return await pool.BuilderAsync(serviceName, path, tag);
        }

        /// <summary>
        /// 回收，使用后需要执行回收
        /// </summary>
        /// <param name="value">资源值</param>
        public static void Recycle(GrpcChannel value)
        {
            var pool = App.GetServiceFromInstance<IGRpcUnityServicePool>();
            pool.Recycle(value);
        }

        /// <summary>
        /// 执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="action">回调</param>
        public static void Exec(string key, Action<GrpcChannel> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("回调不能为null");
            }
            var pool = App.GetServiceFromInstance<IGRpcUnityServicePool>();
            pool.Exec(key, action);
        }

        /// <summary>
        /// 异步生成资源值并执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="func">回调</param>
        public static async Task ExecAsync(string key, Func<GrpcChannel, Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("回调不能为null");
            }
            var pool = App.GetServiceFromInstance<IGRpcUnityServicePool>();
            await pool.ExecAsync(key, func);
        }

        /// <summary>
        /// 异步执行生成，会自动回收
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="func">回调</param>
        /// <param name="path">路径</param>
        /// <param name="tag">标签</param>
        public static async void ExecBuilderAsync(string serviceName, Func<GrpcChannel, Task> func, string path = null, string tag = null)
        {
            await ExecBuilderAsync<object>(serviceName, async (c) =>
            {
                await func(c);
                return null;
            }, path, tag);
        }

        /// <summary>
        /// 异步执行生成，会自动回收
        /// </summary>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="serviceName">服务名</param>
        /// <param name="func">回调</param>
        /// <param name="path">路径</param>
        /// <param name="tag">标签</param>
        public static async Task<ReturnT> ExecBuilderAsync<ReturnT>(string serviceName, Func<GrpcChannel, Task<ReturnT>> func, string path = null, string tag = null)
        {
            if (func == null)
            {
                throw new ArgumentNullException("回调不能为null");
            }
            var channel = await BuilderAsync(serviceName, path, tag);
            try
            {
               return await func(channel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                Recycle(channel);
            }
        }

        /// <summary>
        /// 异步批量执行生成，会自动回收
        /// </summary>
        /// <param name="func">回调</param>
        /// <param name="buildeGRpcChannel">生成GRpc渠道参数</param>
        public static async void BatchExecBuilderAsync(Func<Task> func, params BuilderGRpcChannelInfo[] buildeGRpcChannel)
        {
            await BatchExecBuilderAsync<object>(async () =>
            {
                await func();
                return null;
            }, buildeGRpcChannel);
        }

        /// <summary>
        /// 异步批量执行生成，会自动回收
        /// </summary>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="func">回调</param>
        /// <param name="buildeGRpcChannel">生成GRpc渠道参数</param>
        public static async Task<ReturnT> BatchExecBuilderAsync<ReturnT>(Func<Task<ReturnT>> func, params BuilderGRpcChannelInfo[] buildeGRpcChannel)
        {
            if (func == null)
            {
                throw new ArgumentNullException("回调不能为null");
            }
            if (buildeGRpcChannel.IsNullOrLength0())
            {
                throw new ArgumentNullException("生成GRpc渠道参数不能为空");
            }

            try
            {
                for (var i = 0; i < buildeGRpcChannel.Length; i++)
                {
                    var item = buildeGRpcChannel[i];
                    if (item == null)
                    {
                        throw new ArgumentNullException($"生成GRpc渠道参数第[{i}]个不能为空");
                    }
                    buildeGRpcChannel[i].Value = await BuilderAsync(item.ServiceName, item.Path, item.Tag);
                }

                return await func();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                foreach (var c in buildeGRpcChannel)
                {
                    Recycle(c.Value);
                }
            }
        }
    }
}
