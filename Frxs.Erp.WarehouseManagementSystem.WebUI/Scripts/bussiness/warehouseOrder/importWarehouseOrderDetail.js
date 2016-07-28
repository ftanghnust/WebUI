var fileInfo = '';

var shopId;
var subWID;
$(function () {
    //初始化禁用上传按钮
    $("#btnUpload").linkbutton('disable');

    shopId = frxs.getUrlParam("ShopID");
    subWID = frxs.getUrlParam("SubWID");

    //上传
    $('#file_upload').uploadify({
        'uploader': '../Scripts/plugin/uploadify/uploadify.swf',
        'script': '../SaleFee/UploadExcel',
        'folder': '/FileUpload/Upload',
        'cancelImg': '../Scripts/plugin/uploadify/cancel.png',
        'fileExt': '*.xls;',
        'fileDesc': '*.xls;',
        'sizeLimit': 1024 * 1024 * 4,
        'multi': false,
        'onComplete': callback,
        'onSelect': function () {
            //选择文件后启用
            $("#btnUpload").linkbutton('enable');
        },
        'onCancel': function () {
            //禁用上传按钮
            $("#btnUpload").linkbutton('disable');
        }
    });

    gridload();
});

//插件上传回调
function callback(event, queueId, fileObj, response, data) {
    if (fileObj.type.toLowerCase() == ".xls") {
        if (response != "") {
            initGrid(response + fileObj.type.toLowerCase());
            showInfo("成功上传!", true);
        }
        else {
            showInfo("文件上传出错！", false);
        }
    } else {
        showInfo("上传格式不正确，只能上传后缀为 xls 的EXCEL的文件 ！", false);
    }
}

//grid高度改变
function gridload() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        width: 800,
        height: 430,
        methord: 'get',                    //提交方式
        //url: '../BuyOrderPre/GetImportData',//Aajx地址
        sortName: 'ID',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'ID',       //主键
        pageSize: 5000,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
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
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '序号', field: 'Index', width: 40, align: 'left' },
            { title: '商品编码', field: 'SKU', width: 100 },
            { title: '商品名称', field: 'ProductName', width: 180, formatter: frxs.replaceCode },
            { title: '单位', field: 'SaleUnit', align: 'center', width: 80 },
            {
                title: '订货数量', field: 'PreQty', width: 60, align: 'right', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(2) : value;
                }
            },
            {
                title: '销售数量', field: 'SaleQty', width: 60, align: 'right', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(2) : value;
                }
            },
            {
                title: '配送价格', field: 'SalePrice', align: 'right', width: 70, formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4) : value;
                }
            },
            { title: '平台费率', field: 'ShopAddPerc', width: 70, align: 'center' },
            {
                title: '小计金额', field: 'SubAmt', align: 'right', width: 70, formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4) : value;
                }
            },
            {
                title: '包装数', field: 'SalePackingQty', align: 'right', width: 60, formatter: function (value) {
                    return value ? parseFloat(value).toFixed(2) : value;
                }
            },
            {
                title: '总数量', field: 'UnitQty', align: 'right', width: 60, formatter: function (value) {
                    return value ? parseFloat(value).toFixed(2) : value;
                }
            },
            { title: '国际条码', field: 'BarCode', width: 110 },
            { title: '库存数量', field: 'Stock', align: 'right', width: 60 },
            { title: '在途数量', field: 'OnTheWay', width: 80, align: 'right' },
            { title: '可用数量', field: 'Available', width: 80, align: 'right' },
            { title: '备注', field: 'Remark', width: 100, editor: 'text', formatter: frxs.replaceCode },
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 100 },   //商品ID

            { title: 'MaxOrderQty', field: 'MaxOrderQty', hidden: true, width: 100 },
            { title: 'IsAppend', field: 'IsAppend', hidden: true, width: 100 },
            { title: 'BasePoint', field: 'BasePoint', hidden: true, width: 100 },
            { title: 'VendorPerc1', field: 'VendorPerc1', hidden: true, width: 100 },
            { title: 'VendorPerc2', field: 'VendorPerc2', hidden: true, width: 100 },
            
            { title: 'Unit', field: 'Unit', hidden: true, width: 80 },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 80 },    //商品最小单位价格

            { title: 'BigSalePrice', field: 'BigSalePrice', hidden: true }
            
        ]],
        toolbar: [{
            text: '删除',
            id: 'delDetailsBtn',
            iconCls: 'icon-remove',
            handler: delRow
        }]
    });
}


//grid高度改变
function initGrid(filePath) {
    var load = frxs.loading("正在处理,请稍候...");
    $.ajax({
        url: "../WarehouseOrder/GetImportData",
        type: "post",
        data: {
            fileName: filePath,
            folderPath: "~/FileUpload/Upload",
            shopid: shopId,
            subWId: subWID
        },
        success: function (result) {
            load.close();
            if ($.parseJSON(result).Flag == "FAIL") {
                showInfo($.parseJSON(result).Info, false);
                //有错误禁用上传按钮
                $("#btnUpload").linkbutton('disable');
            } else {
                $('#grid').datagrid({ data: $.parseJSON(result) });
            }
        }
    });
}

//显示提示信息，textstyle2为绿色，即正确信息；textstyl1为红色，即错误信息
function showInfo(msg, type) {
    var msgClass = type == true ? "textstyle2" : "textstyle1";
    $("#result").removeClass();
    $("#result").addClass(msgClass);
    $("#result").html(msg);
}

//如果点击‘导入文件’时选择文件为空，则提示
function checkImport() {
    if ($.trim($('#file_uploadQueue').html()) == "") {
        $.messager.alert('提示', "请先选择要导入的文件！", 'info');
        return false;
    }
    return true;
}

function delRow() {

    var is = "";
    $(".datagrid-body:first table tr").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            is = i + "," + is;
        }
    });

    var rows = $('#grid').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < is.split(',').length; j++) {
        var ci = is.split(',')[j];
        if (ci) {
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#grid').datagrid('getRowIndex', copyRows[i]);
        $('#grid').datagrid('deleteRow', ind);
    }

}

//保存
function importData() {
    var msg = "";
    var rows = $('#grid').datagrid('getRows');
    if (rows.length == 0) {
        msg = "导入的数据行不能为空";
    }


    for (var i = 0; i < rows.length; i++) {
        if (!rows[i].SKU) {
            msg = "序号 " + rows[i].Index + " 的商品编码不能为空";
            break;
        }

        //验证重复数据
        var curI = $("#grid").datagrid('getRowIndex', rows[i]);
        for (var j = 0; j < rows.length; j++) {
            var curJ = $("#grid").datagrid('getRowIndex', rows[j]);
            if (curJ != curI) {
                //商品编码不能重复
                if (rows[i].SKU == rows[j].SKU) {
                    msg = "序号第 " + rows[i].Index + " 行与序号第 " + rows[j].Index + " 行商品编码重复";
                    window.top.$.messager.alert("提示", msg, "info");
                    return false;
                }
                //序号不能重复
                if (rows[i].Index == rows[j].Index) {
                    msg = "Excel出现重复的序号 " + rows[j].Index;
                    window.top.$.messager.alert("提示", msg, "info");
                    return false;
                }
            }
        }


    }
    if (msg) {
        window.top.$.messager.alert("提示", msg, "info");
    } else {
        if (window.frameElement.wapi.backFillProductImport(rows)) {
            frxs.pageClose();
        }
    }
}


