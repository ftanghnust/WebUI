var fileInfo = '';


$(function () {

    //上传
    $('#file_upload').uploadify({
        'uploader': '../Scripts/plugin/uploadify/uploadify.swf',
        'script': '../Shelf/UploadFile',
        'folder': '/FileUpload/Upload',
        'cancelImg': '../Scripts/plugin/uploadify/cancel.png',
        'fileExt': '*.xls;',
        'fileDesc': '*.xls;',
        'sizeLimit': 1024 * 1024 * 4,
        'multi': false,
        'onComplete': callback
    });
    initGrid();
});

//插件上传回调
function callback(event, queueId, fileObj, response, data) {

    if (response != "") {
        isValidate(response + fileObj.type);
        
    }
    else {
        $.messager.alert("提示", "上传格式不正确，只能上传后缀为 xls 的EXCEL的文件 ！", "info");
    }
}

function isValidate(response)
{
    $.ajax({
        url: "../Shelf/isValidate",
        type: "get",
        dataType: "json",
        data: {
            fileName: response,
            folderPath: "~/FileUpload/Upload"
        },
        success: function (result) {
            if (result != undefined && result.Info != undefined) {
                $.messager.alert("提示", result.Info, "info");
                if (result.Flag == "SUCCESS") {
                    initGrid(response);
                }
                else {
                    initGrid();
                }
            }
        },
        error: function (request, textStatus, errThrown) {
            if (textStatus) {                
                $.messager.alert("提示", textStatus, "info");
                initGrid();
            } else if (errThrown) {
                $.messager.alert("提示", errThrown, "info");
                initGrid();
            } else {
                $.messager.alert("提示", "出现错误", "info");
                initGrid();
            }
        }
    });
}

//grid高度改变
function initGrid(filePath) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        width: 800,
        height: 430,
        methord: 'get',                    //提交方式
        url: '../Shelf/GetImportData',//Aajx地址
        //sortName: 'ShelfCode',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        //idField: 'ShelfCode',       //主键
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
        queryParams: {
            fileName: filePath,
            folderPath: "~/FileUpload/Upload"
        },
        columns: [[
            { field: 'ck', checkbox: true }, //选择            
            { title: '货区', field: 'ShelfAreaName', width: 100 },
            { title: '货位编号', field: 'ShelfCode', width: 100 },
            { title: '货区ID', field: 'ShelfAreaID', hidden: true, width: 100 }
        ]],
        toolbar: [{
            text: '删除',
            id: 'delDetailsBtn',
            iconCls: 'icon-remove',
            handler: delRow
        }]
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
function saveData() {
    try {
        var rows = $('#grid').datagrid('getRows');
        var jsonStr = "[";
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].ShelfCode == '') {
                $.messager.alert('提示', "货位编号不能为空！", 'info');
                return;
            }
            jsonStr += "{";
            jsonStr += "\"ShelfAreaID\":\"" + rows[i].ShelfAreaID + "\",";
            jsonStr += "\"ShelfCode\":\"" + rows[i].ShelfCode + "\"";

            jsonStr += "},";
        }
        if (jsonStr.length > 1) {
            jsonStr = jsonStr.substring(0, jsonStr.length - 1);
        }
        jsonStr += "]";

        $.ajax({
            url: "../Shelf/ImportShelfHandle",
            type: "post",
            dataType: 'json',
            data: {
                jsonDetails: jsonStr
            },
            success: function (obj) {

                if (obj.Flag != "SUCCESS") {
                    $.messager.alert('提示', obj.Info, 'info');

                } else {
                    $.messager.alert('提示', "保存成功", 'info', function () {
                        window.frameElement.wapi.$("#grid").datagrid("reload");
                        frxs.pageClose();
                    });
                }
            }, error: function (e) {
                $.messager.alert('提示', "操作失败！", 'info');
            }
        });
    }
    catch (e) {
        $.messager.alert('提示', "请导入数据再提交！", 'info');
    }
}



