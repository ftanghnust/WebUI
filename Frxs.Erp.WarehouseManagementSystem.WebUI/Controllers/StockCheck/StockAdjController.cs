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
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.StockCheck
{
    public class StockAdjController : BaseController
    {
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetStockAdjList(StockAdjQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new StockAdjModel().GetStockAdjPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        public ActionResult GetStockAdj(string id)
        {
            var model = new StockAdjModel();
            model = new StockAdjModel().GetStockAdj(id);
            return Content(model.ToJsonString());
        }

        [ValidateInput(false)]
        public ActionResult GetProductList(ProductListQuery cpm)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsAddedListGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKU = (cpm.searchType == "SKU") ? cpm.searchKey : null,
                    //SKULikeSearch = (cpm.searchType == "SKU") ? true : false,
                    ProductName = (cpm.searchType == "ProductName") ? cpm.searchKey : null,
                    BarCode = (cpm.searchType == "BarCode") ? cpm.searchKey : null,
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort
                });
                if (resp != null && resp.Flag == 0)
                {
                    //result = new ResultData
                    //{
                    //    Flag = ConstDefinition.FLAG_SUCCESS,
                    //    Info = "OK",
                    //    Data = resp.Data.ItemList
                    //}.ToJsonString();
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    result = obj.ToJsonString();
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
        /// 新增盘盈
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520716, 52071601)]
        [ValidateInput(false)]
        public ActionResult AddStockOverAdj(StockAdjModel model)
        {
            var result = new StockAdjModel().AddStockAdj(model);
            return Content(result.ToJsonString());
        }

        /// <summary>
        /// 新增盘亏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071701)]
        [ValidateInput(false)]
        public ActionResult AddStockLossAdj(StockAdjModel model)
        {
            var result = new StockAdjModel().AddStockAdj(model);
            return Content(result.ToJsonString());
        }

        /// <summary>
        /// 编辑盘盈
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520716, 52071602)]
        [ValidateInput(false)]
        public ActionResult EditStockOverAdj(StockAdjModel model)
        {
            var result = new StockAdjModel().EditStockAdj(model);
            return Content(result.ToJsonString());
        }

        /// <summary>
        /// 编辑盘亏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071702)]
        [ValidateInput(false)]
        public ActionResult EditStockLossAdj(StockAdjModel model)
        {
            var result = new StockAdjModel().EditStockAdj(model);
            return Content(result.ToJsonString());
        }

        /// <summary>
        /// 删除盘盈
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520716, 52071603)]
        public ActionResult DeleteStockOverAdj(string ids, int adjType)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    int rows = new StockAdjModel().DeleteStockAdj(ids, adjType);
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功删除{0}条数据", rows)
                    }.ToJsonString();
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中删除数据"
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
        /// 删除盘亏
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071703)]
        public ActionResult DeleteStockLossAdj(string ids, int adjType)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    int rows = new StockAdjModel().DeleteStockAdj(ids, adjType);
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功删除{0}条数据", rows)
                    }.ToJsonString();
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中删除数据"
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
        /// 确认盘盈
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520716, 52071604)]
        public ActionResult ConfirmStockOverAdj(string ids, int flag, int adjType)
        {
            return Content(ConfirmStockAdj(ids, flag, adjType));
        }

        /// <summary>
        /// 反确认盘盈
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520716, 52071606)]
        public ActionResult UnConfirmStockOverAdj(string ids, int flag, int adjType)
        {
            return Content(ConfirmStockAdj(ids, flag, adjType));
        }

        /// <summary>
        /// 确认盘亏
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071704)]
        public ActionResult ConfirmStockLossAdj(string ids, int flag, int adjType)
        {
            return Content(ConfirmStockAdj(ids, flag, adjType));
        }

        /// <summary>
        /// 反确认盘亏
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071706)]
        public ActionResult UnConfirmStockLossAdj(string ids, int flag, int adjType)
        {
            return Content(ConfirmStockAdj(ids, flag, adjType));
        }

        /// <summary>
        /// 确认/反确认 盘盈盘亏单
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag">1指确认,0指反确认</param>
        /// <param name="adjType">0指盘盈单,1指盘亏单</param>
        /// <returns></returns>
        public string ConfirmStockAdj(string ids, int flag, int adjType)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjConfirmRequest()
            {
                IdList = ids.Split(',').ToList(),
                Flag = flag,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            var result = new ResultData();
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (adjType.Equals(0))
                {
                    if (flag.Equals(1))
                    {
                        OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                                               ConstDefinition.XSOperatorActionSure, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionSure, ids));
                    }
                    else if (flag.Equals(0))
                    {
                        OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                                               ConstDefinition.XSOperatorActionNoSure, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionNoSure, ids));
                    }
                }
                else if (adjType.Equals(1))
                {
                    if (flag.Equals(1))
                    {
                        OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                                               ConstDefinition.XSOperatorActionSure, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionSure, ids));
                    }
                    else if (flag.Equals(0))
                    {
                        OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                                               ConstDefinition.XSOperatorActionNoSure, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionNoSure, ids));
                    }
                }
                //细化提示，盘亏单的确认操作，返回接口给出的提示信息
                string info = (flag == 1 && adjType == 1) ? resp.Info : "确认成功";
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = flag.Equals(1) ? info : "反确认成功"
                };
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
            }
            return result.ToJsonString();
        }

        /// <summary>
        /// 过账盘盈
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520716, 52071605)]
        public ActionResult PostingStockOverAdj(string ids, int adjType)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            ServiceCenter.SetTimeout(300 * 1000);//盘点过账操作时间随着明细条数增加而增加。防止超时错误
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjPostingRequest()
            {
                IdList = ids.Split(',').ToList(),
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            var result = new ResultData();
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (adjType.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                                           ConstDefinition.XSOperatorActionEffective, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionEffective, ids));
                }
                else if (adjType.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                                          ConstDefinition.XSOperatorActionEffective, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionEffective, ids));
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "过账成功"
                };
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
            }
            return Content(result.ToJsonString());
        }

        /// <summary>
        /// 过账盘亏
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="adjType"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071705)]
        public ActionResult PostingStockLossAdj(string ids, int adjType)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            ServiceCenter.SetTimeout(300 * 1000);//盘点过账操作时间随着明细条数增加而增加。防止超时错误
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjPostingRequest()
            {
                IdList = ids.Split(',').ToList(),
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            var result = new ResultData();
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (adjType.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                                           ConstDefinition.XSOperatorActionEffective, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionEffective, ids));
                }
                else if (adjType.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                                          ConstDefinition.XSOperatorActionEffective, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionEffective, ids));
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "过账成功"
                };
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
            }
            return Content(result.ToJsonString());
        }

        public ActionResult GetStockAdjDetailCount(string id)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailCountGetRequest()
            {
                AdjID = id,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            var result = new ResultData();
            if (resp != null && resp.Flag == 0)
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = resp.Info,
                    Data = resp.Data
                };
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
            }
            return Content(result.ToJsonString());
        }

        /// <summary>
        /// 汇总生成盘亏单
        /// </summary>
        /// <param name="ids">要汇总生成盘亏单的自动盘盈单集合(,分隔)</param>
        /// <param name="subWCode"></param>
        /// <param name="subWid"></param>
        /// <param name="subWName"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520717, 52071701)]
        public ActionResult GatherAddStockLessAdj( string ids, int subWid, string subWCode, string subWName)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            ServiceCenter.SetTimeout(300 * 1000);//当操作的数据较多时接口执行可能需要比较长的时间
            var requestDto = new ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjCreateDeficitRequest()
            {
                AdjIds = ids.Split(',').ToList(),
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode,
                WName = WorkContext.CurrentWarehouse.Parent.WarehouseName,
                //此处原有的逻辑有错误，当权限高的时候，SubWid的值可能取到父一级的ID
                //SubWid = WorkContext.CurrentWarehouse.WarehouseId,
                //SubWcode = WorkContext.CurrentWarehouse.WarehouseCode,
                //SubWName = WorkContext.CurrentWarehouse.WarehouseName,
                //这里的三个参数应该从前端传过来

                SubWid = subWid,
                SubWcode = subWCode,
                SubWName = subWName,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };
            var resp = ServiceCenter.Execute(requestDto);
            var result = new ResultData();
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                      ConstDefinition.XSOperatorActionAdd, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionAdd, ids));
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "新增盘亏单成功"
                };
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
            }
            return Content(result.ToJsonString());
        }


       
    }
}
