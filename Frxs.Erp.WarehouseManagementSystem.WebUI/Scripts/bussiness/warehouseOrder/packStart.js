
var orderid;
var shpoid;

$(function () {
    orderid = frxs.getUrlParam("OrderID");
    shpoid = frxs.getUrlParam("ShopID");

    initDDL();

    $('.try').numberbox('limit', { lengths: 3 });
});


//保存
function saveData() {
    var packEmpId = $("#PackName").combobox('getValue');
    var packEmpName = $("#PackName").combobox('getText');
    if (packEmpId == "") {
        window.top.$.messager.alert("提示", "请选择装箱员！", "info");
        return false;
    }
    $(this).attr("disabled", "disabled");
    var validate = $("#packform").form('validate');
    if (!validate) {
        return false;
    }
    window.parent.$(window.frameElement).closest(".window").find("#btnPackOk").addClass("easyui-linkbutton").linkbutton('disable');
    var loading = frxs.loading("正在加载中，请稍后...");
    $.ajax({
        url: "../WarehouseOrder/PackStartHandle",
        type: "post",
        dataType: "json",
        data: {
            OrderID: orderid,
            ShopID: shpoid,
            Package1Qty: $("#Package1Qty").val(),
            Package2Qty: $("#Package2Qty").val(),
            Package3Qty: $("#Package3Qty").val(),
            Remark: $("#Remark").val(),
            PackingEmpID: packEmpId,
            PackingEmpName: packEmpName
        },
        success: function (result) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnPackOk").addClass("easyui-linkbutton").linkbutton('enable');
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    window.top.$.messager.alert("提示", "装箱成功", "info", function () {
                        window.frameElement.wapi.$("#grid").datagrid("reload");
                        window.frameElement.wapi.focus();
                        frxs.pageClose();
                    });
                } else {
                    parent.$("#btnPackOk").linkbutton('enable');
                    window.top.$.messager.alert("提示", result.Info, "info");
                }
            }
        },
        error: function (request, textStatus, errThrown) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnPackOk").addClass("easyui-linkbutton").linkbutton('enable');
            parent.$("#btnPackOk").linkbutton('enable');
            if (textStatus) {
                window.top.$.messager.alert("提示", textStatus, "info");
            } else if (errThrown) {
                window.top.$.messager.alert("提示", errThrown, "info");
            } else {
                window.top.$.messager.alert("提示", "出现错误", "info");
            }
        }
    });

    $(this).removeAttr("disabled");
}

//下拉控件数据初始化
function initDDL() {
    $.ajax({
        url: '../WarehouseOrder/GetZXEmpList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            data.unshift({ "EmpID": "", "EmpName": "-请选择-" });
            //创建控件
            $("#PackName").combobox({
                data: data,                //数据源
                valueField: "EmpID",       //id列
                textField: "EmpName"       //value列
            });
            $("#PackName").combobox('select', data[0].EmpID);
        }, error: function (e) {
            debugger;
        }
    });
}