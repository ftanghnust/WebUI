$(function () {

    //重置按钮事件
    $("#aReset").click(function () {
        $("#searchForm").form("clear");
    });
    
    initGrid();
    
    //grid高度改变
    gridresize();
});

//搜索
function search() {
    //实现刷新栏目中的数据
    initGrid();
}

//初始化查询
function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../DaySettlement/DaySettlementDetilData',          //Aajx地址
        idField: 'ID',                  //主键
        pageSize: 50,                       //每页条数
        pageList: [30, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        showFooter: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            totalCalculate();
        },
        queryParams: {
            SKU: $.trim($("#SKU").val()),
            ProductName: $.trim($("#ProductName").val()),
            RefSet_ID: frxs.getUrlParam("Id")
        },
        frozenColumns: [[
            { title: '商品编码', field: 'SKU', width: 100, align: "center" },
            { title: '商品名称', field: 'ProductName', width: 220 }
        ]],
        columns: [[
            { title: '库存单位', field: 'Unit', width: 80, align: "center" },
            {
                title: '期初库存数量', field: 'BeginQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '期初库存金额', field: 'BeginAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '采购数量', field: 'BuyQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '采购金额', field: 'BuyAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '采购退货数量', field: 'BuyBackQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '采购退货金额', field: 'BuyBackAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '销售数量', field: 'SaleQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '销售金额', field: 'SaleAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '销售退货数量', field: 'SaleBackQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '销售退货金额', field: 'SaleBackAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '盘盈数量', field: 'AdjGainQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '盘盈金额', field: 'AjgGginAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '盘亏数量', field: 'AdjLossQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '盘亏金额', field: 'AjgLosssAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '库存数量', field: 'StockQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '库存金额', field: 'StockAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '期末库存数量', field: 'EndQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '期末库存金额', field: 'EndStockAmt', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '差异数量', field: 'EndDiffQty', width: 100, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '差异金额', field: 'EndDiffStockAmt', width: 160, align: "right", formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            }
        ]]
    });
}


//总计
function totalCalculate() {
    var gridData = $("#grid").datagrid("getData");

    $('#grid').datagrid('reloadFooter', [
       {
           "SKU": "当前页合计：",
           "AdjGainQty": gridData.settCurrentSum.AdjGainQty,
           "AdjLossQty": gridData.settCurrentSum.AdjLossQty,
           "AjgGginAmt": gridData.settCurrentSum.AjgGginAmt,
           "AjgLosssAmt": gridData.settCurrentSum.AjgLosssAmt,
           "BeginAmt": gridData.settCurrentSum.BeginAmt,
           "BeginQty": gridData.settCurrentSum.BeginQty,
           "BuyAmt": gridData.settCurrentSum.BuyAmt,
           "BuyBackAmt": gridData.settCurrentSum.BuyBackAmt,
           "BuyBackQty": gridData.settCurrentSum.BuyBackQty,
           "BuyQty": gridData.settCurrentSum.BuyQty,
           "EndDiffQty": gridData.settCurrentSum.EndDiffQty,
           "EndDiffStockAmt": gridData.settCurrentSum.EndDiffStockAmt,
           "EndQty": gridData.settCurrentSum.EndQty,
           "EndStockAmt": gridData.settCurrentSum.EndStockAmt,
           "SaleAmt": gridData.settCurrentSum.SaleAmt,
           "SaleBackAmt": gridData.settCurrentSum.SaleBackAmt,
           "SaleBackQty": gridData.settCurrentSum.SaleBackQty,
           "SaleQty": gridData.settCurrentSum.SaleQty,
           "StockAmt": gridData.settCurrentSum.StockAmt,
           "StockQty": gridData.settCurrentSum.StockQty
       },
       {
           "SKU": "总合计：",
           "AdjGainQty": gridData.settTotalSum.SumAdjGainQty,
           "AdjLossQty": gridData.settTotalSum.SumAdjLossQty,
           "AjgGginAmt":  gridData.settTotalSum.SumAjgGginAmt,
           "AjgLosssAmt": gridData.settTotalSum.SumAjgLosssAmt,
           "BeginAmt": gridData.settTotalSum.SumBeginAmt,
           "BeginQty": gridData.settTotalSum.SumBeginQty,
           "BuyAmt": gridData.settTotalSum.SumBuyAmt,
           "BuyBackAmt": gridData.settTotalSum.SumBuyBackAmt,
           "BuyBackQty": gridData.settTotalSum.SumBuyBackQty,
           "BuyQty": gridData.settTotalSum.SumBuyQty,
           "EndDiffQty": gridData.settTotalSum.SumEndDiffQty,
           "EndDiffStockAmt": gridData.settTotalSum.SumEndDiffStockAmt,
           "EndQty": gridData.settTotalSum.SumEndQty,
           "EndStockAmt": gridData.settTotalSum.SumEndStockAmt,
           "SaleAmt": gridData.settTotalSum.SumSaleAmt,
           "SaleBackAmt": gridData.settTotalSum.SumSaleBackAmt,
           "SaleBackQty": gridData.settTotalSum.SumSaleBackQty,
           "SaleQty": gridData.settTotalSum.SumSaleQty,
           "StockAmt": gridData.settTotalSum.SumStockAmt,
           "StockQty": gridData.settTotalSum.SumStockQty
       }
    ]);
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}