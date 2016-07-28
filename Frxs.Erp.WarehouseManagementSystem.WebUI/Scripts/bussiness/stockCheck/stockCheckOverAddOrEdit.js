var dialogWidth = 850;
var dialogHeight = 400;
var view = "";
var Id = "";
var stockType = "盘盈单";

$(function () {
    Id = frxs.getUrlParam("Id");
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
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }

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
    //alert(Id);
    var id = "";
    if (Id != null) {
        id = Id;
    }
    $.ajax({
        url: "../StockAdj/GetStockAdj",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            $('#formAdd').form('load', obj);
            if (obj.AdjDate != null && obj.AdjDate != "") {
                $("#AdjDate").val(obj.AdjDate.DataFormat("yyyy-MM-dd"));
            }
            initDDL(obj.SubWID);
            initStockAdjDetailGrid();
            bindStockAdjDetailBtn();
            if (obj.Status == 0) {//录单
                if (Id) {
                    SetPermission('ludan');
                }
                else {
                    SetPermission('add');
                }
            }
            else if (obj.Status == 1) {//确认
                SetPermission('sure');
            }
            else if (obj.Status == 2) {//已过账
                SetPermission('posting');
            }
            else if (obj.Status == 3) {//已停用
                SetPermission('stop');
            }
            loading.close();
        }
    });
}

function bindStockAdjDetailBtn() {
    //$("#addStockAdjDetailBtn").click(function () { addStockAdjDetail(); });
    //$("#editStockAdjDetailBtn").click(function () { editCheckStockAdjDetail(); });
    //$("#delStockAdjDetailBtn").click(function () { delCheckStockAdjDetail(); });
    //$("#importStockAdjDetailBtn").click(function () { importStockAdjDetail(); });
    //$("#searchStockAdjDetailBtn").click(function () { searchStockAdjDetail(); });
}

//初始化明细grid
function initStockAdjDetailGrid() {
    if (!Id) {
        return;
    }
    $('#gridStockAdjDetail').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../StockAdjDetail/GetStockAdjDetailPageData',          //Aajx地址
        sortName: 'SerialNumber',                 //排序字段 需求确认要求按照序号升序排列
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        showFooter: true,
        onClickRow: function (rowIndex, rowData) {
            $('#gridStockAdjDetail').datagrid('unselectAll');
            $('#gridStockAdjDetail').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            //查询条件
            SearchValue: $.trim($("#SearchValue").val()),
            AdjID: $("#AdjID").val()
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'ID', field: 'ID', width: 10, hidden: true },
            { title: 'AdjID', field: 'AdjID', width: 10, hidden: true },
            { title: 'ProductID', field: 'ProductID', width: 10, hidden: true },
            { title: '商品编码', field: 'SKU', width: 80 },
            { title: '商品名称', field: 'ProductName', width: 120 , formatter: frxs.formatText},
            { title: '单位', field: 'AdjUnit', width: 50 },
            { title: '数量', field: 'AdjQty', width: 60 },
            { title: '包装数', field: 'AdjPackingQty', width: 60 },
            { title: '总数量', field: 'UnitQty', width: 100 },
            { title: '库存数量', field: 'StockQty', width: 70 },
            { title: '备注', field: 'Remark', width: 120, formatter: frxs.formatText },
            {
                title: '采购单价', field: 'BuyPrice', width: 60, align: 'right'
                //, formatter: function (value, rec) {
                //    return parseFloat(value).toFixed(4);
                //}
            },
            {
                title: '金额', field: 'AdjAmt', width: 110, align: 'right'
                //, formatter: function (value, rec) {
                //    return parseFloat(value).toFixed(4);
                //}
            },
            {
                title: '配送单价', field: 'SalePrice', width: 60, align: 'right'
                //, formatter: function (value, rec) {
                //    return parseFloat(value).toFixed(4);
                //}
            },
            { title: '国际条码', field: 'BarCode', width: 120 }
            
        ]],
        toolbar: [],
        onLoadSuccess: function (data) {
            $("#gridStockAdjDetail").datagrid('clearSelections');
            totalCalculate();
        }
    });
}

//添加明细
function addStockAdjDetail() {
    var thisdlg = frxs.dialog({
        title: "新增盘点明细",
        url: "../StockAdjDetail/StockAdjDetailAddOrEdit?stockId=" + Id,
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData(Id);
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

//编辑明细
function editCheckStockAdjDetail() {
    var rows = $('#gridStockAdjDetail').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        window.focus();
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        return;
    } else {
        editStockAdjDetail(rows[0].ID);
        //$("#gridStockAdjDetail").datagrid('clearSelections');
    }
}

function editStockAdjDetail(detailId) {
    var thisdlg = frxs.dialog({
        title: "编辑盘点明细",
        url: "../StockAdjDetail/StockAdjDetailAddOrEdit?Id=" + detailId + "&stockId=" + Id,
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData(Id);
                $("#gridStockAdjDetail").datagrid("reload");
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

//删除明细
function delCheckStockAdjDetail() {
    var rows = $('#gridStockAdjDetail').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
        //$.messager.alert("提示", "没有选中记录！", "info", function () { window.focus(); });
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        idStr += ("" + rows[i].ID + ",");
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    }
    $.messager.confirm("提示", "确认删除明细？", function (r) {
        if (r) {
            $.ajax({
                url: "../StockAdjDetail/DeleteStockAdjDetail",
                type: "get",
                dataType: "json",
                data: {
                    ids: idStr
                },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        $.messager.alert('提示', result.Info, 'info');
                        if (result.Flag == "SUCCESS") {
                            $("#gridStockAdjDetail").datagrid("reload");
                            $("#gridStockAdjDetail").datagrid('clearSelections');
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

function clearStockAdjDetail() {
    $.messager.confirm("提示", "确认删除所有明细？", function (r) {
        if (r) {
            $.ajax({
                url: "../StockAdjDetail/ClearStockAdjDetail",
                type: "get",
                dataType: "json",
                data: {
                    id: Id
                },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        $.messager.alert('提示', result.Info, 'info');
                        if (result.Flag == "SUCCESS") {
                            $("#gridStockAdjDetail").datagrid("reload");
                            $("#gridStockAdjDetail").datagrid('clearSelections');
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

//导入明细
function importStockAdjDetail() {
    showStockAdjDetailImport();
}

//搜索明细
function searchStockAdjDetail() {
    initStockAdjDetailGrid();
}

function reloadStockAdjDetail() {
    initStockAdjDetailGrid();
}

//显示导入界面
function showStockAdjDetailImport() {
    var adjId = "";
    if (Id) {
        adjId = Id;
    } else {
        return;
    }
    $.ajax({
        url: "/StockAdj/GetStockAdjDetailCount",
        type: "get",
        dataType: "json",
        data: { id: adjId },
        success: function (obj) {
            if (obj != undefined && obj.Info != undefined) {
                if (obj.Flag == "SUCCESS") {
                    if (obj.Data > 0) {
                        //已存在明细，需先删除明细再导入
                        $.messager.alert('提示', "该盘点调整单下有" + obj.Data + "条明细，请先手动删除所有明细后再导入！", 'info');
                    } else {
                        //没有明细直接导入
                        var thisdlg = frxs.dialog({
                            title: "盘点明细-Excel数据导入",
                            url: "../StockAdjDetail/ImportStockAdjDetail?adjType=0",
                            owdoc: window.top,
                            width: 800,
                            height: 260,
                            buttons: [{
                                text: '提交',
                                iconCls: 'icon-ok',
                                handler: function () {
                                    thisdlg.subpage.saveData(Id, this);//传入this对象禁用提交按键
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
                } else {
                    $.messager.alert('提示', obj.Info, 'info');
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


//统计列数据
function totalCalculate() {
    if (!Id) {
        return;
    }

    var adjQtySum = 0.00;
    var adjAmtSum = 0.00;
    $.ajax({
        url: "../StockAdjDetail/GetStockAdjDetailsCalculate",
        type: "post",
        dataType: "json",
        data: { adjID: Id },
        success: function (obj) {
            adjQtySum = obj.AdjQtySum;
            adjAmtSum = obj.AdjAmtSum;
            var totalUnitQty = parseFloat(adjQtySum).toFixed(2);
            var totalAdjAmt = parseFloat(adjAmtSum).toFixed(4);
            $('#gridStockAdjDetail').datagrid('reloadFooter', [
                {
                    "SKU": "总计",
                    "AdjPackingQty": "总数量:",
                    "UnitQty": totalUnitQty,
                    "BuyPrice": "总金额:",
                    "AdjAmt": totalAdjAmt
                }
            ]);
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


//设置Button权限
function SetPermission(type) {
    //alert(type);
    switch (type) {
        case "add":
            $("#btnAddStock").linkbutton('disable');
            $("#btnEditStock").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            //$("#stopBtn").linkbutton('disable');
            SetDetailPermission("add");
            break;
        case "edit":
            //$('input, select').removeAttr('disabled');
            $("#btnAddStock").linkbutton('disable');
            $("#btnEditStock").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            //$("#stopBtn").linkbutton('disable');
            SetDetailPermission("edit");
            break;
        case "ludan":
            //$('input, select').attr('disabled', 'disabled');
            $("#btnAddStock").linkbutton('enable');
            $("#btnEditStock").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('enable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            //$("#stopBtn").linkbutton('disable');
            SetDetailPermission("edit");//录单状态下明细自动可编辑
            break;
        case "sure":
            $("#btnAddStock").linkbutton('enable');
            $("#btnEditStock").linkbutton('disable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            //$("#exportBtn").linkbutton('disable');
            //$("#stopBtn").linkbutton('disable');
            SetDetailPermission("add");
            break;
        case "posting":
            $("#btnAddStock").linkbutton('enable');
            $("#btnEditStock").linkbutton('disable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            //$("#stopBtn").linkbutton('enable');
            SetDetailPermission("add");
            break;
        case "stop":
            $("#btnAddStock").linkbutton('enable');
            $("#btnEditStock").linkbutton('disable');
            $("#btnSave").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#btnUnsure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            //$("#exportBtn").linkbutton('disable');
            //$("#stopBtn").linkbutton('disable');
            SetDetailPermission("add");
            break;
    }
}

function SetDetailPermission(type) {
    switch (type) {
        case "add":
            //$("#addStockAdjDetailBtn").linkbutton('disable');
            //$("#editStockAdjDetailBtn").linkbutton('disable');
            //$("#delStockAdjDetailBtn").linkbutton('disable');
            //$("#clearStockAdjDetailBtn").linkbutton('disable');
            //$("#importStockAdjDetailBtn").linkbutton('disable');
            //$("#searchStockAdjDetailBtn").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnClearStockAdjDetail").linkbutton('disable');
            $("#btnImportStockAdjDetail").linkbutton('disable');
            //$("#btnSearchStockAdjDetail").linkbutton('disable');//查看时允许搜索明细
            break;
        case "edit":
            //$("#addStockAdjDetailBtn").linkbutton('enable');
            //$("#editStockAdjDetailBtn").linkbutton('enable');
            //$("#delStockAdjDetailBtn").linkbutton('enable');
            //$("#clearStockAdjDetailBtn").linkbutton('enable');
            //$("#importStockAdjDetailBtn").linkbutton('enable');
            //$("#searchStockAdjDetailBtn").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnClearStockAdjDetail").linkbutton('enable');
            $("#btnImportStockAdjDetail").linkbutton('enable');
            $("#btnSearchStockAdjDetail").linkbutton('enable');
            break;
    }
}

//新增表头
function saveData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        
        if ($("#SubWID").combobox("getValue") == "") {
            $.messager.alert("提示", "请选择仓库！", "info");
            return false;
        }

        var loading = frxs.loading("正在保存，请稍后...");
        var data = $("#formAdd").serialize();
        //alert(data);
        $.ajax({
            url: "../StockAdj/AddStockOverAdj",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                //alert(obj.Info);
                refreshParentGrid();
                //frxs.tabClose();
                Id = obj.Data;
                init();
                $.messager.alert('提示', obj.Info, 'info');
                loading.close();
            }
        });
    }
}

//编辑表头
function editData() {

    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var adjId = "";
        if (Id) {
            adjId = Id;
        } else {
            return;
        }
        var loading = frxs.loading("正在保存，请稍后...");
        $.ajax({
            url: "/StockAdj/GetStockAdjDetailCount",
            type: "get",
            dataType: "json",
            data: { id: adjId },
            success: function (obj) {
                if (obj != undefined && obj.Info != undefined) {
                    if (obj.Flag == "SUCCESS") {
                        if (obj.Data > 0) {
                            var data = $("#formAdd").serialize()

                            ;
                            //alert(data);
                            $.ajax({
                                url: "../StockAdj/EditStockOverAdj",
                                type: "post",
                                data: data,
                                dataType: 'json',
                                success: function (obj) {
                                    //alert(obj.Info);
                                    refreshParentGrid();
                                    //frxs.tabClose();
                                    init();
                                    $.messager.alert('提示', obj.Info, 'info');
                                    loading.close();
                                }
                            });
                        } else {
                            $.messager.alert('提示', "请添加盘点明细！", 'info');
                            loading.close();
                        }
                    } else {
                        $.messager.alert('提示', obj.Info, 'info');
                        loading.close();
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
}

//新增按钮
function add() {
    location.href = '../StockCheckOver/StockCheckAddOrEdit';
    frxs.updateTabTitle("新增" + stockType, "icon-add");
    $("#btnClose").click(function () {
        frxs.tabClose();
    });
}

//保存按钮
function save() {
    if (Id) {
        editData();
    } else {
        saveData();
    }
}

//编辑按钮
function edit() {
    frxs.updateTabTitle("编辑" + stockType + Id, "icon-edit");
    SetPermission("edit");
}

//确认按钮
function sure() {
    var idStr = "";
    if (Id) {
        idStr = Id;
    } else {
        return;
    }
    $.messager.confirm("提示", "确认该" + stockType + "？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，请稍后...");
            $.ajax({
                url: "/StockAdj/ConfirmStockOverAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 1, adjType: 0 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        if (result.Flag == "SUCCESS") {
                            //window.frameElement.wapi.reload();
                            refreshParentGrid();
                            //frxs.tabClose();
                            init();
                        }
                        $.messager.alert('提示', result.Info, 'info');
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

//反确认按钮
function unsure() {
    var idStr = "";
    if (Id) {
        idStr = Id;
    } else {
        return;
    }
    $.messager.confirm("提示", "反确认该" + stockType + "？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，请稍后...");
            $.ajax({
                url: "/StockAdj/UnConfirmStockOverAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, flag: 0, adjType: 0 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        if (result.Flag == "SUCCESS") {
                            //window.frameElement.wapi.reload();
                            refreshParentGrid();
                            //frxs.tabClose();
                            init();
                        }
                        $.messager.alert('提示', result.Info, 'info');
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

//过账按钮
function posting() {
    var idStr = "";
    if (Id) {
        idStr = Id;
    } else {
        return;
    }
    $.messager.confirm("提示", "过账该" + stockType + "？", function (r) {
        if (r) {
            var loading = frxs.loading("正在保存，当单据的明细数量较多可能需要多一点时间，请稍后...");
            $.ajax({
                url: "/StockAdj/PostingStockOverAdj",
                type: "get",
                dataType: "json",
                data: { ids: idStr, adjType: 0 },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        //alert(result.Info);
                        if (result.Flag == "SUCCESS") {
                            refreshParentGrid();
                            init();
                        }
                        $.messager.alert('提示', result.Info, 'info');
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

}

function refreshParentGrid() {
    frxs.reParentTabGrid("grid");
}

//打印库存盘盈单
function print() {
    var adjId = $("#AdjID").val();
    if (adjId != "") {
        $.ajax({
            url: "../StockAdj/GetStockAdjDetailCount",
            type: "get",
            dataType: "json",
            data: { id: adjId },
            success: function (obj) {
                if (obj != undefined && obj.Info != undefined) {
                    if (obj.Flag == "SUCCESS") {
                        if (obj.Data > 0) {
                            var thisdlg = frxs.dialog({
                                title: "打印库存盘盈单",
                                url: "../FastReportTemplets/Aspx/PrintStockCheckWin.aspx?AdjID=" + adjId,
                                owdoc: window.top,
                                width: 765,
                                height: 600
                            });
                        } else {
                            $.messager.alert('提示', "盘盈明细不能为空！", 'info');
                            loading.close();
                        }
                    } else {
                        $.messager.alert('提示', obj.Info, 'info');
                        loading.close();
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
    } else {
        $.messager.alert("提示", "盘盈单号不能为空！", "info");
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
    $("#gridStockAdjDetail").datagrid({
        height: h2,
    });
    $("#gridGroup").datagrid({
        height: h2,
    });
};


