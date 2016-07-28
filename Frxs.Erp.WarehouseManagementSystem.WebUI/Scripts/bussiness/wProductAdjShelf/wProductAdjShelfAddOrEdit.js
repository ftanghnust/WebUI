var dialogWidth = 950;
var dialogHeight = 600;
var adjID = "";
var isNewRow = false;

$(function () {

    init("1"); //直接加载

});

//加载数据
function init(initType) {

    if (initType == "1") {
        adjID = frxs.getUrlParam("ID");
        type = frxs.getUrlParam("Type");
    } else {

        adjID = $("#AdjID").val();
        type = "edit";
    }
        $.ajax({
            url: "../WProductAdjShelf/WProductAdjShelfAddOrEdit",
            type: "post",
            data: { id: adjID },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);

                if (obj.PageTitle == "Edit") {

                    //加载数据
                    loadgrid();

                    //高度自适应
                    gridresize();

                    //下拉绑定
                    initDDL();

                    if (obj.Status == 0) {//录单
                        if (type == "edit") {
                            SetPermission('edit');
                        } else {
                            SetPermission('query');
                        }
                    }
                    else if (obj.Status == 1) {//确认
                        SetPermission('sure');
                    }
                    else if (obj.Status == 2) {//已生效
                        SetPermission('posting');
                    }
                } else
                {
                    isNewRow = true;
                    //加载数据
                    loadgrid();

                    //高度自适应
                    gridresize();
                    //下拉绑定
                    initDDL();

                    SetPermission('add');

                    $("#Status").val("0") ; //默认
                    $("#StatusToStr").val("录单") ;
                }
            }
        });
   
}

//实现对DataGird控件的绑定操作
function loadgrid() {
    $('#gridDetail').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式 
        url: '../WProductAdjShelf/GetWProductAdjShelfDetailList',          //Aajx地址
        idField: 'ID',                  //主键
        pageSize: 100,                       //每页条数
        pageList: [100, 200, 500, 2000],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickCell: onClickCell,
        onClickRow: function (rowIndex, rowData) {
            $('#gridDetail').datagrid('clearSelections');
            $('#gridDetail').datagrid('selectRow', rowIndex);

        },
        queryParams: {
            AdjID: adjID
        },
        onLoadSuccess: function () {
         //   addGridRow();
            
        },
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }
        ]],
        columns: [[
            { title: 'WID', field: 'WID', hidden: true },
            { title: 'WProductID', field: 'WProductID', hidden: true },
            { title: 'ProductId', field: 'ProductId', hidden: true },
            { title: '商品编码', field: 'SKU', width: 100, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 200 },
            { title: '配送单位', field: 'Unit', width: 100 },
            { title: '包装数', field: 'BigPackingQty', width: 100, align: 'center' },
            { title: 'OldShelfID', field: 'OldShelfID', hidden: true },
            { title: '原货位号', field: 'OldShelfCode', width: 100, align: 'center' },
            { title: 'ShelfID', field: 'ShelfID', hidden: true },
            { title: frxs.setTitleColor('新货位号'), field: 'ShelfCode', width: 100, editor: 'text', align: 'center', formatter: frxs.replaceCode },
            { title: '商品条码', field: 'BarCode', width: 100, align: 'center' },
            { title: frxs.setTitleColor('备注'), field: 'Remark', width: 260, editor: 'text', formatter: frxs.formatText }
        ]], toolbar: toolbarArray
    });
};

//显示导入界面
function showImport() {
  
    var thisdlg = frxs.dialog({
        title: "货位调整-Excel数据导入",
        url: "../WProductAdjShelf/ImportWProductAdjShelfView",
        owdoc: window.top,
        width: 820,
        height: 650,
        buttons: [{
            text: '导入',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                
            }
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

var editIndex = undefined;
var editfield = undefined;
function endEditing() {

    if (editIndex == undefined) {
        return true;
    }
    if ($('#gridDetail').datagrid('validateRow', editIndex)) {
        $('#gridDetail').datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

//单击单元格事件
function onClickCell(index, field) {
    
    if ($("#btnAdd").hasClass("l-btn-disabled") == false) {
        editfield = field;

        if (endEditing()) {
            $('#gridDetail').datagrid('selectRow', index)
                .datagrid('editCell', { index: index, field: field });
            editIndex = index;
        }
    }
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

            $('#gridDetail').datagrid('clearSelections'); //清除所有选中
            $('#gridDetail').datagrid('selectRow', param.index);

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

    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
          
                nextControl();
            
        }
        //向下
        if (e.keyCode == 40) {

            var selected = $('#gridDetail').datagrid('getSelected');
            var index = $('#gridDetail').datagrid('getRowIndex', selected);
            var countIndex = $('#gridDetail').datagrid('getRows');
            $('#gridDetail').datagrid('clearSelections');

            if (index + 1 < countIndex.length) {
                onClickCell(index + 1, field);
            } else {
                onClickCell(0, field);
            }
        }
        //向下
        if (e.keyCode == 38) {

            var selected = $('#gridDetail').datagrid('getSelected');
            var index = $('#gridDetail').datagrid('getRowIndex', selected);
            var countIndex = $('#gridDetail').datagrid('getRows');
            $('#gridDetail').datagrid('clearSelections');

            if (index > 0) {
                onClickCell(index - 1, field);
            } else {
                onClickCell(countIndex.length - 1, field);
            }
        }
    });

}

//去下一个可以编辑框
function nextControl() {
    var selected = $('#gridDetail').datagrid('getSelected');
    var index = $('#gridDetail').datagrid('getRowIndex', selected);
    var currentField = "ShelfCode";

    //编辑字段
    var fields = "ShelfCode".split(',');
    for (var i = 0; i < fields.length; i++) {
        var ciField = fields[i];
        if (ciField == editfield) {
            currentField = fields[i + 1];
            if (currentField == undefined) {
                currentField = "ShelfCode";
                index = index + 1;
                $('#gridDetail').datagrid('clearSelections');
                $('#gridDetail').datagrid('getRowIndex', index);
            }
        }
    }

    //Remark回车到下一行
    if (editfield == "Remark") {
        currentField = "ShelfCode";
        index = index + 1;
    }

    var len = $("#gridDetail").datagrid("getRows").length;
    if (len == index) {
        $('#gridDetail').datagrid('endEdit', index - 1);
        $('#gridDetail').datagrid('selectRow', index - 1);
    } else {
        onClickCell(index, currentField);
    }
}
//添加
function addGridRow() {
    var thisdlg = frxs.dialog({
        title: "选择商品资料",
        url: "../WProductAdjShelf/WProductAdjShelfProduct",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title="【Alt+S】">提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
               
            }
        }, {
            text: '<div title="【ESC】">关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

//删除
function delGridRow() {

    var is = "";
    $(".datagrid-body:first table tr").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            is = i + "," + is;
        }
    });
    if (is == "") {
        $.messager.alert('提示', "请选择一条记录！", 'info');
        return false;
    }

    var rows = $('#gridDetail').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < is.split(',').length; j++) {
        var ci = is.split(',')[j];
        if (ci) {
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#gridDetail').datagrid('getRowIndex', copyRows[i]);
        $('#gridDetail').datagrid('deleteRow', ind);
    }

    setAdjShelfProduct();
    $('#gridDetail').datagrid('clearSelections');


}

function setAdjShelfProduct() {
    var value = "";
    var rows = $('#gridDetail').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        value += row["ProductId"] + ",";
    }
    if (value != "") {
        value = value.substr(0, value.length - 1);
    }
    $("#Products").val(value);

}

function reloadProducts(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i]
        if (!rowProductExist(row["ProductId"])) {
            row["WID"] = i; //默认一值
            row["WProductID"] = row["WProductId"];
            row["ProductId"] = row["ProductId"];
            row["SKU"] = row["SKU"];
            row["BarCode"] = row["BarCode"];
            row["Unit"] = row["Unit"];
            row["BigPackingQty"] = row["BigPackingQty"];
            row["OldShelfID"] = row["ShelfID"];
            row["OldShelfCode"] = row["ShelfCode"];
            row["ShelfID"] = "";
            row["ShelfCode"] = "";
            row["Remark"] = "";
            $('#gridDetail').datagrid('appendRow', row);
        }
    }
    setAdjShelfProduct();
}

function rowProductExist(id) {
    var rows = $('#gridDetail').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["ProductId"] == id) {
            return true;
        }
    }
    return false;
}

//清空查询表单
function reset() {

}

//保存
function saveData() {

    var validate = $("#formAdd").form('validate');

    var selected = $('#gridDetail').datagrid('getSelected');
    var index = $('#gridDetail').datagrid('getRowIndex', selected);
    $('#gridDetail').datagrid('endEdit', index);

    if (validate == false) {
        return false;
    } else {
        var data = $("#formAdd").serialize();

        var jsonOrder = "{";
        if ($("#WID").val() == '') {
            $.messager.alert('提示', "仓库不能为空！", 'info');
            return false;
        }
        jsonOrder += "\"WID\":'" + $("#WID").val() + "',";
        jsonOrder += "\"AdjID\":'" + $("#AdjID").val() + "',";
        jsonOrder += "\"Remark\":'" + $("#Remark").val() + "'";
        jsonOrder += "}";

        var rows = $('#gridDetail').datagrid('getRows');
        var jsonStr = "[";
        for (var i = 0; i < rows.length; i++) {
            jsonStr += "{";
            jsonStr += "\"WID\":\"" + rows[i].WID + "\",";
            jsonStr += "\"WProductID\":\"" + rows[i].WProductID + "\",";
            jsonStr += "\"ProductId\":\"" + rows[i].ProductId + "\",";
            jsonStr += "\"SKU\":\"" + rows[i].SKU + "\",";
            jsonStr += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonStr += "\"BarCode\":\"" + rows[i].BarCode + "\",";
            jsonStr += "\"Unit\":\"" + rows[i].Unit + "\",";
            jsonStr += "\"BigPackingQty\":\"" + rows[i].BigPackingQty + "\",";
            jsonStr += "\"OldShelfID\":\"" + rows[i].OldShelfID + "\",";
            jsonStr += "\"OldShelfCode\":\"" + rows[i].OldShelfCode + "\",";
            jsonStr += "\"ShelfID\":\"" + rows[i].ShelfID + "\",";

            if (rows[i].ShelfCode == '') {
                $.messager.alert('提示', "调整货位不能为空！", 'info');
                return false;
            }
            jsonStr += "\"ShelfCode\":\"" + rows[i].ShelfCode + "\",";
            jsonStr += "\"Remark\":\"" + rows[i].Remark + "\"";
            jsonStr += "},";

        }
        if (jsonStr.length > 1) {
            jsonStr = jsonStr.substring(0, jsonStr.length - 1);
        }
        jsonStr += "]";
        var loading = frxs.loading("正在处理中，请稍后...");
        $.ajax({
            url: "../WProductAdjShelf/WProductAdjShelfHandle",
            type: "post",
            dataType: 'json',
            data: {
                jsonData: jsonOrder, jsonDetails: jsonStr
            },
            success: function (obj) {
                loading.close();
                if (obj.Flag != "SUCCESS") {
                    $.messager.alert('提示', obj.Info, 'info');

                } else {

                    if ($("#AdjID").val() != "") {
                        $.messager.alert("提示", "编辑成功", "info");

                    } else {

                        var adjId = obj.Data.AdjId;
                        $("#AdjID").val(adjId);
                        $("#CreateUserName").val(obj.Data.UserName);
                        $("#CreateTime").val(obj.Data.Time);
                        $.messager.alert("提示", "添加成功", "info");

                    }
                    SetPermission("save");

                }
            }, error: function (e) {
                loading.close();
                $.messager.alert('提示', "操作失败！", 'info');
            }
        });

    }
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 150);
    $('#gridDetail').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}

//下拉控件数据初始化
function initDDL() {
 

}

//选择商品资料
function showDialog(parm) {

    var thisdlg = frxs.dialog({
        title: "选择商品资料",
        url: "/ProductLimit/ProductLimitProduct",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title="【Alt+S】">提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
            }
        }, {
            text: '<div title="【ESC】">关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}


//订单 编辑
function edit() {
    SetPermission("edit");
    frxs.updateTabTitle("编辑货位调整" + $("#AdjID").val(), "icon-edit");
}

//订单状态 确认
function sure() {

    var adjID = $("#AdjID").val();
    var status = $("#Status").val();

    if (status != "0") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return false;
    }

    if (adjID != "") {
        $.ajax({
            url: "../WProductAdjShelf/WProductAdjShelfChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                ids: adjID,
                status: 1
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "确认成功！", "info");

                        $("#Status").val("1");
                        $("#StatusToStr").val("确认");
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

    var adjID = $("#AdjID").val();
    var status = $("#Status").val();
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
        return false;
    }

    if (adjID != "") {
        $.ajax({
            url: "../WProductAdjShelf/WProductAdjShelfChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                ids: adjID,
                status: 0
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "反确认成功！", "info");
                        $("#Status").val("0");
                        $("#StatusToStr").val("录单");
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

    var adjID = $("#AdjID").val();
    var status = $("#Status").val();
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return false;
    }

    if (adjID != "") {
        
        $.messager.confirm("提示", "确定过账单据?", function (r) {
            $.ajax({
                url: "../WProductAdjShelf/WProductAdjShelfChangeStatus",
                type: "get",
                dataType: "json",
                data: {
                    ids: adjID,
                    status: 2
                },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {

                            $.messager.alert("提示", "过账成功！", "info");
                            $("#Status").val("2");
                            $("#StatusToStr").val("过账");
                            $("#PostingUserName").val(result.Data.UserName);
                            $("#PostingTime").val(result.Data.Time);
                            SetPermission("posting");
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
        });
    }
}


//数据回填-导入
function backFillShelfImport(products) {
    
    var rows = $("#gridDetail").datagrid("getRows");
    var msg = "";
    if (rows.length > 0) {
        for (var i = 0; i < products.length; i++) {
            var product = products[i];
            for (var j = 0; j < rows.length; j++) {
                if (rows[j].SKU == product.SKU) {
                    msg += product.SKU + " | ";
                }
            }
        }
    }
    if (msg) {
        window.top.$.messager.alert("提示", "存在重复数据 “商品SKU：" + msg + "”。", "info");
        return false;
    } else {
        //删除最后空行数据
        var ciLen = $('#gridDetail').datagrid('getRows').length - 1;
        var lastRow = $('#gridDetail').datagrid('getRows')[ciLen];
        if (lastRow && !lastRow.SKU) {
            $('#gridDetail').datagrid('deleteRow', ciLen);
        }

        for (var i = 0; i < products.length; i++) {
            var product = products[i];

            var row = {};
            row.WID = "";
            row.WProductID = product.WProductID;
            row.ProductId = product.ProductId;
            row.ProductName = product.ProductName;
            row.SKU = product.SKU;
            row.Unit = product.Unit;
            row.BigPackingQty = product.BigPackingQty;
            row.OldShelfID = product.OldShelfID;
            row.OldShelfCode = product.OldShelfCode;
            row.ShelfID = product.ShelfID;
            row.ShelfCode = product.ShelfCode;
            row.Remark = product.Remark;

            $('#gridDetail').datagrid('appendRow', row);
        }
     
        return true;
    }
}


function backFillInfoImport(adjIdObj) {

    $("#AdjID").val(adjIdObj);

    init("2");//导入

    return true;
}

//订单 添加
function add() {
    location.href = '../WProductAdjShelf/WProductAdjShelfAddOrEditView';
    frxs.updateTabTitle("添加货位调整", "icon-add");
}

//订单 导出
function exportData() {
    var adjID = $("#AdjID").val();
    if (adjID != "") {
        location.href = "../WProductAdjShelf/DataExport?adjID=" + adjID;
    } else {
        $.messager.alert("提示", "货位调整单号为空！", "info");
    }
}

//打印商品货位调整单
function print() {
    var adjid = $("#AdjID").val();
    if (adjid != "") {
        var thisdlg = frxs.dialog({
            title: "打印商品货位调整单",
            url: "../FastReportTemplets/Aspx/PrintWProductAdjShelf.aspx?AdjID=" + adjid,
            owdoc: window.top,
            width: 765,
            height: 600
        });
    } else {
        $.messager.alert("提示", "单据号不能为空！", "info");
    }
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
