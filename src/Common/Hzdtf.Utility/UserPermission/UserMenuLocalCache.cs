﻿using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Cache;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Utils;
using System.Collections.Concurrent;

namespace Hzdtf.Utility.UserPermission
{
    /// <summary>
    /// 用户菜单本地缓存
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    [Inject]
    public class UserMenuLocalCache<IdT> : SingleTypeLocalMemoryBase<IdT, IDictionary<string, string[]>>, IUserMenuPermissionCache<IdT>, IUserMenuPermission<IdT>
    {
        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<IdT, IDictionary<string, string[]>> dicCache = new ConcurrentDictionary<IdT, IDictionary<string, string[]>>();

        /// <summary>
        /// 最后一次访问时间字典
        /// </summary>
        private static readonly IDictionary<IdT, DateTime> dicLastAccessTime = new ConcurrentDictionary<IdT, DateTime>();

        /// <summary>
        /// 用户菜单读取
        /// </summary>
        protected readonly IUserMenuReader<IdT> userMenuReader;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="userMenuReader">用户菜单读取</param>
        public UserMenuLocalCache(IUserMenuReader<IdT> userMenuReader = null)
        {
            this.userMenuReader = userMenuReader;
        }

        /// <summary>
        /// 用户是否拥有权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="funCodes">功能编码数组</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public ReturnInfo<bool> UserHavePermission(IdT userId, string menuCode, string[] funCodes, CommonUseData comData = null)
        {
            var re = new ReturnInfo<bool>();
            var userMenuFunCodes = Get(userId);
            if (userMenuFunCodes == null)
            {
                var reMenuFunCodes = userMenuReader.GetHavePermissionMenuFunCodes(userId, comData);
                if (reMenuFunCodes.Failure())
                {
                    re.FromBasic(reMenuFunCodes);
                    return re;
                }
                if (reMenuFunCodes.Data == null)
                {
                    Add(userId, new ConcurrentDictionary<string, string[]>());

                    return re;
                }
                else
                {
                    Add(userId, reMenuFunCodes.Data);
                    userMenuFunCodes = reMenuFunCodes.Data;
                }
            }
            else
            {
                dicLastAccessTime[userId] = DateTimeExtensions.Now;
            }

            if (userMenuFunCodes.ContainsKey(menuCode))
            {
                var exitsFunCodes = userMenuFunCodes[menuCode];
                if (exitsFunCodes.IsNullOrLength0())
                {
                    return re;
                }
                // 循环需要的功能编码，只要有一个存在，则有权限直接返回
                foreach (var funCode in funCodes)
                {
                    re.Data = exitsFunCodes.Contains(funCode);
                    if (re.Data)
                    {
                        return re;
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// 获取时间范围内没有访问的用户ID数组
        /// </summary>
        /// <param name="timeSpan">时间范围</param>
        /// <returns>时间范围内没有访问的用户ID数组</returns>
        public IdT[] GetWithTSNotAccessKeys(TimeSpan timeSpan)
        {
            return dicLastAccessTime.Where(p => (DateTimeExtensions.Now - p.Value) >= timeSpan).Select(p => p.Key).ToArray();
        }

        /// <summary>
        /// 移除时间范围内没有访问的用户
        /// </summary>
        /// <param name="timeSpan">时间范围</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveWithTSNotAccess(TimeSpan timeSpan)
        {
            var ids = GetWithTSNotAccessKeys(timeSpan);
            if (ids.IsNullOrLength0())
            {
                return false;
            }

            return Remove(ids);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否添加成功</returns>
        public override bool Add(IdT key, IDictionary<string, string[]> value)
        {
            if (Exists(key))
            {
                return false;
            }
            try
            {
                dicCache.Add(key, value);
                dicLastAccessTime.Add(key, DateTimeExtensions.Now);
            }
            catch (ArgumentException) // 忽略添加相同的键异常，为了预防密集的线程过来
            {
                System.Console.WriteLine($"{this.GetType().Name}.发生相同添加相同的key异常(程序忽略),key:{key}.value:{value}");
            }

            return true;
        }

        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public BasicReturnInfo InitCache(IdT userId, CommonUseData comData = null)
        {
            var result = Get(userId);
            if (result == null)
            {
                var reMenuFunCodes = userMenuReader.GetHavePermissionMenuFunCodes(userId, comData);
                if (reMenuFunCodes.Failure())
                {
                    return reMenuFunCodes;
                }
                if (reMenuFunCodes.Data == null)
                {
                    Add(userId, new ConcurrentDictionary<string, string[]>());
                }
                else
                {
                    Add(userId, reMenuFunCodes.Data);
                }

                return reMenuFunCodes;
            }

            return new BasicReturnInfo();
        }

        /// <summary>
        /// 移除键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否移除成功</returns>
        public override bool Remove(IdT key)
        {
            if (key == null)
            {
                return false;
            }
            if (dicCache.ContainsKey(key))
            {
                dicCache.Remove(key);
            }
            if (dicLastAccessTime.ContainsKey(key))
            {
                dicLastAccessTime.Remove(key);
            }

            return false;
        }

        /// <summary>
        /// 移除键数组
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <returns>是否移除成功</returns>
        public override bool Remove(IdT[] keys)
        {
            if (keys.IsNullOrLength0())
            {
                return false;
            }
            foreach (var key in keys)
            {
                if (key == null)
                {
                    continue;
                }
                if (dicCache.ContainsKey(key))
                {
                    dicCache.Remove(key);
                }
                if (dicLastAccessTime.ContainsKey(key))
                {
                    dicLastAccessTime.Remove(key);
                }
            }

            return true;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public override void Clear()
        {
            dicCache.Clear();
            dicLastAccessTime.Clear();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<IdT, IDictionary<string, string[]>> GetCache() => dicCache;
    }
}
