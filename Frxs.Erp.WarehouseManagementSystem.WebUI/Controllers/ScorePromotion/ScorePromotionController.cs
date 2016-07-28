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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.ScorePromotion
{
    public class ScorePromotionController : BaseController
    {
        private int promotionType = 1;

        [AuthorizeMenuFilter(520413)]
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
        public ActionResult GetScorePromotionList(PlatformRateQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                cpm.PromotionType = promotionType;
                cpm.PromotionName = promotionType.Equals(2) ? String.Empty : cpm.PromotionName;
                jsonStr = new PlatformRateModel().GetPlatformRatePageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 新增编辑视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeMenuFilter(520413)]
        public ActionResult ScorePromotionAddOrEdit(string id)
        {
            return View();
        }

        /// <summary>
        /// 复制视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeMenuFilter(520413)]
        public ActionResult ScorePromotionCopy(string id)
        {
            return View();
        }

        /// <summary>
        /// 获取积分促销
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetScorePromotion(string id)
        {
            var platformRateModel = new PlatformRateModel();
            var model = new PlatformRateModel();
            if (!String.IsNullOrEmpty(id))
            {
                model = platformRateModel.GetPlatformRateData(WorkContext.CurrentWarehouse.Parent.WarehouseId, id);
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
                //    var promotionID = resp.Data;
                //    model.Status = 0;
                //    model.PromotionID = promotionID;
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

        /// <summary>
        /// 复制积分促销
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetScorePromotionCopy(string id)
        {
            var platformRateModel = new PlatformRateModel();
            var model = new PlatformRateModel();
            if (!String.IsNullOrEmpty(id))
            {
                model = platformRateModel.GetScorePromotionDataCopy(WorkContext.CurrentWarehouse.Parent.WarehouseId, id);
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
                //    var promotionID = resp.Data;
                //    model.Status = 0;
                //    model.PromotionID = promotionID;
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

        /// <summary>
        /// 积分促销商品视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ScorePromotionProduct()
        {
            return View();
        }

        /// <summary>
        /// 积分促销门店视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ScorePromotionGroup()
        {
            return View();
        }

        /// <summary>
        /// 商品选择列表
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
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
                    WStatus = 1,//修复bug 4145 淘汰的商品不能加入待选列表，经过询问需求方，此处只选择“正常”的商品即可
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
        /// 门店选择列表
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        public ActionResult GetShopList(ShopListQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopWarehouseTableListRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                    ShopCode = (cpm.searchType == "ShopCode") ? cpm.searchKey : null,
                    ShopName = (cpm.searchType == "ShopName") ? cpm.searchKey : null,
                    SortBy = "ShopCode asc"
                });

                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 新增积分促销
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520413, 52041301)]
        public ActionResult AddScorePromotion(PlatformRateModel model)
        {
            var scorePromotionId = GetScorePromotionId();
            if (scorePromotionId.Equals(String.Empty))
            {
                var resultData = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "ID获取失败",
                    Data = null
                };
                return Content(resultData.ToJsonString());
            }
            model.PromotionID = scorePromotionId;
            string flag = "Add";
            string result = string.Empty;
            var products = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<PlatformRateModel.WProductPromotionDetails>(model.jsonProduct);
            var groups = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<PlatformRateModel.WProductPromotionShops>(model.jsonGroup);
            Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest promotion = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest();
            promotion.Flag = flag;
            promotion.BeginTime = model.BeginTime != null ? (DateTime)model.BeginTime : DateTime.Now;
            promotion.EndTime = model.EndTime != null ? (DateTime)model.EndTime : DateTime.Now;
            promotion.WCode = CurrentWarehouse.Parent.WarehouseCode;
            promotion.WarehouseId = CurrentWarehouse.Parent.WarehouseId;
            promotion.WID = CurrentWarehouse.Parent.WarehouseId;
            promotion.WName = CurrentWarehouse.Parent.WarehouseName;
            promotion.Status = model.Status;
            promotion.PromotionID = model.PromotionID;
            //promotion.PromotionName = model.PromotionName;
            promotion.PromotionName = promotionType.Equals(2) ? String.Empty : model.PromotionName;
            promotion.PromotionType = promotionType;
            promotion.Remark = model.Remark;
            var productsdto = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionDetails>();
            foreach (var product in products)
            {
                var productdto = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionDetails()
                {
                    SKU = product.SKU,
                    BarCode = product.BarCode,
                    PackingQty = product.PackingQty,
                    Unit = product.Unit,
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    PromotionID = model.PromotionID,
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    SaleUnit = product.SaleUnit,
                    SalePrice = product.SalePrice,
                    OldPoint = product.OldPoint,
                    Point = product.Point,
                    MaxOrderQty = product.MaxOrderQty,
                    WID = CurrentWarehouse.Parent.WarehouseId,
                    WProductID = product.WProductID
                };
                productsdto.Add(productdto);
            }
            var groupsdto = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionShops>();
            foreach (var group in groups)
            {
                var groupdto = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionShops()
                {
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    PromotionID = promotion.PromotionID,
                    ShopID = group.ShopID,
                    ShopCode = group.ShopCode,
                    ShopName = group.ShopName,
                    ShopType = group.ShopType,
                    FullAddress = group.FullAddress,
                    WID = group.WID
                };
                groupsdto.Add(groupdto);
            }
            promotion.DetailList = productsdto;
            promotion.ShopList = groupsdto;
            var productSdkClient = WorkContext.CreatePromotionSdkClient();
            var resp = productSdkClient.Execute(promotion);
            if (resp!=null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                  ConstDefinition.XSOperatorActionAdd, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionAdd, promotion.PromotionID));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功",
                    Data = promotion.PromotionID
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

        /// <summary>
        /// 编辑积分促销
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520413, 52041302)]
        public ActionResult EditScorePromotion(PlatformRateModel model)
        {
            string flag = "Edit";
            string result = string.Empty;
            var products = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<PlatformRateModel.WProductPromotionDetails>(model.jsonProduct);
            var groups = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<PlatformRateModel.WProductPromotionShops>(model.jsonGroup);
            Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest promotion = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest();
            promotion.Flag = flag;
            promotion.BeginTime = model.BeginTime != null ? (DateTime)model.BeginTime : DateTime.Now;
            promotion.EndTime = model.EndTime != null ? (DateTime)model.EndTime : DateTime.Now;
            //noSale.ConfTime = null;
            //noSale.ConfUserID = null;
            //noSale.ConfUserName = null;
            //noSale.PostingTime = null;
            //noSale.PostingUserID = null;
            //noSale.PostingUserName = null;
            promotion.WCode = CurrentWarehouse.Parent.WarehouseCode;
            promotion.WarehouseId = CurrentWarehouse.Parent.WarehouseId;
            promotion.WID = CurrentWarehouse.Parent.WarehouseId;
            promotion.WName = CurrentWarehouse.Parent.WarehouseName;
            promotion.Status = model.Status;
            promotion.PromotionID = model.PromotionID;
            //promotion.PromotionName = model.PromotionName;
            promotion.PromotionName = promotionType.Equals(2) ? String.Empty : model.PromotionName;
            promotion.PromotionType = promotionType;
            promotion.Remark = model.Remark;
            var productsdto = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionDetails>();
            foreach (var product in products)
            {
                var productdto = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionDetails()
                {
                    SKU = product.SKU,
                    BarCode = product.BarCode,
                    PackingQty = product.PackingQty,
                    Unit = product.Unit,
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    PromotionID = model.PromotionID,
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    SaleUnit = product.SaleUnit,
                    SalePrice = product.SalePrice,
                    OldPoint = product.OldPoint,
                    Point = product.Point,
                    MaxOrderQty = product.MaxOrderQty,
                    WID = CurrentWarehouse.Parent.WarehouseId,
                    WProductID = product.WProductID
                };
                productsdto.Add(productdto);
            }
            var groupsdto = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionShops>();
            foreach (var group in groups)
            {
                var groupdto = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionSaveRequest.WProductPromotionShops()
                {
                    CreateTime = DateTime.Now,
                    CreateUserID = UserIdentity.UserId,
                    CreateUserName = UserIdentity.UserName,
                    PromotionID = promotion.PromotionID,
                    ShopCode = group.ShopCode,
                    ShopID = group.ShopID,
                    ShopName = group.ShopName,
                    ShopType = group.ShopType,
                    FullAddress = group.FullAddress,
                    WID = group.WID
                };
                groupsdto.Add(groupdto);
            }
            promotion.DetailList = productsdto;
            promotion.ShopList = groupsdto;
            var productSdkClient = WorkContext.CreatePromotionSdkClient();
            var resp = productSdkClient.Execute(promotion);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                  ConstDefinition.XSOperatorActionEdit, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionEdit, promotion.PromotionID));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功",
                    Data = promotion.PromotionID
                }.ToJsonString();
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info,
                    Data = promotion.PromotionID
                }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 删除积分促销
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520413, 52041303)]
        public ActionResult DeleteScorePromotion(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    //ids = ids.Substring(0, ids.Length - 1);
                    int rows = new PlatformRateModel().DeletePlatformRate(ids, WorkContext.CurrentWarehouse.Parent.WarehouseId, promotionType);
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
        /// 确认积分促销
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520413, 52041304)]
        public ActionResult ConfirmScorePromotion(string ids, int flag)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreatePromotionSdkClient();
            var req = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionConfirmRequest();
            req.IdList = idlist;
            req.Flag = flag;
            req.PromotionType = promotionType;
            req.WarehouseId = CurrentWarehouse.Parent.WarehouseId;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (flag.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                           ConstDefinition.XSOperatorActionSure, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionSure, ids));
                }
                else if (flag.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                           ConstDefinition.XSOperatorActionNoSure, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionNoSure, ids));
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
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

        /// <summary>
        /// 反确认积分促销
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520413, 52041307)]
        public ActionResult UnConfirmScorePromotion(string ids, int flag)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreatePromotionSdkClient();
            var req = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionConfirmRequest();
            req.IdList = idlist;
            req.Flag = flag;
            req.PromotionType = promotionType;
            req.WarehouseId = CurrentWarehouse.Parent.WarehouseId;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (flag.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                           ConstDefinition.XSOperatorActionSure, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionSure, ids));
                }
                else if (flag.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                           ConstDefinition.XSOperatorActionNoSure, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionNoSure, ids));
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
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

        /// <summary>
        /// 生效积分促销
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520413, 52041305)]
        public ActionResult PostingScorePromotion(string ids)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreatePromotionSdkClient();
            var req = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionPostingRequest();
            req.IdList = idlist;
            req.PromotionType = promotionType;
            req.WarehouseId = CurrentWarehouse.Parent.WarehouseId;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                       ConstDefinition.XSOperatorActionEffective, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionEffective, ids));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
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

        /// <summary>
        /// 停用积分促销
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520413, 52041306)]
        public ActionResult StopScorePromotion(string ids)
        {
            string result = string.Empty;
            var idlist = ids.Split(',').ToList();
            var productSdkClient = WorkContext.CreatePromotionSdkClient();
            var req = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionStopRequest();
            req.IdList = idlist;
            req.PromotionType = promotionType;
            req.WarehouseId = CurrentWarehouse.Parent.WarehouseId;
            var resp = productSdkClient.Execute(req);
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                       ConstDefinition.XSOperatorActionStop, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionStop, ids));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
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

        public string GetScorePromotionId()
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
