using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.StockCheck;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility;
using System.Text;
using System.IO;
using Frxs.Platform.Utility.Excel;
using System.Data;
using Frxs.Platform.Utility.Map;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    public class StockAdjDetailController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult StockAdjDetailAddOrEdit(string id)
        {
            return View();
        }

        public ActionResult StockAdjDetailProducts()
        {
            return View();
        }

        public ActionResult ImportStockAdjDetail()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetStockAdjDetailPageData(StockAdjDetailQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailListGetRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort,
                    AdjID = cpm.AdjID,
                    SearchValue = cpm.SearchValue,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });
                if (resp != null && resp.Flag == 0)
                {
              
                    IList<StockAdjDetailExt> itemList = new List<StockAdjDetailExt>();
                    foreach (var stockAdjDetail in resp.Data.ItemList)
                    {
                        StockAdjDetailExt model = AutoMapperHelper.MapTo<StockAdjDetailExt>(stockAdjDetail);
                        model.AdjAmt = stockAdjDetail.AdjAmt.ToString("0.0000"); //调整金额
                        model.AdjQty = stockAdjDetail.AdjQty.ToString("0.00"); //调整数量
                        model.UnitQty = stockAdjDetail.UnitQty.ToString("0.00"); //库存单位数量
                        model.BuyPrice = stockAdjDetail.BuyPrice.ToString("0.0000"); //采购单价
                        model.SalePrice = stockAdjDetail.SalePrice.ToString("0.0000"); //配送单价
                        itemList.Add(model);
                    }
                    var obj = new { total = resp.Data.TotalRecords, rows = itemList };
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 得到盘盈盘亏单据明细的统计数据
        /// 统计 商品最小单位总数和商品总金额 
        /// </summary>
        /// <param name="adjID">盘盈盘亏单据编号</param>
        /// <returns> 商品最小单位总数和商品总金额 </returns>
        public ActionResult GetStockAdjDetailsCalculate(string adjID)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjQtyAmtSumGetRequest()
                {
                    AdjID = adjID,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });
                if (resp != null && resp.Flag == 0 && resp.Data != null)
                {
                    StockAdjDetailCalculate model = AutoMapperHelper.MapTo<StockAdjDetailCalculate>(resp.Data);
                    jsonStr = model.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);

        }

        public ActionResult GetStockAdjDetail(string id)
        {
            StockAdjDetailModel model = new StockAdjDetailModel().GetStockAdjDetail(id);
            return model != null ? Content(model.ToJsonString()) : Content(String.Empty);
        }




        [ValidateInput(false)]
        public ActionResult SaveStockAdjDetail(StockAdjDetailModel model)
        {
            var result = new StockAdjDetailModel().SaveStockAdjDetail(model, UserIdentity.UserId, UserIdentity.UserName);
            return Content(result.ToJsonString());
        }

        public ActionResult DeleteStockAdjDetail(string ids)
        {
            string result;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    int rows = new StockAdjDetailModel().DeleteStockAdjDetail(ids);
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

        public ActionResult ClearStockAdjDetail(string id)
        {
            string result;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int rows = new StockAdjDetailModel().ClearStockAdjDetail(id);
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
                        Info = "盘点调整ID不正确"
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

        public ActionResult GetProductList(StockAdjDetailProductQuery cpm)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp =
                    serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.
                                              FrxsErpProductWProductsAddedListForStockGetRequest()
                                              {
                                                  PageIndex = cpm.page,
                                                  PageSize = cpm.rows,
                                                  SortBy = cpm.sort,
                                                  WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                                                  SKU = cpm.SKU,
                                                  ProductName = cpm.ProductName,
                                                  BarCode = cpm.BarCode
                                              });
                if (resp != null && resp.Flag == 0)
                {
                    var widList = new List<int>();
                    widList.Add(WorkContext.CurrentWarehouse.Parent.WarehouseId);
                    var pdidList = resp.Data.ItemList.Select(t => t.ProductId).ToList();
                    var orderServiceCenter = WorkContext.CreateOrderSdkClient();
                    //var OrderStockQty = OrderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyQueryRequest()
                    //{
                    //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    //    //SubWID = 211,
                    //    PDIDList = PDIDList
                    //});
                    
                    var orderStockQty = orderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyListQueryRequest()
                    {
                        WIDList = new List<int>() { WorkContext.CurrentWarehouse.Parent.WarehouseId },

                        PDIDList = pdidList
                    });
                    foreach (var item in resp.Data.ItemList)
                    {
                        item.StockQty = 0;
                        if (orderStockQty.Data != null)
                        {
                            if (cpm.SubWid != null && cpm.SubWid.Value > 0)
                            {
                                var temp = orderStockQty.Data.FirstOrDefault(t => t.PID == item.ProductId && cpm.SubWid == t.SubWID);
                                if (temp != null)
                                {
                                    item.StockQty = temp.StockQty;
                                }
                            }
                            else
                            {
                                var temp = orderStockQty.Data.FirstOrDefault(t => t.PID == item.ProductId);
                                if (temp != null)
                                {
                                    item.StockQty = temp.StockQty;
                                }
                            }

                        }
                    }
                    var obj = new {total = resp.Data.TotalRecords, rows = resp.Data.ItemList};
                    result = obj.ToJsonString();
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

        public ActionResult GetProductBySku(string sku)
        {
            var result = String.Empty;
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsAddedListForStockGetRequest()
            {
                PageIndex = 1,
                PageSize = 1,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                SKU = sku,
                ProductName = String.Empty,
                BarCode = String.Empty
            });
            if (resp != null && resp.Flag == 0)
            {
                if (resp.Data.ItemList != null && resp.Data.ItemList.Count > 0)
                {
                    var widList = new List<int>();
                    widList.Add(WorkContext.CurrentWarehouse.Parent.WarehouseId);
                    var pdidList = resp.Data.ItemList.Select(t => t.ProductId).ToList();
                    var orderServiceCenter = WorkContext.CreateOrderSdkClient();
                    var orderStockQty = orderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyListQueryRequest()
                    {
                        WIDList = new List<int>() { WorkContext.CurrentWarehouse.Parent.WarehouseId },
                        PDIDList = pdidList
                    });
                    foreach (var item in resp.Data.ItemList)
                    {
                        item.StockQty = 0;
                        if (orderStockQty.Data != null)
                        {
                            var temp = orderStockQty.Data.FirstOrDefault(t => t.PID == item.ProductId);
                            if (temp != null)
                            {
                                item.StockQty = temp.StockQty;
                            }
                        }
                    }
                    result = resp.Data.ItemList[0].ToJsonString();
                }
            }
            return Content(result);
        }

        public Dictionary<int, decimal> GetProductStockQty(List<int> PDIDList, List<int> WIDList)
        {
            var dic = new Dictionary<int, decimal>();
            var serviceCenter = WorkContext.CreateOrderSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyListQueryRequest()
            {
                PDIDList = PDIDList,
                WIDList = WIDList
            });
            if (resp != null && resp.Flag == 0)
            {
                foreach (var product in resp.Data)
                {
                    dic.Add(product.PID, product.StockQty);
                }
            }
            return dic;
        }

        /// <summary>
        /// 导入明细
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        // [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadStockAdjDetail(HttpPostedFileBase fileData, string folder)
        {
            var resultData = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = string.Format("导入失败！")
            };
            if (fileData != null)
            {
                try
                {
                    var adjID = ControllerContext.HttpContext.Request.Form["adjID"].ToString();
                    if (String.IsNullOrEmpty(adjID))
                    {
                        resultData.Info = "盘点调整单获取失败！";
                        return Content(resultData.ToJsonString());
                    }
                    var stockAdjModel = new StockAdjModel().GetStockAdj(adjID);
                    if (stockAdjModel.SubWID == 0)
                    {
                        resultData.Info = "盘点调整单子机构获取失败！";
                        return Content(resultData.ToJsonString());
                    }
                    var subWid = stockAdjModel.SubWID;
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
                    string fileUrl = filePath + string.Format("/{0}.xlsx", guid);
                    fileData.SaveAs(fileUrl);
                    //string filePath = Server.MapPath(cpm.folder) + string.Format("/{0}.xlsx", cpm.ImportGuid);
                    //ExcelHelper excelOption = new ExcelHelper(fileUrl);
                    //DataTable dt = excelOption.GetDataBySheet("盘点明细");
                    DataTable dt = NpoiExcelhelper.RenderFromExcel(fileUrl);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Columns.Count < 3 || dt.Columns[0].ColumnName != "商品编码" || dt.Columns[1].ColumnName != "数量" || dt.Columns[2].ColumnName != "备注")
                        {
                            resultData.Info = "明细导入失败，Excel数据格式是否正确";
                            return Content(resultData.ToJsonString());
                        }
                        //dt.Rows.RemoveAt(0);
                        var rowIndex = 1;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row[0] == null || String.IsNullOrEmpty(row[0].ToString()))
                            {
                                resultData.Info = "Excel第 " + rowIndex + " 行商品编码为空！";
                                return Content(resultData.ToJsonString());
                            }
                            if (row[1] == null || String.IsNullOrEmpty(row[1].ToString()))
                            {
                                resultData.Info = "Excel第 " + rowIndex + " 行数量为空！";
                                return Content(resultData.ToJsonString());
                            }
                            decimal result;
                            var check = decimal.TryParse(row[1].ToString(), out result);
                            if (!check)
                            {
                                resultData.Info = string.Format("Excel第 {0} 行数量不正确！",rowIndex);
                                return Content(resultData.ToJsonString());
                            }
                            else
                            {
                                if (result > 99999999)
                                {
                                    resultData.Info = "明细导入失败，库存数不能大于99999999！";
                                    return Content(resultData.ToJsonString());
                                }
                            }
                            rowIndex += 1;
                        }
                        var stockAdjDetailImportModels = new List<StockAdjDetailImportModel>();
                        foreach (DataRow row in dt.Rows)
                        {
                            var stockAdjDetailImportModel = new StockAdjDetailImportModel()
                            {
                                ID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString() + Guid.NewGuid().ToString("N"),
                                SKU = row[0].ToString(),
                                AdjQty = decimal.Parse(row[1].ToString()),
                                Remark = row[2].ToString()
                            };
                            stockAdjDetailImportModels.Add(stockAdjDetailImportModel);
                        }
                        var importDetailList = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<StockAdjDetailImportModel, Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailImportRequest.StockAdjDetailImportModel>(stockAdjDetailImportModels);

                        //分批次导入
                        var splitList = SplitList<Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailImportRequest.StockAdjDetailImportModel>(importDetailList.ToList(), 200);
                        var batchIndex = 1;
                        var batchCount = splitList.Count;
                        foreach (var splitItem in splitList)
                        {
                            batchIndex = batchIndex.Equals(batchCount) ? -1 : batchIndex;//最后1批为-1
                            var result = BatchImport(batchIndex, adjID, WorkContext.CurrentWarehouse.Parent.WarehouseId, subWid, splitItem);
                            if (result.Flag == ConstDefinition.FLAG_FAIL)
                            {
                                //其中1批失败退出循环
                                resultData = result;
                                break;
                            }
                            if (batchIndex.Equals(-1))//最后1批接口返回作为最终结果
                            {
                                resultData = new ResultData
                                {
                                    Flag = result.Flag,
                                    Info = result.Info
                                };
                                return Content(resultData.ToJsonString());
                            }
                            else
                            {
                                batchIndex += 1;
                            }
                            //System.Threading.Thread.Sleep(500);
                        }

                        //不分批次导入
                        //var serviceCenter = WorkContext.CreateOrderSdkClient();
                        //serviceCenter.SetTimeout(100000);
                        //var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailImportRequest()
                        //{
                        //    AdjID = adjID,
                        //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        //    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        //    SubWID = subWID,
                        //    ImportList = ImportDetailList
                        //});
                        //if (resp != null && resp.Flag == 0)
                        //{
                        //    resultData = new ResultData
                        //    {
                        //        Flag = ConstDefinition.FLAG_SUCCESS,
                        //        Info = string.Format("盘点明细导入成功！")
                        //    };
                        //}
                        //else
                        //{
                        //    resultData.Info = resp.Info;
                        //}

                        return Content(resultData.ToJsonString());
                    }
                    return Content(resultData.ToJsonString());
                }
                catch (Exception ex)
                {
                    resultData = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        //Info = string.Format("盘点明细导入失败：" + ex.Message)
                        Info = string.Format("明细导入服务端异常！<br />{0}", ex.Message)
                    };
                    return Content(resultData.ToJsonString());
                }
            }
            else
            {
                return Content(resultData.ToJsonString());
            }
        }

        public ResultData BatchImport(int batchIndex, string adjID, int wid, int subWid, IList<Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailImportRequest.StockAdjDetailImportModel> ImportDetailList)
        {
            var resultData = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = string.Format("导入失败！")
            };
            var serviceCenter = WorkContext.CreateOrderSdkClient();
            serviceCenter.SetTimeout(600 * 1000);//最后一次调用接口批量事务写数据库的时间会比较久
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailImportRequest()
            {
                BatchIndex = batchIndex,
                AdjID = adjID,
                WID = wid,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                SubWID = subWid,
                ImportList = ImportDetailList
            });
            if (resp != null && resp.Flag == 0)
            {
                resultData = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = string.Format("盘点明细导入成功！")
                };
            }
            else
            {
                resultData.Info = (resp != null && !String.IsNullOrEmpty(resp.Info)) ? resp.Info : "明细导入未得到服务端的响应！";
            }
            return resultData;
        }

        /// <summary>
        /// 分割List 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<List<T>> SplitList<T>(List<T> list, int pageSize)
        {
            int listSize = list.Count(); // list的大小  
            int page = (listSize + (pageSize - 1)) / pageSize;// 页数  
            List<List<T>> listArray = new List<List<T>>();// 创建list数组,用来保存分割后的list  
            for (int i = 0; i < page; i++)
            {
                // 按照数组大小遍历  
                List<T> subList = new List<T>(); // 数组每一位放入一个分割后的list  
                for (int j = 0; j < listSize; j++)
                {
                    // 遍历待分割的list  
                    int pageIndex = ((j + 1) + (pageSize - 1)) / pageSize;// 当前记录的页码(第几页)  
                    if (pageIndex == (i + 1))
                    {
                        // 当前记录的页码等于要放入的页码时  
                        subList.Add(list[j]); // 放入list中的元素到分割后的list(subList)  
                    }
                    if ((j + 1) == ((j + 1) * pageSize))
                    {// 当放满一页时退出当前循环  
                        break;
                    }
                }
                listArray.Add(subList);// 将分割后的list放入对应的数组的位中  
            }
            return listArray;
        }
    }
}
