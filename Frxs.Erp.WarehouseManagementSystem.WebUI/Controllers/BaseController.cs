using System;
using System.Collections.Generic;
using System.Linq;
using Frxs.Platform.Utility.Web;
using System.Web;
using Frxs.Platform.Utility;
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 所有controller基类
    /// </summary>
    public class BaseController : Frxs.Platform.Utility.Web.BaseController
    {
        /// <summary>
        /// 获取到商品服务中心客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public Frxs.Erp.ServiceCenter.Product.SDK.IApiClient ErpProductSdkClient
        {
            get
            {
                return WorkContext.CreateProductSdkClient();
            }
        }

        /// <summary>
        /// 获取到Order客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public Frxs.Erp.ServiceCenter.Order.SDK.IApiClient ErpOrderSdkClient
        {
            get
            {
                return WorkContext.CreateOrderSdkClient();
            }
        }

        /// <summary>
        /// 获取到ID客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public Frxs.Erp.ServiceCenter.ID.SDK.IApiClient ErpIDSdkClient
        {
            get
            {
                return WorkContext.CreateIDSdkClient();
            }
        }
        /// <summary>
        /// 获取到Promotion客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public Frxs.Erp.ServiceCenter.Promotion.SDK.IApiClient ErpPromotionSdkClient
        {
            get
            {
                return WorkContext.CreatePromotionSdkClient();
            }
        }

        /// <summary>
        /// AJAX调用的时候返回成功JSON对象
        /// </summary>
        /// <param name="info">执行成功后返回的消息</param>
        /// <returns></returns>
        public AjaxPostResult SuccessResult(string info = "")
        {
            return new AjaxPostResult(flag: ConstDefinition.FLAG_SUCCESS, info: info);
        }

        /// <summary>
        /// AJAX调用的时候返回失败JSON对象
        /// </summary>
        /// <param name="info">执行错误，错误消息</param>
        /// <returns></returns>
        public AjaxPostResult ErrorResult(string info)
        {
            return new AjaxPostResult(flag: "ERROR", info: info);
        }

        /// <summary>
        /// 返回失败的JSON对象
        /// </summary>
        /// <param name="info">失败消息</param>
        /// <returns></returns>
        public AjaxPostResult FailResult(string info)
        {
            return new AjaxPostResult(flag: ConstDefinition.FLAG_FAIL, info: info);
        }

        /// <summary>
        /// 返回异常的JSON对象
        /// </summary>
        /// <param name="info">异常消息</param>
        /// <returns></returns>
        public AjaxPostResult ExceptionResult(string info)
        {
            return new AjaxPostResult(flag: ConstDefinition.FLAG_EXCEPTION, info: info);
        }

        /// <summary>
        /// 获取当前登录用户的信息
        /// </summary>
        public UserIdentity UserIdentity
        {
            get
            {
                return WorkContext.UserIdentity;
            }
        }

        /// <summary>
        /// 当前登录用户所属的仓库
        /// </summary>
        public WarehouseIdentity CurrentWarehouse
        {
            get
            {
                return WorkContext.CurrentWarehouse;
            }
        }
    }
}