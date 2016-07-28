var dialogWidth = 850;
var dialogHeight = 600;
var AdjType = 0;
var stockType = "盘亏单";

$(function () {
    init();
    initDDL();
    //initGrid();
    gridresize();
    //document.onkeydown = function (e) {
    //    if (!e) e = window.event;//火狐中是 window.event
    //    if ((e.keyCode || e.which) == 13) {
    //        $("#aSearch").click();
    //    }
    //}
});

function initDDL() {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#SubWID").combobox('select', data[0].WID);
            initGrid();
            gridresize();
        }, error: function (e) {

        }
    });
}

function init() {
    AdjType = $("#AdjType").val();
    $("#searchform").submit(function () {
        //alert("submit");
        initGrid();
        return false;
    });
    //$("#aSearch").click(function () {
    //    initGrid();
    //});
    //重置按钮事件
    $("#aReset").click(function () {
        //$("#searchform").form("clear");
        //$("#Status").combobox("setValue", '');
        //$("#SubWID").combobox("setValue", '');
        $("#AdjID").val('');
        $("#ProductName").val('');
        $("#SKU").val('');
        $("#BeginTime").val('');
        $("#EndTime").val('');
        $("#Status").combobox("setValue", '');
        var subWidData = $("#SubWID").combobox("getData");
        if (subWidData != null && subWidData.length > 0) {
            if (subWidData.length > 1) {
                $("#SubWID").combobox("setValue", '');
            }
        }
    });
}

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../StockAdj/GetStockAdjList',          //Aajx地址
        //sortName: 'CreateTime',                 //排序字段
        //sortOrder: 'desc',                  //排序方式
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
        onDblClickRow: function (index, rowData) {
            frxs.openNewTab("查看" + stockType + rowData.AdjID, "../StockCheckLoss/StockCheckAddOrEdit?view=1&Id=" + rowData.AdjID, "icon-search", window);
        },
        queryParams: {
            //查询条件
            AdjType: $.trim($("#AdjType").val()),
            AdjID: $.trim($("#AdjID").val()),
            ProductName: $.trim($("#ProductName").val()),
            SKU: $.trim($("#SKU").val()),
            Status: $("#Status").combobox("getValue"),
            SubWID: $("#SubWID").combobox("getValue"),
            BeginTime: $("#BeginTime").val(),
            EndTime: $("#EndTime").val() != "" ? $("#EndTime").val() + " 23:59:59" : ""
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '单号', field: 'AdjID', width: 100, hidden: false },
            //冻结列
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "录单";
                    }
                    else if (value == "1") {
                        return "确认";
                    }
                    else if (value == "2") {
                        //return "<span class='thaw_text'>已过账</span>";
                        return "已过账";
                    }
                    else if (value == "3") {
                        //return "<span class='freeze_text'>已废除</span>";
                        return "已废除";
                    } else {
                        return value;
                    }
                }
            }
        ]],
        columns: [[
           
            {
                title: '开单日期', field: 'AdjDate', width: 80,align: 'center', formatter: function (value, rec) {
                    return value.DataFormat("yyyy-MM-dd")
            }
            },
            { title: '仓库', field: 'SubWName', width: 60, align: 'center' },
           
            { title: '录单人员', field: 'CreateUserName', width: 120, align: 'center' },
            { title: '录单时间', field: 'CreateTime', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '确认人员', field: 'ConfUserName', width: 110, align: 'center' },
            { title: '确认时间', field: 'ConfTime', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '过账人员', field: 'PostingUserName', width: 110, align: 'center' },
            { title: '过账时间', field: 'PostingTime', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '备注', field: 'Remark', width: 200, formatter: frxs.formatText }
            
        ]],
        toolbar: vartoolbar
        
    });
}

function viewCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    } else {
        view(rows[0].AdjID);
        //$("#grid").datagrid('unselectAll');
    }
}

function view(id) {
    frxs.openNewTab("查看" + stockType + id, "../StockCheckLoss/StockCheckAddOrEdit?view=1&Id=" + id, "icon-search", window);
}

function add() {
    frxs.openNewTab("新增" + stockType, "../StockCheckLoss/StockCheckAddOrEdit", "icon-add", window);
}

function editCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    } else {
        //if (rows[0].Status != "0") {
        //    $.messager.alert("提示", "只能编辑录单状态的" + stockType + "！", "info");
        //    return;
        //}
        edit(rows[0].AdjID);
    }
}

function edit(id) {
    frxs.openNewTab("编辑" + stockType + id, "../StockCheckLoss/StockCheckAddOrEdit?Id=" + id, "icon-edit", window);
}

function delCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "0") {
            //$.messager.alert("提示", "只能删除录单状态的" + stockType + "！", "info");
            $.messager.alert("提示", "非录单状态不能删除！", "info");
            return;
        }
        idStr += ("" + rows[i].AdjID + ",");
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    }
    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            $.ajax({
                url: "../StockAdj/DeleteStockLossAdj",
                type: "get",
                dataType: "json",
                data: {
                    ids: idStr, adjType: 1
                },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
                        if (result.Flag == "SUCCESS") {
                            $("#grid").datagrid("reload");
                            $("#grid").datagrid('clearSelections');
                        }
                    }
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        alert(textStatus);
                    } else if (errThrown) {
                        alert(errThrown);
                    } else {
                        alert("出现错误");
                    }
                }
            });
        }
    });
}

//确认
function confirmCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "0") {
            //$.messager.alert("提示", "只能确认录单状态的" + stockType + "！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].AdjID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能确认录单状态的" + stockType + "！", "info");
        return;
    }
    //alert(idStr);
    $.messager.confirm("提示", "确认该" + stockType + "？", function (r) {
        if (r) {
            var loading = frxs.loading("正在执行，盘亏单确认环节会检查每个商品的实时库存，请稍后...");
            $.ajax({
                url: "../StockAdj/ConfirmStockLossAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 1, adjType: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {
                            $.messager.alert('提示', result.Info, 'info');
                            $("#grid").datagrid("reload");
                            $('#grid').datagrid('clearSelections');
                        } else {
                            //2016-6-15 当后台给出的错误提示信息太多时，自定义弹窗，允许出现滚动条
                            var objDialog = $("<div style='height: 300px; width: 360px; overflow: auto;'>" + result.Info + "</div>").dialog({
                                title: "温馨提示",
                                height: 300,
                                buttons: [{
                                    text: '确定',
                                    iconCls: 'icon-ok',
                                    handler: function () {
                                        objDialog.dialog("close");
                                    }
                                }]
                            });
                            $("#grid").datagrid("reload");
                        }
                    }
                    loading.close();
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        alert(textStatus);
                    } else if (errThrown) {
                        alert(errThrown);
                    } else {
                        alert("出现错误");
                    }
                    loading.close();
                }
            });
        }
    });
}

//反确认
function unconfirmCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "1") {
            //$.messager.alert("提示", "只能反确认已确认状态的" + stockType + "！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].AdjID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能反确认确认状态的" + stockType + "！", "info");
        return;
    }
    $.messager.confirm("提示", "反确认该" + stockType + "？", function (r) {
        if (r) {
            $.ajax({
                url: "../StockAdj/UnConfirmStockLossAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 0, adjType: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
                        if (result.Flag == "SUCCESS") {
                            $("#grid").datagrid("reload");
                            $('#grid').datagrid('clearSelections');
                        } else {
                            $("#grid").datagrid("reload");
                        }
                    }
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        alert(textStatus);
                    } else if (errThrown) {
                        alert(errThrown);
                    } else {
                        alert("出现错误");
                    }
                }
            });
        }
    });
}

//过账
function postingCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "1") {
            //$.messager.alert("提示", "只能过账确认状态的" + stockType + "！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].AdjID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能过账确认状态的" + stockType + "！", "info");
        return;
    }
    $.messager.confirm("提示", "过账该" + stockType + "？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，当单据的明细数量较多可能需要多一点时间，请稍后...");
            $.ajax({
                url: "../StockAdj/PostingStockLossAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, adjType: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
                        if (result.Flag == "SUCCESS") {
                            $("#grid").datagrid("reload");
                            $('#grid').datagrid('clearSelections');
                        } else {
                            $("#grid").datagrid("reload");
                        }
                    }
                    loading.close();
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        alert(textStatus);
                    } else if (errThrown) {
                        alert(errThrown);
                    } else {
                        alert("出现错误");
                    }
                    loading.close();
                }
            });
        }
    });
}

function stopCheck() {
    
}

function copyCheck() {

}

function reload() {
    $("#grid").datagrid("reload");
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