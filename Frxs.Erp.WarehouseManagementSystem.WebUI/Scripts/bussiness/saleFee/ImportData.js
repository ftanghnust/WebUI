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

function initGridData() {

    var load = frxs.loading("正在处理,请稍候...");
    $.ajax({
        url: '../SaleFee/GetImportData',//Aajx地址
        type: "post",
        data: {
            ImportGuid: $("#SaleFee_GUID").val(),
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
//grid高度改变

function initGrid() {
    $('#importGrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        width: 800,
        height: 430,
        methord: 'get',                    //提交方式
        //   url: '../SaleFee/GetImportData',//Aajx地址
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
            ImportGuid: $("#SaleFee_GUID").val(),
            folder: "~/FileUpload/Upload"
        },
        columns: [[
         { field: 'ck', checkbox: true }, //选择
        { title: '序号', field: 'ID', width: 100, align: 'left' },
         { title: 'WID', field: 'WID', hidden: true },
        { title: '门店ID', field: 'ShopID', hidden: true },
        { title: '门店编号', field: 'ShopCode', width: 200, align: 'left' },
        {
            title: '门店名称', field: 'ShopName', width: 200, align: 'left', formatter: function (value) {
                if (value == "") {
                    return "<div style='width:60px; height:20px;background:red'></div>"
                }
                return value;
            }
        },
        { title: '项目名称', field: 'FeeName', width: 200, align: 'left' }, {
            title: '项目编码', field: 'FeeCode', width: 200, align: 'left', formatter: function (value) {
                if (value == "") {
                    return "<div style='width:60px; height:20px;background:red'></div>"
                }
                return value;
            }
        },
          { title: 'SettleID', field: 'SettleID', hidden: true },
            { title: 'SettleTime', field: 'SettleTime', hidden: true },
            { title: 'SerialNumber', field: 'SerialNumber', hidden: true },
            { title: 'OrderId', field: 'OrderId', hidden: true },
        { title: '金额（元）', field: 'FeeAmt', width: 200, align: 'left' },
        { title: '备注', field: 'Reason', width: 200, align: 'left', formatter: frxs.formatText }
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

    $("#SaleFee_GUID").val(tempGuid);

}

//保存
function saveData() {
    
    var rows = $('#importGrid').datagrid('getRows');

    if (rows.length <= 0) {
        window.top.$.messager.alert('提示', "导入的数据行不能为空！", 'info');
        return;
    }

    for (var i = 0; i < rows.length; i++) {


        if (rows[i].FeeAmt == '') {
            window.top.$.messager.alert('提示', "明细费用不能为空！", 'info');
            return;
        }
        if (rows[i].FeeAmt == 0) {
            window.top.$.messager.alert('提示', "单条明细金额不能为0！", 'info');
            return;
        }
        if ( rows[i].Reason!=null && rows[i].Reason.length > 250) {
            window.top.$.messager.alert('提示', "备注不能大于250个字符！", 'info');
            return;
        }

        if (rows[i].FeeAmt > 100000000) {
            window.top.$.messager.alert('提示', "明细金额不能大于100000000！", 'info');
            return;
        }
    }
    if (window.frameElement.wapi.backFillShopImport(rows)) {
        frxs.pageClose();
    }

}
