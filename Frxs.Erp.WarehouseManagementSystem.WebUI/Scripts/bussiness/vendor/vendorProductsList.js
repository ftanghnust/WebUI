/** 已废弃 **/

var VendorId;
var dialogWidth = 850;
var dialogHeight = 600;

$(function () {

    VendorId = frxs.getUrlParam("vendorId");

    //grid绑定
    initGrid();

    //grid高度改变
    gridresize();

});


//初始化查询
function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../ProductsVendor/GetVendorProductsList?VendorID=' + VendorId,          //Aajx地址
        sortName: 'SKU',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '商品编号', field: 'SKU', width: 80, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 220 },
            { title: '单位', field: 'Unit', width: 50 },
            { title: '包装数', field: 'BigPackingQty', width: 50 },
            { title: '国际条码', field: 'BarCode', width: 100 },
            {
                title: '基本分类', field: 'CategoryNameStr', width: 200, formatter: function (value, rec) {
                    var categoryNameStr = "";
                    if (rec.CategoryName1 != "" && rec.CategoryName1 != undefined) {
                        categoryNameStr = rec.CategoryName1;
                    }
                    if (rec.CategoryName2 != "" && rec.CategoryName2 != undefined) {
                        categoryNameStr += ">>" + rec.CategoryName2;
                    }
                    if (rec.CategoryName3 != "" && rec.CategoryName3 != undefined) {
                        categoryNameStr += ">>" + rec.CategoryName3;
                    }
                    return categoryNameStr;
                }
            },
            {
                title: '设置主供应商', field: 'IsMaster', width: 80, align: 'center', formatter: function (value, row, index) {
                    if (row.IsMaster == 1) {
                        return '<input type="checkbox" name="IsMasterStr" checked="checked">';
                    }
                    else {
                        return '<input type="checkbox" name="IsMasterStr">';
                    }
                }
            },
            { title: '商品编号', field: 'ProductId', hidden: true, width: 100 }
        ]],
        toolbar: [{
            id: 'btnAddProduct',
            text: '添加',
            iconCls: 'icon-add',
            handler: addProduct
        }, '-',
        {
            id: 'btnDelProduct',
            text: '移除',
            iconCls: 'icon-cancel',
            handler: delCheckProduct
        }]
    });
}

//添加商品到供应商供应关系
function addProduct() {
    var thisdlg = frxs.dialog({
        title: "添加商品到供应商供应关系",
        url: "/ProductLimit/ProductLimitProduct",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '提交',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
            }
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                thisdlg.dialog("close");
            }
        }]
    });
}

//从供应商供应关系中移除商品
function delCheckProduct() {
    var rows = $('#grid').datagrid("getSelections");
    if (rows.length == 0) {
        $.messager.alert('提示', "请选择要移除的商品", "info");
        return;
    }
    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            var copyRows = [];
            for (var j = 0; j < rows.length; j++) {
                copyRows.push(rows[j]);
            }
            for (var i = 0; i < copyRows.length; i++) {
                var index = $('#grid').datagrid('getRowIndex', copyRows[i]);
                $('#grid').datagrid('deleteRow', index);
            }
        }
    });
}

//回调函数,通过选择返回的数据
function reloadProducts(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (!rowProductExist(row["ProductId"])) {
            row["ProductId"] = row["ProductId"];
            row["IsMaster"] = 0;
            $('#grid').datagrid('appendRow', row);
        }
    }
}

//判断是否有重复数据，有重复数据去重
function rowProductExist(id) {
    var rows = $('#grid').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["ProductId"] == id) {
            return true;
        }
    }
    return false;
}
function saveData() {
    var rows = $('#grid').datagrid('getRows');
    $("input[name='IsMasterStr']").each(function (i) {
        $('#grid').datagrid("getRows", i);
        var curRow = rows[i];
        if ($(this).prop("checked")) {
            curRow.IsMaster = 1;
        } else {
            curRow.IsMaster = 0;
        }
        $('#grid').datagrid('updateRow',
            {
                index: i,
                row: curRow
            });
    });

    var newrows = $('#grid').datagrid('getRows');
    var jsonObject = JSON.stringify(newrows);
    var data = {
        jsonObject: "" + jsonObject + "",
        vendorID: VendorId
    };

    window.frameElement.wapi.addProductsVendorList(data);
    frxs.pageClose();
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