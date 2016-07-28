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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ValidateInput(false)]
    public class SaleBackPreController : BaseController
    {
        #region 视图
        /// <summary>
        /// 销售退货订单列表视图
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleBackPreList()
        {
            return View();
        }

        /// <summary>
        /// 销售退货订单新增或编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleBackPreAddOrEditNew()
        {
            return View();
        }

        /// <summary>
        /// 销售退货订单 选择门店视图
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectShop()
        {
            return View();
        }

        /// <summary>
        /// 销售退货订单 选择配送商品
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchSaleProduct()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="backids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520317, 52031703)]
        public ActionResult DeleteSaleBackPre(string backids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(backids) && backids.Length > 1)
                {
                    var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreDelRequest()
                    {
                        BackIDs = backids,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });
                    if (resp != null && resp.Flag == 0)
                    {
                        //写操作日志
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3G,
                                               ConstDefinition.XSOperatorActionDel, string.Format("{0}销售退货单号[{1}]", ConstDefinition.XSOperatorActionDel, backids));

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
        [AuthorizeButtonFiter(520317, new int[] { 52031704, 52031705, 52031706 })]
        public ActionResult SaleBackPreChangeStatus(string backids, int status)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(backids) && backids.Length > 1)
                {
                    var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreChangeStatusRequest()
                    {
                        BackIDs = backids,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        Status = status,
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
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3G,
                                              action, string.Format("{0}销售退货单号[{1}]", action, backids));

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
        /// 导出数据到Excel
        /// </summary>
        /// <returns>文件流</returns>
        [AuthorizeButtonFiter(520317, 52031707)]
        public ActionResult DataExport(string Backid)
        {
            IList<SaleBackPreDetailsModel> detailsmodel = new List<SaleBackPreDetailsModel>();
            var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreGetModelRequest()
            {
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BackID = Backid,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            if (resp != null && resp.Flag == 0)
            {
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderSaleBackPreGetModelResp.SaleBackPreDetails, SaleBackPreDetailsModel>(resp.Data.orderdetails);
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
                temp.BackQtystr = item.BackQty.ToString("0.00");
                temp.BackPricestr = item.BackPrice.ToString("0.0000");
                temp.ShopAddPercstr = item.ShopAddPerc.Value.ToString("0.000");
                temp.BackPackingQtystr = Convert.ToDecimal(item.BackPackingQty).ToString("0.00");
                temp.UnitQtystr = item.UnitQty.ToString("0.00");
                temp.SubAmtstr = (item.SubAmt + item.SubAddAmt.Value).ToString("0.0000");
                temp.RowNum = count;
            }
            int maxRows = 1000;
            string fileName = "销售退货单_" + Backid + ".xls";  // DateTime.Now.ToString("yyyyMMddHHmmssfff")
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
        /// 根据BackID获取销售退货订单信息
        /// </summary>
        /// <param name="buyid"></param>
        /// <returns></returns>
        public ActionResult GetSaleBackInfo(string backid)
        {
            SaleBackPreModel model = new SaleBackPreModel();
            //获取数据，绑定
            var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreGetModelRequest()
            {
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BackID = backid
            });
            if (resp != null && resp.Flag == 0)
            {
                model = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<SaleBackPreModel>(resp.Data.order);
                IList<SaleBackPreDetailsModel> detailsmodel = new List<SaleBackPreDetailsModel>();
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderSaleBackPreGetModelResp.SaleBackPreDetails, SaleBackPreDetailsModel>(resp.Data.orderdetails);

                var PIDList = from o in detailsmodel select o.ProductId;
                var respTemp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsSaleListGetRequest()
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    WID = resp.Data.order.WID,
                    ProductIds = PIDList.ToList()
                });
                foreach (var item in detailsmodel)
                {
                    item.SubAmt = item.SubAmt + item.SubAddAmt;
                    if (respTemp != null && respTemp.Data != null)
                    {
                        var tempDetails = respTemp.Data.ItemList.FirstOrDefault(i => i.ProductId == item.ProductId);
                        if (tempDetails != null)
                        {
                            item.MaxSalePrice = tempDetails.SalePrice;
                        }
                    }
                }
                model.Backdetails = detailsmodel;
            }
            return Content(model.ToJsonString());
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetSaleBackPreList(SaleBackSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new SaleBackPreModel().GetSaleBackPrePageData(searchModel);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520317, new int[] { 52031701, 52031702 })]
        public ActionResult SaleBackPreAddOrEditeNewHandle(string jsonData, string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var order = Frxs.Platform.Utility.Json.JsonHelper.FromJson<SaleOrder>(jsonData);
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<SaleOrderDetails>(jsonDetails);

                var part = new OrderPartsServer();
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;


                FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreRequestDto orderdto = new FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreRequestDto();
                if (string.IsNullOrEmpty(order.BackID))
                {
                    flag = "Add";
                    string backid = new SaleBackPreModel().GetSaleBackID();
                    if (string.IsNullOrEmpty(backid))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "获取销售退货单号失败"
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
                orderdto.WID = warehouseId;
                orderdto.SubWID = order.SubWID;
                orderdto.BackDate = order.BackDate;
                orderdto.ShopID = order.ShopID;
                orderdto.ShopCode = order.ShopCode;
                orderdto.ShopName = order.ShopName;
                orderdto.Remark = order.Remark;

                IList<FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsRequestDto> orderdetailsdto
                    = new List<FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsRequestDto>();

                IList<FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsExtRequestDto> orderdetailsextdto
                  = new List<FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsExtRequestDto>();

                var PIDList = from o in orderdetails select o.ProductId;

                //1.调用接口，获取网仓商品信息 （包含限购）
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

                decimal TotalAmt = 0.00m;           //总金额
                decimal TotalBasePoint = 0.00m;     //总积分
                decimal TotalBackQty = 0.00m;       //总数量
                decimal TotalAddAmt = 0.00m;        //门店合计提点金额
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
                    FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsRequestDto temp
                        = new FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsRequestDto();
                    temp.WID = warehouseId;
                    temp.BackID = orderdto.BackID;
                    temp.ProductName = model.ProductName;
                    temp.BarCode = model.BarCode;
                    temp.BackUnit = model.BackUnit;
                    temp.BackPackingQty = model.BackPackingQty;
                    temp.BackQty = model.BackQty;
                    temp.BackPrice = model.BackPrice;
                    temp.UnitQty = (model.BackPackingQty * model.BackQty);       //总数量  计算出来
                    temp.SubAmt = (model.BackPrice * model.BackQty);
                    temp.ProductId = model.ProductId;
                    temp.SKU = model.SKU;
                    temp.Unit = model.Unit;
                    temp.UnitPrice = model.UnitPrice;
                    temp.BasePoint = model.BasePoint;
                    temp.SubBasePoint = temp.SubAmt * temp.BasePoint;

                    temp.ShopAddPerc = model.ShopAddPerc;
                    temp.VendorPerc1 = model.VendorPerc1;
                    temp.VendorPerc2 = model.VendorPerc2;
                    temp.SubAddAmt = model.ShopAddPerc * temp.SubAmt;
                    temp.SubVendor1Amt = model.VendorPerc1 * temp.SubAmt;
                    temp.SubVendor2Amt = model.VendorPerc2 * temp.SubAmt;
                    temp.Remark = model.Remark;

                    temp.ProductImageUrl200 = wProduct.ImageUrl200x200;          //从网仓商品中取得
                    temp.ProductImageUrl400 = wProduct.ImageUrl400x400;          //从网仓商品中取得
                    orderdetailsdto.Add(temp);

                    TotalBasePoint = TotalBasePoint + temp.SubBasePoint;
                    TotalAmt = TotalAmt + temp.SubAmt;
                    TotalBackQty = TotalBackQty + model.BackQty * model.BackPackingQty;
                    TotalAddAmt = TotalAddAmt + temp.SubAddAmt;

                    FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsExtRequestDto detailExt = new FrxsErpOrderSaleBackPreAddOrEditRequest.SaleBackPreDetailsExtRequestDto();
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

                orderdto.TotalBackAmt = TotalAmt;
                orderdto.TotalBasePoint = TotalBasePoint;
                orderdto.TotalBackQty = TotalBackQty;
                orderdto.TotalAddAmt = TotalAddAmt;
                orderdto.PayAmount = TotalAmt + TotalAddAmt;

                //调用接口，执行新增或编辑方法
                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderserviceCenter.Execute(new FrxsErpOrderSaleBackPreAddOrEditRequest()
                {
                    order = orderdto,
                    orderdetails = orderdetailsdto,
                    orderdetailsext = orderdetailsextdto,
                    Flag = flag,
                    WarehouseId = warehouseId,
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
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3G,
                          action, string.Format("{0}销售退货单号[{1}]", action, orderdto.BackID));

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
    public class SaleOrder
    {
        public string BackID { get; set; }
        public DateTime BackDate { get; set; }
        public int SubWID { get; set; }
        public int ShopID { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        //public int BuyEmpID { get; set; }
        //public string BuyEmpName { get; set; }
        //public decimal TotalOrderAmt { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 退货订单 详情模型
    /// </summary>
    public class SaleOrderDetails
    {
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string BackUnit { get; set; }
        public decimal BackQty { get; set; }
        public decimal BackPackingQty { get; set; }
        public decimal BackPrice { get; set; }
        //public decimal SubAmt { get; set; }

        public decimal UnitPrice { get; set; }
        public string Unit { get; set; }
        public decimal ShopAddPerc { get; set; }
        public decimal BasePoint { get; set; }
        public decimal VendorPerc1 { get; set; }
        public decimal VendorPerc2 { get; set; }


        public string Remark { get; set; }
        public string BarCode { get; set; }
    }
}
