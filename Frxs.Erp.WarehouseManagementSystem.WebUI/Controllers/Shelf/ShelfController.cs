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
using Frxs.ServiceCenter.SSO.Client;
using System.Data;
using Frxs.Platform.Utility.Excel;
using System.Collections;
using System.IO;
using System.Text;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ShelfController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 导入 
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520116, 52011601)]
        public ActionResult ImportShelfDetail()
        {
            return View();
        }


        [AuthorizeMenuFilter(520116)]
        public ActionResult EasyuiShelfList()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520116, 52011601)]
        public ActionResult ShelfAddOrEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShelfModel model = new ShelfModel().GetShelfData(id);
                model.PageTitle = "修改货位";
                model.WName = WorkContext.CurrentWarehouse.WarehouseName;
                model.BindShelfAreaList();
                model.BindShelfTypeList();
                model.BindStatusList();
                return View(model);
            }
            else
            {
                ShelfModel model = new ShelfModel { PageTitle = "新增货位" };
                model.WName = WorkContext.CurrentWarehouse.WarehouseName;
                model.BindShelfAreaList();
                model.BindShelfTypeList();
                model.BindStatusList();
                return View(model);
            }
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520116, 52011602)]
        public ActionResult ShelfEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShelfModel model = new ShelfModel().GetShelfData(id);
                model.PageTitle = "修改货位";
                model.WName = WorkContext.CurrentWarehouse.WarehouseName;
                model.BindShelfAreaList();
                model.BindShelfTypeList();
                model.BindStatusList();
                return View(model);
            }
            else
            {
                ShelfModel model = new ShelfModel { PageTitle = "新增货位" };
                model.WName = WorkContext.CurrentWarehouse.WarehouseName;
                model.BindShelfAreaList();
                model.BindShelfTypeList();
                model.BindStatusList();
                return View(model);
            }
        }

        [ValidateInput(false)]
        public ActionResult ShelfHandle(ShelfModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfSaveRequest()
                    {
                        ShelfID = model.ShelfID,
                        ShelfAreaID = model.ShelfAreaID,
                        ShelfType = model.ShelfType,
                        ShelfCode = model.ShelfCode,
                        Status = model.Status,
                        Remark = model.Remark,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    });

                    if (resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功"
                        }.ToJsonString();

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1F, ConstDefinition.XSOperatorActionAdd, "保存" + model.ShelfCode + "货位！");
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
                    //服务端错误信息
                    string errorMsg = base.GetValidateErrorMsg();
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = string.Format("校验失败，失败原因为：{0}", errorMsg)
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

        #region 批量删除
        [AuthorizeButtonFiter(520116, 52011603)]
        public ActionResult DeleteShelf(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    result = new ShelfModel().DeleteShelf(ids).ToJson();
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1F, ConstDefinition.XSOperatorActionDel, "删除" + ids + "货位！");
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
        #endregion

        #region 获取分页列表
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetShelfList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ShelfModel().GetShelfPageData(cpm.page, cpm.rows, cpm.sort);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion

        /// <summary>
        ///  获EXCEL里的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImportData(string fileName, string folderPath)
        {
            string jsonStr = "[]";
            try
            {
                if (fileName.IsNullOrEmpty())
                {
                    return Content(jsonStr);
                }
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;
                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);


                var list = new List<ShelfModel>();

                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaTableListRequest()
                {
                    PageIndex = 1,
                    PageSize = 10000,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp == null || resp.Data == null)
                {
                    jsonStr = new { info = "货区获取失败，请联系管理员！" }.ToJsonString();
                    return Content(jsonStr);
                }
                Hashtable ht = new Hashtable();



                foreach (DataRow row in dt.Rows)
                {
                    ShelfModel item = new ShelfModel();



                    if ((Validator.IsNumeric(row[0].ToString()) || Validator.IsNumeric(row[1].ToString())))
                    {
                        if (!ht.ContainsKey(row[0].ToString() + row[1].ToString()))//ShelfCode
                        {
                            var shelfArea = resp.Data.ItemList.FirstOrDefault(o => o.ShelfAreaCode.ToString() == row[1].ToString());

                            if (shelfArea != null)
                            {
                                ht.Add(row[0].ToString() + row[1].ToString(), row[0]);
                                item.ShelfCode = row[0].ToString();
                                item.ShelfAreaID = shelfArea.ShelfAreaID;
                                item.ShelfAreaName = shelfArea.ShelfAreaName;

                                list.Add(item);
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
        /// 上传文件
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="guid"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        // [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFile(HttpPostedFileBase fileData, string folder)
        {
            if (fileData != null)
            {
                try
                {
                    if (!fileData.FileName.Contains(".xls"))
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

        #region 验证
        public ActionResult isValidate(string fileName, string folderPath)
        {
            string result = string.Empty;
            try
            {
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;

                try
                {
                    DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);


                    if (dt == null || dt.Rows.Count < 1)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "表格无数据，请重新导入！"
                        }.ToJsonString();
                    }
                    else if (dt.Columns.Count < 2)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "文件上传出错,请检查文件类型及内容格式！"
                        }.ToJsonString();
                    }
                    else if (dt.Columns[0].ColumnName != "货位编号" || dt.Columns[1].ColumnName != "所属货区编码")
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "文件上传出错,请检查文件类型及内容格式！"
                        }.ToJsonString();
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "上传成功！"
                        }.ToJsonString();
                    }
                }
                catch
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "文件上传出错,请检查文件类型及内容格式！"
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
        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ImportShelfHandle(string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {
                var list = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<ImportShelfModel>(jsonDetails);

                if (list == null || list.Count == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "无导入数据！"
                    }.ToJsonString();

                    return Content(result);
                }

                List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfListSaveRequest.Shelf> shelfList = new List<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfListSaveRequest.Shelf>();

                foreach (var item in list)
                {
                    var model = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfListSaveRequest.Shelf();
                    model.ShelfAreaID = item.ShelfAreaID;
                    model.ShelfType = "0";
                    model.ShelfCode = item.ShelfCode;
                    model.Status = "0";
                    model.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                    shelfList.Add(model);
                }

                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfListSaveRequest()
                {
                    ShelfList = shelfList,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp != null && resp.Flag == 0)
                {
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1F, ConstDefinition.XSOperatorActionAdd, "导入" + shelfList.Count + "个货位！");

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("导入成功！")
                    }.ToJsonString();
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp != null ? resp.Info : "调用导入接口异常FrxsErpProductShelfListSaveRequest"
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

        public class ImportShelfModel
        {
            /// <summary>
            /// 货位编号(同一个仓库不能重复)
            /// </summary> 
            public string ShelfCode { get; set; }

            /// <summary>
            /// 所属货区ID(ShelfArea.ShelfAreaID)
            /// </summary>
            public int ShelfAreaID { get; set; }
        }
    }
}
