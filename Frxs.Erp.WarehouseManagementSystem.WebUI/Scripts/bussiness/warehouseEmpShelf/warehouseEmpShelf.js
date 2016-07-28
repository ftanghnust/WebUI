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
        url: '../WarehouseEmpShelf/GetWarehouseEmpShelfList',          //Aajx地址
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
        onDblClickRow: function () {
            edit();
        },
        queryParams: {            
            //查询条件
            EmpName: $("#EmpName").val(),
            UserAccount: $("#UserAccount").val(),
            ShelfAreaID: $("#ShelfAreaID").combobox("getValue"),
            IsFrozen: $("#IsFrozen").combobox("getValue")
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '所属机构', field: 'WName', width: 130, align: 'center', sortable: true },
            { title: '用户名称', field: 'EmpName', width: 260, align: 'center', formatter: frxs.replaceCode },
            { title: '帐户', field: 'UserAccount', width: 180, align: 'center', formatter: frxs.replaceCode },            
            {
                title: '状态', field: 'StatusStr', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "冻结") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            },
            { title: '货区名称', field: 'ShelfAreaName', width: 180, align: 'center' },
            { title: '货位数量', field: 'ShelfNum', width: 160, align: 'center' }

        ]],
        toolbar: toolbarArray
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
            url: "../WarehouseEmpShelf/WarehouseEmpShelfEdit?Id=" + rows[0].EmpID + "&name=" + rows[0].EmpName + "&userAccount=" + rows[0].UserAccount,
            owdoc: window.top,
            width: 460,
            height: 600,
            buttons: editBtn
        });
    }
}

//查询
function search() {
    initGrid();
}

function resetSearch() {
    $("#EmpName").attr("value", "");
    $("#UserAccount").attr("value", "");
    $('#ShelfAreaID').combobox('setValue', '');
    $('#IsFrozen').combobox('setValue', '');
}

function initDDL() {
    $.ajax({
        url: '../ShelfArea/GetShelfAreaSelectList',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            
            if (data.length > 0) {
                data.unshift({ "ShelfAreaID": "", "ShelfAreaName": "-请选择-" });
            }

            var shelData = new Array();
            for (var i = 0; i < data.length;i++) {
                var item = new Object();
                item.ShelfAreaID = data[i].ShelfAreaID;
                item.ShelfAreaName = frxs.replaceCode(data[i].ShelfAreaName);
                shelData[i] = item;
            }

            //创建控件
            $("#ShelfAreaID").combobox({
                data: shelData,                       //数据源
                valueField: "ShelfAreaID",       //id列
                textField: "ShelfAreaName"       //value列
            });
            $("#ShelfAreaID").combobox('select', data[0].ShelfAreaID);

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