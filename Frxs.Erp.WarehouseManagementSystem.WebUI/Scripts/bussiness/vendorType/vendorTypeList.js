var dialogWidth = 500;
var dialogHeight = 200;

$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();
});

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../VendorType/GetVendorTypeList',          //Aajx地址
        sortName: 'VendorTypeID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'VendorTypeID',                  //主键
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
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        queryParams: {

        },
        onDblClickRow: function (index, rowData) {
            var thisdlg = frxs.dialog({
                title: "编辑",
                url: "../VendorType/VendorTypeAddOrEdit?Id=" + rowData.VendorTypeID,
                owdoc: window.top,
                width: 380,
                height: 260,
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
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'VendorTypeID', field: 'VendorTypeID', width: 130, hidden: true },
            { title: '供应商分类名称', field: 'VendorTypeName', width: 200, align: 'left' },
            { title: '创建时间', field: 'CreateTime', width: 200, align: 'center' },
            { title: '修改时间', field: 'ModifyTime', width: 200, align: 'center' }
        ]],
        toolbar: [{
            id: 'btnReload',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                //实现刷新栏目中的数据
                $("#grid").datagrid("reload");
            }
        }, '-',
        {
            id: 'btnReload',
            text: '新增',
            iconCls: 'icon-add',
            handler: add
        },
        {
            id: 'btnReload',
            text: '删除',
            iconCls: 'icon-remove',
            handler: del
        }, {
            id: 'btnReload',
            text: '编辑',
            iconCls: 'icon-edit',
            handler: edit
        }]
    });
}


//删除按钮事件
function del() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        idStr += ("" + rows[i].VendorTypeID + ",");
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    }
    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            $.ajax({
                url: "../VendorType/DeleteVendorType",
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
    });
}

//新增按钮事件
function add() {
    var thisdlg = frxs.dialog({
        title: "新增",
        url: "../VendorType/VendorTypeAddOrEdit",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
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
            url: "../VendorType/VendorTypeAddOrEdit?Id=" + rows[0].VendorTypeID,
            owdoc: window.top,
            width: dialogWidth,
            height: dialogHeight,
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
    initGrid();
}

function resetSearch() {
    
}

function initDDL() {

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
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}