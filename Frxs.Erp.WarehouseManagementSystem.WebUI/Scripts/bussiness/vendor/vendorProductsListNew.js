
var VendorId;
var VendorName;
var dialogWidth = 850;
var dialogHeight = 600;

$(function () {
    VendorId = frxs.getUrlParam("vendorId");
    VendorName = frxs.getUrlParam("vendorName");

    $("#lblVendorName").html(VendorName);

    //grid绑定
    initGrid();

    //新增
    $("#btnAdd").click(function () {
        addProducts();
    });

    //删除
    $("#btnDel").click(function () {
        delVendorProducts();
    });

    //设置主供应商
    $("#btnSave").click(function () {
        setMasterVendor();
    });

    //查找
    $("#aSearch").click(function () {
        searchgrid();
    });

    //grid高度改变
    gridresize();

});


//初始化查询
function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../VerdorProducts/GetVendorProductsList?VendorID=' + VendorId,          //Aajx地址
        //sortName: 'SKU',                 //排序字段
        //sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
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
        queryParams: {
            VendorID: VendorId,
            KeyWord: $.trim($('#KeyWord').val()),
            IsMaster: $('#SelIsMaster').combobox('getValue')
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '商品编码', field: 'SKU', width: 60, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 220, formatter: frxs.formatText },
            { title: '单位', field: 'Unit', width: 50, align: 'center' },
            { title: '包装数', field: 'BigPackingQty', width: 50, align: 'center' },
            { title: '国际条码', field: 'BarCode', width: 105, formatter: frxs.formatText },
            {
                title: '是否主供应商', field: 'IsMaster', width: 80, align: 'center', formatter: function (value) {
                    if (value == 1) {
                        return '是';
                    }
                    else {
                        return '否';
                    }
                }
            },
            {
                title: '基本分类', field: 'CategoryNameStr', width: 210, formatter: function (value, rec) {
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
                    return frxs.formatText(categoryNameStr);
                }
            },
            { title: '商品编号', field: 'ProductId', hidden: true, width: 100 }
        ]],
        toolbar: '#toolbar'
    });
}

//设置主供应商
function setMasterVendor() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    }
    else {
        var idStr = "";
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].IsMaster == 1) {
                $.messager.alert('提示', "不能设置存在主供应关系的商品[" + rows[i].ProductName + "]", "info");
                return;
            }
            if (rows[i].IsMaster == 0) {
                idStr += ("" + rows[i].ProductId + ",");
            }
        }

        if (idStr == "") {
            $.messager.alert('提示', "没有选中一条非主供应商商品关系记录", "info");
            return;
        }

        $(this).attr("disabled", "disabled");

        window.top.$.messager.confirm("操作提示", "是否设置为主供应商？", function (isdata) {
            if (isdata) {
                var load = frxs.loading("正在保存...");
                $.ajax({
                    url: "../VerdorProducts/SetMasterVendor",
                    type: "post",
                    dataType: "json",
                    data: {
                        vendorID: VendorId,
                        prodcutids: idStr
                    },
                    success: function (data) {
                        load.close();
                        if (data.Flag == "SUCCESS") {
                            $.messager.alert('提示', "设置主供应商成功", "info", function () {
                                $("#grid").datagrid("reload");
                            });
                        }
                        else {
                            window.top.$.messager.alert("提示", data.Info, "info");
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            window.top.$.messager.alert("提示", textStatus, "info");
                        } else if (errThrown) {
                            window.top.$.messager.alert("提示", errThrown, "info");
                        } else {
                            window.top.$.messager.alert("提示", "出现错误", "info");
                        }
                    }
                });
            }
        });

        $(this).removeAttr("disabled");

    }

}

//搜索
function searchgrid() {
    initGrid();
}

//添加商品到供应商供应关系
function addProducts() {

    $(this).attr("disabled", "disabled");
    var thisdlg = frxs.dialog({
        title: "添加商品到供应商供应关系",
        url: "../ProductLimit/ProductLimitProduct",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title=【ALT+S】>保存</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });

    $(this).removeAttr("disabled");
}

//从供应商供应关系中移除商品
function delVendorProducts() {
    var rows = $('#grid').datagrid("getSelections");
    if (rows.length == 0) {
        $.messager.alert('提示', "请选择要移除的商品", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].IsMaster == 1) {
            $.messager.alert('提示', "不能删除存在主供应关系的商品[" + rows[i].ProductName + "]", "info");
            return;
        }
        if (rows[i].IsMaster == 0) {
            idStr += ("" + rows[i].ProductId + ",");
        }
    }

    if (idStr == "") {
        $.messager.alert('提示', "没有选中一条非主供应商商品关系记录", "info");
        return;
    }
    
    $(this).attr("disabled", "disabled");

    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            var load = frxs.loading("正在保存...");
            $.ajax({
                url: "../VerdorProducts/DelVendorProduct",
                type: "post",
                dataType: "json",
                data: {
                    vendorID: VendorId,
                    prodcutids: idStr
                },
                success: function (data) {
                    load.close();
                    if (data.Flag == "SUCCESS") {
                        $.messager.alert('提示', "供应商商品关系移除成功", "info", function () {
                            $("#grid").datagrid("reload");
                        });
                    }
                    else {
                        window.top.$.messager.alert("提示", data.Info, "info");
                    }
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        window.top.$.messager.alert("提示", textStatus, "info");
                    } else if (errThrown) {
                        window.top.$.messager.alert("提示", errThrown, "info");
                    } else {
                        window.top.$.messager.alert("提示", "出现错误", "info");
                    }
                }
            });
        }
    });

    $(this).removeAttr("disabled");
}



//回调函数,通过选择返回的数据
function reloadProducts(grid) {
    //debugger;
    var rows = grid.datagrid("getRows");
    var copyRows = [];
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (!rowProductExist(row["ProductId"])) {
            row["ProductId"] = row["ProductId"];
            row["IsMaster"] = 0;
            copyRows.push(row);
        }
    }
    addProductsVendors(copyRows);
}

//去数据库中新增数据
function addProductsVendors(copyRows) {
    var jsonObject = JSON.stringify(copyRows);
    var data = {
        jsonObject: "" + jsonObject + "",
        vendorID: VendorId
    };
    $.ajax({
        url: "../VerdorProducts/AddProductsVendorListHandle",
        type: "post",
        dataType: "json",
        data: data,
        success: function (result) {
            if (result.Flag == "SUCCESS") {
                $.messager.alert('提示', "供应商商品关系添加成功", "info", function () {
                    $("#grid").datagrid("reload");
                });
            }
            else {
                $.messager.alert('提示', result.Info, "info");
            }
        },
        error: function (request, textStatus, errThrown) {
            if (textStatus) {
                $.messager.alert('错误', textStatus, 'warning');
            } else if (errThrown) {
                $.messager.alert('错误', errThrown, 'warning');
            } else {
                $.messager.alert('错误', "商品设置失败", 'warning');
            }
        }
    });
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


//function saveData() {
//    var rows = $('#grid').datagrid('getRows');
//    $("input[name='IsMasterStr']").each(function (i) {
//        $('#grid').datagrid("getRows", i);
//        var curRow = rows[i];
//        if ($(this).prop("checked")) {
//            curRow.IsMaster = 1;
//        } else {
//            curRow.IsMaster = 0;
//        }
//        $('#grid').datagrid('updateRow',
//            {
//                index: i,
//                row: curRow
//            });
//    });

//    var newrows = $('#grid').datagrid('getRows');
//    var jsonObject = JSON.stringify(newrows);
//    var data = {
//        jsonObject: "" + jsonObject + "",
//        vendorID: VendorId
//    };

//    window.frameElement.wapi.addProductsVendorList(data);
//    frxs.pageClose();
//}


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


//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();