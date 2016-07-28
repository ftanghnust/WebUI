﻿$(function () {
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
        var isValidate = $("form[action='WarehouseMessageHandle']").form("validate");

        if (isValidate) {
            //提交表单
            submitForm("WarehouseMessageHandle", null, null, false, callbackResponse);
        }
        $(this).removeAttr("disabled");
    });
})



function callbackResponse(obj, seq) {
    if (obj.Flag == "SUCCESS") {
       
        window.frameElement.wapi.location.href = window.frameElement.wapi.frameElement.src;
        //window.frameElement.wapi.location.href = window.frameElement.wapi.location.href;
       
        xsjs.pageClose();
    } else {
        alert(obj.Info);
    }
}