using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 供应商表Vendor实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class Vendor : BaseModel
    {
        /// <summary>
        /// 供应商类型(VendorType.VendorTypeID)
        /// </summary>
        [DataMember]
        [DisplayName("类型")]
        public string VendorTypeName { get; set; }

        /// <summary>
        /// 结算方式( 数据字典: VendorSettleTimeType)
        /// </summary>
        [DataMember]
        [DisplayName("结算方式")]
        public string SettleTimeTypeName { get; set; }

        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusName { get; set; }

        [DataMember]
        [DisplayName("等级")]
        public string CreditLevelName { get; set; }
    }

    /// <summary>
    /// 供应商表Vendor实体类
    /// </summary>
    public partial class Vendor : BaseModel
    {

        #region 模型
        /// <summary>
        /// 供应商分类ID
        /// </summary>
        [DataMember]
        [DisplayName("ID")]
        public int VendorID { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        [DisplayName("编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string VendorCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string VendorName { get; set; }

        /// <summary>
        /// 供应商简称
        /// </summary>
        [DataMember]
        [DisplayName("简称")]
        public string VendorShortName { get; set; }

        /// <summary>
        /// 供应商类型(VendorType.VendorTypeID)
        /// </summary>
        [DataMember]
        [DisplayName("类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int VendorTypeID { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [DataMember]
        [DisplayName("联系人姓名")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string LinkMan { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        [DisplayName("联系电话")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Telephone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [DataMember]
        [DisplayName("传真")]

        public string Fax { get; set; }

        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Status { get; set; }

        /// <summary>
        /// 企业法人
        /// </summary>
        [DataMember]
        [DisplayName("企业法人")]

        public string LegalPerson { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [DataMember]
        [DisplayName("电子邮箱")]

        public string Email { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        [DataMember]
        [DisplayName("公司网址")]

        public string WebUrl { get; set; }

        /// <summary>
        /// 行政区域
        /// </summary>
        [DataMember]
        [DisplayName("行政区域")]

        public string Region { get; set; }

        /// <summary>
        /// 结算方式( 数据字典: VendorSettleTimeType)
        /// </summary>
        [DataMember]
        [DisplayName("结算方式")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SettleTimeType { get; set; }

        /// <summary>
        /// 供应商级别(数据字典: VendorLevel; A:A级;B:B级;C:C级)
        /// </summary>
        [DataMember]
        [DisplayName("供应商级别")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CreditLevel { get; set; }

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
        /// 是否删除(0:未删除;1:已删除);
        /// </summary>
        [DataMember]
        [DisplayName("是否删除")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]

        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]

        public string CreateUserName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
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

        #endregion
    }
}