using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Excel;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;
using System.Data;
//using Frxs.Platform.Cache;
//using Frxs.Platform.Cache.Provide;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ValidateInput(false)]
    public class BuyBackPreController : BaseController
    {
        #region 视图
        /// <summary>
        /// 采购退货订单列表视图
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyBackPreList()
        {
            return View();
        }

        /// <summary>
        /// 采购退货新增或编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyBackPreAddOrEditNew()
        {
            return View();
        }

        /// <summary>
        /// 导入 采购退货单明细
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportBackOrderDetail()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="backids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520512, 52051203)]
        public ActionResult DeleteBuyBackPre(string backids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(backids) && backids.Length > 1)
                {
                    var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyBackPreDelRequest()
                    {
                        BackIDs = backids,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });
                    if (resp != null && resp.Flag == 0)
                    {
                        //写操作日志
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5C,
                                               ConstDefinition.XSOperatorActionDel, string.Format("{0}采购退货单号[{1}]", ConstDefinition.XSOperatorActionDel, backids));

                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK"
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
        /// 批量状态改变
        /// </summary>
        /// <param name="backids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520512, new int[] { 52051204, 52051205, 52051206 })]
        public ActionResult BuyBackPreChangeStatus(string backids, int status)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(backids) && backids.Length > 1)
                {
                    var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyBackPreChangeStatusRequest()
                    {
                        BackIDs = backids,
                        Status = status,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });

                    if (resp != null && resp.Flag == 0)
                    {
                        //写操作日志
                        string action = string.Empty;
                        if (status == 1)       //确认
                        {
                            action = ConstDefinition.XSOperatorActionSure;
                        }
                        else if (status == 2)  //过账
                        {
                            action = ConstDefinition.XSOperatorActionPasting;
                        }
                        else if (status == 0)   //反确认
                        {
                            action = ConstDefinition.XSOperatorActionNoSure;
                        }
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5C,
                                              action, string.Format("{0}采购退货单号[{1}]", action, backids));

                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK",
                            Data = new
                            {
                                UserId = WorkContext.UserIdentity.UserId,
                                UserName = WorkContext.UserIdentity.UserName,
                                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }
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
        /// 获取分页列表
        /// </summary>
        /// <param name="searchModel">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetBuyBackPreList(BuyBackSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new BuyBackPreModel().GetBuyBackPrePageData(searchModel);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <returns>文件流</returns>
        [AuthorizeButtonFiter(520512, 52051207)]
        public ActionResult DataExport(string Backid)
        {
            IList<BuyBackPreDetailsModel> detailsmodel = new List<BuyBackPreDetailsModel>();
            var resp = this.ErpOrderSdkClient.Execute(new FrxsErpOrderBuyBackPreGetModelRequest()
            {
                BackID = Backid,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            if (resp != null && resp.Flag == 0)
            {
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderBuyBackPreGetModelResp.BuyBackPreDetails, BuyBackPreDetailsModel>(resp.Data.orderdetails);
            }
            int count = 0;
            foreach (var item in resp.Data.orderdetails)
            {
                count = count + 1;
                var temp = detailsmodel.FirstOrDefault(o => o.ProductId == item.ProductId);
                var ext = resp.Data.orderdetailsext.FirstOrDefault(o => o.ID == item.ID);
                if (temp != null)
                {
                    temp.ShopCategoryName = ext.ShopCategoryId1Name + ">>" + ext.ShopCategoryId2Name + ">>" + ext.ShopCategoryId3Name;
                }
                temp.BackQtystr = temp.BackQty.ToString("0.00");
                temp.BackPricestr = temp.BackPrice.ToString("0.0000");
                temp.SubAmtstr = temp.SubAmt.ToString("0.0000");
                temp.BackPackingQtystr = temp.BackPackingQty.ToString("0.00");
                temp.UnitQtystr = temp.UnitQty.ToString("0.00");
                temp.RowNum = count;
            }
            int maxRows = 1000;
            string fileName = "采购退货单_" + Backid + ".xls";   //DateTime.Now.ToString("yyyyMMddHHmmssfff")
            byte[] byteArr = NpoiExcelhelper.ExportExcel
                (
                    detailsmodel,
                    maxRows,
                    Server.MapPath("UploadFile"),
                    fileName
                );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }

        /// <summary>
        ///  获EXCEL里的数据
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520512, new int[] { 52051201, 52051202 })]
        public ActionResult GetImportData(string fileName, string folderPath, int vendorid, int subWID)
        {
            string jsonStr = "[]";
            try
            {
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;
                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);
                var list = new List<BackOrderDetails>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                    {
                        continue;
                        //jsonStr = new ResultData
                        //{
                        //    Flag = ConstDefinition.FLAG_FAIL,
                        //    Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品编码为空", dt.Rows[i][1].ToString())
                        //}.ToJsonString();
                        //return Content(jsonStr);
                    }
                    var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsBuyListGetRequest()
                    {
                        PageIndex = 1,
                        PageSize = 10,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        VendorID = vendorid,
                        SKU = dt.Rows[i][0].ToString(),  //SKU
                        SKULikeSearch = false,
                        AllWState = true
                    });
                    if (resp != null && resp.Flag == 0 && resp.Data != null && resp.Data.TotalRecords > 0)
                    {
                        if (resp.Data.ItemList[0].WStatus == 2)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】已经淘汰", dt.Rows[i][0].ToString())
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (resp.Data.ItemList[0].WStatus == 3)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】已经冻结", dt.Rows[i][0].ToString())
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (resp.Data.ItemList[0].WStatus == 0)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】已经移除", dt.Rows[i][0].ToString())
                            }.ToJsonString();
                            return Content(jsonStr);
                        }

                        var model = new BackOrderDetails();
                        //退货数量
                        decimal BackQty = 0;
                        if (!decimal.TryParse(dt.Rows[i][2].ToString(), out BackQty))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】退货数量格式不正确", dt.Rows[i][0].ToString()) 
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        if (BackQty > 99999999)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】退货数量过大，最大数为99999999", dt.Rows[i][0].ToString()) 
                            }.ToJsonString();
                            return Content(jsonStr);
                        }

                        //单位
                        string unit = dt.Rows[i][1].ToString();
                        if (string.IsNullOrEmpty(unit))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】单位为空", dt.Rows[i][0].ToString()) 
                            }.ToJsonString();
                            return Content(jsonStr);
                        }

                        if (unit == resp.Data.ItemList[0].Unit)
                        {
                            model.BackUnit = resp.Data.ItemList[0].Unit;
                            model.BackPackingQty = 1;
                            model.BackPrice = resp.Data.ItemList[0].UnitPrice;
                        }
                        else if (unit == resp.Data.ItemList[0].BuyUnit)
                        {
                            model.BackUnit = resp.Data.ItemList[0].BuyUnit;
                            model.BackPackingQty = decimal.Parse(resp.Data.ItemList[0].PackingQty.ToString());
                            model.BackPrice = resp.Data.ItemList[0].BuyPrice;
                        }
                        else
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】单位数据库中不存在", dt.Rows[i][0].ToString())
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        model.SKU = resp.Data.ItemList[0].SKU;
                        model.ProductName = resp.Data.ItemList[0].ProductName;
                        model.BackQty = BackQty;     //退货数量
                        model.Remark = Convert.ToString(dt.Rows[i][3]);      //备注
                        model.BarCode = resp.Data.ItemList[0].BarCode;
                        model.ProductId = resp.Data.ItemList[0].ProductId;
                        model.UnitPrice = resp.Data.ItemList[0].UnitPrice;
                        model.OldBuyPrice = model.BackPrice;
                        model.UnitQty = model.BackQty * model.BackPackingQty;
                        model.SubAmt = model.BackQty * model.BackPrice;
                        

                        model.MinUnit = resp.Data.ItemList[0].Unit;
                        model.UnitPrice = resp.Data.ItemList[0].UnitPrice;

                        model.SalePrice = resp.Data.ItemList[0].SalePrice;
                        model.SaleUnitPrice = resp.Data.ItemList[0].SaleUnitPrice;

                        model.MinBuyPrice = resp.Data.ItemList[0].UnitPrice;
                        model.MinSalePrice = resp.Data.ItemList[0].SaleUnitPrice;
                        model.MaxBuyPrice = resp.Data.ItemList[0].BuyPrice;
                        model.MaxSalePrice = resp.Data.ItemList[0].SalePrice;

                        model.Index = list.Count + 1;
                        list.Add(model);
                    }
                    else
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】与所选供应商不匹配", dt.Rows[i][0].ToString())
                        }.ToJsonString();
                        return Content(jsonStr);

                        //没找到添加一个空的
                        //var model = new BuyOrderDetails();
                        //int index = 0;
                        //if (!int.TryParse(dt.Rows[i][0].ToString(), out index))
                        //{
                        //    index = list.Count + 1;
                        //}
                        //model.Index = index;
                        //list.Add(model);
                    }
                }


                //获取库存列表
                var pdidList = from o in list select o.ProductId;
                var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                {
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    PDIDList = pdidList.ToList(),
                    WIDList = widList
                };
                var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                foreach (var item in list)
                {
                    if (respstock != null && respstock.Data != null)
                    {
                        var frxsErpOrderStockQtyListQueryRespData = respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == subWID);
                        if (frxsErpOrderStockQtyListQueryRespData != null)
                        {
                            item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
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
        /// 根据BackID获取退货订单信息
        /// </summary>
        /// <param name="buyid"></param>
        /// <returns></returns>
        public ActionResult GetBuyBackInfo(string backid)
        {
            BuyBackPreModel model = new BuyBackPreModel();
            //获取数据，绑定
            var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyBackPreGetModelRequest()
            {
                BackID = backid,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            if (resp != null && resp.Flag == 0)
            {
                //获取库存列表
                var pdidList = from o in resp.Data.orderdetails select o.ProductId;
                var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                {
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    PDIDList = pdidList.ToList(),
                    WIDList = widList
                };
                var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);


                model = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<BuyBackPreModel>(resp.Data.order);
                IList<BuyBackPreDetailsModel> detailsmodel = new List<BuyBackPreDetailsModel>();
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderBuyBackPreGetModelResp.BuyBackPreDetails, BuyBackPreDetailsModel>(resp.Data.orderdetails);
                if (detailsmodel.Count > 0)
                {
                    var PIDList = from o in detailsmodel select o.ProductId;
                    var respTemp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsBuyListGetRequest()
                    {
                        PageIndex = 1,
                        PageSize = int.MaxValue,
                        WID = resp.Data.order.WID,
                        VendorID = resp.Data.order.VendorID,
                        ProductIds = PIDList.ToList()
                    });

                    foreach (var item in detailsmodel)
                    {
                        item.OldBuyPrice = item.BackPrice;
                        if (respstock != null && respstock.Data != null)
                        {
                            //获取相应子机构商品库存
                            var frxsErpOrderStockQtyListQueryRespData = respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == resp.Data.order.SubWID);
                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }
                        }

                        if (respTemp != null && respTemp.Data != null)
                        {
                            var tempDetails = respTemp.Data.ItemList.FirstOrDefault(i => i.ProductId == item.ProductId);
                            if (tempDetails != null)
                            {
                                item.MaxSalePrice = tempDetails.SalePrice;
                                item.MinSalePrice = tempDetails.SaleUnitPrice;
                                item.MaxBuyPrice = tempDetails.BuyPrice;
                                item.MinBuyPrice = tempDetails.UnitPrice;
                            }
                        }
                    }
                }
                model.Backdetails = detailsmodel;
            }

            //设置缓存
            //RedisHelper redisHelper = ErpRedisCacheProvide.GetWarehouseCacheHelper();
            //redisHelper.SetCache(string.Format(ConstDefinition.CACHE_BUYBACK_KEY, backid), model, DateTime.Now.AddDays(30));
            return Content(model.ToJsonString());
        }

        /// <summary>
        /// 退货订单记录保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeButtonFiter(520512, new int[] { 52051201, 52051202 })]
        public ActionResult BuyBackPreAddOrEditeNewHandle(string jsonData, string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var order = Frxs.Platform.Utility.Json.JsonHelper.FromJson<BackOrder>(jsonData);
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<BackOrderDetails>(jsonDetails);

                var part = new OrderPartsServer();
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreRequestDto orderdto = new FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreRequestDto();
                if (string.IsNullOrEmpty(order.BackID))
                {
                    flag = "Add";
                    string backid = new BuyBackPreModel().GetBuyBackID();
                    if (string.IsNullOrEmpty(backid))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "获取采购退货单号失败"
                        }.ToJsonString();
                        return Content(result);
                    }
                    orderdto.BackID = backid;
                    orderdto.WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
                    orderdto.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                    WarehouseIdentity subwareHouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(o => o.WarehouseId == order.SubWID);
                    orderdto.SubWCode = subwareHouse.WarehouseCode;
                    orderdto.SubWName = subwareHouse.WarehouseName;
                }
                else
                {
                    orderdto.BackID = order.BackID;
                    flag = "Edit";
                }
                orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                orderdto.SubWID = order.SubWID;
                orderdto.OrderDate = order.OrderDate;
                orderdto.BuyEmpID = order.BuyEmpID;
                orderdto.BuyEmpName = order.BuyEmpName;
                orderdto.VendorID = order.VendorID;
                orderdto.VendorCode = order.VendorCode;
                orderdto.VendorName = order.VendorName;
                orderdto.Remark = order.Remark;

                IList<FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsRequestDto> orderdetailsdto
                    = new List<FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsRequestDto>();

                IList<FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsExtRequestDto> orderdetailsextdto
                  = new List<FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsExtRequestDto>();

                var PIDList = from o in orderdetails select o.ProductId;

                //1.调用接口，获取网仓商品信息
                var wProudcts = part.GetWProductsExtInfo(warehouseId, PIDList.ToList(), userid, username);

                //2.调用接口，获取商品信息
                var products = part.GetProductsInfo(PIDList.ToList(), userid, username);

                //3.调用接口，获取品牌信息
                var brandIdList = new List<int>();
                foreach (var product in products.ItemList)
                {
                    if (product.BrandId1 > 0)
                    {
                        brandIdList.Add(product.BrandId1);
                    }
                    if (product.BrandId2 > 0)
                    {
                        brandIdList.Add(product.BrandId2);
                    }
                }
                var brandList = part.GetBrands(brandIdList, userid, username);

                double TotalAmt = 0.0;    //总金额
                foreach (var model in orderdetails)
                {
                    var wProduct = wProudcts.ItemList.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
                    var product = products.ItemList.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
                    if (product == null)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("没有找到商品{0}", model.SKU)
                        }.ToJsonString();
                        return Content(result);
                    }
                    if (wProduct == null)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("仓库中没有找到商品{0}", model.SKU)
                        }.ToJsonString();
                        return Content(result);
                    }

                    FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsRequestDto temp = new FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsRequestDto();
                    temp.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                    temp.BackID = orderdto.BackID;
                    temp.ProductName = model.ProductName;
                    temp.BarCode = model.BarCode;
                    temp.BackUnit = model.BackUnit;
                    temp.BackPackingQty = model.BackPackingQty;
                    temp.BackQty = model.BackQty;
                    temp.BackPrice = (double)model.BackPrice;
                    temp.ProductId = model.ProductId;
                    temp.SKU = model.SKU;
                    temp.ProductImageUrl200 = wProduct.ImageUrl200x200;          //从网仓商品中取得
                    temp.ProductImageUrl400 = wProduct.ImageUrl400x400;          //从网仓商品中取得
                    temp.Unit = wProduct.Unit;
                    temp.UnitQty = (model.BackPackingQty * model.BackQty);       //总数量  计算出来
                    temp.UnitPrice = (double)model.UnitPrice;                    //不计算  (model.BuyPrice / model.BuyPackingQty); 
                    temp.SubAmt = ((double)model.BackPrice * (double)model.BackQty);
                    temp.Remark = model.Remark;
                    temp.SalePrice = model.SaleUnitPrice;

                    orderdetailsdto.Add(temp);

                    TotalAmt = TotalAmt + ((double)model.BackPrice * (double)model.BackQty);

                    FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsExtRequestDto detailExt
                        = new FrxsErpOrderBuyBackPreAddOrEditRequest.BuyBackPreDetailsExtRequestDto();
                    var shopCategoryNames = product.ShopCategoryName.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                    detailExt.ShopCategoryId1 = product.ShopCategoryId1;
                    detailExt.ShopCategoryId1Name = shopCategoryNames[0];
                    detailExt.ShopCategoryId2 = product.ShopCategoryId2;
                    detailExt.ShopCategoryId2Name = shopCategoryNames[1];
                    detailExt.ShopCategoryId3 = product.CategoryId3;
                    detailExt.ShopCategoryId3Name = shopCategoryNames[2];
                    detailExt.BrandId1 = product.BrandId1;
                    var brand1 = brandList.Where(x => x.BrandId == detailExt.BrandId1).FirstOrDefault();
                    if (brand1 != null)
                    {
                        detailExt.BrandId1Name = brand1.BrandName;
                    }
                    else
                    {
                        detailExt.BrandId1Name = "";
                    }
                    detailExt.BrandId2 = product.BrandId2;
                    var brand2 = brandList.Where(x => x.BrandId == detailExt.BrandId2).FirstOrDefault();
                    if (brand2 != null)
                    {
                        detailExt.BrandId2Name = brand2.BrandName;
                    }
                    else
                    {
                        detailExt.BrandId1Name = "";
                    }
                    detailExt.CategoryId1 = product.CategoryId1;
                    detailExt.CategoryId2 = product.CategoryId2;
                    detailExt.CategoryId3 = product.CategoryId3;
                    var categoryNames = product.CategoryName.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                    detailExt.CategoryId1Name = categoryNames[0];
                    detailExt.CategoryId2Name = categoryNames[1];
                    detailExt.CategoryId3Name = categoryNames[2];
                    detailExt.ModifyTime = DateTime.Now;
                    orderdetailsextdto.Add(detailExt);
                }

                orderdto.TotalAmt = TotalAmt;
                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderserviceCenter.Execute(new FrxsErpOrderBuyBackPreAddOrEditRequest()
                {
                    order = orderdto,
                    orderdetails = orderdetailsdto,
                    orderdetailsext = orderdetailsextdto,
                    WarehouseId = warehouseId,
                    Flag = flag,
                    UserId = userid,
                    UserName = username
                });

                if (resp.Flag == 0)
                {
                    //写操作日志
                    string action = string.Empty;
                    if (flag == "Edit")
                    {
                        action = ConstDefinition.XSOperatorActionEdit;
                    }
                    else
                    {
                        action = ConstDefinition.XSOperatorActionAdd;
                    }
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_5C,
                          action, string.Format("{0}采购退货单号[{1}]", action, orderdto.BackID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = new
                        {
                            BackID = orderdto.BackID,
                            UserId = WorkContext.UserIdentity.UserId,
                            UserName = WorkContext.UserIdentity.UserName,
                            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                        }
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
    }

    /// <summary>
    /// 退货订单 主表模型
    /// </summary>
    public class BackOrder
    {
        public string BackID { get; set; }
        public DateTime OrderDate { get; set; }
        public int SubWID { get; set; }
        public int VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public int BuyEmpID { get; set; }
        public string BuyEmpName { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 退货订单 详情模型
    /// </summary>
    public class BackOrderDetails
    {
        public int Index { get; set; }
        public decimal UnitQty { get; set; }
        public decimal OldBuyPrice { get; set; }
        public decimal WStock { get; set; }

        public int ProductId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string BackUnit { get; set; }
        public decimal BackQty { get; set; }
        public decimal BackPackingQty { get; set; }
        public decimal BackPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubAmt { get; set; }
        public string Remark { get; set; }
        public string BarCode { get; set; }

        public decimal SaleUnitPrice { get; set; }
        public decimal SalePrice { get; set; }

        public string MinUnit { get; set; }
        public decimal MinBuyPrice { get; set; }
        public decimal MinSalePrice { get; set; }
        public decimal MaxBuyPrice { get; set; }
        public decimal MaxSalePrice { get; set; }
    }
}
