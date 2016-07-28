var dialogWidth = 980;
var dialogHeight = 600;
var view = "";
var nosaleId = "";

$(function () {
    nosaleId = frxs.getUrlParam("Id");
    //下拉绑定
    //initDDL();
    init();
    gridresize();
    eventBind();
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
        url: "../PlatformRate/GetPlatformRate",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            obj.BeginTime = frxs.dateFormat(obj.BeginTime);
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
            if (obj.Status == 0) {//录单
                if (nosaleId) {
                    SetPermission('ludan');
                }
                else {
                    SetPermission('add');
                }
            }
            else if (obj.Status == 1) {//确认
                SetPermission('sure');
            }
            else if (obj.Status == 2) {//已生效
                SetPermission('posting');
            }
            else if (obj.Status == 3) {//已停用
                SetPermission('stop');
            }
            loading.close();
        }
    });
}

var editIndex = -1;
//初始化商品列表
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
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex, rowData) {
            $('#gridProduct').datagrid('clearSelections');
            $('#gridProduct').datagrid('selectRow', rowIndex);
            onClickRow(rowIndex);
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'ProductID', field: 'ProductID', width: 100, align: 'center', sortable: true, hidden: true },
            { title: 'WProductID', field: 'WProductID', width: 100, hidden: true },
            { title: '商品编码', field: 'SKU', width: 150, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 200 },
            { title: '库存单位', field: 'Unit', width: 100, align: 'center' },
            { title: '配送单位', field: 'SaleUnit', width: 100, hidden: true },//SaleUnit
            { title: '库存单位配送价格', field: 'SalePrice', width: 150, align: 'right' },
            { title: '国际条码', field: 'BarCode', width: 150, hidden: true, align: 'center' },
            //{ title: '原平台费率', field: 'Point', width: 100 }
            { title: '原平台费率', field: 'OldPoint', width: 100, align: 'center' }
            ,{
                title: frxs.setTitleColor('平台费率'), field: 'Point', width: 100, align: 'center'
                , editor: {
                    type: 'numberbox',
                    min: 0,
                    max: 1,
                    options: {
                        precision: 3,
                        required: true
                    }
                }
                //, formatter: function (value, rec) {
                //    return "<span class='thaw_text'>" + value + "</span>";
                //}
            }
            , { title: '包装数', field: 'PackingQty', width: 100, align: 'center' }//PackingQty
            ,{ title: '平台费用', field: 'PointPrice', width: 100, align: 'right' }
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
            //setPlatformRateProduct();
            //beginEditProduct();
            scoreProduct();
        }
    });
}

function beginEditProduct() {
    
}

function endEditProduct() {
    var rows = $('#gridProduct').datagrid('getRows');
    for (var i = 0; i < rows.length; i++) {
        var index = $("#gridProduct").datagrid("getRowIndex", rows[i]);
        $('#gridProduct').datagrid('endEdit', index);
    }
}

//事件绑定
function eventBind() {
    //绑定上下键事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        var index;
        //向下
        if (ev.keyCode == 40) {
            var selRow = $("#gridProduct").datagrid("getSelected");
            if (selRow != null) {
                index = $("#gridProduct").datagrid("getRowIndex", $("#gridProduct").datagrid("getSelected"));
                //$('#gridProduct').datagrid('selectRow', index).datagrid('endEdit', index);
                index = index + 1;
            } else {
                index = 0;
            }
            if (index > $("#gridProduct").datagrid("getRows").length - 1) {
                index = $("#gridProduct").datagrid("getRows").length - 1;
            }
            $('#gridProduct').datagrid('clearSelections');
            //$("#gridProduct").datagrid("selectRow", index);
            onClickRow(index);
        }

        //向上
        if (ev.keyCode == 38) {
            var selRow = $("#gridProduct").datagrid("getSelected");
            if (selRow != null) {
                index = $("#gridProduct").datagrid("getRowIndex", $("#gridProduct").datagrid("getSelected"));
                //$('#gridProduct').datagrid('selectRow', index).datagrid('endEdit', index);
                index = index - 1;
            } else {
                index = $("#gridProduct").datagrid("getRows").length - 1;
            }
            if (index < 0) {
                index = 0;
            }
            $('#gridProduct').datagrid('clearSelections');
            //$("#gridProduct").datagrid("selectRow", index);
            onClickRow(index);
        }
    };
}



//修改平台费率
function onClickRow(index) {
    if (editIndex != index && editIndex != -1) {
        $('#gridProduct').datagrid('endEdit', editIndex);
    }
    $('#gridProduct').datagrid('selectRow', index).datagrid('beginEdit', index);
    var rows = $('#gridProduct').datagrid("getRows");
    var row = rows[index];
    var ed = $('#gridProduct').datagrid('getEditor', { index: index, field: 'Point' });
    $(ed.target).numberbox({
        min: 0,
        max: 1,
        options: {
            precision: 3,
            required: true
        },
        onChange: function () {
            var point = $(ed.target).numberbox('getValue');
            var salePrice = row.SalePrice;
            var packingQty = row.PackingQty;
            //row.PointPrice = salePrice * point;//平台费用=配送价格*平台费率
            row.PointPrice = salePrice * packingQty * point;//平台费用=配送价格*包装数*平台费率
            row.PointPrice = parseFloat(row.PointPrice).toFixed(3);
            //$('#gridProduct').datagrid('selectRow', index).datagrid('endEdit', index);

            //var oldPoint = row.Point;
            //var point = $(ed.target).numberbox('getValue');
            //if (point < 0 || point > 1) {
            //    alert("平台费率只能输入0至1的小数！");
            //    $(ed.target).numberbox('setValue', oldPoint);
            //} else {
            //    var salePrice = row.SalePrice;
            //    var packingQty = row.PackingQty;
            //    row.PointPrice = salePrice * packingQty * point;
            //    row.PointPrice = parseFloat(row.PointPrice).toFixed(3);
            //}
            //$('#gridProduct').datagrid('selectRow', index).datagrid('endEdit', index);
        }
    });
    
    var ed = $("#gridProduct").datagrid('getEditor', { index: index, field: 'Point' });
    $(ed.target).textbox('textbox').focus();
    
    editIndex = index;
}

function onEndEdit(index) {

}

//保存时重新计算修改后平台费率
function scoreProduct() {
    var rows = $('#gridProduct').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        $('#gridProduct').datagrid('beginEdit', i);
        var row = rows[i];
        var point = row.Point;
        var salePrice = row.SalePrice;
        var packingQty = row.PackingQty;
        //row.PointPrice = salePrice * point;//平台费用=配送价格*平台费率
        row.PointPrice = salePrice * packingQty * point;//平台费用=配送价格*包装数*平台费率
        row.PointPrice = parseFloat(row.PointPrice).toFixed(3)
        $('#gridProduct').datagrid('endEdit', i);
    }
}

//商品选择对话框
function addProduct() {
    var thisdlg = frxs.dialog({
        title: "增加商品到平台费率调整",
        url: "../PlatformRate/PlatformRateProduct",
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

//批量删除商品
function delCheckProduct() {
    var rows = $('#gridProduct').datagrid("getSelections");
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return;
    }
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridProduct').datagrid('getRowIndex', copyRows[i]);
        $('#gridProduct').datagrid('deleteRow', index);
    }
    setPlatformRateProduct();
    $('#gridGroup').datagrid('clearSelections');
}


function setPlatformRateProduct() {
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
}

//商品选择对话框选择商品后重新加载商品列表
function reloadProducts(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (!rowProductExist(row["ProductId"])) {
            row["ProductID"] = row["ProductId"];
            row["WProductID"] = row["WProductId"];
            row["PackingQty"] = row["BigPackingQty"];
            row["SaleUnit"] = row["BigUnit"];
            row["OldPoint"] = row["ShopAddPerc"];
            //row["Point"] = 0;
            row["Point"] = row["ShopAddPerc"];//新增时平台费率和原平台费率=商品原平台费率字段
            //row["PointPrice"] = parseFloat(row["SalePrice"] * row["ShopAddPerc"]).toFixed(3);
            row["SalePrice"] = row["UnitSalePrice"];
            row["PointPrice"] = parseFloat(row["SalePrice"] * row["PackingQty"] * row["OldPoint"]).toFixed(3);
            $('#gridProduct').datagrid('appendRow', row);
        }
    }
    setPlatformRateProduct();
    beginEditProduct();
}

//判断商品列表是否已存在商品
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

//初始化门店列表
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
            
        }
    });
}

//选择门店对话框
function addGroup() {
    var thisdlg = frxs.dialog({
        title: "增加门店到平台费率调整",
        url: "../PlatformRate/PlatformRateGroup",
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

//删除选择门店
function delCheckGroup() {
    var rows = $('#gridGroup').datagrid("getSelections");
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return;
    }
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridGroup').datagrid('getRowIndex', copyRows[i]);
        $('#gridGroup').datagrid('deleteRow', index);
    }
    setPlatformRateGroup();
    $('#gridGroup').datagrid('clearSelections');
}

function setPlatformRateGroup() {
    var value = "";
    var rows = $('#gridGroup').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        value += row["ShopID"] + ",";
    }
    if (value != "") {
        value = value.substr(0, value.length - 1);
    }
    $("#Groups").val(value);
}

function reloadGroups(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i]
        if (!rowGroupExist(row["ShopID"])) {
            $('#gridGroup').datagrid('appendRow', row);
        }
    }
    setPlatformRateGroup();
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
        //    $("#btnSave").linkbutton('disable');
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
            $("#btnSave").linkbutton('enable');
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
            $("#btnSave").linkbutton('enable');
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
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('enable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
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
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            //$("#exportBtn").linkbutton('disable');
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
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
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
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnCopy").linkbutton('enable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
    }
}

//新增
function saveData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        endEditProduct();
        onEndEdit(editIndex);
        if (getProductCheck() == false) {
            return false;
        }
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
            url: "../PlatformRate/AddPlatformRate",
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

//编辑
function editData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        endEditProduct();
        onEndEdit(editIndex);
        if (getProductCheck() == false) {
            return false;
        }
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
            url: "../PlatformRate/EditPlatformRate",
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
    location.href = '../PlatformRate/PlatformRateAddOrEdit?view=1&Id=' + pid;
    frxs.updateTabTitle("查看平台费率调整单" + pid, "icon-add");
    $("#btnClose").click(function () {
        frxs.tabClose();
    });
}

function add() {
    location.href = '../PlatformRate/PlatformRateAddOrEdit';
    frxs.updateTabTitle("新增平台费率调整单", "icon-add");
    $("#closeBtn").click(function () {
        frxs.tabClose();
    });
}

function save() {
    if (nosaleId) {
        editData();
    } else {
        saveData();
    }
}

function edit() {
    frxs.updateTabTitle("编辑平台费率调整单" + nosaleId, "icon-edit");
    SetPermission("edit");
}

//确认
function sure() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "确认该平台费率调整单？", function (r) {
        if (r) {
            $.ajax({
                url: "../PlatformRate/ConfirmPlatformRate",
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

//反确认
function unsure() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "反确认该平台费率调整单？", function (r) {
        if (r) {
            $.ajax({
                url: "../PlatformRate/UnConfirmPlatformRate",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 0 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            //window.frameElement.wapi.reload();
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

//生效
function posting() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "过账该平台费率调整单？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，请稍后...");
            $.ajax({
                url: "../PlatformRate/PostingPlatformRate",
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
            })
        }
    });
}

//停用
function stop() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "停用该平台费率调整单？", function (r) {
        if (r) {
            $.ajax({
                url: "../PlatformRate/StopPlatformRate",
                type: "get",
                dataType: "json",
                data: { ids: idStr },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            //window.frameElement.wapi.reload();
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

//复制
function copy() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "复制该平台费率调整单？", function (r) {
        if (r) {
            location.href = '../PlatformRate/PlatformRateCopy?Id=' + idStr;
            frxs.updateTabTitle("复制平台费率调整单" + idStr, "icon-add");
            $("#closeBtn").click(function () {
                frxs.tabClose();
            });
        }
    });
}

function refreshParentGrid() {
    frxs.reParentTabGrid("grid");
}

//检查平台费率
function getProductCheck() {
    var rows = $('#gridProduct').datagrid('getRows');
    for (var i = 0; i < rows.length; i++) {
        var point = rows[i].Point;
        if (point < 0 || point > 1) {
            $.messager.alert("提示", "平台费率应该在 0-1.000 之间", "info");
            return false;
        }
    }
    return true;
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
            jsonProduct += "\"Point\":" + rows[i].Point + "";
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

//打印门店平台费率调整单
function print() {
    var promotionId = $("#PromotionID").val();
    if (promotionId != "") {
        var thisdlg = frxs.dialog({
            title: "打印门店平台费率调整单",
            url: "../FastReportTemplets/Aspx/PrintPlatformRate.aspx?PromotionID=" + promotionId,
            owdoc: window.top,
            width: 765,
            height: 600
        });
    } else {
        $.messager.alert("提示", "单据号不能为空！", "info");
    }
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 185);
    var h2 = h - 30;
    $('#grid').height(h);
    $('.easyui-tabs').height(h);
    $('#gridProduct').datagrid({
        height: h2
    });
    $('#gridGroup').datagrid({
        height: h2
    });
}