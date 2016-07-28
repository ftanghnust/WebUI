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
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using System.Data;
using System.Text;
using System.IO;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.WProductAdjShelf
{
    /// <summary>
    /// 
    /// </summary>
    [ValidateInput(false)]
    public class WProductAdjShelfController : BaseController
    {

        [AuthorizeMenuFilter(520229)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <returns>文件流</returns>
        [AuthorizeButtonFiter(520229, 52022907)]
        public ActionResult DataExport(string adjID)
        {
            IList<WProductAdjShelfDetailsExcel> detailsmodel = new List<WProductAdjShelfDetailsExcel>();
          
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfGetModelRequest()
            {
                AdjId = adjID,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                UserId = UserIdentity.UserId,
                UserName = UserIdentity.UserName
            });

            var dataTable = new DataTable();
            if (resp != null && resp.Flag == 0)
            {
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductAdjShelfGetModelResp.WProductAdjShelfDetails, WProductAdjShelfDetailsExcel>(resp.Data.wProductAdjShelfDetailsList);

                dataTable.Columns.Add("商品编码");
                dataTable.Columns.Add("商品名称");
                dataTable.Columns.Add("配送单位");
                dataTable.Columns.Add("包装数");
                dataTable.Columns.Add("原货位号");
                dataTable.Columns.Add("新货位号");
                dataTable.Columns.Add("商品条码");
                dataTable.Columns.Add("备注");
                dataTable.Columns.Add("录单人员");

                foreach (var item in detailsmodel)
                {
                    var newRow = dataTable.NewRow();
                    newRow["商品编码"] = item.SKU;
                    newRow["商品名称"] = item.ProductName;
                    newRow["配送单位"] = item.Unit;
                    newRow["包装数"] = item.BigPackingQty;
                    newRow["原货位号"] = item.OldShelfCode;
                    newRow["新货位号"] = item.ShelfCode;
                    newRow["商品条码"] = item.BarCode;
                    newRow["备注"] = item.Remark;
                    newRow["录单人员"] = item.CreateUserName;
                    
                    dataTable.Rows.Add(newRow);
                }

            }

            int maxRows = 1000;
            string fileName = "货位调整单_" + adjID + ".xls";
            byte[] byteArr = NpoiExcelhelper.ExportExcel
                (
                    dataTable,
                    maxRows,
                    Server.MapPath("UploadFile"),
                    fileName
                );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }


        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
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
                    ProductName = (cpm.searchType == "ProductName") ? cpm.searchKey.Trim() : null,
                    BarCode = (cpm.searchType == "BarCode") ? cpm.searchKey : null,
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort
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
        /// 获取货位调整分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetWProductAdjShelfList(WProductAdjShelfQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WProductAdjShelfModel().GetWProductAdjShelfPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }


        /// <summary>
        /// 获取明细分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetWProductAdjShelfDetailList(WProductAdjShelfQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WProductAdjShelfModel().GetWProductAdjShelfDetailData(cpm.AdjID, UserIdentity.UserId, UserIdentity.UserName);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="buyids"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520229, 52022903)]
        public ActionResult DeleteWProductAdjShelf(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) && ids.Length > 1)
                {
                    int rows = new WProductAdjShelfModel().DeleteWProductAdjShelf(ids, UserIdentity.UserId, UserIdentity.UserName);

                    if (rows > 0)
                    {
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
                            Info = string.Format("删除货位调整失败，请检查刷新当前单据状态！", rows)
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
        /// 货位调整批量状态改变
        /// </summary>
        /// <param name="buyids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520229, new int[] { 52022904, 52022906, 52022905 })]
        public ActionResult WProductAdjShelfChangeStatus(string ids, int status)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) && ids.Length >= 1)
                {
                    //int rows = new WProductAdjShelfModel().WProductAdjShelfChangeStatus(ids, status, UserIdentity.UserId, UserIdentity.UserName);
                    var ServiceCenter = WorkContext.CreateProductSdkClient();
                    var idsAry = ids.Split(',');
                    IList<string> idsList = new List<string>();
                    foreach (var id in idsAry)
                    {
                        idsList.Add(id);
                    }
                    var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfChangeStatusRequest()
                    {
                        AdjIDs = idsList,
                        Status = status,
                        UserId = UserIdentity.UserId,
                        UserName = UserIdentity.UserName
                    });
                    StatusParam resultParam = new StatusParam();
                    resultParam.UserName = UserIdentity.UserName;
                    resultParam.Time = DateTime.Now.ToLocalTime().ToString();
                    if (resp != null && resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Data = resultParam,
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
        /// 货位调整操作保存
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520229, new int[] { 52022901, 52022902 })]
        public ActionResult WProductAdjShelfHandle(string jsonData, string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var model = Frxs.Platform.Utility.Json.JsonHelper.FromJson<WProductAdjShelfOpertion>(jsonData);
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<WProductAdjShelfDetailsOpertion>(jsonDetails);

                Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfRequestDto();
                if (string.IsNullOrEmpty(model.AdjID))
                {
                    flag = "Add";
                    orderdto.AdjID = new WProductAdjShelfModel().GetShelfAdjustID();
                }
                else
                {
                    flag = "Edit";
                    orderdto.AdjID = model.AdjID;
                }
                var serviceCenter = WorkContext.CreateProductSdkClient();

                orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;//int.Parse( m);
                orderdto.Status = 0; //默认为录单
                orderdto.Remark = model.Remark;
                orderdto.AdjType = 0;

                IList<FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto> orderdetailsdto
                = new List<FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto>();

                int i = 1;
                foreach (var obj in orderdetails)
                {
                    FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto temp
                        = new FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto();

                    temp.ID = 0;
                    temp.WID = orderdto.WID;
                    temp.AdjID = orderdto.AdjID;
                    temp.OldShelfCode = obj.OldShelfCode;

                    temp.OldShelfID = string.IsNullOrEmpty(obj.OldShelfID) || obj.OldShelfID == "null" ? 0 : int.Parse(obj.OldShelfID);
                    temp.ProductId = int.Parse(obj.ProductId);
                    temp.Remark = obj.Remark;
                    if (obj.ShelfCode.Contains("<") || obj.ShelfCode.Contains(">"))
                    {

                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "调整货位含有非法字符，请重新输入！新货位号由字母或数字组成！"
                        }.ToJsonString();
                        return Content(result);
                    }
                    temp.ShelfCode = obj.ShelfCode;
                    temp.ShelfID = 0;
                    temp.Unit = obj.Unit;
                    temp.WProductID = int.Parse(obj.WProductID);
                    orderdetailsdto.Add(temp);
                    i++;
                }

                var orderServiceCenter = WorkContext.CreateProductSdkClient();
                var resp = orderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfAddOrEditRequest()
                {
                    WProductAdjShelf = orderdto,
                    orderdetailsList = orderdetailsdto,
                    Flag = flag,
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName,
                    WareHouseId = CurrentWarehouse.Parent.WarehouseId
                });

                if (resp.Flag == 0)
                {

                    StatusParam resultParam = new StatusParam();
                    resultParam.AdjId = orderdto.AdjID;
                    resultParam.UserName = UserIdentity.UserName;
                    resultParam.Time = DateTime.Now.ToLocalTime().ToString();

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = resultParam

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
        /// 导入货位调整信息保存
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520229, new int[] { 52022901, 52022902 })]
        public ActionResult ImportWProductAdjShelfHandle(string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<WProductAdjShelfDetailsOpertion>(jsonDetails);

                Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfRequestDto();
                if (orderdetails != null && orderdetails.Count > 0)
                {
                    flag = "Add";
                    var serviceCenter = WorkContext.CreateProductSdkClient();


                    orderdto.AdjID = new WProductAdjShelfModel().GetShelfAdjustID();
                    orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                    orderdto.AdjType = 0; //todo
                    orderdto.Status = 0;
                    orderdto.Remark = "导入操作";

                    IList<FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto> orderdetailsdto
                    = new List<FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto>();

                    int i = 1;
                    foreach (var obj in orderdetails)
                    {
                        FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto temp
                            = new FrxsErpProductWProductAdjShelfAddOrEditRequest.WProductAdjShelfDetailsRequestDto();

                        temp.ID = 0;
                        temp.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                        temp.AdjID = orderdto.AdjID;
                        temp.OldShelfCode = obj.OldShelfCode;
                        temp.OldShelfID = string.IsNullOrEmpty(obj.OldShelfID) || obj.OldShelfID == "null" ? 0 : int.Parse(obj.OldShelfID);
                        temp.ProductId = int.Parse(obj.ProductId);
                        temp.Remark = obj.Remark;
                        temp.ShelfCode = obj.ShelfCode;
                        temp.ShelfID = int.Parse(obj.ShelfID);
                        temp.Unit = obj.Unit;
                        temp.WProductID = int.Parse(obj.WProductID);

                        orderdetailsdto.Add(temp);
                        i++;
                    }

                    var orderServiceCenter = WorkContext.CreateProductSdkClient();
                    var resp = orderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfAddOrEditRequest()
                    {
                        WProductAdjShelf = orderdto,
                        orderdetailsList = orderdetailsdto,
                        Flag = flag,
                        UserId = UserIdentity.UserId,
                        UserName = UserIdentity.UserName,
                        WareHouseId = CurrentWarehouse.Parent.WarehouseId
                    });

                    if (resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Data = orderdto.AdjID,
                            Info = "操作成功"

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
                        Info = "导入明细不能为空"
                    }.ToJsonString();
                }

            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("导入失败，请检查导入明细是否正确，确保无红色验证单元格（红色单元格为验证失败）！")
                }.ToJsonString();
            }
            return Content(result);
        }

        #region   Easyui 扩展请求

        /// <summary>
        /// 查询界面响应
        /// </summary>
        /// <returns></returns>
        public ActionResult WProductAdjShelfList()
        {

            return View();

        }
        /// <summary>
        /// 查询界面响应
        /// </summary>
        /// <returns></returns>
        public ActionResult WProductAdjShelfProduct()
        {

            return View();

        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult WProductAdjShelfAddOrEditView()
        {
            return View();
        }

        /// <summary>
        /// 读取单条信息
        /// </summary>
        /// <returns></returns>
        public ActionResult WProductAdjShelfAddOrEdit(string id)
        {
            WProductAdjShelfModel model = new WProductAdjShelfModel();
            WProductAdjShelfOpertion resultModel = new WProductAdjShelfOpertion();
            if (!string.IsNullOrEmpty(id) && id!="null")
            {
               resultModel = model.GetWProductAdjShelfData(id, UserIdentity.UserId, UserIdentity.UserName);
               resultModel.PageTitle = "Edit";
            }

            #region 对仓库进行处理 
            resultModel.WID = CurrentWarehouse.Parent.WarehouseId.ToString();
            resultModel.WIDName=CurrentWarehouse.Parent.WarehouseName;
            #endregion

            JsonResult jsonResult = Json(resultModel, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }


        #endregion


        #region 导入

        public ActionResult ImportWProductAdjShelfView()
        {
            return View();
        }

        /// <summary>
        ///  对上传EXCEL文件进行验证
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        public ActionResult GetImportData(WProductAdjShelfQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                string filePath = Server.MapPath(cpm.folder) + string.Format("/{0}.xls", cpm.ImportGuid);

                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "SKU";
                dt.Columns[2].ColumnName = "ShelfCode";
                dt.Columns[3].ColumnName = "Remark";
                #region 扩展门店及项目费用字段

                dt.Columns.Add("WID");
                dt.Columns.Add("WProductID");
                dt.Columns.Add("ProductId");
                dt.Columns.Add("ProductName");
                dt.Columns.Add("BarCode");
                dt.Columns.Add("Unit");
                dt.Columns.Add("BigPackingQty");
                dt.Columns.Add("OldShelfID");
                dt.Columns.Add("OldShelfCode");
                dt.Columns.Add("ShelfID");


                #endregion
              

                IList<string> prodcutSKUList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    prodcutSKUList.Add(string.Format("'{0}'", row["SKU"].ToString().Trim()));
                }

                IList<string> shelfCodeList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    shelfCodeList.Add(string.Format("'{0}'", row["ShelfCode"].ToString().Trim()));
                }

                Dictionary<string, FrxsErpProductWProductsGetByIDsExtResp.FrxsErpProductWProductsGetByIDsExtRespData> wProductsList = new Dictionary<string, FrxsErpProductWProductsGetByIDsExtResp.FrxsErpProductWProductsGetByIDsExtRespData>();
                Dictionary<string, FrxsErpProductShelfListResp.FrxsErpProductShelfListRespData> shelfList = new Dictionary<string, FrxsErpProductShelfListResp.FrxsErpProductShelfListRespData>();

                #region 商品信息

                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductWProductsGetByIDsExtRequest()
                {
                    ProductSKUs = prodcutSKUList,
                    ProductIds = null,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId

                });
                if (resp != null && resp.Flag == 0)
                {
                    foreach (FrxsErpProductWProductsGetByIDsExtResp.FrxsErpProductWProductsGetByIDsExtRespData obj in resp.Data)
                    {
                        wProductsList.Add(obj.SKU, obj);
                    }
                }
                #endregion

                #region 获取货位信息

                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter2 = WorkContext.CreateProductSdkClient();
                var resp2 = serviceCenter2.Execute(new FrxsErpProductShelfListRequest()
                {
                    ShelfCodeList = shelfCodeList,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId

                });

                if (resp2 != null && resp2.Flag == 0)
                {
                    foreach (FrxsErpProductShelfListResp.FrxsErpProductShelfListRespData obj in resp2.Data)
                    {
                        shelfList.Add(obj.ShelfCode, obj);
                    }
                }
                #endregion

                #region 数据验证
                WProductAdjShelfModel WProductAdjShelfModel = new WProductAdjShelfModel();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (shelfList.ContainsKey(dt.Rows[i]["ShelfCode"].ToString().Trim()))
                    {
                        dt.Rows[i]["ShelfID"] = shelfList[dt.Rows[i]["ShelfCode"].ToString().Trim()].ShelfID;

                    }
                    else
                    {
                        var info = string.IsNullOrEmpty(dt.Rows[i]["ShelfCode"].ToString()) ?
                            "Excel第 " + (i + 1) + " 行" + string.Format("新货位号不能为空"):
                            "Excel第 " + (i + 1) + " 行" + string.Format("无法匹配新货位号【{0}】，请检查是否正确", dt.Rows[i]["ShelfCode"].ToString());
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = info
                        }.ToJsonString();
                        return Content(jsonStr);
                    }

                    if (wProductsList.ContainsKey(dt.Rows[i]["SKU"].ToString().Trim()))
                    {
                        dt.Rows[i]["WProductID"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].WProductID;
                        dt.Rows[i]["ProductId"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].ProductId;
                        dt.Rows[i]["ProductName"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].ProductName;
                        dt.Rows[i]["BarCode"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].BarCode;
                        dt.Rows[i]["Unit"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].Unit;
                        dt.Rows[i]["BigPackingQty"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].BigPackingQty;
                        dt.Rows[i]["OldShelfID"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].ShelfID;
                        dt.Rows[i]["OldShelfCode"] = wProductsList[dt.Rows[i]["SKU"].ToString().Trim()].ShelfCode;
                    }
                    else
                    {
                        var info = string.IsNullOrEmpty(dt.Rows[i]["SKU"].ToString()) ?
                            "Excel第 " + (i + 1) + " 行" + string.Format("商品编码不能为空") :
                            "Excel第 " + (i + 1) + " 行" + string.Format("无法匹配商品编码：【{0}】，请检查是否正确", dt.Rows[i]["SKU"].ToString());
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = info
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                    dt.Rows[i]["WID"] = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                }
                #endregion

                var objstr = new { total = dt.Rows.Count, rows = dt.ToJson() };
                jsonStr = objstr.ToJsonString();
                
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "导入失败：错误："+ex.Message
                }.ToJsonString();
                return Content(jsonStr);

            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="guid"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
       //  [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult UploadExcel(HttpPostedFileBase fileData, string folder)
        {
            if (fileData != null)
            {
                try
                {
                    if (!fileData.FileName.ToLower().Contains(".xls"))
                    {
                        return Content("文件格式不对!");
                    }

                    ControllerContext.HttpContext.Request.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.Charset = "UTF-8";
                    string guid = Guid.NewGuid().ToString();
                    // 文件上传后的保存路径
                    string filePath = Server.MapPath("~" + folder);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileUrl = filePath + string.Format("/{0}.xls", guid);

                    fileData.SaveAs(fileUrl);

                    return Content(guid);

                }
                catch (Exception ex)
                {
                    return Content("");
                }
            }
            else
            {
                return Content("");
            }
        }

        #endregion
    }


}
