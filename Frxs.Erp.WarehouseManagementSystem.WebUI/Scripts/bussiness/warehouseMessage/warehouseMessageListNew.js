$(function () {

    //grid绑定
    // initGrid();

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
        url: '../WarehouseMessage/GetWarehouseMessageList',                //Aajx地址
        sortName: 'IsFirst',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'ID',                  //主键
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
            var thisdlg = frxs.dialog({
                title: "信息管理",
                url: "../WarehouseMessage/WarehouseMessageAddOrEditSearch?ID=" + rowData.ID,
                owdoc: window.top,
                width: 830,
                height: 650,
                buttons: [{
                    text: '关闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        thisdlg.dialog("close");
                    }
                }]
            });
        },
        queryParams: {
            //查询条件
            Title: $("#Title").val(),
            ConfUserName: $("#ConfUserName").val(),
            Status: $("#Status").combobox("getValue"),
            MessageType: $("#MessageType").combobox("getValue"),
            BeginTime: $("#BeginTime").val(),
            EndTime: $("#EndTime").val()
        },
        frozenColumns: [[
            //冻结列
          { field: 'ck', checkbox: true } //选择
        ]],
        columns: [[
            { title: 'ID', field: 'ID', width: 260, hidden: true },
            { title: '标题', field: 'Title', width: 260, formatter: frxs.formatText },
            { title: '消息类型', field: 'MessageTypeStr', width: 160, align: 'center' },
            { title: '开始时间', field: 'BeginTime', width: 200, formatter: frxs.dateFormat, align: 'center' },
            { title: '结束时间', field: 'EndTime', width: 200, formatter: frxs.dateFormat, align: 'center' },
            { title: '状态', field: 'StatusStr', width: 150, align: 'center' },
            { title: '是否至顶', field: 'IsFirstStr', width: 100, align: 'center' },
          //  { title: '门店', field: 'WID', width: 150 },            { title: '发布人', field: 'ConfUserName', width: 150, align: 'center' }
         
        ]],
        toolbar: toolbarArray
    });
}

//查看按钮事件
function view() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        var thisdlg = frxs.dialog({
            title: "信息管理",
            url: "../WarehouseMessage/WarehouseMessageAddOrEditSearch?ID=" + rows[0].ID,
            owdoc: window.top,
            width: 820,
            height: 650,
            buttons: [{
                text: '关闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    thisdlg.dialog("close");
                }
            }]
        });
    }
}

//新增按钮事件
function add() {
    var thisdlg = frxs.dialog({
        title: "新增",
        url: "../WarehouseMessage/WarehouseMessageAddOrEditView",
        owdoc: window.top,
        width: 820,
        height: 650,
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

//编辑按钮事件
function edit() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {

        if (rows[0].StatusStr == "已发布" || rows[0].StatusStr == "已停止") {
            $.messager.alert("提示", "只能对未发布状态信息进行编辑！", "info");
            return false;
        }

        var thisdlg = frxs.dialog({
            title: "编辑",
            url: "../WarehouseMessage/WarehouseMessageAddOrEditView?ID=" + rows[0].ID,
            owdoc: window.top,
            width: 820,
            height: 650,
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
}

//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusStr == "未发布") {
            ss.push(rows[i].ID);
        } else {
            $.messager.alert("提示", "未发布状态才能删除！", "info");
            return false;
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (ss.length == 0) {
        $.messager.alert("提示", "状态为未发布的信息才能删除！", "info");
        return false;
    }

    $.messager.confirm("提示", "是否确认删除信息?", function (r) {
        if (r) {
            $.ajax({
                url: "../WarehouseMessage/DeleteWarehouseMessage",
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
                            $('#grid').datagrid('clearSelections');
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
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusStr == "未发布") {
            ss.push(rows[i].ID);
        } else {
            $.messager.alert("提示", "未发布状态才能确认！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "确认以上消息吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../WarehouseMessage/WarehouseMessageChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: ss.join(','),
                        status: 1
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
}



//停止显示按钮事件
function resetview() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusStr == "已发布") {
            ss.push(rows[i].ID);
        } else {
            $.messager.alert("提示", "状态为已发布才能停止发布！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "确认停止发布以上消息吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../WarehouseMessage/WarehouseMessageChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: ss.join(','),
                        status: 2
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
}


//反确认按钮事件
function resetsure() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusStr == "已发布") {
            ss.push(rows[i].ID);
        } else {
            $.messager.alert("提示", "已发布状态才能反确认！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "确认以上消息吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../WarehouseMessage/WarehouseMessageChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: ss.join(','),
                        status: 0
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
    $('#MessageType').combobox('setValue', '');
    $('#Status').combobox('setValue', '');
    $('#Title').val('');
    $('#ConfUserName').val('');
    $('#BeginTime').val('');
    $('#EndTime').val('');
}


//下拉控件数据初始化
function initDDL() {

    $.ajax({
        url: '../Common/GetMessageType',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#MessageType").combobox({
                data: data,             //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"      //value列
            });
        }, error: function (e) {

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
        width: $(window).width() - 11,
        height: h
    });
}


