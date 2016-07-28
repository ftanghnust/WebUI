
var lineName;
var lineID;
var orderid;

$(function () {
    lineName = frxs.getUrlParam("LineName");
    lineID = frxs.getUrlParam("LineID");
    orderid = frxs.getUrlParam("OrderID");

    $("#currentLine").text(lineName);

    queryLineInfo("");
});

//
function queryLineInfo(linename) {
    $.ajax({
        url: '../WarehouseOrder/GetWarehouseLineList',
        type: 'get',
        data: { LineName: linename },
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            var lineNameList = $("#lineNameList");
            lineNameList.empty();
            for (var i = 0; i < data.rows.length; i++) {
                lineNameList.append("<option value=\"" + data.rows[i].LineID + "\" title=\"" + data.rows[i].LineName + "\">" + data.rows[i].LineName + "</option>");
            }
        }, error: function (e) {
            debugger;
        }
    });
}

//搜索
function search() {
    queryLineInfo($.trim($("#lineName").val()));
}

//保存
function saveData() {
    if ($("#lineNameList option:selected").length == 0) {
        frxs.pageClose();
        return false;
    } else if ($("#lineNameList option:selected").length > 1) {
        window.top.$.messager.alert("提示", "只能选择一条线路！", "info")
        return false;
    }
    else {
        if ($("#lineNameList option:selected").text() == lineName) {
            frxs.pageClose();
            return false;
        }
    }
    $(this).attr("disabled", "disabled");
    var validate = $("#lineChangeform").form('validate');
    if (!validate) {
        return false;
    }
    window.parent.$(window.frameElement).closest(".window").find("#btnChangeOk").addClass("easyui-linkbutton").linkbutton('disable');
    var loading = frxs.loading("正在加载中，请稍后...");
    $.ajax({
        url: "../WarehouseOrder/LineChangeHandle",
        type: "post",
        dataType: "json",
        data: {
            OrderID: orderid,
            LineName: $("#lineNameList option:selected").text(),
            LineID: $("#lineNameList option:selected").val()
        },
        success: function (result) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnChangeOk").addClass("easyui-linkbutton").linkbutton('enable');
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    window.top.$.messager.alert("提示", "修改成功", "info", function () {
                        window.frameElement.wapi.$("#grid").datagrid("reload");
                        window.frameElement.wapi.focus();
                        frxs.pageClose();
                    });
                } else {
                    parent.$("#btnChangeOk").linkbutton('enable');
                    window.top.$.messager.alert("提示", result.Info, "info");
                }
            }
        },
        error: function (request, textStatus, errThrown) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnChangeOk").addClass("easyui-linkbutton").linkbutton('enable');
            parent.$("#btnChangeOk").linkbutton('enable');
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