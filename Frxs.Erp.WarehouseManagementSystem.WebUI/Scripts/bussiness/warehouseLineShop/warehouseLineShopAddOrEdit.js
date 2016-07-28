$(function () {
    $('.easyui-combobox').combobox({

        panelHeight: 'auto'

    })
    GetWarehouseLineShopList();
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

    $("#bu li").each(function (i,item) {
        $(this).click(function () {
            switch (i) {
                case 0:
                    topSelectedOption();
                    break;
                case 1:
                    upSelectedOption();
                    break;
                case 2:
                    downSelectedOption();
                    break;
                case 3:
                    endSelectedOption();
                    break;
                default:

            }
        })
    });

    //提交按钮事件
    $("#aSubmit").click(function () {
        $(this).attr("disabled", "disabled");
        var idlist = "";
        $("#where option").each(function () { //遍历全部option
            var txt = $(this).val(); //获取option的内容
            idlist += txt + ",";
        });
        $("#idList").val(idlist);
        submitForm("warehouseLineShopHandle", null, null, false, callbackResponse);        
        $(this).removeAttr("disabled");
    });

  
})

/** 
* 向上移动选中的option 
*/
function upSelectedOption() {
    if (null == $('#where').val()) {
        $.messager.alert("提示", "请选择一项", "info");
        return false;
    }
    //选中的索引,从0开始 
    var optionIndex = $('#where').get(0).selectedIndex;
    //如果选中的不在最上面,表示可以移动 
    if (optionIndex > 0) {
        $('#where option:selected').insertBefore($('#where option:selected').prev('option'));
    }
}

function topSelectedOption() {
    if (null == $('#where').val()) {
        $.messager.alert("提示", "请选择一项", "info");
        return false;
    }
    //选中的索引,从0开始 
    var optionIndex = $('#where').get(0).selectedIndex;
    //如果选中的不在最上面,表示可以移动 
    if (optionIndex > 0) {
        $('#where option:selected').insertBefore($('#where option').eq(0));
    }
}

function endSelectedOption() {
    if (null == $('#where').val()) {
        $.messager.alert("提示", "请选择一项", "info");
       
        return false;
    }
    //索引的长度,从1开始 
    var optionLength = $('#where')[0].options.length;
    //选中的索引,从0开始 
    var optionIndex = $('#where').get(0).selectedIndex;
    //如果选中的不在最上面,表示可以移动 
    if (optionIndex < (optionLength - 1)) {
        $('#where option:selected').appendTo($('#where'));
    }
}

/** 
* 向下移动选中的option 
*/
function downSelectedOption() {
    if (null == $('#where').val()) {
        $.messager.alert("提示", "请选择一项", "info");
        return false;
    }
    //索引的长度,从1开始 
    var optionLength = $('#where')[0].options.length;
    //选中的索引,从0开始 
    var optionIndex = $('#where').get(0).selectedIndex;
    //如果选择的不在最下面,表示可以向下 
    if (optionIndex < (optionLength - 1)) {
        $('#where option:selected').insertAfter($('#where option:selected').next('option'));
    }
}

function callbackResponse(obj, seq) {
    if (obj.Flag == "SUCCESS") {       
        frxs.pageClose();
    } else {
        alert(obj.Info);
    }
}

function saveData() {

    $(this).attr("disabled", "disabled");
    var idlist = "";
    $("#where option").each(function () { //遍历全部option
        var txt = $(this).val(); //获取option的内容
        idlist += txt + ",";
    });
    $("#idList").val(idlist);
    submitForm("warehouseLineShopHandle", null, null, false, callbackResponse);
    $(this).removeAttr("disabled");
}

//获取门店
function GetWarehouseLineShopList() {
    var ShelfAreaID = $("select[name='where']");   
    $.ajax({
        url: '/WarehouseLineShop/GetWarehouseLineShopSelectList?id=' + $("#LineID").val(),
        type: 'get',
        success: function (obj) {
            var list = JSON.parse(obj);
            ShelfAreaID.empty();            
            for (var i = 0; i < list.length; i++) {
                ShelfAreaID.append("<option value='" + list[i].ID + "'>" + list[i].ShopName + "</option>");
            }
        },
        error: function () {
        }
    });
}