﻿

$(function () {

    //初始化控件
    initControl();

    //加载数据
    initGrid();
    //高度自适应
    gridresize();

    //事件绑定
    eventBind();
});


//初始化控件
function initControl() {
    var shopCode = frxs.getUrlParam("shopCode");
    var shopName = frxs.getUrlParam("shopName");
    $("#ShopCode").val(shopCode);
    $("#ShopName").val(shopName);
    $("#ShopCode").focus();
}


function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../Common/GetShopInfo',          //Aajx地址
        sortName: 'ShopCode',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopID',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        singleSelect: true,                 //单选
        onDblClickRow: function (index, rowData) {
            window.frameElement.wapi.saleSettleFillShop(rowData);
            window.frameElement.wapi.focus();
            frxs.pageClose();
        },
        queryParams: {
            //查询条件
            ShopCode: $.trim($("#ShopCode").val()),
            ShopName: $.trim($("#ShopName").val())
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '门店ID', field: 'ShopID', hidden: true, width: 80 },
            { title: '门店编码', field: 'ShopCode', width: 100 },
            { title: '门店名称', field: 'ShopName', width: 580 }
        ]]
    });
}

//查询
function loadgrid() {
    initGrid();
}

//事件绑定
function eventBind() {
    //绑定上下键事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        var index;
        //向下
        var selRow;
        if (ev.keyCode == 40) {
            //移除光标
            $("#ShopCode").blur();
            $("#ShopName").blur();

            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            selRow = $("#grid").datagrid("getSelected");
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
            //移除光标
            $("#ShopCode").blur();
            $("#ShopName").blur();

            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            selRow = $("#grid").datagrid("getSelected");
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
            //判断是否有弹窗
            if ($("body").text().indexOf("没有选中行") < 0) {
                
                //获取焦点元素
                var focusObj = $(document.activeElement);

                //判断焦点是不是在文本框上面，是：搜索 不是:选中并回填
                if (focusObj.attr("type") == "text") {
                    initGrid();
                } else {
                    selRow = $("#grid").datagrid("getSelected");
                    if (selRow != null) {
                        window.frameElement.wapi.saleSettleFillShop(selRow);
                        window.frameElement.wapi.focus();
                        frxs.pageClose();
                        return false;
                    } else {
                        $.messager.alert("提示", "没有选中行！", "info");
                    }
                }
            }
        }
    };
}


//清空数据
function resetData() {
    $("#searchForm").form("clear");
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 10);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 5,
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


