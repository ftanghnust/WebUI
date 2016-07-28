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
        url: '../Shelf/GetShelfList',          //Aajx地址
        sortName: 'ShelfID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShelfID',                  //主键
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
            //grid高度改变
            gridresize();
        },
        queryParams: {            
            //查询条件
            ShelfCode: $("#ShelfCode").val(),
            ShelfAreaID: $("#ShelfAreaID").combobox("getValue"),
            Status: $("#Status").combobox("getValue")
        },
        onDblClickRow: function (rowIndex) {
            edit();
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true }, //选择
            { title: '货位编号', field: 'ShelfCode', width: 130, align: 'center', formatter: frxs.replaceCode }
            //冻结列
        ]],
        columns: [[
           
            { title: '仓库名称', field: 'WName', width: 260, align: 'center' },
            { title: '货区', field: 'ShelfAreaName', width: 180, align: 'center', formatter: frxs.replaceCode },
            { title: '货位类型', field: 'ShelfType', width: 160, align: 'center' },
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
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


//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {        
         ss.push(rows[i].ShelfID);        
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
  
    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            $.ajax({
                url: "../Shelf/DeleteShelf",
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
        url: "../Shelf/ShelfAddOrEdit",
        owdoc: window.top,
        width: 320,
        height: 320,
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
            url: "../Shelf/ShelfEdit?Id=" + rows[0].ShelfID,
            owdoc: window.top,
            width: 320,
            height: 320,
            buttons: editBtn
        });
    }
}

//查询
function search() {
    initGrid(toolbarArray);
}

function resetSearch() {
    $("#ShelfCode").attr("value","");
    $('#ShelfAreaID').combobox('setValue', '');
    $('#Status').combobox('setValue', '');
}

function initDDL() {
    $.ajax({
        url: '../ShelfArea/GetShelfAreaSelectList',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "ShelfAreaID": "", "ShelfAreaName": "-请选择-" });
            //创建控件
            $("#ShelfAreaID").combobox({
                data: data,                       //数据源
                valueField: "ShelfAreaID",       //id列
                textField: "ShelfAreaName"       //value列
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

//显示导入界面
function showImport() {

    var thisdlg = frxs.dialog({
        title: "货位-Excel数据导入",
        url: "../Shelf/ImportShelfDetail",
        owdoc: window.top,
        width: 820,
        height: 650,
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
