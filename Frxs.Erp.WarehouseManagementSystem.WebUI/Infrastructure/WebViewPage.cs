/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2015/12/31 13:00:01
 * *******************************************************/
using System;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 客户端的程序如果是MVC模式的，需要将VIEWS文件夹下的web.config文件基类配置成此抽象视图类
    /// </summary>
    /// <typeparam name="TModel">强类型视图抽象类</typeparam>
    public abstract class WebViewPage<TModel> : Frxs.ServiceCenter.SSO.Client.WebViewPage<TModel>
    {
        /// <summary>
        /// 当前登录用户所属的仓库信息
        /// </summary>
        public WarehouseIdentity CurrentWarehouse
        {
            get
            {
                return WorkContext.CurrentWarehouse;
            }
        }
    }

    /// <summary>
    /// 弱类型的视图抽象类
    /// </summary>
    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }

}
