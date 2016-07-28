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
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class StockAdjQuery : BasePageModel
    {
        public string AdjType { get; set; }
        public string AdjID { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int? Status { get; set; }
        public string WID { get; set; }
        public string SubWID { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class StockAdjModel : BaseModel
    {
        #region Model
        private string _adjid;
        private DateTime? _adjdate;
        private string _planid;
        private int _wid;
        private string _wcode;
        private string _wname;
        private int _subwid;
        private string _subwcode;
        private string _subwname;
        private int _status;
        private int _adjtype;
        private DateTime? _conftime;
        private int? _confuserid;
        private string _confusername;
        private DateTime? _postingtime;
        private int? _postinguserid;
        private string _postingusername;
        private string _remark;
        private DateTime? _createtime;
        private int _createuserid;
        private string _createusername;
        private DateTime? _modifytime;
        private int? _modifyuserid;
        private string _modifyusername;
        /// <summary>
        /// 调整ID(WID+ ID服务)
        /// </summary>
        public string AdjID
        {
            set { _adjid = value; }
            get { return _adjid; }
        }
        /// <summary>
        /// 开单日期
        /// </summary>
        public DateTime? AdjDate
        {
            set { _adjdate = value; }
            get { return _adjdate; }
        }
        /// <summary>
        /// 盘点计划ID
        /// </summary>
        public string PlanID
        {
            set { _planid = value; }
            get { return _planid; }
        }
        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        public int WID
        {
            set { _wid = value; }
            get { return _wid; }
        }
        /// <summary>
        /// 仓库编号(Warehouse.WCode)
        /// </summary>
        public string WCode
        {
            set { _wcode = value; }
            get { return _wcode; }
        }
        /// <summary>
        /// 仓库名称(Warehouse.WarehouseName)
        /// </summary>
        public string WName
        {
            set { _wname = value; }
            get { return _wname; }
        }
        /// <summary>
        /// 仓库子机构ID(WarehouseID)
        /// </summary>
        public int SubWID
        {
            set { _subwid = value; }
            get { return _subwid; }
        }
        /// <summary>
        /// 仓库子机构编号(Warehouse.WCode)
        /// </summary>
        public string SubWCode
        {
            set { _subwcode = value; }
            get { return _subwcode; }
        }
        /// <summary>
        /// 仓库柜台名称(Warehouse.WarehouseName)
        /// </summary>
        public string SubWName
        {
            set { _subwname = value; }
            get { return _subwname; }
        }
        /// <summary>
        /// 状态(0:未提交;1:已提交;2:已过帐;3:作废) 0>1 1->2 1>0; 1>3; 0  删除时物理删除;
        /// </summary>
        public int Status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 调整类型(0:调增库存;1:调减库存)
        /// </summary>
        public int AdjType
        {
            set { _adjtype = value; }
            get { return _adjtype; }
        }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? ConfTime
        {
            set { _conftime = value; }
            get { return _conftime; }
        }
        /// <summary>
        /// 提交用户ID
        /// </summary>
        public int? ConfUserID
        {
            set { _confuserid = value; }
            get { return _confuserid; }
        }
        /// <summary>
        /// 提交用户名称
        /// </summary>
        public string ConfUserName
        {
            set { _confusername = value; }
            get { return _confusername; }
        }
        /// <summary>
        /// 过帐时间
        /// </summary>
        public DateTime? PostingTime
        {
            set { _postingtime = value; }
            get { return _postingtime; }
        }
        /// <summary>
        /// 过帐用户ID
        /// </summary>
        public int? PostingUserID
        {
            set { _postinguserid = value; }
            get { return _postinguserid; }
        }
        /// <summary>
        /// 过帐用户名称
        /// </summary>
        public string PostingUserName
        {
            set { _postingusername = value; }
            get { return _postingusername; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 创建用户ID
        /// </summary>
        public int CreateUserID
        {
            set { _createuserid = value; }
            get { return _createuserid; }
        }
        /// <summary>
        /// 创建用户名称
        /// </summary>
        public string CreateUserName
        {
            set { _createusername = value; }
            get { return _createusername; }
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? ModifyTime
        {
            set { _modifytime = value; }
            get { return _modifytime; }
        }
        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        public int? ModifyUserID
        {
            set { _modifyuserid = value; }
            get { return _modifyuserid; }
        }
        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        public string ModifyUserName
        {
            set { _modifyusername = value; }
            get { return _modifyusername; }
        }
        #endregion Model

        #region 从表

        #endregion

        public string GetStockAdjPageData(StockAdjQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateOrderSdkClient();
                //Dictionary<string, object> conditionDict = base.PrePareFormParam();
                int? subWID = null;
                if (!String.IsNullOrEmpty(cpm.SubWID))
                {
                    subWID = int.Parse(cpm.SubWID);
                }
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjListGetRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    ProductName = cpm.ProductName,
                    AdjType = int.Parse(cpm.AdjType),
                    AdjID = cpm.AdjID,
                    SKU = cpm.SKU,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SubWID = subWID,
                    Status = cpm.Status,
                    AdjDateBegin = cpm.BeginTime,
                    AdjDateEnd = cpm.EndTime,
                    //SortBy = cpm.sort + " " + cpm.order
                    SortBy = "CreateTime" + " " + "desc"
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

        public int DeleteStockAdj(string ids, int adjType)
        {
            var IdList = ids.Split(',').ToList();
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDelRequest()
            {
                AdjIDs = IdList,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (adjType.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                      ConstDefinition.XSOperatorActionDel, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionDel, ids));
                }
                else if (adjType.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                      ConstDefinition.XSOperatorActionDel, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionDel, ids));
                }

                result = resp.Data;
            }
            return result;
        }

        public StockAdjModel GetStockAdj(string id)
        {
            StockAdjModel model = new StockAdjModel();
            if (!String.IsNullOrEmpty(id))
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjGetRequest()
                {
                    AdjID = id,
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });
                if (resp != null && resp.Flag == 0)
                {
                    model = AutoMapperHelper.MapTo<StockAdjModel>(resp.Data.StockAdjMain);
                }
            }
            else
            {
                //var serviceCenter = WorkContext.CreateIDSdkClient();
                //var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
                //{
                //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                //    Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.StockAdj
                //});
                //if (resp != null && resp.Flag == 0)
                //{
                //    var AdjID = resp.Data;
                //    model.AdjID = AdjID;
                //}
            }
            return model;
        }

        public ResultData AddStockAdj(StockAdjModel model)
        {
            //获取指定子机构
            var subWCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
            var subWid = WorkContext.CurrentWarehouse.Parent.WarehouseId;
            var subWName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
            //if (WorkContext.CurrentWarehouse.Parent.WarehouseId != WorkContext.CurrentWarehouse.WarehouseId)
            //{
            //    var SubWarehouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(t => t.WarehouseId.Equals(model.SubWID));
            //    if (SubWarehouse != null)
            //    {
            //        SubWCode = SubWarehouse.WarehouseCode;
            //        SubWID = SubWarehouse.WarehouseId;
            //        SubWName = SubWarehouse.WarehouseName;
            //    }
            //}
            var subWarehouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(t => t.WarehouseId.Equals(model.SubWID));
            if (subWarehouse != null)
            {
                subWCode = subWarehouse.WarehouseCode;
                subWid = subWarehouse.WarehouseId;
                subWName = subWarehouse.WarehouseName;
            }
            //获取ID
            var stockAdjId = GetStockAdjId();
            if (stockAdjId.Equals(String.Empty))
            {
                var result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = "ID获取失败",
                    Data = null
                };
                return result;
            }
            model.AdjID = stockAdjId;
            var serviceCenter = WorkContext.CreateOrderSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjSaveRequest()
            {
                AdjDate = (DateTime)model.AdjDate,
                AdjID = model.AdjID,
                AdjType = model.AdjType,
                Flag = 0,
                PlanID = String.Empty,
                Remark = model.Remark,
                Status = 0,
                SubWCode = subWCode,
                SubWID = subWid,
                SubWName = subWName,
                WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                WName = WorkContext.CurrentWarehouse.Parent.WarehouseName
            });
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (model.AdjType.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                      ConstDefinition.XSOperatorActionAdd, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionAdd, model.AdjID));
                }
                else if (model.AdjType.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                      ConstDefinition.XSOperatorActionAdd, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionAdd, model.AdjID));
                }

                var result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功",
                    Data = model.AdjID
                };
                return result;
            }
            else
            {
                var result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info,
                    Data = null
                };
                return result;
            }
        }

        public ResultData EditStockAdj(StockAdjModel model)
        {
            var SubWCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
            var SubWID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
            var SubWName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
            //if (WorkContext.CurrentWarehouse.Parent.WarehouseId != WorkContext.CurrentWarehouse.WarehouseId)
            //{
            //    var SubWarehouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(t => t.WarehouseId.Equals(model.SubWID));
            //    if (SubWarehouse != null)
            //    {
            //        SubWCode = SubWarehouse.WarehouseCode;
            //        SubWID = SubWarehouse.WarehouseId;
            //        SubWName = SubWarehouse.WarehouseName;
            //    }
            //}
            var SubWarehouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(t => t.WarehouseId.Equals(model.SubWID));
            if (SubWarehouse != null)
            {
                SubWCode = SubWarehouse.WarehouseCode;
                SubWID = SubWarehouse.WarehouseId;
                SubWName = SubWarehouse.WarehouseName;
            }
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjSaveRequest()
            {
                AdjDate = (DateTime)model.AdjDate,
                AdjID = model.AdjID,
                AdjType = model.AdjType,
                Flag = 1,
                PlanID = String.Empty,
                Remark = model.Remark,
                Status = model.Status,
                SubWCode = SubWCode,
                SubWID = SubWID,
                SubWName = SubWName,
                WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                WName = WorkContext.CurrentWarehouse.Parent.WarehouseName
            });
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (model.AdjType.Equals(0))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7E,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}盘盈单[{1}]", ConstDefinition.XSOperatorActionEdit, model.AdjID));
                }
                else if (model.AdjType.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_7F,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}盘亏单[{1}]", ConstDefinition.XSOperatorActionEdit, model.AdjID));
                }

                var result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功",
                    Data = model.AdjID
                };
                return result;
            }
            else
            {
                var result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
                return result;
            }
        }

        public string GetStockAdjId()
        {
            var serviceCenter = WorkContext.CreateIDSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.StockAdj
            });
            if (resp != null && resp.Flag == 0)
            {
                return resp.Data;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}