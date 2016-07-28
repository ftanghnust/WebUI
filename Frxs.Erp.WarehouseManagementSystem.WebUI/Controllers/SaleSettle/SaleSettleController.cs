using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Sale;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Excel;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.SaleSettle
{
    /// <summary>
    /// 结算单
    /// </summary>
    public class SaleSettleController : BaseController
    {

        /// <summary>
        /// 结算单查询页面
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520611)]
        public ActionResult SaleSettleList()
        {
            return View();
        }


        /// <summary>
        /// 获取结算单据列表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeMenuFilter(520611)]
        [ValidateInput(false)]
        public ActionResult GetSaleSettleList(SaleSettleListSearchModel model)
        {
            string jsonStr = "[]";
            try
            {
                string sortBy = "SettleTime  desc";
                if (!model.sort.IsNullOrEmpty() && !model.order.IsNullOrEmpty())
                {
                    sortBy = model.sort + "  " + model.order;
                }

                var r = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleGetListRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    SettleID = model.SettleID == null ? null : model.SettleID.Trim(),
                    SettleType = model.SettleType == null ? null : model.SettleType.Trim(),
                    ShopCode = model.ShopCode == null ? null : model.ShopCode.Trim(),
                    ShopName = model.ShopName == null ? null : model.ShopName.Trim(),
                    PageIndex = model.page,
                    PageSize = model.rows,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    SortBy = sortBy,
                    Status = model.Status
                };

                //获取列表
                var resp = WorkContext.CreateOrderSdkClient().Execute(r);
                if (resp != null && resp.Data != null)
                {
                    var obj = new {total = resp.Data.TotalRecords, rows = resp.Data.ItemList, SubAmt = resp.Data.SubAmt};
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
        /// 门店结算单新增和编辑界面
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleSettleAddOrEdit(string settleId)
        {
            return View();
        }

        /// <summary>
        /// 门店结算单单条数据明细
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSaleSettleInfoNew(GetSaleSettleInfoModel info)
        {
            string jsonStr = "[]";
            try
            {
                var r = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleGetModelRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SettleID = Utils.NoHtml(info.SettleID).Trim() == "" ? null : Utils.NoHtml(info.SettleID).Trim(),
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                };
                //获取列表
                var resp = WorkContext.CreateOrderSdkClient().Execute(r);
                if (resp == null)
                {
                    return ErrorResult("读取数据错误");
                }

                if (resp.Flag == 0)
                {
                    List<Models.Sale.SaleSettleDetail> saleSettleDetailList = resp.Data.SaleSettleDetailList.Select(item => new SaleSettleDetail
                    {
                        FeeName = item.FeeName,
                        BillType = item.BillType,
                        BillID = item.BillID,
                        BillDate = item.BillDate,
                        BillDetailsID = item.BillDetailsID,
                        FeeCode = item.FeeCode,
                        BillTypeName = item.BillTypeStr,
                        Remark = item.Remark,
                        BillAmt = item.BillAmt.ToString("0.0000"),
                        BillAddAmt = item.BillAddAmt.ToString("0.0000"),
                        BillPayAmt = item.BillPayAmt.ToString("0.0000")
                    }).ToList();

                    SaleSettleViewModel data = new SaleSettleViewModel
                    {
                        SaleSettle = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<Models.Sale.SaleSettle>(resp.Data.SaleSettle),
                        SaleSettleDetailList = new { total = saleSettleDetailList.Count, rows = saleSettleDetailList }.ToJson()
                    };

                    return new AjaxPostResult(flag: ConstDefinition.FLAG_SUCCESS, info: resp.Info, data: data);
                }
                else
                {
                    return ErrorResult(resp.Info);
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
        /// 查看单个结算单页面
        /// </summary>
        /// <param name="settleId">结算编号</param>
        /// <returns></returns>
        public ActionResult ShowSaleSettle(string settleId)
        {
            return View();
        }


        /// <summary>
        /// 获取单个结算单数据(废弃)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ActionResult GetSaleSettleInfo(GetSaleSettleInfoModel info)
        {
            string jsonStr = "[]";
            try
            {
                var r = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleGetModelRequest
                            {
                                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                                SettleID = Utils.NoHtml(info.SettleID).Trim() == "" ? null : Utils.NoHtml(info.SettleID).Trim(),
                                UserId = WorkContext.UserIdentity.UserId,
                                UserName = WorkContext.UserIdentity.UserName
                            };
                //获取列表
                var resp = WorkContext.CreateOrderSdkClient().Execute(r);
                if (resp == null)
                {
                    return ErrorResult("读取数据错误");
                }
                if (resp.Flag == 0)
                {
                    List<Models.Sale.SaleSettleDetail> saleSettleDetailList = resp.Data.SaleSettleDetailList.Select(item => new SaleSettleDetail
                                                        {
                                                            FeeName = item.FeeName,
                                                            BillType = item.BillType,
                                                            BillID = item.BillID,
                                                            BillAmt = item.BillAmt.ToString("0.0000"),
                                                            BillAddAmt = item.BillAddAmt.ToString("0.0000"),
                                                            BillPayAmt = item.BillPayAmt.ToString("0.0000")
                                                        }).ToList();

                    SaleSettleViewModel data = new SaleSettleViewModel
                                                   {
                                                       SaleSettle = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<Models.Sale.SaleSettle>(resp.Data.SaleSettle),
                                                       // SaleSettleDetailList = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<FrxsErpOrderSaleSettleGetModelResp.SaleSettleDetail, Models.Sale.SaleSettleDetail>(resp.Data.SaleSettleDetailList)
                                                       //SaleSettleDetailList = saleSettleDetailList
                                                   };

                    return new AjaxPostResult(flag: ConstDefinition.FLAG_SUCCESS, info: resp.Info, data: data);
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
        /// 选取要结算的单据
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectSaleSettleDetail()
        {
            return View();
        }

        /// <summary>
        ///  选取要结算的单据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSaleSettleDetailList(SaleSettleDetailSearchModel model)
        {
            string jsonStr = "[]";
            try
            {
                var r = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderFreeGetListRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ShopID = model.ShopID,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                };
                //获取列表
                var resp = WorkContext.CreateOrderSdkClient().Execute(r);
                if (resp == null)
                {
                    return ErrorResult("读取数据错误");
                }
                if (resp.Flag == 0 && resp.Data != null)
                {
                    List<Models.Sale.SaleSettleDetail> saleSettleDetailList = resp.Data.Select(item => new SaleSettleDetail
                    {
                        FeeName = item.FeeName,
                        BillType = item.BillType,
                        BillID = item.BillID,
                        BillDate = item.BillDate,
                        BillDetailsID = item.BillDetailsID,
                        FeeCode = item.FeeCode,
                        Remark = item.Remark,
                        BillAmt = item.BillAmt.ToString("0.0000"),
                        BillAddAmt = item.BillAddAmt.ToString("0.0000"),
                        BillPayAmt = item.BillPayAmt.ToString("0.0000")
                    }).ToList();
                    var obj = new { total = resp.Data.Count, rows = saleSettleDetailList };
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
        /// 选取门店
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectShop()
        {
            return View();
        }


        /// <summary>
        /// 新增和编辑方法的保存数据
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520611, new int[] { 52061105, 52061106 })]
        public ActionResult SaleSettleAddOrEditeHandle(string jsonData, string jsonDetails)
        {
            string result = string.Empty;
            jsonData = Frxs.Platform.Utility.Utils.UrlDecode(jsonData);
            jsonDetails = Frxs.Platform.Utility.Utils.UrlDecode(jsonDetails);

            try
            {
                var saleSettleAdd = Frxs.Platform.Utility.Json.JsonHelper.FromJson<FrxsErpOrderSaleSettleAddRequest.SaleSettle>(jsonData);
                //验证数据的准确性 和 给其他数据后台赋值:
                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                if (string.IsNullOrEmpty(saleSettleAdd.SettleID))
                {
                    string saleSettleid = string.Empty; //结算单号
                    var resp = WorkContext.CreateIDSdkClient().Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
                    {
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.SaleSettle,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });

                    if (resp != null && resp.Flag == 0)
                    {
                        saleSettleid = resp.Data;
                        if (string.IsNullOrEmpty(saleSettleid))
                        {
                            result = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "获取门店结算单单号失败"
                            }.ToJsonString();
                            return Content(result);
                        }
                    }

                    //var saleSettle = Frxs.Platform.Utility.Json.JsonHelper.FromJson<FrxsErpOrderSaleSettleAddRequest.SaleSettle>(jsonData);
                    saleSettleAdd.SettleID = saleSettleid;
                    //saleSettle.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                    //saleSettle.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                    //saleSettle.WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;

                    var saleSettledetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<FrxsErpOrderSaleSettleAddRequest.SaleSettleDetail>(jsonDetails);

                    decimal totalSaleAmt = (decimal)0.00;
                    decimal totalBackAmt = (decimal)0.00;
                    decimal totalFeeAmt = (decimal)0.00;
                    decimal totalSettleAmt = (decimal)0.00;

                    if (saleSettledetails != null && saleSettledetails.Count > 0)
                    {
                        //读取单据类型：    // 单据类型(0:销售订单;1:销售退货单;2:销售费用单) 
                        foreach (var item in saleSettledetails)
                        {
                            totalSettleAmt += item.BillPayAmt;
                            switch (item.BillType)
                            {
                                case 0:
                                    totalSaleAmt += item.BillPayAmt;
                                    break;
                                case 1:
                                    totalBackAmt += item.BillPayAmt;
                                    break;
                                case 2:
                                    totalFeeAmt += item.BillPayAmt;
                                    break;
                            }
                        }
                    }
                    saleSettleAdd.SaleAmt = totalSaleAmt;
                    saleSettleAdd.BackAmt = totalBackAmt;
                    saleSettleAdd.FeeAmt = totalFeeAmt;
                    saleSettleAdd.SettleAmt = totalSettleAmt;


                    Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                    var respShopSettleType = serviceCenter.Execute(new FrxsErpProductSysDictDetailGetListRequest()
                    {
                        dictCode = "ShopSettleType",
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });
                    if (respShopSettleType != null && respShopSettleType.Flag == 0)
                    {
                        IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> data =
                            respShopSettleType.Data;
                        var frxsErpProductSysDictDetailGetListRespData = data.FirstOrDefault(i => i.DictValue == saleSettleAdd.SettleType);
                        if (frxsErpProductSysDictDetailGetListRespData != null)
                        {
                            saleSettleAdd.SettleName = frxsErpProductSysDictDetailGetListRespData.DictLabel;
                        }
                    }


                    FrxsErpOrderSaleSettleAddRequest r = new FrxsErpOrderSaleSettleAddRequest()
                    {
                        Details = saleSettledetails,
                        Salesettle = saleSettleAdd,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    };

                    var respadd = orderserviceCenter.Execute(r);
                    if (respadd == null)
                    {
                        return ErrorResult("数据错误，新增门店结算单失败");
                    }

                    if (respadd.Flag != 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = respadd.Info
                        }.ToJsonString();

                    }
                    if (respadd.Flag == 0)
                    {
                        //写操作日志
                        const string action = ConstDefinition.XSOperatorActionAdd;
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                              action, string.Format("{0}门店结算单[{1}]", action, saleSettleAdd.SettleID));

                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = new
                            {
                                SettleID = saleSettleAdd.SettleID,
                                UserId = WorkContext.UserIdentity.UserId,
                                UserName = WorkContext.UserIdentity.UserName,
                                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }
                        }.ToJsonString();
                    }
                }
                else
                {
                    var saleSettle = Frxs.Platform.Utility.Json.JsonHelper.FromJson<FrxsErpOrderSaleSettleEditRequest.SaleSettle>(jsonData);
                    var saleSettledetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<FrxsErpOrderSaleSettleEditRequest.SaleSettleDetail>(jsonDetails);

                    decimal totalSaleAmt = (decimal)0.00;
                    decimal totalBackAmt = (decimal)0.00;
                    decimal totalFeeAmt = (decimal)0.00;
                    decimal totalSettleAmt = (decimal)0.00;

                    if (saleSettledetails != null && saleSettledetails.Count > 0)
                    {
                        //读取单据类型：    // 单据类型(0:销售订单;1:销售退货单;2:销售费用单) 
                        foreach (var item in saleSettledetails)
                        {
                            totalSettleAmt += item.BillPayAmt;
                            switch (item.BillType)
                            {
                                case 0:
                                    totalSaleAmt += item.BillPayAmt;
                                    break;
                                case 1:
                                    totalBackAmt += item.BillPayAmt;
                                    break;
                                case 2:
                                    totalFeeAmt += item.BillPayAmt;
                                    break;
                            }
                        }
                    }
                    saleSettle.SaleAmt = totalSaleAmt;
                    saleSettle.BackAmt = totalBackAmt;
                    saleSettle.FeeAmt = totalFeeAmt;
                    saleSettle.SettleAmt = totalSettleAmt;

                    Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                    var respShopSettleType = serviceCenter.Execute(new FrxsErpProductSysDictDetailGetListRequest()
                    {
                        dictCode = "ShopSettleType",
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });
                    if (respShopSettleType != null && respShopSettleType.Flag == 0)
                    {
                        IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> data =
                            respShopSettleType.Data;
                        var frxsErpProductSysDictDetailGetListRespData = data.FirstOrDefault(i => i.DictValue == saleSettleAdd.SettleType);
                        if (frxsErpProductSysDictDetailGetListRespData != null)
                        {
                            saleSettle.SettleName = frxsErpProductSysDictDetailGetListRespData.DictLabel;
                        }
                    }


                    FrxsErpOrderSaleSettleEditRequest r = new FrxsErpOrderSaleSettleEditRequest()
                    {
                        Details = saleSettledetails,
                        Salesettle = saleSettle,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    };
                    var respEdit = orderserviceCenter.Execute(r);
                    if (respEdit == null)
                    {
                        return ErrorResult("编辑门店结算单失败");
                    }

                    if (respEdit.Flag != 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = respEdit.Info
                        }.ToJsonString();
                    }

                    if (respEdit.Flag == 0)
                    {
                        //写操作日志
                        const string action = ConstDefinition.XSOperatorActionEdit;
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                              action, string.Format("{0}门店结算单[{1}]", action, saleSettle.SettleID));
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = new
                            {
                                SettleID = saleSettle.SettleID,
                                UserId = WorkContext.UserIdentity.UserId,
                                UserName = WorkContext.UserIdentity.UserName,
                                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }
                        }.ToJsonString();
                    }
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
        /// 删除
        /// </summary>
        /// <param name="settleIds"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520611, new int[] { 52061107 })]
        public ActionResult DeleteSaleSettle(string settleIds)
        {
            string result;
            try
            {
                if (!string.IsNullOrEmpty(settleIds) && settleIds.Length > 1)
                {

                    var serviceCenter = WorkContext.CreateOrderSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleDelRequest()
                    {
                        SettleID = settleIds,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    });



                    if (resp != null && resp.Flag == 0)
                    {
                        result = new ResultData
                                     {
                                         Flag = ConstDefinition.FLAG_SUCCESS,
                                         Info = string.Format("删除成功")
                                     }.ToJsonString();
                        OperatorLogHelp.Write(
                            ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6B,
                            ConstDefinition.XSOperatorActionDel,
                            string.Format("{0}[删除门店结算单，单号：{1}]", ConstDefinition.XSOperatorActionDel, settleIds));
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("删除门店结算单失败，请检查刷新当前单据状态！")
                        }.ToJsonString();
                    }
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中删除单据"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("删除失败，请检查日志！")
                }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 过账
        /// </summary>
        /// <param name="settleIds"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520611, new int[] { 52061102 })]
        public ActionResult SaleSettlePost(string settleIds)
        {
            string result;
            try
            {
                if (!string.IsNullOrEmpty(settleIds) && settleIds.Length > 1)
                {
                    var serviceCenter = WorkContext.CreateOrderSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettlePostRequest()
                    {
                        SettleID = settleIds,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    });


                    if (resp == null)
                    {
                        return ErrorResult(string.Format("数据读取失败！"));
                    }

                    if (resp.Flag != 0)
                    {
                        return ErrorResult(resp.Info);
                    }

                    IDictionary<string, object> resultObj = new Dictionary<string, object>();
                    resultObj.Add("UserName", UserIdentity.UserName);
                    resultObj.Add("Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm"));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("过账成功"),
                        Data = resultObj
                    }.ToJsonString();

                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                                               ConstDefinition.XSOperatorActionEdit, string.Format("{0}[过账门店结算单，单号：{1}]", ConstDefinition.XSOperatorActionEdit, settleIds));


                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中过账单据"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("修改状态失败，请检查日志！")
                }.ToJsonString();
            }
            return Content(result);
        }


        /// <summary>
        /// 确认和反确认
        /// </summary>
        /// <param name="settleIds"></param>
        /// <param name="status"></param>
        /// <param name="issure"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520611, new int[] { 52061101, 52061103 })]
        public ActionResult SaleSettleSureOrNo(string settleIds, int status, int issure)
        {
            string result;
            try
            {
                bool sure = issure == 1 ? true : false;
                var str = issure == 1 ? "确认" : "反确认";
                if (!string.IsNullOrEmpty(settleIds) && settleIds.Length > 1)
                {
                    var serviceCenter = WorkContext.CreateOrderSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleSureOrNoRequest()
                    {
                        Sure = sure,
                        SettleID = settleIds,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    });


                    if (resp != null && resp.Flag == 0)
                    {
                        IDictionary<string, object> resultObj = new Dictionary<string, object>();
                        resultObj.Add("UserName", UserIdentity.UserName);
                        resultObj.Add("Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm"));
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = string.Format("{0}成功", str),
                            Data = resultObj
                        }.ToJsonString();

                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                                                   ConstDefinition.XSOperatorActionEdit, string.Format("{0}[{1}门店结算单状态，单号：{2}]", ConstDefinition.XSOperatorActionEdit, str, settleIds));

                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("修改门店结算单状态失败，请检查相应单据状态不是正确！")
                        }.ToJsonString();
                    }

                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = string.Format("未选中{0}单据", str)
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("修改状态失败，请检查日志！")
                }.ToJsonString();
            }
            return Content(result);
        }


        /// <summary>
        /// 初始化主仓库
        /// </summary>
        /// <returns></returns>
        public ActionResult InitParentWarehores()
        {
            string jsonStr = "[]";
            jsonStr = new
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                WName = WorkContext.CurrentWarehouse.Parent.WarehouseName,
                WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode
            }.ToJsonString();
            return Content(jsonStr);
        }



        /// <summary>
        /// 导出数据到Excel (按照查询条件查询所有要导出的数据) 导出列表数据
        /// </summary>
        /// <returns>文件流</returns>
        public ActionResult DataExport(SaleSettleListSearchModel model)
        {
            string sortBy = "SettleID  asc";
            if (!model.sort.IsNullOrEmpty() && !model.order.IsNullOrEmpty())
            {
                sortBy = model.sort + "  " + model.order;
            }

            IList<Models.Sale.SaleSettle> saleSettlelist = new List<Models.Sale.SaleSettle>();
            var r = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleGetListRequest
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                SettleID = model.SettleID == null ? null : model.SettleID.Trim(),
                SettleType = model.SettleType == null ? null : model.SettleType.Trim(),
                ShopCode = model.ShopCode == null ? null : model.ShopCode.Trim(),
                ShopName = model.ShopName == null ? null : model.ShopName.Trim(),
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortBy = sortBy,
                Status = model.Status,
            };
            //获取列表
            var resp = WorkContext.CreateOrderSdkClient().Execute(r);
            if (resp != null && resp.Flag == 0)
            {
                saleSettlelist = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<FrxsErpOrderSaleSettleGetListResp.SaleSettle, Models.Sale.SaleSettle>(resp.Data.ItemList);
            }

            const int maxRows = int.MaxValue;
            string fileName = "门店结算单" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            byte[] byteArr = NpoiExcelhelper.ExportExcel
                (
                    saleSettlelist,
                    maxRows,
                    Server.MapPath("UploadFile"),
                    fileName
                );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }


        /// <summary>
        /// 结算单明细数据导出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult DataExportItems(SaleSettleListSearchModel model)
        {
            var r = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleGetModelRequest
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                SettleID = model.SettleID == null ? null : model.SettleID.Trim(),
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };
            //获取列表
            var resp = WorkContext.CreateOrderSdkClient().Execute(r);
            if (resp == null)
            {
                return ErrorResult("读取数据错误");
            }

            if (resp.Flag != 0)
            {
                return ErrorResult(resp.Info);
            }
            List<Models.Sale.SaleSettleDetail> saleSettleDetailList = null;
            if (resp.Data != null && resp.Data.SaleSettleDetailList != null && resp.Data.SaleSettleDetailList.Count > 0)
            {
                saleSettleDetailList = resp.Data.SaleSettleDetailList.Select(item => new SaleSettleDetail
                {
                    FeeName = item.FeeName,
                    BillType = item.BillType,
                    BillID = item.BillID,
                    BillDate = item.BillDate,
                    Remark = item.Remark,
                    BillDetailsID = item.BillDetailsID,
                    FeeCode = item.FeeCode,
                    SettleID = item.SettleID,
                    WID = item.WID,
                    BillTypeName = item.BillTypeStr,
                    ID = item.ID,
                    BillAmt = item.BillAmt.ToString("0.0000"),
                    BillAddAmt = item.BillAddAmt.ToString("0.0000"),
                    BillPayAmt = item.BillPayAmt.ToString("0.0000")
                }).ToList();
            }

            if (saleSettleDetailList == null)
            {
                return ErrorResult("读取门店结算单明细数据错误，不能导出");
            }

            const int maxRows = int.MaxValue;
            string fileName = "门店结算单明细数据" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            byte[] byteArr = NpoiExcelhelper.ExportExcel
                (
                    saleSettleDetailList,
                    maxRows,
                    Server.MapPath("UploadFile"),
                    fileName
                );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);

        }
    }
}
