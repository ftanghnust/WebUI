var dialogWidth = 850;
var dialogHeight = 600;
var view = "";
var nosaleId = "";

$(function () {
    nosaleId = frxs.getUrlParam("Id");
    //下拉绑定
    //initDDL();
    init();
    gridresize();
});

//下拉控件数据初始化
function initDDL(wid) {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            //data.unshift({ "WID": "", "WName": "-请选择-" });
            //创建控件
            $("#WID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#WID").combobox('select', data[0].WID);
            if (wid != null && wid != 0) {
                $("#WID").combobox('select', wid);
            }
        }, error: function (e) {

        }
    });
}

//加载数据
function init() {
    var loading = frxs.loading("正在加载中，请稍后...");
    view = frxs.getUrlParam("view");
    //alert(nosaleId);
    var id = "";
    if (nosaleId != null) {
        id = nosaleId;
    }
    $.ajax({
        url: "../ScorePromotion/GetScorePromotionCopy",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            obj.BeginTime = frxs.dateFormat(obj.BeginTime);
            obj.EndTime = frxs.dateFormat(obj.EndTime);
            $('#formAdd').form('load', obj);
            //initDDL(obj.WID);
            var comboboxData = [{ "WID": obj.WID, "WName": obj.WName }];
            $("#WID").combobox({
                data: comboboxData,
                valueField: "WID",
                textField: "WName"
            });
            $("#WID").combobox('select', comboboxData[0].WID);
            initProductGrid(obj.DetailsList);
            initGroupGrid(obj.ShopList);
            if (nosaleId) {
                $("#PromotionID").val("");
                $("#Status").combobox('select', '0');//复制单重置为录单状态
                SetPermission('add');
            }
            else {
                SetPermission('add');
            }
            //if (obj.Status == 0) {//录单
            //    if (nosaleId) {
            //        SetPermission('ludan');
            //    }
            //    else {
            //        SetPermission('add');
            //    }
            //}
            //else if (obj.Status == 1) {//确认
            //    SetPermission('sure');
            //}
            //else if (obj.Status == 2) {//已生效
            //    SetPermission('posting');
            //}
            //else if (obj.Status == 3) {//已停用
            //    SetPermission('stop');
            //}
            loading.close();
        }
    });
}

var editIndex = -1;

function initProductGrid(griddata) {
    $('#gridProduct').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        data: griddata,
        //url: ajaxUrl,          //Aajx地址
        sortName: 'ProductID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductID',                  //主键
        pageSize: 100,                       //每页条数
        pageList: [100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: false,
        selectOnCheck: true,
        onClickRow: function (rowIndex, rowData) {
            $('#gridProduct').datagrid('unselectAll');
            $('#gridProduct').datagrid('selectRow', rowIndex);
            onClickRow(rowIndex);
        },
        //onClickCell: function (rowIndex, rowData) {
        //    $('#gridProduct').datagrid('beginEdit', rowIndex);
        //},
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'ProductID', field: 'ProductID', width: 100, align: 'center', sortable: true, hidden: true },
            { title: 'WProductID', field: 'WProductID', width: 100, hidden: true },
            { title: '商品编码', field: 'SKU', width: 150 },
            { title: '商品名称', field: 'ProductName', width: 200 },
            { title: '库存单位', field: 'Unit', width: 100 },

            //{ title: '包装数', field: 'PackingQty', width: 100 },//PackingQty
            //{ title: '配送单位', field: 'SaleUnit', width: 100 },//SaleUnit
            //{ title: '配送价格', field: 'SalePrice', width: 100 },
            //{ title: '国际条码', field: 'BarCode', width: 150, hidden: true },
            //{ title: '原门店积分', field: 'OldPoint', width: 100 }
            //,{
            //    title: '小单位积分', field: 'Point', width: 100
            //    , editor: {
            //        type: 'numberbox',
            //        options: {
            //            precision: 2,
            //            required: true
            //        }
            //    }
            //}
            //, { title: '积分', field: 'PointPrice', width: 100 }
            //, {
            //    title: '限购小单位数量', field: 'MaxOrderQty', width: 100
            //    , editor: {
            //        type: 'numberbox',
            //        options: {
            //            precision: 2,
            //            required: true
            //        }
            //    }
            //}

            { title: '原库存单位积分', field: 'OldPoint', width: 100 },
            {
                title: frxs.setTitleColor('库存单位积分'), field: 'Point', width: 100
                , editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2,
                        required: true
                    }
                }
            },
            //{
            //    title: frxs.setTitleColor('限购库存单位数量'), field: 'MaxOrderQty', width: 100
            //    , editor: {
            //        type: 'numberbox',
            //        options: {
            //            precision: 2,
            //            required: true
            //        }
            //    }
            //},
            {
                title: frxs.setTitleColor('配送单位限购数量'), field: 'MaxOrderQty', width: 100
                , editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2,
                        required: true
                    }
                }
            },
            { title: '积分', field: 'PointPrice', width: 100, align: 'right' },
            { title: '配送单位', field: 'SaleUnit', width: 100 },//SaleUnit
            { title: '配送价格', field: 'SalePrice', width: 100, align: 'right' },
            { title: '包装数', field: 'PackingQty', width: 100, align: 'right' },//PackingQty
            { title: '国际条码', field: 'BarCode', width: 150, hidden: true }
        ]],
        toolbar: [
        //{
        //    id: 'btnReload',
        //    text: '刷新',
        //    iconCls: 'icon-reload',
        //    handler: function () {
        //        //实现刷新栏目中的数据
        //        $("#grid").datagrid("reload");
        //    }
        //}, '-',
        {
            id: 'btnAdd',
            text: '新增',
            iconCls: 'icon-add',
            handler: function () {
                addProduct()
            }
        },
        {
            id: 'btnDel',
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                delCheckProduct()
            }
        }
        ],
        onLoadSuccess: function (data) {
            //setScorePromotionProduct();
            beginEditProduct();
            scoreProduct();
        }
    });
}

function onClickRow(index) {
    if (editIndex != index && editIndex != -1) {
        $('#gridProduct').datagrid('endEdit', editIndex);
    }
    $('#gridProduct').datagrid('selectRow', index).datagrid('beginEdit', index);
    var rows = $('#gridProduct').datagrid("getRows");
    var row = rows[index];
    var ed = $('#gridProduct').datagrid('getEditor', { index: index, field: 'Point' });
    $(ed.target).numberbox({
        onChange: function () {
            var point = $(ed.target).numberbox('getValue');
            //var salePrice = row.SalePrice;
            var packingQty = row.PackingQty;
            row.PointPrice = packingQty * point;//积分=小单位积分*配送单位包装数
            $('#gridProduct').datagrid('selectRow', index).datagrid('endEdit', index);
        }
    });
    editIndex = index;
}

function onEndEdit(index) {
    if (index == -1) {
        return;
    }
    $('#gridProduct').datagrid('beginEdit', index);
    var rows = $('#gridProduct').datagrid("getRows");
    var row = rows[index];
    var point = row.Point;
    var packingQty = row.PackingQty;
    row.PointPrice = packingQty * point;//积分=小单位积分*配送单位包装数
    $('#gridProduct').datagrid('endEdit', index);
    editIndex = index;
}

function scoreProduct() {
    var rows = $('#gridProduct').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        $('#gridProduct').datagrid('beginEdit', i);
        var row = rows[i];
        var point = row.Point;
        var packingQty = row.PackingQty;
        row.PointPrice = packingQty * point;//积分=小单位积分*配送单位包装数
        $('#gridProduct').datagrid('endEdit', i);
    }
}

function beginEditProduct() {
    //var rows = $('#gridProduct').datagrid("getRows");
    //for (var i = 0, l = rows.length; i < l; i++) {
    //    var row = rows[i];
    //}
}

function endEditProduct() {
    //var rows = $('#gridProduct').datagrid("getRows");
    //for (var i = 0, l = rows.length; i < l; i++) {
    //    var row = rows[i];
    //    $('#gridProduct').datagrid('endEdit', i);
    //}
}

function addProduct() {
    var thisdlg = frxs.dialog({
        title: "增加商品到积分促销",
        url: "../ScorePromotion/ScorePromotionProduct",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                //$("#grid").datagrid("reload");
                //thisdlg.dialog("close");
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                //$("#grid").datagrid("reload");
                thisdlg.dialog("close");
            }
        }]
    });
}

function delCheckProduct() {
    var rows = $('#gridProduct').datagrid("getSelections");
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridProduct').datagrid('getRowIndex', copyRows[i]);
        $('#gridProduct').datagrid('deleteRow', index);
    }
    setScorePromotionProduct();
    $('#gridGroup').datagrid('clearSelections');
}

function setScorePromotionProduct() {
    var value = "";
    var rows = $('#gridProduct').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        //alert(row["ProductID"]);
        value += row["ProductID"] + ",";
    }
    if (value != "") {
        value = value.substr(0, value.length - 1);
    }
    $("#Products").val(value);
    //alert($("#Products").val());
}

function reloadProducts(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (!rowProductExist(row["ProductId"])) {
            row["ProductID"] = row["ProductId"];
            row["WProductID"] = row["WProductId"];
            row["PackingQty"] = row["BigPackingQty"];
            row["SaleUnit"] = row["BigUnit"];
            row["SalePrice"] = row["UnitSalePrice"];
            row["OldPoint"] = row["ShopPoint"];
            row["Point"] = 0;
            row["PointPrice"] = 0;
            row["MaxOrderQty"] = 0;
            $('#gridProduct').datagrid('appendRow', row);
        }
    }
    setScorePromotionProduct();
    beginEditProduct();
}

function rowProductExist(id) {
    var rows = $('#gridProduct').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["ProductID"] == id) {
            return true;
        }
    }
    return false;
}

function initGroupGrid(griddata) {
    $('#gridGroup').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        data: griddata,
        //url: '../Shop/GetShopList',          //Aajx地址
        sortName: 'ShopID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopID',                  //主键
        pageSize: 20,                       //每页条数
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
            
        },
        queryParams: {
            
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'ShopID', field: 'ShopID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: 'WID', field: 'WID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: '门店编号', field: 'ShopCode', width: 130, align: 'center', sortable: true },
            { title: '门店名称', field: 'ShopName', width: 260, align: 'left', formatter: frxs.replaceCode },
            {
                title: '门店类型', field: 'ShopType', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "加盟店";
                    }
                    else if (value == "1") {
                        return "签约店";
                    }
                    else {
                        return value;
                    }
                }
            },
            { title: '门店地址', field: 'FullAddress', width: 360, align: 'left', formatter: frxs.replaceCode },
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            }
        ]],
        toolbar: [
        //{
        //    id: 'btnReload',
        //    text: '刷新',
        //    iconCls: 'icon-reload',
        //    handler: function () {
        //        //实现刷新栏目中的数据
        //        $("#grid").datagrid("reload");
        //    }
        //}, '-',
        {
            id: 'btnAddGroup',
            text: '新增',
            iconCls: 'icon-add',
            handler: function () {
                addGroup()
            }
        },
        {
            id: 'btnDeleteGroup',
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                delCheckGroup()
            }
        }
        ],
        onLoadSuccess: function (data) {
            //setScorePromotionGroup();
        }
    });
}

function addGroup() {
    var thisdlg = frxs.dialog({
        title: "增加门店到积分促销",
        url: "../ScorePromotion/ScorePromotionGroup",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                //$("#grid").datagrid("reload");
                //thisdlg.dialog("close");
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                //$("#grid").datagrid("reload");
                thisdlg.dialog("close");
            }
        }]
    });
}

function delCheckGroup() {
    var rows = $('#gridGroup').datagrid("getSelections");
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridGroup').datagrid('getRowIndex', copyRows[i]);
        $('#gridGroup').datagrid('deleteRow', index);
    }
    setScorePromotionGroup();
    $('#gridGroup').datagrid('clearSelections');
}

function setScorePromotionGroup() {
    var value = "";
    var rows = $('#gridGroup').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        //alert(row["ShopID"]);
        value += row["ShopID"] + ",";
    }
    if (value != "") {
        value = value.substr(0, value.length - 1);
    }
    $("#Groups").val(value);
    //alert($("#Groups").val());
}

function reloadGroups(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i]
        if (!rowGroupExist(row["ShopID"])) {
            $('#gridGroup').datagrid('appendRow', row);
        }
    }
    setScorePromotionGroup();
}

function rowGroupExist(id) {
    var rows = $('#gridGroup').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["ShopID"] == id) {
            return true;
        }
    }
    return false;
}


//设置Button权限
function SetPermission(type) {
    switch (type) {
        //case "view":
        //    $("#btnAddPlatformRate").linkbutton('enable');
        //    $("#btnEdit").linkbutton('enable');
        //    $("#saveBtn").linkbutton('disable');
        //    $("#btnSure").linkbutton('disable');
        //    $("#btnUnsure").linkbutton('disable');
        //    $("#btnPost").linkbutton('disable');
        //    $("#exportBtn").linkbutton('disable');
        //    $("#btnStop").linkbutton('disable');
        //    $("#btnAdd").linkbutton('disable');
        //    $("#btnDel").linkbutton('disable');
        //    $("#btnAddGroup").linkbutton('disable');
        //    $("#btnDeleteGroup").linkbutton('disable');
        //    break;
        case "add":
            $("#btnAddPlatformRate").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#saveBtn").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnCopy").linkbutton('disable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnAddGroup").linkbutton('enable');
            $("#btnDeleteGroup").linkbutton('enable');
            break;
        case "edit":
            $("#btnAddPlatformRate").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#saveBtn").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnCopy").linkbutton('disable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnAddGroup").linkbutton('enable');
            $("#btnDeleteGroup").linkbutton('enable');
            break;
        case "ludan":
            $("#btnAddPlatformRate").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#saveBtn").linkbutton('disable');
            $("#btnSure").linkbutton('enable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnCopy").linkbutton('enable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
        case "sure":
            $("#btnAddPlatformRate").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            //$("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnCopy").linkbutton('enable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
        case "posting":
            $("#btnAddPlatformRate").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnStop").linkbutton('enable');
            $("#btnCopy").linkbutton('enable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
        case "stop":
            $("#btnAddPlatformRate").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            $("#saveBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnCopy").linkbutton('enable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
    }
}

function saveData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var beginDate = $("#BeginTime").val();
        var endDate = $("#EndTime").val();
        var d1 = new Date(beginDate.replace(/\-/g, "\/"));
        var d2 = new Date(endDate.replace(/\-/g, "\/"));
        if (beginDate != "" && endDate != "" && d1 >= d2) {
            $.messager.alert("提示", "开始时间不能大于结束时间！", "info");
            return false;
        }
        endEditProduct();
        onEndEdit(editIndex);
        var jsonProduct = getProductJosn();
        var jsonGroup = getGroupJosn();
        //alert(jsonProduct);
        //alert(jsonGroup);
        //if (jsonProduct == "" || jsonGroup == "" || jsonProduct == "[]" || jsonGroup == "[]") {
        //    $.messager.alert("提示", "请添加商品和门店！", "info");
        //    return false;
        //}
        if ((jsonProduct == "" && jsonGroup == "") || (jsonProduct == "[]" && jsonGroup == "[]")) {
            $.messager.alert("提示", "请添加商品和门店！", "info");
            return false;
        }
        if (jsonProduct == "" || jsonProduct == "[]") {
            $.messager.alert("提示", "请添加商品！", "info");
            return false;
        }
        if (jsonGroup == "" || jsonGroup == "[]") {
            $.messager.alert("提示", "请添加门店！", "info");
            return false;
        }
        $("#jsonProduct").val(jsonProduct);
        $("#jsonGroup").val(jsonGroup);
        var data = $("#formAdd").serialize();
        //alert(data);
        var loading = frxs.loading("正在保存，请稍后...");
        $.ajax({
            url: "../ScorePromotion/AddScorePromotion",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                loading.close();
                if (obj.Flag == "SUCCESS") {
                    refreshParentGrid();
                    nosaleId = obj.Data;
                    //$.messager.alert("提示", obj.Info, "info");
                    $.messager.alert("提示", obj.Info, "info", function () {
                        reloadview(nosaleId);
                        //init();
                    });
                } else {
                    $.messager.alert("提示", obj.Info, "info");
                }
            }
        });
    }
}

function editData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        endEditProduct();
        onEndEdit(editIndex);
        var jsonProduct = getProductJosn();
        var jsonGroup = getGroupJosn();
        //alert(jsonProduct);
        //alert(jsonGroup);
        //if (jsonProduct == "" || jsonGroup == "" || jsonProduct == "[]" || jsonGroup == "[]") {
        //    $.messager.alert("提示", "请添加商品和门店！", "info");
        //    return false;
        //}
        if ((jsonProduct == "" && jsonGroup == "") || (jsonProduct == "[]" && jsonGroup == "[]")) {
            $.messager.alert("提示", "请添加商品和门店！", "info");
            return false;
        }
        if (jsonProduct == "" || jsonProduct == "[]") {
            $.messager.alert("提示", "请添加商品！", "info");
            return false;
        }
        if (jsonGroup == "" || jsonGroup == "[]") {
            $.messager.alert("提示", "请添加门店！", "info");
            return false;
        }
        $("#jsonProduct").val(jsonProduct);
        $("#jsonGroup").val(jsonGroup);
        var data = $("#formAdd").serialize();
        //alert(data);
        var loading = frxs.loading("正在保存，请稍后...");
        $.ajax({
            url: "../ScorePromotion/EditScorePromotion",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                loading.close();
                if (obj.Flag == "SUCCESS") {
                    refreshParentGrid();
                    nosaleId = obj.Data;
                    init();
                    $.messager.alert("提示", obj.Info, "info");
                } else {
                    $.messager.alert("提示", obj.Info, "info");
                }
            }
        });
    }
}

function reloadview(pid) {
    location.href = '../ScorePromotion/ScorePromotionAddOrEdit?view=1&Id=' + pid;
    frxs.updateTabTitle("查看积分促销单" + pid, "icon-add");
    $("#closeBtn").click(function () {
        frxs.tabClose();
    });
}

function add() {
    location.href = '../ScorePromotion/ScorePromotionAddOrEdit';
    frxs.updateTabTitle("新增积分促销单", "icon-add");
    $("#closeBtn").click(function () {
        frxs.tabClose();
    });
}

function save() {
    saveData();//保存复制单
}

function edit() {
    SetPermission("edit");
}

function sure() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "确认该积分促销单？", function (r) {
        if (r) {
            $.ajax({
                url: "../ScorePromotion/ConfirmScorePromotion",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            refreshParentGrid();
                            //frxs.tabClose();
                            init();
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

function unsure() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "反确认该积分促销单？", function (r) {
        if (r) {
            $.ajax({
                url: "../ScorePromotion/ConfirmScorePromotion",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 0 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            refreshParentGrid();
                            //frxs.tabClose();
                            init();
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

function posting() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "过账该积分促销单？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，请稍后...");
            $.ajax({
                url: "../ScorePromotion/PostingScorePromotion",
                type: "get",
                dataType: "json",
                data: { ids: idStr },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            refreshParentGrid();
                            //frxs.tabClose();
                            init();
                        }
                    }
                    loading.close();
                },
                error: function (request, textStatus, errThrown) {
                    if (textStatus) {
                        alert(textStatus);
                    } else if (errThrown) {
                        alert(errThrown);
                    } else {
                        alert("出现错误");
                    }
                    loading.close();
                }
            });
        }
    });
}

function stop() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "停用该积分促销单？", function (r) {
        if (r) {
            $.ajax({
                url: "../ScorePromotion/StopScorePromotion",
                type: "get",
                dataType: "json",
                data: { ids: idStr },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            refreshParentGrid();
                            //frxs.tabClose();
                            init();
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

function refreshParentGrid() {
    frxs.reParentTabGrid("grid");
}

function getProductJosn() {
    //$('#gridProduct').datagrid('endEdit', editIndex);
    var rows = $('#gridProduct').datagrid('getRows');
    var jsonProduct = "[";
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        //if (rows[i].ProductID) {
        jsonProduct += "{";
        jsonProduct += "\"ProductID\":" + rows[i].ProductID + ",";
        jsonProduct += "\"SKU\":" + rows[i].SKU + ",";
        jsonProduct += "\"WProductID\":" + rows[i].WProductID + ",";
        jsonProduct += "\"Unit\":\"" + rows[i].Unit + "\",";
        jsonProduct += "\"ProductName\":\"" + rows[i].ProductName + "\",";
        jsonProduct += "\"PackingQty\":" + rows[i].PackingQty + ",";
        jsonProduct += "\"SaleUnit\":\"" + rows[i].SaleUnit + "\",";
        jsonProduct += "\"SalePrice\":" + rows[i].SalePrice + ",";
        jsonProduct += "\"BarCode\":\"" + rows[i].BarCode + "\",";
        jsonProduct += "\"OldPoint\":" + rows[i].OldPoint + ",";
        jsonProduct += "\"Point\":" + rows[i].Point + ",";
        jsonProduct += "\"MaxOrderQty\":" + rows[i].MaxOrderQty + "";
        jsonProduct += "},";
        count++;
        //}
    }
    if (count > 0) {
        jsonProduct = jsonProduct.substring(0, jsonProduct.length - 1);
    }
    jsonProduct += "]";
    return jsonProduct;
}

function getGroupJosn() {
    //$('#gridGroup').datagrid('endEdit', editIndex);
    var rows = $('#gridGroup').datagrid('getRows');
    var jsonGroup = "[";
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        //if (rows[i].ShopID) {
        jsonGroup += "{";
        jsonGroup += "\"ShopID\":" + rows[i].ShopID + ",";
        jsonGroup += "\"WID\":" + rows[i].WID + ",";
        jsonGroup += "\"ShopCode\":\"" + rows[i].ShopCode + "\",";
        jsonGroup += "\"ShopName\":\"" + rows[i].ShopName + "\",";
        jsonGroup += "\"ShopType\":" + rows[i].ShopType + ",";
        jsonGroup += "\"FullAddress\":\"" + rows[i].FullAddress + "\"";
        jsonGroup += "},";
        count++;
        //}
    }
    if (count > 0) {
        jsonGroup = jsonGroup.substring(0, jsonGroup.length - 1);
    }
    jsonGroup += "]";
    return jsonGroup;
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 185);
    var h2 = h-30;
    $('#grid').height(h);
    $('.easyui-tabs').height(h);
    $('#gridProduct').datagrid({
        height: h2
    });
    $('#gridGroup').datagrid({
        height: h2
    });
}