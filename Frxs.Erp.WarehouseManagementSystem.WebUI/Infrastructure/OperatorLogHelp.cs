using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure
{
    public class OperatorLogHelp
    {
        /// <summary>
        /// 仓库管理系统操作写日志
        /// </summary>
        /// <param name="menuID">菜单ID</param>
        /// <param name="action">动作</param>
        /// <param name="remark">备注</param>
        public static void Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum menuID, string action, string remark)
        {
            //可以透过代理服务器
            string userIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //判断是否有代理服务器
            if (string.IsNullOrEmpty(userIp))
            {
                //没有代理服务器,如果有代理服务器获取的是代理服务器的IP
                userIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            var erpIDSdkClient = WorkContext.CreateIDSdkClient();
            erpIDSdkClient.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                MenuID = menuID,
                Action = action,
                IPAddress = userIp,
                Remark = remark
            });
        }
    }
}