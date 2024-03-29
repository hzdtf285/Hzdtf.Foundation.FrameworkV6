﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility;
using Hzdtf.Utility.Extensions;
using Hzdtf.Utility.Utils;
using System.Security.Authentication;

namespace System.Net.Http
{
    /// <summary>
    /// HTTP客户端扩展类
    /// @ 黄振东
    /// </summary>
    public static class HttpClientExtension
    {
        /// <summary>
        /// 默认Tls http客户端处理
        /// </summary>
        public static readonly HttpClientHandler DefaultTlsHttpClientHandler = CreateSslProtocolsHttpClientHandler(SslProtocols.Tls12);

        #region Get

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string Get(string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                return httpClient.Get(url, customerOptions, timeout);
            }
        }

        /// <summary>
        /// Get请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string Get(this HttpClient httpClient, string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return httpClient.RequestJson(url, "Get", httpContent =>
            {
                return httpClient.GetAsync(url);
            }, customerOptions: customerOptions, timeout: timeout);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> GetAsync(string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => Get(url, customerOptions, timeout));
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> GetAsync(this HttpClient httpClient, string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => httpClient.Get(url, customerOptions, timeout));
        }

        #endregion

        #region Post

        /// <summary>
        /// Post请求JSON
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string PostJson(string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                return httpClient.PostJson(url, data, customerOptions, timeout);
            }
        }

        /// <summary>
        /// Post请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string PostJson(this HttpClient httpClient, string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return httpClient.RequestJson(url, "Post", httpContent =>
            {
                return httpClient.PostAsync(url, httpContent);
            }, data, customerOptions, timeout);
        }

        /// <summary>
        /// Post请求JSON
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> PostJsonAsync(string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => PostJson(url, data, customerOptions, timeout));
        }

        /// <summary>
        /// Post请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> PostJsonAsync(this HttpClient httpClient, string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => httpClient.PostJson(url, data, customerOptions, timeout));
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string Delete(string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                return httpClient.Delete(url, customerOptions, timeout);
            }
        }

        /// <summary>
        /// Get请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string Delete(this HttpClient httpClient, string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return httpClient.RequestJson(url, "Delete", httpContent =>
            {
                return httpClient.DeleteAsync(url);
            }, customerOptions: customerOptions, timeout: timeout);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> DeleteAsync(string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => Delete(url, customerOptions, timeout));
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> DeleteAsync(this HttpClient httpClient, string url, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => httpClient.Delete(url, customerOptions, timeout));
        }

        #endregion

        #region Put

        /// <summary>
        /// Put请求JSON
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string PutJson(string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                return httpClient.PutJson(url, data, customerOptions, timeout);
            }
        }

        /// <summary>
        /// Put请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string PutJson(this HttpClient httpClient, string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return httpClient.RequestJson(url, "Put", httpContent =>
            {
                return httpClient.PutAsync(url, httpContent);
            }, data, customerOptions, timeout);
        }

        /// <summary>
        /// Put请求JSON
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> PutJsonAsync(string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => PutJson(url, data, customerOptions, timeout));
        }

        /// <summary>
        /// Put请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> PutJsonAsync(this HttpClient httpClient, string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => httpClient.PutJson(url, data, customerOptions, timeout));
        }

        #endregion

        #region Patch

        /// <summary>
        /// Patch请求JSON
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string PatchJson(string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                return httpClient.PatchJson(url, data, customerOptions, timeout);
            }
        }

        /// <summary>
        /// Patch请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string PatchJson(this HttpClient httpClient, string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return httpClient.RequestJson(url, "Patch", httpContent =>
            {
                return httpClient.PatchAsync(url, httpContent);
            }, data, customerOptions, timeout);
        }

        /// <summary>
        /// Patch请求JSON
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> PatchJsonAsync(string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => PatchJson(url, data, customerOptions, timeout));
        }

        /// <summary>
        /// Patch请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="data">数据</param>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>任务</returns>
        public static Task<string> PatchJsonAsync(this HttpClient httpClient, string url, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            return Task<string>.Run(() => httpClient.PatchJson(url, data, customerOptions, timeout));
        }

        #endregion

        /// <summary>
        /// 添加token到头里
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="token">token</param>
        public static void AddBearerTokenToHeader(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Add(AuthUtil.AUTH_KEY, token.AddBearerToken());
        }

        /// <summary>
        /// 添加包含了token到头里
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="containerBearerToken">包含了bearer token</param>
        public static void AddContainerBearerTokenToHeader(this HttpClient httpClient, string containerBearerToken)
        {
            httpClient.DefaultRequestHeaders.Add(AuthUtil.AUTH_KEY, containerBearerToken);
        }

        /// <summary>
        /// 创建http客户端
        /// </summary>
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>http客户端</returns>
        public static HttpClient CreateHttpClient(Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            var httpClient = new HttpClient();
            var cusOptions = new ChannelCustomerOptions();
            if (customerOptions != null)
            {
                customerOptions(cusOptions);
                if (cusOptions.ComData != null && !string.IsNullOrWhiteSpace(cusOptions.ComData.ClientRemoteIp))
                {
                    httpClient.DefaultRequestHeaders.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, cusOptions.ComData.ClientRemoteIp);
                }
            }

            var token = cusOptions.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Add($"{AuthUtil.AUTH_KEY}", token.AddBearerToken());
            }
            var eventId = cusOptions.GetEventId();
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                httpClient.DefaultRequestHeaders.Add(App.EVENT_ID_KEY, eventId);
            }
            if (timeout != null)
            {
                httpClient.Timeout = (TimeSpan)timeout;
            }

            return httpClient;
        }

        /// <summary>
        /// 请求请求JSON
        /// </summary>
        /// <param name="httpClient">http客户端</param>
        /// <param name="url">URL</param>
        /// <param name="method">方法</param>
        /// <param name="callbackRequest">回调请求</param>
        /// <param name="data">数据</param>        
        /// <param name="customerOptions">自定义选项配置</param>
        /// <param name="timeout">超时</param>
        /// <returns>返回字符串</returns>
        public static string RequestJson(this HttpClient httpClient, string url, string method, Func<HttpContent, Task<HttpResponseMessage>> callbackRequest, object data = null, Action<ChannelCustomerOptions> customerOptions = null, TimeSpan? timeout = null)
        {
            HttpContent content = null;
            if (data != null)
            {
                string dataJson = data.ToJsonString();
                if (string.IsNullOrWhiteSpace(dataJson))
                {
                    content = new StringContent(string.Empty);
                }
                else
                {
                    content = new StringContent(dataJson, Encoding.UTF8);
                }
            }
            else
            {
                content = new StringContent(string.Empty);
            }
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            Task<HttpResponseMessage> task = null;
            httpClient.DefaultRequestHeaders.Add("Method", method);

            var cusOptions = new ChannelCustomerOptions();
            if (customerOptions != null)
            {
                customerOptions(cusOptions);
                if (cusOptions.ComData != null && !string.IsNullOrWhiteSpace(cusOptions.ComData.ClientRemoteIp))
                {
                    httpClient.DefaultRequestHeaders.Add(App.CLIENT_REMOTE_IP_HEAD_KEY, cusOptions.ComData.ClientRemoteIp);
                }
            }

            var token = cusOptions.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Add(AuthUtil.AUTH_KEY, token.AddBearerToken());
            }
            var eventId = cusOptions.GetEventId();
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                content.Headers.Add(App.EVENT_ID_KEY, eventId);
            }
            if (timeout != null)
            {
                httpClient.Timeout = (TimeSpan)timeout;
            }

            Exception exce = null;
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                task = callbackRequest(content);
                task.Wait();
                watch.Stop();
            }
            catch (Exception ex)
            {
                watch.Stop();
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (App.InfoEvent != null)
                {
                    App.InfoEvent.RecordAsync($"http发起请求地址:{url}.方法:{method}.接口:{cusOptions.Api}.耗时:{watch.ElapsedMilliseconds}ms",
                        exce, "RequestJson", eventId, url, cusOptions.Api);
                }
            }

            if (task.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var readTask = task.Result.Content.ReadAsStringAsync();
                readTask.Wait();

                return readTask.Result;
            }
            else
            {
                throw new Exception(task.Result.StatusCode.ToString());
            }
        }

        /// <summary>
        /// 创建安全协议http客户端处理
        /// </summary>
        /// <param name="protocols">安全协议类型</param>
        /// <returns>http客户端处理</returns>
        public static HttpClientHandler CreateSslProtocolsHttpClientHandler(SslProtocols protocols)
        {
            return new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = protocols,
                ServerCertificateCustomValidationCallback = (x, y, z, m) => true,
            };
        }
    }
}
