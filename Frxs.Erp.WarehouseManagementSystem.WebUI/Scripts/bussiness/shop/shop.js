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
        url: '../Shop/GetShopList',          //Aajx地址
        sortName: 'ShopID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopID',                  //主键
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
            ShopAccount: $("#ShopAccount").val(),
            LinkMan: $("#LinkMan").val(),
            LineID: $("#LineID").combobox("getValue"),
            Status: $("#Status").combobox("getValue")
        },
        onDblClickRow: function (rowIndex) {
            edit();
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '门店编号', field: 'ShopCode', width: 100, align: 'center', sortable: true }
            //冻结列
        ]],
        columns: [[
           
            { title: '门店名称', field: 'ShopName', width: 210, align: 'left', formatter: frxs.replaceCode },
            { title: '门店账号', field: 'ShopAccount', width: 120, align: 'center', formatter: frxs.replaceCode },
            { title: '联系人', field: 'LinkMan', width: 100, formatter: frxs.replaceCode, align: 'center' },
            { title: '联系电话', field: 'Telephone', width: 130, align: 'center' },
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            },
            { title: '送货线路', field: 'LineName', width:240,  formatter: frxs.replaceCode },
            { title: '发货排序', field: 'SerialNumberStr', width: 80, align: 'center' },
            { title: '地址全称', field: 'FullAddress', width: 360, align: 'left', formatter: frxs.replaceCode }
            
        ]],
        toolbar: toolbarArray
    });
}

//查看按钮事件
function view() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        var thisdlg = frxs.dialog({
            title: "查看",
            url: "../Shop/ShopView?Id=" + rows[0].ShopID,
            owdoc: window.top,
            width: 750,
            height: 650,
            buttons: [{
                text: '关闭',
                iconCls: 'icon-cancel',
                handler: function () {
                    thisdlg.dialog("close");
                }
            }]
        });
    }
}

function IsFrozen() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    var message = "";
    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].ShopID);
        if (rows[i].Status == 0) {
            message = rows[i].ShopName + "门店为冻结状态，请重新选择！";
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

    $.messager.confirm("提示", "确认冻结？", function (r) {
        if(r) {
            $.ajax({
                url: "../Shop/IsFrozenShop",
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


function Frozen() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    var message = "";
    for (var i = 0; i < rows.length; i++) {
        ss.push(rows[i].ShopID);
        if (rows[i].Status == 1) {
            message = rows[i].ShopName + "门店为正常状态，请重新选择！";
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
    $.messager.confirm("提示", "确认解冻？", function (r) {
        if(r) {
            $.ajax({
                url: "../Shop/FrozenShop",
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

//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {        
         ss.push(rows[i].ShopID);        
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
  
    if (confirm("确认删除吗？")) {
        $.ajax({
            url: "../Shop/DeleteShop",
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
    };

}

//新增按钮事件
function add() {
    var thisdlg = frxs.dialog({
        title: "新增",
        url: "../Shop/ShopAddOrEdit",
        owdoc: window.top,
        width: 850,
        height: 650,
        buttons: [{
            text: '<div title="【Ａlt+S】">提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                debugger;
                //$("#grid").datagrid("reload");
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
    if (quickEditKey == true) {
        var rows = $('#grid').datagrid('getSelections');
        if (rows.length > 1) {
            $.messager.alert("提示", "只能选中一条！", "info");
        } else if (rows.length == 0) {
            $.messager.alert("提示", "没有选中记录！", "info");
        } else {
            var thisdlg = frxs.dialog({
                title: "编辑",
                url: "../Shop/ShopAddOrEdit?Id=" + rows[0].ShopID,
                owdoc: window.top,
                width: 750,
                height: 650,
                buttons: [{
                    text: '<div title="【Ａlt+S】">提交</div>',
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
    }
    else {
        var rows = $('#grid').datagrid('getSelections');
        if (rows.length > 1) {
            $.messager.alert("提示", "只能选中一条！", "info");
        } else if (rows.length == 0) {
            $.messager.alert("提示", "没有选中记录！", "info");
        } else {
            var thisdlg = frxs.dialog({
                title: "查看",
                url: "../Shop/ShopView?Id=" + rows[0].ShopID,
                owdoc: window.top,
                width: 750,
                height: 650,
                buttons: [{
                    text: '关闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        thisdlg.dialog("close");
                    }
                }]
            });
        }
    }
}

//查询
function search() {
    initGrid(toolbarArray);
}

function resetSearch() {
    $("#ShopCode").attr("value", "");
    $("#ShopName").attr("value", "");
    $("#ShopAccount").attr("value", "");
    $("#LinkMan").attr("value", "");

    $('#LineID').combobox('setValue', '');
    $('#Status').combobox('setValue', '');
}

function initDDL() {
    $.ajax({
        url: '../Common/GetWarehouseLineList',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "LineID": "", "LineName": "-请选择-" });
            //创建控件
            $("#LineID").combobox({
                data: data,                       //数据源
                valueField: "LineID",       //id列
                textField: "LineName"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
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


function ResetPassword() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else if (rows[0].ShopAccount=="")
    {
        $.messager.alert("提示", "该门店没有账号信息，请重新选择", "info");
        }else {
        $.messager.confirm("提示", "是否重置密码，确认密码将重置为初始密码？", function (r) {
            if (r) {

                $.ajax({
                    url: "../Shop/ResetPasswordShop",
                    type: "get",
                    dataType: "json",
                    data: {
                        id: rows[0].ShopAccount
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
}