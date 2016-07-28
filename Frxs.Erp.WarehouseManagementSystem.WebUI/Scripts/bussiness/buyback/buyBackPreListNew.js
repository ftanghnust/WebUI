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
        url: '../BuyBackPre/GetBuyBackPreList',          //Aajx地址
        //sortName: 'OrderDate',                 //排序字段
        //sortOrder: 'desc',                  //排序方式
        idField: 'BackID',                  //主键
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
        showFooter: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function (index, rowData) {
            frxs.openNewTab("查看采购退货单" + rowData.BackID, "../BuyBackPre/BuyBackPreAddOrEditNew?BackID=" + rowData.BackID, "icon-search");
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            totalCalculate();
        },
        queryParams: {
            //查询条件
            BackID: $.trim($("#BackID").val()),
            SubWID: $("#WName").combobox("getValue"),
            VendorCodeOrName: $.trim($("#VendorCodeOrName").val()),
            Status: $("#Status").combobox("getValue"),
            ProductName: $.trim($("#ProductName").val()),
            SKU: $.trim($("#SKU").val()),
            OrderDateBegin: $.trim($("#StartDate").val()),
            OrderDateEnd: $.trim($("#EndDate").val()),
            Remark: $.trim($("#Remark").val())
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '退货单号', field: 'BackID', width: 120, align: 'center' }
            //冻结列

        ]],
        columns: [[
            { title: '退货时间', field: 'OrderDate', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '仓库', field: 'WNameStr', width: 200 },
            {
                title: '供应商', field: 'VendorName', width: 300, formatter: function (value, rec) {
                    if (value) {
                        return rec.VendorCode + '-' + value;
                    }
                }
            },
            { title: '状态', field: 'StatusStr', width: 80, align: 'center' },
            {
                title: '退货总金额', field: 'TotalAmt', width: 120, align: 'right', formatter: function (value, rec) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '备注', field: 'Remark', width: 200
            }
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
        frxs.openNewTab("查看采购退货单" + rows[0].BackID, "../BuyBackPre/BuyBackPreAddOrEditNew?BackID=" + rows[0].BackID, "icon-search");
    }
}

//新增按钮事件
function add() {
    frxs.openNewTab("添加退货单", "../BuyBackPre/BuyBackPreAddOrEditNew", "icon-add");
}

//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusStr == "录单") {
            ss.push(rows[i].BackID);
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
                    url: "../BuyBackPre/DeleteBuyBackPre",
                    type: "get",
                    dataType: "json",
                    data: {
                        backids: ss.join(',')
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
        if (rows[i].StatusStr == "录单") {
            ss.push(rows[i].BackID);
        } else {
            $.messager.alert("提示", "录单状态才能确认！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "确认以上订单吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../BuyBackPre/BuyBackPreChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        backids: ss.join(','),
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
        if (rows[i].StatusStr == "确认") {
            ss.push(rows[i].BackID);
        } else {
            $.messager.alert("提示", "确认状态才能反确认！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "反确认以上订单吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../BuyBackPre/BuyBackPreChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        backids: ss.join(','),
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
        if (rows[i].StatusStr == "确认") {
            ss.push(rows[i].BackID);
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
                    url: "../BuyBackPre/BuyBackPreChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        backids: ss.join(','),
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

//导出按钮事件
function exportout() {
    $.messager.alert("提示", "导出！", "info");
}

//查询
function search() {
    $('#grid').datagrid('clearSelections');
    initGrid(toolbarArray);
}

//重置
function resetSearch() {
    $("#searchform").form("clear");
    //$('#WName').combobox('setValue', '');
    var data1 = $('#WName').combobox('getData');  //赋默认值
    $("#WName ").combobox('setValue', data1[0].WID);
    $('#Status').combobox('setValue', '');
}

//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totalSubAmt = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var subAmt = parseFloat(rows[i].TotalAmt);
        totalSubAmt += subAmt;
    }

    //$("#TotalOrderAmt").val(parseFloat(totalSubAmt).toFixed(4));
    $('#grid').datagrid('reloadFooter', [
       { "StatusStr": "<span style='float:right'>当前页合计：</span>", "TotalAmt": parseFloat(totalSubAmt).toFixed(4) },
       { "StatusStr": "<span style='float:right'>总合计：</span>", "TotalAmt": $("#grid").datagrid("getData").SubAmt }
    ]);
}


//下拉控件数据初始化
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
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}
