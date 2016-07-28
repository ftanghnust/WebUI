$(function () {
    //grid绑定
    initGrid(toolbarArray);

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();
});

function initGrid(toolbarArray) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../BatchRecommend/GetBatchRecommendList',          //Aajx地址
        //sortName: 'EditDate',                 //排序字段
        //sortOrder: 'desc',                  //排序方式
        idField: 'EditID',                  //主键
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
        onDblClickRow: function () {
            var rows = $('#grid').datagrid('getSelections');
            frxs.openNewTab("查看商品批量推荐订单" + rows[0].EditID, "../BatchRecommend/BatchRecommendAddOrEdit?EditID=" + rows[0].EditID, "icon-search");
        },
        queryParams: {
            //查询条件
            EditID: $.trim($("#EditID").val()),
            Status: $("#Status").combobox("getValue"),
            Remark: $.trim($("#Remark").val()),
            SubWID: $("#WName").combobox("getValue"),
            CreateTimeBegin: $.trim($("#CreateTimeStart").val()),
            CreateTimeEnd: $.trim($("#CreateTimeEnd").val()),
            CreateUserName: $.trim($("#CreateUserName").val())
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true },
            { title: '单号', field: 'EditID', width: 120 , align: 'center'}
            //冻结列
        ]],
        columns: [[

            { title: '录单时间', field: 'EditDate', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '订单数量', field: 'OrderCount', width: 190, align: 'right' },
            { title: '录单人', field: 'CreateUserName', width: 120, align: 'center' },
            {
                title: '状态', field: 'StatusName', width: 100, align: 'center',
                formatter: function (value, rec) {
                    var str = "";
                    switch (rec.Status) {
                        case 0:
                            str += "录单";
                            break;
                        case 1:
                            str += "确认";
                            break;
                        case 2:
                            str += "过账";
                            break;

                    }
                    return str;
                }
            },
            { title: '状态', field: 'Status', width: 100, hidden: true },
            { title: '备注', field: 'Remark', width: 180, formatter: frxs.formatText }
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
        frxs.openNewTab("查看商品批量推荐订单" + rows[0].EditID, "../BatchRecommend/BatchRecommendAddOrEdit?EditID=" + rows[0].EditID, "icon-search");
    }
}

//新增按钮事件
function add() {
    frxs.openNewTab("添加批量推荐订单", "../BatchRecommend/BatchRecommendAddOrEdit", "icon-add");
}

//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 0) {
            ss.push(rows[i].EditID);
        } else {
            $.messager.alert("提示", "非录单状态不能删除！", "info");
            return false;
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "确认删除吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../BatchRecommend/DeleteBatchRecommend",
                    type: "get",
                    dataType: "json",
                    data: {
                        editids: ss.join(',')
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "删除成功！", "info");
                            } else {
                                $.messager.alert("提示", result.Info, "info");
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

//确认按钮事件
function sure() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 0) {
            ss.push(rows[i].EditID);
        } else {
            $.messager.alert("提示", "录单状态才能确认！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "确认以上订单吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../BatchRecommend/BatchRecommendChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        editids: ss.join(','),
                        status: 1
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "确认成功！", "info");
                            } else {
                                $.messager.alert("提示", result.Info, "info");
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
function noSure() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 1) {
            ss.push(rows[i].EditID);
        } else {
            $.messager.alert("提示", "确认状态才能反确认！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "反确认以上订单吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../BatchRecommend/BatchRecommendChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        editids: ss.join(','),
                        status: 0
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "反确认成功！", "info");
                            } else {
                                $.messager.alert("提示", result.Info, "info");
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

//过账按钮事件
function posting() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 1) {
            ss.push(rows[i].EditID);
        } else {
            $.messager.alert("提示", "确认状态才能过账！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "过账以上订单吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../BatchRecommend/BatchRecommendChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        editids: ss.join(','),
                        status: 2
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "过账成功！", "info");
                            } else {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", result.Info, "info");
                            }
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        loading.close();
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

//查询
function search() {
    $('#grid').datagrid('clearSelections');
    initGrid(toolbarArray);
}

//重置
function resetSearch() {
    $("#searchform").form("clear");

    var data2 = $('#WName').combobox('getData');  //赋默认值
    $("#WName ").combobox('setValue', data2[0].WID);

    $('#Status').combobox('setValue', '');
}

//下拉控件数据初始化
function initDDL() {
    $.ajax({
        url: '../Common/GetWarehouseLineList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "LineID": "", "LineName": "-全部-" });
            }
            //创建控件
            $("#LineName").combobox({
                data: data,             //数据源
                valueField: "LineID",       //id列
                textField: "LineName"      //value列
            });
            $("#LineName").combobox('select', data[0].LineID);
        }, error: function (e) {
            debugger;
        }
    });

    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            if (data.Flag && data.Flag == "EXCEPTION") {
                $.messager.alert("错误", data.Info, "error");
                return false;
            }
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#WName").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#WName").combobox('select', data[0].WID);
        }, error: function (e) {
            debugger;
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


