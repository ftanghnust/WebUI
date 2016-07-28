
//销售结算单列表
$(function () {

    //加载仓库编号和结算方式下拉列表
    initSettleType();
    //initWCList();

    //重置按钮事件
    $("#aReset").click(function () {
        resetSearch();
    });

});


//加载仓库编号和结算方式下拉列表
function initSettleType() {
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        data: {
            dictType: "ShopSettleType"
        },
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "DictValue": "", "DictLabel": "-全部-" });
            }
            //创建控件
            $("#SettleType").combobox({
                data: data,             //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"      //value列
            });

            $("#SettleType").combobox('select', data[0].DictValue);


        }, error: function (e) {
            debugger;
        }
    });
};


//下拉控件数据初始化
function initWCList() {
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



function initGrid(btnEdit, vartoolbar) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../SaleSettle/GetSaleSettleList',          //Aajx地址
        sortName: 'SettleTime',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'SettleID',                  //主键
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
            if (btnEdit) {
                if (rowData.Status != "0") {
                    frxs.openNewTab("查看门店结算单" + rowData.SettleID, "../SaleSettle/SaleSettleAddOrEdit?Type=query&SettleId=" + rowData.SettleID, "icon-edit");
                } else {
                    frxs.openNewTab("编辑门店结算单" + rowData.SettleID, "../SaleSettle/SaleSettleAddOrEdit?Type=edit&SettleId=" + rowData.SettleID, "icon-edit");
                }
            }
            else {
                frxs.openNewTab("查看门店结算单" + rowData.SettleID, "../SaleSettle/SaleSettleAddOrEdit?Type=query&SettleId=" + rowData.SettleID, "icon-edit");
            }
        },
        queryParams: {
            ShopCode: $("#ShopCode").val(),
            ShopName: $("#ShopName").val(),
            SettleID: $("#SettleID").val(),
            StartTime: $('#StartTime').val(),
            EndTime: $('#EndTime').val(),
            SettleType: $('#SettleType').combobox('getValue'),
            Status: $('#Status').combobox('getValue'),
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            gridresize();
            totalCalculate();
        },
        frozenColumns: [[
            { field: 'ck', checkbox: true },   //选择
            { title: '结算单号', field: 'SettleID', width: 120, align: 'center' }
            //冻结列
        ]],
        columns: [[
            { title: '结算时间', field: 'SettleTime', width: 140, align: 'center' },
            {
                //0:未结算[预留];1:已确认[预留];2:已过帐
                title: '状态', field: 'Status', width: 140, align: 'center', formatter: function (value) {
                    var statusstr = "";
                    if (value == 0) {
                        statusstr = "录单";
                    }
                    else if (value == 1) {
                        statusstr = "确认";
                    }
                    else if (value == 2) {
                        statusstr = "已过帐";
                    }
                    return statusstr;
                }
            },
            { title: '门店编号', field: 'ShopCode', width: 80, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 200 },
            { title: '结算方式', field: 'SettleName', width: 100, align: 'center' },
            {
                title: '结算总金额', field: 'SettleAmt', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '开户行', field: 'BankType', width: 110 },
            { title: '开户名', field: 'BankAccountName', width: 80, align: 'center' },
            { title: '银行账号', field: 'BankAccount', width: 120, align: 'center' },
            { title: '信用额度', field: 'CreditAmt', width: 70, align: 'right' }
        ]],
        toolbar: vartoolbar
    });
}


//查看
function show() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        frxs.openNewTab("查看门店结算单" + rows[0].SettleID, "../SaleSettle/SaleSettleAddOrEdit?Type=query&SettleId=" + rows[0].SettleID, "icon-search");
    }
}

//新增按钮事件
function add() {
    frxs.openNewTab("添加门店结算", "../SaleSettle/SaleSettleAddOrEdit", "icon-add");
}

//编辑按钮事件
function edit() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        //if (rows[0].Status != "0") {
        //    $.messager.alert("提示", "只有录单状态费用才能编辑！", "info");
        //} else {
        frxs.openNewTab("编辑门店结算单" + rows[0].SettleID, "../SaleSettle/SaleSettleAddOrEdit?Type=edit&SettleId=" + rows[0].SettleID, "icon-edit");
        //}
    }
}

//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');

    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }

    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == "0") {
            ss.push(rows[i].SettleID);
        }
    }
    if (ss.length == 0) {
        $.messager.alert("提示", "非录单状态不能删除！", "info");
        return false;
    }
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return false;
    }

    $.messager.confirm("提示", "是否确认删除结算单录单单据?", function (r) {
        if (r) {
            $.ajax({
                url: "../SaleSettle/DeleteSaleSettle",
                type: "post",
                dataType: "json",
                data: {
                    settleIds: ss.join(',')
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
    return true;
}

//确认按钮事件
function sure() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == "0") {
            ss.push(rows[i].SettleID);
        } else {
            $.messager.alert("提示", "录单状态才能确认！", "info");
            return false;
        }
    }

    $.messager.confirm("提示", "是否确认单据?", function (r) {
        if (r) {
            $.ajax({
                url: "../SaleSettle/SaleSettleSureOrNo",
                type: "post",
                dataType: "json",
                data: {
                    settleIds: rows[0].SettleID,
                    status: 1,
                    issure: 1 //确认
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
        };
    });
    return true;
}

//反确认按钮事件
function resetSure() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == "1") {
            ss.push(rows[i].SettleID);
        } else {
            $.messager.alert("提示", "确认状态才能反确认！", "info");
            return false;
        }
    }

    $.messager.confirm("提示", "是否反确认单据?", function (r) {
        if (r) {
            $.ajax({
                url: "../SaleSettle/SaleSettleSureOrNo",
                type: "post",
                dataType: "json",
                data: {
                    settleIds: rows[0].SettleID,
                    status: 0,
                    issure: 0 //反确认
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
        };
    });
    return true;
}

//过帐按钮事件
function posting() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return false;
    }

    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == "1") {
            ss.push(rows[i].SettleID);
        } else {
            $.messager.alert("提示", "确认状态才能过帐！", "info");
            return false;
        }
    }

    $.messager.confirm("提示", "是否过账单据?", function (r) {
        if (r) {
            $.ajax({
                url: "../SaleSettle/SaleSettlePost",
                type: "post",
                dataType: "json",
                data: {
                    settleIds: rows[0].SettleID,
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
        };
    });
    return true;
}





//导出
function exportData() {

    //site_common.ShowProgressBar();
    location.href = "../SaleSettle/DataExport?ShopCode=" + $("#ShopCode").val() +
        "&ShopName=" + $("#ShopName").val() +
        "&SettleID=" + $("#SettleID").val() +
        "&StartTime=" + $('#StartTime').val() +
        "&EndTime=" + $('#EndTime').val() +
        "&SettleType=" + $('#SettleType').combobox('getValue');
    //site_common.HideProgressBar();
}


//重置
function resetSearch() {
    $("#ShopCode").val('');
    $("#ShopName").val('');
    $("#SettleID").val('');
    $("#StartTime").val('');
    $("#EndTime").val('');
    $('#Status').combobox('setValue', ''),
    $('#SettleType').combobox('setValue', '');
}



//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totalSubAmt = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var subAmt = parseFloat(rows[i].SettleAmt);
        totalSubAmt += subAmt;
    }

    $('#grid').datagrid('reloadFooter', [
       { "SettleName": "<div style='width:100%;text-align:right'>当前页合计：</div>", "SettleAmt": parseFloat(totalSubAmt).toFixed(4).toString() },
       { "SettleName": "<div style='width:100%;text-align:right'>总合计：</div>", "SettleAmt": parseFloat($("#grid").datagrid("getData").SubAmt).toFixed(4).toString() }
    ]);
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


