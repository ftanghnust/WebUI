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
using System.Data;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class ReportModel : BaseModel
    {
        #region 获取订单库数据

        /// <summary>
        /// 获取订单库数据
        /// </summary>
        /// <param name="falg">是否为导出，true：查询，false：导出</param>
        /// <param name="procedurename">存储过程名称 </param>
        /// <returns></returns>
        public string GetReportData(bool falg, string procedurename)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();
                IList<ServiceCenter.Order.SDK.Request.FrxsErpOrderReportGetRequest.parameter> list = new List<ServiceCenter.Order.SDK.Request.FrxsErpOrderReportGetRequest.parameter>();

                if (conditionDict != null || conditionDict.Count > 0)
                {
                    foreach (var item in conditionDict)
                    {
                        var parameters = new ServiceCenter.Order.SDK.Request.FrxsErpOrderReportGetRequest.parameter();
                        parameters.key = item.Key;
                        parameters.value = item.Value.ToString();
                        list.Add(parameters);
                    }
                }
                list.Add(new ServiceCenter.Order.SDK.Request.FrxsErpOrderReportGetRequest.parameter()
                {
                    key = "WID",
                    value = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString()
                });
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderReportGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    Procedure_Name = procedurename,
                    parameters = list
                });



                if (resp != null && resp.Flag == 0)
                {
                    if (falg)
                    {
                        jsonStr = "{\"total\": 1000000,\"rows\":" + resp.Data + "}";
                    }
                    else
                    {
                        jsonStr = resp.Data;
                    }
                }
                else
                {
                    if (!falg)
                    {
                        jsonStr = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        #endregion



        #region 获取数据

        /// <summary>
        /// 获取数据-基础库
        /// </summary>
        /// <param name="falg">是否为导出，true：查询，false：导出</param>
        /// <param name="procedurename">存储过程名称 </param>
        /// <returns></returns>
        public string GetProductReportData(bool falg, string procedurename)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();
                IList<ServiceCenter.Product.SDK.Request.FrxsErpProductReportGetRequest.parameter> list = new List<ServiceCenter.Product.SDK.Request.FrxsErpProductReportGetRequest.parameter>();

                if (conditionDict != null || conditionDict.Count > 0)
                {
                    foreach (var item in conditionDict)
                    {
                        var parameters = new ServiceCenter.Product.SDK.Request.FrxsErpProductReportGetRequest.parameter();
                        parameters.key = item.Key;
                        parameters.value = item.Value.ToString();
                        list.Add(parameters);
                    }
                }
                list.Add(new ServiceCenter.Product.SDK.Request.FrxsErpProductReportGetRequest.parameter()
                {
                    key = "WID",
                    value = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString()
                });
                var resp = serviceCenter.Execute(new ServiceCenter.Product.SDK.Request.FrxsErpProductReportGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    Procedure_Name = procedurename,
                    parameters = list
                });



                if (resp != null && resp.Flag == 0)
                {
                    if (falg)
                    {
                        jsonStr = "{\"total\": 1000000,\"rows\":" + resp.Data + "}";
                    }
                    else
                    {
                        jsonStr = resp.Data;
                    }
                }
                else
                {
                    if (!falg)
                    {
                        jsonStr = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        #endregion


        #region 获取同步数据
        /// <summary>
        /// 获取数据-同步报表
        /// </summary>
        /// <param name="falg"></param>
        /// <returns></returns>
        public string GetSyncReportData(bool falg)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSyncReportGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SarteTime = conditionDict.ContainsKey("SarteTime") ? Utils.NoHtml(conditionDict["SarteTime"].ToString()) : DateTime.Now.ToLongDateString(),
                    EndTime = conditionDict.ContainsKey("EndTime") ? Utils.NoHtml(conditionDict["EndTime"].ToString()) : DateTime.Now.ToLongDateString(),
                    fale = conditionDict.ContainsKey("Status") ? int.Parse(Utils.NoHtml(conditionDict["Status"].ToString())) : 0,
                    SyncTableName = Utils.NoHtml(conditionDict["SyncTableName"].ToString()) == "1" ? true : false
                });



                if (resp != null && resp.Flag == 0)
                {
                    if (falg)
                    {
                        jsonStr = "{\"total\": 1000000,\"rows\":" + resp.Data + "}";
                    }
                    else
                    {
                        jsonStr = resp.Data;
                    }
                }
                else
                {
                    if (!falg)
                    {
                        jsonStr = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        #endregion
    }
}