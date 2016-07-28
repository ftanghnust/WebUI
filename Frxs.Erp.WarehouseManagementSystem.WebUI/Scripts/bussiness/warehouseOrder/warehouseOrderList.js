$(function () {
    //grid绑定
    initGrid(toolbarArray, permission);

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

});

function initGrid(toolbarArray, permission) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WarehouseOrder/GetWarehouseOrderList',          //Aajx地址
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
        showFooter: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function () {
            var rows = $('#grid').datagrid('getSelections');
            frxs.openNewTab("查看门店订单" + rows[0].OrderId, "../WarehouseOrder/WarehouseOrderAddOrEdit?OrderID=" + rows[0].OrderId, "icon-search");
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            totalCalculate();
            gridresize();
        },
        queryParams: {
            //查询条件
            OrderId: $.trim($("#OrderId").val()),
            Status: $("#Status").combobox("getValue"),
            ShopCode: $.trim($("#ShopCode").val()),
            ShopName: $.trim($("#ShopName").val()),
            SubWID: $("#WName").combobox("getValue"),
            LineID: $("#LineName").combobox("getValue"),
            SendDateBegin: $.trim($("#SendStartDate").val()),
            SendDateEnd: $.trim($("#SendEndDate").val()),
            OrderDateBegin: $.trim($("#OrderStartDate").val()),
            OrderDateEnd: $.trim($("#OrderEndDate").val()),
            StationNumber: $.trim($("#StationNumber").val()),
            OrderType: $("#OrderType").combobox("getValue"),
            ConfDateBegin: $.trim($("#ConfStartDate").val()),
            ConfDateEnd: $.trim($("#ConfEndDate").val()),
            ShopType: $("#ShopType").combobox("getValue"),
            SKU: $.trim($("#SKU").val())
        },
        frozenColumns: [[
            { field: 'ck', checkbox: true },
            { title: '订单编号', field: 'OrderId', width: 100, align: 'center' },
            //冻结列
        ]],
        columns: [[
            { title: '下单时间', field: 'OrderDate', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '确认时间', field: 'ConfDate', width: 120, formatter: frxs.dateFormat, align: 'center' },
            { title: '门店编号', field: 'ShopCode', width: 100, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 170 },
            {
                title: '预计配送日期', field: 'SendDate', width: 120, align: 'center',
                formatter: function (value, rec) {
                    if (value && value.indexOf("合计") < 0) {
                        return value.DataFormat("yyyy-MM-dd");
                    } else {
                        return value;
                    }
                }
            },

            { title: '订单金额', field: 'PayAmount', width: 130, align: 'right',formatter:function (value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '状态', field: 'Status', width: 70, hidden: true },
            { title: '状态', field: 'StatusName', width: 100, align: 'center' },
            {
                field: 'opt',
                title: '操作',
                align: 'center',
                width: 130,
                formatter: function (value, rec) {
                    var str = "";
                    switch (rec.Status) {
                        case 1:  //等待确认
                            if (permission.sure) {
                                str += "<a class='rowbtn' onclick=\"sure('" + rec.OrderId + "')\">确认</a>";
                            }
                            if (permission.cancel) {
                                str += "<a class='rowbtn' onclick=\"cancel('" + rec.OrderId + "')\">取消订单</a>";
                            }
                            break;
                        case 2:  //等待拣货
                            if (permission.startpick) {
                                str += "<a class='rowbtn' onclick=\"pickStart('" + rec.OrderId + "')\">开始拣货</a>";
                                str += "<a class='rowbtn' onclick=\"cancel('" + rec.OrderId + "')\">取消订单</a>";
                            }
                            break;
                        case 3:  //正在拣货
                            if (permission.pickfinish) {
                                str += "<a class='rowbtn' onclick=\"pickFinish('" + rec.OrderId + "')\">拣货完成</a>";
                                str += "<a class='rowbtn' onclick=\"cancel('" + rec.OrderId + "')\">取消订单</a>";
                            }
                            break;
                        case 4:  //等待装箱
                            if (permission.startpack) {
                                str += "<a class='rowbtn' onclick=\"packChoose('" + rec.OrderId + "','" + rec.ShopID + "')\">装箱</a>";
                                str += "<a class='rowbtn' onclick=\"cancel('" + rec.OrderId + "')\">取消订单</a>";
                            }
                            break;
                        case 5:  //等待配送
                            if (permission.shipping) {
                                str += "<a class='rowbtn' onclick=\"shipping('" + rec.OrderId + "','" + rec.LineID + "')\">装车</a>";
                            }
                            break;
                        case 6:  //正在配送
                            if (rec.ShippingEndDate) {  //有配送结束时间
                                if (permission.dealfinish) {
                                    str += "<a class='rowbtn' onclick=\"dealFinish('" + rec.OrderId + "')\">交易完成</a>";
                                }
                            } else {
                                if (permission.shippingfinish) {
                                    str += "<a class='rowbtn' onclick=\"shippingFinish('" + rec.OrderId + "','" + rec.LineID + "')\">配送完成</a>";
                                }
                            }
                            break;
                        case 7:  //交易完成
                            break;
                        case 8:  //客户交易取消
                            break;
                        case 9:  //客服交易关闭
                            break;
                    }
                    return str;
                }
            },
            { title: '门店类型', field: 'ShopTypeName', width: 80, align: 'center' },
            { title: '缺货率', field: 'StockOutRate', width: 70, align: 'center' },
            { title: '配送线路', field: 'LineName', width: 180 },
            {
                title: '配送日期', field: 'ShippingBeginDate', width: 120,
                formatter: function (value, rec) {
                    if (value) {
                        return value.DataFormat("yyyy-MM-dd");
                    }
                }, align: 'center'
            },
            { title: '待装区编号', field: 'StationNumber', width: 80, align: 'center', align: 'center' },

            { title: '门店ID', field: 'ShopID', width: 80, hidden: true },
            { title: '线路ID', field: 'LineID', width: 80, hidden: true }

        ]],
        toolbar: toolbarArray
    });
}



//详情按钮事件
function details() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        var thisdlg = frxs.dialog({
            title: "订单编号：" + rows[0].OrderId,
            url: "../WarehouseOrder/GetOrderDetails?OrderID=" + rows[0].OrderId,
            owdoc: window.top,
            width: 800,
            height: 500
        });

    }
}

//修改当次线路
function linechange() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        if (rows[0].Status <= 5) {
            var thisdlg = frxs.dialog({
                title: "修改当次线路",
                url: "../WarehouseOrder/LineChange?LineName=" + rows[0].LineName + "&LineID=" + rows[0].LineID + "&OrderID=" + rows[0].OrderId,
                owdoc: window.top,
                width: 410,
                height: 380,
                buttons: [{
                    id: 'btnChangeOk',
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
        } else {
            $.messager.alert("提示", "状态错误，不能修改！", "info");
        }
    }
}

//查看按钮事件
function view() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        frxs.openNewTab("查看门店订单" + rows[0].OrderId, "../WarehouseOrder/WarehouseOrderAddOrEdit?OrderID=" + rows[0].OrderId, "icon-search");
    }
}

//新增按钮事件
function add() {
    frxs.openNewTab("添加门店订单", "../WarehouseOrder/WarehouseOrderAddOrEdit", "icon-add");
}

//确认订单
function sure(orderid) {
    if (orderid) {
        $.messager.confirm("提示", "确认订单【" + orderid + "】吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WarehouseOrder/OrderSure",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderID: orderid
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
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//取消订单
function cancel(orderid) {
    if (orderid) {
        $.messager.confirm("提示", "取消订单【" + orderid + "】吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WarehouseOrder/OrderCancel",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderID: orderid,
                        Status: 9
                    },
                    success: function (result) {
                        loading.close();
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
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//开始拣货
function pickStart(orderid) {
    if (orderid) {
        $.messager.confirm("提示", "确认订单【" + orderid + "】开始拣货吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WarehouseOrder/OrderPickStart",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderID: orderid
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "操作成功！", "info");
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
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//拣货
function pickFinish(orderid) {
    var thisdlg = frxs.dialog({
        title: "拣货数量确认",
        url: "../WarehouseOrder/PickFinish?OrderID=" + orderid,
        owdoc: window.top,
        width: 900,
        height: 500,
        buttons: [{
            id: 'btnPickFinishOk',
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


//装箱选择
var dlgChoose;
function packChoose(orderid, shopid) {
    dlgChoose = frxs.dialog({
        title: "装箱信息录入",
        url: "../WarehouseOrder/PackChoose?OrderID=" + orderid + "&ShopID=" + shopid,
        owdoc: window.top,
        width: 410,
        height: 180
    });
}

//装箱关闭弹出
function closeSonOpenPackDialog(orderid, shopid) {
    dlgChoose.dialog("close");
    packStart(orderid, shopid);
}

//对货关闭弹出
function closeSonOpenCheckDialog(orderid, shopid) {
    dlgChoose.dialog("close");
    packCheck(orderid, shopid);
}

//开始装箱
function packStart(orderid, shopid) {
    var thisdlg = frxs.dialog({
        title: "装箱信息确认",
        url: "../WarehouseOrder/PackStart?OrderID=" + orderid + "&ShopID=" + shopid,
        owdoc: window.top,
        width: 410,
        height: 320,
        buttons: [{
            id: 'btnPackOk',
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

//开始对货
function packCheck(orderid, shopid) {
    var thisdlg = frxs.dialog({
        title: "发货数量确认",
        url: "../WarehouseOrder/PickCheck?OrderID=" + orderid + "&ShopID=" + shopid,
        owdoc: window.top,
        width: 900,
        height: 500,
        buttons: [{
            id: 'btnPickCheckOk',
            text: '开始装箱',
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

//装车
function shipping(orderid, lineid) {
    if (orderid) {
        $.messager.confirm("提示", "确认订单【" + orderid + "】装车吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WarehouseOrder/OrderShipping",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderID: orderid,
                        LineID: lineid
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "操作成功！", "info");
                            } else {
                                $.messager.alert("提示", result.Info, "error");
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
                            $.messager.alert("提示", "出现错误", "error");
                        }
                    }
                });
            }
        });
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//配送完成
function shippingFinish(orderid, lineid) {
    if (orderid) {
        $.messager.confirm("提示", "确认订单【" + orderid + "】配送完成吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WarehouseOrder/OrderShippingFinish",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderID: orderid,
                        LineID: lineid
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "操作成功！", "info");
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
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//交易完成
function dealFinish(orderid) {
    if (orderid) {
        $.messager.confirm("提示", "确认订单【" + orderid + "】交易完成吗?", function (r) {
            if (r) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WarehouseOrder/OrderDealFinish",
                    type: "get",
                    dataType: "json",
                    data: {
                        OrderID: orderid
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
                                $.messager.alert("提示", "操作成功！", "info");
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
    else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//查询
function search() {
    $('#grid').datagrid('clearSelections');
    initGrid(toolbarArray, permission);
}

//重置
function resetSearch() {
    $("#searchform").form("clear");

    var data1 = $('#LineName').combobox('getData');  //赋默认值
    $("#LineName ").combobox('setValue', data1[0].LineID);

    var data2 = $('#WName').combobox('getData');  //赋默认值
    $("#WName ").combobox('setValue', data2[0].WID);

    $('#Status').combobox('setValue', '');
    $('#ShopType').combobox('setValue', '');
    $('#OrderType').combobox('setValue', '');
}


//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totalSubAmt = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var subAmt = parseFloat(rows[i].PayAmount);
        totalSubAmt += subAmt;
    }

    //$("#TotalOrderAmt").val(parseFloat(totalSubAmt).toFixed(4));
    $('#grid').datagrid('reloadFooter', [
       { "SendDate": "<span style='float:right'>当前页合计：</span>", "PayAmount": parseFloat(totalSubAmt).toFixed(4) },
       { "SendDate": "<span style='float:right'>总合计：</span>", "PayAmount": $("#grid").datagrid("getData").SubAmt }
    ]);
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
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}


