using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Report
{
    public class SyncReportController : Controller
    {
        //
        // GET: /SyncReport/

        public ActionResult Index()
        {
            return View();
        }

        #region 页面

        /// <summary>
        /// 销售同步
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSyncSaleOrder()
        {
            return View();
        }

        /// <summary>
        /// 采购同步
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSyncBuyOrder()
        {
            return View();
        }

        /// <summary>
        /// 商贸系统同步
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSynTradeOrder()
        {
            return View();
        }

        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表-同步销售表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetSyncReportData(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetSyncReportData(true);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="SarteTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="SyncTableName"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(522001, new int[] { 52200105 })]
        public ActionResult SyncButtonDate(string SarteTime, string EndTime, int SyncTableName)
        {
            string result = string.Empty;
            try
            {

                var serviceCenter = WorkContext.CreateOrderSdkClient();
                if (!string.IsNullOrEmpty(SarteTime) && SarteTime.Length > 1)
                {
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSyncReportSetRequest()
                    {
                        SarteTime = SarteTime,
                        EndTime = EndTime,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SyncTableName = SyncTableName == 0 ? true : false
                    });
                    if (resp != null && resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = resp.Data.ToString()
                        }.ToJsonString();
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = resp.Info
                        }.ToJsonString();
                    }
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中确认数据"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("出现异常：{0}", ex.Message)
                }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 商贸系统同步数据
        /// </summary>
        /// <param name="SarteTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="SyncTableName"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(522001, new int[] { 52200105 })]
        public ActionResult SyncTradeDate(string SarteTime, string EndTime, int SyncTableName)
        {
            string result = string.Empty;
            try
            {

                var serviceCenter = WorkContext.CreateOrderSdkClient();
                if (!string.IsNullOrEmpty(SarteTime) && SarteTime.Length > 1)
                {
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSyncReportSetRequest()
                    {
                        SarteTime = SarteTime,
                        EndTime = EndTime,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SyncTableName = SyncTableName == 0 ? true : false
                    });
                    if (resp != null && resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = resp.Data.ToString()
                        }.ToJsonString();
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = resp.Info
                        }.ToJsonString();
                    }
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中确认数据"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("出现异常：{0}", ex.Message)
                }.ToJsonString();
            }
            return Content(result);
        }

        #endregion
    }
}
