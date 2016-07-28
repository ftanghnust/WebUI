/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2016/1/7 15:26:34
 * *******************************************************/
using System;
using System.Collections.Generic;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    ///List Extensions
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// 添加一个新的对象到集合，并返回新的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">当前集合</param>
        /// <param name="objs"></param>
        /// <returns>返回附加后的新集合</returns> 
        public static IList<T> Append<T>(this IList<T> list, params T[] objs)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list");
            }
            if (null == objs || objs.Length == 0)
            {
                return list;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                list.Add(objs[i]);
            }
            return list;
        }

        /// <summary>
        /// 添加一个新的对象到集合，并返回新的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">当前集合</param>
        /// <param name="default">通过一个委托返回待添加的集合</param>
        /// <returns>返回附加后的新集合</returns>
        public static IList<T> Append<T>(this IList<T> list, Func<T> @default)
        {
            return list.Append(@default());
        }

        /// <summary>
        /// 判断集合是否为空（即包含0个元素）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>包含0个元素，返回true</returns>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        public static bool IsEmpty<T>(this IList<T> list)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list");
            }
            return list.Count == 0;
        }
    }
}
