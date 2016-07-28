
var productId;

$(function () {

    productId = frxs.getUrlParam("productid");
    //grid绑定
    initGrid();

    //grid高度改变
    gridresize();


});


function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WProductsVendor/GetProductsVendorList?productid=' + productId,          //Aajx地址
        sortName: 'VendorCode',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ID',                  //主键
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: true,                   //列均匀分配
        striped: true,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        singleSelect: true,
        onClickRow: function (rowIndex, rowData) {
        },
        onLoadSuccess: function () {
            //var rows = $("#grid").datagrid("getRows");
            //for (var i = 0; i < rows.length; i++) {
            //    if (rows[i].IsMaster == 1) {
            //        $("#grid").datagrid("selectRow", i);
            //    }
            //}
        },
        queryParams: {
        },
        frozenColumns: [[
            //冻结列
        ]],

        columns: [[
            //{ field: 'ck', checkbox: true }, //选择
            { title: '供应商编号', field: 'VendorCode', width: 100 },
            { title: '供应商名称', field: 'VendorName', width: 260 },
            { title: '供应商类别', field: 'VendorTypeName', width: 120 },
            { title: '供应商联系人', field: 'LinkMan', width: 120 },
            { title: '负责人电话', field: 'Telephone', width: 120 },
            {
                title: '主供应商', field: 'MasterStr', width: 100, formatter: function (value, rowData) {
                    return rowData.IsMaster == 1 ? "是" : "否";
                }
            },
            {
                title: '状态', field: 'StatusStr', width: 60, formatter: function (value, rowData) {
                    return rowData.Status == 1 ? "正常" : "冻结";
                }
            },
        { title: 'VendorID', field: 'VendorID', hidden: true, width: 160 }
        ]],
        toolbar: [
            {
                id: 'btnReload',
                text: '刷新',
                iconCls: 'icon-reload',
                handler: function () {
                    //实现刷新栏目中的数据
                    $("#grid").datagrid("reload");
                }
            }
            //{
            //id: 'btnSetMasterVendor',
            //text: '设置主供应商',
            //iconCls: 'icon-edit',
            //handler: SetMasterVendor}
        ]
    });
}

//设置主供应商
function SetMasterVendor() {
    var row = $('#grid').datagrid('getSelected');
    if (!row) {
        $.messager.alert("提示", "没有选中行数", "info");
    }
    else {
        var data = {
            ProductId: productId,
            VendorID: row.VendorID
        };
        var load = frxs.loading("正在保存...");
        $.ajax({
            url: '../WProductsVendor/SetMasterVendor',
            data: data,
            type: "post",
            success: function (result) {
                load.close();
                var obj = $.parseJSON(result);
                if (obj.Flag == "FAIL") {
                    $.messager.alert("提示", obj.Info, "info");
                } else {
                    $.messager.alert("提示", "设置主供应商成功", "info", function () {
                        //window.frameElement.wapi.$("#grid").datagrid("reload");
                        $("#grid").datagrid("reload");
                    });
                }
            }
        });
    }
}



//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}
