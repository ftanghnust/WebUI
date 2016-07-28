using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Warehouse
{
    /// <summary>
    /// 
    /// </summary>
    public class WarehouseController : BaseController
    {

        /// <summary>
        /// 显示仓库信息
        /// </summary>
        /// <returns></returns>
        [Platform.Utility.Filter.ExceptionFilterAttribute]
        [AuthorizeMenuFilter(520110)]
        public ActionResult WarehouseView()
        {
            Models.WarehouseModel model = new WarehouseModel();
            try
            {
                //当前登录用户所属的仓库（根节点，非子机构）
                int id = this.CurrentWarehouse.Parent.WarehouseId;
                model = new WarehouseModel().GetWarehouseData(id);
            }
            catch (Exception ex)
            {
                model.Remark = string.Format("出现异常，获取仓库信息失败!", ex.Message);
            }
            return View(model);
        }

        #region 修改数据
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model">仓库id</param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520110, 52011001)]
        public ActionResult WarehouseHandle(WarehouseModel model)
        {
            string result = string.Empty;

            if (model.WID.HasValue)
            {
                //先取出原有数据，然后修改部分数据（电话信息） 注意避免 空指针异常
                WarehouseModel modelInDB = new WarehouseModel().GetWarehouseData(model.WID.Value);
                if (modelInDB != null)
                {
                    WarehouseModel modelModi = AutoMapperHelper.MapTo<WarehouseModel>(modelInDB);
                    modelModi.ModityTime = DateTime.Now;
                    modelModi.ModityUserID = WorkContext.UserIdentity.UserId;
                    modelModi.ModityUserName = WorkContext.UserIdentity.UserName;
                    //电话和备注信息
                    modelModi.WCustomerServiceTel = model.WCustomerServiceTel;//400电话即投诉电话
                    modelModi.CWTel = model.CWTel;
                    modelModi.YW1Tel = model.YW1Tel;
                    modelModi.YW2Tel = model.YW2Tel;
                    modelModi.Remark = model.Remark;
                    //操作日志在Model层实现
                    result = new WarehouseModel().WarehouseEditData(modelModi).ToJson();
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "系统中找不到要编辑的数据"
                    }.ToJsonString();
                }
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "未选中数据"
                }.ToJsonString();
            }
            return Content(result);
        }
        #endregion

        /// <summary>
        /// 显示当前登录的仓库下面管辖的仓库子机构的信息集合
        /// </summary>
        /// <returns>子机构信息集合的视图</returns>
        [AuthorizeMenuFilter(520110)]
        public ActionResult WarehouseSub()
        {
            WarehouseSubListModel model = new WarehouseSubListModel();
            List<WarehouseSubModel> list = new List<WarehouseSubModel>();

            string parentCode = this.CurrentWarehouse.Parent.WarehouseCode;
            string parentName = this.CurrentWarehouse.Parent.WarehouseName;

            var currentSubList = this.CurrentWarehouse.ParentSubWarehouses;
            if (currentSubList != null && currentSubList.Count > 0)
            {
                foreach (var item in currentSubList)
                {
                    WarehouseSubModel subItem = new WarehouseSubModel();
                    subItem.ParentWName = parentName;
                    subItem.ParentWCode = parentCode;
                    subItem.SubWCode = item.WarehouseCode;
                    subItem.SubWName = item.WarehouseName;
                    list.Add(subItem);
                }
            }
            model.SubList = list;

            return View(model);
        }
    }
}
