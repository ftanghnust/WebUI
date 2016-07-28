$(function () {
    //grid绑定
    initGrid(toolbarArray);

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();
});

function initGrid(toolbarArray) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WStationNumber/GetWStationNumberList',          //Aajx地址
        //sortName: 'ID',                 //排序字段
        //sortOrder: 'asc',                  //排序方式
        idField: 'ID',                  //主键
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
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        queryParams: {            
            //查询条件
            ShopCode: $("#ShopCode").val(),
            ShopName: $("#ShopName").val(),
            StationNumber: $("#StationNumber").val(),
            Status: $("#Status").combobox("getValue"),
            OrderStatus: $("#OrderStatus").combobox("getValue")
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '订单编号', field: 'OrderID', width: 140, formatter: frxs.replaceCode, align: 'center' },
            { title: '待装区编号', field: 'StationNumber', width: 100, align: 'center' }
            //冻结列
        ]],        
        columns: [[
           
            {
                title: '配送日期', field: 'OrderConfDate', width: 150, formatter: frxs.replaceCode, align: 'center', formatter: function (value, rec) {
                    if (value) {
                        return value.DataFormat("yyyy-MM-dd");
                    }
                }
            },
             {
                 title: '订单状态', field: 'OrderStatus', width: 100, align: 'center', formatter: function (value, rec) {
                     if (value == "3") {
                         return "正在拣货";
                     }
                     else if (value == "4") {
                         return "拣货完成";
                     }
                     else if (value == "5") {
                         return "等待配送";
                     }                    
                     else {
                         return "";
                     }
                 }
             },
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "2") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else if (value == "0") {
                        return "空闲";
                    }
                    else {
                        return "正在使用";
                    }
                }
            },
            { title: '门店编号', field: 'ShopCode', width: 140, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 200, },
            
            
            { title: '配送线路', field: 'LineName', width: 200,  },
            { title: '配送员', field: 'EmpName', width: 160, align: 'center' }
           

        ]],
        toolbar: toolbarArray
    });
}

function IsFrozen() {
    var ss = [];
    var message = "";
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].ID);
        if (rows[i].Status == 2) {
            message = rows[i].StationNumber + "待装区为冻结状态，请重新选择！";
        }
        if (rows[i].Status == 1) {
            message = rows[i].StationNumber + "待装区为正在使用状态，请重新选择！";
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (message != "") {
        $.messager.alert("提示", message, "info");
        return false;
    }
    $.messager.confirm("提示", "确认冻结吗?", function (r) {
        if (r) {
            $.ajax({
                url: "../WStationNumber/IsFrozenWStationNumber",
                type: "get",
                dataType: "json",
                data: {
                    ids: ss.join(',')
                },
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
                        $.messager.alert("提示", textStatus, "info");
                    } else if (errThrown) {
                        $.messager.alert("提示", errThrown, "info");
                    } else {
                        $.messager.alert("提示", "出现错误", "info");
                    }
                }
            });
        }
    });
}

function ResetPassword() {
    var ss = [];
   
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].ID);
       
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
   
    $.messager.confirm("提示", "确认清空吗?", function (r) {
        if (r) {
            $.ajax({
                url: "../WStationNumber/ResetWStationNumber",
                type: "get",
                dataType: "json",
                data: {
                    ids: ss.join(',')
                },
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
                        $.messager.alert("提示", textStatus, "info");
                    } else if (errThrown) {
                        $.messager.alert("提示", errThrown, "info");
                    } else {
                        $.messager.alert("提示", "出现错误", "info");
                    }
                }
            });
        }
        });
}


function Frozen() {
    var ss = [];
    var message = "";
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].ID);
        if (rows[i].Status == 0) {
            message = rows[i].StationNumber + "待装区为空闲状态，请重新选择！";
        }
        if (rows[i].Status == 1) {
            message = rows[i].StationNumber + "待装区为正在使用状态，请重新选择！";
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (message != "") {
        $.messager.alert("提示", message, "info");
        return false;
    }
    $.messager.confirm("提示", "确认解冻吗?", function (r) {
        if (r) {
            $.ajax({
                url: "../WStationNumber/FrozenWStationNumber",
                type: "get",
                dataType: "json",
                data: {
                    ids: ss.join(',')
                },
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
                        $.messager.alert("提示", textStatus, "info");
                    } else if (errThrown) {
                        $.messager.alert("提示", errThrown, "info");
                    } else {
                        $.messager.alert("提示", "出现错误", "info");
                    }
                }
            });
        }
        });
}


//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    var message = "";
    for (var i = 0; i < rows.length; i++) {        
        ss.push(rows[i].ID);        
        if (rows[i].Status == 1) {
            message = rows[i].StationNumber + "待装区为正在使用状态，请重新选择！";
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (message != "") {
        $.messager.alert("提示", message, "info");
        return false;
    }
  
    $.messager.confirm("提示", "确认删除吗?", function (r) {
        if (r) {
            $.ajax({
                url: "../WStationNumber/DeleteWStationNumber",
                type: "get",
                dataType: "json",
                data: {
                    ids: ss.join(',')
                },
                success: function (result) {
                    if (result != undefined && result.Info != undefined) {
                        $.messager.alert("提示", result.Info, "info");
                        if (result.Flag == "SUCCESS") {
                            $("#grid").datagrid("reload");
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
    });

}

//新增按钮事件
function add() {
    var thisdlg = frxs.dialog({
        title: "新增",
        url: "../WStationNumber/AddWStationNumberList",
        owdoc: window.top,
        width: 300,
        height: 180,
        buttons: [{
            text: '提交',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                $("#grid").datagrid("reload");
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

//编辑按钮事件
function edit() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {       
        var thisdlg = frxs.dialog({
            title: "编辑",
            url: "../WStationNumber/WStationNumberAddOrEdit?Id=" + rows[0].EmpID,
            owdoc: window.top,
            width: 330,
            height: 280,
            buttons: [{
                text: '提交',
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
}

//查询
function search() {
    initGrid(toolbarArray);
}

function resetSearch() {
    $("#ShopCode").attr("value", "");
    $("#ShopName").attr("value", "");
    $("#StationNumber").attr("value", "");
    $('#Status').combobox('setValue', '');
    $('#OrderStatus').combobox('setValue', '');  
}

function initDDL() {
  
}



//窗口大小改变
$(window).resize(function () {
    gridresize();
});


//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height()-21);
    $('#grid').datagrid('resize', {
        width:$(window).width()-10,
        height: h
    });
}