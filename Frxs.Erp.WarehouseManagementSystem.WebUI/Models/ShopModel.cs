using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using System.Web.Mvc;
using Frxs.Platform.Utility.Map;
using System.Runtime.Serialization;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class ShopSearchModel : SearchModelBase
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember]
        [DisplayName("门店编号")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMember]
        [DisplayName("门店名称")]
        public string ShopName { get; set; }

        [DataMember]
        [DisplayName("门店账号")]
        public string ShopAccount { get; set; }

        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public int Status { get; set; }

        /// <summary>
        /// 门店联系人姓名
        /// </summary>
        [DataMember]
        [DisplayName("联系人")]
        public string LinkMan { get; set; }

        /// <summary>
        /// 线路ID WarehouseLineShop.LineID
        /// </summary>
        [DataMember]
        [DisplayName("所属线路")]
        public string LineID { get; set; }

    }

    /// <summary>
    /// 显示列表
    /// </summary>
    public class ShopListModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 门店ID
        /// </summary>       
        [DisplayName("门店ID")]
        public int ShopID { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember]
        [DisplayName("门店编号")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMember]
        [DisplayName("门店名称")]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店联系人姓名
        /// </summary>
        [DataMember]
        [DisplayName("联系人")]
        public string LinkMan { get; set; }

        /// <summary>
        /// 门店联系电话
        /// </summary>
        [DataMember]
        [DisplayName("联系电话")]
        public string Telephone { get; set; }

        /// <summary>
        /// 地址全称
        /// </summary>
        [DataMember]
        [DisplayName("地址全称")]
        public string FullAddress { get; set; }

        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态(1:正常;0:冻结)")]
        public int Status { get; set; }

        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusStr { get; set; }


        [DataMember]
        [DisplayName("门店账号")]
        public string ShopAccount { get; set; }

        /// <summary>
        /// 线路ID WarehouseLineShop.LineID
        /// </summary>
        [DataMember]
        [DisplayName("送货线路")]
        public int LineName { get; set; }

        /// <summary>
        /// 发货排序 WarehouseLineShop.SerialNumber 
        /// </summary>
        [DataMember]
        [DisplayName("发货排序")]
        public string SerialNumberStr { get; set; }

        #endregion

    }

    /// <summary>
    /// 门店资料表Shop实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class ShopModel : BaseModel
    {

        #region 模型
        /// <summary>
        /// 门店ID
        /// </summary>
        [DataMember]
        [DisplayName("门店ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ShopID { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember]
        [DisplayName("门店编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店类型(0:加盟店;1:签约店;)
        /// </summary>
        [DataMember]
        [DisplayName("门店类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ShopType { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMember]
        [DisplayName("门店名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopName { get; set; }

        /// <summary>
        /// 结算方式(0:现金 + 数据字典: ShopSettleType)
        /// </summary>
        [DataMember]
        [DisplayName("结算方式")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SettleType { get; set; }

        /// <summary>
        /// 配送仓库ID(WarehouseInfo.WID)
        /// </summary>
        [DataMember]
        [DisplayName("配送仓库ID(WarehouseInfo.WID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 门店联系人姓名
        /// </summary>
        [DataMember]
        [DisplayName("联系人")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string LinkMan { get; set; }

        /// <summary>
        /// 门店联系电话
        /// </summary>
        [DataMember]
        [DisplayName("联系电话")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Telephone { get; set; }

        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Status { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        [DataMember]
        [DisplayName("法人")]

        public string LegalPerson { get; set; }

        /// <summary>
        /// 结算时间( 数据字典: ShopSettleTimeType)
        /// </summary>
        [DataMember]
        [DisplayName("结算时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SettleTimeType { get; set; }

        /// <summary>
        /// 门店级别(数据字典: ShopLevel; A:A级;B:B级;C:C级)
        /// </summary>
        [DataMember]
        [DisplayName("门店级别")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CreditLevel { get; set; }

        /// <summary>
        /// 信用额度
        /// </summary>
        [DataMember]
        [DisplayName("门店信用额度")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double CreditAmt { get; set; }

        /// <summary>
        /// 区域负责人
        /// </summary>
        [DataMember]
        [DisplayName("区域负责人")]

        public string AreaPrincipal { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [DataMember]
        [DisplayName("省")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ProvinceID { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [DataMember]
        [DisplayName("市")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int CityID { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [DataMember]
        [DisplayName("区")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int RegionID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        [DisplayName("地址")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Address { get; set; }

        /// <summary>
        /// 地址全称
        /// </summary>
        [DataMember]
        [DisplayName("地址全称")]

        public string FullAddress { get; set; }

        /// <summary>
        /// 门店面积(平方米)
        /// </summary>
        [DataMember]
        [DisplayName("门店面积")]
        [Required(ErrorMessage = "{0}不能为空")]
        public decimal ShopArea { get; set; }

        /// <summary>
        /// 是否删除(0:未删除;1:已删除)
        /// </summary>
        [DataMember]
        [DisplayName("是否删除")]

        public int IsDeleted { get; set; }

        /// <summary>
        /// 纬度（百度座标)
        /// </summary>
        [DataMember]
        [DisplayName("纬度（百度座标)")]

        public string Latitude { get; set; }

        /// <summary>
        /// 经度（百度座标)
        /// </summary>
        [DataMember]
        [DisplayName("经度（百度座标)")]

        public string Longitude { get; set; }

        /// <summary>
        /// 门店累计积分
        /// </summary>
        [DataMember]
        [DisplayName("累计积分")]
        //[Required(ErrorMessage = "{0}不能为空")]
        public double TotalPoint { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        [DataMember]
        [DisplayName("银行帐号")]

        public string BankAccount { get; set; }

        /// <summary>
        /// 银行开户名称
        /// </summary>
        [DataMember]
        [DisplayName("开户名")]

        public string BankAccountName { get; set; }

        /// <summary>
        /// 银行类型
        /// </summary>
        [DataMember]
        [DisplayName("开户行")]

        public string BankType { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [DataMember]
        [DisplayName("身份证号")]

        public string CardID { get; set; }

        /// <summary>
        /// 区域负责人
        /// </summary>
        [DataMember]
        [DisplayName("区域负责人")]

        public string RegionMaster { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户 ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户 ID")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        [DisplayName("修改时间")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]
        public string ModifyUserName { get; set; }


        [DataMember]
        [DisplayName("门店账号")]
        public string ShopAccount { get; set; }

        /// <summary>
        /// 线路ID WarehouseLineShop.LineID
        /// </summary>
        [DataMember]
        [DisplayName("所属线路")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int LineID { get; set; }

        /// <summary>
        /// 发货排序 WarehouseLineShop.SerialNumber 
        /// </summary>
        [DataMember]
        [DisplayName("发货排序")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int SerialNumber { get; set; }

       

        #endregion

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }


        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public ShopModel GetShopData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopWarehouseGetRequest()
            {
                ShopID = int.Parse(id)

            });

            ShopModel model = AutoMapperHelper.MapTo<ShopModel>(resp.Data);
            if (model.SerialNumber < 0)
            {
                model.SerialNumber = 0;
            }
            return model;
        }
        #endregion

        

        #region 冻结数据
        /// <summary>
        /// 冻结数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public int FrozenShop(string ids, int frozen)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopWarehouseFreezeRequest()
            {
                ShopID = ids,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                Status = frozen

            });

            return resp.Data;
        }
        #endregion

        #region 保存数据
        public object SaveShopData(ShopModel model)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopWarehouseSaveRequest()
            {
                ShopCode = model.ShopCode,
                ShopType = model.ShopType,
                ShopName = model.ShopName,
                SettleType = model.SettleType,
                WID = model.WID,
                LinkMan = model.LinkMan,
                Telephone = model.Telephone,
                Status = model.Status,
                LegalPerson = model.LegalPerson,
                SettleTimeType = model.SettleTimeType,
                CreditLevel = model.CreditLevel,
                CreditAmt = model.CreditAmt,
                AreaPrincipal = model.AreaPrincipal,
                ProvinceID = model.ProvinceID,
                CityID = model.CityID,
                RegionID = model.RegionID,
                Address = model.Address,
                FullAddress = model.FullAddress,
                ShopArea = model.ShopArea,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                BankAccount = model.BankAccount,
                BankAccountName = model.BankAccountName,
                BankType = model.BankType,
                CardID = model.CardID,
                RegionMaster = model.RegionMaster,
                ShopID = model.ShopID,
                ShopAccount = model.ShopAccount,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                LineID=model.LineID,
                SerialNumber = model.SerialNumber
            });

            if (resp.Flag == 0)
            {
                return new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功"
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



        }
        #endregion

        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetShopModelPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopWarehouseTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    ShopCode = conditionDict.ContainsKey("ShopCode") ? Utils.NoHtml(conditionDict["ShopCode"].ToString()) : null,

                    ShopName = conditionDict.ContainsKey("ShopName") ? Utils.NoHtml(conditionDict["ShopName"].ToString()) : null,
                    ShopAccount = conditionDict.ContainsKey("ShopAccount") ? Utils.NoHtml(conditionDict["ShopAccount"].ToString()) : null,
                    Status = conditionDict.ContainsKey("Status") ? Utils.NoHtml(conditionDict["Status"].ToString()) : null,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                    LinkMan = conditionDict.ContainsKey("LinkMan") ? Utils.NoHtml(conditionDict["LinkMan"].ToString()) : null,
                    LineID = conditionDict.ContainsKey("LineID") ? Utils.NoHtml(conditionDict["LineID"].ToString()) : null,
                    SortBy = "LineID asc"
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

            return jsonStr;
        }
        #endregion

        /// <summary>
        /// 结帐方式集合
        /// </summary>
        public SelectList SettleTypeList { get; set; }

        public void BindSettleTypeList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysDictDetailGetListRequest()
            {
                dictCode = "ShopSettleType"
            });

            if (resp != null && resp.Flag == 0)
            {

                SettleTypeList = new SelectList(resp.Data, "DictValue", "DictLabel", true);
            }
            else
            {
                SettleTypeList = null;
            }
        }

        /// <summary>
        /// 结算方式集合
        /// </summary>
        public SelectList SettleTimeTypeList { get; set; }

        public void BindSettleTimeTypeList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysDictDetailGetListRequest()
            {
                dictCode = "ShopSettleTimeType"
            });

            if (resp != null && resp.Flag == 0)
            {

                SettleTimeTypeList = new SelectList(resp.Data, "DictValue", "DictLabel", true);
            }
            else
            {
                SettleTimeTypeList = null;
            }
        }

        /// <summary>
        /// 门店级别集合
        /// </summary>
        public SelectList CreditLevelList { get; set; }

        public void BindCreditLevelList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysDictDetailGetListRequest()
            {
                dictCode = "ShopLevel"
            });

            if (resp != null && resp.Flag == 0)
            {

                CreditLevelList = new SelectList(resp.Data, "DictValue", "DictLabel", true);
            }
            else
            {
                CreditLevelList = null;
            }
        }

        /// <summary>
        /// 门店类型集合
        /// </summary>
        public SelectList ShopTypeList { get; set; }

        public void BindShopTypeList()
        {
            ShopTypeList = new SelectList(new[] { new { Text = "加盟店", Value = "0" }, new { Text = "签约店", Value = "1" } }, "Value", "Text", true);
        }

        /// <summary>
        /// 门店级别集合
        /// </summary>
        public SelectList WarehouseLineList { get; set; }

        public void BindWarehouseLineList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId             
            });           
            if (resp != null && resp.Flag == 0)
            {
                WarehouseLineList = new SelectList(resp.Data.ItemList, "LineID", "LineName", true);

            }
            else
            {
                WarehouseLineList = null;
            }
        }

    }

}