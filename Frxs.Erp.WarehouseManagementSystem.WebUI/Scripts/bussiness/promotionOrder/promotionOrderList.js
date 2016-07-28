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
        url: '../PromotionOrder/GetPromotionOrderList',          //Aajx地址
        //sortName: 'OrderDate',                 //排序字段
        //sortOrder: 'desc',                  //排序方式
        idField: 'OrderId',                  //主键
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
            frxs.openNewTab("查看订货平台订单" + rowData.OrderId, "../PromotionOrder/PromotionOrderAddOrEdit?OrderID=" + rowData.OrderId, "icon-search");
        },
        queryParams: {
            //查询条件
            OrderId: $.trim($("#OrderId").val()),
            ShopCode: $.trim($("#ShopCode").val()),
            ShopName: $.trim($("#ShopName").val()),
            SubWID: $("#WName").combobox("getValue"),
            LineID: $("#LineName").combobox("getValue"),
            Status: $("#Status").combobox("getValue"),
            ShopType: $("#ShopType").combobox("getValue"),
            SendDateBegin: $.trim($("#SendStartDate").val()),
            SendDateEnd: $.trim($("#SendEndDate").val()),
            OrderDateBegin: $.trim($("#OrderStartDate").val()),
            OrderDateEnd: $.trim($("#OrderEndDate").val())
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true },
            { title: '订单编号', field: 'OrderId', width: 120, align: 'center' }
            //冻结列
        ]],
        columns: [[

            { title: '下单时间', field: 'OrderDate', width: 150, formatter: frxs.dateFormat, align: 'center' },
            { title: '门店类型', field: 'ShopTypeName', width: 100, align: 'center' },
            { title: '门店编号', field: 'ShopCode', width: 100, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 200, align: 'center' },
            { title: '订单金额', field: 'PayAmount', width: 100, align: 'right' },
            { title: '状态', field: 'StatusName', width: 100, align: 'center' },
            {
                title: '预计配送日期', field: 'SendDate', width: 100,
                formatter: function (value, rec) {
                    if (value) {
                        return value.DataFormat("yyyy-MM-dd");
                    }
                }, align: 'center'
            },
            { title: '配送线路', field: 'LineName', width: 260 },

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
        frxs.openNewTab("查看订货平台订单" + rows[0].OrderId, "../PromotionOrder/PromotionOrderAddOrEdit?OrderID=" + rows[0].OrderId, "icon-search");
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
        if (rows[i].StatusName == "等待确认") {
            ss.push(rows[i].OrderId);
        } else {
            $.messager.alert("提示", "等待确认状态才能确认！", "info");
            return false;
        }
    }

    if (ss.length == 0) {
        $.messager.alert("提示", "没有可确认的订单！", "info");
        return false;
    }
    if (ss.length > 1) {
        $.messager.alert("提示", "一次只能确认一个订单！", "info");
        return false;
    }

    if (ss.length > 0) {
        $.messager.confirm("提示", "确认以上订单吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../PromotionOrder/PromotionOrderSure",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderId: ss[0]
                    },
                    success: function (result) {
                        loading.close();
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

//取消按钮事件
function cancel() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var ss = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].StatusName == "等待确认") {
            ss.push(rows[i].OrderId);
        } else {
            $.messager.alert("提示", "等待确认状态才能取消！", "info");
            return false;
        }
    }
    if (ss.length > 0) {
        $.messager.confirm("提示", "取消以上订单吗?", function (r) {
            if (r) {
                $.ajax({
                    url: "../PromotionOrder/PromotionOrderCancel",
                    type: "get",
                    dataType: "json",
                    data: {
                        orderIdList: ss.join(','),
                        Status: 9,
                        CloseReason: ''
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "取消成功！", "info");
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


//查询
function search() {
    $('#grid').datagrid('clearSelections');
    initGrid(toolbarArray);
}

//重置
function resetSearch() {
    $("#searchform").form("clear");

    var data1 = $('#LineName').combobox('getData');  //赋默认值
    $("#LineName ").combobox('setValue', data1[0].LineID);

    var data2 = $('#WName').combobox('getData');  //赋默认值
    $("#WName ").combobox('setValue', data2[0].WID);

    $('#Status').combobox('setValue', 1);
    $('#ShopType').combobox('setValue', '');
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


