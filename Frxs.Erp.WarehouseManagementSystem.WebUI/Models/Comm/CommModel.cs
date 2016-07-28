using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Comm
{
    /// <summary>
    /// model模块调用公共方法
    /// </summary>
    public static class CommModel
    {
        /// <summary>
        /// 通过父基本分类编号取得子基本分类列表(基本分类操作)
        /// </summary>
        /// <param name="pcategoryId">父基本分类编号</param>
        /// <returns>子基本分类列表</returns>
        public static List<FrxsErpProductCategoriesChildsGetResp.FrxsErpProductCategoriesChildsGetRespData> GetCategorieList(int? pcategoryId)
        {
            FrxsErpProductCategoriesChildsGetRequest r = new FrxsErpProductCategoriesChildsGetRequest
            {
                CategoryId = pcategoryId.HasValue ? pcategoryId.Value : (int?)null,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };

            //获取列表
            var resp = WorkContext.CreateProductSdkClient().Execute(r);
            //获取分类List解析的对象
            if (resp != null && resp.Data != null && resp.Data.Count > 0)
            {
                return resp.Data;
            }
            return null;
        }


        /// <summary>
        /// 通过父基本分类编号取得子基本分类列表
        /// </summary>
        /// <param name="pshopcategoryId">父基本分类编号</param>
        /// <returns>子基本分类列表</returns>
        public static List<FrxsErpProductShopCategoriesChildsGetResp.FrxsErpProductShopCategoriesChildsGetRespData> GetShopCategorieList(int? pshopcategoryId)
        {
            FrxsErpProductShopCategoriesChildsGetRequest r = new FrxsErpProductShopCategoriesChildsGetRequest
            {
                CategoryId = pshopcategoryId.HasValue ? pshopcategoryId.Value : (int?)null,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };

            //获取列表
            var resp = WorkContext.CreateProductSdkClient().Execute(r);
            //获取分类List解析的对象
            if (resp != null && resp.Data != null && resp.Data.Count > 0)
            {
                return resp.Data;
            }
            return null;
        }



        /// <summary>
        /// 取得基础数据字典列表
        /// </summary>
        /// <param name="dictCode">字典编码（类似单位 "Unit"）</param>
        /// <returns>字典明细列表</returns>
        public static IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> GetSysDictDetailList(string dictCode)
        {
            FrxsErpProductSysDictDetailGetListRequest r = new FrxsErpProductSysDictDetailGetListRequest { dictCode = dictCode };
            var resp = WorkContext.CreateProductSdkClient().Execute(r);
            if (resp != null && resp.Flag == 0)
            {
                return resp.Data;
            }
            return new List<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData>();
        }



        /// <summary>
        /// 通过参数编号和网仓编号 获取网仓参数信息
        /// 
        /// </summary>
        /// <param name="paramCode">参数编码（可以为空，业务设置参数编码；如果此参数编码未指定值，系统将查询出仓库所有的业务配置，如果指定了将返回单个参数配置）</param>
        /// <returns></returns>
        public static List<FrxsErpProductWarehouseSysParamsGetResp.FrxsErpProductWarehouseSysParamsGetRespData> GetWarehouseSysParams(string paramCode)
        {
            FrxsErpProductWarehouseSysParamsGetRequest r = new FrxsErpProductWarehouseSysParamsGetRequest
            {
                ParamCode = paramCode,
                WID = WorkContext.CurrentWarehouse.WarehouseId,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };

            //获取列表
            var resp = WorkContext.CreateProductSdkClient().Execute(r);
            //获取分类List解析的对象
            if (resp != null && resp.Data != null && resp.Data.Count > 0)
            {
                return resp.Data;
            }
            return null;
        }

    }
}