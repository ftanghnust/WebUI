var fileInfo = '';


$(function () {
    //初始化禁用上传按钮
    $("#btnUpload").linkbutton('disable');
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
            initGrid(response + fileObj.type);
            showInfo("成功上传!", true);
        }
        else {
            showInfo("文件上传出错！", false);
        }
    } else {
        showInfo("上传格式不正确，只能上传后缀为 xls 的EXCEL的文件 ！", false);
    }
}

function gridload() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        width: 800,
        height: 430,
        methord: 'get',                    //提交方式
        //url: '../Again/GetImportData',//Aajx地址
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
            {
                title: '序号', field: 'Index', width: 80, align: 'left'
            },
            {
                title: '商品编码', field: 'SKU', width: 100, formatter: function (value) {
                    if (!value) {
                        return "<div style='width:100%;background:red'>&nbsp;</div>";
                    }
                    return value;
                }
            },
            {
                title: '商品名称', field: 'ProductName', width: 180, formatter: function (value) {
                    if (!value) {
                        return "<div style='width:100%;background:red'>&nbsp;</div>";
                    }
                    return value;
                }
            },
            { title: '单位', field: 'Unit', width: 80, align: 'center' },
            {
                title: '补货数量', field: 'AppQty', width: 80, align: 'center', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            { title: '补货价格', field: 'NewPrice', width: 80,formatter:function (value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '国际条码', field: 'BarCode', width: 120 },
            { title: 'WStock', field: 'WStock', hidden: true, width: 100 },
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 100 },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 100 },
            { title: 'OldBuyPrice', field: 'OldBuyPrice', hidden: true, width: 100 },

            { title: 'MinUnit', field: 'MinUnit', hidden: true, width: 100 },
            { title: 'BuyPrice', field: 'BuyPrice', hidden: true, width: 100 }

        ]],
        toolbar: [{
            text: '删除',
            id: 'delDetailsBtn',
            iconCls: 'icon-remove',
            handler: delRow
        }]
    });
}


function initGrid(filePath) {
    var load = frxs.loading("正在处理,请稍候...");
    $.ajax({
        url: "../Again/GetImportBuyRepairData",
        type: "post",
        data: {
            fileName: filePath,
            folderPath: "~/FileUpload/Upload",
            type: 1,
            subWId: window.frameElement.wapi.$("#SubWID").combobox('getValue')        //父窗口子仓库ID
        },
        success: function (result) {
            load.close();
            if ($.parseJSON(result).Flag == "FAIL") {
                showInfo($.parseJSON(result).Info, false);
                //有错误警用上传按钮
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

    if ($('.datagrid').length == 0) {
        $.messager.alert("提示", "没有需要导入的数据行，请先上传数据", "info");
        return false;
    }
    var rows = $('#grid').datagrid('getRows');

    if (rows.length == 0) {
        msg = "导入的数据行不能为空";
    }

    for (var i = 0; i < rows.length; i++) {
        if (!rows[i].SKU) {
            msg = "序号 " + rows[i].Index + " 的商品编码不能为空";
            break;
        }
        if (!rows[i].AppQty || isNaN(rows[i].AppQty)) {
            msg = "序号 " + rows[i].Index + " 的补货数量不正确";
            break;
        }
        //验证重复数据
        var curI = $("#grid").datagrid('getRowIndex', rows[i]);
        for (var j = 0; j < rows.length; j++) {
            var curJ = $("#grid").datagrid('getRowIndex', rows[j]);
            if (curJ != curI) {
                if (rows[i].SKU == rows[j].SKU) {
                    msg = "序号 " + rows[i].Index + " 与序号 " + rows[j].Index + " 的商品编码重复";
                    $.messager.alert("提示", msg, "info");
                    return false;
                }
                //序号不能重复
                if (rows[i].Index == rows[j].Index) {
                    msg = "Excel出现重复的序号 " + rows[j].Index;
                    $.messager.alert("提示", msg, "info");
                    return false;
                }
            }
        }
    }
    if (msg) {
        $.messager.alert("提示", msg, "info");
    } else {
        if (window.frameElement.wapi.backFillProductImport(rows)) {
            frxs.pageClose();
        }
    }
}


