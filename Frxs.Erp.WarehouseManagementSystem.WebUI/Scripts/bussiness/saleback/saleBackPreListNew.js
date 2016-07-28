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
        url: '../SaleBackPre/GetSaleBackPreList',          //Aajx地址
        //sortName: 'BackDate',                 //排序字段
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
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function (index, rowData) {
            openNewTab("查看销售退货单" + rowData.BackID, "../SaleBackPre/SaleBackPreAddOrEditNew?BackID=" + rowData.BackID, "icon-search");
        },
        queryParams: {
            //查询条件
            BackID: $.trim($("#BackID").val()),
            SubWID: $("#WName").combobox("getValue"),
            ShopCode: $.trim($("#ShopCode").val()),
            Status: $("#Status").combobox("getValue"),
            ShopName: $.trim($("#ShopName").val()),
            OrderDateBegin: $.trim($("#StartDate").val()),
            OrderDateEnd: $.trim($("#EndDate").val())
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '退货单号', field: 'BackID', width: 125, align: 'center' },
            { title: '结算单号', field: 'SettleID', width: 125, align: 'center' }
            //冻结列
        ]],
        columns: [[
          
           
            { title: '门店编号', field: 'ShopCode', width: 120, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 200 },
            {
                title: '退货商品总数量', field: 'TotalBackQty', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            { title: '退货总金额', field: 'PayAmount', width: 120, align: 'right',formatter:function (value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '状态', field: 'StatusStr', width: 75, align: 'center' },
            { title: '入库时间', field: 'BackDate', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '录单时间', field: 'CreateTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
            { title: '确认时间', field: 'ConfTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
            { title: '过帐时间', field: 'PostingTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
            { title: '备注', field: 'Remark', width: 150, formatter: frxs.formatText }
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
        openNewTab("查看销售退货单" + rows[0].BackID, "../SaleBackPre/SaleBackPreAddOrEditNew?BackID=" + rows[0].BackID, "icon-search");
    }
}

//新增按钮事件
function add() {
    openNewTab("添加销售退货单", "../SaleBackPre/SaleBackPreAddOrEditNew", "icon-add");
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
                    url: "../SaleBackPre/DeleteSaleBackPre",
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
                    url: "../SaleBackPre/SaleBackPreChangeStatus",
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
                    url: "../SaleBackPre/SaleBackPreChangeStatus",
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
                    url: "../SaleBackPre/SaleBackPreChangeStatus",
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

//打开新窗口
function openNewTab(title, url, icon) {
    if (!parent.$("#tabs").tabs('exists', title)) {
        parent.$("#tabs").tabs('add', {
            title: title,
            content: '<iframe frameborder="0" scrolling="true" src="' + url + '" style="width:100%;height:98%;position:relative;"></iframe>',
            closable: true,
            icon: icon
        });
    } else {
        parent.$("#tabs").tabs('select', title);
    }

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
