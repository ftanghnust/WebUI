using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.ID.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Enum;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Import;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Product;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Excel;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Product
{
    /// <summary>
    /// 商品加入列表
    /// </summary>
    [ValidateInput(false)]
    public class ProductListController : BaseController
    {

        /// <summary>
        /// 商品加入列表
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520221)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="productListSearchModel">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [AuthorizeMenuFilter(520221)]
        [ValidateInput(false)]
        public ActionResult GetProductList(ProductListSearchModel productListSearchModel)
        {
            string jsonStr = "[]";
            try
            {
                string sortBy = "Sku  asc";
                if (!productListSearchModel.sort.IsNullOrEmpty() && !productListSearchModel.order.IsNullOrEmpty())
                {
                    sortBy = productListSearchModel.sort + "  " + productListSearchModel.order;
                }
                var r = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsWaitAddListGetRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ProductName = productListSearchModel.ProductName == null ? null : productListSearchModel.ProductName.Trim(),
                    BarCode = productListSearchModel.BarCode == null ? null : productListSearchModel.BarCode.Trim(),
                    CategoryId1 = productListSearchModel.CategoriesId1,
                    CategoryId2 = productListSearchModel.CategoriesId2,
                    CategoryId3 = productListSearchModel.CategoriesId3,
                    SKU = productListSearchModel.Sku == null ? null : productListSearchModel.Sku.Trim(),
                    PageIndex = productListSearchModel.page,
                    PageSize = productListSearchModel.rows,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    SortBy = sortBy
                };

                //获取列表
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
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
        /// 查看商品主图
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult ShowProductPictureDetail(int productId)
        {
            //新增和修改信息
            ProductPictureDetailsDataModel data = null;
            var resp = WorkContext.CreateProductSdkClient().Execute(request: new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductProductsPictureDetailGetRequest { ProductId = productId });
            if (resp != null && resp.Flag == 0 && resp.Data != null)
            {
                if (resp.Data.ProductsPictureDetailList != null)
                    data = new ProductPictureDetailsDataModel
                    {
                        IsBaseProductPicture = resp.Data.IsBaseProductPicture,
                        ProductsPictureDetailList = resp.Data.ProductsPictureDetailList
                    };
            }
            return View(data);
        }




        /// <summary>
        /// 商品价格调整单
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520224)]
        public ActionResult ProductPriceChange()
        {
            return View();
        }

        /// <summary>
        /// 商品价格调整单——添加编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPriceChangeAddOrEdit()
        {
            return View();
        }

        /// <summary>
        /// 商品价格调整单
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPriceChangeList(AdjustPriceListSearchModel mode)
        {
            string jsonStr = "[]";
            try
            {
                var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjustPriceListGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdjType = mode.AdjType,     //0:采购(进货)价; 1:配送(批发)价; 3:费率及积分
                    AdjID = mode.AdjID,
                    ProductName = mode.ProductName,
                    SKU = mode.SKU,
                    BarCode = mode.BarCode,
                    Status = mode.Status,
                    BeginTime1 = mode.PostingTime1,
                    BeginTime2 = mode.PostingTime2,
                    PageIndex = mode.page,
                    PageSize = mode.rows
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
        /// 配送价格调整单保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProductPriceChangeAddOrEditHandle(string jsonData, string jsonDetails)
        {
            var result = string.Empty;
            try
            {
                var adj = JsonHelper.FromJson<AdjPrice>(jsonData);
                var adjDetails = JsonHelper.GetObjectIList<AdjPriceDetail>(jsonDetails);

                var listAdj = new List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceAddOrUpdateRequest.ItemDetail>();
                foreach (var item in adjDetails)
                {
                    var adjDetail = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceAddOrUpdateRequest.ItemDetail
                    {
                        ProductId = item.ProductID,
                        Price = item.Price,
                        ShopPerc = (decimal?)item.ShopPerc,
                        ShopPoint = (decimal?)item.ShopPoint,
                        BasePoint = (decimal?)item.BasePoint,
                        VendorPerc1 = (decimal?)item.VendorPerc1,
                        VendorPerc2 = (decimal?)item.VendorPerc2,
                    };

                    listAdj.Add(adjDetail);

                }

                int? adjId = null;
                if (!string.IsNullOrEmpty(adj.AdjID))
                {
                    adjId = int.Parse(adj.AdjID);
                }

                var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceAddOrUpdateRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdjID = adjId,
                    AdjType = adj.AdjType,//配送单
                    BeginTime = adj.BeginTime,
                    Remark = adj.Remark,
                    Items = listAdj
                });


                if (resp != null && resp.Flag == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = resp.ToJsonString()
                    }.ToJsonString();

                    //写日志
                    WriteSaveLog(adj.AdjID, adj.AdjType, resp.Data);
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp != null ? resp.Info : "调用删除接口异常"
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
        /// 写日志
        /// </summary>
        public void WriteSaveLog(string adjId, int type, int? adjCode)
        {
            var action = ConstDefinition.XSOperatorActionAdd;
            if (!string.IsNullOrEmpty(adjId))
            {
                action = ConstDefinition.XSOperatorActionEdit;
            }
            var typeName = "";
            var menu = ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2E;
            if (type == 0)
            {
                menu = ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2E;
                typeName = "进货价";
            }
            else if (type == 1)
            {
                menu = ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2D;
                typeName = "配送价格";
            }
            else if (type == 2)
            {
                menu = ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2F;
                typeName = "费率积分";
            }

            //写操作日志
            OperatorLogHelp.Write(menu, action, string.Format("{0}{1}调整单[{2}]", action, typeName, adjCode));

        }


        /// <summary>
        /// 获取商品价格调整单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductPriceInfo(int ajdId)
        {
            string jsonStr = "[]";
            try
            {
                var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdjID = ajdId
                });

                if (resp != null && resp.Flag == 0)
                {
                    var data = new
                    {
                        adjPrice = resp.Data.WProductAdjPrice,
                        gridData = new
                        {
                            total = resp.Data.Details.Count,
                            rows = resp.Data.Details
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
        /// 价格调整单 - 确认订单-反确认
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPriceConfirmOrReconfirm(string ajdIds, int type, int menuType)
        {
            var listIds = ajdIds.Split(',').Select(int.Parse).ToList();

            string jsonStr;
            try
            {
                var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjustPriceConfirmRequest()
                {
                    WID = WorkContext.CurrentWarehouse.WarehouseId,
                    AdjIds = listIds,
                    Type = type == 0 ? FrxsErpProductWProductAdjustPriceConfirmRequest.OperatorType.Confirm : FrxsErpProductWProductAdjustPriceConfirmRequest.OperatorType.UndoConfirm
                });


                if (resp != null && resp.Flag == 0)
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "成功"
                    }.ToJsonString();


                    var menuName = menuType == 0 ? "进货价" :
                                   menuType == 1 ? "配送价格" :
                                   menuType == 2 ? "费率积分" : "";

                    var menu = menuType == 0 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2E :
                                   menuType == 1 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2D :
                                   menuType == 2 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2F : 0;

                    var action = type == 0 ? ConstDefinition.XSOperatorActionSure : ConstDefinition.XSOperatorActionNoSure;

                    //写操作日志
                    OperatorLogHelp.Write(menu, action, string.Format("{0}{1}调整单[{2}]", action, menuName, ajdIds));

                }
                else
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp != null ? resp.Info : "调用删除接口异常"
                    }.ToJsonString();
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
        /// 价格调整单 -  立即生效
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPricePosting(string ajdIds, int menuType)
        {
            var listIds = ajdIds.Split(',').Select(int.Parse).ToList();

            string jsonStr;
            try
            {
                var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWProductAdjustPricePostingRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdjIds = listIds
                });


                if (resp != null && resp.Flag == 0)
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "成功"
                    }.ToJsonString();


                    var menuName = menuType == 0 ? "进货价" :
                                   menuType == 1 ? "配送价格" :
                                   menuType == 2 ? "费率积分" : "";

                    var menu = menuType == 0 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2E :
                                   menuType == 1 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2D :
                                   menuType == 2 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2F : 0;

                    var action = ConstDefinition.XSOperatorActionEffective;

                    //写操作日志
                    OperatorLogHelp.Write(menu, action, string.Format("{0}{1}调整单[{2}]", action, menuName, ajdIds));

                }
                else
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp != null ? resp.Info : "调用删除接口异常"
                    }.ToJsonString();
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
        /// 价格调整单 - 过账
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPriceDel(string ajdIds, int menuType)
        {
            var listIds = ajdIds.Split(',').Select(int.Parse).ToList();

            string jsonStr;
            try
            {
                var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWProductAdjustPriceDelRequest()
                {
                    WID = WorkContext.CurrentWarehouse.WarehouseId,
                    AdjIds = listIds
                });


                if (resp != null && resp.Flag == 0)
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "成功"
                    }.ToJsonString();

                    var menuName = menuType == 0 ? "进货价" :
                                   menuType == 1 ? "配送价格" :
                                   menuType == 2 ? "费率积分" : "";

                    var menu = menuType == 0 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2E :
                                   menuType == 1 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2D :
                                   menuType == 2 ? ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2F : 0;

                    var action = ConstDefinition.XSOperatorActionDel;

                    //写操作日志
                    OperatorLogHelp.Write(menu, action, string.Format("{0}{1}调整单[{2}]", action, menuName, ajdIds));
                }
                else
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp != null ? resp.Info : "调用删除接口异常"
                    }.ToJsonString();
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
        /// 积分价促销
        /// </summary>
        /// <returns></returns>
        public ActionResult PriceSales()
        {
            return View();
        }

        /// <summary>
        /// 积分价促销-添加新增
        /// </summary>
        /// <returns></returns>
        public ActionResult PriceSalesAddOrEdit()
        {
            return View();
        }



        /// <summary>
        /// 采购订单 主表模型
        /// </summary>
        public class AdjPrice
        {
            public string AdjID { get; set; }
            public DateTime? BeginTime { get; set; }
            public int AdjType { get; set; }
            public string Remark { get; set; }
        }

        /// <summary>
        /// 采购订单 详情模型
        /// </summary>
        public class AdjPriceDetail
        {
            public int ProductID { get; set; }
            public decimal Price { get; set; }
            /// <summary>
            /// 门店库存单位积分
            /// </summary>
            public double ShopPoint { get; set; }
            /// <summary>
            /// 平台费率
            /// </summary>
            public double ShopPerc { get; set; }
            /// <summary>
            /// 库存单位绩效积分(司机及门店共用)
            /// </summary>
            public double BasePoint { get; set; }
            /// <summary>
            /// 库存单位物流费率(供应商) (%)
            /// </summary>
            public double VendorPerc1 { get; set; }
            /// <summary>
            /// 库存单位仓储费率(供应商)(%)
            /// </summary>
            public double VendorPerc2 { get; set; }
            
            
        }




        #region EXCEL导入 导出

        /// <summary>
        /// 配送价格调整单导入
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportShippPrice()
        {
            return View();
        }


        /// <summary>
        ///  获EXCEL里的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImportData(string fileName, string folderPath, int type)
        {
            //type:0配送价格 1费率积分 2进货价
            string jsonStr = "[]";
            try
            {
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;
                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);

                var list = new List<ImportPriceModel>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][1].ToString().Trim() == "")
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行商品编码不能为空"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }

                    var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWProductsSaleListGetRequest()
                    {
                        PageIndex = 1,
                        PageSize = 10,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SKU = dt.Rows[i][1].ToString(),
                        SKULikeSearch = true
                    });

                    if (resp != null && resp.Flag == 0 && resp.Data != null && resp.Data.TotalRecords > 0)
                    {
                        //没找到添加一个空的
                        var model = new ImportPriceModel();
                        model.BarCode = resp.Data.ItemList[0].BarCode;
                        model.BasePoint = resp.Data.ItemList[0].BasePoint;
                        model.PackingQty = resp.Data.ItemList[0].PackingQty;
                        model.ProductId = resp.Data.ItemList[0].ProductId;
                        model.ProductName = resp.Data.ItemList[0].ProductName;
                        model.SKU = resp.Data.ItemList[0].SKU;
                        model.SalePrice = resp.Data.ItemList[0].SalePrice;
                        model.SaleUnit = resp.Data.ItemList[0].SaleUnit;
                        model.ShopAddPerc = resp.Data.ItemList[0].ShopAddPerc;
                        model.ShopPoint = resp.Data.ItemList[0].ShopPoint;
                        model.Unit = resp.Data.ItemList[0].Unit;
                        model.UnitPrice = resp.Data.ItemList[0].UnitPrice;
                        model.BuyPrice = resp.Data.ItemList[0].BuyPrice;
                        model.VendorPerc1 = resp.Data.ItemList[0].VendorPerc1;
                        model.VendorPerc2 = resp.Data.ItemList[0].VendorPerc2;


                        //EXCEL里输入的价格
                        //新配送价
                        if (type == 0)
                        {
                            decimal newprice = 0;
                            if (!decimal.TryParse(dt.Rows[i][2].ToString(), out newprice))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新库存单位配送价不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            if (newprice > (decimal)999999.9999)
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新库存单位配送价不能大于999999.9999"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            model.NewPrice = newprice;
                        }
                        else if (type == 1)
                        {
                            //新门店积分
                            decimal newShopPoint = 0;
                            if (!decimal.TryParse(dt.Rows[i][2].ToString(), out newShopPoint))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新门店积分不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            model.NewShopPoint = newShopPoint;


                            //新绩效分率
                            decimal newBasePoint = 0;
                            if (!decimal.TryParse(dt.Rows[i][3].ToString(), out newBasePoint))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新绩效分率不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            model.NewBasePoint = newBasePoint;

                            //新物流费率
                            decimal newVendorPerc1 = 0;
                            if (!decimal.TryParse(dt.Rows[i][4].ToString(), out newVendorPerc1))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新物流费率不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            model.NewVendorPerc1 = newVendorPerc1;

                            //新仓储费率
                            decimal newVendorPerc2 = 0;
                            if (!decimal.TryParse(dt.Rows[i][5].ToString(), out newVendorPerc2))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新仓储费率不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            model.NewVendorPerc2 = newVendorPerc2;


                            //新平台费率
                            decimal newShopAddPerc = 0;
                            if (!decimal.TryParse(dt.Rows[i][6].ToString(), out newShopAddPerc))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新平台费率不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            model.NewShopAddPerc = newShopAddPerc;
                            

                        }
                        else if (type == 2)
                        {
                            //新进货价
                            decimal newprice = 0;
                            if (!decimal.TryParse(dt.Rows[i][2].ToString(), out newprice))
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新库存单位进货价不正确"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }
                            if (newprice > (decimal)999999.9999)
                            {
                                jsonStr = new ResultData
                                {
                                    Flag = ConstDefinition.FLAG_FAIL,
                                    Info = "Excel第 " + (i + 1) + " 行新库存单位进货价不能大于999999.9999"
                                }.ToJsonString();
                                return Content(jsonStr);
                            }

                            model.NewPrice = newprice;

                        }

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
        /// 导出EXCEL-配送价格调整单
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcel(int ajdId)
        {
            var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                AdjID = ajdId
            });

            if (resp != null && resp.Flag == 0)
            {
                var details = resp.Data.Details;
                var fileName = "配送价格调整单_" + ajdId + ".xls";

                var dataTable = new DataTable();
                dataTable.Columns.Add("商品编码");
                dataTable.Columns.Add("商品名称");
                dataTable.Columns.Add("库存单位");
                dataTable.Columns.Add("包装数");
                dataTable.Columns.Add("原配送价");
                dataTable.Columns.Add("配送价");
                dataTable.Columns.Add("国际条码");

                foreach (var item in details)
                {
                    var newRow = dataTable.NewRow();
                    newRow["商品编码"] = item.SKU;
                    newRow["商品名称"] = item.ProductName;
                    newRow["库存单位"] = item.Unit;
                    newRow["包装数"] = item.PackingQty;
                    newRow["原配送价"] = item.OldPrice;
                    newRow["配送价"] = item.Price;
                    newRow["国际条码"] = item.BarCode;
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


        #endregion

    }
}
