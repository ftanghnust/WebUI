using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Request;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Platform.Utility.Map;
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    public class SaleOrderSendController : BaseController
    {
        [AuthorizeMenuFilter(520316)]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult SearchSaleOrderSend(string ShopCode, string OrderId)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberGetSearchOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ShopCode = ShopCode,
                    OrderId = OrderId
                });

                if (resp == null)
                {
                    result = new ResultData
                   {
                       Flag = ConstDefinition.FLAG_FAIL,
                       Info = "操作失败",
                       Data = null
                   };
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
                else if (resp.Flag == 0)
                {
                    if (resp.Data.SaleOrderPre != null)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = resp.Data.SaleOrderPre
                        };
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }


        /// <summary>
        /// 发货线路列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSaleOrderLineList()
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetSaleOrderSendNumberLineListRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.SaleOrderSendNumberLineList.Count, rows = resp.Data.SaleOrderSendNumberLineList };
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 通过线路获取线路下门店顺利列表
        /// </summary>
        /// <param name="lineId">线路编号</param>
        /// <returns></returns>
        public ActionResult GetSaleOrderShopList(string lineId)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetSaleOrderSendNumberShopListRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = int.Parse(lineId)
                });
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.SaleOrderSendNumberShopList.Count, rows = resp.Data.SaleOrderSendNumberShopList };
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        [AuthorizeButtonFiter(520316, 52031601)]
        public ActionResult ChangeSaleOrderLineOrderUp(int lineId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp =
                    serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.
                                              FrxsErpOrderSaleOrderSendNumberChangeLineOrderRequest()
                                              {
                                                  WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                                                  LineID = lineId,
                                                  ChangeType = changeType
                                              });


                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行路线上移操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(
                        ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                        ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货线路顺序[{1}]", "上移", lineId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                                     {
                                         Flag = ConstDefinition.FLAG_SUCCESS,
                                         Info = "操作成功",
                                         Data = null
                                     };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031602)]
        public ActionResult ChangeSaleOrderLineOrderDown(int lineId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeLineOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    ChangeType = changeType
                });

                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行路线下移操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货线路顺序[{1}]", "下移", lineId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031603)]
        public ActionResult ChangeSaleOrderLineOrderTop(int lineId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeLineOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    ChangeType = changeType
                });

                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行路线置顶操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货线路顺序[{1}]", "置顶", lineId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }

            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031603)]
        public ActionResult ChangeSaleOrderLineOrderBottom(int lineId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeLineOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    ChangeType = changeType
                });

                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行路线置底操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货线路顺序[{1}]", "置底", lineId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031601)]
        public ActionResult ChangeSaleOrderShopOrderUp(int lineId, string orderId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeShopOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    OrderId = orderId,
                    ChangeType = changeType
                });

                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行门店上移操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货门店订单顺序[{1}]", "上移", orderId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031602)]
        public ActionResult ChangeSaleOrderShopOrderDown(int lineId, string orderId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeShopOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    OrderId = orderId,
                    ChangeType = changeType
                });
                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行门店上移操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货门店订单顺序[{1}]", "下移", orderId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031603)]
        public ActionResult ChangeSaleOrderShopOrderTop(int lineId, string orderId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeShopOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    OrderId = orderId,
                    ChangeType = changeType
                });
                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行门店置顶操作,请刷新界面");
                }
                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货门店订单顺序[{1}]", "置顶", orderId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }

        [AuthorizeButtonFiter(520316, 52031603)]
        public ActionResult ChangeSaleOrderShopOrderBottom(int lineId, string orderId, int changeType)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderSendNumberChangeShopOrderRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    LineID = lineId,
                    OrderId = orderId,
                    ChangeType = changeType
                });
                if (resp == null)
                {
                    return ErrorResult("数据传输有误,不能进行门店置底操作,请刷新界面");
                }

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}发货门店订单顺序[{1}]", "置底", orderId));

                    if (resp.Data.result > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = null
                        };
                    }
                }
                else if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }
    }
}
