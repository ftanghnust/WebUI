var dialogWidth = 850;
var dialogHeight = 600;
var AdjType = 0;
var stockType = "盘盈单";

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
            frxs.openNewTab("查看" + stockType + rowData.AdjID, "../StockCheckOver/StockCheckAddOrEdit?view=1&Id=" + rowData.AdjID, "icon-search", window);
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
            { title: '单号', field: 'AdjID', width: 100, hidden: false, align: 'center' },
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
                title: '开单日期', field: 'AdjDate', width: 70, align: 'center', formatter: function (value, rec) {
                    return value.DataFormat("yyyy-MM-dd")
                }
            },
            { title: '仓库', field: 'SubWName', width: 50 },
            { title: 'SubWID', field: 'SubWID', width: 100, hidden: true, align: 'center' },
            { title: 'SubWCode', field: 'SubWCode', width: 100, hidden: true, align: 'center' },
            //{ title: 'SubWName', field: 'SubWName', width: 100, hidden: true, align: 'center' },
            {
                title: '盘盈类型', field: 'CreateFlag', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "手动";
                    }
                    else if (value == "1") {
                        return "<span style='color:red;'>自动</span>";
                    }
                }
            },
            {
                title: '是否生成盘亏单', field: 'IsDispose', width: 90, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        if (rec.CreateFlag == "1" && rec.Status == "2") {
                            return "<span style='color:blue;'>否</span>";
                        }
                        else {
                            return "否";
                        }
                    }
                    else if (value == "1" || value == "2") {
                        return "<span style='color:red;'>是</span>";
                    }
                }
            },
            { title: '盘亏单号', field: 'RefAdjID', width: 100, align: 'center' },
            { title: '自动盘盈订单号', field: 'RefBID', width: 120, align: 'center' },
            { title: '录单人员', field: 'CreateUserName', width: 85, align: 'center' },
            { title: '录单时间', field: 'CreateTime', width: 110, formatter: frxs.dateFormat, align: 'center' },
            { title: '确认人员', field: 'ConfUserName', width: 100, align: 'center' },
            { title: '确认时间', field: 'ConfTime', width: 110, formatter: frxs.dateFormat, align: 'center' },
            { title: '过账人员', field: 'PostingUserName', width: 100, align: 'center' },
            { title: '过账时间', field: 'PostingTime', width: 110, formatter: frxs.dateFormat, align: 'center' },
            { title: '备注', field: 'Remark', width: 160, formatter: frxs.formatText }
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
    frxs.openNewTab("查看" + stockType + id, "../StockCheckOver/StockCheckAddOrEdit?view=1&Id=" + id, "icon-search", window);
}

function add() {
    frxs.openNewTab("新增" + stockType, "../StockCheckOver/StockCheckAddOrEdit", "icon-add", window);
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
    frxs.openNewTab("编辑" + stockType + id, "../StockCheckOver/StockCheckAddOrEdit?Id=" + id, "icon-edit", window);
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
            $.messager.alert("提示", "非录单状态不能删除！", "info");
            return;
        } else {
            idStr += ("" + rows[i].AdjID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    }
    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，请稍后...");
            $.ajax({
                url: "../StockAdj/DeleteStockOverAdj",
                type: "get",
                dataType: "json",
                data: {
                    ids: idStr, adjType: 0
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

function confirmCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "0") {
            //$.messager.alert("提示", "只能确认录单状态的" + stockType + "！", "info");
            //window.focus();
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
            $.ajax({
                url: "../StockAdj/ConfirmStockOverAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 1, adjType: 0 },
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

function unconfirmCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "1") {
            //$.messager.alert("提示", "只能反确认已确认状态的" + stockType + "！", "info");
            //window.focus();
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
                url: "../StockAdj/UnConfirmStockOverAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 0, adjType: 0 },
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

//过账操作
function postingCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "1") {
            //$.messager.alert("提示", "只能过账确认状态的" + stockType + "！", "info");
            //window.focus();
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
            //因为涉及到每个单据的每条明细的库存更新，操作时间可能比较久，需求有遮罩等待效果。
            var loading = frxs.loading("正在保存，当单据的明细数量较多可能需要多一点时间，请稍后...");
            $.ajax({
                url: "../StockAdj/PostingStockOverAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, adjType: 0 },
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

//汇总生成盘亏单
function addStockLess() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return false;
    }
    //单据号集合
    var idStr = "";
    var checkTempSubWId = "";
    debugger;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].CreateFlag != "1") {
            $.messager.alert("提示", rows[i].AdjID + "不是“自动盘盈”的" + stockType + "！", "info");
            window.focus();
            return false;
        }
        else if (rows[i].Status != "2") {
            $.messager.alert("提示", rows[i].AdjID + "不是“已过账”的" + stockType + "！", "info");
            window.focus();
            return false;
        }
        else if (rows[i].IsDispose != "0") {
            $.messager.alert("提示", rows[i].AdjID + "已经生成了盘亏单！", "info");
            window.focus();
            return false;
        }
        else {
            idStr += ("" + rows[i].AdjID + ",");

            if (checkTempSubWId == "") {
                checkTempSubWId = rows[i].SubWID;
            }
            //需增加判断，所选择的仓库子机构必须是统一的，就是说必须只选择某一个仓库子机构的盘盈单进行操作。
            if (rows[i].SubWID != checkTempSubWId) {
                $.messager.alert("提示", "所选择的仓库子机构必须为同一仓库", "info");
                return false;
            }


        }

    }
    
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能选择“自动盘盈”的" + stockType + "！", "info");
        return false;
    }



    var subWid=rows[0].SubWID;
    var subWCode=rows[0].SubWCode;
    var subWName = rows[0].SubWName;
    
    //调用接口
    $.messager.confirm("提示", "要汇总生成盘亏单吗？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，请稍后...");
            $.ajax({
                url: "../StockAdj/GatherAddStockLessAdj",
                type: "post",
                dataType: "json",
                data: { ids: idStr, subWid: subWid, subWCode: subWCode, subWName: subWName },//
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
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