//全局变量
var orderID;
//切换提示
var comboVal;

//是否排单订单
var isPd = false;

$(function () {
    isPd = frxs.getUrlParam("isPd");
    //下拉绑定
    initDDL();

    //供应商-采购员事件绑定
    eventBind();

    //加载详情数据
    //loadgrid();

    //加载主表数据
    init();

    //高度自适应
    //gridresize();

});


//查询历史  打开
function showHistory() {
    var temp = getCookie("WarehouseOrder");
    var obj = JSON.parse(temp);
    var htmltr = "<table><thead><th>单号</th><th style=\"text-align:left\">门店编号-名称</th><th>操作</th></thead>";
    $.each(obj, function () {
        htmltr = htmltr + "<tr><td width=\"100px\" height=\"24\" style=\"cursor:pointer\" onclick=\"loadData(this)\">" + this.ID + "</td><td width=\"300px\" style=\"text-align:left;\">" + this.VendorCode + "-" + this.VendorName + "</td><td><a href=\"#\" onclick=\"removeHistory(this)\">移除</a></td></tr>";
    });

    htmltr = htmltr + "</table>";
    $("#history").empty().append(htmltr);
    $(".history").toggle(200);
}

// 查询历史 记录单击
function loadData(obj) {
    var orderid = $(obj).text();
    window.location.href = "../WarehouseOrder/WarehouseOrderAddOrEdit?OrderID=" + orderid;
    frxs.updateTabTitle("查看门店订单" + orderid, "icon-search");
}

//查询历史 删除
function removeHistory(obj) {
    var orderid = $(obj).parents("tr").find("td:eq(0)").text();
    var temp = getCookie("WarehouseOrder");
    var objs = JSON.parse(temp);

    var result = [];
    for (var i = 0; i < objs.length; i++) {
        if (objs[i].ID != orderid) {
            result.push(objs[i]);
        }
    }
    delCookie("WarehouseOrder");
    if (result.length != 0) {
        addCookie("WarehouseOrder", JSON.stringify(result), 0);
        $(obj).parents("tr").remove();
    } else {
        $(obj).parents("tr").remove();
        $("#history").find("table").remove();
    }
}

//获取cookies
function getCookie(cookie_name) {
    var allcookies = document.cookie;
    var cookie_pos = allcookies.indexOf(cookie_name);   //索引的长度  
    // 如果找到了索引，就代表cookie存在，  
    // 反之，就说明不存在。  
    if (cookie_pos != -1) {
        // 把cookie_pos放在值的开始，只要给值加1即可。  
        cookie_pos += cookie_name.length + 1;      //这里我自己试过，容易出问题，所以请大家参考的时候自己好好研究一下。。。  
        var cookie_end = allcookies.indexOf(";", cookie_pos);
        if (cookie_end == -1) {
            cookie_end = allcookies.length;
        }
        var value = allcookies.substring(cookie_pos, cookie_end); //这里就可以得到你想要的cookie的值了。。。  
    }
    return value;
}

//删除cookies
function delCookie(name) {//为了删除指定名称的cookie，可以将其过期时间设定为一个过去的时间 
    var date = new Date();
    date.setTime(date.getTime() - 10000);
    document.cookie = name + "=a; expires=" + date.toGMTString() + ";path=/";
}

//新增cookies
function addCookie(objName, objValue, objHours) {//添加cookie 
    var str = objName + "=" + objValue;
    if (objHours > 0) {//为0时不设定过期时间，浏览器关闭时cookie自动消失 
        var date = new Date();
        var ms = objHours * 3600 * 1000;
        date.setTime(date.getTime() + ms);
        str += "; expires=" + date.toGMTString();
    }
    document.cookie = str + ";path=/";
}

//加载数据
function init() {
    orderID = frxs.getUrlParam("OrderID");
    if (orderID) {
        //编辑
        $.ajax({
            url: "../WarehouseOrder/GetWarehouseOrdeInfo",
            type: "post",
            data: { OrderID: orderID },
            dataType: 'json',
            success: function (obj) {
                //格式化日期
                obj.Order.OrderDate = frxs.dateFormat(obj.Order.OrderDate);
                obj.Order.CreateTime = frxs.dateFormat(obj.Order.CreateTime);
                //obj.Order.ConfTime = frxs.dateFormat(obj.Order.ConfTime);
                $('#OrderForm').form('load', obj.Order);
                $("#hidShopCode").val(obj.Order.ShopCode);

                loadgrid(obj.Details);

                

                //高度自适应
                gridresize();

                if (obj.Order.Status == 1) {
                    SetPermission('query');
                }
                else if (obj.Order.Status == 2) {
                    SetPermission('sure');
                } else {
                    SetPermission('other');
                }

                var rows = $("#grid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    var index = $('#grid').datagrid('getRowIndex', rows[i]);

                    rows[i].PreQty = parseFloat(rows[i].PreQty).toFixed(2);
                    rows[i].SaleQty = parseFloat(rows[i].SaleQty).toFixed(2);
                    rows[i].UnitQty = parseFloat(rows[i].UnitQty).toFixed(2);
                    rows[i].SalePackingQty = parseFloat(rows[i].SalePackingQty).toFixed(2);
                    rows[i].Stock = parseFloat(rows[i].Stock).toFixed(2);
                    rows[i].OnTheWay = parseFloat(rows[i].OnTheWay).toFixed(2);
                    rows[i].Available = parseFloat(rows[i].Available).toFixed(2);
                    rows[i].ShopAddPerc = parseFloat(rows[i].ShopAddPerc).toFixed(2);
                    rows[i].SalePrice = parseFloat(rows[i].SalePrice).toFixed(4);
                    rows[i].SubAmt = parseFloat(rows[i].SubAmt).toFixed(4);

                    rows[i].MinSalePrice = rows[i].UnitPrice;

                    //更新行
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: rows[i]
                    });
                }

                totalCalculate();

                //退货门店不可编辑
                $('#ShopCode').attr("readonly", "readonly");
                $("#btnSelShop").attr("disabled", true);

                $('#ShopCode').addClass("readonly");
                $('#ShopName').addClass("readonly");

                //下单时间
                if (obj.Order.Status != 1) {
                    $("#OrderDate").attr("disabled", true);
                    $("#OrderDate").css({ "backgroundColor": "#eee" });
                }

                //仓库不可编辑
                $("#SubWID").combobox("disable");
            }
        });
    } else {
        //添加
        $("#Status").combobox('setValue', '1');

        loadgrid();

        //高度自适应
        gridresize();

        //初始化添加一行
        addGridRow();

        SetPermission('add');


        $("#OrderDate").val(frxs.nowDateTime());
    }
}

//实现对DataGird控件的绑定操作
function loadgrid(griddata) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        data: griddata,
        idField: 'SKU',                  //主键
        //pageSize: 2000,                       //每页条数
        //pageList: [5, 10, 1000, 2000],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        showFooter: true,
        onClickRow: function (rowIndex) {
            //$('#grid').datagrid('unselectAll');
            $('#grid').datagrid('clearSelections'); //清除所有选中
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onClickCell: onClickCell,
        onLoadSuccess: function () {
            //
        },
        queryParams: {

        },
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }, //选择
            {
                title: frxs.setTitleColor('商品编码'), field: 'SKU', width: 100, align: 'center', editor: {
                    type: 'text',
                    options: {
                    }
                }
            },
            { title: '商品名称', field: 'ProductName', width: 180, formatter: frxs.formatText }
        ]],
        columns: [[
            {
                title: '单位', field: 'SaleUnit', width: 80, align: 'center', editor: {
                    type: 'combobox',
                    options: {
                        //url: '../Common/GetProductUnit?ProductID=2927',
                        //valueField: 'PackingQty',
                        valueField: 'PackingQty',
                        textField: 'UnitName',
                        editable: false,
                        panelHeight: 'auto',
                        onChange: function (value) {
                            //设置为Text

                            if (!isNaN(value)) {
                                var row = $("#grid").datagrid("getSelected");
                                var index = $("#grid").datagrid("getRowIndex", row);
                                //赋值包装数
                                row.SalePackingQty = value;

                                //计算数量=数量*包装数
                                row.UnitQty = parseFloat(row.SalePackingQty * row.SaleQty).toFixed(2);

                                //下拉框设置
                                var text = $(this).combobox("getText");
                                row.SaleUnit = text;
                                if (text == row.Unit) {
                                    row.SalePrice = row.MinSalePrice;
                                } else {
                                    row.SalePrice = row.MaxSalePrice;
                                }

                                row.PreQty = parseFloat(row.SaleQty).toFixed(2);

                                var sumPrice = row.SaleQty * row.SalePrice;
                                sumPrice = sumPrice * (parseFloat(1 + parseFloat(row.ShopAddPerc)));
                                row.SubAmt = parseFloat(sumPrice).toFixed(4);


                                $(".datagrid-row .combo").prev().combobox("hidePanel");
                                //更新行
                                $('#grid').datagrid('updateRow', {
                                    index: index,
                                    row: row
                                });
                                //更新总数
                                totalCalculate();
                            }
                        }
                    }
                }
            },
            { title: '订货数量', field: 'PreQty', width: 70, align: 'right' },
            {
                title: frxs.setTitleColor('销售数量'), field: 'SaleQty', width: 70, align: 'right', editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2,
                        min: 0
                    }
                }
            },
            {
                title: '配送价格', field: 'SalePrice', width: 100, align: 'right', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4) : "";
                }
            },
            { title: '平台费率', field: 'ShopAddPerc', width: 70, align: 'center' },
            {
                title: '小计金额', field: 'SubAmt', width: 160, align: 'right', formatter: function (value) {
                    value = value ? value : "0";
                    return isNaN(value)? value: parseFloat(value).toFixed(4);
                }
            },
            {
                title: '包装数', field: 'SalePackingQty', width: 70, align: 'right', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4) : "";
                }
            },
            {
                title: '总数量', field: 'UnitQty', width: 160, align: 'right', formatter: function (value) {
                    value = value ? value : "0";
                    return isNaN(value) ? value : parseFloat(value).toFixed(2);
                }
            },
            { title: '国际条码', field: 'BarCode', width: 100, align: 'center' },
            { title: '库存数量', field: 'Stock', width: 80, align: 'right' },
            { title: '在途数量', field: 'OnTheWay', width: 80, align: 'right' },
            { title: '可用数量', field: 'Available', width: 80, align: 'right' },
            {
                title: '推荐', field: 'IsAppendStr', width: 40, align: 'center',
                formatter: function (value, rec) {
                    var str = "";
                    switch (rec.IsAppend) {
                        case 0:
                            str += "否";
                            break;
                        case 1:
                            str += "是";
                            break;
                    }
                    return str;
                }
            },
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 80 },   //商品ID

            { title: 'MaxOrderQty', field: 'MaxOrderQty', hidden: true, width: 100 },
            { title: 'IsAppend', field: 'IsAppend', hidden: true, width: 100 },
            { title: 'BasePoint', field: 'BasePoint', hidden: true, width: 100 },
            { title: 'VendorPerc1', field: 'VendorPerc1', hidden: true, width: 100 },
            { title: 'VendorPerc2', field: 'VendorPerc2', hidden: true, width: 100 },
            { title: frxs.setTitleColor('备注'), field: 'Remark', width: 150, editor: 'text', formatter: frxs.formatText },

            { title: 'Unit', field: 'Unit', hidden: true, width: 80 },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 80 },    //商品最小单位价格

            { title: 'MinSalePrice', field: 'MinSalePrice', hidden: true },
            { title: 'MaxSalePrice', field: 'MaxSalePrice', hidden: true }
        ]],
        hideColumn: 'STAFF_ID',
        toolbar: [{
            text: '添加',
            id: 'btnAdd',
            iconCls: 'icon-add',
            handler: addGridRow
        }, '-', {
            text: '删除',
            id: 'btnDel',
            iconCls: 'icon-remove',
            handler: del
        }]
    });


};

//查询
function search() {
    if ($("#searchPlan").is(":hidden")) {
        $('#grid').datagrid('resize', {
            height: ($(window).height() - $("#searchPlan").height() - 19)
        });
        $("#searchPlan").show();
    } else {
        $('#grid').datagrid('resize', {
            height: ($(window).height() - 2)
        });
        $("#searchPlan").hide();
    }
}

//添加
function addGridRow() {

    var ciLen = $('#grid').datagrid('getRows').length - 1;
    var lastRow = $('#grid').datagrid('getRows')[ciLen];

    if (lastRow && lastRow.SKU == "") {

        $('#grid').datagrid('clearSelections');
        onClickCell($('#grid').datagrid('getRows').length - 1, "SKU");

        return false;
    } else {
        var index = $('#grid').datagrid('getRows').length;
        $("#grid").datagrid("insertRow", {
            index: index + 1,
            row: { SKU: "", ProductName: "", SaleUnit: "", SaleQty: "", PreQty: "", SalePackingQty: "", UnitQty: "", SalePrice: "", Stock: "", OnTheWay: "", Available: "", ShopAddPerc: "", SubAmt: "", Remark: "", BarCode: "" }
            //row: { SKU: "", ProductName: "", SaleUnit: "", SaleQty: "0.00", PreQty: "0.00", SalePackingQty: "0", UnitQty: "0.00", SalePrice: "0.0000", Stock: "0.00", OnTheWay: "0.00", Available: "0.00", ShopAddPerc: "0.00", SubAmt: "0.0000", Remark: "", BarCode: "" }

        });
        //onClickCell($('#grid').datagrid('getRows').length - 1, "SKU");

        return true;
    }
}

//删除
function del() {

    var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));
    $('#grid').datagrid('endEdit', index);

    var is = "";
    $(".datagrid-body:first table tr.datagrid-row").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            is = i + "," + is;
        }
    });

    var rows = $('#grid').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < is.split(',').length; j++) {
        var ci = is.split(',')[j];
        if (ci) {
            if (rows[ci].SKU) {
                if (rows[ci].IsAppend == 0) {
                    $.messager.alert("提示", " 商品【" + rows[ci].ProductName + "】不能删除！！", "info");       //非强配商品不能删除
                    return false;
                }
            }
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#grid').datagrid('getRowIndex', copyRows[i]);
        $('#grid').datagrid('deleteRow', ind);
    }
    totalCalculate();
}


//详情查看
function details() {
    var orderid = $("#OrderId").val();
    if (orderid != "") {
        var thisdlg = frxs.dialog({
            title: "订单编号：" + orderid,
            url: "../WarehouseOrder/GetOrderDetails?OrderID=" + orderid,
            owdoc: window.top,
            width: 800,
            height: 500
        });
    } else {
        $.messager.alert("提示", "订单编号不能为空", "info");
    }
}



//清空查询表单
function reset() {

}

//保存
function saveData() {
    var validate = $("#OrderForm").form('validate');
    if (!validate) {
        return false;
    }
    
    if ($("#SubWID").combobox("getValue")=="") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }

    if ($("#hidShopCode").val() != $("#ShopCode").val()) {
        $.messager.alert("提示", "订货门店编码有误，请重新选择！", "info");
        return false;
    }

    if ($("#OrderId").val() == "") {
        $.ajax({
            url: "../WarehouseOrder/OrderAddCheck",
            type: "post",
            data: { ShopID: $("#ShopID").val() },
            dataType: "json",
            success: function (result) {
                if (result.Flag != "SUCCESS") {
                    if (result.Info == "1") {
                        $.messager.alert("提示", "有未确认的订单，不能下单！", "info");
                        return false;
                    }
                    if (result.Info == "2") {
                        $.messager.confirm("提示", "有已确认的订单，是否创建新订单？", function (r) {
                            if (r) {
                                saveData1();
                            }
                        });
                    }
                } else {
                    saveData1();
                }
            }
        });
    } else {
        saveData1();
    }
}

function saveData1() {
    $('#grid').datagrid('endEdit', editIndex);
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            if (rows[i].IsAppend == 1) {
                if (rows[i].SaleQty == 0) {
                    $.messager.alert("提示", "商品【" + rows[i].ProductName + "】销售数量不能为零！", "info");
                    return false;
                }
            }
            jsonStr += "{";
            jsonStr += "\"BarCode\":\"" + (rows[i].BarCode == null ? "" : rows[i].BarCode) + "\",";
            jsonStr += "\"SalePackingQty\":\"" + rows[i].SalePackingQty + "\",";
            jsonStr += "\"SalePrice\":\"" + rows[i].SalePrice + "\",";
            jsonStr += "\"SaleQty\":\"" + rows[i].SaleQty + "\",";
            jsonStr += "\"SaleUnit\":\"" + rows[i].SaleUnit + "\",";
            jsonStr += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonStr += "\"Remark\":\"" + frxs.filterText(rows[i].Remark ? rows[i].Remark : "") + "\",";
            jsonStr += "\"SKU\":\"" + rows[i].SKU + "\",";
            jsonStr += "\"SubAmt\":\"" + rows[i].SubAmt + "\",";
            jsonStr += "\"IsAppend\":\"" + rows[i].IsAppend + "\",";
            jsonStr += "\"UnitPrice\":\"" + rows[i].UnitPrice + "\",";
            jsonStr += "\"Unit\":\"" + rows[i].Unit + "\",";
            jsonStr += "\"ShopAddPerc\":\"" + rows[i].ShopAddPerc + "\",";
            jsonStr += "\"BasePoint\":\"" + rows[i].BasePoint + "\",";
            jsonStr += "\"VendorPerc1\":\"" + rows[i].VendorPerc1 + "\",";
            jsonStr += "\"VendorPerc2\":\"" + rows[i].VendorPerc2 + "\",";
            jsonStr += "\"ProductId\":\"" + rows[i].ProductId + "\"";
            jsonStr += "},";
            count++;
        }
    }
    jsonStr = jsonStr.substring(0, jsonStr.length - 1);
    jsonStr += "]";
    if (count == 0) {
        $.messager.alert("提示", "商品数据不能为空！", "info");
        return false;
    }

    var loading = frxs.loading("正在加载中，请稍后...");
    var jsonBuyOrder = "{";
    jsonBuyOrder += "\"OrderID\":\"" + $("#OrderId").val() + "\",";
    jsonBuyOrder += "\"OrderDate\":\"" + $("#OrderDate").val() + "\",";
    jsonBuyOrder += "\"SubWID\":\"" + $("#SubWID").combobox('getValue') + "\",";
    jsonBuyOrder += "\"ShopID\":\"" + $("#ShopID").val() + "\",";
    jsonBuyOrder += "\"ShopCode\":\"" + $("#ShopCode").val() + "\",";
    jsonBuyOrder += "\"ShopName\":\"" + $("#ShopName").val() + "\",";
    jsonBuyOrder += "\"Status\":\"" + $("#Status").combobox('getValue') + "\",";
    jsonBuyOrder += "\"Remark\":\"" + frxs.filterText($("#Remark").val()) + "\"";
    jsonBuyOrder += "}";

    $.ajax({
        url: "../WarehouseOrder/WarehouseOrderAddOrEditeHandle",
        type: "post",
        data: { jsonData: jsonBuyOrder, jsonDetails: jsonStr, isPD: isPd },
        dataType: "json",
        success: function (result) {
            loading.close();
            if (result.Flag == "SUCCESS") {
                if ($("#OrderId").val() != "") {
                    $.messager.alert("提示", "编辑成功", "info");
                } else {
                    $("#OrderId").val(result.Data.OrderID);
                    $("#CreateUserName").val(result.Data.UserName);
                    $("#CreateTime").val(result.Data.Time);
                    $.messager.alert("提示", "添加成功", "info");
                }
                frxs.updateTabTitle("查看门店订单" + result.Data.OrderID, "icon-search");

                if ($("#Status").combobox('getValue') == 2) {
                    SetPermission('sure');
                } else {
                    SetPermission("save");
                }

                //退货门店不可编辑
                $('#ShopCode').attr("readonly", "readonly");
                $("#btnSelShop").attr("disabled", true);

                $('#ShopCode').addClass("readonly");
                $('#ShopName').addClass("readonly");

                //仓库不可编辑
                $("#SubWID").combobox("disable");
            } else {
                $.messager.alert("提示", result.Info, "info");
            }
        }
    });
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 152);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
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
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName",      //value列
                onSelect: function (rec) {
                    if (comboVal > 0) {
                        var msgdlg = frxs.dialog({
                            title: "提示",
                            content: "切换仓库将清空已添加的商品数据。确定切换？",
                            width: 300,
                            height: 130,
                            modal: true,
                            minimizable: false,
                            maximizable: false,
                            buttons: [{
                                text: '&nbsp;确 定&nbsp;',
                                handler: function () {
                                    comboVal = rec.WID;
                                    msgdlg.dialog("close");
                                    $('#grid').datagrid('loadData', { total: 0, rows: [] });
                                    addGridRow();
                                }
                            }, {
                                text: '&nbsp;取 消&nbsp;',
                                handler: function () {
                                    $("#SubWID").combobox('setValue', comboVal);
                                    msgdlg.dialog("close");
                                }
                            }],
                            onClose: function () {
                                $("#SubWID").combobox('setValue', comboVal);
                            }
                        });
                    }
                }
            });
            $("#SubWID").combobox('select', data[0].WID);
            comboVal = data[0].WID;
        }, error: function (e) {

        }
    });
}

$.extend($.fn.datagrid.methods, {
    //编辑单元格事件
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }

            $(this).datagrid('beginEdit', param.index);

            $('#grid').datagrid('clearSelections'); //清除所有选中
            $('#grid').datagrid('selectRow', param.index);


            var ed = $(this).datagrid('getEditor', param);
            if (ed && ed.type && ed.type == "combobox") {

                //ed.target.combobox('getValue')
                //绑定单位字段
                var productId = $("#grid").datagrid("getSelected").ProductId;
                if (productId > 0) {
                    debugger;
                    var url = '../Common/GetSaleProductUnit?ProductID=' + productId;
                    ed.target.combobox('reload', url); //联动下拉列表重载
                }
            }
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
                    if (ed.field == "SaleQty") {
                        obj.attr("maxlength", "10");
                        obj.keyup(function () {
                            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                        }).bind("paste", function () {  //CTR+V事件处理    
                            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                        }).css("ime-mode", "disabled"); //CSS设置输入法不可用   
                    }
                    objEvent(obj, ed.field, param.index);
                } else {
                    objEvent($(ed.target), ed.field, param.index);
                }
            }

            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});

var valueinput = "";
//绑定对象事件
function objEvent(obj, field, index) {

    obj.focus();

    setTimeout(function () {
        obj.select();
    }, 100);
    valueinput = $(obj).val();

    if (field == "SaleQty") {
        obj.bind("change", function () {
            changeBuyQtyValue($(this).val());
        });
    }

    if (field == "SalePrice") {
        obj.bind("change", function () {
            changeBuyPriceValue($(this).val());
        });
    }


    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "SKU") {
                if ($(this).val() == "") {
                    showDialog("");
                    $(this).blur();
                } else if ($(this).val() != valueinput) {
                    showDialog($(this).val());
                    $(this).blur();
                    //$.messager.alert("提示", "弹窗,带入参数" + $(this).val());
                } else {
                    nextControl();
                }
            } else {
                if (field == "SaleQty") {
                    var row = $('#grid').datagrid('getSelected');
                    var index = $("#grid").datagrid("getRowIndex", row);
                    var temp = parseFloat($(this).val()) * parseFloat(row.SalePackingQty);
                    if (temp > 999999) {
                        row.SaleQty = "0.00";
                        $('#grid').datagrid('updateRow', {
                            index: index,
                            row: row
                        });
                        $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                        return false;
                    }
                    obj.change();
                }
                if (field == "SalePrice") {
                    obj.change();
                }
                nextControl();
            }
        }

        //向下
        if (e.keyCode == 40) {
            if (field == "SaleQty") {
                var row = $('#grid').datagrid('getSelected');
                var index = $("#grid").datagrid("getRowIndex", row);
                var temp = parseFloat($(this).val()) * parseFloat(row.SalePackingQty);
                if (temp > 999999) {
                    row.SaleQty = "0.00";
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                    return false;
                }
                obj.change();
            }
            if (field == "SalePrice") {
                obj.change();
            }
            var selected = $('#grid').datagrid('getSelected');
            var index = $('#grid').datagrid('getRowIndex', selected);
            var countIndex = $('#grid').datagrid('getRows');
            $('#grid').datagrid('clearSelections');

            if (index + 1 < countIndex.length) {
                onClickCell(index + 1, field);
            } else {
                onClickCell(0, field);
            }
        }

        //向下
        if (e.keyCode == 38) {
            if (field == "SaleQty") {
                var row = $('#grid').datagrid('getSelected');
                var index = $("#grid").datagrid("getRowIndex", row);
                var temp = parseFloat($(this).val()) * parseFloat(row.SalePackingQty);
                if (temp > 999999) {
                    row.SaleQty = "0.00";
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                    return false;
                }
                obj.change();
            }
            if (field == "SalePrice") {
                obj.change();
            }
            var selected = $('#grid').datagrid('getSelected');
            var index = $('#grid').datagrid('getRowIndex', selected);
            var countIndex = $('#grid').datagrid('getRows');
            $('#grid').datagrid('clearSelections');

            if (index > 0) {
                onClickCell(index - 1, field);
            } else {
                onClickCell(countIndex.length - 1, field);
            }
        }
    });

    //为SKU绑定移开变回事件
    if ($(obj).parents('td:eq(1)').attr("field") == "SKU") {
        obj.bind("blur", function () {
            $(this).val(valueinput);
        });
    }
}

//金额计算 并且更新grid的行数据
function changeBuyQtyValue(SaleQty) {
    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    //判断购买数量不能大于限购数
    var SalePackingQty = row.SalePackingQty;
    if (row.MaxOrderQty && SaleQty) {
        if (parseFloat(SaleQty) > parseFloat(row.MaxOrderQty)) {
            SaleQty = parseFloat(row.MaxOrderQty);   // parseFloat(SalePackingQty)
        }
    }
    //var SalePackingQty = row.SalePackingQty;
    var SalePrice = row.SalePrice;
    var ShopAddPerc = row.ShopAddPerc;

    row.UnitQty = parseFloat(parseFloat(SaleQty) * parseFloat(SalePackingQty)).toFixed(2);
    if (row.UnitQty > 999999) {
        row.SaleQty = "0.00";
        row.UnitQty = "0.00";
        $('#grid').datagrid('updateRow', {
            index: index,
            row: row
        });
        $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, 'SaleQty'); });
        return false;
    }

    row.SaleQty = parseFloat(SaleQty).toFixed(2);
    var temp = parseFloat(1 + parseFloat(ShopAddPerc)) * parseFloat(SalePrice);
    row.SubAmt = parseFloat(parseFloat(SaleQty) * temp).toFixed(4);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.SaleQty)) {
        row.SaleQty = "0.00";
    }
    if (isNaN(row.SubAmt)) {
        row.SubAmt = "0.0000";
    }

    row.PreQty = row.SaleQty;

    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    totalCalculate();
}


//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totalSubAmt = 0.0000;
    var totalUnitQty = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            var unitQty = parseFloat(rows[i].UnitQty);
            var subAmt = parseFloat(rows[i].SubAmt);
            totalUnitQty += unitQty;
            totalSubAmt += subAmt;
        }
    }

    $("#PayAmount").val(parseFloat(totalSubAmt).toFixed(4));
    $('#grid').datagrid('reloadFooter', [
       { "UnitQty": "数量总计：" + parseFloat(totalUnitQty).toFixed(2), "SubAmt": "金额总计：" + parseFloat(totalSubAmt).toFixed(4) }
    ]);

}


//金额计算 并且更新grid的行数据
function changeBuyPriceValue(SalePrice) {

    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    //判断采购价不能大于OldPrice价格
    //if (row.OldBuyPrice && SalePrice) {
    //    if (parseFloat(SalePrice) > parseFloat(row.OldBuyPrice)) {
    //        SalePrice = parseFloat(row.OldBuyPrice);
    //    }
    //}


    var SalePackingQty = row.SalePackingQty;
    var SaleQty = row.SaleQty;

    row.SalePrice = parseFloat(SalePrice).toFixed(4);
    row.UnitQty = parseFloat(parseFloat(SaleQty) * parseFloat(SalePackingQty)).toFixed(2);
    row.SaleQty = parseFloat(SaleQty).toFixed(2);
    row.SubAmt = parseFloat(parseFloat(SaleQty) * parseFloat(SalePrice)).toFixed(2);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.SaleQty)) {
        row.SaleQty = "0.00";
    }
    if (isNaN(row.SubAmt)) {
        row.SubAmt = "0.0000";
    }
    row.PreQty = row.SaleQty;
    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    totalCalculate();
}


//去下一个可以编辑框
function nextControl() {
    var selected = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', selected);
    var currentField = "SKU";

    //编辑字段
    var fields = "SKU,SaleQty".split(',');
    for (var i = 0; i < fields.length; i++) {
        var ciField = fields[i];
        if (ciField == editfield) {
            currentField = fields[i + 1];
            if (currentField == undefined) {
                currentField = "SKU";
                index = index + 1;
                $('#grid').datagrid('clearSelections');
                $('#grid').datagrid('getRowIndex', index);
            }
        }
    }

    //单位-下拉框回车处理
    if (editfield == "SaleUnit") {
        $('#grid').datagrid('updateRow', {
            index: index,
            row: selected
        });
        currentField = "SaleQty";
    }


    //Remark回车到下一行
    if (editfield == "Remark") {
        currentField = "SKU";
        index = index + 1;
    }

    var len = $("#grid").datagrid("getRows").length;
    if (len == index) {
        //插入行
        if (addGridRow()) {
            onClickCell(index, currentField);
        }
    } else {
        onClickCell(index, currentField);
    }
}

//点击编辑
var editIndex = undefined;
var editfield = undefined;
function endEditing() {
    if (editIndex == undefined) {
        return true;
    }
    if ($('#grid').datagrid('validateRow', editIndex)) {
        $('#grid').datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

//单击单元格事件
function onClickCell(index, field) {

    //判断当前是否需要添加-根据添加按钮是否禁用来判断
    if ($("#btnAdd").hasClass("l-btn-disabled") == false) {

        editfield = field;

        if (endEditing()) {
            $('#grid').datagrid('selectRow', index)
                .datagrid('editCell', { index: index, field: field });
            editIndex = index;
        }
    }
}

//供门店 回车事件绑定
function eventBind() {
    $("#ShopCode").keydown(function (e) {
        if (e.keyCode == 13) {
            eventShop();
        }
    });

    $("#ShopName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventShop();
        }
    });

}

//门店 回车事件
function eventShop() {
    $.ajax({
        url: "../Common/GetShopInfo",
        type: "post",
        data: {
            shopCode: $("#ShopCode").val(),
            //shopName: $("#ShopName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                var rows = $('#grid').datagrid("getRows");
                if ($("#ShopID").val() != "" && $("#ShopID").val() != obj.rows[0].ShopID) {
                    if (rows.length > 0 && rows[0].SKU) {
                        $.messager.confirm("提示", "确认更换订货门店吗?（将清除商品信息）", function (r) {
                            if (r) {
                                if (rows) {
                                    //for (var i = rows.length - 1; i >= 0; i--) {
                                    //    var index = $('#grid').datagrid('getRowIndex', rows[i]);
                                    //    $('#grid').datagrid('deleteRow', index);
                                    //}
                                    $('#grid').datagrid('loadData', { total: 0, rows: [] });
                                    addGridRow();
                                    $("#ShopID").val(obj.rows[0].ShopID);
                                    $("#ShopCode").val(obj.rows[0].ShopCode);
                                    $("#hidShopCode").val(obj.rows[0].ShopCode);
                                    $("#ShopName").val(obj.rows[0].ShopName);
                                }
                            } else {
                                return false;
                            }
                        });
                    } else {
                        $("#ShopID").val(obj.rows[0].ShopID);
                        $("#ShopCode").val(obj.rows[0].ShopCode);
                        $("#hidShopCode").val(obj.rows[0].ShopCode);
                        $("#ShopName").val(obj.rows[0].ShopName);
                    }
                } else {
                    $("#ShopID").val(obj.rows[0].ShopID);
                    $("#ShopCode").val(obj.rows[0].ShopCode);
                    $("#hidShopCode").val(obj.rows[0].ShopCode);
                    $("#ShopName").val(obj.rows[0].ShopName);
                }
            } else {
                selShop();
            }
        }
    });
}

//选择门店
function selShop() {
    var shopCode = $("#ShopCode").val();
    //var shopName = $("#ShopName").val();
    frxs.dialog({
        title: "选择门店",
        url: "../SaleBackPre/SelectShop?shopCode=" + encodeURIComponent(shopCode),//+ "&shopName=" + encodeURIComponent(shopName)
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填门店
function backFillShop(shopID, shopCode, shopName) {
    var rows = $('#grid').datagrid("getRows");
    if ($("#ShopID").val() != "" && $("#ShopID").val() != shopID) {
        if (rows.length > 0 && rows[0].SKU) {
            $.messager.confirm("提示", "确认更换订货门店吗?（将清除商品信息）", function (r) {
                if (r) {
                    if (rows) {
                        //for (var i = rows.length - 1; i >= 0; i--) {
                        //    var index = $('#grid').datagrid('getRowIndex', rows[i]);
                        //    $('#grid').datagrid('deleteRow', index);
                        //}
                        $('#grid').datagrid('loadData', { total: 0, rows: [] });
                        addGridRow();
                        $("#ShopID").val(shopID);
                        $("#ShopCode").val(shopCode);
                        $("#hidShopCode").val(shopCode);
                        $("#ShopName").val(shopName);
                    }
                } else {
                    return false;
                }
            });
        } else {
            $("#ShopID").val(shopID);
            $("#ShopCode").val(shopCode);
            $("#hidShopCode").val(shopCode);
            $("#ShopName").val(shopName);
        }
    } else {
        $("#ShopID").val(shopID);
        $("#ShopCode").val(shopCode);
        $("#hidShopCode").val(shopCode);
        $("#ShopName").val(shopName);
    }
}


//选择商品资料
function showDialog(parm) {
    var subWID = $("#SubWID").combobox('getValue');
    var shopID = $("#ShopID").val();
    
    if (shopID) {
        
        //仓库验证
        if(!subWID) {
            $.messager.alert("提示", "请先选择仓库", "info");
            return false;
        }

        var type = "SKU";
        //判断需要查询的类型
        var onebyte = parm ? parm.substring(0, 1) : "";
        if (isNaN(onebyte) && onebyte != "赠") {
            type = "ProductName";
        }

        $.ajax({
            url: "../Common/GetSaleProductStockInfo",
            type: "get",
            dataType: "json",
            data: {
                ShopID: shopID,
                SubWID: subWID,
                Value: parm,
                Type: type,
                SKULikeSearch: true
            },
            success: function (obj) {
                if (obj && obj.total == 1) {
                    backFillProduct(obj.rows[0]);
                } else {
                    frxs.dialog({
                        title: "选择商品资料",
                        url: "../WarehouseOrder/SearchSaleProductStock?productCode=" + parm + "&subWID=" + subWID + "&shopID=" + shopID,
                        owdoc: window.top,
                        width: 850,
                        height: 500
                    });
                }
            }
        });
    } else {
        $.messager.alert("提示", "请先选择订货门店", "info");
    }
}

//回填商品信息
function backFillProduct(product) {
    //判断是否重复
    var rep = false;
    var rows = $("#grid").datagrid("getRows");
    var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));

    //如果选中的是相同的数据 过滤
    //if ($('#grid').datagrid('getSelected').SKU == product.SKU) {
    //    return;
    //}

    for (var i = 0; i < rows.length; i++) {

        if (rows[i].SKU == product.SKU && index != i) {
            rep = true;
            break;
        }
    }
    if (rep) {
        $.messager.alert("提示", "存在相同商品，不能添加！", "info", function () { onClickCell(index, "SKU"); });
        return false;


        //var msgdlg = frxs.dialog({
        //    title: "提示",
        //    content: "存在重复数据。是否放弃操作？",
        //    width: 300,
        //    height: 130,
        //    modal: true,
        //    buttons: [{
        //        text: '放弃',
        //        iconCls: 'icon-no',
        //        handler: function () {
        //            msgdlg.dialog("close");
        //        }
        //    }, {
        //        text: '添加',
        //        iconCls: 'icon-add',
        //        handler: function () {
        //            backFillProduct2(product);
        //            msgdlg.dialog("close");
        //        }
        //    }]
        //});

        //$.messager.confirm("提示", "存在重复数据。是否继续？", function(r) {
        //    if(r) {
        //        backFillProduct2(product);
        //    }
        //});
    } else {
        backFillProduct2(product);
    }


}

//数据回填
function backFillProduct2(product) {
    var row = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', row);

    $('#grid').datagrid('endEdit', index);

    row.SKU = product.SKU;
    row.ProductName = product.ProductName;
    row.SaleUnit = product.BigUnit;
    row.SalePackingQty = parseFloat(product.BigPackingQty).toFixed(2);
    row.SalePrice = parseFloat(product.BigSalePrice).toFixed(4);
    row.BarCode = product.BarCode;
    row.ProductId = product.ProductId;
    row.UnitPrice = product.SalePrice;
    row.Unit = product.Unit;

    row.ShopAddPerc = parseFloat(product.ShopAddPerc).toFixed(2);
    row.Stock = parseFloat(product.Stock).toFixed(2);
    row.OnTheWay = parseFloat(product.OnTheWay).toFixed(2);
    row.Available = parseFloat(product.Available).toFixed(2);

    row.IsAppend = 1;  //默认为强配商品
    row.MaxOrderQty = product.MaxOrderQty;
    //row.BigShopPoint = product.BigShopPoint;
    row.BasePoint = product.BasePoint;
    row.VendorPerc1 = product.VendorPerc1;
    row.VendorPerc2 = product.VendorPerc2;

    row.MinSalePrice = product.SalePrice;
    row.MaxSalePrice = product.BigSalePrice;


    row.SaleQty = '0.00';
    row.UnitQty = '0.00';
    row.SubAmt = '0.0000';


    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });


    onClickCell(index, "SaleQty");
}



//订单 添加
function add() {
    location.href = '../WarehouseOrder/WarehouseOrderAddOrEdit';
    frxs.updateTabTitle("添加门店订单", "icon-add");
}

//订单 编辑
function edit() {
    SetPermission("edit");

    frxs.updateTabTitle("编辑门店订单" + $("#OrderId").val(), "icon-edit");
}

//订单状态 确认
function sure() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "等待确认状态才能确认！", "info");
        return false;
    }
    var orderid = $("#OrderId").val();
    if (orderid != "") {
        $.ajax({
            url: "../WarehouseOrder/OrderSure",
            type: "get",
            dataType: "json",
            data: {
                OrderID: orderid
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "确认成功！", "info");
                        $("#Status").combobox('setValue', '2');
                        //$("#ConfUserName").val(result.Data.UserName);
                        //$("#ConfTime").val(result.Data.Time);
                        SetPermission("sure");
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
}


//订单明细 导入
function importData() {
    var ShopID = $("#ShopID").val();
    var subWID = $("#SubWID").combobox('getValue');
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }
    if (ShopID) {
        var thisdlg = frxs.dialog({
            title: "数据导入",
            url: "../WarehouseOrder/ImportWarehouseOrderDetail?ShopID=" + ShopID + "&SubWID=" + subWID,
            owdoc: window.top,
            width: 825,
            height: 650,
            buttons: [{
                text: '导入',
                iconCls: 'icon-ok',
                handler: function () {
                    thisdlg.subpage.importData();
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
        $.messager.alert("提示", "请先选择订货门店", "info");
    }
}

//数据回填-导入
function backFillProductImport(products) {
    var rows = $("#grid").datagrid("getRows");

    var msg = [];
    for (var i = 0; i < products.length; i++) {
        var product = products[i];
        for (var j = 0; j < rows.length; j++) {
            if (rows[j].SKU == product.SKU) {
                //msg += product.SKU + " | ";
                msg.push(product.SKU);
            }
        }
    }
    if (msg.length > 0) {
        window.top.$.messager.alert("提示", "商品【" + msg.join('|') + "】重复", "info");
        return false;
    } else {

        //删除最后空行数据
        var ciLen = $('#grid').datagrid('getRows').length - 1;
        var lastRow = $('#grid').datagrid('getRows')[ciLen];
        if (lastRow && !lastRow.SKU) {
            $('#grid').datagrid('deleteRow', ciLen);
        }

        for (var i = 0; i < products.length; i++) {
            products[i].PreQty = parseFloat(products[i].PreQty).toFixed(2);
            products[i].SaleQty = parseFloat(products[i].SaleQty).toFixed(2);
            products[i].UnitQty = parseFloat(products[i].UnitQty).toFixed(2);
            products[i].SalePackingQty = parseFloat(products[i].SalePackingQty).toFixed(2);
            products[i].Stock = parseFloat(products[i].Stock).toFixed(2);
            products[i].OnTheWay = parseFloat(products[i].OnTheWay).toFixed(2);
            products[i].Available = parseFloat(products[i].Available).toFixed(2);
            products[i].ShopAddPerc = parseFloat(products[i].ShopAddPerc).toFixed(2);
            products[i].SalePrice = parseFloat(products[i].SalePrice).toFixed(4);
            products[i].SubAmt = parseFloat(products[i].SubAmt).toFixed(4);

            products[i].MaxSalePrice = products[i].BigSalePrice;
            products[i].MinSalePrice = products[i].UnitPrice;

            $('#grid').datagrid('appendRow', products[i]);
        }
        totalCalculate();
        return true;
    }
}

//订单 导出
function exportData() {
    var orderid = $("#OrderId").val();
    if (orderid != "") {
        location.href = "../WarehouseOrder/DataExport?OrderID=" + orderid;
    } else {
        $.messager.alert("提示", "订单号为空！", "info");
    }
}

//打印拣货单
function printSalePick() {
    var orderid = $("#OrderId").val();
    if (orderid != "") {
        var thisdlg = frxs.dialog({
            title: "打印拣货单",
            url: "../FastReportTemplets/Aspx/PrintSalePick.aspx?OrderID=" + orderid,
            owdoc: window.top,
            width: 780,
            height: 600
        });
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//打印出库单
function printSaleOut() {
    var status = $("#Status").combobox('getValue');
    if (status == 5 || status == 6 || status == 7) {
        var orderid = $("#OrderId").val();
        if (orderid != "") {
            var thisdlg = frxs.dialog({
                title: "打印出库单",
                url: "../FastReportTemplets/Aspx/PrintSaleOut.aspx?OrderID=" + orderid,
                owdoc: window.top,
                width: 760,
                height: 600
            });
        } else {
            $.messager.alert("提示", "订单号不能为空！", "info");
        }
    } else {
        $.messager.alert("提示", "该状态不能打印出库单！", "info");
    }
}

//打印结算单
function printSaleSettle() {
    var status = $("#Status").combobox('getValue');
    if (status == 5 || status == 6 || status == 7) {
        var orderid = $("#OrderId").val();
        if (orderid != "") {
            var thisdlg = frxs.dialog({
                title: "打印结算单",
                url: "../FastReportTemplets/Aspx/PrintSaleSettle.aspx?OrderID=" + orderid,
                owdoc: window.top,
                width: 760,
                height: 600
            });
        } else {
            $.messager.alert("提示", "订单号不能为空！", "info");
        }
    } else {
        $.messager.alert("提示", "该状态不能打印结算单！", "info");
    }
}


//打印二维码
function printSaleQrCode() {
    var status = $("#Status").combobox('getValue');
    if (status >= 3 && status <= 7) {
        var orderid = $("#OrderId").val();
        if (orderid != "") {
            var thisdlg = frxs.dialog({
                title: "打印二维码",
                url: "../FastReportTemplets/Aspx/PrintSaleQrCode.aspx?OrderID=" + orderid,
                owdoc: window.top,
                width: 375,
                height: 500
            });
        } else {
            $.messager.alert("提示", "订单号不能为空！", "info");
        }
    } else {
        $.messager.alert("提示", "该状态不能打印二维码！", "info");
    }
}


//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#addBtn").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#importBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnDetails").linkbutton('disable');
            $("#printSaleOut").linkbutton('disable');
            $("#printSaleSettle").linkbutton('disable');
            $("#printSaleQrCode").linkbutton('disable');
            break;
        case "edit":
            $("#addBtn").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnDetails").linkbutton('enable');
            $("#printSaleOut").linkbutton('enable');
            $("#printSaleSettle").linkbutton('enable');
            $("#printSaleQrCode").linkbutton('enable');
            break;
        case "query":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnDetails").linkbutton('enable');
            $("#printSaleOut").linkbutton('enable');
            $("#printSaleSettle").linkbutton('enable');
            $("#printSaleQrCode").linkbutton('enable');
            break;
        case "sure":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnDetails").linkbutton('enable');
            $("#printSaleOut").linkbutton('enable');
            $("#printSaleSettle").linkbutton('enable');
            $("#printSaleQrCode").linkbutton('enable');
            break;
        case "other":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnDetails").linkbutton('enable');
            $("#printSaleOut").linkbutton('enable');
            $("#printSaleSettle").linkbutton('enable');
            $("#printSaleQrCode").linkbutton('enable');
            break;
        case "save":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnDetails").linkbutton('enable');
            $("#printSaleOut").linkbutton('enable');
            $("#printSaleSettle").linkbutton('enable');
            $("#printSaleQrCode").linkbutton('enable');
            break;
    }

}

