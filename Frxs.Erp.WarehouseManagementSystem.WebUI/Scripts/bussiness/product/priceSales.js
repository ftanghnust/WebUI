$(function () {
    //grid绑定
    initGrid();

    //grid高度改变
    gridresize();

    //查询按钮事件
    $("#aSearch").click(function () {
        search();
    });
    //重置按钮事件
    $("#aReset").click(function () {
        resetSearch();
    });

});

//搜索
function search() {
    //实现刷新栏目中的数据
    initGrid();
}

//初始化查询
function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../Vendor/VendorList',          //Aajx地址
        sortName: 'VendorID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'VendorCode',                  //主键
        pageSize: 50,                       //每页条数
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
        onDblClickCell: function (rowIndex) {
            edit();
        },
        queryParams: {
            //查询条件

        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '积分促销单号', field: 'VendorCode', width: 110, align: 'center', sortable: true },
            { title: '录单时间', field: 'VendorName', width: 360 },
            { title: '活动名称', field: 'LinkMan', width: 100 },
            { title: '开始时间', field: 'Telephone', width: 100 },
            { title: '结束时间', field: 'VendorTypeName', width: 100 },
            { title: '创建人', field: 'SettleTimeTypeName', width: 100 },
            { title: '备注', field: 'SettleTimeTypeName', width: 100 },
            { title: '活动状态', field: 'SettleTimeTypeName', width: 100 },
            {
                title: '单据状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            }
        ]],
        toolbar: [{
            id: 'btnReload',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                //实现刷新栏目中的数据
                $("#grid").datagrid("reload");
            }
        }, '-', {
            text: '查看',
            iconCls: 'icon-search',
            handler: function () {
                frxs.openNewTab("查看价格调整单", "../ProductList/PriceSalesAddOrEdit", "icon-search");
            }
        }, {
            id: 'btnReload',
            text: '新增',
            iconCls: 'icon-add',
            handler: function () {
                frxs.openNewTab("新增价格调整单", "../ProductList/PriceSalesAddOrEdit", "icon-add");
            }
        }, {
            id: 'btnReload',
            text: '编辑',
            iconCls: 'icon-edit',
            handler: function () {
                frxs.openNewTab("编辑价格调整单", "../ProductList/PriceSalesAddOrEdit", "icon-edit");
            }
        },
        {
            id: 'btnReload',
            text: '删除',
            iconCls: 'icon-remove',
            handler: del
        }, '-',
        {
            text: '确认',
            iconCls: 'icon-ok',
            handler: confirmOrder
        },
        {
            text: '反确认',
            iconCls: 'icon-upf',
            handler: noConfirmOrder
        },
        '-', {
            text: '立即开始',
            iconCls: 'icon-dict',
            handler: goEffect
        }, {
            text: '停用',
            iconCls: 'icon-wait',
            handler: goEffect
        }]
    });
}

//确认
function confirmOrder() {

}

//反确认
function noConfirmOrder() {

}

//立即生效
function goEffect() {

}




function del() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        idStr += ("'" + rows[i].VendorID + "',");
    }
    if (confirm("确认删除？")) {
        $.ajax({
            url: "../vendor/Delete",
            type: "get",
            dataType: "json",
            data: {
                ids: idStr
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    alert(result.Info);
                    if (result.Flag == "SUCCESS") {
                        //实现刷新栏目中的数据
                        $("#grid").datagrid("reload");
                    }
                }
            }
        });
    };
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