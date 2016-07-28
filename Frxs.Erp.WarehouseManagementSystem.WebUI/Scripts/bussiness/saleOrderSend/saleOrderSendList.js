var dialogWidth = 800;
var dialogHeight = 600;
var currentLineId = -1;
var selectOrderId = -1;

$(function () {
    init();
    //initDDLLine(null);
    //grid绑定
    initgridLine();

    //grid高度改变
    gridresize();
});

function init() {
    //$("#searchform").submit(function () {
    //    //alert("submit");
    //    search();
    //    return false;
    //});
    $("#aSearch").click(function () {
        search();
    });
    //重置按钮事件
    $("#aReset").click(function () {
        $("#searchform").form("clear");
    });
}

function search() {
    var shopCode = $.trim($("#ShopCode").val());
    var orderId = $.trim($("#OrderId").val());
    if (shopCode == "" && orderId == "") {
        initgridLine();
    } else {
        $.ajax({
            url: '../SaleOrderSend/SearchSaleOrderSend',
            type: "post",
            dataType: "json",
            data: { ShopCode: shopCode, OrderId: orderId },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        if (result.Data != null) {
                            //alert(result.Data.LineID);
                            //alert(result.Data.OrderId);
                            selectOrderId = result.Data.OrderId;
                            initgridShop(result.Data.LineID);

                            var rows = $('#gridLine').datagrid('getRows');
                            for (var i = 0; i < rows.length; i++) {
                                if (rows[i].LineID == result.Data.LineID) {
                                    var index = $("#gridLine").datagrid("getRowIndex", rows[i]);
                                    $('#gridLine').datagrid('clearSelections');
                                    $('#gridLine').datagrid('selectRow', index);
                                    break;
                                }
                            }

                        } else {
                            $.messager.alert("提示", "没有找到相关数据！", "info");
                        }
                    } else {
                        $.messager.alert("提示", "查询失败！", "info");
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
}

function initDDLLine(lineId) {
    $.ajax({
        url: '../Common/GetWarehouseLineList',
        type: 'get',
        data: {},
        success: function (data) {
            //alert(data);
            data = $.parseJSON(data);
            data.unshift({ "LineID": 0, "LineName": "-请选择-" });
            $("#LineID").combobox({
                data: data,             
                valueField: "LineID",       
                textField: "LineName"      
            });
            $("#LineID").combobox('select', data[0].LineID);
            if (lineId != null && lineId != 0) {
                $("#LineID").combobox('select', lineId);
            }
        }, error: function (e) {

        }
    });
}

function initgridLine() {
    $('#gridLine').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
       // width: 600,
        //height:700,
        url: '../SaleOrderSend/GetSaleOrderLineList',          //Aajx地址
        sortName: 'LineSerialNumber',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'LineSerialNumber',                  //主键
        pageSize: 5,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex, rowData) {
            loadShop(rowData.LineID);
            $('#gridLine').datagrid('clearSelections');
            $('#gridLine').datagrid('selectRow', rowIndex);
        },
        onDblClickCell: function (rowIndex) {
            // edit(rowIndex);
        },
        queryParams: {
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: false, hidden: true }, //选择
            { title: 'LineID', field: 'LineID', width: 130, hidden: true },
            { title: '线路顺序', field: 'LineSerialNumber', width: 90, align: 'center', hidden: true },
            { title: '线路编号', field: 'LineCode', width: 90, align: 'center' },
            { title: '线路名称', field: 'LineName', width: 160 },
            {
                field: 'opt',
                title: '操作',
                align: 'center',
                width: 180,
                formatter: function (value, rec) {
                    var str = "";
                    if (authUp) {
                        str += "<a class='rowbtn' onclick=\"javascript:upline(" + rec.LineID + ")\">上移</a>";
                    }
                    if (authDown) {
                        str += "<a class='rowbtn' onclick=\"javascript:downline(" + rec.LineID + ")\">下移</a>";
                    }
                    if (authTop) {
                        str += "<a class='rowbtn' onclick=\"javascript:topline(" + rec.LineID + ")\">置顶</a>";
                        str += "<a class='rowbtn' onclick=\"javascript:bottomline(" + rec.LineID + ")\">置底</a>";
                    }
                    return str;
                }
            }
        ]],
        //toolbar: [{
        //    id: 'btnReload',
        //    text: '刷新',
        //    iconCls: 'icon-reload',
        //    handler: function () {
        //        //实现刷新栏目中的数据
        //        $("#gridLine").datagrid("reload");
        //    }
        //    }
        //],
        toolbar: vartoolbar1,
        onLoadSuccess: function () {
            $('#gridLine').datagrid('clearSelections');
            var rows = $('#gridLine').datagrid("getRows");
            if (rows != null && rows.length > 0) {
                var lineId = rows[0].LineID;
                currentLineId = -1
                loadShop(lineId);
            }
        }
    });
}

function initgridShop(lineId) {
    var ajaxUrl = "";
    if (lineId != null) {
        ajaxUrl = '../SaleOrderSend/GetSaleOrderShopList?lineId=' + lineId;
    }
    $('#gridShop').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        //width: 1000,
        //height: 700,
        url: ajaxUrl,          //Aajx地址
        sortName: 'ShopSerialNumber',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopSerialNumber',                  //主键
        pageSize: 5,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#gridShop').datagrid('clearSelections');
            $('#gridShop').datagrid('selectRow', rowIndex);
        },
        onDblClickCell: function (rowIndex) {
            // edit(rowIndex);
        },
        queryParams: {
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: false, hidden: true }, //选择
            { title: 'LineID', field: 'LineID', width: 100, hidden: true },
            { title: '门店顺序', field: 'ShopSerialNumber', width: 80, align: 'center', hidden: true },
            { title: '订单编号', field: 'OrderId', width: 100, align: 'center' },
            { title: '线路名称', field: 'LineName', width: 100 },
             {
                 field: 'opt',
                 title: '操作',
                 align: 'center',
                 width: 180,
                 formatter: function (value, rec) {
                     var str = "";
                     if (authUp) {
                         str += "<a class='rowbtn' onclick=\"javascript:upshop(" + rec.LineID + ",'" + rec.OrderId + "')\">上移</a>";
                     }
                     if (authDown) {
                         str += "<a class='rowbtn' onclick=\"javascript:downshop(" + rec.LineID + ",'" + rec.OrderId + "')\">下移</a>";
                     }
                     if (authTop) {
                         str += "<a class='rowbtn' onclick=\"javascript:topshop(" + rec.LineID + ",'" + rec.OrderId + "')\">置顶</a>";
                         str += "<a class='rowbtn' onclick=\"javascript:bottomshop(" + rec.LineID + ",'" + rec.OrderId + "')\">置底</a>";
                     }
                     return str;
                 }
             },
            { title: '门店编号', field: 'ShopCode', width: 80, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 160 },
            //{ title: '门店类型', field: 'ShopType', width: 100 },
            {
                title: '门店类型', field: 'ShopType', width: 100, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "加盟店";
                    }
                    else if (value == "1") {
                        return "签约店";
                    }
                    else {
                        return "未知";
                    }
                }
            }
            
           
        ]],
        rowStyler:function(index,row){    
            //if (row.OrderId == selectOrderId) {
            //    return 'background-color:pink;color:#000;font-weight:bold;';    
            //}    
        },
        //toolbar: [{
        //    id: 'btnReload',
        //    text: '刷新',
        //    iconCls: 'icon-reload',
        //    handler: function () {
        //        //实现刷新栏目中的数据
        //        $("#gridShop").datagrid("reload");
        //    }
        //}
        //],
        toolbar: vartoolbar2,
        onLoadSuccess: function () {
            $('#gridShop').datagrid('clearSelections');
            //gridresize();
        }
    });
    gridresize();
}

function loadShop(lineId) {
    if (currentLineId != lineId) {
        initgridShop(lineId);
        currentLineId = lineId;
    }
}


function upline(lineId) {
    var rows = $('#gridLine').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[0];
        if (lineId == row.LineID) {
            $.messager.alert("提示", "已经置顶，不能再上移！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "上移该线路发货顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderLineOrderUp',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, changeType: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridLine();
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

function downline(lineId) {
    var rows = $('#gridLine').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[rows.length - 1];
        if (lineId == row.LineID) {
            $.messager.alert("提示", "已经置底，不能再下移！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "下移该线路发货顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderLineOrderDown',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, changeType: 2 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridLine();
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

function topline(lineId) {
    var rows = $('#gridLine').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[0];
        if (lineId == row.LineID) {
            $.messager.alert("提示", "已经置顶，不能再置顶！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "置顶该线路发货顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderLineOrderTop',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, changeType: 3 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridLine();
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

function bottomline(lineId) {
    var rows = $('#gridLine').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[rows.length-1];
        if (lineId == row.LineID) {
            $.messager.alert("提示", "已经置底，不能再置底！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "置底该线路发货顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderLineOrderBottom',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, changeType: 4 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridLine();
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

function upshop(lineId, orderId) {
    var rows = $('#gridShop').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[0];
        if (lineId == row.LineID && orderId == row.OrderId) {
            $.messager.alert("提示", "已经置顶，不能再上移！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "上移该门店顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderShopOrderUp',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, orderId: orderId, changeType: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridShop(currentLineId);
                            //currentLineId = -1;
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

function downshop(lineId, orderId) {
    var rows = $('#gridShop').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[rows.length - 1];
        if (lineId == row.LineID && orderId == row.OrderId) {
            $.messager.alert("提示", "已经置底，不能再下移！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "下移该门店顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderShopOrderDown',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, orderId: orderId, changeType: 2 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridShop(currentLineId);
                            //currentLineId = -1;
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

function topshop(lineId, orderId) {
    var rows = $('#gridShop').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[0];
        if (lineId == row.LineID && orderId == row.OrderId) {
            $.messager.alert("提示", "已经置顶，不能再置顶！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "置顶该门店顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderShopOrderTop',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, orderId: orderId, changeType: 3 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridShop(currentLineId);
                            //currentLineId = -1;
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

function bottomshop(lineId, orderId) {
    var rows = $('#gridShop').datagrid('getRows');
    if (rows.length > 0) {
        var row = rows[rows.length - 1];
        if (lineId == row.LineID && orderId == row.OrderId) {
            $.messager.alert("提示", "已经置底，不能再置底！", "info");
            return;
        }
    }
    $.messager.confirm("提示", "置底该门店顺序？", function (r) {
        if (r) {
            $.ajax({
                url: '../SaleOrderSend/ChangeSaleOrderShopOrderBottom',
                type: "get",
                dataType: "json",
                data: { lineId: lineId, orderId: orderId, changeType: 4 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            initgridShop(currentLineId);
                            //currentLineId = -1;
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


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 22);
    $('#gridLine').datagrid('resize', {
        width: $(window).width()/2 - 10,
        height: h
    });
    $('#gridShop').datagrid('resize', {
        width: $(window).width()/2 - 10,
        height: h
    });
}