$(function () {
    
    //grid高度改变
    gridresize();

    //查询按钮事件
    $("#aSearch").click(function () {
        search();
    });

    //重置按钮事件
    $("#aReset").click(function () {
        $("#searchForm").form("clear");
        $("#Status").combobox("setValue", '');
    });

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
        url: '../ProductList/ProductPriceChangeList',          //Aajx地址
        //sortName: 'AdjID',                 //排序字段
        //sortOrder: 'asc',                  //排序方式
        idField: 'AdjID',                  //主键
        pageSize: 30,                       //每页条数
        pageList: [10, 30, 50, 100],//可以设置每页记录条数的列表 
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
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        onDblClickCell: function (rowIndex) {
            var selRow = $("#grid").datagrid("getSelected");
            frxs.openNewTab("查看进货价调整单" + selRow.AdjID, "../InsertPriceChange/InsertPriceOrderAddOrEdit?action=view&adjId=" + selRow.AdjID, "icon-search");
        },
        queryParams: {
            AdjType: 0,
            //查询条件
            AdjID: $.trim($("#AdjID").val()),
            ProductName: $.trim($("#ProductName").val()),
            SKU: $.trim($("#SKU").val()),
            Status: $("#Status").combobox("getValue"),
            BarCode: $.trim($("#BarCode").val()),
            PostingTime1: $("#PostingTime1").val(),
            PostingTime2: $("#PostingTime2").val()
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '单据号', field: 'AdjID', width: 110, align: 'center' }
            //冻结列
        ]],
        columns: [[
           
            { title: '生效时间', field: 'BeginTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
            {
                title: '状态', field: 'Status', width: 80, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "录单";
                    } else if (value == "1") {
                        return "确认";
                    } else {
                        return "过账";
                    }
                }
            },
            { title: '录员人员', field: 'CreateUserName', width: 120, align: 'center' },
            { title: '录单时间', field: 'CreateTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
            { title: '确认人员', field: 'ConfUserName', width: 120, align: 'center' },
            { title: '确认时间', field: 'ConfTime', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '过账人员', field: 'PostingUserName', width: 120, align: 'center' },
            { title: '过账时间', field: 'PostingTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
             { title: '备注', field: 'Remark', width: 160, formatter: frxs.formatText }
            
        ]],
        toolbar: toolbarArray
    });
}

//添加
function add() {
    frxs.openNewTab("添加进货价调整单", "../InsertPriceChange/InsertPriceOrderAddOrEdit", "icon-add", window);
}
//查看
function look() {
    var selRow = $("#grid").datagrid("getSelections");
    if (selRow.length == 0) {
        $.messager.alert("提示", "没有选中记录", "info");
        return;
    }
    if (selRow.length > 1) {
        $.messager.alert("提示", "只能选择一行", "info");
        return;
    }

    frxs.openNewTab("查看进货价调整单" + selRow[0].AdjID, "../InsertPriceChange/InsertPriceOrderAddOrEdit?action=view&adjId=" + selRow[0].AdjID, "icon-search");
}

//确认
function confirmOrder() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ajdIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 0) {
            ajdIds += rows[i].AdjID + ",";
        }
    }

    if (ajdIds == "") {
        $.messager.alert("提示", "选中的记录中没有可确认的数据！", "info");
        return false;
    }
    ajdIds = ajdIds.substring(0, ajdIds.length - 1);
    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../ProductList/ProductPriceConfirmOrReconfirm",
        type: "get",
        dataType: "json",
        data: {
            ajdIds: ajdIds,
            type: 0,
            menuType:0
        },
        success: function (result) {
            loading.close();
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    $.messager.alert("提示", "确认成功！", "info");
                    $("#grid").datagrid("reload");
                } else {
                    $.messager.alert("提示", result.Info, "info");
                }
            }
        }
    });
}

//反确认
function noConfirmOrder() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ajdIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 1) {
            ajdIds += rows[i].AdjID + ",";
        }
    }

    if (ajdIds == "") {
        $.messager.alert("提示", "选中的记录中没有可反确认的数据！", "info");
        return false;
    }

    ajdIds = ajdIds.substring(0, ajdIds.length - 1);
    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../ProductList/ProductPriceConfirmOrReconfirm",
        type: "get",
        dataType: "json",
        data: {
            ajdIds: ajdIds,
            type: 1,
            menuType: 0
        },
        success: function (result) {
            loading.close();
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    //实现刷新栏目中的数据
                    $.messager.alert("提示", "反确认成功", "info", function () {
                        $("#grid").datagrid("reload");
                    });
                } else {
                    $.messager.alert("提示", result.Info, "info");
                }
            }
        }
    });

}

//过账
function goEffect() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ajdIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 1) {
            ajdIds += rows[i].AdjID + ",";
        }
    }

    if (ajdIds == "") {
        $.messager.alert("提示", "选中的记录中没有可过账的数据！", "info");
        return false;
    }

    ajdIds = ajdIds.substring(0, ajdIds.length - 1);
    $.messager.confirm("提示", "确定过账选中单据？", function (r) {
        if (r) {
            //遮罩层
            var loading = window.top.frxs.loading();
            $.ajax({
                url: "../ProductList/ProductPricePosting",
                type: "get",
                dataType: "json",
                data: {
                    ajdIds: ajdIds,
                    menuType: 0
                },
                success: function (result) {
                    loading.close();
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {
                            //实现刷新栏目中的数据
                            $.messager.alert("提示", "过账成功", "info", function() {
                                $("#grid").datagrid("reload");
                            });
                        } else {
                            $.messager.alert("提示", result.Info, "info");
                        }
                    }
                }
            });
        }
    });
}

//删除
function del() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ajdIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != 0) {
            $.messager.alert("提示", "错误：非录单状态不能删除！", "info");
            return false;
        }
        ajdIds += rows[i].AdjID + ",";
    }

    ajdIds = ajdIds.substring(0, ajdIds.length - 1);


    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            //遮罩层
            var loading = window.top.frxs.loading();
            $.ajax({
                url: "../ProductList/ProductPriceDel",
                type: "get",
                dataType: "json",
                data: {
                    ajdIds: ajdIds,
                    menuType:0
                },
                success: function (result) {
                    loading.close();
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {
                            //实现刷新栏目中的数据
                            $('#grid').datagrid('clearSelections'); //清除所有选中
                            $("#grid").datagrid("reload");
                        } else {
                            $.messager.alert("提示", result.Info, "info");
                        }
                    }
                }
            });
        }
    });

}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 22);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}