//全局变量
var appId;
//切换提示
var comboVal;

//动作
var action;
$(function () {
    action = frxs.getUrlParam("action");

    //加载主表数据
    init();

    //仓库
    initDDL();

    $("#Status").val("录单");
});

//加载数据
function init() {
    appId = frxs.getUrlParam("appId");
    if (appId) {
        //编辑
        var load = frxs.loading("正在加载中，请稍后...");
        //编辑
        $.ajax({
            url: "../Again/GetAgainInfo",
            type: "post",
            data: { appId: appId },
            dataType: 'json',
            success: function (obj) {
                load.close();
                var appObj = obj.appPre;
                //格式化日期
                appObj.CreateTime = frxs.dateFormat(appObj.CreateTime);
                appObj.ConfTime = frxs.dateFormat(appObj.ConfTime);
                appObj.PostingTime = frxs.dateFormat(appObj.PostingTime);
                $("#AppID").val(appObj.AppID);

                var stausVal = appObj.Status;

                if (stausVal == "0") {
                    appObj.Status = "录单";
                } else if (stausVal == "1") {
                    appObj.Status = "确认";
                }
                else if (stausVal == "2") {
                    appObj.Status = "过账";
                }


                $('#myForm').form('load', appObj);

                loadgrid(obj.gridData);

                //高度自适应
                gridresize();

                //查看
                if (stausVal == 0) {
                    SetPermission('query');
                }
                else if (stausVal == 1) {
                    SetPermission('sure');
                }
                else if (stausVal == 2) {
                    SetPermission('posting');
                }

                totalCalculate();

                var rows = $("#grid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    var index = $('#grid').datagrid('getRowIndex', rows[i]);

                    rows[i].AppQty = parseFloat(rows[i].AppQty).toFixed(2);
                    rows[i].UnitQty = parseFloat(rows[i].UnitQty).toFixed(2);
                    rows[i].Price = parseFloat(rows[i].Price).toFixed(4);
                    rows[i].SubAmt = parseFloat(rows[i].SubAmt).toFixed(4);
                    rows[i].MinUnit = rows[i].Unit;
                    rows[i].Unit = rows[i].AppUnit;
                    
                    //更新行
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: rows[i]
                    });
                }
                //仓库不可编辑
                $("#SubWID").combobox("disable");
            }
        });
    } else {

        //添加
        loadgrid();

        //高度自适应
        gridresize();

        //初始化添加一行
        addGridRow();

        SetPermission('add');

    }
}

//实现对DataGird控件的绑定操作
function loadgrid(griddata) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        data: griddata,
        idField: 'AppId',                  //主键
        pageSize: 2000,                       //每页条数
        pageList: [5, 10, 1000, 2000],//可以设置每页记录条数的列表 
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
            { field: 'ck', checkbox: true }, //选择
            {
                title: frxs.setTitleColor('商品编码'), field: 'SKU', width: 100, editor: 'text', align: 'center'
            },
            { title: '商品名称', field: 'ProductName', width: 220, formatter: frxs.replaceCode }
        ]],
        columns: [[
            {
                title: frxs.setTitleColor('单位'), field: 'Unit', align: 'center', width: 80, editor: {
                    type: 'combobox',
                    options: {
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
                                row.PackingQty = value;

                                //下拉框设置
                                var text = $(this).combobox("getText");
                                row.Unit = text;

                                if (text == row.MinUnit) {
                                    row.Price = row.UnitPrice;
                                } else {
                                    row.Price = row.MaxBuyPrice;
                                }

                                //单价*数量*平台费率
                                row.SubAmt = parseFloat(row.Price * row.AppQty).toFixed(4);
                                row.UnitQty = parseFloat(row.AppQty * row.PackingQty).toFixed(2);
                                row.Price = parseFloat(row.Price).toFixed(4);
                                row.PackingQty = parseFloat(row.PackingQty).toFixed(2);

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
                title: frxs.setTitleColor('返配数量'), field: 'AppQty', width: 120, editor: 'text', align: 'center'
            },
            {
                title: '返配价格', field: 'Price', width: 120, align: 'right'
            },
            {
                title: '返配金额', field: 'SubAmt', width: 120, align: 'right'
            },
            { title: '包装数', field: 'PackingQty', width: 120, align: 'center' },
            {
                title: '总数量', field: 'UnitQty', width: 120, align: 'center'
            },
             { title: '库存数量', field: 'WStock', width: 100, align: 'center' },

            {
                title: '国际条形码', field: 'BarCode', width: 140, align: 'center'
            },
            {
                title: frxs.setTitleColor('备注'), field: 'Remark', width: 140, editor: 'text', formatter: frxs.replaceCode
            },
            
            { title: 'ProductId', field: 'ProductId', hidden: true },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true },
            { title: 'MinUnit', field: 'MinUnit', hidden: true },
            { title: 'MaxBuyPrice', field: 'MaxBuyPrice', hidden: true }
        ]],
        toolbar: toolbarArray
    });
};


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
        }
    });
}

//添加
function addGridRow() {
    var ciLen = $('#grid').datagrid('getRows').length - 1;
    var lastRow = $('#grid').datagrid('getRows')[ciLen];

    if (lastRow && !lastRow.SKU) {

        $('#grid').datagrid('clearSelections');
        onClickCell($('#grid').datagrid('getRows').length - 1, "SKU");

        return false;
    } else {
        var index = $('#grid').datagrid('getRows').length;
        $("#grid").datagrid("insertRow", {
            index: index + 1,
            row: {}
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
    if (!is) {
        $.messager.alert("提示", "请选择要删除的数据！", "info");
        return false;
    }

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

    //计算总数量
    totalCalculate();
}

//清空查询表单
function reset() {

}

//保存
function saveData() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }

    $('#grid').datagrid('endEdit', editIndex);
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";
    var count = 0;
    var message = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            if (rows[i].AppQty <= 0) {
                message = "商品：" + rows[i].ProductName + " 返配数量不能等于 0<br/>";
                $.messager.alert("提示", message, "info");
                return false;
            }
            if (rows[i].UnitQty > 9999999999.99) {
                message = "商品：" + rows[i].ProductName + " 总数量不能大于 9999999999.99";
                $.messager.alert("提示", message, "info");
                return false;
            }
            
            jsonStr += "{";
            jsonStr += "\"ProductId\":\"" + rows[i].ProductId + "\",";
            jsonStr += "\"Unit\":\"" + rows[i].Unit + "\",";
            jsonStr += "\"AppQty\":\"" + rows[i].AppQty + "\",";
            jsonStr += "\"PackingQty\":\"" + rows[i].PackingQty + "\",";
            jsonStr += "\"Price\":\"" + rows[i].Price + "\",";
            jsonStr += "\"Remark\":\"" + frxs.filterText(rows[i].Remark ? rows[i].Remark : "") + "\"";

            jsonStr += "},";
            count++;
        }
    }
    jsonStr = jsonStr.substring(0, jsonStr.length - 1);
    jsonStr += "]";
    if (message) {
        $.messager.alert("提示", message);
        return false;
    }
    if (count == 0) {
        $.messager.alert("提示", "商品数据不能为空！", "info");
        return false;
    }

    var jsonAgain = "{";
    jsonAgain += "\"SubWID\":'" + $("#SubWID").combobox('getValue') + "',";
    jsonAgain += "\"Remark\":\"" + frxs.filterText($("#Remark").val()) + "\",";
    //是否编辑
    if ($("#AppID").val() != "") {
        jsonAgain += "\"AppId\":\"" + $("#AppID").val() + "\",";
    }
    jsonAgain += "\"Type\":\"0\"";     //(0:返配 1:补货)

    jsonAgain += "}";


    var loading = window.top.frxs.loading("正在加载中，请稍后...");

    $.ajax({
        url: "../Again/AgainAddOrEditHandle",
        type: "post",
        data: { jsonData: jsonAgain, jsonDetails: jsonStr },
        dataType: "json",
        success: function (result) {
            loading.close();
            if (result.Flag == "SUCCESS") {
                if ($("#AppID").val() != "") {
                    $.messager.alert("提示", "编辑成功", "info");
                } else {
                    var appId = $.parseJSON(result.Data);
                    $("#AppID").val(appId);
                    $.messager.alert("提示", "添加成功", "info");
                }
                SetPermission("save");
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
    var h = ($(window).height() - $("fieldset").height() - 158);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 12,
        height: h
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
    if (field == "AppQty") {
        obj.bind("change", function () {
            changeAppQty($(this).val());
        });
    }

    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "SKU") {
                if ($(this).val() == "") {
                    showDialog();
                    $(this).blur();
                } else if ($(this).val() != valueinput) {
                    showDialog($(this).val());
                    $(this).blur();
                } else {
                    nextControl();
                }
            } else {
                if (field == "AppQty") {
                    obj.change();
                }
                nextControl();
            }
        }

        //向下
        if (e.keyCode == 40) {
            if (field == "AppQty") {
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
            if (field == "AppQty") {
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



//验证 并且更新grid的行数据 
function changeAppQty(value) {
    if (isNaN(value) || value == "") {
        value = 0;
    }
    if (value < 0) {
        value = 0;
    }
    if (value > 9999999999.99) {
        value = 9999999999.99;
    }

    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    var packingQty = row.PackingQty;
    var price = row.Price;

    row.AppQty = parseFloat(value).toFixed(2);
    row.UnitQty = parseFloat(parseFloat(value) * parseFloat(packingQty)).toFixed(2);
    row.SubAmt = parseFloat(parseFloat(value) * parseFloat(price)).toFixed(4);

    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });
    totalCalculate();
}


//计算总数量
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totalSubAmt = 0.0000;
    var totalUnitQty = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            var appQty = parseFloat(rows[i].AppQty);
            var subAmt = parseFloat(rows[i].SubAmt);
            totalUnitQty += appQty;
            totalSubAmt += subAmt;
        }
    }

    $("#TotalAmt").val(parseFloat(totalSubAmt).toFixed(4));
    $('#grid').datagrid('reloadFooter', [
       { "AppQty": "数量总计：" + parseFloat(totalUnitQty).toFixed(2), "SubAmt": "金额总计：" + parseFloat(totalSubAmt).toFixed(4) }
    ]);
}



//去下一个可以编辑框
function nextControl() {
    var selected = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', selected);
    var currentField = "SKU";

    //编辑字段
    var fields = "SKU,AppQty".split(',');
    
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
    if (editfield == "Unit") {
        $('#grid').datagrid('updateRow', {
            index: index,
            row: selected
        });
        currentField = "AppQty";
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


//选择商品资料   不需传递供应商ID,但需模糊查找
function showDialog(parm) {
    //仓库验证
    if (!$("#SubWID").combobox('getValue')) {
        $.messager.alert("提示", "请先选择仓库", "info");
        return false;
    }
    
    //如果商品编码有参数
    if (parm) {
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
                Value: parm,
                Type: type,
                vendorid: 0,
                SKULikeSearch: true,
                subWId: $("#SubWID").combobox('getValue')
            },
            success: function (obj) {
                if (obj && obj.total == 1) {
                    backFillProduct(obj.rows[0]);
                } else {
                    showSearchProduct(parm);
                }
            }
        });
    } else {
        //如果没有参数直接弹出
        showSearchProduct("");
    }
}

function showSearchProduct(parm) {
    frxs.dialog({
        title: "选择商品资料",
        url: "../BuyOrderPre/SearchBuyProduct?productCode=" + parm + "&SubWID=" + $("#SubWID").combobox('getValue') + "&vendorId=0",
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填商品信息
function backFillProduct(product) {
    //判断是否重复
    var rep = false;
    var rows = $("#grid").datagrid("getRows");
    var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));

    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU == product.SKU && index != i) {
            rep = true;
            break;
        }
    }
    if (rep) {

        $.messager.alert("提示", "存在重复数据。", "info", function () {
            var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));
            onClickCell(index, "SKU");
        });
        return;

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
    row.BarCode = product.BarCode;
    row.Unit = product.BuyUnit;
    row.PackingQty = parseFloat(product.PackingQty).toFixed(2);
    row.Price = parseFloat(product.BuyPrice).toFixed(4);
    row.ProductId = product.ProductId;

    row.MinUnit = product.Unit;
    row.UnitPrice = product.UnitPrice;
    row.MaxBuyPrice = product.BuyPrice;

    row.AppQty = "0.00";
    row.WStock = product.WStock;
    row.UnitQty = "0.00";
    row.SubAmt = "0.0000";

    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    onClickCell(index, "AppQty");
}



//订单 添加
function add() {
    //修改标题
    frxs.updateTabTitle("添加返配申请单", "icon-add");
    location.href = '../Again/AgainApplyAddOrEdit';
}

//订单 编辑
function edit() {

    SetPermission("edit");

    //仓库可编辑
    $("#SubWID").combobox("enable");

    //修改标题
    frxs.updateTabTitle("编辑返配申请单" + $("#AppID").val(), "icon-edit");
    //location.href = '../Again/AgainApplyAddOrEdit?appId='+$("#AppID").val();
    //SetPermission("edit");
}

//订单状态 确认
function sure() {
    var status = $("#Status").val();
    if (status != "录单") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return false;
    }
    var appId = $("#AppID").val();
    if (appId != "") {
        ajaxSure(appId, 1);
    }
}

//订单状态 反反确认
function reSure() {
    var status = $("#Status").val();
    if (status != "确认") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
        return false;
    }
    var appId = $("#AppID").val();
    if (appId != "") {
        ajaxSure(appId, 0);
    }
}


function ajaxSure(appId, type) {
    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../Again/AgainConfirmOrReconfirm",
        type: "get",
        dataType: "json",
        data: {
            appIds: appId,
            type: type,
            menuType: 0
        },
        success: function (result) {
            loading.close();
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {

                    if (type == 0) {
                        $.messager.alert("提示", "反确认成功！", "info");
                        $("#Status").val('录单');
                        SetPermission("nosure");
                    } else {
                        $("#Status").val('确认');
                        SetPermission("sure");
                        $.messager.alert("提示", "确认成功！", "info");
                    }

                } else {
                    $.messager.alert("提示", result.Info, "info");
                }
            }
        }
    });
}

//订单状态 过账
function posting() {
    var status = $("#Status").val();
    if (status != "确认") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return false;
    }
    var appId = $("#AppID").val();
    if (appId != "") {
        $.messager.confirm("提示", "确定过账单据？", function (r) {
            if (r) {
                //遮罩层
                var loading = window.top.frxs.loading();
                $.ajax({
                    url: "../Again/AgainPosting",
                    type: "get",
                    dataType: "json",
                    data: {
                        appIds: appId,
                        menuType: 0
                    },
                    success: function (result) {
                        loading.close();
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $.messager.alert("提示", "过账成功！", "info");

                                $("#Status").val('过账');
                                SetPermission("posting");
                            } else {
                                $.messager.alert("提示", result.Info, "info");
                            }
                        }
                    }
                });
            }
        });
    }
}

//导入
function importData() {
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }
    var thisdlg = frxs.dialog({
        title: "数据导入",
        url: "../Again/ImportAgain",
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
}

//打印
function print() {
    var appId = $("#AppID").val();
    if (appId != "") {
        var thisdlg = frxs.dialog({
            title: "打印返配申请单",
            url: "../FastReportTemplets/Aspx/PrintAgainApply.aspx?AppID=" + appId,
            owdoc: window.top,
            width: 765,
            height: 600
        });
    } else {
        $.messager.alert("提示", "单据号不能为空！", "info");
    }

}
//数据回填-导入
function backFillProductImport(products) {
    var rows = $("#grid").datagrid("getRows");
    var msg = "";
    for (var i = 0; i < products.length; i++) {
        var product = products[i];
        for (var j = 0; j < rows.length; j++) {
            if (rows[j].SKU == product.SKU) {
                msg += product.SKU + " | ";
            }
        }
    }
    if (msg) {
        window.top.$.messager.alert("提示", "存在重复数据 “商品编码：" + msg + "”。", "info");
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

            var row = {};
            row.SKU = product.SKU;
            row.ProductName = product.ProductName;
            row.BarCode = product.BarCode;
            row.Unit = product.Unit;
            row.PackingQty = product.PackingQty;
            row.Price = parseFloat(product.NewPrice).toFixed(4);
            row.ProductId = product.ProductId;
            row.AppQty = parseFloat(product.AppQty).toFixed(2);
            row.WStock = product.WStock;

            row.UnitQty = parseFloat(parseFloat(product.AppQty) * parseFloat(product.PackingQty)).toFixed(2);
            row.SubAmt = parseFloat(parseFloat(product.AppQty) * parseFloat(product.NewPrice)).toFixed(4);
            
            row.MinUnit = product.MinUnit;
            row.UnitPrice = product.UnitPrice;
            row.MaxBuyPrice = product.BuyPrice;

            $('#grid').datagrid('appendRow', row);
        }
        totalCalculate();
        return true;
    }
}

//导出
function exportData() {
    if (!appId) {
        appId = $("#AppID").val();
    }
    location.href = "../Again/ExportExcelAgain?appId=" + appId + "&type=0";
}

//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#btnAdd2").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('enable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            break;
        case "edit":
            $("#btnAdd2").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            break;
        case "query":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "nosure":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "sure":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "posting":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
        case "save":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#importBtn").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            break;
    }

}


