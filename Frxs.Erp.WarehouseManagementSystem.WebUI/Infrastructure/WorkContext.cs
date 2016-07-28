/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2016/3/18 20:21:48
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 当前工作上下文
    /// </summary>
    public class WorkContext
    {
        /// <summary>
        /// 获取到商品服务中心客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public static Frxs.Erp.ServiceCenter.Product.SDK.IApiClient CreateProductSdkClient()
        {
            return new Frxs.Erp.ServiceCenter.Product.SDK.DefaultApiClient(sdkConfigSectionName: "frxsErpProductSdkConfig", defaultUser: () =>
            {
                return new Frxs.Erp.ServiceCenter.Product.SDK.ApiUser()
                {
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName
                };
            });
        }

        /// <summary>
        /// 获取到Order客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public static Frxs.Erp.ServiceCenter.Order.SDK.IApiClient CreateOrderSdkClient()
        {
            return new Frxs.Erp.ServiceCenter.Order.SDK.DefaultApiClient(sdkConfigSectionName: "frxsErpOrderSdkConfig", defaultUser: () =>
            {
                return new Frxs.Erp.ServiceCenter.Order.SDK.ApiUser()
                {
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName
                };
            });
        }

        /// <summary>
        /// 获取到ID客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public static Frxs.Erp.ServiceCenter.ID.SDK.IApiClient CreateIDSdkClient()
        {
            return new Frxs.Erp.ServiceCenter.ID.SDK.DefaultApiClient(sdkConfigSectionName: "frxsErpIDSdkConfig", defaultUser: () =>
            {
                return new Frxs.Erp.ServiceCenter.ID.SDK.ApiUser()
                {
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName
                };
            });
        }
        /// <summary>
        /// 获取到Promotion客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public static Frxs.Erp.ServiceCenter.Promotion.SDK.IApiClient CreatePromotionSdkClient()
        {
            //return new Frxs.Erp.ServiceCenter.Promotion.SDK.DefaultApiClient(sdkConfigSectionName: "frxsErpPromotionSdkConfig");
            return new Frxs.Erp.ServiceCenter.Promotion.SDK.DefaultApiClient(sdkConfigSectionName: "frxsErpPromotionSdkConfig", defaultUser: () =>
            {
                return new Frxs.Erp.ServiceCenter.Promotion.SDK.ApiUser()
                {
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName
                };
            });
        }

        /// <summary>
        /// 获取到Member客户端SDK访问对象
        /// </summary>
        /// <returns></returns>
        public static Frxs.Erp.ServiceCenter.Member.SDK.IApiClient CreateMemberSdkClient()
        {
            return new Frxs.Erp.ServiceCenter.Member.SDK.DefaultApiClient(sdkConfigSectionName: "frxsErpMemberSdkConfig", defaultUser: () =>
            {
                return new Frxs.Erp.ServiceCenter.Member.SDK.ApiUser()
                {
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName
                };
            });
        }

        /// <summary>
        /// 当前登录用户所属的仓库
        /// </summary>
        public static WarehouseIdentity CurrentWarehouse
        {
            get
            {
                var resp = CreateProductSdkClient().Execute(new FrxsErpProductWarehouseGetForSSORequest()
                {
                    WCode = UserIdentity.WCode,
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName
                }, new ServiceCenter.Product.SDK.CacheOptions() { FromLocalCache = true, CacheTime = 30 }); // 缓存在本地，防止重复从服务中心去获取
                if (null == resp || null == resp.Data)
                {
                    System.Web.HttpContext.Current.Response.Redirect(Frxs.ServiceCenter.SSO.Client.AuthorizationManager.Instance.AuthServerUrl, true);
                }
                if (resp.Flag == 0)
                {
                    return new WarehouseIdentity()
                    {
                        WarehouseId = resp.Data.Current.WID.Value,
                        WarehouseName = resp.Data.Current.WName,
                        WarehouseCode = resp.Data.Current.WCode,
                        Parent = new WarehouseIdentity()
                        {
                            WarehouseId = resp.Data.Parent.WID.Value,
                            WarehouseName = resp.Data.Parent.WName,
                            WarehouseCode = resp.Data.Parent.WCode
                        },
                        ParentSubWarehouses = (from item in resp.Data.ParentSubWarehouses
                                               select new WarehouseIdentity()
                                               {
                                                   WarehouseId = item.WID.Value,
                                                   WarehouseCode = item.WCode,
                                                   WarehouseName = item.WName
                                               }).ToList()
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// 获取当前登录用户的信息
        /// </summary>
        public static UserIdentity UserIdentity
        {
            get
            {
                //获取当前登录用户信息
                return AuthorizationManager.Instance.GetUserIdentity();
            }
        }
    }
}