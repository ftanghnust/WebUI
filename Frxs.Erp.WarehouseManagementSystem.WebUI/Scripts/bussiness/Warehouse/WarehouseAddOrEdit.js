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
    //仓库基本资料编辑权限配置
    if (editPermission == true) {
        //提交按钮事件
        $("#aSubmit").click(function () {
            $(this).attr("disabled", "disabled");
            //easyUI表单校验
            var isValidate = $("form").form("validate");
            if (isValidate) {
                //提交表单
                submitForm("WarehouseHandle", null, null, false, callbackResponse);
            }
            $(this).removeAttr("disabled");
        });
    }
    else {
        $("#aSubmit").hide();
        //$("#aSubmit").attr("title", "您当前没有“编辑”权限!");
    }

    //关闭按钮事件
    $("#cancel").click(function () {
        frxs.tabClose();
    });

    //初始化区域下拉菜单
    region.init({
        ddlProvince: $("#ddlProvince"),
        ddlCity: $("#ddlCity"),
        ddlCounty: $("#ddlCountry"),
        provinceID: $("#ProvinceID").val(),
        cityID: $("#CityID").val(),
        countyID: $("#RegionID").val()
    });

})

//提交后回调
function callbackResponse(obj, seq) {
    if (obj.Flag == "SUCCESS") {
        $.messager.alert('提示', '保存成功', "info");
        if (window != window.top && window.frameElement.wapi) {
            //window.frameElement.wapi.location.href = window.frameElement.wapi.frameElement.src;
            window.frameElement.wapi.location.href = window.frameElement.wapi.location.href;
            frxs.pageClose();
        }
    }
    else {
        var msg = (obj.Info != undefined) ? obj.Info : "操作失败!调用接口失败。";
        $.messager.alert('错误', msg, " info");
    }
}

