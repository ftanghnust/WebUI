var dialogWidth = 850;
var dialogHeight = 600;
var view = "";
var nosaleId = "";

$(function () {
    nosaleId = frxs.getUrlParam("Id");
    init();
    gridresize();
});

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
            $("#WID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#WID").combobox('select', data[0].WID);
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
        url: "../ProductLimit/GetProductLimit",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            obj.BeginTime = frxs.dateFormat(obj.BeginTime);
            $('#formAdd').form('load', obj);
            //initDDL();
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
            $('#gridProduct').datagrid('unselectAll');
            $('#gridProduct').datagrid('selectRow', rowIndex);
        },
        //queryParams: {
        //    //查询条件
        //    nosaleId: nosaleId
        //},
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'ProductID', field: 'ProductID', width: 100, align: 'center', sortable: true, hidden: true },
            { title: 'WProductID', field: 'WProductID', width: 100, hidden: true },
            { title: '商品编码', field: 'SKU', width: 150,align:'center' },
            { title: '名称', field: 'ProductName', width: 200 },
            { title: '配送价格', field: 'SalePrice', width: 100, align: 'right' },
            { title: '库存单位', field: 'Unit', width: 100, align: 'center' },
            { title: '包装数', field: 'BigPackingQty', width: 100, align: 'center' },
            { title: '配送单位', field: 'BigUnit', width: 100, align: 'center' },
            { title: '国际条码', field: 'BarCode', width: 150, align: 'center' }
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
            //setProductLimitProduct();
        }
    });
}

function addProduct() {
    var thisdlg = frxs.dialog({
        title: "增加商品到限购",
        url: "/ProductLimit/ProductLimitProduct",
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
    setProductLimitProduct();
    $('#gridProduct').datagrid('clearSelections');
}

function setProductLimitProduct() {
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

function reloadProducts(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i]
        if (!rowProductExist(row["ProductId"])) {
            row["ProductID"] = row["ProductId"];
            row["WProductID"] = row["WProductId"];
            $('#gridProduct').datagrid('appendRow', row);
        }
    }
    setProductLimitProduct();
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
    //alert(griddata);
    $('#gridGroup').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        //methord: 'get',                    //提交方式
        data: griddata,
        //url: ajaxUrl,          //Aajx地址
        sortName: 'GroupID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'GroupID',                  //主键
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
            $('#gridGroup').datagrid('unselectAll');
            $('#gridGroup').datagrid('selectRow', rowIndex);
        },
        //queryParams: {
        //    //查询条件
        //    nosaleId: nosaleId
        //},
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'GroupID', field: 'GroupID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: '群组编号', field: 'GroupCode', width: 100, align: 'center' },
            { title: '群组名称', field: 'GroupName', width: 200 },
            { title: '门店数', field: 'ShopNum', width: 100, align: 'center' },
            { title: '备注', field: 'Remark', width: 280 }
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
            //setProductLimitGroup();
        }
    });
}

function addGroup() {
    var thisdlg = frxs.dialog({
        title: "增加门店群组到限购",
        url: "/ProductLimit/ProductLimitGroup",
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
    setProductLimitGroup();
    $('#gridGroup').datagrid('clearSelections');
}

function setProductLimitGroup() {
    var value = "";
    var rows = $('#gridGroup').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        //alert(row["GroupID"]);
        value += row["GroupID"] + ",";
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
        if (!rowGroupExist(row["GroupID"])) {
            $('#gridGroup').datagrid('appendRow', row);
        }
    }
    setProductLimitGroup();
}

function rowGroupExist(id) {
    var rows = $('#gridGroup').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["GroupID"] == id) {
            return true;
        }
    }
    return false;
}


//设置Button权限
function SetPermission(type) {
    //alert(type);
    switch (type) {
        //case "view":
        //    $("#btnAddProductLimit").linkbutton('enable');
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
            $("#btnAddProductLimit").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnAddGroup").linkbutton('enable');
            $("#btnDeleteGroup").linkbutton('enable');
            break;
        case "edit":
            //$('input, select').removeAttr('disabled');
            $("#btnAddProductLimit").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnAddGroup").linkbutton('enable');
            $("#btnDeleteGroup").linkbutton('enable');
            break;
        case "ludan":
            //$('input, select').attr('disabled', 'disabled');
            $("#btnAddProductLimit").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('enable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
        case "sure":
            $("#btnAddProductLimit").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            $("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
        case "posting":
            $("#btnAddProductLimit").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('enable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
        case "stop":
            $("#btnAddProductLimit").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnStop").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnAddGroup").linkbutton('disable');
            $("#btnDeleteGroup").linkbutton('disable');
            break;
    }
}

//保存限购信息
function saveData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var jsonProduct = getProductJosn();
        var jsonGroup = getGroupJosn();
        //alert(jsonProduct);
        //alert(jsonGroup);
        if (jsonProduct == "" || jsonGroup == "" || jsonProduct == "[]" || jsonGroup == "[]") {
            $.messager.alert("提示", "请添加商品和门店分组！", "info");
            return false;
        }
        $("#jsonProduct").val(jsonProduct);
        $("#jsonGroup").val(jsonGroup);
        var data = $("#formAdd").serialize();
        //alert(data);
        var loading = frxs.loading("正在保存，请稍后...");
        $.ajax({
            url: "../ProductLimit/AddProductLimit",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                //alert(obj.Info);
                $.messager.alert('提示', obj.Info, 'info');
                refreshParentGrid();
                //frxs.tabClose();
                nosaleId = obj.Data;
                init();
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
}

function editData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var jsonProduct = getProductJosn();
        var jsonGroup = getGroupJosn();
        //alert(jsonProduct);
        //alert(jsonGroup);
        if (jsonProduct == "" || jsonGroup == "" || jsonProduct == "[]" || jsonGroup == "[]") {
            $.messager.alert("提示", "请添加商品和门店分组！", "info");
            return false;
        }
        $("#jsonProduct").val(jsonProduct);
        $("#jsonGroup").val(jsonGroup);
        var data = $("#formAdd").serialize();
        //alert(data);
        var loading = frxs.loading("正在保存，请稍后...");
        $.ajax({
            url: "../ProductLimit/EditProductLimit",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                //alert(obj.Info);
                $.messager.alert('提示', obj.Info, 'info');
                refreshParentGrid();
                //frxs.tabClose();
                nosaleId = obj.Data;
                init();
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
}

function add() {
    location.href = '../ProductLimit/ProductLimitAddOrEdit';
    frxs.updateTabTitle("新增商品限购单", "icon-add");
    $("#closeBtn").click(function () {
        frxs.tabClose();
    });
}

//单据保存按键事件
function save() {
    if (nosaleId) {
        editData();
    } else {
        saveData();
    }
}

function edit() {
    frxs.updateTabTitle("编辑商品限购" + nosaleId, "icon-edit");
    SetPermission("edit");
}

//确认单据
function sure() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "确认该限购单？", function (r) {
        if (r) {
            $.ajax({
                url: "/ProductLimit/ConfirmProductLimit",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 1 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
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
    $.messager.confirm("提示", "反确认该限购单？", function (r) {
        if (r) {
            $.ajax({
                url: "/ProductLimit/UnConfirmProductLimit",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 0 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
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

//过账
function posting() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "过账该限购单？", function (r) {
        if (r) {
            $.ajax({
                url: "/ProductLimit/PostingProductLimit",
                type: "get",
                dataType: "json",
                data: { ids: idStr },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
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

//停用
function stop() {
    var idStr = "";
    if (nosaleId) {
        idStr = nosaleId;
    } else {
        return;
    }
    $.messager.confirm("提示", "停用该限购单？", function (r) {
        if (r) {
            $.ajax({
                url: "/ProductLimit/StopProductLimit",
                type: "get",
                dataType: "json",
                data: { ids: idStr },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
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
            jsonProduct += "\"WProductID\":" + rows[i].WProductID + ",";
            jsonProduct += "\"Unit\":\"" + rows[i].Unit + "\",";
            jsonProduct += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonProduct += "\"BigPackingQty\":" + rows[i].BigPackingQty + ",";
            jsonProduct += "\"BigUnit\":\"" + rows[i].BigUnit + "\",";
            jsonProduct += "\"SalePrice\":" + rows[i].SalePrice + ",";
            jsonProduct += "\"BarCode\":\"" + rows[i].BarCode + "\"";
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
        //if (rows[i].GroupID) {
            jsonGroup += "{";
            jsonGroup += "\"GroupID\":" + rows[i].GroupID + "";
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

//打印门店商品限购单
function print() {
    var nosaleid = $("#NoSaleID").val();
    if (nosaleid != "") {
        var thisdlg = frxs.dialog({
            title: "打印门店商品限购单",
            url: "../FastReportTemplets/Aspx/PrintProductLimit.aspx?NoSaleID=" + nosaleid,
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
    var h2 = h - 32;
    $("#grid").height(h);
    $("#easyuitabs").tabs({
        height: h,
    });
    $("#gridProduct").datagrid({
        height: h2,
    });
    $("#gridGroup").datagrid({
        height: h2,
    });
};


