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
using Frxs.Platform.Utility.Map;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Wadvertisement
{
    public class WadvertisementController : BaseController
    {
        [AuthorizeMenuFilter(520411)]
        public ActionResult WAdvertisementList(CustomerPageModel cpm)
        {
            return View();
        }

        [AuthorizeMenuFilter(520411)]
        public ActionResult WadvertisementProduct()
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
        public ActionResult GetWAdvertisementList(WadvertisementQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WadvertisementModel().GetWadvertisementPageData(cpm.page, cpm.rows, cpm.sort);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 获取橱窗推荐
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWAdvertisement(int id)
        {
            WadvertisementModel model = new WadvertisementModel();
            model.PageTitle = "修改橱窗推荐";
            //获取数据，绑定
            var resp = this.ErpPromotionSdkClient.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementGetModelRequest()
            {
                ID = id.ToString(),
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            if (resp != null && resp.Flag == 0)
            {
                model = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<WadvertisementModel>(resp.Data.order);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加或者编辑视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult WAdvertisementAddOrEdit(string id)
        {
            return View();
        }

        //[ValidateInput(false)]
        //public ActionResult WAdvertisementHandle(WadvertisementModel model)
        //{
        //    string flag = string.Empty;
        //    string result = string.Empty;
        //    try
        //    {
        //        Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementRequestDto orderdto
        //            = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementRequestDto();
        //        if (model.AdvertisementID == 0)
        //        {
        //            flag = "Add";
        //            orderdto.CreateTime = DateTime.Now;
        //            orderdto.CreateUserID = UserIdentity.UserId;
        //            orderdto.CreateUserName = UserIdentity.UserName;
        //            orderdto.ModityTime = DateTime.Now;
        //            orderdto.ModityUserID = UserIdentity.UserId;
        //            orderdto.ModityUserName = UserIdentity.UserName;
        //        }
        //        else
        //        {
        //            flag = "Edit";
        //            orderdto.AdvertisementID = model.AdvertisementID;
        //            orderdto.ModityTime = DateTime.Now;
        //            orderdto.ModityUserID = UserIdentity.UserId;
        //            orderdto.ModityUserName = UserIdentity.UserName;
        //        }
        //        orderdto.AdvertisementName = model.AdvertisementName;
        //        orderdto.Sort = model.Sort;
        //        orderdto.AdvertisementPosition = 3;//橱窗
        //        orderdto.StartTime = DateTime.Now;
        //        orderdto.EndTime = DateTime.Now;
        //        orderdto.ImagesSrc = model.ImagesSrc;
        //        orderdto.AdvertisementType = model.AdvertisementType;
        //        //orderdto.EndTime = model.EndTime;
        //        orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;

        //        var advertisementProductList = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementProductRequestDto>();
        //        if (!String.IsNullOrEmpty(model.AdvertisementProduct))
        //        {
        //            var products = model.AdvertisementProduct.Split(',');
        //            foreach (var product in products)
        //            {
        //                var advertisementProduct = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementProductRequestDto()
        //                {
        //                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
        //                    AdvertisementID = model.AdvertisementID,
        //                    CreateTime = DateTime.Now,
        //                    CreateUserID = UserIdentity.UserId,
        //                    CreateUserName = UserIdentity.UserName,
        //                    ProductID = int.Parse(product)
        //                };
        //                advertisementProductList.Add(advertisementProduct);
        //            }
        //        }

        //        var serviceCenter = WorkContext.CreatePromotionSdkClient();
        //        var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest()
        //        {
        //            order = orderdto,
        //            advertisementProduct = advertisementProductList,
        //            Flag = flag
        //        });

        //        if (resp.Flag == 0)
        //        {
        //            //写操作日志
        //            string action = string.Empty;
        //            if (flag == "Edit")
        //            {
        //                action = ConstDefinition.XSOperatorActionEdit;
        //                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
        //                  action, string.Format("{0}橱窗推荐[{1}]", action, orderdto.AdvertisementID));
        //            }
        //            else
        //            {
        //                action = ConstDefinition.XSOperatorActionAdd;
        //                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
        //                  action, string.Format("{0}橱窗推荐[{1}]", action, resp.Data));
        //            }

        //            result = new ResultData
        //            {
        //                Flag = ConstDefinition.FLAG_SUCCESS,
        //                Info = "操作成功",
        //                Data = orderdto.ToJsonString()
        //            }.ToJsonString();
        //        }
        //        else
        //        {
        //            result = new ResultData
        //            {
        //                Flag = ConstDefinition.FLAG_FAIL,
        //                Info = resp.Info
        //            }.ToJsonString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetInstance().Fatal(ex);
        //        result = new ResultData
        //        {
        //            Flag = ConstDefinition.FLAG_EXCEPTION,
        //            Info = string.Format("出现异常：{0}", ex.Message)
        //        }.ToJsonString();
        //    }
        //    return Content(result);
        //}

        /// <summary>
        /// 新增橱窗
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520411, 52041101)]
        public ActionResult AddWAdvertisement(WadvertisementModel model)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementRequestDto();
                if (model.AdvertisementID == 0)
                {
                    flag = "Add";
                    orderdto.CreateTime = DateTime.Now;
                    orderdto.CreateUserID = UserIdentity.UserId;
                    orderdto.CreateUserName = UserIdentity.UserName;
                    orderdto.ModityTime = DateTime.Now;
                    orderdto.ModityUserID = UserIdentity.UserId;
                    orderdto.ModityUserName = UserIdentity.UserName;
                }
                else
                {
                    flag = "Edit";
                    orderdto.AdvertisementID = model.AdvertisementID;
                    orderdto.ModityTime = DateTime.Now;
                    orderdto.ModityUserID = UserIdentity.UserId;
                    orderdto.ModityUserName = UserIdentity.UserName;
                }
                orderdto.AdvertisementName = model.AdvertisementName;
                orderdto.Sort = model.Sort;
                orderdto.AdvertisementPosition = 3;//橱窗
                orderdto.StartTime = DateTime.Now;
                orderdto.EndTime = DateTime.Now;
                orderdto.ImagesSrc = model.ImagesSrc;
                orderdto.AdvertisementType = model.AdvertisementType;
                //orderdto.EndTime = model.EndTime;
                orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;

                var advertisementProductList = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementProductRequestDto>();
                if (!String.IsNullOrEmpty(model.AdvertisementProduct))
                {
                    var products = model.AdvertisementProduct.Split(',');
                    foreach (var product in products)
                    {
                        var advertisementProduct = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementProductRequestDto()
                        {
                            WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                            AdvertisementID = model.AdvertisementID,
                            CreateTime = DateTime.Now,
                            CreateUserID = UserIdentity.UserId,
                            CreateUserName = UserIdentity.UserName,
                            ProductID = int.Parse(product)
                        };
                        advertisementProductList.Add(advertisementProduct);
                    }
                }

                var serviceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest()
                {
                    order = orderdto,
                    advertisementProduct = advertisementProductList,
                    Flag = flag
                });

                if (resp.Flag == 0)
                {
                    //写操作日志
                    string action = string.Empty;
                    if (flag == "Edit")
                    {
                        action = ConstDefinition.XSOperatorActionEdit;
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
                          action, string.Format("{0}橱窗推荐[{1}]", action, orderdto.AdvertisementID));
                    }
                    else
                    {
                        action = ConstDefinition.XSOperatorActionAdd;
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
                          action, string.Format("{0}橱窗推荐[{1}]", action, resp.Data));
                    }

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = orderdto.ToJsonString()
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

        /// <summary>
        /// 编辑橱窗
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520411, 52041102)]
        public ActionResult EditWAdvertisement(WadvertisementModel model)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementRequestDto();
                if (model.AdvertisementID == 0)
                {
                    flag = "Add";
                    orderdto.CreateTime = DateTime.Now;
                    orderdto.CreateUserID = UserIdentity.UserId;
                    orderdto.CreateUserName = UserIdentity.UserName;
                    orderdto.ModityTime = DateTime.Now;
                    orderdto.ModityUserID = UserIdentity.UserId;
                    orderdto.ModityUserName = UserIdentity.UserName;
                }
                else
                {
                    flag = "Edit";
                    orderdto.AdvertisementID = model.AdvertisementID;
                    orderdto.ModityTime = DateTime.Now;
                    orderdto.ModityUserID = UserIdentity.UserId;
                    orderdto.ModityUserName = UserIdentity.UserName;
                }
                orderdto.AdvertisementName = model.AdvertisementName;
                orderdto.Sort = model.Sort;
                orderdto.AdvertisementPosition = 3;//橱窗
                orderdto.StartTime = DateTime.Now;
                orderdto.EndTime = DateTime.Now;
                orderdto.ImagesSrc = model.ImagesSrc;
                orderdto.AdvertisementType = model.AdvertisementType;
                //orderdto.EndTime = model.EndTime;
                orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;

                var advertisementProductList = new List<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementProductRequestDto>();
                if (!String.IsNullOrEmpty(model.AdvertisementProduct))
                {
                    var products = model.AdvertisementProduct.Split(',');
                    foreach (var product in products)
                    {
                        var advertisementProduct = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest.WAdvertisementProductRequestDto()
                        {
                            WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                            AdvertisementID = model.AdvertisementID,
                            CreateTime = DateTime.Now,
                            CreateUserID = UserIdentity.UserId,
                            CreateUserName = UserIdentity.UserName,
                            ProductID = int.Parse(product)
                        };
                        advertisementProductList.Add(advertisementProduct);
                    }
                }

                var serviceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementAddOrEditRequest()
                {
                    order = orderdto,
                    advertisementProduct = advertisementProductList,
                    Flag = flag
                });

                if (resp.Flag == 0)
                {
                    //写操作日志
                    string action = string.Empty;
                    if (flag == "Edit")
                    {
                        action = ConstDefinition.XSOperatorActionEdit;
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
                          action, string.Format("{0}橱窗推荐[{1}]", action, orderdto.AdvertisementID));
                    }
                    else
                    {
                        action = ConstDefinition.XSOperatorActionAdd;
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
                          action, string.Format("{0}橱窗推荐[{1}]", action, resp.Data));
                    }

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = orderdto.ToJsonString()
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

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="buyids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520411, 52041103)]
        [ValidateInput(false)]
        public ActionResult DeleteWAdvertisement(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    //ids = ids.Substring(0, ids.Length - 1);
                    int rows = new WadvertisementModel().DeleteWadvertisement(ids, WorkContext.CurrentWarehouse.Parent.WarehouseId);
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
        /// 获取商品信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetProductList(ProductListQuery cpm)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsSaleListGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKU = (cpm.searchType == "SKU") ? cpm.searchKey : null,
                    SKULikeSearch = (cpm.searchType == "SKU") ? true : false,
                    ProductName = (cpm.searchType == "ProductName") ? cpm.searchKey : null,
                    BarCode = (cpm.searchType == "BarCode") ? cpm.searchKey : null,                   
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort,
                });
                if (resp != null && resp.Flag == 0)
                {
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
        /// 获取橱窗商品信息
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        public ActionResult GetWadvertisementProductList(WadvertisementProductListQuery cpm)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementProductGetListRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdvertisementID = int.Parse(cpm.AdvertisementID),
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort
                });
                if (resp != null && resp.Flag == 0)
                {
                    var wadvertisementProducts = new List<WadvertisementProductModel>();
                    var wadvertisementProductModel = new WadvertisementProductModel();
                    List<int> pidList = new List<int>();
                    foreach (var product in resp.Data.ItemList)
                    {
                        pidList.Add(product.ProductID);//商品ID
                    }

                    var resp2 = WorkContext.CreateProductSdkClient().Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsSaleListGetRequest()
                    {
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        ProductIds = pidList,
                        PageIndex = 1,
                        PageSize = 10000,
                        SortBy = cpm.sort,
                    });
                    if (resp2.Flag == 0)
                    {
                        if (resp2.Data != null && resp2.Data.ItemList.Count > 0)
                        {
                            foreach (var item in resp2.Data.ItemList)
                            {
                                WadvertisementProductModel model = AutoMapperHelper.MapTo<WadvertisementProductModel>(item);
                                wadvertisementProducts.Add(model);
                            }
                        }
                        if (wadvertisementProducts.Count > 0)
                        {
                            var obj = new { total = wadvertisementProducts.Count, rows = wadvertisementProducts };
                            result = obj.ToJsonString();
                        }
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_EXCEPTION,
                            Info = string.Format("出现异常：{0}",resp2.Info)
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
    }
}
