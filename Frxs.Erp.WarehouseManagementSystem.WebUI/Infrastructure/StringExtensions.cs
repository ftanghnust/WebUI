/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2016/3/17 9:03:35
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// String.Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 将当前字符串重复count次
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Replicate(this string value, int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count参数错误");
            }
            string text = string.Empty;
            for (int i = 0; i < count; i++)
            {
                text = text + value;
            }
            return text;
        }
    }
}