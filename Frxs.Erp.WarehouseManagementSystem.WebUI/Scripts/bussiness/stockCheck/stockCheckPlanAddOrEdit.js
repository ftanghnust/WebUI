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
function initDDL(wid) {
    $("#CheckType").combobox({
        onChange: function (n, o) {
            checkTypeChange();
        }
    });
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            data = $.parseJSON(data);
            $("#SubWID").combobox({
                data: data,
                valueField: "WID",
                textField: "WName"
            });
            $("#SubWID").combobox('select', data[0].WID);
            if (wid != null && wid != 0) {
                $("#SubWID").combobox('select', wid);
            }
            if (Id != null) {
                //$("#SubWID").combobox('disable');//disable 方式 var data = $("#formAdd").serialize()会失效
                $("#SubWID").combobox('readonly');//编辑状态禁用子机构下拉框
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
        url: "../StockCheckPlan/GetStockCheckPlan",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            initDDL(0);
            initStockCheckPlanDetail();
            checkTypeChange();
            loading.close();
        }
    });
}

function checkTypeChange() {
    var checkType = $("#CheckType").combobox("getValue");
    $('#gridStockCheckPlanDetail').datagrid('loadData', { total: 0, rows: [] });//清空
    if (checkType == 0) {
        $("#trStockCheckPlanDetail").css('display', 'none');
    } else if (checkType == 1) {
        $("#trStockCheckPlanDetail").css('display', '');
    } else if (checkType == 2) {
        $("#trStockCheckPlanDetail").css('display', '');
    }
}

function initStockCheckPlanDetail(griddata) {
    $('#gridStockCheckPlanDetail').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        data: griddata,
        //url: ajaxUrl,          //Aajx地址
        sortName: 'BaseInfoID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'BaseInfoID',                  //主键
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
            $(this).datagrid('unselectAll');
            $(this).datagrid('selectRow', rowIndex);
        },
        frozenColumns: [[
            
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'BaseInfoID', field: 'BaseInfoID', hidden: true },
            { title: 'BaseInfoCode', field: 'BaseInfoCode', width: 150, hidden: true },
            { title: '名称', field: 'BaseInfoName', width: 200, align: 'left' }
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
                addStockCheckPlanDetail()
            }
        },
        {
            id: 'btnDel',
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                delStockCheckPlanDetail()
            }
        }
        ],
        onLoadSuccess: function (data) {
            
        }
    });
}

function addStockCheckPlanDetail() {
    var checkType = $("#CheckType").combobox("getValue");
    if (checkType == 1) {
        addShelf();
    } else if (checkType == 2) {
        addShelfArea();
    }
}

function delStockCheckPlanDetail() {
    var rows = $('#gridStockCheckPlanDetail').datagrid("getSelections");
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridStockCheckPlanDetail').datagrid('getRowIndex', copyRows[i]);
        $('#gridStockCheckPlanDetail').datagrid('deleteRow', index);
    }
    $('#gridStockCheckPlanDetail').datagrid('clearSelections');
}

function StockCheckPlanDetailExist(id) {
    var rows = $('#gridStockCheckPlanDetail').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["BaseInfoID"] == id) {
            return true;
        }
    }
    return false;
}

function addShelf() {
    var thisdlg = frxs.dialog({
        title: "选择货位",
        url: "/StockCheckPlan/EasyuiShelfList",
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

function loadShelf(grid) {
    var rows = grid.datagrid('getRows');
    if (rows != null && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (!StockCheckPlanDetailExist(rows[i].ShelfID)) {
                var row = { BaseInfoID: rows[i].ShelfID, BaseInfoCode: rows[i].ShelfCode, BaseInfoName: rows[i].ShelfCode };
                $('#gridStockCheckPlanDetail').datagrid('appendRow', row);
            }
        }
    }
}

function addShelfArea() {
    var thisdlg = frxs.dialog({
        title: "选择货区",
        url: "/StockCheckPlan/EasyuiShelfAreaList",
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

function loadShelfArea(grid) {
    var rows = grid.datagrid('getRows');
    if (rows != null && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (!StockCheckPlanDetailExist(rows[i].ShelfID)) {
                var row = { BaseInfoID: rows[i].ShelfAreaID, BaseInfoCode: rows[i].ShelfAreaCode, BaseInfoName: rows[i].ShelfAreaName };
                $('#gridStockCheckPlanDetail').datagrid('appendRow', row);
            }
        }
    }
}

//设置Button权限
function SetPermission(type) {
    
}

function saveData() {
   
}

function editData() {
   
}

function add() {
    location.href = '../StockCheckPlan/StockCheckPlanAddOrEdit';
    frxs.updateTabTitle("新增盘点计划单", "icon-add");
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
    frxs.updateTabTitle("编辑商品限购" + nosaleId, "icon-edit");
    SetPermission("edit");
}

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


