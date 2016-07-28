//全局变量
var buyID;
//切换提示
var comboVal;


var newvar;
$(function () {
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
    var temp = getCookie("BuyOrder");
    var obj = JSON.parse(temp);
    var htmltr = "<table><thead><th>单号</th><th style=\"text-align:left\">供应商编号-名称</th><th>操作</th></thead>";
    $.each(obj, function () {
        htmltr = htmltr + "<tr><td width=\"100px\" height=\"24\" style=\"cursor:pointer\" onclick=\"loadData(this)\">" + this.ID + "</td><td width=\"300px\" style=\"text-align:left;\">" + this.VendorCode + "-" + this.VendorName + "</td><td><a href=\"#\" onclick=\"removeHistory(this)\">移除</a></td></tr>";
    });

    htmltr = htmltr + "</table>";
    $("#history").empty().append(htmltr);
    $(".history").toggle(200);
}

// 查询历史 记录单击
function loadData(obj) {
    var buyid = $(obj).text();
    window.location.href = "../BuyOrderPre/BuyOrderPreAddOrEditNew?BuyID=" + buyid;

    frxs.updateTabTitle("查看采购收货单" + buyid, "icon-search");
}

//查询历史 删除
function removeHistory(obj) {
    var buyid = $(obj).parents("tr").find("td:eq(0)").text();
    var temp = getCookie("BuyOrder");
    var objs = JSON.parse(temp);

    var result = [];
    for (var i = 0; i < objs.length; i++) {
        if (objs[i].ID != buyid) {
            result.push(objs[i]);
        }
    }
    delCookie("BuyOrder");
    if (result.length != 0) {
        addCookie("BuyOrder", JSON.stringify(result), 0);
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
                $("#hidVendorCode").val(obj.VendorCode);
                $("#hidBuyEmpName").val(obj.BuyEmpName);

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

                var rows = $("#grid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    var index = $('#grid').datagrid('getRowIndex', rows[i]);

                    rows[i].BuyQty = parseFloat(rows[i].BuyQty).toFixed(2);
                    rows[i].UnitQty = parseFloat(rows[i].UnitQty).toFixed(2);
                    rows[i].BuyPackingQty = parseFloat(rows[i].BuyPackingQty).toFixed(2);
                    rows[i].WStock = parseFloat(rows[i].WStock).toFixed(2);
                    rows[i].BuyPrice = parseFloat(rows[i].BuyPrice).toFixed(4);
                    rows[i].SubAmt = parseFloat(rows[i].SubAmt).toFixed(4);
                   
                    //更新行
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: rows[i]
                    });
                }


                //供应商不可编辑
                $('#VendorCode').attr("readonly", "readonly");
                $("#bntSelVendor").attr("disabled", true);

                $('#VendorCode').addClass("readonly");
                $('#VendorName').addClass("readonly");

                //仓库不可编辑
                $("#SubWID").combobox("disable");
            }
        });
    } else {
        //添加
        $("#Status").combobox('setValue', '0');

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
            { title: '商品名称', field: 'ProductName', width: 180, formatter: frxs.replaceCode }
        ]],
        columns: [[
            {
                title: '单位', field: 'BuyUnit', align: 'center', width: 80, editor: {
                    type: 'combobox',
                    options: {
                        //url: '../Common/GetBuyProductUnit?ProductID=2927',
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
                                row.BuyPackingQty = value;

                                //下拉框设置
                                var text = $(this).combobox("getText");
                                row.BuyUnit = text;

                                if (text == row.Unit) {
                                    row.BuyPrice = row.MinBuyPrice;
                                    row.SalePrice = row.MinSalePrice;
                                } else {
                                    row.BuyPrice = row.MaxBuyPrice;
                                    row.SalePrice = row.MaxSalePrice;
                                }

                                //单价*数量
                                row.SubAmt = parseFloat(row.BuyPrice * row.BuyQty).toFixed(4);
                                row.UnitQty = parseFloat(row.BuyQty * row.BuyPackingQty).toFixed(2);
                                row.BuyPackingQty = parseFloat(row.BuyPackingQty).toFixed(2);
                                row.BuyPrice = parseFloat(row.BuyPrice).toFixed(4);
                                row.SalePrice = parseFloat(row.SalePrice).toFixed(4);

                                $(".datagrid-row .combo").prev().combobox("hidePanel");
                                //更新行
                                $('#grid').datagrid('updateRow', {
                                    index: index,
                                    row: row
                                });

                                totalCalculate();
                            }

                        }
                    }
                }
            },
            {
                title: frxs.setTitleColor('采购数量'), field: 'BuyQty', width: 130, align: 'right', editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2,
                        min: 0
                    }
                }
            },
            {
                title: '采购价格', field: 'BuyPrice', align: 'right', width: 130
            },
            { title: '采购金额', field: 'SubAmt', align: 'right', width: 130 },
            { title: '包装数', field: 'BuyPackingQty', align: 'right', width: 100 },
            { title: '配送价', field: 'SalePrice', align: 'right', width: 100 },   //配送大单位价格
            { title: '总数量', field: 'UnitQty', align: 'right', width: 180 },
            { title: '库存数量', field: 'WStock', align: 'right', width: 100, },
            { title: '国际条码', field: 'BarCode', width: 180, align: 'center' },
            { title: frxs.setTitleColor('备注'), field: 'Remark', width: 200, editor: 'text', formatter: frxs.formatText },
            
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 100 },   //商品ID
            { title: 'OldBuyPrice', field: 'OldBuyPrice', hidden: true, width: 80 },
            { title: 'Unit', field: 'Unit', hidden: true, width: 80 },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 80 },
            { title: 'MinBuyPrice', field: 'MinBuyPrice', hidden: true },
            { title: 'MinSalePrice', field: 'MinSalePrice', hidden: true },
            { title: 'MaxBuyPrice', field: 'MaxBuyPrice', hidden: true, width: 80 },
            { title: 'MaxSalePrice', field: 'MaxSalePrice', hidden: true }
            
        ]],
        //hideColumn: 'STAFF_ID',
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
            row: { SKU: "", ProductName: "", BuyUnit: "", BuyQty: "", BuyPackingQty: "", UnitQty: "", BuyPrice: "", SubAmt: "", WStock: "", Remark: "", BarCode: "" }
            //row: { SKU: "", ProductName: "", BuyUnit: "", BuyQty: "0.00", BuyPackingQty: "0", UnitQty: "0.00", BuyPrice: "0.0000", SubAmt: "0.0000", WStock: "0", Remark: "", BarCode: "" }
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
    if (copyRows.length == 0) {
        $.messager.alert("提示", "请选择一条记录！", "info");
        return false;
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#grid').datagrid('getRowIndex', copyRows[i]);
        $('#grid').datagrid('deleteRow', ind);
    }
    totalCalculate();
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
    
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }

    if ($("#hidVendorCode").val() != $("#VendorCode").val()) {
        $.messager.alert("提示", "供应商编码有误，请重新选择！", "info");
        return false;
    }
    if ($("#BuyEmpName").val() != "") {
        if ($("#hidBuyEmpName").val() != $("#BuyEmpName").val()) {
            $.messager.alert("提示", "采购员名称有误，请重新选择！", "info");
            return false;
        }
    }
    $('#grid').datagrid('endEdit', editIndex);
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            if (rows[i].BuyQty <= 0) {
                $.messager.alert("提示", "商品【" + rows[i].ProductName + "】采购数量不能为零！", "info");
                return false;
            }
            //if (rows[i].BuyPrice <= 0) {
            //    $.messager.alert("提示", "商品【" + rows[i].ProductName + "】采购价格不能为零！", "info");
            //    return false;
            //}
            jsonStr += "{";
            jsonStr += "\"BarCode\":\"" + (rows[i].BarCode == null ? "" : rows[i].BarCode) + "\",";
            jsonStr += "\"BuyPackingQty\":\"" + rows[i].BuyPackingQty + "\",";
            jsonStr += "\"BuyPrice\":\"" + rows[i].BuyPrice + "\",";
            jsonStr += "\"BuyQty\":\"" + rows[i].BuyQty + "\",";
            jsonStr += "\"BuyUnit\":\"" + rows[i].BuyUnit + "\",";
            jsonStr += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonStr += "\"Remark\":\"" + frxs.filterText(rows[i].Remark ? rows[i].Remark : "") + "\",";
            jsonStr += "\"SKU\":\"" + rows[i].SKU + "\",";
            jsonStr += "\"SubAmt\":\"" + rows[i].SubAmt + "\",";
            jsonStr += "\"UnitPrice\":\"" + rows[i].UnitPrice + "\",";
            jsonStr += "\"SaleUnitPrice\":\"" + rows[i].SalePrice + "\",";
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
    var loading = window.top.frxs.loading("正在加载中，请稍后...");
    var jsonBuyOrder = "{";
    jsonBuyOrder += "\"BuyID\":\"" + $("#BuyID").val() + "\",";
    jsonBuyOrder += "\"OrderDate\":\"" + $("#OrderDate").val() + "\",";
    jsonBuyOrder += "\"SubWID\":\"" + $("#SubWID").combobox('getValue') + "\",";
    jsonBuyOrder += "\"VendorID\":\"" + $("#VendorID").val() + "\",";
    jsonBuyOrder += "\"VendorCode\":\"" + $("#VendorCode").val() + "\",";
    jsonBuyOrder += "\"VendorName\":\"" + $("#VendorName").val() + "\",";
    jsonBuyOrder += "\"BuyEmpID\":\"" + $("#BuyEmpID").val() + "\",";
    jsonBuyOrder += "\"BuyEmpName\":\"" + $("#BuyEmpName").val() + "\",";
    jsonBuyOrder += "\"TotalOrderAmt\":\"" + $("#TotalOrderAmt").val() + "\",";
    jsonBuyOrder += "\"Remark\":\"" + frxs.filterText($("#Remark").val()) + "\"";
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
                    //var data = jQuery.parseJSON(result.Data);
                    $("#BuyID").val(result.Data.BuyID);
                    $("#CreateUserName").val(result.Data.UserName);
                    $("#CreateTime").val(result.Data.Time);
                    $.messager.alert("提示", "添加成功", "info");
                }
                frxs.updateTabTitle("查看采购收货单" + result.Data.BuyID, "icon-search");

                SetPermission("save");

                //供应商不可编辑
                $('#VendorCode').attr("readonly", "readonly");
                $("#bntSelVendor").attr("disabled", true);

                $('#VendorCode').addClass("readonly");
                $('#VendorName').addClass("readonly");

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
            if (data.length > 1) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName",    //value列
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
                    var url = '../Common/GetBuyProductUnit?ProductID=' + productId;
                    ed.target.combobox('reload', url); //联动下拉列表重载
                }
            }
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
                    if (ed.field == "BuyQty" || ed.field == "BuyPrice") {
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

    //if (field == "SKU") {
    //    obj.click(function () {
    //        alert("aaaa");
    //    });

    //    obj.dblclick(function () {
    //        alert("bbbb");
    //    });
    //}

    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "SKU") {
                if ($(this).val() == "") {
                    showDialog("");
                    $(this).blur();
                }
                else if ($(this).val() != valueinput) {
                    showDialog($(this).val());
                    $(this).blur();
                } else {
                    nextControl();
                }
            } else {
                if (field == "BuyQty") {
                    var row = $('#grid').datagrid('getSelected');
                    var index = $("#grid").datagrid("getRowIndex", row);
                    var temp = parseFloat($(this).val()) * parseFloat(row.BuyPackingQty);
                    if (temp > 999999) {
                        row.BuyQty = "0.00";
                        $('#grid').datagrid('updateRow', {
                            index: index,
                            row: row
                        });
                        $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                        return false;
                    }
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
                var row = $('#grid').datagrid('getSelected');
                var index = $("#grid").datagrid("getRowIndex", row);
                var temp = parseFloat($(this).val()) * parseFloat(row.BuyPackingQty);
                if (temp > 999999) {
                    row.BuyQty = "0.00";
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                    return false;
                }
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
                var row = $('#grid').datagrid('getSelected');
                var index = $("#grid").datagrid("getRowIndex", row);
                var temp = parseFloat($(this).val()) * parseFloat(row.BuyPackingQty);
                if (temp > 999999) {
                    row.BuyQty = "0.00";
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                    return false;
                }
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
    if (row.UnitQty > 999999) {
        row.BuyQty = "0.00";
        row.UnitQty = "0.00";
        $('#grid').datagrid('updateRow', {
            index: index,
            row: row
        });
        $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, 'BuyQty'); });
        return false;
    }

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
        if (rows[i].SKU) {
            var unitQty = parseFloat(rows[i].UnitQty);
            var subAmt = parseFloat(rows[i].SubAmt);
            totalUnitQty += unitQty;
            totalSubAmt += subAmt;
        }
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
    var fields = "SKU,BuyQty".split(',');  //SKU,BuyQty,BuyPrice,Remark
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
    if (editfield == "BuyUnit") {
        $('#grid').datagrid('updateRow', {
            index: index,
            row: selected
        });
        currentField = "BuyQty";
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
            //VendorName: $("#VendorName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                var rows = $('#grid').datagrid("getRows");
                if ($("#VendorID").val() != "" && $("#VendorID").val() != obj.rows[0].VendorID) {
                    if (rows.length > 0 && rows[0].SKU) {
                        $.messager.confirm("提示", "确认更换供应商吗?（将清除商品信息）", function (r) {
                            if (r) {
                                if (rows) {
                                    //for (var i = rows.length - 1; i >= 0; i--) {
                                    //    var index = $('#grid').datagrid('getRowIndex', rows[i]);
                                    //    $('#grid').datagrid('deleteRow', index);
                                    //}
                                    $('#grid').datagrid('loadData', { total: 0, rows: [] });
                                    addGridRow();
                                    $("#VendorID").val(obj.rows[0].VendorID);
                                    $("#VendorCode").val(obj.rows[0].VendorCode);
                                    $("#hidVendorCode").val(obj.rows[0].VendorCode);
                                    $("#VendorName").val(obj.rows[0].VendorName);
                                }
                            } else {
                                return false;
                            }
                        });
                    } else {
                        $("#VendorID").val(obj.rows[0].VendorID);
                        $("#VendorCode").val(obj.rows[0].VendorCode);
                        $("#hidVendorCode").val(obj.rows[0].VendorCode);
                        $("#VendorName").val(obj.rows[0].VendorName);
                    }
                } else {
                    $("#VendorID").val(obj.rows[0].VendorID);
                    $("#VendorCode").val(obj.rows[0].VendorCode);
                    $("#hidVendorCode").val(obj.rows[0].VendorCode);
                    $("#VendorName").val(obj.rows[0].VendorName);
                }
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
                $("#hidBuyEmpName").val(obj.rows[0].EmpName);
            } else {
                selBuyEmp();
            }
        }
    });
}

//选择供应商
function selVendor() {
    var vendorCode = $("#VendorCode").val();
    //var vendorName = $("#VendorName").val();
    frxs.dialog({
        title: "选择供应商",
        url: "../BuyOrderPre/SelectVendor?vendorCode=" + encodeURIComponent(vendorCode),  // + "&vendorName=" + encodeURIComponent(vendorName)
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填供应商
function backFillVendor(vendorId, vendorCode, vendorName) {
    var rows = $('#grid').datagrid("getRows");
    if ($("#VendorID").val() != "" && $("#VendorID").val() != vendorId) {
        if (rows.length > 0 && rows[0].SKU) {
            $.messager.confirm("提示", "确认更换供应商吗?（将清除商品信息）", function (r) {
                if (r) {
                    if (rows) {
                        //for (var i = rows.length - 1; i >= 0; i--) {
                        //    var index = $('#grid').datagrid('getRowIndex', rows[i]);
                        //    $('#grid').datagrid('deleteRow', index);
                        //}
                        $('#grid').datagrid('loadData', { total: 0, rows: [] });
                        addGridRow();
                        $("#VendorID").val(vendorId);
                        $("#VendorCode").val(vendorCode);
                        $("#hidVendorCode").val(vendorCode);
                        $("#VendorName").val(vendorName);
                    }
                } else {
                    return false;
                }
            });
        } else {
            $("#VendorID").val(vendorId);
            $("#VendorCode").val(vendorCode);
            $("#hidVendorCode").val(vendorCode);
            $("#VendorName").val(vendorName);
        }
    } else {
        $("#VendorID").val(vendorId);
        $("#VendorCode").val(vendorCode);
        $("#hidVendorCode").val(vendorCode);
        $("#VendorName").val(vendorName);
    }
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
    $("#hidBuyEmpName").val(empName);
}

//选择商品资料
function showDialog(parm) {
    var VendorID = $("#VendorID").val();
    var subWID = $("#SubWID").combobox('getValue');
    //vendorId = 61;
    if (VendorID) {
        //仓库验证
        if (!subWID) {
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
            url: "../Common/GetBuyProductInfo",
            type: "get",
            dataType: "json",
            data: {
                vendorid: VendorID,
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
                        url: "../BuyOrderPre/SearchBuyProduct?productCode=" + parm + "&vendorId=" + VendorID + "&SubWID=" + subWID,
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
    row.BuyPackingQty = parseFloat(product.PackingQty).toFixed(2);
    row.BuyPrice = parseFloat(product.BuyPrice).toFixed(4);// product.BuyPrice;
    row.BarCode = product.BarCode;
    row.ProductId = product.ProductId;

    row.UnitPrice = product.UnitPrice;
    row.OldBuyPrice = product.OldBuyPrice;
    row.WStock = parseFloat(product.WStock).toFixed(2);

    row.SalePrice = parseFloat(product.SalePrice).toFixed(4);// product.BuyPrice;
    row.SaleUnitPrice = product.SaleUnitPrice;

    row.Unit = product.Unit;
    row.UnitPrice = product.UnitPrice;
    
    row.MinBuyPrice = product.UnitPrice;
    row.MaxBuyPrice = product.BuyPrice;
    row.MinSalePrice = product.SaleUnitPrice;
    row.MaxSalePrice = product.SalePrice;
    
    //aaa

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

//打印
var dlgChoose;
function print() {
    dlgChoose = frxs.dialog({
        title: "打印模版选择",
        url: "../BuyOrderPre/PrintChoose?BuyID=" + $("#BuyID").val(),
        owdoc: window.top,
        width: 410,
        height: 300
    });
}

//A4纸打印无价格
function closeSonOpenPrintA4No(buyid) {
    dlgChoose.dialog("close");
    //打印
    if (buyid != "") {
        var thisdlg = frxs.dialog({
            title: "A4纸打印无价格采购入库单",
            url: "../FastReportTemplets/Aspx/PrintPurChaseIn.aspx?BuyID=" + buyid + "&Type=A4No",
            owdoc: window.top,
            width: 760,
            height: 600
        });
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//A4纸打印有价格
function closeSonOpenPrintA4Yes(buyid) {
    dlgChoose.dialog("close");
    //打印
    if (buyid != "") {
        var thisdlg = frxs.dialog({
            title: "A4纸打印有价格采购入库单",
            url: "../FastReportTemplets/Aspx/PrintPurChaseIn.aspx?BuyID=" + buyid + "&Type=A4Yes",
            owdoc: window.top,
            width: 760,
            height: 600
        });
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//三联纸打印无价格
function closeSonOpenPrintThreeNo(buyid) {
    dlgChoose.dialog("close");
    //打印
    if (buyid != "") {
        var thisdlg = frxs.dialog({
            title: "三联纸打印无价格采购入库单",
            url: "../FastReportTemplets/Aspx/PrintPurChaseIn.aspx?BuyID=" + buyid + "&Type=ThreeNo",
            owdoc: window.top,
            width: 820,
            height: 600
        });
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//三联纸打印有价格
function closeSonOpenPrintThreeYes(buyid) {
    dlgChoose.dialog("close");
    //打印
    if (buyid != "") {
        var thisdlg = frxs.dialog({
            title: "三联纸打印有价格采购入库单",
            url: "../FastReportTemplets/Aspx/PrintPurChaseIn.aspx?BuyID=" + buyid + "&Type=ThreeYes",
            owdoc: window.top,
            width: 820,
            height: 600
        });
    } else {
        $.messager.alert("提示", "订单号不能为空！", "info");
    }
}

//订单 添加
function add() {
    location.href = '../BuyOrderPre/BuyOrderPreAddOrEditNew';
    frxs.updateTabTitle("添加采购收货单", "icon-add");
}

//订单 编辑
function edit() {
    SetPermission("edit");

    frxs.updateTabTitle("编辑采购收货单" + $("#BuyID").val(), "icon-edit");
}

//订单状态 确认
function sure() {
    var status = $("#Status").combobox('getValue');
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
                        $("#Status").combobox('setValue', '1');
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

//订单状态 反确认
function noSure() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
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
                status: 0
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "反确认成功！", "info");
                        $("#Status").combobox('setValue', '0');
                        $("#ConfUserName").val("");
                        $("#ConfTime").val("");
                        SetPermission("nosure");
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

//订单状态 过账
function posting() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return false;
    }
    var buyid = $("#BuyID").val();
    if (buyid != "") {
        $.messager.confirm("提示", "确定过账单据？", function (r) {
            if (r) {
                //禁用按钮
                var loading = window.top.frxs.loading("正在处理中，请稍后...");
                $.ajax({
                    url: "../BuyOrderPre/BuyOrderPreChangeStatus",
                    type: "get",
                    dataType: "json",
                    data: {
                        buyids: buyid,
                        status: 2
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $.messager.alert("提示", "过账成功！", "info");
                                $("#Status").combobox('setValue', '2');
                                $("#PostingUserName").val(result.Data.UserName);
                                $("#PostingTime").val(result.Data.Time);
                                SetPermission("posting");
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

//订单明细 导入
function importData() {
    var VendorID = $("#VendorID").val();
    var subWID = $("#SubWID").combobox('getValue');
    
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }

    if (VendorID) {
        var thisdlg = frxs.dialog({
            title: "数据导入",
            url: "../BuyOrderPre/ImportBuyOrderDetail?VendorID=" + VendorID + "&SubWID=" + subWID,
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
        $.messager.alert("提示", "请先选择供应商", "info");
    }
}

//订单 导出
function exportData() {
    var buyid = $("#BuyID").val();
    if (buyid != "") {
        location.href = "../BuyOrderPre/DataExport?Buyid=" + buyid;
    } else {
        $.messager.alert("提示", "收货单号为空！", "info");
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
            var product = products[i];
            product.Unit = product.MinUnit;
            $('#grid').datagrid('appendRow', product);
        }
        totalCalculate();
        return true;
    }
}

//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#addBtn").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            break;
        case "edit":
            $("#addBtn").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            break;
        case "query":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "nosure":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "sure":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "posting":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "save":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
    }

}

