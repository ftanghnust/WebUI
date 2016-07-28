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
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.SaleFee
{

    /// <summary>
    /// 门店费用
    /// </summary>
    [ValidateInput(false)]
    public class SaleFeeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520612)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <returns>文件流</returns>
        [AuthorizeButtonFiter(520612, 52061201)]
        public ActionResult DataExport(string feeID)
        {
            IList<SaleFeeDetailsExcel> detailsmodel = new List<SaleFeeDetailsExcel>();
            var serviceCenter = WorkContext.CreateOrderSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeDetailGetModelRequest()
            {
                FeeID = feeID,
                UserId = UserIdentity.UserId,
                UserName = UserIdentity.UserName,
                WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });

            if (resp != null && resp.Flag == 0)
            {
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderSaleFeeDetailGetModelResp.SaleFeeDetails, SaleFeeDetailsExcel>(resp.Data.saleFeePreDetailsList);

                foreach (SaleFeeDetailsExcel modelobj in detailsmodel)
                {
                    //modelobj.FeeAmt = modelobj.FeeAmt.Trim().PadRight(modelobj.FeeAmt.Length+1, '0'); 
                    modelobj.FeeAmt = (decimal.Parse(modelobj.FeeAmt).ToString("#0.0000"));//修复bug 4044 费用4位小数格式问题。
                }

            }

            int maxRows = 1000;
            string fileName = "门店费用_" + feeID + ".xls";
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
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetSaleFeeList(SaleFeeQuery cpm)
        {

            string jsonStr = "[]";
            try
            {
                jsonStr = new SaleFeeModel().GetSaleFeePageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = "查询失败，请查询日志！" }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetSaleFeeDetailList(SaleFeeQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new SaleFeeModel().GetSaleFeeDetailData(cpm.FeeID, UserIdentity.UserId, UserIdentity.UserName);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = "查询失败，请查询日志！" }.ToJsonString();
            }

            return Content(jsonStr);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520612, 52061203)]
        public ActionResult DeleteSaleFee(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) && ids.Length > 1)
                {
                    //  ids = ids.Substring(0, ids.Length - 1);
                    int rows = new SaleFeeModel().DeleteSaleFee(ids, UserIdentity.UserId, UserIdentity.UserName);

                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6B,
                                               ConstDefinition.XSOperatorActionDel, string.Format("{0}[删除门店费用，单号：{1}]", ConstDefinition.XSOperatorActionDel, ids));
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
                            Info = string.Format("删除门店费用失败，请检查刷新当前单据状态！", rows)
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
                    Info = string.Format("删除失败，请检查日志！")
                }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 批量状态改变
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520612, new int[] { 52061204, 52061206, 52061205 })]
        public ActionResult SaleFeeChangeStatus(string ids, int status)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) && ids.Length >= 1)
                {
                    int rows = new SaleFeeModel().SaleFeeChangeStatus(ids, status, UserIdentity.UserId, UserIdentity.UserName);

                    if (rows > 0)
                    {
                        IDictionary<string, object> resultObj = new Dictionary<string, object>();
                        resultObj.Add("UserName", UserIdentity.UserName);
                        resultObj.Add("Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm"));

                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = string.Format("成功改变{0}条数据状态", rows),
                            Data = resultObj
                        }.ToJsonString();

                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6B,
                                                   ConstDefinition.XSOperatorActionEdit, string.Format("{0}[更新门店费用状态，单号：{1}，更新状态为{2}]", ConstDefinition.XSOperatorActionDel, ids, status));
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("修改门店费用状态失败，请检查相应单据状态不是正确！")
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
                    Info = string.Format("修改状态失败，请检查日志！")
                }.ToJsonString();
            }
            return Content(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="jsonDetails"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520612, new int[] { 52061201, 52061202 })]
        public ActionResult SaleFeeHandle(string jsonData, string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var model = Frxs.Platform.Utility.Json.JsonHelper.FromJson<SaleFeeOpertion>(jsonData);
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<SaleFeeDetailsOpertion>(jsonDetails);

                Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeRequestDto();
                if (string.IsNullOrEmpty(model.FeeID))
                {
                    flag = "Add";
                    orderdto.FeeID = new SaleFeeModel().GetFeeID();
                    orderdto.CreateTime = DateTime.Now;
                    orderdto.CreateUserID = UserIdentity.UserId;
                    orderdto.CreateUserName = UserIdentity.UserName;
                }
                else
                {
                    flag = "Edit";
                    orderdto.FeeID = model.FeeID;

                }
                var serviceCenter = WorkContext.CreateProductSdkClient();

                orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                orderdto.WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
                orderdto.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                WarehouseIdentity subwareHouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(o => o.WarehouseId.ToString() == model.SubWID);
                orderdto.SubWID = subwareHouse.WarehouseId;
                orderdto.SubWCode = subwareHouse.WarehouseCode;
                orderdto.SubWName = subwareHouse.WarehouseName;

                orderdto.ModifyTime = DateTime.Now;
                orderdto.ModifyUserID = UserIdentity.UserId;
                orderdto.ModifyUserName = UserIdentity.UserName;
                orderdto.FeeDate = DateTime.Parse(model.FeeDate);
                orderdto.Status = 0;
                orderdto.Remark = model.Remark;


                IList<FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto> orderdetailsdto
                = new List<FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto>();

                double totalOrderAmt = 0.0;    //总金额 
                int i = 1;
                foreach (var obj in orderdetails)
                {
                    FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto temp
                        = new FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto();

                    temp.ID = obj.ID;
                    temp.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                    temp.FeeAmt = obj.FeeAmt;
                    temp.FeeCode = int.Parse(obj.FeeCode);
                    temp.FeeID = orderdto.FeeID;
                    temp.FeeName = obj.FeeName;
                    temp.ModifyTime = DateTime.Now;
                    temp.ModifyUserID = UserIdentity.UserId;
                    temp.ModifyUserName = UserIdentity.UserName;
                    temp.OrderId = obj.OrderId;
                    if (obj.Reason == "null")
                    {

                        temp.Reason = "";
                    }
                    else
                    {
                        temp.Reason = obj.Reason;
                    }
                    temp.ShopCode = obj.ShopCode;
                    temp.ShopName = obj.ShopName;
                    temp.ShopID = int.Parse(obj.ShopID);
                    temp.SerialNumber = i;
                    temp.SettleTime = DateTime.Now;

                    orderdetailsdto.Add(temp);
                    totalOrderAmt = totalOrderAmt + temp.FeeAmt;
                    i++;
                }

                orderdto.TotalFeeAmt = totalOrderAmt;

                var orderServiceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeAddOrEditRequest()
                {

                    saleFee = orderdto,
                    orderdetailsList = orderdetailsdto,
                    Flag = flag,
                    UserId = UserIdentity.UserId,
                    UserName = UserIdentity.UserName,
                    WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp.Flag == 0)
                {

                    IDictionary<string, object> resultObj = new Dictionary<string, object>();
                    resultObj.Add("FeeID", orderdto.FeeID);
                    resultObj.Add("UserName", UserIdentity.UserName);
                    resultObj.Add("Time", DateTime.Now.ToString("yyyy/MM/dd hh:mm"));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = resultObj

                    }.ToJsonString();

                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6B,
                        flag == "Add" ? ConstDefinition.XSOperatorActionAdd : ConstDefinition.XSOperatorActionEdit, string.Format("{0}[更新门店费用状态，单号：{1}]", flag == "Add" ? ConstDefinition.XSOperatorActionAdd : ConstDefinition.XSOperatorActionEdit, orderdto.FeeID));
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
                    Info = string.Format("操作门店费用失败，请检查日志！")
                }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonDetails"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ImportSaleFeeHandle(string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<SaleFeeDetailsOpertion>(jsonDetails);

                Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeRequestDto();
                if (orderdetails != null && orderdetails.Count > 0)
                {
                    flag = "Add";
                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var respWhInfo = serviceCenter.Execute(new FrxsErpProductWarehouseGetSubWNameRequest()
                    {
                        WID = CurrentWarehouse.Parent.WarehouseId

                    });

                    orderdto.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                    orderdto.WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
                    orderdto.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                    WarehouseIdentity subwareHouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(o => o.WarehouseId == WorkContext.CurrentWarehouse.WarehouseId);
                    if (subwareHouse != null)
                    {

                        orderdto.SubWID = subwareHouse.WarehouseId;
                        orderdto.SubWCode = subwareHouse.WarehouseCode;
                        orderdto.SubWName = subwareHouse.WarehouseName;
                    }
                    else
                    {
                        orderdto.SubWID = WorkContext.CurrentWarehouse.WarehouseId;
                        orderdto.SubWCode = WorkContext.CurrentWarehouse.WarehouseCode;
                        orderdto.SubWName = WorkContext.CurrentWarehouse.WarehouseName;
                    }
                    orderdto.FeeID = new SaleFeeModel().GetFeeID();
                    orderdto.CreateTime = DateTime.Now;
                    orderdto.CreateUserID = UserIdentity.UserId;
                    orderdto.CreateUserName = UserIdentity.UserName;
                    orderdto.ModifyTime = DateTime.Now;
                    orderdto.ModifyUserID = UserIdentity.UserId;
                    orderdto.ModifyUserName = UserIdentity.UserName;
                    orderdto.FeeDate = DateTime.Now.ToLocalTime();
                    orderdto.Status = 0;
                    orderdto.Remark = "导入操作";


                    IList<FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto> orderdetailsdto
                    = new List<FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto>();

                    double totalOrderAmt = 0.0;    //总金额 
                    int i = 1;
                    foreach (var obj in orderdetails)
                    {
                        FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto temp
                            = new FrxsErpOrderSaleFeeAddOrEditRequest.SaleFeeDetailsRequestDto();

                        temp.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                        temp.FeeAmt = obj.FeeAmt;
                        temp.FeeCode = int.Parse(obj.FeeCode);
                        temp.FeeID = orderdto.FeeID;
                        temp.FeeName = obj.FeeName;
                        temp.ModifyTime = DateTime.Now;
                        temp.ModifyUserID = UserIdentity.UserId;
                        temp.ModifyUserName = UserIdentity.UserName;
                        temp.OrderId = obj.OrderId;
                        temp.Reason = obj.Reason;
                        temp.ShopCode = obj.ShopCode;
                        temp.ShopName = obj.ShopName;
                        temp.ShopID = int.Parse(obj.ShopID);
                        temp.SerialNumber = i;
                        temp.SettleTime = DateTime.Now;

                        orderdetailsdto.Add(temp);
                        totalOrderAmt = totalOrderAmt + temp.FeeAmt;
                        i++;
                    }

                    orderdto.TotalFeeAmt = totalOrderAmt;

                    var orderServiceCenter = WorkContext.CreateOrderSdkClient();
                    var resp = orderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeAddOrEditRequest()
                    {
                        saleFee = orderdto,
                        orderdetailsList = orderdetailsdto,
                        Flag = flag,
                        UserId = UserIdentity.UserId,
                        UserName = UserIdentity.UserName,
                        WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    });

                    if (resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Data = orderdto.FeeID,
                            Info = "操作成功"

                        }.ToJsonString();

                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6B,
                      ConstDefinition.XSOperatorActionAdd, string.Format("{0}[导入门店费用状态，单号：{1}]", ConstDefinition.XSOperatorActionAdd, orderdto.FeeID));

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
        public ActionResult SaleFeeList()
        {

            return View();

        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleFeeAddOrEditView()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleFeeAddOrEdit(string id)
        {
            SaleFeeModel model = new SaleFeeModel();

            SaleFeeOpertion resultModel = model.GetSaleFeeData(id, UserIdentity.UserId, UserIdentity.UserName);

            resultModel.PageTitle = "查看消息";

            JsonResult jsonResult = Json(resultModel, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        /// <summary>
        /// 查询子仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSubWhinfo()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var respWhInfo = serviceCenter.Execute(new FrxsErpProductWarehouseGetSubWNameRequest()
            {
                WID = CurrentWarehouse.Parent.WarehouseId
            });

            JsonResult jsonResult = Json(respWhInfo.Data, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        /// <summary>
        ///  获取门店
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        public ActionResult GetShopModelPageData(SaleFeeQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new SaleFeeModel().GetShopModelPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }


        /// <summary>
        /// 采购订单 选择商品资料
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchShopInfoView()
        {
            return View();
        }



        #endregion


        #region 导入

        public ActionResult ImportSaleFeeView()
        {
            return View();
        }

        /// <summary>
        ///  获取门店
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        public ActionResult GetImportData(SaleFeeQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                string filePath = Server.MapPath(cpm.folder) + string.Format("/{0}.xls", cpm.ImportGuid);


                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "ShopCode";

                dt.Columns[2].ColumnName = "FeeName";
                dt.Columns[3].ColumnName = "FeeAmt";
                dt.Columns[4].ColumnName = "Reason";
                #region 扩展门店及项目费用字段

                dt.Columns.Add("FeeCode");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("ShopName");

                #endregion


                Dictionary<string, FrxsErpProductShopTableListResp.Shop> shopList = new Dictionary<string, FrxsErpProductShopTableListResp.Shop>();
                Dictionary<string, FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> feeList =
                    new Dictionary<string, FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData>();

                #region 费用项

                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductSysDictDetailGetListRequest()
                {
                    dictCode = "SaleFeeCode"
                });
                if (resp != null && resp.Flag == 0)
                {
                    IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> feeobj = resp.Data;
                    foreach (FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData feeModel in feeobj)
                    {
                        feeList.Add(feeModel.DictLabel, feeModel);
                    }
                }
                #endregion

                #region 数据验证
                SaleFeeModel saleFeeModel = new SaleFeeModel();
                string shopCode = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (feeList.ContainsKey(dt.Rows[i]["FeeName"].ToString().Trim()))
                    {
                        dt.Rows[i]["FeeCode"] = feeList[dt.Rows[i]["FeeName"].ToString().Trim()].DictValue;

                    }
                    else
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行" + string.Format("无法匹配项目名称【{0}】，请检查是否正确", dt.Rows[i]["FeeName"].ToString())
                        }.ToJsonString();
                        return Content(jsonStr);
                    }

                    shopCode = dt.Rows[i]["ShopCode"].ToString().Trim();

                    if (string.IsNullOrEmpty(shopCode))
                    {
                        jsonStr = new ResultData
                      {
                          Flag = ConstDefinition.FLAG_FAIL,
                          Info = "Excel第 " + (i + 1) + " 行" + string.Format("无法匹配门店编号【{0}】，请检查是否正确", dt.Rows[i]["ShopCode"].ToString())
                      }.ToJsonString();
                        return Content(jsonStr);
                    }
                    else
                    {
                        if (shopList.ContainsKey(shopCode))
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("门店编号【{0}】有重复，请检查是否正确", dt.Rows[i]["ShopCode"].ToString())
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        else
                        {
                            FrxsErpProductShopTableListResp.Shop shop = saleFeeModel.GetShopModel(shopCode);
                            if (shop != null && shopCode==shop.ShopCode)
                            {
                                dt.Rows[i]["ShopID"] = shop.ShopID;
                                dt.Rows[i]["ShopName"] = shop.ShopName;
                                shopList.Add(shop.ShopCode, shop);
                            }
                            else
                            {
                                jsonStr = new ResultData
                               {
                                   Flag = ConstDefinition.FLAG_FAIL,
                                   Info = "Excel第 " + (i + 1) + " 行" + string.Format("无法匹配门店编号【{0}】，请检查是否正确", dt.Rows[i]["ShopCode"].ToString())
                               }.ToJsonString();
                                return Content(jsonStr);
                            }

                        }
                    }
                }
                #endregion
              
                var obj = new { total = dt.Rows.Count, rows = dt.ToJson() };
                jsonStr = obj.ToJsonString();
                
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "导入失败：错误：" + ex.Message
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
        // [AcceptVerbs(HttpVerbs.Post)]
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
