//全局变量
var buyID;
$(function () {
    //下拉绑定
    //initDDL();

    //供应商-采购员事件绑定
    //eventBind();

    //加载详情数据
    //loadgrid();

    //加载主表数据
    init();

    //高度自适应
    //gridresize();

});

//加载数据
function init() {
    buyID = frxs.getUrlParam("BuyID");
    if (buyID) {
        //编辑
        $.ajax({
            url: "../BuyOrderPre/GetBuyOrderInfo",
            type: "post",
            data: { buyid: buyID },
            dataType: 'json',
            success: function (obj) {
                //格式化日期
                obj.OrderDate = frxs.dateFormat(obj.OrderDate);
                obj.CreateTime = frxs.dateFormat(obj.CreateTime);
                obj.ConfTime = frxs.dateFormat(obj.ConfTime);
                obj.PostingTime = frxs.dateFormat(obj.PostingTime);
                $('#OrderForm').form('load', obj);

                loadgrid(obj.Orderdetails);

                totalCalculate();

                //高度自适应
                gridresize();

                if (obj.Status == 0) {
                    SetPermission('query');
                }
                else if (obj.Status == 1) {
                    SetPermission('sure');
                }
                else if (obj.Status == 2) {
                    SetPermission('posting');
                }
            }
        });
    } else {
        //添加
        //$("#Status").combobox('setValue', '0');

        loadgrid();

        //高度自适应
        gridresize();

        //初始化添加一行
        addGridRow();

        //SetPermission('add');

        
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
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            {
                title: 'ERP编码', field: 'SKU', width: 100
            },
            { title: '商品名称', field: 'ProductName', width: 180, formatter: frxs.replaceCode },

            {
                title: '库存单位', field: 'BuyQty', width: 100
            },
            { title: '包装数', field: 'BuyPackingQty', width: 160 },
            { title: '原门店积分', field: 'UnitQty', width: 160 },

            { title: '小单位积分', field: 'SubAmt', width: 160, editor: 'text' },
            {
                title: '配送单位', field: 'Remark', width: 160, formatter: frxs.replaceCode
            },
            { title: '积分', field: 'BarCode', width: 160 },
            { title: '限购小单位数量', field: 'BarCode', width: 160, editor: 'text' },
            { title: '国际条码', field: 'BarCode', width: 160 }
        ]],
        toolbar: [{
            text: '添加',
            id: 'addDetailsBtn',
            iconCls: 'icon-add',
            handler: addGridRow
        }, '-', {
            text: '删除',
            id: 'delDetailsBtn',
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
            row: { SKU: "", ProductName: "", BuyUnit: "", BuyQty: "0.00", BuyPackingQty: "0", UnitQty: "0.00", BuyPrice: "0.0000", SubAmt: "0.0000", Remark: "", BarCode: "" }
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

    $('#grid').datagrid('endEdit', editIndex);
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            jsonStr += "{";
            jsonStr += "\"BarCode\":\"" + rows[i].BarCode + "\",";
            jsonStr += "\"BuyPackingQty\":\"" + rows[i].BuyPackingQty + "\",";
            jsonStr += "\"BuyPrice\":\"" + rows[i].BuyPrice + "\",";
            jsonStr += "\"BuyQty\":\"" + rows[i].BuyQty + "\",";
            jsonStr += "\"BuyUnit\":\"" + rows[i].BuyUnit + "\",";
            jsonStr += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonStr += "\"Remark\":\"" + rows[i].Remark + "\",";
            jsonStr += "\"SKU\":\"" + rows[i].SKU + "\",";
            jsonStr += "\"SubAmt\":\"" + rows[i].SubAmt + "\",";
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
    jsonBuyOrder += "\"BuyID\":'" + $("#BuyID").val() + "',";
    jsonBuyOrder += "\"OrderDate\":'" + $("#OrderDate").val() + "',";
    jsonBuyOrder += "\"SubWID\":'" + $("#SubWID").combobox('getValue') + "',";
    jsonBuyOrder += "\"VendorID\":'" + $("#VendorID").val() + "',";
    jsonBuyOrder += "\"VendorCode\":'" + $("#VendorCode").val() + "',";
    jsonBuyOrder += "\"VendorName\":'" + $("#VendorName").val() + "',";
    jsonBuyOrder += "\"BuyEmpID\":'" + $("#BuyEmpID").val() + "',";
    jsonBuyOrder += "\"BuyEmpName\":'" + $("#BuyEmpName").val() + "',";
    jsonBuyOrder += "\"TotalOrderAmt\":'" + $("#TotalOrderAmt").val() + "',";
    jsonBuyOrder += "\"Remark\":'" + $("#Remark").val() + "'";
    jsonBuyOrder += "}";

    $.ajax({
        url: "../BuyOrderPre/BuyOrderPreAddOrEditeNewHandle",
        type: "post",
        data: { jsonData: jsonBuyOrder, jsonDetails: jsonStr },
        dataType: "json",
        success: function (result) {
            loading.close();
            //result = jQuery.parseJSON(result);
            if (result.Flag == "SUCCESS") {
                if ($("#BuyID").val() != "") {
                    $.messager.alert("提示", "编辑成功", "info");
                } else {
                    var data = jQuery.parseJSON(result.Data);
                    $("#BuyID").val(data.BuyID);
                    $.messager.alert("提示", "添加成功", "info");
                }
                SetPermission("save");
            } else {
                $.messager.alert("提示", "保存失败", "info");
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

            var ed = $(this).datagrid('getEditor', param);
            if (ed && ed.type && ed.type == "combobox") {

                //ed.target.combobox('getValue')
                //绑定单位字段
                var productId = $("#grid").datagrid("getSelected").ProductId;
                if (productId > 0) {
                    var url = '../Common/GetProductUnit?ProductID=' + productId;
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

    if (field == "BuyQty") {
        obj.bind("change", function () {
            changeBuyQtyValue($(this).val());
        });
    }

    if (field == "BuyPrice") {
        obj.bind("change", function () {
            changeBuyPriceValue($(this).val());
        });
    }


    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "SKU") {
                if ($(this).val() != valueinput) {

                    showDialog($(this).val());
                    //$.messager.alert("提示", "弹窗,带入参数" + $(this).val());
                } else {
                    nextControl();
                }
            } else {

                if (field == "BuyQty") {
                    obj.change();
                }
                if (field == "BuyPrice") {
                    obj.change();
                }
                nextControl();
            }
        }

        //向下
        if (e.keyCode == 40) {
            if (field == "BuyQty") {
                obj.change();
            }
            if (field == "BuyPrice") {
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
            if (field == "BuyQty") {
                obj.change();
            }
            if (field == "BuyPrice") {
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
function changeBuyQtyValue(buyQty) {
    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    var buyPackingQty = row.BuyPackingQty;

    var buyPrice = row.BuyPrice;

    row.UnitQty = parseFloat(parseFloat(buyQty) * parseFloat(buyPackingQty)).toFixed(2);
    row.BuyQty = parseFloat(buyQty).toFixed(2);
    row.SubAmt = parseFloat(parseFloat(buyQty) * parseFloat(buyPrice)).toFixed(4);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.BuyQty)) {
        row.BuyQty = "0.00";
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

    $("#TotalOrderAmt").val(parseFloat(totalSubAmt).toFixed(4));
    $('#grid').datagrid('reloadFooter', [
       { "UnitQty": "数量总计：" + parseFloat(totalUnitQty).toFixed(2), "SubAmt": "金额总计：" + parseFloat(totalSubAmt).toFixed(4) }
    ]);

}


//金额计算 并且更新grid的行数据
function changeBuyPriceValue(buyPrice) {

    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    //判断采购价不能大于OldPrice价格
    if (row.OldBuyPrice && buyPrice) {
        if (parseFloat(buyPrice) > parseFloat(row.OldBuyPrice)) {
            buyPrice = parseFloat(row.OldBuyPrice);
        }
    }


    var buyPackingQty = row.BuyPackingQty;
    var buyQty = row.BuyQty;

    row.BuyPrice = parseFloat(buyPrice).toFixed(4);
    row.UnitQty = parseFloat(parseFloat(buyQty) * parseFloat(buyPackingQty)).toFixed(2);
    row.BuyQty = parseFloat(buyQty).toFixed(2);
    row.SubAmt = parseFloat(parseFloat(buyQty) * parseFloat(buyPrice)).toFixed(2);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.BuyQty)) {
        row.BuyQty = "0.00";
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
    var fields = "SKU,BuyQty,BuyPrice,Remark".split(',');
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
    if ($("#addDetailsBtn").hasClass("l-btn-disabled") == false) {

        editfield = field;

        if (endEditing()) {
            $('#grid').datagrid('selectRow', index)
                .datagrid('editCell', { index: index, field: field });
            editIndex = index;
        }
    }
}




//供应商-采购员事件绑定
function eventBind() {
    $("#VendorCode").keydown(function (e) {
        if (e.keyCode == 13) {
            eventVendorCodeName();
        }
    });

    $("#VendorName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventVendorCodeName();
        }
    });

    $("#BuyEmpName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventBuyEmp();
        }
    });
}

//供应商名称Code
function eventVendorCodeName() {
    $.ajax({
        url: "../Common/GetVendorInfo",
        type: "post",
        data: {
            VendorCode: $("#VendorCode").val(),
            VendorName: $("#VendorName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#VendorID").val(obj.rows[0].VendorID);
                $("#VendorCode").val(obj.rows[0].VendorCode);
                $("#VendorName").val(obj.rows[0].VendorName);
            } else {
                selVendor();
            }
        }
    });
}

//采购员
function eventBuyEmp() {
    $.ajax({
        url: "../Common/GetBuyEmpInfo",
        type: "post",
        data: {
            EmpName: $("#BuyEmpName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#BuyEmpID").val(obj.rows[0].EmpID);
                $("#BuyEmpName").val(obj.rows[0].EmpName);
            } else {
                selBuyEmp();
            }
        }
    });
}

//选择供应商
function selVendor() {
    var vendorCode = $("#VendorCode").val();
    var vendorName = $("#VendorName").val();
    frxs.dialog({
        title: "选择供应商",
        url: "../BuyOrderPre/SelectVendor?vendorCode=" + encodeURIComponent(vendorCode) + "&vendorName=" + encodeURIComponent(vendorName),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填供应商
function backFillVendor(vendorId, vendorCode, vendorName) {
    $("#VendorID").val(vendorId);
    $("#VendorCode").val(vendorCode);
    $("#VendorName").val(vendorName);
}

//选择采购员
function selBuyEmp() {
    frxs.dialog({
        title: "选择采购员",
        url: "../BuyOrderPre/SelectBuyEmp?buyEmpName=" + encodeURIComponent($("#BuyEmpName").val()),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填采购员
function backFillBuyEmp(empID, empName) {
    $("#BuyEmpID").val(empID);
    $("#BuyEmpName").val(empName);
}

//选择商品资料
function showDialog(parm) {
    var VendorID = $("#VendorID").val();
    //vendorId = 61;
    if (VendorID) {
        var type = "SKU";
        //判断需要查询的类型
        var onebyte = parm ? parm.substring(0, 1) : "";
        if (isNaN(onebyte) && onebyte != "赠") {
            type = "ProductName";
        }
        $.ajax({
            url: "../Common/GetProductInfo",
            type: "get",
            dataType: "json",
            data: {
                vendorid: VendorID,
                Value: parm,
                Type: type,
                SKULikeSearch: false
            },
            success: function (obj) {
                if (obj && obj.total > 0) {
                    backFillProduct(obj.rows[0]);
                } else {
                    frxs.dialog({
                        title: "选择商品资料",
                        url: "../BuyOrderPre/SearchProduct?productCode=" + parm + "&vendorId=" + VendorID,
                        owdoc: window.top,
                        width: 850,
                        height: 500
                    });
                }
            }
        });

    } else {
        $.messager.alert("提示", "请先选择供应商", "info");
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
    row.BuyUnit = product.BuyUnit;
    row.BuyPackingQty = product.PackingQty;
    row.BuyPrice = product.BuyPrice;
    row.BarCode = product.BarCode;
    row.ProductId = product.ProductId;

    row.UnitPrice = product.UnitPrice;
    row.OldBuyPrice = product.OldBuyPrice;

    row.BuyQty = '0.00';
    row.UnitQty = '0.00';
    row.SubAmt = '0.0000';


    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });


    onClickCell(index, "BuyQty");
}



//订单 添加
function add() {
    location.href = '../BuyOrderPre/BuyOrderPreAddOrEditNew';
}

//订单 编辑
function edit() {
    SetPermission("edit");

    //标记是否编辑状态
    //isEdit = true;
}

//订单状态 确认
function sure() {
    var status = $("#Status").combobox('getValue');   // $("#Status").val();
    if (status != "0") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return false;
    }
    var buyid = $("#BuyID").val();
    if (buyid != "") {
        $.ajax({
            url: "../BuyOrderPre/BuyOrderPreChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                buyids: buyid,
                status: 1
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "确认成功！", "info");
                        //$("#Status").val(1);
                        $("#Status").combobox('setValue', '1');
                        SetPermission("sure");
                    } else {
                        $.messager.alert("提示", "确认失败！", "info");
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

//订单状态 过账
function posting() {
    var status = $("#Status").combobox('getValue');   // $("#Status").val();
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return false;
    }
    var buyid = $("#BuyID").val();
    if (buyid != "") {
        $.ajax({
            url: "../BuyOrderPre/BuyOrderPreChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                buyids: buyid,
                status: 2
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "过账成功！", "info");
                        //$("#Status").val(2);
                        $("#Status").combobox('setValue', '2');
                        SetPermission("posting");
                    } else {
                        $.messager.alert("提示", "过账失败！", "info");
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
    var buyid = $("#BuyID").val();
    if (buyid != "") {
        //site_common.ShowProgressBar();
        location.href = "../BuyOrderPre/DataExport?Buyid=" + buyid;
        //site_common.HideProgressBar();
    } else {
        $.messager.alert("提示", "收货单号为空！", "info");
    }
}

//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#addBtn").linkbutton('disable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#postBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('enable');
            $("#addDetailsBtn").linkbutton('enable');
            $("#delDetailsBtn").linkbutton('enable');
            break;
        case "edit":
            $("#addBtn").linkbutton('disable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#postBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('enable');
            $("#addDetailsBtn").linkbutton('enable');
            $("#delDetailsBtn").linkbutton('enable');
            break;
        case "query":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('enable');
            $("#sureBtn").linkbutton('enable');
            $("#postBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
        case "sure":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#postBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
        case "posting":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('disable');
            $("#sureBtn").linkbutton('disable');
            $("#postBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
        case "save":
            $("#addBtn").linkbutton('enable');
            $("#editBtn").linkbutton('enable');
            $("#sureBtn").linkbutton('enable');
            $("#postBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#addDetailsBtn").linkbutton('disable');
            $("#delDetailsBtn").linkbutton('disable');
            break;
    }

}

