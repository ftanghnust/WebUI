
var subWid;

$(function () {

    subWid = frxs.getUrlParam("subWid");

    init();
    initGrid();
});

function init() {
    $("#aSearch").click(function () {
        initGrid();
    });
}

function initGrid() {
    $('#gridproduct').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../StockAdjDetail/GetProductList',          //Aajx地址
        sortName: 'ProductName',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 30,                       //每页条数
        pageList: [10, 30 , 50, 1000],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: false,
        selectOnCheck: false,
        onClickRow: function (rowIndex, rowData) {
            $('#gridproduct').datagrid('clearSelections');
            $('#gridproduct').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function (index, rowData) {
            saveData();//商品列表双击事件
        },
        queryParams: {
            //查询条件
            SKU: $.trim($("#SKU").val()),
            ProductName: $.trim($("#ProductName").val()),
            BarCode: $.trim($("#BarCode").val()),
            SubWid: subWid
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[

            { title: 'ProductId', field: 'ProductId', width: 30, align: 'center', sortable: true, hidden: true },
            { title: '商品编号', field: 'SKU', width: 65 },
            { title: '商品名称', field: 'ProductName', width: 150 },
            { title: '采购单位', field: 'BigUnit', width: 60 },
            { title: '采购价格', field: 'UnitBuyPrice', width: 60 },//不是BuyPrice
            { title: '包装数', field: 'BigPackingQty', width: 50 },
            { title: '国际条码', field: 'BarCode', width: 100 },
            { title: '库存数量', field: 'StockQty', width: 55 },
            { title: '库存单位', field: 'Unit', width: 55 },
            { title: '配送价格', field: 'UnitSalePrice', width: 60 },//不是SalePrice
            {
                field: 'opt',
                title: '基本分类',
                align: 'left',
                width: 200,
                formatter: function (value, rec) {
                    var str = rec.CategoryName1 + " -> " + rec.CategoryName2 + " -> " + rec.CategoryName3;
                    return frxs.formatText(str);
                }//
            },
            { title: '主供应商ID', field: 'VendorID', width: 10, hidden: true },
            { title: '主供应商编号', field: 'VendorCode', width: 10, hidden: true },
            { title: '主供应商名称', field: 'VendorName', width: 10, hidden: true },
            { title: 'CategoryId1', field: 'CategoryId1', width: 10, hidden: true },
            { title: 'CategoryName1', field: 'CategoryName1', width: 10, hidden: true },
            { title: 'CategoryId2', field: 'CategoryId2', width: 10, hidden: true },
            { title: 'CategoryName2', field: 'CategoryName2', width: 10, hidden: true },
            { title: 'CategoryId3', field: 'CategoryId3', width: 10, hidden: true },
            { title: 'CategoryName3', field: 'CategoryName3', width: 10, hidden: true }
        ]],
        toolbar: []
    });
}

function saveData() {
    var rows = $('#gridproduct').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    } else {
        var row = rows[0];
        //alert(row);
        window.frameElement.wapi.reloadProduct(row);
        window.frameElement.wapi.focus();
        frxs.pageClose();
    }
}



//提交和关闭快捷键
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData();
    }
    else if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();
