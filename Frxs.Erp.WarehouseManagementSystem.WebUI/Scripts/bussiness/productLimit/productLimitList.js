var dialogWidth = 850;
var dialogHeight = 600;

$(function () {
    init();

    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

    //document.onkeydown = function (e) {
    //    if (!e) e = window.event;//火狐中是 window.event
    //    if ((e.keyCode || e.which) == 13) {
    //        $("#aSearch").click();
    //    }
    //}
});


function init() {
    $("#searchform").submit(function () {
        initGrid();
        return false;
    });
    //$("#aSearch").click(function () {
    //    initGrid();
    //});
    //重置按钮事件
    $("#aReset").click(function () {
        $("#searchform").form("clear");
        $("#Status").combobox("setValue", '');
    });
}

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../ProductLimit/GetProductLimitList',          //Aajx地址
        //sortName: 'CreateTime',                 //排序字段
        //sortOrder: 'desc',                  //排序方式
        idField: 'NoSaleID',                  //主键
        pageSize: 30,                       //每页条数
        pageList: [10, 30, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function (index, rowData) {
            frxs.openNewTab("查看商品限购" + rowData.NoSaleID, "../ProductLimit/ProductLimitAddOrEdit?view=1&Id=" + rowData.NoSaleID, "icon-search", window);
        },
        queryParams: {
            //查询条件
            NoSaleID: $.trim($("#NoSaleID").val()),
            ProductName: $.trim($("#ProductName").val()),
            SKU: $.trim($("#SKU").val()),
            BarCode: $.trim($("#BarCode").val()),
            Status: $("#Status").combobox("getValue"),
            BeginTimeFrom: $("#BeginTime").val(),
            //BeginTimeEnd: $("#EndTime").val() != "" ? $("#EndTime").val() + " 23:59:59" : "",
            BeginTimeEnd: $("#EndTime").val(),
            PromotionName: $.trim($("#PromotionName").val())
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '限购单号', field: 'NoSaleID', width: 105, sortable: true, hidden: false, align: 'center' }
            //冻结列
        ]],
        columns: [[
           
            { title: '生效时间', field: 'BeginTime', width: 110, align: 'center', formatter: frxs.dateFormat },
            { title: '活动名称', field: 'PromotionName', width: 150, formatter: frxs.formatText },
             {
                 title: '状态', field: 'Status', width: 60,align:'center', formatter: function (value, rec) {
                     if (value == "0") {
                         return "录单";
                     }
                     else if (value == "1") {
                         return "确认";
                     }
                     else if (value == "2") {
                         return "已过账";
                     }
                     else if (value == "3") {
                         return "已停用";
                     } else {
                         return value;
                     }
                 }
             },
            
            { title: '录单人员', field: 'CreateUserName', width: 100, align: 'center' },
            { title: '录单时间', field: 'CreateTime', width: 140, align: 'center' },
            { title: '确认人员', field: 'ConfUserName', width: 100, align: 'center' },
            { title: '确认时间', field: 'ConfTime', width: 140, align: 'center' },
            { title: '过账人员', field: 'PostingUserName', width: 100, align: 'center' },
            { title: '过账时间', field: 'PostingTime', width: 140, align: 'center' },
            { title: '备注', field: 'Remark', width: 150, formatter: frxs.formatText }
        ]],
        toolbar: vartoolbar
        
    });
}

function viewCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    } else {
        view(rows[0].NoSaleID);
        //$("#grid").datagrid('unselectAll');
    }
}

function view(id) {
    frxs.openNewTab("查看商品限购" + id, "../ProductLimit/ProductLimitAddOrEdit?view=1&Id=" + id, "icon-search", window);
}

function editCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    } else {
        if (rows[0].Status != "0") {
            $.messager.alert("提示", "只能编辑录单状态的限购单！", "info");
            return;
        }
        edit(rows[0].NoSaleID);
    }
}

function edit(id) {
    frxs.openNewTab("编辑商品限购" + id, "../ProductLimit/ProductLimitAddOrEdit?Id=" + id, "icon-edit", window);
}

function delCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "0") {
            $.messager.alert("提示", "非录单状态不能删除！", "info");
            return;
        } else {
            idStr += ("" + rows[i].NoSaleID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    }
    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            $.ajax({
                url: "../ProductLimit/DeleteProductLimit",
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
                            $("#grid").datagrid("reload");
                            $("#grid").datagrid('clearSelections');
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
    })
}

function confirmCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "0") {
            //$.messager.alert("提示", "只能确认录单状态的限购单！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].NoSaleID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能确认录单状态的限购单！", "info");
        return;
    }
    //alert(idStr);
   

        $.messager.confirm("提示", "确认该限购单？", function (r) {
            if (r) {
                $.ajax({
                    url: "../ProductLimit/ConfirmProductLimit",
                    type: "get",
                    dataType: "json",
                    data: { ids: idStr, flag: 1 },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            $.messager.alert("提示", result.Info, "info");
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
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
    })
}

function unconfirmCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "1") {
            //$.messager.alert("提示", "只能反确认已确认状态的限购单！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].NoSaleID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能反确认确认状态的限购单！", "info");
        return;
    }
    
        $.messager.confirm("提示", "反确认该限购单？", function (r) {
            if (r) {
                $.ajax({
                    url: "../ProductLimit/UnConfirmProductLimit",
                    type: "get",
                    dataType: "json",
                    data: { ids: idStr, flag: 0 },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            $.messager.alert("提示", result.Info, "info");
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
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
    })
}

function postingCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "1") {
            //$.messager.alert("提示", "只能过账已确认状态的限购单！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].NoSaleID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能过账确认状态的限购单！", "info");
        return;
    }
    
        $.messager.confirm("提示", "过账该限购单？", function (r) {
            if (r) {
                $.ajax({
                    url: "../ProductLimit/PostingProductLimit",
                    type: "get",
                    dataType: "json",
                    data: { ids: idStr },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            $.messager.alert("提示", result.Info, "info");
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $('#grid').datagrid('clearSelections');
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
    })
}

function stopCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != "2") {
            //$.messager.alert("提示", "只能停用已过账状态的限购单！", "info");
            //return;
        } else {
            idStr += ("" + rows[i].NoSaleID + ",");
        }
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    } else {
        $.messager.alert("提示", "只能停用已过账状态的限购单！", "info");
        return;
    }
    $.messager.confirm("提示", "停用该限购单？", function (r) {
        if (r) {
            $.ajax({
                url: "../ProductLimit/StopProductLimit",
                type: "get",
                dataType: "json",
                data: { ids: idStr },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            $("#grid").datagrid("reload");
                            $('#grid').datagrid('clearSelections');
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

function initDDL() {
    
}

function add() {
    frxs.openNewTab("新增商品限购", "../ProductLimit/ProductLimitAddOrEdit", "icon-add", window);
}

function reload() {
    $("#grid").datagrid("reload");
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 22);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}