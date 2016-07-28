$(function () {

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

    document.onkeydown = function (e) {
        if (!e) e = window.event;//火狐中是 window.event
        if ((e.keyCode || e.which) == 13) {
            $("#aSearch").click();
        }
    }
});

function initGrid() {

    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WProductAdjShelf/GetWProductAdjShelfList',                //Aajx地址
        // sortName: 'AdjID',                 //排序字段
        //  sortOrder: 'desc',                  //排序方式
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
            frxs.openNewTab("货位调整" + rowData.AdjID, "../WProductAdjShelf/WProductAdjShelfAddOrEditView?Type=query&ID=" + rowData.AdjID, "icon-search");
        },
        queryParams: {
            //查询条件
            AdjID: $("#AdjID").val(),
            Status: $("#Status").combobox("getValue"),
            StartDate: $("#StartDate").val(),
            EndDate: $("#EndDate").val()
        },
        frozenColumns: [[
            //冻结列
              { field: 'ck', checkbox: true } //选择
        ]],
        columns: [[
               { title: '单号', field: 'AdjID', width: 120, align: 'center' },
            { title: '调整货位数量', field: 'TotalShelfCount', width: 90, align: 'center' },
            { title: '录单人员', field: 'CreateUserName', width: 180, align: 'center' },
            { title: '录单时间', field: 'CreateTime', width: 180, formatter: frxs.dateFormat, align: 'center' },
            { title: '状态', field: 'StatusToStr', width: 80, align: 'center' },
            { title: '确认人员', field: 'ConfUserName', width: 100, align: 'center' },
            { title: '确认时间', field: 'ConfTime', width: 180, formatter: frxs.dateFormat, align: 'center' },
            { title: '过账人员', field: 'PostingUserName', width: 100, align: 'center' },
            { title: '过账时间', field: 'PostingTime', width: 180, formatter: frxs.dateFormat, align: 'center' },
            { title: '备注', field: 'Remark', width: 200, formatter: frxs.formatText }
        ]],
        toolbar: toolbarArray
    });
    $('#grid').datagrid('clearSelections');
}

//查看按钮事件
function view() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        frxs.openNewTab("查看货位调整" + rows[0].AdjID, "../WProductAdjShelf/WProductAdjShelfAddOrEditView?Type=query&ID=" + rows[0].AdjID, "icon-search");

    }
}

//新增按钮事件
function add() {
    frxs.openNewTab("添加货位调整", "../WProductAdjShelf/WProductAdjShelfAddOrEditView", "icon-add");
}

//编辑按钮事件
function edit() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        if (rows[0].StatusToStr != "录单") {

            $.messager.alert("提示", "只有录单状态才能编辑！", "info");
        } else {
            frxs.openNewTab("编辑货位调整" + rows[0].AdjID, "../WProductAdjShelf/WProductAdjShelfAddOrEditView?Type=edit&ID=" + rows[0].AdjID, "icon-edit");
        }
    }
}

//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusToStr == "录单") {
            ss.push(rows[i].AdjID);
        } else {
            $.messager.alert("提示", "非录单状态不能删除！", "info");
            return false;
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (ss.length == 0) {
        $.messager.alert("提示", "非录单状态不能删除！", "info");
        return false;
    }
    $.messager.confirm("提示", "是否确认删除信息?", function (r) {
        if (r) {
            $.ajax({
                url: "../WProductAdjShelf/DeleteWProductAdjShelf",
                type: "get",
                dataType: "json",
                data: {
                    ids: ss.join(',')
                },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            $("#grid").datagrid("reload");
                        }
                    }
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        $.messager.alert("提示", textStatus, "info");
                    } else if (errThrown) {
                        $.messager.alert("提示", errThrown, "info");
                    } else {
                        $.messager.alert("提示", "出现错误", "info");
                    }
                }
            });
        }
    });

}

//确认按钮事件
function sure() {
    var rows = $('#grid').datagrid('getSelections');
    var status = 1;
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusToStr == "录单") {
            ss.push(rows[i].AdjID);
        } else {
            $.messager.alert("提示", "录单状态才能确认！", "info");
            return false;
        }
    }
    setStatus(ss, status, "确认审核所选单号吗？");
}

//反确认按钮事件
function resetSure() {
    var rows = $('#grid').datagrid('getSelections');
    var status = 0;
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusToStr == "确认") {
            ss.push(rows[i].AdjID);
        } else {
            $.messager.alert("提示", "确认状态才能反确认！", "info");
            return false;
        }
    }
    setStatus(ss, status, "反确认所选单据吗？");
}

//过帐按钮事件
function bill() {
    var rows = $('#grid').datagrid('getSelections');
    var status = 2;
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusToStr == "确认") {
            ss.push(rows[i].AdjID);
        } else {
            $.messager.alert("提示", "确认状态才能过帐！", "info");
            return false;
        }
    }
    setStatus(ss, status, "过账所选单号吗？");

}


function setStatus(ss, status, tipTitle) {
    if (ss.length > 0) {
        if (status == 1) {
            var statusStr = "确认";
        } else if (status == 2) {
            var statusStr = "过帐";
        } else {
            var statusStr = "反确认";
        }
        $.messager.confirm("提示", tipTitle, function (r) {
            if (r) {
                $.ajax({
                    url: "../WProductAdjShelf/WProductAdjShelfChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: ss.join(','),
                        status: status
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $.messager.alert("提示", statusStr + "成功！", "info");
                            }
                            else {
                                $.messager.alert("提示", result.Info, "info");
                            }
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            $.messager.alert("提示", statusStr + "失败！", "info");
                        } else if (errThrown) {
                            $.messager.alert("提示", errThrown, "info");
                        } else {
                            $.messager.alert("提示", "出现错误", "info");
                        }
                    }
                });
            }
        });
    }
}


//导出按钮事件
function exportout() {
    $.messager.alert("提示", "导出！", "info");
}

//查询
function search() {
    initGrid();
}

//重置
function resetSearch() {
    $("#searchform").form("clear");
    $('#Status').combobox('setValue', '');
    $("#AdjID").val("");
    $("#StartDate").val('');
    $("#EndDate").val('');
}

//下拉控件数据初始化
function initDDL() {

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

