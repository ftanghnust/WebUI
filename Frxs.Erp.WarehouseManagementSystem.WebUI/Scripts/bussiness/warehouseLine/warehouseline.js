$(function () {
  
    //grid绑定
    initGrid(toolbarArray);

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();
    //select下拉框自适应高度    
    $('.easyui-combobox').combobox({
        panelHeight: 'auto'
    });
});

function initGrid(toolbarArray) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WarehouseLine/GetWarehouseLineList',          //Aajx地址
        sortName: 'LineID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'LineID',                  //主键
        pageSize: 50,                       //每页条数
        pageList: [10,30,50,100],//可以设置每页记录条数的列表 
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
            LineName: $("#LineName").val(),
            EmpName: $("#EmpName").val(),
            UserMobile: $("#UserMobile").val(),
            SendW: $("#SendW").combobox("getValue")
        },
        onDblClickRow: function (rowIndex) {
            edit();
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '配送线路编号', field: 'LineCode', width: 130, align: 'center' },
            { title: '配送线路名称', field: 'LineName', width: 130, align: 'left', sortable: true, formatter: frxs.replaceCode },
            { title: '配送负责人', field: 'EmpName', width: 160, align: 'center' },
            { title: '负责人电话', field: 'UserMobile', width: 180, align: 'center' },
            { title: '配送门店数', field: 'ShopNum', width: 160, align: 'center' },
            { title: '配送周期', field: 'SendW', width: 160, align: 'left' },
            { title: '排序号', field: 'SerialNumber', width: 160, align: 'center' }

        ]],
        toolbar: toolbarArray
    });
}


//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {        
        ss.push(rows[i].LineID);
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
  
    
    $.messager.confirm("提示", "确认删除？", function (r) {
        if(r) {

            $.ajax({
                url: "../WarehouseLine/DeleteWarehouseLine",
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
        url: "../WarehouseLine/WarehouseLineAddOrEdit",
        owdoc: window.top,
        width: 400,
        height: 440,
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
    var editBtn = new Array();
    if (quickEditKey == true) {//有修改权限
        editBtn = [{
            id: 'warehouseEditBtn',
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
    }
    else {//无修改权限，只显示关闭按键
        editBtn = [{
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
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
            url: "../WarehouseLine/WarehouseLineAddOrEdit?Id=" + rows[0].LineID,
            owdoc: window.top,
            width: 400,
            height: 440,
            buttons: editBtn
        });
    }
}

//编辑门店顺序
function order() {
    
    
   
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        var thisdlg = frxs.dialog({
            title: "编辑门店顺序",
            url: "../WarehouseLineShop/WarehouseLineShopAddOrEdit?Id=" + rows[0].LineID,
            owdoc: window.top,
            width: 400,
            height: 700,
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
    $("#LineName").attr("value", "");
    $("#EmpName").attr("value", "");
    $("#UserMobile").attr("value", "");
    $('#SendW').combobox('setValue', '');
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