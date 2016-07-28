using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 下拉框数据绑定类
    /// 方法只能为静态方法，返回值必须为SelectList，参数支持无参或一个来源于ConstDefinition类中定义的string类型参数
    /// </summary>
    public class SelectDataBinder
    {
        #region 获取性别
        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns></returns>
        public static SelectList GetGender()
        {
            return new SelectList(new[] { new { Text = "男", Value = true }, new { Text = "女", Value = false } }, "Value", "Text", true);
        }
        #endregion      

    }
}