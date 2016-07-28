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
        url: '../WarehouseEmp/GetWarehouseEmpList',          //Aajx地址
        sortName: 'EmpID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'EmpID',                  //主键
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
            EmpName: $("#EmpName").val(),
            UserAccount: $("#UserAccount").val(),
            UserType: $("#UserType").combobox("getValue"),
            IsFrozen: $("#IsFrozen").combobox("getValue")
        },
        frozenColumns: [[
            //冻结列
        ]],
        onDblClickRow: function (rowIndex) {
            edit();
        },
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '所属机构', field: 'WName', width: 130, align: 'center', sortable: true },
            { title: '用户名称', field: 'EmpName', width: 260, align: 'left', formatter: frxs.replaceCode },
            { title: '帐户', field: 'UserAccount', width: 180, align: 'left', formatter: frxs.replaceCode },
            { title: '职位', field: 'UserTypeStr', width: 160, align: 'left' },
            {
                title: '状态', field: 'IsFrozen', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "1") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            }

        ]],
        toolbar: toolbarArray
    });
}

function IsFrozen() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');

    var message="";

    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].EmpID);
        if (rows[i].IsFrozen == 1)
        {
            message = rows[i].EmpName + "用户为冻结状态，请重新选择！";
        }
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    if (message != "")
    {
        $.messager.alert("提示", message, "info");
        return false;
    }

    
    $.messager.confirm("提示", "确认冻结吗？", function (r ) {
        if(r) {

            $.ajax({
                url: "../WarehouseEmp/IsFrozenWarehouseEmp",
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
            });}
    });
}

function ResetPassword() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        $.messager.confirm("提示", "是否重置密码，确认密码将重置为初始密码？", function (r) {
            if(r) {
       
                $.ajax({
                    url: "../WarehouseEmp/ResetPasswordWarehouseEmp",
                    type: "get",
                    dataType: "json",
                    data: {
                        id: rows[0].EmpID
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
                });}
        });
    }
}


function Frozen() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    var message = "";
    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].EmpID);
        if (rows[i].IsFrozen == 0) {
            message = rows[i].EmpName + "用户为正常状态，请重新选择！";
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
    
    $.messager.confirm("提示", "确定解冻吗？", function (r) {
        if (r) {

            $.ajax({
                url: "../WarehouseEmp/FrozenWarehouseEmp",
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
    for (var i = 0; i < rows.length; i++) {        
        ss.push(rows[i].EmpID);
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
  
    $.messager.confirm("提示", "确认删除？", function (r) {
        if(r) {

            $.ajax({
                url: "../WarehouseEmp/DeleteWarehouseEmp",
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
            });}
    });

}

//新增按钮事件
function add() {
    var thisdlg = frxs.dialog({
        title: "新增",
        url: "../WarehouseEmp/WarehouseEmpAddOrEdit",
        owdoc: window.top,
        width: 400,
        height: 350,
        buttons: [{
            text: '<div title="【Alt+S】">提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                $("#grid").datagrid("reload");
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

//编辑按钮事件
function edit() {
    var editBtn = new Array();
   
    if (quickEditKey == true) {//有修改权限
        editBtn = [{
            id: 'warehouseEditBtn',
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
    }
    else {//无修改权限，只显示关闭按键
        editBtn = [{
            text: '<div title="【ESC】">关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    }
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {       
        var thisdlg = frxs.dialog({
            title: "编辑",
            url: "../WarehouseEmp/WarehouseEmpEdit?Id=" + rows[0].EmpID,
            owdoc: window.top,
            width: 400,
            height:350,
            buttons: editBtn
        });
    }
}

//查询
function search() {
    initGrid(toolbarArray);
}

function resetSearch() {
    $("#EmpName").attr("value", "");
    $("#UserAccount").attr("value", "");
    $('#UserType').combobox('setValue', '');
    $('#IsFrozen').combobox('setValue', '');
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