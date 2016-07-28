var fileInfo = '';


$(function () {

    //初始化禁用上传按钮
    $("#btnUpload").linkbutton('disable');
    //上传
    $('#file_upload').uploadify({
        'uploader': '../Scripts/plugin/uploadify/uploadify.swf',
        'script': '../WProductAdjShelf/UploadExcel',
        'folder': '/FileUpload/Upload',
        'cancelImg': '../Scripts/plugin/uploadify/cancel.png',
        'fileExt': '*.xls;',
        'fileDesc': '*.xls;',
        'sizeLimit': 1024 * 1024 * 4,
        'multi': false,
        'onComplete': fun,
        'onSelect': function () {
            //选择文件后启用
            $("#btnUpload").linkbutton('enable');
        },
        'onCancel': function () {
            //禁用上传按钮
            $("#btnUpload").linkbutton('disable');
        }
    });

    initGrid();

});

function fun(event, queueID, fileObj, response, data) {

    if (response != "") {
        initGuid(response);
        initGridData();
        showInfo("成功上传!", true);
    }
    else {
        showInfo("文件上传出错,请检查文件类型及内容格式！", false);
    }
}

//grid高度改变

function initGridData() {

    var load = frxs.loading("正在处理,请稍候...");
    $.ajax({
        url: '../WProductAdjShelf/GetImportData',//Aajx地址
        type: "post",
        data: {
            ImportGuid: $("#WProductAdjShelf_GUID").val(),
            folder: "~/FileUpload/Upload"
        },
        success: function (result) {
            load.close();

            if ($.parseJSON(result).Flag == "FAIL") {
                showInfo($.parseJSON(result).Info, false);
                //有错误禁用上传按钮
                $("#btnUpload").linkbutton('disable');
            } else {

                var datajson = {};
                datajson.total = $.parseJSON(result).total;
                datajson.rows = $.parseJSON($.parseJSON(result).rows);
                $('#importGrid').datagrid({ data: datajson });
            }
        }
    });
}

function initGrid() {
    $('#importGrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        width: 800,
        height: 430,
        methord: 'get',                    //提交方式
        //  url: '../WProductAdjShelf/GetImportData',//Aajx地址
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
        onClickRow: function (rowIndex, rowData) {
            $('#importGrid').datagrid('clearSelections');
            $('#importGrid').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            ImportGuid: $("#WProductAdjShelf_GUID").val(),
            folder: "~/FileUpload/Upload"
        },
        columns: [[
             { field: 'ck', checkbox: true }, //选择
             { title: '序号', field: 'ID', width: 100, align: 'left' },
          { title: 'WID', field: 'WID', hidden: true },
            { title: 'WProductID', field: 'WProductID', hidden: true },
            { title: 'ProductId', field: 'ProductId', hidden: true },
            { title: '商品编码', field: 'SKU', width: 100, editor: 'text' },
            {
                title: '商品名称', field: 'ProductName', width: 200, formatter: function (value) {
                    if (value == "") {
                        return "<div style='width:60px; height:20px;background:red'></div>"
                    }
                    return value;
                }
            },
            { title: '商品条码', field: 'BarCode', width: 100 },
            { title: '配送单位', field: 'Unit', width: 100 },
            { title: '包装数', field: 'BigPackingQty', width: 100 },
            { title: 'OldShelfID', field: 'OldShelfID', hidden: true },
            { title: '原货位号', field: 'OldShelfCode', width: 100 },
            {
                title: 'ShelfID', field: 'ShelfID', hidden: 100, formatter: function (value) {
                    if (value == "") {
                        return "<div style='width:60px; height:20px;background:red'></div>";
                    }
                    return value;
                }
            },
            { title: '新货位号', field: 'ShelfCode', width: 100, editor: 'text' },
            { title: '备注', field: 'Remark', width: 260,formatter:frxs.formatText }
        ]],
        toolbar: [{
            text: '删除',
            id: 'delDetailsBtn',
            iconCls: 'icon-remove',
            handler: delRow
        }]
    });
}

function delRow() {

    var is = "";
    $(".datagrid-body:first table tr").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            is = i + "," + is;
        }
    });

    if (is == "") {
        $.messager.alert('提示', "请选择一条记录！", 'info');
        return false;
    }
    var rows = $('#importGrid').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < is.split(',').length; j++) {
        var ci = is.split(',')[j];
        if (ci) {
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#importGrid').datagrid('getRowIndex', copyRows[i]);
        $('#importGrid').datagrid('deleteRow', ind);
    }

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

///获取GUID
function initGuid(tempGuid) {

    $("#WProductAdjShelf_GUID").val(tempGuid);

}

//保存
function saveData() {

    var rows = $('#importGrid').datagrid('getRows');

    if (rows.length <= 0) {
        parent.$.messager.alert('提示', "导入的数据行不能为空！", 'info');
        return false;
    }
    for (var i = 0; i < rows.length; i++) {


        if (rows[i].WProductID == '') {
            parent.$.messager.alert('提示', "序号：" + rows[i].ID + "  SKU:" + rows[i].SKU + "有误，请检查！", 'info');
            return false;
        }
        if (rows[i].ShelfID == '') {
            parent.$.messager.alert('提示', "序号：" + rows[i].ID + "  货位编码:" + rows[i].ShelfCode + "有误，请检查！", 'info');
            return false;
        }
        
        var msg = "";
        //验证重复数据
        var curI = $("#importGrid").datagrid('getRowIndex', rows[i]);
        for (var j = 0; j < rows.length; j++) {
            var curJ = $("#importGrid").datagrid('getRowIndex', rows[j]);
            if (curJ != curI) {
                //商品编码不能重复
                if (rows[i].SKU == rows[j].SKU) {
                    msg = "序号 " + rows[i].ID + " 与序号 " + rows[j].ID + " 的商品编码重复";
                    parent.$.messager.alert("提示", msg, "info");
                    return false;
                }
                //序号不能重复
                if (rows[i].ID == rows[j].ID) {
                    msg = "Excel出现重复的序号 " + rows[j].ID;
                    parent.$.messager.alert("提示", msg, "info");
                    return false;
                }
            }
        }
    }
    
    if (window.frameElement.wapi.backFillShelfImport(rows)) {
        frxs.pageClose();
    }
}

