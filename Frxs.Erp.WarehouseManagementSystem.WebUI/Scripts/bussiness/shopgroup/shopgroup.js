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
        url: '../ShopGroup/GetShopGroupList',          //Aajx地址
        sortName: 'GroupID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'GroupID',                  //主键
        pageSize: 30,                       //每页条数
        pageList: [10,30, 50, 100],//可以设置每页记录条数的列表 
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
        onDblClickRow: function (rowIndex) {
            edit();
        },
        queryParams: {            
            //查询条件
            GroupCode: $("#GroupCode").val(),
            GroupName: $("#GroupName").val()
        },
        frozenColumns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '群组编号', field: 'GroupCode', width: 130, align: 'center', sortable: true },
            //冻结列
        ]],
        columns: [[
            
            { title: '群组名称', field: 'GroupName', width: 240, align: 'center', formatter: frxs.replaceCode },
            { title: '门店数量', field: 'ShopNum', width: 160, align: 'center' },
            { title: '创建用户名称', field: 'CreateUserName', width: 160, align: 'center' },
            { title: '创建时间', field: 'CreateTime', width: 160, align: 'center' } ,
        { title: '备注', field: 'Remark', width: 230,  formatter: frxs.replaceCode }
        ]],
        toolbar: toolbarArray
    });
}


//删除按钮事件
function del() {
    var ss = [];
    var rows = $('#grid').datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {        
        ss.push(rows[i].GroupID);
    }
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
  


        $.messager.confirm("提示", "确认删除？", function (r) {
            if (r) {
                $.ajax({
                    url: "../ShopGroup/DeleteShopGroup",
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
    frxs.openNewTab("添加门店群组", "../ShopGroup/ShopGroupNewAddOrEdit", "icon-add", window);
}

//编辑按钮事件
function edit() {
    if (quickEditKey == true) {//有修改权限
        var rows = $('#grid').datagrid('getSelections');
        if (rows.length > 1) {
            $.messager.alert("提示", "只能选中一条！", "info");
        } else if (rows.length == 0) {
            $.messager.alert("提示", "没有选中记录！", "info");
        } else {
            frxs.openNewTab("修改门店群组", "../ShopGroup/ShopGroupNewEdit?ID=" + rows[0].GroupID, "icon-edit", window);
        }
    }
}

//查看按钮事件
function view() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        frxs.openNewTab("查看门店群组", "../ShopGroup/ShopGroupShow?ID=" + rows[0].GroupID, "icon-edit", window);
    }
}

//查询
function search() {
    initGrid();
}

function resetSearch() {
    $("#GroupCode").attr("value", "");
    $("#GroupName").attr("value", "");   
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