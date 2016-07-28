using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Web;
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Log;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using System.Linq;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 仓库主表Warehouse实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class WarehouseModel : BaseModel
    {

        #region 模型
        /// <summary>
        /// 仓库ID(从1000开始编号)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(从1000开始编号)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? WID { get; set; }

        /// <summary>
        /// 仓库编号(唯一)
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        [DisplayName("仓库名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WName { get; set; }

        /// <summary>
        /// 仓库级别(0:总部[预留];1:仓库;2:仓库子机构物流/退货])
        /// </summary>
        [DataMember]

        [DisplayName("仓库级别(0:总部[预留];1:仓库;2:仓库子机构物流/退货])")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WLevel { get; set; }

        /// <summary>
        /// 子机构类型(0:退货;1:物流库;)
        /// </summary>
        [DataMember]
        [DisplayName("子机构类型(0:退货;1:物流库;)")]

        public int? WSubType { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [DataMember]
        [DisplayName("父级ID")]

        public int? ParentWID { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        [DisplayName("联系电话")]


        public string WTel { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [DataMember]
        [DisplayName("联系人姓名")]


        public string WContact { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [DataMember]
        [DisplayName("省")]

        public int? ProvinceID { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [DataMember]
        [DisplayName("市")]

        public int? CityID { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [DataMember]
        [DisplayName("区")]

        public int? RegionID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        [DisplayName("地址")]


        public string WAddress { get; set; }

        /// <summary>
        /// 全称地址
        /// </summary>
        [DataMember]
        [DisplayName("全称地址")]

        public string WFullAddress { get; set; }

        /// <summary>
        /// 400客服电话
        /// </summary>
        [DataMember]
        [DisplayName("400客服电话")]

        public string WCustomerServiceTel { get; set; }

        /// <summary>
        /// 退货部电话
        /// </summary>
        [DataMember]
        [DisplayName("退货部电话")]

        public string THBTel { get; set; }

        /// <summary>
        /// 财务室电话
        /// </summary>
        [DataMember]
        [DisplayName("财务室电话")]

        public string CWTel { get; set; }

        /// <summary>
        /// 业务咨询电话1
        /// </summary>
        [DataMember]
        [DisplayName("业务咨询电话1")]

        public string YW1Tel { get; set; }

        /// <summary>
        /// 业务咨询电话2
        /// </summary>
        [DataMember]
        [DisplayName("业务咨询电话2")]

        public string YW2Tel { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]

        public string Remark { get; set; }

        /// <summary>
        /// 是否已被冻结(0:未冻结;1、已冻结)
        /// </summary>
        [DataMember]
        [DisplayName("冻结状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? IsFreeze { get; set; }

        /// <summary>
        /// 是否已被删除(0:未删除;1、已删除)
        /// </summary>
        [DataMember]
        [DisplayName("是否已被删除(0:未删除;1、已删除)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户 ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户 ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        [DisplayName("修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime ModityTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ModityUserID { get; set; }

        /// <summary>
        /// 最后修改用记名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用记名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ModityUserName { get; set; }

        #endregion


        #region 自定义方法、属性

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }
        /// <summary>
        /// 冻结状态(0:正常; 1:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string FreezeStatus { get; set; }

        /// <summary>
        /// 管辖的仓库子机构数量
        /// </summary>
        [DataMember]
        [DisplayName("仓库子机构")]
        public int SubNum { get; set; }



        #region 获取管辖的仓库子机构的数量
        /// <summary>
        /// 管辖的子机构数量
        /// </summary>
        /// <returns></returns> 
        public int GetSubNum()
        {
            return 0;
        }
        #endregion

        #region 获取仓库数据
        /// <summary>
        /// 获取仓库数据
        /// </summary>
        /// <param name="WID">WID</param>
        /// <returns></returns>
        [Platform.Utility.Filter.ExceptionFilterAttribute]
        public WarehouseModel GetWarehouseData(int ID)
        {
            WarehouseModel model = new WarehouseModel();
            try
            {
                //获取
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductWarehouseGetRequest()
                {
                    WID = ID
                });

                if (resp != null && resp.Flag == 0 || resp.Data != null)
                {
                    model = AutoMapperHelper.MapTo<WarehouseModel>(resp.Data);
                }
                else
                {
                    model.Remark = "访问接口服务器失败，未能获取到仓库信息!";
                }
            }
            catch (Exception ex)
            {
                model.Remark = string.Format("出现异常，未能获取到仓库信息! {0},{1}", Environment.NewLine, ex.Message);
            }
            return model;
        }

        #endregion


        #region 修改仓库数据
        /// <summary>
        /// 仓库后台中 设定只允许修改联系电话和备注信息
        /// </summary>
        /// <param name="model">WarehouseModel</param>
        /// <returns>ResultData对象</returns>
        public object WarehouseEditData(WarehouseModel model)
        {
            try
            {
                //获取上送参数的值
                var RequestDto = new FrxsErpProductWarehouseEditRequest();
                RequestDto = AutoMapperHelper.MapTo<FrxsErpProductWarehouseEditRequest>(model);

                //执行处理
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(RequestDto);

                #region 记录操作日志
                if (resp != null && resp.Flag == 0)
                {
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1A,
                                               ConstDefinition.XSOperatorActionEdit, string.Format("{0}仓库[{1}]资料", ConstDefinition.XSOperatorActionEdit, model.WName));
                    return new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = resp.Info
                    };
                }
                else
                {
                    return new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    };
                }
                #endregion

            }
            catch (Exception ex)
            {
                //throw ex;
                return new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = ex.Message
                };
            }
        }
        #endregion

        #region 获取仓库子机构列表
        /// <summary>
        /// 获取仓库子机构列表
        /// </summary>
        /// <param name="ParentWID">仓库子机构的父级ID</param>
        /// <returns>json格式字符串</returns>
        public List<WarehouseSubModel> GetWarehouseSubList(int ParentWID)
        {
            List<WarehouseSubModel> warehouseSubList = new List<WarehouseSubModel>();
            try
            {
                //获取父仓库信息


                //获取列表
                var resp = WorkContext.CreateProductSdkClient().Execute(new FrxsErpProductWarehouseTableListRequest()
                {
                    PageIndex = 1,
                    PageSize = 100,
                    ParentWID = ParentWID
                });

                if (resp != null && resp.Flag == 0 && resp.Data.ItemList != null && resp.Data.ItemList.Count > 0)
                {
                    foreach (var item in resp.Data.ItemList)
                    {
                        WarehouseSubModel warehouseSub = new WarehouseSubModel();
                        warehouseSub.SubWCode = item.WCode;
                        warehouseSub.SubWName = item.WName;
                        warehouseSub.WLevel = item.WLevel.Value;
                        warehouseSubList.Add(warehouseSub);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return warehouseSubList;
        }
        #endregion

        #endregion
    }

    [Serializable]
    [DataContract]
    public partial class WarehouseSubModel : BaseModel
    {
        /// <summary>
        /// 父机构名称
        /// </summary>
        public string ParentWName { get; set; }

        /// <summary>
        /// 父机构编号
        /// </summary>
        public string ParentWCode { get; set; }

        /// <summary>
        /// 仓库子机构编号
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWCode { get; set; }

        /// <summary>
        /// 仓库子机构名称
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWName { get; set; }

        /// <summary>
        /// WLevel 仓库级别(0:总部[预留];1:仓库;2:仓库子机构物流/退货])
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        public int WLevel { get; set; }


        /// <summary>
        /// 子机构类型(0:退货;1:物流库;)
        /// </summary>       
        [DisplayName("子机构类型(0:退货;1:物流库;)")]

        public int WSubType { get; set; }
    }

    public class WarehouseSubListModel : BaseModel
    {
        /// <summary>
        /// 子机构列表
        /// </summary>
        public List<WarehouseSubModel> SubList { get; set; }
    }


}


