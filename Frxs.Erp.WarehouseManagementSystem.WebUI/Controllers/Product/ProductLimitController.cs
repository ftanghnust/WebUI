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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Product
{
    public class ProductLimitController : BaseController
    {
        [AuthorizeMenuFilter(520228)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetProductLimitList(ProductLimitQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ProductLimitModel().GetProductLimitPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        [ValidateInput(false)]
        public ActionResult ProductLimitAddOrEdit(string id)
        {
            return View();
        }

        public ActionResult GetProductLimit(string id)
        {
            var productLimitModel = new ProductLimitModel();
            var model = new ProductLimitModel();
            if (!String.IsNullOrEmpty(id))
            {
                model = productLimitModel.GetProductLimitData(id);
            }
            else
            {
                //var serviceCenter = WorkContext.CreateIDSdkClient();
                //var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
                //{
                //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                //    Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.BaseInfoID
                //});
                //if (resp != null && resp.Flag == 0)
                //{
                //    var noSaleID = resp.Data;
                //    model.Status = 0;
                //    model.NoSaleID = noSaleID;
                //    model.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                //    model.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                //    model.CreateUserID = UserIdentity.UserId;
                //    model.CreateUserName = UserIdentity.UserName;
                //    model.CreateTime = DateTime.Now;
                //}
                //else
                //{
                //    return Content(String.Empty);
                //}
                model.Status = 0;
                model.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                model.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
            }
            return Content(model.ToJsonString());
        }

        public ActionResult DeleteProductLimit(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    //ids = ids.Substring(0, ids.Length - 1);
                    int rows = new ProductLimitModel().DeleteProductLimit(ids);
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

        public ActionResult ProductLimitProduct()
        {
            return View();
        }

        public ActionResult ProductLimitGroup()
        {
            return View();
        }

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
                    WStatus = 1,//修复bug 4146 淘汰的商品不能加入待选列表，经过询问需求方，此处只选择“正常”的商品即可
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

        public ActionResult GetShopGroupList(ShopgroupListQuery cpm)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupTableListRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    GroupCode = (cpm.searchType == "GroupCode") ? cpm.searchKey : null,
                    GroupName = (cpm.searchType == "GroupName") ? cpm.searchKey : null,
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
                else
                {
                    //result = new ResultData
                    //{
                    //    Flag = ConstDefinition.FLAG_FAIL,
                    //    Info = string.Format("获取数据异常！")
                    //}.ToJsonString();
                    var obj = new { total = 0, rows = 0 };
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


        [ValidateInput(false)]
        [AuthorizeButtonFiter(520228, 52022801)]
        public ActionResult AddProductLimit(ProductLimitModel model)
        {
            var productLimitId = GetProductLimitId();
            if (productLimitId.Equals(String.Empty))
            {
                var resultData = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "ID获取失败",
                    Data = null
                };
                return Content(resultData.ToJsonString());
            }
            model.NoSaleID = productLimitId;
            string flag = "Add";
            string result = string.Empty;
            var products = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<Frxs.Erp.WarehouseManagementSystem.WebUI.Models.ProductLimitModel.WProductNoSaleDetails>(model.jsonProduct);
            var groups = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<Frxs.Erp.WarehouseManagementSystem.WebUI.Models.ProductLimitModel.WProductNoSaleShops>(model.jsonGroup);
            Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest noSale = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest();
            noSale.Flag = flag;
            noSale.BeginTime = model.BeginTime != null ? (DateTime)model.BeginTime : DateTime.Now;
            noSale.EndTime = model.EndTime != null ? (DateTime)model.EndTime : DateTime.Now;
            //noSale.ConfTime = null;
            //noSale.ConfUserID = null;
            //noSale.ConfUserName = null;
            //noSale.PostingTime = null;
            //noSale.PostingUserID = null;
            //noSale.PostingUserName = null;
            noSale.WCode = CurrentWarehouse.Parent.WarehouseCode;
            //noSale.WID = model.WID;
            noSale.WID = CurrentWarehouse.Parent.WarehouseId;
            noSale.WName = CurrentWarehouse.Parent.WarehouseName;
            noSale.Status = model.Status;
            noSale.NoSaleID = model.NoSaleID;
            noSale.PromotionName = model.PromotionName;
            noSale.Remark = model.Remark;
            List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleDetails> productsdto = new List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleDetails>();
            foreach (var product in products)
            {
                var productdto = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleDetails()
                {
                    BarCode = product.BarCode,
                    BigPackingQty = product.BigPackingQty,
                    BigUnit = product.BigUnit,
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    NoSaleID = model.NoSaleID,
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    SalePrice = product.SalePrice,
                    Unit = product.Unit,
                    WID = CurrentWarehouse.Parent.WarehouseId,
                    WProductID = product.WProductID
                };
                productsdto.Add(productdto);
            }
            List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleShops> groupsdto = new List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleShops>();
            foreach (var group in groups)
            {
                var groupdto = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleShops()
                {
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    GroupID = group.GroupID,
                    GroupName = group.GroupName,
                    NoSaleID = model.NoSaleID,
                    ShopNum = group.ShopNum,
                    WID = CurrentWarehouse.Parent.WarehouseId
                };
                groupsdto.Add(groupdto);
            }
            noSale.DetailsList = productsdto;
            noSale.ShopList = groupsdto;
            var productSdkClient = WorkContext.CreateProductSdkClient();
            var resp = productSdkClient.Execute(noSale);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                  ConstDefinition.XSOperatorActionAdd, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionAdd, noSale.NoSaleID));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功",
                    Data = noSale.NoSaleID
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
            return Content(result);
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520228, 52022802)]
        public ActionResult EditProductLimit(ProductLimitModel model)
        {
            string flag = "Edit";
            string result = string.Empty;
            var products = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<Frxs.Erp.WarehouseManagementSystem.WebUI.Models.ProductLimitModel.WProductNoSaleDetails>(model.jsonProduct);
            var groups = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<Frxs.Erp.WarehouseManagementSystem.WebUI.Models.ProductLimitModel.WProductNoSaleShops>(model.jsonGroup);
            Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest noSale = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest();
            noSale.Flag = flag;
            noSale.BeginTime = model.BeginTime != null ? (DateTime)model.BeginTime : DateTime.Now;
            noSale.EndTime = model.EndTime != null ? (DateTime)model.EndTime : DateTime.Now;
            //noSale.ConfTime = null;
            //noSale.ConfUserID = null;
            //noSale.ConfUserName = null;
            //noSale.PostingTime = null;
            //noSale.PostingUserID = null;
            //noSale.PostingUserName = null;
            noSale.WCode = CurrentWarehouse.Parent.WarehouseCode;
            //noSale.WID = model.WID;
            noSale.WID = CurrentWarehouse.Parent.WarehouseId;
            noSale.WName = CurrentWarehouse.Parent.WarehouseName;
            noSale.Status = model.Status;
            noSale.NoSaleID = model.NoSaleID;
            noSale.PromotionName = model.PromotionName;
            noSale.Remark = model.Remark;
            List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleDetails> productsdto = new List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleDetails>();
            foreach (var product in products)
            {
                var productdto = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleDetails()
                {
                    BarCode = product.BarCode,
                    BigPackingQty = product.BigPackingQty,
                    BigUnit = product.BigUnit,
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    NoSaleID = model.NoSaleID,
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    SalePrice = product.SalePrice,
                    Unit = product.Unit,
                    WID = CurrentWarehouse.Parent.WarehouseId,
                    WProductID = product.WProductID
                };
                productsdto.Add(productdto);
            }
            List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleShops> groupsdto = new List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleShops>();
            foreach (var group in groups)
            {
                var groupdto = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleSaveRequest.WProductNoSaleShops()
                {
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    GroupID = group.GroupID,
                    GroupName = group.GroupName,
                    NoSaleID = model.NoSaleID,
                    ShopNum = group.ShopNum,
                    WID = CurrentWarehouse.Parent.WarehouseId
                };
                groupsdto.Add(groupdto);
            }
            noSale.DetailsList = productsdto;
            noSale.ShopList = groupsdto;
            var productSdkClient = WorkContext.CreateProductSdkClient();
            var resp = productSdkClient.Execute(noSale);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                  ConstDefinition.XSOperatorActionEdit, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionEdit, noSale.NoSaleID));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功",
                    Data = noSale.NoSaleID
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
            return Content(result);
        }

        [AuthorizeButtonFiter(520228, 52022804)]
        public ActionResult ConfirmProductLimit(string ids, int flag)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreateProductSdkClient();
            Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleConfirmRequest req = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleConfirmRequest();
            req.IdList = idlist;
            req.Flag = flag;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (flag.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                           ConstDefinition.XSOperatorActionSure, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionSure, ids));
                }
                else if (flag.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                           ConstDefinition.XSOperatorActionNoSure, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionNoSure, ids));
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    //Info = "操作成功",
                    Info = flag.Equals(1) ? "确认成功" : "反确认成功",
                    Data = resp.ToJsonString()
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
            return Content(result);
        }

        [AuthorizeButtonFiter(520228, 52022806)]
        public ActionResult UnConfirmProductLimit(string ids, int flag)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreateProductSdkClient();
            Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleConfirmRequest req = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleConfirmRequest();
            req.IdList = idlist;
            req.Flag = flag;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (flag.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                           ConstDefinition.XSOperatorActionSure, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionSure, ids));
                }
                else if (flag.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                           ConstDefinition.XSOperatorActionNoSure, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionNoSure, ids));
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    //Info = "操作成功",
                    Info = "反确认成功",
                    Data = resp.ToJsonString()
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
            return Content(result);
        }

        [AuthorizeButtonFiter(520228, 52022805)]
        public ActionResult PostingProductLimit(string ids)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreateProductSdkClient();
            Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSalePostingRequest req = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSalePostingRequest();
            req.IdList = idlist;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                       ConstDefinition.XSOperatorActionEffective, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionEffective, ids));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    //Info = "操作成功",
                    Info = "过账成功",
                    Data = resp.ToJsonString()
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
            return Content(result);
        }

        [AuthorizeButtonFiter(520228, 52022807)]
        public ActionResult StopProductLimit(string ids)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreateProductSdkClient();
            Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleStopRequest req = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleStopRequest();
            req.IdList = idlist;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                       ConstDefinition.XSOperatorActionStop, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionStop, ids));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    //Info = "操作成功",
                    Info = "停用成功",
                    Data = resp.ToJsonString()
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
            return Content(result);
        }

        public string GetProductLimitId()
        {
            var serviceCenter = WorkContext.CreateIDSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.BaseInfoID
            });
            if (resp != null && resp.Flag == 0)
            {
                return resp.Data;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
