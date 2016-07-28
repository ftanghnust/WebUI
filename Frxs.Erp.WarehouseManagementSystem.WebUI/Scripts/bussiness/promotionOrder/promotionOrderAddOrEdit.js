//全局变量
var orderID;
$(function () {

    //下拉绑定
    initDDL();

    //供应商-采购员事件绑定
    eventBind();


    //加载主表数据
    init();

});


//加载数据
function init() {
    orderID = frxs.getUrlParam("OrderID");
    if (orderID) {
        //编辑
        $.ajax({
            url: "../PromotionOrder/GetPromotionOrderInfo",
            type: "post",
            data: { OrderID: orderID },
            dataType: 'json',
            success: function (obj) {
                //格式化日期
                obj.Order.OrderDate = frxs.dateFormat(obj.Order.OrderDate);
                obj.Order.CreateTime = frxs.dateFormat(obj.Order.CreateTime);
                obj.Order.ConfTime = frxs.dateFormat(obj.Order.ConfTime);

                $('#OrderForm').form('load', obj.Order);

                loadgrid(obj.Details);

                totalCalculate();

                //高度自适应
                gridresize();

                //if (obj.Order.Status == 1) {
                //    SetPermission('query');
                //}
                //else if (obj.Order.Status == 2) {
                //    SetPermission('sure');
                //} else {
                //    SetPermission('other');
                //}
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

        //SetPermission('add');

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
        rowStyler: function (index, row) {
            if (row.IsAppend == 1) {
                return 'background-color:#6293BB;color:#fff;font-weight:bold;';
            }
        },
        queryParams: {

        },
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }, //选择
            {
                title: '商品编码', field: 'SKU', width: 100, editor: {
                    type: 'text',
                    options: {
                    }
                }
            },
            { title: '商品名称', field: 'ProductName', width: 180, formatter: frxs.replaceCode }
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

                                //计算数量=总数量/包装数
                                var unitQty = parseFloat(parseFloat(row.UnitQty) / parseFloat(value)).toFixed(2);
                                row.SaleQty = unitQty;
                                //计算采购价=UnitPrice*包装数
                                var SalePrice = parseFloat(parseFloat(value) * parseFloat(row.UnitPrice)).toFixed(4);
                                row.SalePrice = SalePrice;

                                //下拉框设置
                                var text = $(this).combobox("getText");
                                row.SaleUnit = text;

                                $(".datagrid-row .combo").prev().combobox("hidePanel");
                                //更新行
                                $('#grid').datagrid('updateRow', {
                                    index: index,
                                    row: row
                                });
                            }

                        }
                    }
                }
            },
            {
                title: '订货数量', field: 'SaleQty', width: 80, align: 'right', editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2
                    }
                }
            },
            {
                title: '配送价格', field: 'SalePrice', width: 100, align: 'right'
            },
            {
                title: '平台费率', field: 'ShopAddPerc', width: 70, align: 'right'
            },
            {
                title: '小计金额', field: 'SubAmt', width: 160, align: 'right'
            },
            {
                title: '备注', field: 'Remark', width: 180, editor: 'text', formatter: frxs.replaceCode
            },
            { title: '包装数', field: 'SalePackingQty', width: 80, align: 'right' },
            { title: '总数量', field: 'UnitQty', width: 160, align: 'right' },
            //{ title: '库存数量', field: 'Stock', width: 80, align: 'right' },
            //{ title: '在途数量', field: 'OnTheWay', width: 80, align: 'right' },
            //{ title: '可用数量', field: 'Available', width: 80, align: 'right' },
            { title: '国际条码', field: 'BarCode', width: 180 },
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 80 },   //商品ID
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 80 },    //商品最小单位价格
            { title: 'Unit', field: 'Unit', hidden: true, width: 80 },
            { title: 'MaxOrderQty', field: 'MaxOrderQty', hidden: true, width: 100 },
            { title: 'IsAppend', field: 'IsAppend', hidden: true, width: 100 },
            //{ title: 'BigShopPoint', field: 'BigShopPoint', hidden: true, width: 100 },
            //{ title: 'ShopAddPerc', field: 'ShopAddPerc', hidden: true, width: 100 },
            //{ title: 'ShopPoint', field: 'ShopPoint', hidden: true, width: 100 },
            { title: 'BasePoint', field: 'BasePoint', hidden: true, width: 100 },
            { title: 'VendorPerc1', field: 'VendorPerc1', hidden: true, width: 100 },
            { title: 'VendorPerc2', field: 'VendorPerc2', hidden: true, width: 100 }
        ]]
        //hideColumn: 'STAFF_ID',
        //toolbar: [{
        //    text: '添加',
        //    id: 'addDetailsBtn',
        //    iconCls: 'icon-add',
        //    handler: addGridRow
        //}, '-', {
        //    text: '删除',
        //    id: 'delDetailsBtn',
        //    iconCls: 'icon-remove',
        //    handler: del
        //}]
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
            row: { SKU: "", ProductName: "", SaleUnit: "", SaleQty: "0.00", SalePackingQty: "0", UnitQty: "0.00", SalePrice: "0.0000", Stock: "0.00", OnTheWay: "0.00", Available: "0.00", ShopAddPerc: "0.00", SubAmt: "0.0000", Remark: "", BarCode: "" }
        });
        return true;
    }
}

//删除
function del() {

    var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));
    $('#grid').datagrid('endEdit', index);

    var is = "";
    $(".datagrid-body:first table tr").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            is = i + "," + is;
        }
    });

    var rows = $('#grid').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < is.split(',').length; j++) {
        var ci = is.split(',')[j];
        if (ci) {
            if (rows[ci].IsAppend == 0) {
                $.messager.alert("提示", " 该商品不能删除！！", "info");       //非强配商品不能删除
                return false;
            }
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#grid').datagrid('getRowIndex', copyRows[i]);
        $('#grid').datagrid('deleteRow', ind);
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
            jsonStr += "{";
            jsonStr += "\"BarCode\":\"" + rows[i].BarCode + "\",";
            jsonStr += "\"SalePackingQty\":\"" + rows[i].SalePackingQty + "\",";
            jsonStr += "\"SalePrice\":\"" + rows[i].SalePrice + "\",";
            jsonStr += "\"SaleQty\":\"" + rows[i].SaleQty + "\",";
            jsonStr += "\"SaleUnit\":\"" + rows[i].SaleUnit + "\",";
            jsonStr += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonStr += "\"Remark\":\"" + rows[i].Remark + "\",";
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
        $.messager.alert("提示", "订单详情数据不能为空！", "info");
        return false;
    }

    var loading = frxs.loading("正在加载中，请稍后...");
    var jsonBuyOrder = "{";
    jsonBuyOrder += "\"OrderID\":'" + $("#OrderId").val() + "',";
    jsonBuyOrder += "\"OrderDate\":'" + $("#OrderDate").val() + "',";
    jsonBuyOrder += "\"SubWID\":'" + $("#SubWID").combobox('getValue') + "',";
    jsonBuyOrder += "\"ShopID\":'" + $("#ShopID").val() + "',";
    jsonBuyOrder += "\"ShopCode\":'" + $("#ShopCode").val() + "',";
    jsonBuyOrder += "\"ShopName\":'" + $("#ShopName").val() + "',";
    jsonBuyOrder += "\"Status\":'" + $("#Status").combobox('getValue') + "',";
    jsonBuyOrder += "\"Remark\":'" + $("#Remark").val() + "'";
    jsonBuyOrder += "}";

    $.ajax({
        url: "../WarehouseOrder/WarehouseOrderAddOrEditeHandle",
        type: "post",
        data: { jsonData: jsonBuyOrder, jsonDetails: jsonStr },
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
                frxs.updateTabTitle("查看采购收货单" + result.Data.OrderID, "icon-search");

                SetPermission("save");
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
    var h = ($(window).height() - $("fieldset").height() - 185);
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
            //data.unshift({ "WID": "", "WName": "-请选择-" });
            //创建控件
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#SubWID").combobox('select', data[0].WID);
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
                    var url = '../Common/GetSaleProductUnit?ProductID=' + productId;
                    ed.target.combobox('reload', url); //联动下拉列表重载
                }
            }
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
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
                $(this).blur();
                if ($(this).val() != valueinput) {

                    showDialog($(this).val());
                    //$.messager.alert("提示", "弹窗,带入参数" + $(this).val());
                } else {
                    nextControl();
                }
            } else {

                if (field == "SaleQty") {
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
        if ((parseFloat(SaleQty) * parseFloat(SalePackingQty)) > parseFloat(row.MaxOrderQty)) {
            SaleQty = parseFloat(row.MaxOrderQty) / parseFloat(SalePackingQty);
        }
    }
    //var SalePackingQty = row.SalePackingQty;
    var SalePrice = row.SalePrice;
    var ShopAddPerc = row.ShopAddPerc;

    row.UnitQty = parseFloat(parseFloat(SaleQty) * parseFloat(SalePackingQty)).toFixed(2);
    row.SaleQty = parseFloat(SaleQty).toFixed(2);
    row.SubAmt = parseFloat(parseFloat(SaleQty) * parseFloat(SalePrice) * parseFloat(1 + ShopAddPerc)).toFixed(4);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.SaleQty)) {
        row.SaleQty = "0.00";
    }
    if (isNaN(row.SubAmt)) {
        row.SubAmt = "0.0000";
    }

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
        var unitQty = parseFloat(rows[i].UnitQty);
        var subAmt = parseFloat(rows[i].SubAmt);
        totalUnitQty += unitQty;
        totalSubAmt += subAmt;
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
    var fields = "SKU,SaleQty,Remark".split(',');
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
    //if ($("#addDetailsBtn").hasClass("l-btn-disabled") == false) {
    if (false) {
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
            shopName: $("#ShopName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#ShopID").val(obj.rows[0].ShopID);
                $("#ShopCode").val(obj.rows[0].ShopCode);
                $("#ShopName").val(obj.rows[0].ShopName);
            } else {
                selShop();
            }
        }
    });
}

//选择门店
function selShop() {
    var shopCode = $("#ShopCode").val();
    var shopName = $("#ShopName").val();
    frxs.dialog({
        title: "选择门店",
        url: "../SaleBackPre/SelectShop?shopCode=" + encodeURIComponent(shopCode) + "&shopName=" + encodeURIComponent(shopName),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填门店
function backFillShop(shopID, shopCode, shopName) {
    $("#ShopID").val(shopID);
    $("#ShopCode").val(shopCode);
    $("#ShopName").val(shopName);
}


//选择商品资料
function showDialog(parm) {
    var subWID = $("#SubWID").combobox('getValue');
    var shopID = $("#ShopID").val();
    if (shopID) {
        
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
    if ($('#grid').datagrid('getSelected').SKU == product.SKU) {
        return;
    }

    for (var i = 0; i < rows.length; i++) {

        if (rows[i].SKU == product.SKU && index != i) {
            rep = true;
            break;
        }
    }
    if (rep) {

        var msgdlg = frxs.dialog({
            title: "提示",
            content: "存在重复数据。是否放弃操作？",
            width: 300,
            height: 130,
            modal: true,
            buttons: [{
                text: '放弃',
                iconCls: 'icon-no',
                handler: function () {
                    msgdlg.dialog("close");
                }
            }, {
                text: '添加',
                iconCls: 'icon-add',
                handler: function () {
                    backFillProduct2(product);
                    msgdlg.dialog("close");
                }
            }]
        });

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
    row.SalePackingQty = product.BigPackingQty;
    row.SalePrice = product.BigSalePrice;
    row.BarCode = product.BarCode;
    row.ProductId = product.ProductId;
    row.UnitPrice = product.SalePrice;
    row.Unit = product.Unit;

    row.ShopAddPerc = product.ShopAddPerc;
    row.Stock = product.Stock;
    row.OnTheWay = product.OnTheWay;
    row.Available = product.Available;

    row.IsAppend = 1;  //默认为强配商品
    row.MaxOrderQty = product.MaxOrderQty;
    //row.BigShopPoint = product.BigShopPoint;
    row.BasePoint = product.BasePoint;
    row.VendorPerc1 = product.VendorPerc1;
    row.VendorPerc2 = product.VendorPerc2;

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
                        $("#ConfUserName").val(result.Data.UserName);
                        $("#ConfTime").val(result.Data.Time);
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




//订单 导出
function exportData() {
    var orderid = $("#OrderId").val();
    if (orderid != "") {
        location.href = "../WarehouseOrder/DataExport?OrderID=" + orderid;
    } else {
        $.messager.alert("提示", "订单号为空！", "info");
    }
}

//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#addBtn").linkbutton('disable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('enable');
            $("#addDetailsBtn").linkbutton('enable');
            $("#delDetailsBtn").linkbutton('enable');
            break;
        case "edit":
            $("#addBtn").linkbutton('disable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('enable');
            $("#addDetailsBtn").linkbutton('enable');
            $("#delDetailsBtn").linkbutton('enable');
            break;
        case "query":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('enable');
            $("#sureBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
        case "sure":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('enable');
            $("#sureBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
        case "other":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
        case "save":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('enable');
            $("#sureBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
    }

}

