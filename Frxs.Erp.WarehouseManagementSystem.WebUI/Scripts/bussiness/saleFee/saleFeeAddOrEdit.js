
var feeId = "";
var initSubWID = 0;
var isNewRow = false;
$(function () {

    init("1");

});

//加载数据
function init(initType) {

    if (initType == "1") {
        feeId = frxs.getUrlParam("ID");
        type = frxs.getUrlParam("Type");
    } else {

        feeId = $("#FeeID").val();
        type = "edit";
    }

    if (feeId) {

        $.ajax({
            url: "../SaleFee/SaleFeeAddOrEdit",
            type: "post",
            data: { id: feeId },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);

                initSubWID = obj.SubWID;

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
                else {
                    SetPermission('posting');
                }

            }
        });
    } else {
        isNewRow = true;
        //添加
        $("#FeeName").combobox('setValue', '');
        //加载数据
        loadgrid();

        //高度自适应
        gridresize();
        //下拉绑定
        initDDL();

        SetPermission('add');

    }
}

//实现对DataGird控件的绑定操作
function loadgrid() {
    $('#gridDetail').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式 
        url: '../SaleFee/GetSaleFeeDetailList',          //Aajx地址
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
        showFooter: true,
        onClickRow: function (rowIndex) {
            $('#gridDetail').datagrid('clearSelections');
            $('#gridDetail').datagrid('selectRow', rowIndex);
        },
        onClickCell: onClickCell,
        queryParams: {
            FeeID: feeId
        },
        onLoadSuccess: function () {
            addGridRow();

            if (feeId) {
                totalCalculate();
            }
        },
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }
        ]],
        columns: [[
            { title: 'WID', field: 'WID', hidden: true },
            { title: 'ShopID', field: 'ShopID', hidden: true },
            { title: frxs.setTitleColor('门店编号'), field: 'ShopCode', width: 160, editor: 'text', align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 200 },
            {
                title: frxs.setTitleColor('项目名称'), field: 'FeeName', width: 80, editor: {
                    type: 'combobox', id: "test",
                    options: {
                        id: "test",
                        required: true,
                        missingMessage: '选择费用项目',
                        url: '../Common/GetSaleFee',
                        valueField: 'DictValue',
                        textField: 'DictLabel',
                        editable: false,
                        panelHeight: 'auto',
                        onChange: function (value) {
                            if (!isNaN(value)) {

                                var row = $("#gridDetail").datagrid("getSelected");
                                var index = $("#gridDetail").datagrid("getRowIndex", row);

                                row.FeeName = $(this).combobox("getText");
                                row.FeeCode = value;
                                $(".datagrid-row .combo").prev().combobox("hidePanel");
                                
                                //更新行
                                $('#gridDetail').datagrid('updateRow', {
                                    index: index,
                                    row: row
                                });

                                nextControl();
                            }
                        }
                    }
                }
            },
            { title: 'FeeCode', field: 'FeeCode', hidden: true },
            { title: 'SettleID', field: 'SettleID', hidden: true },
            { title: 'SettleTime', field: 'SettleTime', hidden: true },
            { title: 'SerialNumber', field: 'SerialNumber', hidden: true },
            { title: frxs.setTitleColor('订单编号'), field: 'OrderId', width: 200, editor: 'text', align: 'center' },
            {
                title: frxs.setTitleColor('金额(元)'), field: 'FeeAmt', width: 180, align: 'right', editor: {
                    type: 'numberbox',
                    options: {
                        precision: 4
                    }
                }
            },
            { title: frxs.setTitleColor('备注'), field: 'Reason', width: 260, editor: 'text', formatter: frxs.formatText }
        ]],
        toolbar: toolbarArray
    });
};

//显示导入界面
function showImport() {
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }

    var thisdlg = frxs.dialog({
        title: "门店费用-Excel数据导入",
        url: "../SaleFee/ImportSaleFeeView",
        owdoc: window.top,
        width: 820,
        height: 650,
        buttons: [{
            text: '导入',
            id: 'import1',
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

//查询
function search() {

    if ($("#searchPlan").is(":hidden")) {
        $('#gridDetail').datagrid('resize', {
            height: ($(window).height() - $("#searchPlan").height() - 19)
        });
        $("#searchPlan").show();
    } else {
        $('#gridDetail').datagrid('resize', {
            height: ($(window).height() - 2)
        });
        $("#searchPlan").hide();
    }
}

//添加
function addGridRow() {
    if (isNewRow) {

        var ciLen = $('#gridDetail').datagrid('getRows').length - 1;
        var lastRow = $('#gridDetail').datagrid('getRows')[ciLen];

        if (lastRow && lastRow.ShopCode == "") {
            $('#gridDetail').datagrid('clearSelections');
            onClickCell($('#gridDetail').datagrid('getRows').length - 1, "ShopCode");
            return false;
        } else {

            var index = $('#gridDetail').datagrid('getRows').length;
            $("#gridDetail").datagrid("insertRow", {
                index: index + 1,
                row: { WID: "", ShopID: "", ShopCode: "", ShopName: "", FeeName: "", FeeCode: "", SettleID: "", SettleTime: "", SerialNumber: "0.00", OrderId: "", FeeAmt: "", Reason: "" }
            });
            return true;
        }

    }
}

//删除
function del() {
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
    totalCalculate();
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

        if ($("#FeeDate").val() == '') {
            $.messager.alert('提示', "费用日期为必选项！", 'info');
            return false;
        }

        if ($("#SubWID").combobox("getValue") == "") {
            $.messager.alert("提示", "请选择仓库！", "info");
            return false;
        }

        jsonOrder += "\"FeeDate\":'" + $("#FeeDate").val() + "',";

        if ($("#SubWID").combobox('getValue') == '') {

            $.messager.alert('提示', "仓库不能为空！", 'info');
            return false;
        }
        jsonOrder += "\"SubWID\":'" + $("#SubWID").combobox('getValue') + "',";
        jsonOrder += "\"Status\":'" + $("#Status").val() + "',";
        jsonOrder += "\"FeeID\":'" + $("#FeeID").val() + "',";
        jsonOrder += "\"ID\":'" + $("#ID").val() + "',";
        jsonOrder += "\"Remark\":'" + $("#Remark").val() + "'";
        jsonOrder += "}";

        var rows = $('#gridDetail').datagrid('getRows');

        if (rows.length <= 0) {
            $.messager.alert('提示', "明细不能为空！", 'info');
            return false;
        }

        var jsonStr = "[";
        for (var i = 0; i < rows.length; i++) {
            jsonStr += "{";
            jsonStr += "\"WID\":\"" + rows[i].WID + "\",";
            jsonStr += "\"ShopID\":\"" + rows[i].ShopID + "\",";

            if (rows[i].ShopID == '') {
                $.messager.alert('提示', "门店名称未获取！", 'info');
                return false;
            }
            jsonStr += "\"ShopCode\":\"" + rows[i].ShopCode + "\",";
            jsonStr += "\"ShopName\":\"" + rows[i].ShopName + "\",";

            if (rows[i].FeeCode == '') {
                $.messager.alert('提示', "项目名称不能为空！", 'info');
                return false;
            }
            jsonStr += "\"FeeCode\":\"" + rows[i].FeeCode + "\",";
            jsonStr += "\"FeeName\":\"" + rows[i].FeeName + "\",";
            jsonStr += "\"SettleID\":\"" + rows[i].SettleID + "\",";
            jsonStr += "\"SettleTime\":\"" + rows[i].SettleTime + "\",";
            jsonStr += "\"SerialNumber\":\"" + rows[i].SerialNumber + "\",";
            jsonStr += "\"OrderId\":\"" + rows[i].OrderId + "\",";

            if (rows[i].FeeAmt == '') {
                $.messager.alert('提示', "明细金额不能为空！", 'info');
                return false;
            }

            if (rows[i].FeeAmt == 0) {
                $.messager.alert('提示', "单条明细金额不能为0！", 'info');
                return false;
            }

            if (rows[i].FeeAmt > 100000000) {
                $.messager.alert('提示', "明细金额不能大于100000000！", 'info');
                return false;
            }

            jsonStr += "\"FeeAmt\":\"" + rows[i].FeeAmt + "\",";
            jsonStr += "\"Reason\":\"" + rows[i].Reason + "\"";
            jsonStr += "},";
        }
        if (jsonStr.length > 1) {
            jsonStr = jsonStr.substring(0, jsonStr.length - 1);
        }
        jsonStr += "]";
        var loading = frxs.loading("正在处理中，请稍后...");
        $.ajax({
            url: "../SaleFee/SaleFeeHandle",
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
                    if ($("#FeeID").val() != "") {
                        $.messager.alert("提示", "编辑成功", "info");
                    } else {

                        var feeId = obj.Data.FeeID;
                        $("#FeeID").val(feeId);
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
    var h = ($(window).height() - $("fieldset").height() - 195);
    $('#gridDetail').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}

//下拉控件数据初始化
function initDDL() {
    var tempSubWID = "";
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);

            if (data.length > 1) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].WID == initSubWID) {
                        tempSubWID = initSubWID;
                    }
                }
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName",      //value列
                onSelect: function (rec) {

                }
            });

            if (tempSubWID == "") {
                $("#SubWID").combobox('select', data[0].WID);
            } else {
                $("#SubWID").combobox('select', tempSubWID);
            }
        }
    });
}

//编辑单元格事件
$.extend($.fn.datagrid.methods, {
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
            if (ed && ed.type && ed.type == "combobox") {
                //绑定单位字段
                var FeeCode = $("#gridDetail").datagrid("getSelected").FeeCode;
                if (FeeCode > 0) {
                    var url = '../Common/GetSaleFee';
                    ed.target.combobox('reload', url); //联动下拉列表重载  
                }
            }
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
                    if (ed.field == "FeeAmt") {
                        obj.attr("maxlength", "10");
                        obj.keyup(function () {
                            $(this).val($(this).val().replace(/[^\-?\d.]/g, ''));
                        }).bind("paste", function () {  //CTR+V事件处理    
                            $(this).val($(this).val().replace(/[^\-?\d.]/g, ''));
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
    if (field == "FeeAmt") {
        obj.bind("change", function () {
            changeAppQty($(this).val());
        });
    }
    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "ShopCode") {
                if ($(this).val() == "") {
                    showDialog("");
                    $(this).blur();
                } else if ($(this).val() != valueinput) {
                    showDialog($(this).val());
                    $(this).blur();
                } else {
                    nextControl();
                }
            } else {
                if (field == "FeeAmt") {
                    obj.change();
                }
                nextControl();
            }
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
    //为门店绑定移开变回事件
    if ($(obj).parents('td:eq(3)').attr("field") == "ShopCode") {
        obj.bind("blur", function () {
            $(this).val(valueinput);
        });
    }
}

//去下一个可以编辑框
function nextControl() {
    var selected = $('#gridDetail').datagrid('getSelected');
    var index = $('#gridDetail').datagrid('getRowIndex', selected);
    var currentField = "ShopCode";

    //编辑字段
    var fields = "ShopCode,FeeName,FeeAmt".split(',');
    for (var i = 0; i < fields.length; i++) {
        var ciField = fields[i];
        if (ciField == editfield) {
            currentField = fields[i + 1];
            if (currentField == undefined) {
                currentField = "ShopCode";
                index = index + 1;
                $('#gridDetail').datagrid('clearSelections');
                $('#gridDetail').datagrid('getRowIndex', index);
            }
        }
    }

    //Remark回车到下一行
    if (editfield == "Reason") {
        currentField = "ShopCode";
        index = index + 1;
    }

    var len = $("#gridDetail").datagrid("getRows").length;
    if (len == index) {
        //插入行
        if (addGridRow()) {
            onClickCell(index, currentField);
        }
    } else {
        onClickCell(index, currentField);
    }
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
            if (field == "FeeName") {
                $(".datagrid-row .combo").prev().combobox("showPanel");
            }

            editIndex = index;
        }
    }
}

//选择门店资料
function showDialog(parm) {

    $.ajax({
        url: "../SaleFee/GetShopModelPageData",
        type: "get",
        dataType: "json",
        data: {
            ShopCode: parm
        },
        success: function (obj) {
            if (obj && obj.total == 1) {
                backFillShopInfo(obj.rows[0]);
            } else {
                frxs.dialog({
                    title: "选择门店资料",
                    url: "../SaleFee/SearchShopInfoView?ShopCode=" + parm,
                    owdoc: window.top,
                    width: 850,
                    height: 500
                });
            }
        }
    });
}

//回填信息
function backFillShopInfo(product) {

    //判断是否重复
    var rep = false;
    var rows = $("#gridDetail").datagrid("getRows");
    var index = $('#gridDetail').datagrid('getRowIndex', $('#gridDetail').datagrid('getSelected'));

    //如果选中的是相同的数据 过滤
    if ($('#gridDetail').datagrid('getSelected').ShopCode == product.ShopCode) {
        return;
    }

    for (var i = 0; i < rows.length; i++) {

        if (rows[i].ShopCode == product.ShopCode && index != i) {
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
                    backFillShopInfo2(product);
                    msgdlg.dialog("close");
                }
            }]
        });

    } else {
        backFillShopInfo2(product);
    }
}

//数据回填-导入
function backFillShopImport(products) {
    var rows = $("#gridDetail").datagrid("getRows");
    var msg = "";
    for (var i = 0; i < products.length; i++) {
        var product = products[i];
        for (var j = 0; j < rows.length; j++) {
            if (rows[j].ShopCode == product.ShopCode) {
                msg += product.ShopCode + " | ";
            }
        }
    }
    if (msg) {
        window.top.$.messager.alert("提示", "存在重复数据 “门店编号：" + msg + "”。", "info");
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
            row.ShopID = product.ShopID;
            row.ShopCode = product.ShopCode;
            row.ShopName = product.ShopName;
            row.FeeName = product.FeeName;
            row.FeeCode = product.FeeCode;
            row.SettleID = product.SettleID;
            row.SettleTime = product.SettleTime;
            row.SerialNumber = product.SerialNumber;
            row.OrderId = "";
            row.FeeAmt = parseFloat(product.FeeAmt).toFixed(4);
            row.Reason = product.Reason;

            $('#gridDetail').datagrid('appendRow', row);
        }
        totalCalculate();
        return true;
    }
}



//数据回填
function backFillShopInfo2(product) {
    
    //特殊需求 自动选择上一条的项目名称第一条特殊处理
    var isFrist = $('#gridDetail').datagrid('getRows').length == 1;


    var row = $('#gridDetail').datagrid('getSelected');
    var index = $('#gridDetail').datagrid('getRowIndex', row);

    $('#gridDetail').datagrid('endEdit', index);

    row.WID = product.WID;
    row.ShopID = product.ShopID;
    row.ShopCode = product.ShopCode;
    row.ShopName = product.ShopName;
    
    //如果不是第一行
    if (!isFrist) {
        var lastRow = $('#gridDetail').datagrid('getRows')[$('#gridDetail').datagrid('getRows').length - 2];
        row.FeeName = lastRow.FeeName;
        row.FeeCode = lastRow.FeeCode;
    }

    //更新行
    $('#gridDetail').datagrid('updateRow', {
        index: index,
        row: row
    });

    if (isFrist) {
        onClickCell(index, "FeeName");
    } else {
        onClickCell(index, "FeeAmt");
    }
}

//订单 编辑
function edit() {
    SetPermission("edit");

    frxs.updateTabTitle("编辑费用明细" + $("#FeeID").val(), "icon-edit");
}

//订单状态 确认
function sure() {

    var feeID = $("#FeeID").val();
    var status = $("#Status").val();
    if (status != "0") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return false;
    }

    if (feeID != "") {
        $.ajax({
            url: "../SaleFee/SaleFeeChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                ids: feeID,
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

//订单状态 反确认
function noSure() {

    var feeID = $("#FeeID").val();
    var status = $("#Status").val();
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
        return false;
    }
    if (feeID != "") {
        $.ajax({
            url: "../SaleFee/SaleFeeChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                ids: feeID,
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
                        $.messager.alert("提示", "反确认失败！", "info");
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

    var feeID = $("#FeeID").val();
    var status = $("#Status").val();
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return false;
    }
    if (feeID != "") {
        $.messager.confirm("提示", "确定过账门店费用?", function (r) {
            $.ajax({
                url: "../SaleFee/SaleFeeChangeStatus",
                type: "get",
                dataType: "json",
                data: {
                    ids: feeID,
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
        });
    }
}

function backFillInfoImport(adjIdObj) {

    $("#FeeID").val(adjIdObj);

    init("2");//导入

    return true;
}

//订单 添加
function add() {
    location.href = '../SaleFee/SaleFeeAddOrEditView';
    frxs.updateTabTitle("添加门店费用", "icon-add");
}

//订单 导出
function exportData() {
    var feeID = $("#FeeID").val();
    if (feeID != "") {
        //site_common.ShowProgressBar();
        location.href = "../SaleFee/DataExport?feeID=" + feeID;
        //site_common.HideProgressBar();
    } else {
        $.messager.alert("提示", "门店费用单号为空！", "info");
    }
}


//打印门店费用单
function print() {
    var feeId = $("#FeeID").val();
    if (feeId != "") {
        var thisdlg = frxs.dialog({
            title: "打印门店费用单",
            url: "../FastReportTemplets/Aspx/PrintSaleFee.aspx?FeeID=" + feeId,
            owdoc: window.top,
            width: 765,
            height: 600
        });
    } else {
        $.messager.alert("提示", "单据号不能为空！", "info");
    }
}


//验证 并且更新grid的行数据 
function changeAppQty(value) {
    if (isNaN(value) || value == "") {
        value = 0;
    }
    //if (value < 0) {
    //    value = 0;  //取编辑的值
    //}

    var row = $('#gridDetail').datagrid('getSelected');
    var index = $("#gridDetail").datagrid("getRowIndex", row);

    row.FeeAmt = parseFloat(value).toFixed(4);

    //更新行
    $('#gridDetail').datagrid('updateRow', {
        index: index,
        row: row
    });
    totalCalculate();
}

//计算总数量
function totalCalculate() {

    var rows = $("#gridDetail").datagrid("getRows");
    var totalFeeAmt = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var feeAmt = parseFloat(rows[i].FeeAmt);
        totalFeeAmt += feeAmt;
    }

    $('#gridDetail').datagrid('reloadFooter', [
       { "FeeAmt": "金额总计：" + parseFloat(totalFeeAmt).toFixed(4) }
    ]);
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
