﻿$(function () {
    init();
    //grid绑定
    initGrid();
    initSubGrid();
    //grid高度改变
    gridresize();
});

function init() {
    $("#aSearch").click(function () {
        initGrid();
    });
}

function initGrid() {
    $('#main_datagrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        border:false,
        url: '../WProductAdjShelf/GetProductList',          //Aajx地址
        sortName: 'ProductName',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 50,                       //每页条数
        pageList: [50, 100, 200],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: false,
        selectOnCheck: false,
        onClickRow: function (rowIndex, rowData) {
            MainGridClickRow(rowIndex, rowData);
            $(this).datagrid('unselectRow', rowIndex);

            //var subselect = false;
            //var rows = $('#sub_datagrid').datagrid('getRows');
            //if (rows != null && rows.length > 0) {
            //    for (var i = 0; i < rows.length; i++) {
            //        if (rowData.ProductId == rows[i].ProductId) {
            //            subselect = true;
            //            break;
            //        }
            //    }
            //}
            //if (subselect) {
            //    $(this).datagrid('selectRow', rowIndex);
            //} else {
            //    $(this).datagrid('unselectRow', rowIndex);
            //}
        },
        queryParams: {
            //查询条件
            searchType: $("#searchType").val(),
            searchKey: $.trim($("#searchKey").val()),
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: 'ProductId', field: 'ProductId', width: 130, align: 'center', sortable: true, hidden: true },
            { title: 'WProductId', field: 'WProductId', width: 100, hidden: true },
            { title: '原平台费率', field: 'ShopAddPerc', width: 100, hidden: true },
            { title: '商品编号', field: 'SKU', width: 100, align: 'center' },
            { title: '名称', field: 'ProductName', width: 200 },
            { title: '配送价格', field: 'SalePrice', width: 100, align: 'right' },
            { title: '配送单位', field: 'BigUnit', width: 60, align: 'center' },
            { title: '包装数', field: 'BigPackingQty', width: 60, align: 'center' },
            { title: 'ShelfID', field: 'ShelfID', hidden: true },
            { title: '货位编码', field: 'ShelfCode', width: 60, align: 'center' },    
            { title: '国际条码', field: 'BarCode', width: 100, align: 'center' },
            { title: '库存单位', field: 'Unit', width: 60, align: 'center' }
        ]],
        toolbar: []
    });
}

function initSubGrid() {
    $('#sub_datagrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        sortName: 'ProductName',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 50,                       //每页条数
        pageList: [50, 100, 200],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: false,
        selectOnCheck: false,
        onClickRow: function (rowIndex, rowData) {
            SubGridClickRow(rowIndex, rowData);
            $(this).datagrid('unselectRow', rowIndex);
        },
        queryParams: {
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: 'ProductId', field: 'ProductId', width: 130, align: 'center', sortable: true, hidden: true },
            { title: 'WProductId', field: 'WProductId', width: 100, hidden: true },
            { title: '商品编号', field: 'SKU', width: 100, align: 'center' },
            { title: '名称', field: 'ProductName', width: 260 },
            { title: '包装数', field: 'BigPackingQty', width: 100, align: 'center' },
            { title: 'ShelfID', field: 'ShelfID', hidden: true },
            { title: '货位编码', field: 'ShelfCode', width: 100, align: 'center' },
            { title: '国际条码', field: 'BarCode', width: 100, align: 'center' },
            { title: '库存单位', field: 'Unit', width: 100, align: 'center' }

        ]],
        toolbar: []
    });
}

function saveData() {
    window.frameElement.wapi.reloadProducts($('#sub_datagrid'));
    window.frameElement.wapi.focus();
    frxs.pageClose();
}
function gridresize() {
    var h = ($(window).height()  - 38);
    $('#main_datagrid').datagrid('resize', {
        height: h
    });
    $('#sub_datagrid').datagrid('resize', {
        height: h
    });
}

//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData();//弹窗提交

    }
    else if (e.keyCode == 27) {

        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭


    }
});
window.focus();