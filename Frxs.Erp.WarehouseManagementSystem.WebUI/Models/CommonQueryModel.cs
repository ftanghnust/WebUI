using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 公共查询模型
    /// 作者:蔡睿
    /// 创建日期:2013-9-4
    /// </summary>
    public class CommonQueryModel
    {
        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 操作控件html
        /// </summary>
        public string OperateControlStr { get; set; }

        /// <summary>
        /// 查询条件表格行html
        /// </summary>
        public string SearchTrStr { get; set; }

        /// <summary>
        /// 高级搜索允许行数
        /// </summary>
        public int AdvancedSearchRows { get; set; }

        /// <summary>
        /// 表名数组
        /// </summary>
        public string[] TableNameList { get; set; }

        /// <summary>
        /// 表描述名数组
        /// </summary>
        public string[] TableDescList { get; set; }

        /// <summary>
        /// 表对应显示列数数组
        /// </summary>
        public int[] ShowColumnCount { get; set; }

        /// <summary>
        /// js脚本
        /// </summary>
        public string JsScript { get; set; }

        /// <summary>
        /// 父表节点配置中对应子表名称属性数组
        /// </summary>
        public string[] ChildTableNameAttrList { get; set; }

        /// <summary>
        /// 子表名称数组
        /// </summary>
        public string[] ChildTableNameList { get; set; }

        /// <summary>
        /// 子表对应显示列数数组
        /// </summary>
        public int[] ChildShowColumnCount { get; set; }

        /// <summary>
        /// 子表是否显示分页
        /// </summary>
        public bool[] ChildShowPage { get; set; }

        /// <summary>
        /// 查询参数列表
        /// </summary>
        public Dictionary<string, string> QueryParamList { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 获取公共查询模型
        /// </summary>
        /// <returns>公共查询模型</returns>
        public CommonQueryModel GetCommonQueryModel()
        {
            CommonQueryModel model = new CommonQueryModel();

            QueryPageBuilder queryPageBuilder = new QueryPageBuilder(ConfigName, PageName);

            queryPageBuilder.StartAssemblyName = this.GetType().Assembly.GetName().Name;
            model.OperateControlStr = queryPageBuilder.BuildOperateControls();
            model.SearchTrStr = queryPageBuilder.BuildSearchControls();
            model.AdvancedSearchRows = queryPageBuilder.XmlConfig.AdvancedSearchRows;
            model.TableNameList = new string[queryPageBuilder.XmlConfig.TableList.Length];
            model.TableDescList = new string[queryPageBuilder.XmlConfig.TableList.Length];
            model.ShowColumnCount = new int[queryPageBuilder.XmlConfig.TableList.Length];
            model.PageName = PageName;
            int i = 0;
            foreach (XMLTable xmlTable in queryPageBuilder.XmlConfig.TableList)
            {
                model.TableNameList[i] = xmlTable.TableName;
                model.TableDescList[i] = xmlTable.TableDesc;
                model.ShowColumnCount[i] = xmlTable.ShowColumnCount;
                i++;
            }
            model.JsScript = queryPageBuilder.BuildJsScript(QueryParamList);

            return model;
        }

        /// <summary>
        /// 获取公共查询模型
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <param name="pageName">页面名称</param>
        /// <returns>公共查询模型</returns>
        public CommonQueryModel GetCommonQueryModel(string configName, string pageName)
        {
            CommonQueryModel model = new CommonQueryModel();

            QueryPageBuilder queryPageBuilder = new QueryPageBuilder(configName, pageName);

            queryPageBuilder.ConditionDict = new Dictionary<string, string>();
            foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
            {
                string value = HttpContext.Current.Request.QueryString[key];
                if (key != "configName" && key != "pageName" && !string.IsNullOrEmpty(value.Trim()))
                {
                    queryPageBuilder.ConditionDict.Add(key, value.Trim());
                }
            }

            queryPageBuilder.StartAssemblyName = this.GetType().Assembly.GetName().Name;
            model.OperateControlStr = queryPageBuilder.BuildOperateControls();
            model.SearchTrStr = queryPageBuilder.BuildSearchControls();
            model.AdvancedSearchRows = queryPageBuilder.XmlConfig.AdvancedSearchRows;
            model.TableNameList = new string[queryPageBuilder.XmlConfig.TableList.Length];
            model.TableDescList = new string[queryPageBuilder.XmlConfig.TableList.Length];
            model.ShowColumnCount = new int[queryPageBuilder.XmlConfig.TableList.Length];
            model.ChildTableNameAttrList = new string[queryPageBuilder.XmlConfig.TableList.Length];
            model.ChildTableNameList = new string[queryPageBuilder.XmlConfig.TableList.Length];
            model.ChildShowColumnCount = new int[queryPageBuilder.XmlConfig.TableList.Length];
            model.ChildShowPage = new bool[queryPageBuilder.XmlConfig.TableList.Length];
            model.PageTitle = queryPageBuilder.XmlConfig.PageTitle;
            int i = 0;
            foreach (XMLTable xmlTable in queryPageBuilder.XmlConfig.TableList)
            {
                model.TableNameList[i] = xmlTable.TableName;
                model.TableDescList[i] = xmlTable.TableDesc;
                model.ShowColumnCount[i] = xmlTable.ShowColumnCount;
                model.ChildTableNameAttrList[i] = xmlTable.ChildTableNameAttr;
                if (xmlTable.ChildTable != null)
                {
                    model.ChildTableNameList[i] = xmlTable.ChildTable.ChildTableName;
                    model.ChildShowColumnCount[i] = xmlTable.ChildTable.ChildTableShowColumnCount;
                    model.ChildShowPage[i] = xmlTable.ChildTable.IsShowPage;
                }
                i++;
            }
            model.JsScript = queryPageBuilder.BuildJsScript(QueryParamList);

            return model;
        }
    }
}