
var orderid;
var shpoid;

$(function () {
    orderid = frxs.getUrlParam("OrderID");
    shpoid = frxs.getUrlParam("ShopID");

    //加载详情数据
    loadgrid();

    //高度自适应
    gridresize();
});



//加载数据
function loadgrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../WarehouseOrder/GetWaitPickDetails?OrderID=' + orderid,
        idField: 'ProductID',                      //主键
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
        onLoadSuccess: onLoadSuccess,
        queryParams: {

        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: '商品编码', field: 'SKU', width: 80, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 120, formatter: frxs.formatText },
            {
                title: '单位', field: 'SaleUnit', width: 50, align: 'center'
                //editor: {
                //    type: 'combobox',
                //    options: {
                //        valueField: 'PackingQty',
                //        textField: 'UnitName',
                //        editable: false,
                //        panelHeight: 'auto',
                //        onChange: function (value) {
                //            //设置为Text
                //            if (!isNaN(value)) {
                //                var row = $("#grid").datagrid("getSelected");
                //                var index = $("#grid").datagrid("getRowIndex", row);

                //                //获取包装数
                //                var oldSalePackingQty = row.SalePackingQty;
                //                var oldPickQty = row.PickQty;

                //                //计算数量
                //                var Qty = parseFloat(parseFloat(oldSalePackingQty) * parseFloat(oldPickQty) / parseFloat(value)).toFixed(2);

                //                //计算订单数量
                //                row.PickQty = Qty;

                //                //计算数量
                //                row.CheckQty = Qty;

                //                //赋值包装数
                //                row.SalePackingQty = value;

                //                //下拉框设置
                //                var text = $(this).combobox("getText");
                //                row.SaleUnit = text;

                //                if (text != row.OldSaleUnit) {
                //                    row.IsSet = 1;
                //                } else {
                //                    row.IsSet = 0;
                //                }

                //                totalCalculate();

                //                $(".datagrid-row .combo").prev().combobox("hidePanel");
                //                //更新行
                //                $('#grid').datagrid('updateRow', {
                //                    index: index,
                //                    row: row
                //                });
                //            }

                //        }
                //    }
                //}
            },
            { title: '包装数', field: 'SalePackingQty', width: 50, align: 'center' },
            { title: '拣货数量', field: 'PickQty', width: 150, align: 'right' },
            {
                title: frxs.setTitleColor('对货数量'), field: 'CheckQty', align: 'right', width: 150, editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2,
                        min: 0
                    }, formatter: function (value) {
                        return value ? value : 0;
                    }
                }
            },
            { title: '备注', field: 'Remark', width: 80 },
            { title: '单位', field: 'OldSaleUnit', width: 80, hidden: true },
            { title: '商品ID', field: 'ProductID', width: 80, hidden: true },
            { title: 'IsSet', field: 'IsSet', width: 80, hidden: true },
            { title: '国际条码', field: 'BarCode', width: 110 },
            { title: '货位', field: 'ShelfCode', width: 50, align: 'center' }
        ]]
    });
};



//保存方法
function saveData() {
    window.parent.$(window.frameElement).closest(".window").find("#btnPickCheckOk").addClass("easyui-linkbutton").linkbutton('disable');
    var loading = frxs.loading("正在加载中，请稍后...");
    var rows = $('#grid').datagrid('getRows');
    var products = JSON.stringify(rows);
    $.ajax({
        url: "../WarehouseOrder/PickCheckHandle",
        type: "post",
        dataType: "json",
        data: {
            OrderID: orderid,
            Products: products
        },
        success: function (result) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnPickCheckOk").addClass("easyui-linkbutton").linkbutton('enable');
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    window.frameElement.wapi.$("#grid").datagrid("reload");
                    //window.frameElement.wapi.focus();
                    //调用开始装箱 packStart(orderid, shopid) 
                    window.frameElement.wapi.packStart(orderid, shpoid);
                    frxs.pageClose();
                } else {
                    window.top.$.messager.alert("提示", result.Info, "info");
                }
            }
        },
        error: function (request, textStatus, errThrown) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnPickCheckOk").addClass("easyui-linkbutton").linkbutton('enable');
            if (textStatus) {
                window.top.$.messager.alert("提示", textStatus, "info");
            } else if (errThrown) {
                window.top.$.messager.alert("提示", errThrown, "info");
            } else {
                window.top.$.messager.alert("提示", "出现错误", "info");
            }
        }
    });
}


//加载成功
function onLoadSuccess() {
    var rows = $('#grid').datagrid('getRows');
    for (var i = 0; i < rows.length; i++) {
        if (!rows[i].CheckQty) {
            rows[i].CheckQty = rows[i].PickQty;
        } 
        rows[i].OldSaleUnit = rows[i].SaleUnit;
        rows[i].IsSet = 0;
        $('#grid').datagrid('updateRow', {
            index: i,
            row: rows[i]
        });
    }
    totalCalculate();
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
            $('#grid').datagrid('clearSelections');     //清除所有选中
            $('#grid').datagrid('selectRow', param.index);

            var ed = $(this).datagrid('getEditor', param);
            if (ed && ed.type && ed.type == "combobox") {
                var productId = $("#grid").datagrid("getSelected").ProductID;
                if (productId > 0) {
                    var url = '../Common/GetSaleProductUnit?ProductID=' + productId;
                    ed.target.combobox('reload', url); //联动下拉列表重载
                }
            }
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
                    if (ed.field == "CheckQty") {
                        obj.attr("maxlength", "6");
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

    setTimeout(function () { obj.select(); }, 100);
    valueinput = $(obj).val();

    if (field == "CheckQty") {
        obj.bind("change", function () {
            changeQtyValue($(this).val());
        });
    }
    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "CheckQty") {
                obj.change();
            }
            nextRow();  //去下一行
        }
        //向下
        if (e.keyCode == 40) {
            if (field == "CheckQty") {
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
        //向上
        if (e.keyCode == 38) {
            if (field == "CheckQty") {
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
}

//去下一行
function nextRow() {
    var selected = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', selected);
    var currentField = "CheckQty";
    var len = $("#grid").datagrid("getRows").length;
    index = index + 1;
    if (len > index) {
        onClickCell(index, currentField);
    }
}


//更新grid的行数据
function changeQtyValue(Qty) {
    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);
    Qty = Qty > 0 ? Qty : 0;
    row.CheckQty = Qty;
    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    totalCalculate();
}

//Foot计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");

    var totalCheckQty = 0.00;
    var totalPickQty = 0.00;
    for (var i = 0; i < rows.length; i++) {
        var CheckQty = parseFloat(rows[i].CheckQty);
        var PickQty = parseFloat(rows[i].PickQty);
        totalCheckQty += CheckQty;
        totalPickQty += PickQty;
    }

    $('#grid').datagrid('reloadFooter', [
       { "CheckQty": "对货数总计：" + parseFloat(totalCheckQty).toFixed(2), "PickQty": "拣货数总计：" + parseFloat(totalPickQty).toFixed(2) }
    ]);
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
    editfield = field;
    if (endEditing()) {
        $('#grid').datagrid('selectRow', index).datagrid('editCell', { index: index, field: field });
        editIndex = index;
    }
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 10);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 5,
        height: h
    });
}