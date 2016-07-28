$(function () {
    $('.easyui-combobox').combobox({

        panelHeight: 'auto'

    })
 
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
        var isValidate = $("form[action='ShelfHandle']").form("validate");

        if (isValidate) {
            //提交表单
            submitForm("ShelfHandle", null, null, false, callbackResponse);
        }
        $(this).removeAttr("disabled");
    });

    //关闭事件
    $("#cancel").click(function () {
        frxs.pageClose();
    });
})
//取消按钮事件
//$("#cancel").click(function () {
//    $(this).parents(window).hide();

//})
var loading;
function saveData()
{
    if ($(".messager-window").length > 0) {
        return false;
    }
    //debugger;
    $(this).attr("disabled", "disabled");
     //loading = window.top.frxs.loading();
    //easyUI表单校验
    var isValidate = $("form[action='ShelfHandle']").form("validate");

    if (isValidate) {
        //提交表单
        window.frameElement.wapi.focus();
        submitForm("ShelfHandle", null, null, false, callbackResponse);
    }
    
    $(this).removeAttr("disabled");
}

function callbackResponse(obj, seq) {

    if (obj.Flag == "SUCCESS") {       
        window.frameElement.wapi.$("#grid").datagrid("reload");
       
        frxs.pageClose();
    } else {
       window.top.$.messager.alert("提示", obj.Info, "info");
        //loading.close();

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