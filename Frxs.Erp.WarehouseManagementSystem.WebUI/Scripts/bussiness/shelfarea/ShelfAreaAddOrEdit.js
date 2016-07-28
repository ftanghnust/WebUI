$(function () {
    $("input.textbox-text").addClass('jump');
    $('input:first').focus();//定义第一个input获取焦点
    var $inp = $('.jump');       //定义要依次跳转的IPNUT 都给一个.jump样式
    $inp.bind('keydown', function (e) {
        var key = e.which;
        if (key == 13) {
            e.preventDefault();
            var nxtIdx = $inp.index(this) + 1;
            $(".jump:eq(" + nxtIdx + ")").focus();
        }
    });


    //提交按钮事件
    $("#aSubmit").click(function () {
        $(this).attr("disabled", "disabled");

        //easyUI表单校验
        var isValidate = $("form[action='ShelfAreaHandle']").form("validate");

        if (isValidate) {
            //提交表单
            submitForm("ShelfAreaHandle", null, null, false, callbackResponse);
        }
        $(this).removeAttr("disabled");
    });
    //关闭事件
    $("#cancel").click(function () {
        frxs.pageClose();
    });
})
function saveData() {

    $(this).attr("disabled", "disabled");

    //easyUI表单校验
    var isValidate = $("form[action='ShelfAreaHandle']").form("validate");

    if (isValidate) {
        //提交表单
        submitForm("ShelfAreaHandle", null, null, false, callbackResponse);
    }
    $(this).removeAttr("disabled");
}

function callbackResponse(obj, seq) {
    if (obj.Flag == "SUCCESS") {
       
        window.frameElement.wapi.$("#grid").datagrid("reload");
        //window.frameElement.wapi.location.href = window.frameElement.wapi.location.href;
        window.frameElement.wapi.focus();
        frxs.pageClose();
    } else {
        window.top.$.messager.alert("提示", obj.Info, "info");
        // alert(obj.Info);
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