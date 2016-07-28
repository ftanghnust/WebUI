using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Again;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Import;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Excel;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Again
{
    [ValidateInput(false)]
    public class AgainController : BaseController
    {
        /// <summary>
        /// 返配申请单
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520513)]
        public ActionResult AgainApply()
        {
            return View();
        }

        /// <summary>
        /// 返配申请单-添加or修改
        /// </summary>
        /// <returns></returns>
        public ActionResult AgainApplyAddOrEdit()
        {
            return View();
        }


        /// <summary>
        /// 补货申请单
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520514)]
        public ActionResult RepairApply()
        {
            return View();
        }

        /// <summary>
        /// 补货申请单-添加or修改
        /// </summary>
        /// <returns></returns>
        public ActionResult RepairApplyAddOrEdit()
        {
            return View();
        }


        /// <summary>
        /// 新增修改
        /// </summary>
        /// <returns></returns>
        public ActionResult AgainAddOrEditHandle(string jsonData, string jsonDetails)
        {
            var result = "";
            var appId = "";
            var again = JsonHelper.FromJson<Models.Again.Again>(jsonData);
            var againDetail = JsonHelper.GetObjectIList<Models.Again.AgainDetail>(jsonDetails);

            //新增的时候获取新的APPID
            if (string.IsNullOrEmpty(again.AppId))
            {
                //创建单据号
                var serviceCenter = WorkContext.CreateIDSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    Type =
                        Frxs.Erp.ServiceCenter.ID.SDK.Request.
                        FrxsErpIdIdsGetRequest.IDTypes.BuyOrder,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                });

                if (resp != null && resp.Flag == 0)
                {
                    appId = resp.Data;
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "创建新单据号失败"
                    }.ToJsonString();
                    return Content(result);
                }
            }

            var listDetail = new List<FrxsErpOrderBuyPreAppAddOrEditRequest.BuyPreAppDetailsRequest>();
            foreach (var item in againDetail)
            {
                var obj = new FrxsErpOrderBuyPreAppAddOrEditRequest.BuyPreAppDetailsRequest();
                obj.AppPackingQty = item.PackingQty;
                obj.AppQty = item.AppQty;
                obj.AppUnit = item.Unit;
                obj.ProductId = item.ProductId;
                obj.Remark = item.Remark;
                listDetail.Add(obj);
            }

            WarehouseIdentity subwareHouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(o => o.WarehouseId == again.SubWID);

            //添加OR修改
            var resp1 = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppAddOrEditRequest()
            {
                Flag = string.IsNullOrEmpty(again.AppId) ? "Add" : "Edit",
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BuyPreAppModel = new FrxsErpOrderBuyPreAppAddOrEditRequest.BuyPreAppRequest()
                {
                    AppDate = DateTime.Now,
                    AppID = string.IsNullOrEmpty(again.AppId) ? appId : again.AppId,
                    AppType = again.Type,
                    Remark = again.Remark,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode,
                    WName = WorkContext.CurrentWarehouse.Parent.WarehouseName,
                    SubWID = again.SubWID,
                    SubWCode = subwareHouse != null ? subwareHouse.WarehouseCode : "",
                    SubWName = subwareHouse != null ? subwareHouse.WarehouseName : ""
                },
                BuyPreAppDetailsList = listDetail,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp1 != null && resp1.Flag == 0)
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = resp1.Info,
                    Data = string.IsNullOrEmpty(again.AppId) ? appId : again.AppId
                }.ToJsonString();


                var menuName = again.Type == 0 ? "返配" : again.Type == 1 ? "补货" : "";

                var menu = again.Type == 0 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5D :
                               again.Type == 1 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5E : 0;

                var action = string.IsNullOrEmpty(again.AppId) ? ConstDefinition.XSOperatorActionAdd : ConstDefinition.XSOperatorActionEdit;

                //写操作日志
                OperatorLogHelp.Write(menu, action, string.Format("{0}{1}申请单[{2}]", action, menuName, string.IsNullOrEmpty(again.AppId) ? appId : again.AppId));

            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp1 != null ? resp1.Info : "调用删除接口异常"
                }.ToJsonString();
            }

            return Content(result);
        }


        /// <summary>
        /// 获取单据信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAgainInfo(string appId)
        {
            string jsonStr = "[]";
            try
            {
                var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppGetModelRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AppID = appId,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                });

                if (resp != null && resp.Flag == 0)
                {

                    var pdidList = from o in resp.Data.BuyPreAppDetailsList select o.ProductId;

                    var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList.ToList(),
                        WIDList = widList
                    };


                    //转化为本机数据集合
                    var listDetails = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<FrxsErpOrderBuyPreAppGetModelResp.BuyPreAppDetails, AgainGridData>(resp.Data.BuyPreAppDetailsList.OrderBy(x => x.ModifyTime));

                    //获取库存列表
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    if (respstock != null && respstock.Data != null)
                    {
                        var PIDList = from o in listDetails select o.ProductId;
                        var respTemp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsAddedListGetRequest()
                        {
                            PageIndex = 1,
                            PageSize = int.MaxValue,
                            WID = resp.Data.BuyPreApp.WID,
                            WStatus = 1,
                            ProductIds = PIDList.ToList()
                        });
                        foreach (var item in listDetails)
                        {
                            var frxsErpOrderStockQtyListQueryRespData =
                                respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == resp.Data.BuyPreApp.SubWID);
                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }

                            //字段替换
                            item.Price = item.AppPrice;
                            item.PackingQty = item.AppPackingQty;

                            if (respTemp != null && respTemp.Data != null)
                            {
                                var tempDetails = respTemp.Data.ItemList.FirstOrDefault(i => i.ProductId == item.ProductId);
                                if (tempDetails != null)
                                {
                                    item.MaxBuyPrice = tempDetails.BuyPrice;
                                }
                            }
                        }
                    }

                    var data = new
                    {
                        appPre = resp.Data.BuyPreApp,
                        gridData = new
                        {
                            total = listDetails.Count,
                            rows = listDetails
                        }
                    };
                    jsonStr = data.ToJson();
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
        /// 确认订单-反确认
        /// </summary>
        /// <returns></returns>
        public ActionResult AgainConfirmOrReconfirm(string appIds, int type, int menuType)
        {
            //type 0反确认 1确认
            var jsonStr = "";
            var listIds = appIds.Split(',').ToList();
            var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppChangeStatusRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BuyPreAppIDs = listIds,
                Status = type,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            if (resp != null && resp.Flag == 0)
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "成功"
                }.ToJsonString();


                var menuName = menuType == 0 ? "返配" : menuType == 1 ? "补货" : "";

                var menu = menuType == 0 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5D :
                               menuType == 1 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5E : 0;

                var action = type == 0 ? ConstDefinition.XSOperatorActionNoSure : ConstDefinition.XSOperatorActionSure;

                //写操作日志
                OperatorLogHelp.Write(menu, action, string.Format("{0}{1}申请单[{2}]", action, menuName, appIds));

            }
            else
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp != null ? resp.Info : "调用删除接口异常"
                }.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 立即生效
        /// </summary>
        /// <returns></returns>
        public ActionResult AgainPosting(string appIds, int menuType)
        {
            //type 0反确认 1确认
            var jsonStr = "";
            var listIds = appIds.Split(',').ToList();
            var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppChangeStatusRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BuyPreAppIDs = listIds,
                Status = 2,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            if (resp != null && resp.Flag == 0)
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "成功"
                }.ToJsonString();

                var menuName = menuType == 0 ? "返配" : menuType == 1 ? "补货" : "";

                var menu = menuType == 0 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5D :
                               menuType == 1 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5E : 0;

                var action = ConstDefinition.XSOperatorActionEffective;

                //写操作日志
                OperatorLogHelp.Write(menu, action, string.Format("{0}{1}申请单[{2}]", action, menuName, appIds));
            }
            else
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp != null ? resp.Info : "调用删除接口异常FrxsErpOrderBuyPreAppChangeStatusRequest"
                }.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult AgainDel(string appIds)
        {
            var jsonStr = "";
            var listIds = appIds.Split(',').ToList();
            var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppDelRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BuyPreAppIDs = listIds,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            if (resp != null && resp.Flag == 0)
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "成功"
                }.ToJsonString();
            }
            else
            {
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp != null ? resp.Info : "调用删除接口异常"
                }.ToJsonString();
            }

            return Content(jsonStr);
        }



        /// <summary>
        /// 主列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AaginList(AgainSearchModel mode)
        {
            string jsonStr = "[]";
            try
            {
                int? subWid = mode.SubWID;
                if (!mode.SubWID.HasValue)
                {
                    if (WorkContext.CurrentWarehouse.Parent.WarehouseId != WorkContext.CurrentWarehouse.WarehouseId)
                    {
                        subWid = WorkContext.CurrentWarehouse.WarehouseId;
                    }
                }

                var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppGetListRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AppID = mode.AppID,
                    AppDateEnd = mode.AppDateEnd,
                    AppDateStart = mode.AppDateStart,
                    AppType = mode.AppType,
                    PageIndex = mode.page,
                    PageSize = mode.rows,
                    PostingTimeEnd = mode.PostingTimeEnd,
                    PostingTimeStart = mode.PostingTimeStart,
                    SortBy = "AppID DESC",
                    Status = mode.Status,
                    SubWID = subWid
                });

                if (resp != null && resp.Data != null)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
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
        /// 导入
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportAgain()
        {
            return View();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelAgain(string appId, int type)
        {
            var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyPreAppGetModelRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AppID = appId,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                });

            if (resp != null && resp.Flag == 0)
            {

                var pdidList = from o in resp.Data.BuyPreAppDetailsList select o.ProductId;

                var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                                     {
                                         UserId = WorkContext.UserIdentity.UserId,
                                         UserName = WorkContext.UserIdentity.UserName,
                                         PDIDList = pdidList.ToList(),
                                         WIDList = widList
                                     };


                //转化为本机数据集合
                var listDetails = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<FrxsErpOrderBuyPreAppGetModelResp.BuyPreAppDetails, AgainGridData>(resp.Data.BuyPreAppDetailsList.OrderBy(x => x.ModifyTime));

                //获取库存列表
                var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                if (respstock != null && respstock.Data != null)
                {
                    foreach (var item in listDetails)
                    {
                        var frxsErpOrderStockQtyListQueryRespData =
                            respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == resp.Data.BuyPreApp.SubWID);
                        if (frxsErpOrderStockQtyListQueryRespData != null)
                        {
                            item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                        }
                    }
                }


                var name = type == 0 ? "返配" : "补货";
                var fileName = name + "申请单_" + appId + ".xls";

                var dataTable = new DataTable();
                dataTable.Columns.Add("商品编码");
                dataTable.Columns.Add("商品名称");
                dataTable.Columns.Add("单位");
                dataTable.Columns.Add(name + "数量");
                dataTable.Columns.Add(name + "价格");
                dataTable.Columns.Add(name + "金额");
                dataTable.Columns.Add("包装数");
                dataTable.Columns.Add("总数量");
                dataTable.Columns.Add("库存数量");
                dataTable.Columns.Add("备注");
                dataTable.Columns.Add("国际条形码");

                foreach (var item in listDetails)
                {
                    var newRow = dataTable.NewRow();
                    newRow["商品编码"] = item.SKU;
                    newRow["商品名称"] = item.ProductName;
                    newRow["单位"] = item.Unit;
                    newRow[name + "数量"] = item.AppQty;
                    newRow[name + "价格"] = item.AppPrice;
                    newRow[name + "金额"] = Convert.ToDecimal(item.AppQty * item.AppPrice).ToString("0.0000");
                    newRow["包装数"] = item.AppPackingQty;
                    newRow["总数量"] = item.AppQty * item.AppPackingQty;
                    newRow["库存数量"] = Convert.ToDecimal(item.WStock).ToString("0");
                    newRow["备注"] = item.Remark;
                    newRow["国际条形码"] = item.BarCode;

                    dataTable.Rows.Add(newRow);
                }

                byte[] byteArr = NpoiExcelhelper.ExportExcel
                    (
                        dataTable,
                        5000,
                        Server.MapPath("UploadFile"),
                        fileName
                    );

                return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);

            }

            return Content("找不到单据信息,单据可能已被删除");
        }

        /// <summary>
        /// 导入-补货
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportRepair()
        {
            return View();
        }


        /// <summary>
        ///  获EXCEL里的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImportData(string fileName, string folderPath, int type, int subWId)
        {
            string jsonStr = "[]";
            try
            {
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;
                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);


                var list = new List<ImportPriceModel>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //过滤空行
                    if (string.IsNullOrEmpty(dt.Rows[i][0].ToString().Trim())
                        && string.IsNullOrEmpty(dt.Rows[i][1].ToString().Trim())
                        && string.IsNullOrEmpty(dt.Rows[i][2].ToString().Trim())
                        && string.IsNullOrEmpty(dt.Rows[i][3].ToString().Trim()))
                    {
                        continue;
                    }

                    if (dt.Rows[i][1].ToString().Trim() == "")
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行商品编码不能为空"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }

                    var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWProductsAddedListGetRequest()
                    {
                        PageIndex = 1,
                        PageSize = 10,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SKU = dt.Rows[i][1].ToString()
                    });

                    if (resp != null && resp.Flag == 0 && resp.Data != null && resp.Data.TotalRecords > 0)
                    {
                        var model = new ImportPriceModel();
                        model.BarCode = resp.Data.ItemList[0].BarCode;
                        model.ProductId = resp.Data.ItemList[0].ProductId;
                        model.ProductName = resp.Data.ItemList[0].ProductName;
                        model.SKU = resp.Data.ItemList[0].SKU;
                        model.ShopAddPerc = resp.Data.ItemList[0].ShopAddPerc;
                        model.ShopPoint = resp.Data.ItemList[0].ShopPoint;

                        model.MinUnit = resp.Data.ItemList[0].Unit;

                        model.UnitPrice = resp.Data.ItemList[0].UnitBuyPrice;
                        model.BuyPrice = resp.Data.ItemList[0].BuyPrice;
                        

                        //商品状态验证
                        if (resp.Data.ItemList[0].WStatus == 2)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "为淘汰商品"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (resp.Data.ItemList[0].WStatus == 3)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "为冻结商品"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (resp.Data.ItemList[0].WStatus != 1)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "为非正常状态"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }



                        //单位验证
                        string unit = dt.Rows[i][2].ToString();
                        if (string.IsNullOrEmpty(unit))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "单位为空"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (unit == resp.Data.ItemList[0].Unit)
                        {
                            model.Unit = resp.Data.ItemList[0].Unit;
                            model.PackingQty = 1;
                            model.NewPrice = resp.Data.ItemList[0].UnitBuyPrice;
                        }
                        else if (unit == resp.Data.ItemList[0].BigUnit)
                        {
                            model.Unit = resp.Data.ItemList[0].BigUnit;
                            model.PackingQty = decimal.Parse(resp.Data.ItemList[0].BuyBigPackingQty.ToString());
                            model.NewPrice = resp.Data.ItemList[0].BuyPrice;
                        }
                        else
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][0]) + "单位数据库中不存在"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }


                        //返配数量
                        decimal appQty = 0;
                        if (!decimal.TryParse(dt.Rows[i][3].ToString(), out appQty))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + (type == 0 ? "返配" : "补货") + "数量不正确"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        model.AppQty = appQty;

                        int index;
                        if (!int.TryParse(dt.Rows[i][0].ToString(), out index))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行序号不正确，应为整数"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }

                        model.Index = index;
                        list.Add(model);
                    }
                    else
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行无法匹配商品编码【" + dt.Rows[i][1] + "】，请检查是否正确"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                }

                //获取库存字段
                if (list.Count > 0 && list[0].ProductId > 0)
                {
                    var pdidList = from o in list select o.ProductId;

                    var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList.ToList(),
                        WIDList = widList
                    };

                    //获取库存列表
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    if (respstock != null && respstock.Data != null)
                    {
                        foreach (var item in list)
                        {
                            var frxsErpOrderStockQtyListQueryRespData =
                                respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == subWId);
                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }
                        }
                    }
                }

                var obj = new { total = list.Count, rows = list };
                jsonStr = obj.ToJsonString();


            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }


        /// <summary>
        ///  获EXCEL里的数据-Buy
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImportBuyRepairData(string fileName, string folderPath, int type, int subWId)
        {
            string jsonStr = "[]";
            try
            {
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;
                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);


                var list = new List<ImportPriceModel>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //过滤空行
                    if (string.IsNullOrEmpty(dt.Rows[i][0].ToString().Trim())
                        && string.IsNullOrEmpty(dt.Rows[i][1].ToString().Trim())
                        && string.IsNullOrEmpty(dt.Rows[i][2].ToString().Trim())
                        && string.IsNullOrEmpty(dt.Rows[i][3].ToString().Trim()))
                    {
                        continue;
                    }

                    if (dt.Rows[i][1].ToString().Trim() == "")
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行商品编码不能为空"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }

                    var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWProductsAddedListGetRequest()
                    {
                        PageIndex = 1,
                        PageSize = 10,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SKU = dt.Rows[i][1].ToString()
                    });

                    if (resp != null && resp.Flag == 0 && resp.Data != null && resp.Data.TotalRecords > 0)
                    {
                        var model = new ImportPriceModel();
                        model.BarCode = resp.Data.ItemList[0].BarCode;
                        model.ProductId = resp.Data.ItemList[0].ProductId;
                        model.ProductName = resp.Data.ItemList[0].ProductName;
                        model.SKU = resp.Data.ItemList[0].SKU;
                        model.MinUnit = resp.Data.ItemList[0].Unit;
                        model.UnitPrice = resp.Data.ItemList[0].UnitBuyPrice;
                        model.BuyPrice = resp.Data.ItemList[0].BuyPrice;
                        model.PackingQty = resp.Data.ItemList[0].BuyBigPackingQty;

                        //商品状态验证
                        if (resp.Data.ItemList[0].WStatus == 2)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "为淘汰商品"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (resp.Data.ItemList[0].WStatus == 3)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "为冻结商品"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (resp.Data.ItemList[0].WStatus != 1)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "为非正常状态"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }


                        //单位验证
                        string unit = dt.Rows[i][2].ToString();
                        if (string.IsNullOrEmpty(unit))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][1]) + "单位为空"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (unit == resp.Data.ItemList[0].Unit)
                        {
                            model.Unit = resp.Data.ItemList[0].Unit;
                            model.PackingQty = 1;
                            model.NewPrice = resp.Data.ItemList[0].UnitBuyPrice;
                        }
                        else if (unit == resp.Data.ItemList[0].BigUnit)
                        {
                            model.Unit = resp.Data.ItemList[0].BigUnit;
                            model.PackingQty = decimal.Parse(resp.Data.ItemList[0].BuyBigPackingQty.ToString());
                            model.NewPrice = resp.Data.ItemList[0].BuyPrice;
                        }
                        else
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][0]) + "单位数据库中不存在"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }


                        //返配数量
                        decimal appQty = 0;
                        if (!decimal.TryParse(dt.Rows[i][3].ToString(), out appQty))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + (type == 0 ? "返配" : "补货") + "数量不正确"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        model.AppQty = appQty;

                        int index;
                        if (!int.TryParse(dt.Rows[i][0].ToString(), out index))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行序号不正确，应为整数"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }

                        model.Index = index;
                        list.Add(model);
                    }
                    else
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行无法匹配商品编码【" + dt.Rows[i][1] + "】，请检查是否正确"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                }

                //获取库存字段
                if (list.Count > 0 && list[0].ProductId > 0)
                {
                    var pdidList = from o in list select o.ProductId;

                    var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList.ToList(),
                        WIDList = widList
                    };

                    //获取库存列表
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    if (respstock != null && respstock.Data != null)
                    {
                        foreach (var item in list)
                        {
                            var frxsErpOrderStockQtyListQueryRespData =
                                respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == subWId);
                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }
                        }
                    }
                }

                var obj = new { total = list.Count, rows = list };
                jsonStr = obj.ToJsonString();


            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }





    }
}
