using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Comm;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.DaySettlement;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.DaySettlement
{
    public class DaySettlementController : Controller
    {
        /// <summary>
        /// 日结报表列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DaySettlementList()
        {
            return View();
        }

        /// <summary>
        /// 日结报表列表-数据
        /// </summary>
        /// <returns></returns>
        public ActionResult DaySettlementListData(DaySettlementSearchModel model)
        {
            var orderClient = WorkContext.CreateOrderSdkClient();

            var parm = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetSettlementListRequest();
            if(!string.IsNullOrEmpty(model.SerchTime))
            {
                parm.SerchTime = Convert.ToDateTime(model.SerchTime);
            }
            parm.PageIndex = model.page;
            parm.PageSize = model.rows;
            parm.SubWID = model.SubWID;
            parm.StockName = model.StockName;
            parm.WID = WorkContext.CurrentWarehouse.WarehouseId;

            var jsonStr = "";
            var resp = orderClient.Execute(parm);
            if (resp != null && resp.Data != null)
            {
                var obj = new
                {
                    total = resp.Data.PageCount,
                    rows = resp.Data.SettlementList.OrderByDescending(p=>p.SettleDate)
                };
                jsonStr = obj.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public ActionResult DaySettlementExecute(int subWId, string settleDate)
        {
            var jsonStr = "";

            var settleDateTime = Convert.ToDateTime(settleDate);

            var pdDateTimeStr = "";
            var respParam = CommModel.GetWarehouseSysParams(Models.Enum.SysParams.日结最早时间);
            if (respParam != null && respParam.Count>0)
            {
                pdDateTimeStr = respParam[0].ParamValue;
            }
            else
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "没有设定日结算单业务参数"
                }.ToJsonString();
                return Content(jsonStr);
            }

            var pdDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd "+pdDateTimeStr));

            //如果操作是当天
            if(settleDateTime.ToString("yyyyMMdd")==pdDateTime.ToString("yyyMMdd"))
            {
                if(DateTime.Now<pdDateTime)
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "不能在小于业务设定的 " + pdDateTimeStr + " 之前执行"
                    }.ToJsonString();
                    return Content(jsonStr);
                }
            }
            
            var subWName = "";
            var subWCode = "";
            foreach (var warehouses in WorkContext.CurrentWarehouse.ParentSubWarehouses)
            {
                if(warehouses.WarehouseId==subWId)
                {
                    subWName = warehouses.WarehouseName;
                    subWCode = warehouses.WarehouseCode;
                    break;
                }
            }

            var orderClient = WorkContext.CreateOrderSdkClient();
            orderClient.SetTimeout(600000);
            var resp = orderClient.Execute(new ServiceCenter.Order.SDK.Request.FrxsErpOrderSettlementAddRequest()
            {
                Wid = WorkContext.CurrentWarehouse.WarehouseId,
                SettleDate = settleDate,
                Status = 0,     //0手动
                SubWid = subWId,
                SubWCode = subWCode,
                SubWName = subWName,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                WCode = WorkContext.CurrentWarehouse.WarehouseCode,
                WName = WorkContext.CurrentWarehouse.WarehouseName
            });

            
            if (resp != null && resp.Flag == 0)
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "成功"
                }.ToJsonString();
            }

            return Content(jsonStr);
        }



        /// <summary>
        /// 重新计算
        /// </summary>
        /// <returns></returns>
        public ActionResult DaySettlementAgainExecute(string settleDate, int wid , int subWid)
        {
            var orderClient = WorkContext.CreateOrderSdkClient();
            orderClient.SetTimeout(600000);
            //先删除当天数据
            var respdel = orderClient.Execute(new ServiceCenter.Order.SDK.Request.FrxsErpOrderSettlementDelRequest()
            {
                Wid = WorkContext.CurrentWarehouse.WarehouseId,
                SettleDate = Convert.ToDateTime(settleDate),
                SubWid = subWid,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            var jsonStr = "";

            //是否删除成功
            if (respdel != null && respdel.Flag == 0)
            {
                //添加逻辑不变
                var subWName = "";
                var subWCode = "";
                foreach (var warehouses in WorkContext.CurrentWarehouse.ParentSubWarehouses)
                {
                    if (warehouses.WarehouseId == subWid)
                    {
                        subWName = warehouses.WarehouseName;
                        subWCode = warehouses.WarehouseCode;
                        break;
                    }
                }


                var resp = orderClient.Execute(new ServiceCenter.Order.SDK.Request.FrxsErpOrderSettlementAddRequest()
                {
                    Wid = WorkContext.CurrentWarehouse.WarehouseId,
                    SettleDate = settleDate,
                    Status = 0,     //0手动
                    SubWid = subWid,
                    SubWCode = subWCode,
                    SubWName = subWName,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    WCode = WorkContext.CurrentWarehouse.WarehouseCode,
                    WName = WorkContext.CurrentWarehouse.WarehouseName
                });
                
                if (resp != null && resp.Flag == 0)
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "成功"
                    }.ToJsonString();
                }
            }
            return Content(jsonStr);
        }


        /// <summary>
        /// 日结报表详细
        /// </summary>
        /// <returns></returns>
        public ActionResult DaySettlementDetail()
        {
            return View();
        }


        /// <summary>
        /// 日结报表详细-数据
        /// </summary>
        /// <returns></returns>
        public ActionResult DaySettlementDetilData(DaySettlementDetailSearchModel model)
        {
            var orderClient = WorkContext.CreateOrderSdkClient();

            var parm = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetSettlementDetailListRequest();
            
            parm.PageIndex = model.page;
            parm.PageSize = model.rows;
            parm.ProductName = model.ProductName;
            parm.SKU = model.SKU;
            parm.RefSet_ID = model.RefSet_ID;
            parm.WID = WorkContext.CurrentWarehouse.WarehouseId.ToString();

            var jsonStr = "";
            var resp = orderClient.Execute(parm);
            if (resp != null && resp.Data != null)
            {
                var obj = new
                {
                    total = resp.Data.PageCount,
                    rows = resp.Data.SettDetail,
                    settTotalSum = resp.Data.SettTotalSum,
                    settCurrentSum = resp.Data.SettCurrentSum
                };
                jsonStr = obj.ToJsonString();
            }
            return Content(jsonStr);
        }

    }
}
