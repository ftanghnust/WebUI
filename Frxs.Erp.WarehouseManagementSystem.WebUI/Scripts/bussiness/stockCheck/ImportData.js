var fileInfo = '';
var loading;
var adjType;
var refreshSeconds;//刷新读秒显示

$(function () {
    adjType = frxs.getUrlParam("adjType");
    //alert(adjType);
    if (adjType == 0) {
        $('#aTemplate').attr('href', '/FileUpload/盘盈明细导入模板.xls');
    } else if (adjType == 1) {
        $('#aTemplate').attr('href', '/FileUpload/盘亏明细导入模板.xls');
    }
    //上传
    $('#file_upload').uploadify({
        'uploader': '../Scripts/plugin/uploadify/uploadify.swf',
        'script': '../StockAdjDetail/UploadStockAdjDetail',
        'folder': '/FileUpload/Upload',
        'cancelImg': '../Scripts/plugin/uploadify/cancel.png',
        'fileExt': '*.xls;*.xlsx;',
        'fileDesc': '*.xls;*.xlsx;',
        'sizeLimit': 1024 * 1024 * 4,
        'multi': false,
        'onComplete': fun
    });
    //gridresize();
});

function fun(event, queueID, fileObj, response, data) {
    var data = $.parseJSON(response);
    loading.close();
    if (data.Flag == "SUCCESS") {
        //showInfo("成功上传!", true);
        window.top.$.messager.alert("提示", data.Info, "info");
        window.frameElement.wapi.reloadStockAdjDetail();
        frxs.pageClose();
    } else {
        //showInfo(data.Info, false);
        var msg = (data != null && data != undefined && data.Info != undefined) ? data.Info : "调用接口失败!";
        //window.top.$.messager.alert("提示", msg, "info");//用于显示汇总的错误信息
        //2016-6-15 当后台给出的错误提示信息太多时，自定义弹窗，允许出现滚动条
        var objDialog = window.top.$("<div style='height: 300px; width: 360px; overflow: auto;'>" + msg + "</div>").dialog({
            title: "温馨提示",
            height: 300,
            buttons: [{
                text: '确定',
                iconCls: 'icon-ok',
                handler: function () {
                    objDialog.dialog("close");
                }
            }]
        });
        frxs.pageClose();
    }
}

//显示提示信息，textstyle2为绿色，即正确信息；textstyl1为红色，即错误信息
function showInfo(msg, type) {
    var msgClass = type == true ? "textstyle2" : "textstyle1";
    $("#result").removeClass();
    $("#result").addClass(msgClass);
    $("#result").html(msg);
    loading.close();
    clearInterval(refreshSeconds);
}

//如果点击‘导入文件’时选择文件为空，则提示
function checkImport() {
    if ($.trim($('#file_uploadQueue').html()) == "") {
        //window.top.$.messager.alert('提示', "请先选择要导入的文件！", 'info');
        showInfo("请先选择要导入的文件！", false);
        return false;
    }
    return true;
}

function saveData(adjID, btnObj) {
    if (checkImport()) {
        var msg = " 正在导入明细数据，若数据量大可能需要长一点时间，请稍后...";
        window.top.$(btnObj).linkbutton("disable");//禁用提交按键，防止重复提交
        loading = frxs.loading(msg);
        var i = 0;
        refreshSeconds = window.setInterval(function () {
            loading.close();
            i += 1;
            loading = frxs.loading(i + msg);
        }, 1000);//刷新时间,按秒计数
        $('#file_upload').uploadifySettings('scriptData', { 'adjID': adjID });
        $('#file_upload').uploadifyUpload();
    }
}

