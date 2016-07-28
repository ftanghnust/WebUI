var dialogWidth = 400;
var dialogHeight = 200;

$(function () {

    //grid绑定
    //initGrid();

    //下拉绑定
    //initDDL();

    //grid高度改变
    //gridresize();
});

function initGrid(vartoolbar) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../SysParams/GetList',          //Aajx地址
        sortName: 'VendorTypeID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'VendorTypeID',                  //主键
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

            var rows = $("#grid").datagrid("getRows");
            for (var i = 0; i < rows.length; i++) {
                var index = $('#grid').datagrid('getRowIndex', rows[i]);
                rows[i].ParamValue = rows[i].ParamValue;
                //更新行
                $('#grid').datagrid('updateRow', {
                    index: index,
                    row: rows[i]
                });
            }
        },
        queryParams: {

        },
        onDblClickRow: function (index, rowData) {
            var thisdlg = frxs.dialog({
                title: "编辑",
                url: "../SysParams/Update?paramCode=" + rowData.ParamCode,
                owdoc: window.top,
                width: dialogWidth,
                height: dialogHeight,
                buttons: [{
                    text: '<div title=【Alt+S】>提交</div>',
                    iconCls: 'icon-ok',
                    handler: function () {
                        thisdlg.subpage.saveData();
                    }
                }, {
                    text: '<div title=【ESC】>关闭</div>',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        thisdlg.dialog("close");
                    }
                }]
            });
        },
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }, //选择
            { title: '参数编码', field: 'ParamCode', width: 100, hidden: false, align: 'center' },
        ]],
        columns: [[
            
            { title: '参数名称', field: 'ParamName', width: 200, align: 'left' },
            { title: '参数值', field: 'ParamValue', width: 120, align: 'right' }, 
            { title: '创建时间', field: 'CreateTime', width: 200, align: 'center' },
            { title: '创建用户', field: 'CreateUserName', width: 200, align: 'center' },
            { title: '备注 ', field: 'Remark', width: 260, align: 'left' },
        ]],
        toolbar: vartoolbar
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
            url: "../SysParams/Update?paramCode=" + rows[0].ParamCode,
            owdoc: window.top,
            width: dialogWidth,
            height: dialogHeight,
            buttons: [{
                text: '<div title=【Alt+S】>提交</div>',
                iconCls: 'icon-ok',
                handler: function () {
                    thisdlg.subpage.saveData();
                }
            }, {
                text: '<div title=【ESC】>关闭</div>',
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
    var h = ($(window).height()-5);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}