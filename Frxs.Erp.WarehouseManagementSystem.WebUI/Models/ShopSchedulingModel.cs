using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using System.Web.Mvc;
using Frxs.Platform.Utility.Map;
using System.Runtime.Serialization;
using System.Collections;
using Frxs.Platform.Utility.Filter;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// ShopScheduling实体类
    /// </summary>
    [Serializable]
    public partial class ShopSchedulingModel : BaseModel
    {



        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetShopSchedulingPageData(ShopSchedulingQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateProductSdkClient();
                //  Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGetListAndOrderRequest()
                {
                    IsOrder = !string.IsNullOrEmpty(cpm.IsOrder) ? Utils.NoHtml(cpm.IsOrder) : null,
                    LineId = !string.IsNullOrEmpty(cpm.LineId) ? Utils.NoHtml(cpm.LineId) : null,
                    ShopType = !string.IsNullOrEmpty(cpm.ShopType) ? int.Parse(cpm.ShopType) : (int?)null,
                    SearchDate = DateTime.Now.ToString("yyyyMMdd"),
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString()

                });

                if (resp != null && resp.Flag == 0)
                {
                    IList<FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData> shopGetList = resp.Data;
                   //销售订单
                    var orderSdkClient = WorkContext.CreateOrderSdkClient();
                    var orderResp = orderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderGetExtRequest()
                    {
                        SearchDate = DateTime.Now.ToString("yyyyMMdd"),
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId //因为方法为公用，此处照传
                    });

                    //促销订单
                    var promotionSdkClient = WorkContext.CreatePromotionSdkClient();
                    var promotionResp = promotionSdkClient.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionSaleOrderShopGetRequest()
                    {
                        SearchDate = DateTime.Now.ToString("yyyyMMdd"),
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId //因为方法为公用，此处照传
                    });


                    if (orderResp.Flag == 0)
                    {
                        IList<FrxsErpOrdervSaleOrderGetExtResp.FrxsErpOrdervSaleOrderGetExtRespData> saleOrderList = orderResp.Data;
                        if (saleOrderList != null)
                        {
                            foreach (FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData shopObj in shopGetList)
                            {
                                foreach (FrxsErpOrdervSaleOrderGetExtResp.FrxsErpOrdervSaleOrderGetExtRespData orderObj in saleOrderList)
                                {
                                    if (shopObj.ShopID == orderObj.ShopID)
                                    {
                                        shopObj.OrderId = orderObj.OrderId;
                                    }
                                }
                            }
                        }
                    }

                    if (promotionResp.Flag == 0)
                    {
                        IList<FrxsErpPromotionSaleOrderShopGetResp.FrxsErpPromotionSaleOrderShopGetRespData> saleOrderList2 = promotionResp.Data;
                        if (saleOrderList2 != null)
                        {
                            foreach (FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData shopObj in shopGetList)
                            {
                                foreach (FrxsErpPromotionSaleOrderShopGetResp.FrxsErpPromotionSaleOrderShopGetRespData orderObj2 in saleOrderList2)
                                {
                                    if (shopObj.ShopID == orderObj2.ShopID)
                                    {
                                        shopObj.OrderId = orderObj2.OrderId;
                                    }
                                }
                            }
                        }
                    }

                    IList<FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData> resultList = new List<FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData>();
                    foreach (FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData shopObj in shopGetList)
                    {
                        shopObj.DeliverWeek = ReturnDeliverWeek(shopObj);
                        if (string.IsNullOrEmpty(cpm.IsOrder))
                        {
                            resultList.Add(shopObj);
                        }
                        else
                        {
                            if (cpm.IsOrder == "1")
                            {
                                if (!string.IsNullOrEmpty(shopObj.OrderId))
                                {
                                    resultList.Add(shopObj);
                                }
                            }
                            else if (cpm.IsOrder == "0")
                            {
                                if (string.IsNullOrEmpty(shopObj.OrderId))
                                {
                                    resultList.Add(shopObj);
                                }
                            }
                        }
                    }

                    var obj = resultList;
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        #endregion


        /// <summary>
        /// 返回配送周期
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ReturnDeliverWeek(FrxsErpProductShopGetListAndOrderResp.FrxsErpProductShopGetListAndOrderRespData obj)
        {
            StringBuilder resultStr = new StringBuilder();
            string resutl = "";
            if (obj != null)
            {
                resultStr.AppendFormat("{0}", obj.SendW1 == 1 ? "周一," : "");
                resultStr.AppendFormat("{0}", obj.SendW2 == 1 ? "周二," : "");
                resultStr.AppendFormat("{0}", obj.SendW3 == 1 ? "周三," : "");
                resultStr.AppendFormat("{0}", obj.SendW4 == 1 ? "周四," : "");
                resultStr.AppendFormat("{0}", obj.SendW5 == 1 ? "周五," : "");
                resultStr.AppendFormat("{0}", obj.SendW6 == 1 ? "周六," : "");
                resultStr.AppendFormat("{0}", obj.SendW7 == 1 ? "周日," : "");
                if (resultStr.Length > 0)
                {
                    resutl = resultStr.ToString().Substring(0, resultStr.Length - 1);
                }

            }
            return resutl;
        }

    }

    /// <summary>
    /// 查询类
    /// </summary>
    public class ShopSchedulingQuery : BasePageModel
    {


        #region 扩展字段
        /// <summary>
        /// SearchDate
        /// </summary>

        [DisplayName("SearchDate")]
        public string SearchDate { get; set; }

        /// <summary>
        /// LineId
        /// </summary>

        [DisplayName("LineId")]
        public string LineId { get; set; }


        /// <summary>
        /// ShopType
        /// </summary>

        [DisplayName("ShopType")]
        public string ShopType { get; set; }


        /// <summary>
        /// IsOrder
        /// </summary>
        [DisplayName("IsOrder")]
        public string IsOrder { get; set; }

        #endregion
    }

}