using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Request;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Excel;
using System.Collections;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>

    [ValidateInput(false)]
    public class WarehouseMessageController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520412)]
        public ActionResult Index()
        {
            return View();
        }

        #region  原有访求方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520412, new int[] { 52041201, 52041202 })]
        public ActionResult WarehouseMessageHandle(string jsonData, string jsonDetails)
        {
            string flag = string.Empty;
            string result = string.Empty;
            try
            {

                var model = Frxs.Platform.Utility.Json.JsonHelper.FromJson<WarehouseMessageOpertion>(jsonData);
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<WarehouseMessageShopsQuery>(jsonDetails);

                Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageAddOrEditRequest.WarehouseMessageRequestDto orderdto
                    = new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageAddOrEditRequest.WarehouseMessageRequestDto();
                if (model.Status == 0)
                {
                    if (model.ID == 0)
                    {
                        flag = "Add";
                    }
                    else
                    {
                        flag = "Edit";
                        orderdto.ID = model.ID.Value;
                    }

                    if (int.Parse(DateTime.Parse(model.BeginTime).ToString("yyyyMMdd"))
                        < int.Parse(DateTime.Now.ToString("yyyyMMdd") ) )
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "发布开始时间需大于当前时间！"

                        }.ToJsonString();
                        return Content(result);
                    }
                    string title=Utils.NoHtml(model.Title).Replace('<',' ').Replace('>',' ') .Trim();
                    if (string.IsNullOrEmpty(title))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "标题不能为空或含有特殊字符！"

                        }.ToJsonString();
                        return Content(result);
                    }

                    if (string.IsNullOrEmpty(model.Message.Replace("&nbsp;","").Trim()))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "内容不能为空！"

                        }.ToJsonString();
                        return Content(result);
                    }

                    if (DateTime.Parse(model.BeginTime) >= DateTime.Parse(model.EndTime))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "发布开始时间需小于结束时间！"

                        }.ToJsonString();
                        return Content(result);
                    }

                    orderdto.BeginTime = DateTime.Parse(model.BeginTime);
                    orderdto.EndTime = DateTime.Parse(model.EndTime);
                    orderdto.IsFirst = model.IsFirst.Value;
                    orderdto.Message = model.Message;
                    orderdto.MessageType = int.Parse(model.MessageType);
                    orderdto.RangType = model.RangType.Value;
                    orderdto.Status = 0;
                    orderdto.Title = title;
                    orderdto.WID = CurrentWarehouse.Parent.WarehouseId;

                    #region 门店群组操作

                    IList<FrxsErpPromotionWarehouseMessageAddOrEditRequest.WarehouseMessageShopsRequestDto> orderdetailsdto
             = new List<FrxsErpPromotionWarehouseMessageAddOrEditRequest.WarehouseMessageShopsRequestDto>();


                    foreach (var obj in orderdetails)
                    {
                        FrxsErpPromotionWarehouseMessageAddOrEditRequest.WarehouseMessageShopsRequestDto temp
                            = new FrxsErpPromotionWarehouseMessageAddOrEditRequest.WarehouseMessageShopsRequestDto();

                        temp.ID = obj.ID;
                        temp.CreateTime = DateTime.Now.ToLocalTime();
                        temp.CreateUserID = UserIdentity.UserId;
                        temp.CreateUserName = UserIdentity.UserName;
                        temp.GroupCode = obj.GroupCode;
                        temp.GroupID = obj.GroupID;
                        temp.GroupName = obj.GroupName;
                        temp.WarehouseMessageID = orderdto.ID == null ? 0 : orderdto.ID.Value;
                        temp.WID = obj.WID;


                        orderdetailsdto.Add(temp);
                    }


                    #endregion

                    var serviceCenter = WorkContext.CreatePromotionSdkClient();

                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageAddOrEditRequest()
                    {
                        order = orderdto,
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
                            Info = "操作成功"

                        }.ToJsonString();

                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4B,
                      ConstDefinition.XSOperatorActionAdd, string.Format("{0}[操作信息]", ConstDefinition.XSOperatorActionAdd ));
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
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "只能对未发布状态信息进行修改！"

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
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520412, 52041204)]
        public ActionResult DeleteWarehouseMessage(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) && ids.Length > 1)
                {

                    int rows = new WarehouseMessageModel().DeleteWarehouseMessage(ids, UserIdentity.UserId, UserIdentity.UserName);

                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4B,
                  ConstDefinition.XSOperatorActionDel, string.Format("{0}[删除信息]", ConstDefinition.XSOperatorActionDel));
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
                            Info = string.Format("删除信息失败，请检查刷新当前信息状态！", rows)
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
        /// <param name="buyids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520412, new int[] { 52041205, 52041201 })]
        public ActionResult WarehouseMessageChangeStatus(string ids, int status)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) && ids.Length >= 1)
                {
                    int rows = new WarehouseMessageModel().WarehouseMessageChangeStatus(ids, status, UserIdentity.UserId, UserIdentity.UserName);
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4B,
              "状态更改", string.Format("状态：{0}[状态更改]", status));

                    if (rows > 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = string.Format("成功改变{0}条数据状态", rows)
                        }.ToJsonString();
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("改变{0}条数据状态", rows)
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
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetWarehouseMessageList(WarehouseMessageQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseMessageModel().GetWarehouseMessagePageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion

        #region   Easyui 扩展请求

        /// <summary>
        /// 查询界面响应
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseMessageList()
        {

            return View();

        }

        /// <summary>
        /// 查询界面响应
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseMessageAddOrEditSearch()
        {

            return View();

        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseMessageAddOrEditView()
        {
            return View();
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseMessageAddOrEdit(int id)
        {
            WarehouseMessageModel model = new WarehouseMessageModel();
            WarehouseMessageOpertion resultModel = model.GetWarehouseMessageData(id.ToString(), UserIdentity.UserId, UserIdentity.UserName);
            resultModel.PageTitle = "查看消息";
            resultModel.BindIsFirstByBool();
            resultModel.BindStatusByBool();

            JsonResult jsonResult = Json(resultModel, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        #endregion

        #region  获取门店分组信息

        /// <summary>
        /// 获取门店分组信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderField"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetShopGroupModelPageData(WarehouseMessageShopsQuery searchQuery)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseMessageModel().GetShopGroupModelPageData(searchQuery);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        #endregion

        #region 获取已选门店分组数据

        /// <summary>
        /// 获取门店分组数据
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMessageShopGroupData(WarehouseMessageShopsQuery searchQuery)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseMessageModel().GetMessageShopGroupData(searchQuery);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion

    }
}
