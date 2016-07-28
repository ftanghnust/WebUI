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
    /// 门店群组明细表ShopGroupDetails实体类
    /// </summary>
   
    public partial class ShopGroupDetails : BaseModel
    {

        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]       
        public long ID { get; set; }

        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]       
        public long GroupID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]        
        public int WID { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [DataMember]
        [DisplayName("门店ID")]        
        public int ShopID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]        
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

        #endregion
    }

     /// <summary>
    /// 门店群组明细表ShopGroupDetails实体类
    /// </summary>    
    public partial class ShopGroupDetails : BaseModel
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

        /// <summary>
        /// 门店地址
        /// </summary>
        [DataMember]
        [DisplayName("门店地址")]
        public string FullAddress { get; set; }
    }
}