﻿
var productCode;
var vendorId;
var subWID;

$(function () {

    productCode = frxs.getUrlParam("productCode");
    //判断输入的是编码还是商品名称
    if(productCode) {
        var onebyte = productCode.substring(0, 1);
        if(isNaN(onebyte)&&onebyte!="赠") {
            $("#type").combobox("setValue", "ProductName");
        }
    }
    vendorId = frxs.getUrlParam("vendorId");
    subWID = frxs.getUrlParam("SubWID");
    $("#productCode").val(productCode);

    $("#productCode").focus();

    //加载数据
    loadgrid();
    //高度自适应
    gridresize();

    //事件绑定
    eventBind();

    productCodeKeypress();
});


function loadgrid() {

    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../Common/GetBuyProductInfo?vendorId=' + vendorId + "&SubWID=" + subWID,          //Aajx地址
        sortName: 'ProductId',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 30,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        singleSelect: true,                 //单选
        //设置点击行为单选，点击行中的复选框为多选
        //checkOnSelect: true,
        //selectOnCheck: true,
        onClickRow: function (rowIndex) {
            //$('#grid').datagrid('clearSelections');
            //$('#grid').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function (index, rowData) {

            window.frameElement.wapi.backFillProduct(rowData);
            frxs.pageClose();
        },
        queryParams: {
            //查询条件
            Value: $.trim($("#productCode").val()),
            Type: $("#type").combobox("getValue")
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '商品编码', field: 'SKU', width: 80 }
            //冻结列
        ]],
        columns: [[
            { title: '商品名称', field: 'ProductName', width: 200 },
            { title: '单位', field: 'BuyUnit', width: 70, align: 'center' },
            { title: '包装数', field: 'Unit', width: 70, align: 'center' },
            { title: '库存数量', field: 'WStock', width: 70, align: 'center' },
            {
                title: '采购价', field: 'BuyPrice', width: 80, formatter: function (value) {
                    value = value ? value : "0";
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '配送价', field: 'SalePrice', width: 80, formatter: function (value) {
                    value = value ? value : "0";
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '国际条码', field: 'BarCode', width: 120 },
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 100 },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 100 },
            { title: 'SaleUnitPrice', field: 'SaleUnitPrice', hidden: true, width: 100 },
            { title: 'OldBuyPrice', field: 'OldBuyPrice', hidden: true, width: 100 }
        ]]
    });


}


//事件绑定
function eventBind() {

    //绑定上下键事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        var index;
        //向下
        if (ev.keyCode == 40) {
            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            var selRow = $("#grid").datagrid("getSelected");
            if (selRow != null) {
                index = $("#grid").datagrid("getRowIndex", $("#grid").datagrid("getSelected"));
                index = index + 1;
            } else {
                index = 0;
            }
            if (index > $("#grid").datagrid("getRows").length - 1) {
                index = 0;
            }
            $('#grid').datagrid('clearSelections');
            $("#grid").datagrid("selectRow", index);
        }

        //向上
        if (ev.keyCode == 38) {
            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            var selRow = $("#grid").datagrid("getSelected");
            if (selRow != null) {
                index = $("#grid").datagrid("getRowIndex", $("#grid").datagrid("getSelected"));
                index = index - 1;
            } else {
                index = $("#grid").datagrid("getRows").length - 1;
            }
            if (index < 0) {
                index = $("#grid").datagrid("getRows").length - 1;
            }
            $('#grid').datagrid('clearSelections');
            $("#grid").datagrid("selectRow", index);
        }

        //回车事件
        if (ev.keyCode == 13) {
            var selRow = $("#grid").datagrid("getSelected");
            if (selRow != null) {
                window.frameElement.wapi.backFillProduct(selRow);
                frxs.pageClose();
            } else {
                //$.messager.alert("提示", "没有选中行！", "info");
            }
        }

        //ESC按键
        if (ev.keyCode == 27) {
            var parentRow = window.frameElement.wapi.$("#grid").datagrid("getSelected");
            var parentIndex = window.frameElement.wapi.$("#grid").datagrid("getRowIndex", parentRow);
            window.frameElement.wapi.onClickCell(parentIndex, "SKU");

            frxs.pageClose();
        }

    };
}


//回车事件
function productCodeKeypress() {
    $("#productCode").keypress(function (e) {
        if (e.keyCode == 13) {
            loadgrid();
        }
    });

}





//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 20);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 5,
        height: h
    });
}
