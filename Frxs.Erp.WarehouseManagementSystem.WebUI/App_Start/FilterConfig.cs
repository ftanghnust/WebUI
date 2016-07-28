using System.Web;
using System.Web.Mvc;
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Platform.Utility.Filter;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionFilterAttribute());
            //校验单点登录
            filters.Add(new LoginAuthorizationAttribute());
        }
    }
}