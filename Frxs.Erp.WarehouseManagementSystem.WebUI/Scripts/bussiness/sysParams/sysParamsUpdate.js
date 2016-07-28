

//保存数据
function saveData() {
    var validate = $("#formAdd").form('validate');
    var paramCode = $("#ParamCode").val();
    if (paramCode == "008") {
        var paramValue = $("#ParamValue").val();
        
        if (paramValue.indexOf(":") < 0 ||
            paramValue.length != 5 ||
            isNaN(paramValue.split(":")[0]) ||
            parseInt(paramValue.split(":")[0]) > 23 ||
            isNaN(paramValue.split(":")[1]) ||
            parseInt(paramValue.split(":")[1]) > 59) {
            parent.$.messager.alert("提示", "参数值设定不正确", "info");
            validate = false;
        } else {
            validate = true;
        }
    }
    
    if (validate == false) {
        return false;
    } else {
        var data = $("#formAdd").serialize();
        $.ajax({
            url: "../sysParams/update",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                if (obj.Flag == "ERROR") {
                    $.messager.alert("提示", obj.Info, "info");
                } else {
                    //$("#grid").datagrid("reload");
                    window.frameElement.wapi.reload();
                    window.frameElement.wapi.focus();
                    frxs.pageClose();
                }
            }
        });
    }
}
//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData();//弹窗提交

    }
    else if (e.keyCode == 27) {

        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭


    }
});
window.focus();
