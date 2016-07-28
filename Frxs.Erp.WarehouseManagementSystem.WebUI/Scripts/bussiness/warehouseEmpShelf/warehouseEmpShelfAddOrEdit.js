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

    $("#bu li").each(function (i,item) {
        $(this).click(function () {
            switch (i) {
                case 0:
                    downSelectedOption();
                    break;
                case 1:
                    endSelectedOption();
                    break;                
                default:

            }
        })
    });

    $('#ShelfAreaID').combobox({
        onChange: function () {
            $.messager.confirm("提示", "选择更改货区后货位将清空，是否确认？", function (r) {
                if (r) {
                    $('#where').empty();
                }
            });
        }
    });


    //提交按钮事件
    $("#aSubmit").click(function () {
        $(this).attr("disabled", "disabled");

        var idlist = "";
        $("#where option").each(function () { //遍历全部option
            var txt = $(this).val(); //获取option的内容
            idlist += txt + ",";
        });
        $("#ShelfIDs").val(idlist);


        submitForm("WarehouseEmpShelfHandle", null, null, false, callbackResponse);        
        $(this).removeAttr("disabled");
    });

    //关闭事件
    $("#cancel").click(function () {
        frxs.pageClose();
    });


    //提交按钮事件
    $("#bSubmit").click(function () {
        var ShelfID = $("select[name='ShelfID']");
       
        $.ajax({
            url: '/WarehouseEmpShelf/GetWarehouseEmpShelfSelectList?ShelfAreaID=' + $('#ShelfAreaID').combobox("getValue") + "&ShelfCodeStart=" + $("#ShelfCodeStart").val() + "&ShelfCodeEnd=" + $("#ShelfCodeEnd").val(),
            type: 'get',
            success: function (obj) {
                var list = JSON.parse(obj);
                var j=true;
                //ShelfID.empty();
                for (var i = 0; i < list.Data.length; i++) {
                    var j=true;
                    $("#where option").each(function () { //遍历全部option
                        var txt = $(this).val(); //获取option的内容
                        if (list.Data[i].ShelfID == txt)
                        {
                            j=false;
                        }
                    });
                    if (j) {
                        ShelfID.append("<option value='" + list.Data[i].ShelfID + "'>" + list.Data[i].ShelfCode + "</option>");
                    }
                }
            },
            error: function () {
            }
        });
    });
  
})



function endSelectedOption() {
    $('#where').empty();
}

/** 
* 向下移动选中的option 
*/
function downSelectedOption() {
    if (null == $('#where').val()) {
        $.messager.alert("提示", "请选择一项", "info");
        return false;
    }
   
    $('#where option:selected').remove();
    
}

function saveData() {

    $(this).attr("disabled", "disabled");

    var idlist = "";
    $("#where option").each(function () { //遍历全部option
        var txt = $(this).val(); //获取option的内容
        idlist += txt + ",";
    });
    $("#ShelfIDs").val(idlist);


    submitForm("WarehouseEmpShelfHandle", null, null, false, callbackResponse);
    $(this).removeAttr("disabled");
}

function callbackResponse(obj, seq) {
    if (obj.Flag == "SUCCESS") {
       
        window.frameElement.wapi.location.href = window.frameElement.wapi.frameElement.src;
        //window.frameElement.wapi.location.href = window.frameElement.wapi.location.href;
        window.frameElement.wapi.focus();
        frxs.pageClose();
    } else {
        window.top.$.messager.alert("提示", obj.Info, "info");

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