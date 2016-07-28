//全局变量
var adjId;

//动作
var action;
$(function () {
    action = frxs.getUrlParam("action");
    
    //加载主表数据
    init();

    $("#Status").val("录单");
});

//加载数据
function init() {
    adjId = frxs.getUrlParam("adjId");
    if (adjId) {
        //编辑
        var load = frxs.loading("正在加载中，请稍后...");
        $.ajax({
            url: "../ProductList/GetProductPriceInfo",
            type: "post",
            data: { ajdId: adjId },
            dataType: 'json',
            success: function (obj) {
                load.close();
                var adjObj = obj.adjPrice;
                //格式化日期
                adjObj.BeginTime = frxs.dateFormat(adjObj.BeginTime);

                //生效时间的有效选择时间
                $("#BeginTime").attr("onclick", "WdatePicker({ dateFmt: 'yyyy-M-d HH:mm' })");
                
                adjObj.CreateTime = frxs.dateFormat(adjObj.CreateTime);
                adjObj.ConfTime = frxs.dateFormat(adjObj.ConfTime);
                adjObj.PostingTime = frxs.dateFormat(adjObj.PostingTime);

                var stausVal = adjObj.Status;

                if (stausVal == "0") {
                    adjObj.Status = "录单";
                } else if (stausVal == "1") {
                    adjObj.Status = "确认";
                }
                else if (stausVal == "2") {
                    adjObj.Status = "过账";
                }


                $('#myForm').form('load', adjObj);


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
        idField: 'AdjID',                  //主键
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
            { field: 'ck', checkbox: true }, //选择
            {
            title: frxs.setTitleColor('商品编码'), field: 'SKU', width: 100,align: 'center', editor: 'text'
            },
            { title: '商品名称', field: 'ProductName', width: 220, formatter: frxs.replaceCode }
            //冻结列
        ]],
        columns: [[
           
            {
                title: '原库存单位进货价', field: 'OldPrice', width: 180, align: 'right', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4) : "0.0000";
            }},
            {
            title: frxs.setTitleColor('库存单位进货价'), field: 'Price', width: 200, align: 'right', editor: 'text', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4) : "0.0000";
                }
            },
             {
                 title: '库存单位', field: 'Unit', width: 140, align: 'center'
             },
            { title: '包装数', field: 'PackingQty', width: 140, align: 'center' },
            { title: '国际条码', field: 'BarCode', width: 180, align: 'center' },
            { title: 'ProductID', field: 'ProductID', width: 180, hidden: true }
        ]],
        toolbar: toolbarArray
    });


};


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
            row: { }
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
}


//保存
function saveData() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }

    if (!adjId) {
        var beginTime = $("#BeginTime").val();
        var bt = new Date(beginTime);
        if (bt < new Date(frxs.nowDateTime())) {//只算到分钟
            $.messager.alert("提示", "生效时间不能小于当前时间！", "info");
            return false;
        }
    }

    $('#grid').datagrid('endEdit', editIndex);
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";
    var count = 0;
    var message = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            
            if (rows[i].Price < 0 || rows[i].Price > 999999.9999) {
                message = "商品：" + rows[i].ProductName + " 库存单位进货价应为 0-999999.9999 之间<br/>";
                $.messager.alert("提示", message, "info");
                return false;
            }

            jsonStr += "{";
            jsonStr += "\"ProductID\":\"" + rows[i].ProductID + "\",";
            jsonStr += "\"Price\":\"" + rows[i].Price + "\"";
            jsonStr += "},";
            count++;
        }
    }
    jsonStr = jsonStr.substring(0, jsonStr.length - 1);
    jsonStr += "]";
    
    if (message) {
        $.messager.alert("提示", message, "info");
        return false;
    }

    if (count == 0) {
        $.messager.alert("提示", "商品数据不能为空！", "info");
        return false;
    }


    var jsonBuyOrder = "{";
    jsonBuyOrder += "\"BeginTime\":\"" + $("#BeginTime").val() + "\",";
    jsonBuyOrder += "\"Remark\":\"" + frxs.filterText($("#Remark").val()) + "\",";
    //是否编辑
    if ($("#AdjID").val() != "") {
        jsonBuyOrder += "\"AdjID\":\"" + $("#AdjID").val() + "\",";
    }
    jsonBuyOrder += "\"AdjType\":\"0\"";     //(0:采购(进货)价;1:配送(批发)价;3:费率及积分)

    jsonBuyOrder += "}";

    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../ProductList/ProductPriceChangeAddOrEditHandle",
        type: "post",
        data: { jsonData: jsonBuyOrder, jsonDetails: jsonStr },
        dataType: "json",
        success: function (result) {
            loading.close();
            if (result.Flag == "SUCCESS") {
                if ($("#AdjID").val() != "") {
                    $.messager.alert("提示", "编辑成功", "info");
                } else {
                    var adjId = $.parseJSON(result.Data).Data;
                    $("#AdjID").val(adjId);
                    $.messager.alert("提示", "添加成功", "info");
                }
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

    if (field == "Price") {
        obj.bind("change", function () {
            changeBuyPriceValue($(this).val());
        });
    }

    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "SKU") {
                if ($(this).val()=="") {
                    showDialog();
                    $(this).blur();
                }else if ($(this).val() != valueinput){
                    showDialog($(this).val());
                    $(this).blur();
                } else {
                    nextControl();
                }
            } else {
                if (field == "Price") {
                    obj.change();
                }
                nextControl();
            }
        }

        //向下
        if (e.keyCode == 40) {
            if (field == "Price") {
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
            if (field == "Price") {
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


//计算 并且更新grid的行数据
function changeBuyPriceValue(price) {

    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    if (isNaN(price) || price == "" || price < 0) {
        row.Price = "0.0000";
    } else {
        row.Price = parseFloat(price).toFixed(4);
    }

    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

}


//去下一个可以编辑框
function nextControl() {
    var selected = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', selected);
    var currentField = "SKU";

    //编辑字段
    var fields = "SKU,Price".split(',');
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
    //如果商品编码有参数
    if (parm) {
        var type = "SKU";
        //判断需要查询的类型
        var onebyte = parm ? parm.substring(0, 1) : "";
        if (isNaN(onebyte) && onebyte != "赠") {
            type = "ProductName";
        }
        
        $.ajax({
            url: "../Common/GetSaleProductInfo",
            type: "get",
            dataType: "json",
            data: {
                Value: parm,
                Type: type,
                SKULikeSearch: true
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
        url: "../SaleBackPre/SearchSaleProduct?productCode=" + parm,
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
    row.Unit = product.Unit;
    row.PackingQty = product.PackingQty;

    row.OldPrice = parseFloat(product.BuyPrice).toFixed(4);
    row.BarCode = product.BarCode;
    row.ProductID = product.ProductId;

    row.Price = '0.0000';
    row.PackingQty1 = "没做";


    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    onClickCell(index, "Price");
}



//订单 添加
function add() {
    //修改标题
    frxs.updateTabTitle("添加进货价调整单", "icon-add");
    location.href = '../InsertPriceChange/InsertPriceOrderAddOrEdit';
}

//订单 编辑
function edit() {
    //修改标题
    SetPermission("edit");
    frxs.updateTabTitle("编辑进货价调整单" + $("#AdjID").val(), "icon-edit");
    //location.href = '../InsertPriceChange/InsertPriceOrderAddOrEdit?adjId=' + $("#AdjID").val();
    
}

//订单状态 确认
function sure() {
    var status = $("#Status").val();
    if (status != "录单") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return false;
    }
    var adjId = $("#AdjID").val();
    if (adjId != "") {
        ajaxSure(adjId, 0);
    }
}

//订单状态 反反确认
function reSure() {
    var status = $("#Status").val();
    if (status != "确认") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
        return false;
    }
    var adjId = $("#AdjID").val();
    if (adjId != "") {
        ajaxSure(adjId, 1);
    }
}


function ajaxSure(adjId, type) {
    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../ProductList/ProductPriceConfirmOrReconfirm",
        type: "get",
        dataType: "json",
        data: {
            ajdIds: adjId,
            type: type,
            menuType: 0
        },
        success: function (result) {
            loading.close();
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    if (type == 1) {
                        $("#Status").val('录单');
                        $.messager.alert("提示", "反确认成功！", "info");
                        SetPermission("nosure");
                    } else {
                        $("#Status").val('确认');
                        $.messager.alert("提示", "确认成功！", "info");
                        SetPermission("sure");
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
    var adjId = $("#AdjID").val();
    if (adjId != "") {
        $.messager.confirm("提示", "确定过账单据？", function(r) {
            if (r) {
                //遮罩层
                var loading = window.top.frxs.loading();
                $.ajax({
                    url: "../ProductList/ProductPricePosting",
                    type: "get",
                    dataType: "json",
                    data: {
                        ajdIds: adjId,
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
    var thisdlg = frxs.dialog({
        title: "数据导入",
        url: "../InsertPriceChange/ImportInsertPrice",
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


//打印进价调整单
function print() {
    var adjid = $("#AdjID").val();
    if (adjid != "") {
        var thisdlg = frxs.dialog({
            title: "打印进价调整单",
            url: "../FastReportTemplets/Aspx/PrintInsertPrice.aspx?AdjID=" + adjid,
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
    //$('#grid').datagrid('endEdit', index);

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
            row.Unit = product.Unit;
            row.PackingQty = parseFloat(product.PackingQty).toFixed(2);

            row.OldPrice = parseFloat(product.BuyPrice).toFixed(4);
            row.BarCode = product.BarCode;
            row.ProductID = product.ProductId;

            row.Price = parseFloat(product.NewPrice).toFixed(4);


            $('#grid').datagrid('appendRow', row);

        }

        return true;
    }
}



//导出
function exportData() {
    if (!adjId) {
        adjId = $("#AdjID").val();
    }
    location.href = "../InsertPriceChange/ExportExcelInsertPrice?ajdId=" + adjId;
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