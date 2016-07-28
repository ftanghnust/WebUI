/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2016/5/4 15:12:10
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SysParamModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string ParamCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string ParamValue { get; set; }
    }
}