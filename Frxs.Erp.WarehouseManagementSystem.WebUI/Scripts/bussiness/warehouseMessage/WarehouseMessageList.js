$(function () {
    $("#Status").empty().append("<option value=''>全部</option>").append("<option value='0'>未发布</option>").append("<option value='1'>已发布</option>").append("<option value='2'>已停止</option>");
    $("#MessageType").empty().append("<option value=''>全部</option>").append("<option value='0'>重要消息</option>").append("<option value='1'>促销</option>").append("<option value='2'>其他</option>");  //TODO 需要改为获取数据字典
  
    $("#btnDelete").attr("disabled", "disabled");
    var tbody = $("table[name='tblWarehouseMessage']").children("tbody");
    $("#chkAll").click(function () {
        if (tbody.children("tr").length > 0) {
            var arr = new Array();
            arr[0] = $("#btnDelete");
            checkAll(tbody, $(this), arr);
        }
    });

    //单个查看
    $("#btnView").click(function () {
        if (tbody.find(":checked").length > 0) {
            if (tbody.find(":checked").length > 1) {
                alert("只能选中一条数据再查看！");
            }
            else {
                var idStr = "";
                tbody.children("tr").each(function () {
                    if ($(this).find(":checked").length > 0) {
                        var id = $(this).children("td:eq(0)").find(":hidden").val();
                        idStr = id;
                    }
                });
              
                //查看品牌                
                xsjs.window({
                    title: "查看消息",
                    url: "../WarehouseMessage/WarehouseMessageView?id=" + idStr,
                    owdoc: window.top,
                    width: 800,
                    height: 680,
                    modal: true
                });
            }


        };
    });


    //发布
    $("#btnConf").click(function () {
        if (tbody.find(":checked").length > 0) {
            var messageIdStr = "";
            tbody.children("tr").each(function () {
                if ($(this).find(":checked").length > 0) {
                    var id = $(this).children("td:eq(0)").find(":hidden").val();
                    var status = $(this).children("td:eq(9)").html();
                   
                    if (status == "未发布" || status == "已停止") {
                        messageIdStr += (id + ",");
                    } else {
                        messageIdStr = "";
                        alert("已确认单不能再次发布！");
                        return false; 
                    }
                }
            });
            if (messageIdStr != "") {
                messageIdStr = messageIdStr.substr(0, messageIdStr.length - 1);
                if (confirm("确认以上消息吗？")) {
                    $.ajax({
                        url: "../WarehouseMessage/WarehouseMessageChangeStatus",
                        type: "get",
                        dataType: "json",
                        data: {
                            ids: messageIdStr,
                            status: 1
                        },
                        success: function (result) {
                            if (result != undefined && result.Info != undefined) {
                                alert(result.Info);
                                if (result.Flag == "SUCCESS") {
                                    $("#aSearch").click();
                                }
                            }
                        },
                        error: function (request, textStatus, errThrown) {
                            if (textStatus) {
                                alert(textStatus);
                            } else if (errThrown) {
                                alert(errThrown);
                            } else {
                                alert("出现错误");
                            }
                        }
                    });
                };
            }
        } else {
            alert("没有选中记录");
        }
    });

    //单个删除
    $("#btnDelete").click(function () {
        if (tbody.find(":checked").length > 0) {
            var idStr = "";
            tbody.children("tr").each(function () {
                if ($(this).find(":checked").length > 0) {
                    var id = $(this).children("td:eq(0)").find(":hidden").val();
                    idStr += ( id + ",");
                }
            });
            if (confirm("确认删除？")) {
                $.ajax({
                    url: "../WarehouseMessage/DeleteWarehouseMessage",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: idStr
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            alert(result.Info);
                            if (result.Flag == "SUCCESS") {
                                $("#aSearch").click();
                            }
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            alert(textStatus);
                        } else if (errThrown) {
                            alert(errThrown);
                        } else {
                            alert("出现错误");
                        }
                    }
                });
            };
        };
    });

    //单个编辑
    $("#btnEdit").click(function () {
        if (tbody.find(":checked").length > 0) {
            if (tbody.find(":checked").length > 1) {
                alert("只能选中一条数据再编辑！");
            }
            else {
                var idStr = "";
                tbody.children("tr").each(function () {
                    if ($(this).find(":checked").length > 0) {
                        var id = $(this).children("td:eq(0)").find(":hidden").val();
                        idStr = id;
                    }
                });
                //  window.top.addTabs('修改消息', '../WarehouseMessage/WarehouseMessageAddOrEdit?BuyID=' + messageId);
            
                //编辑消息                
                xsjs.window({
                    title: "修改消息",
                    url: "../WarehouseMessage/WarehouseMessageAddOrEdit?Id=" + idStr,
                    owdoc: window.top,
                    width: 800,
                    height: 680,
                    modal: true
                });
            }


        };
    });
   
});





function OpenTabs() {
    //if (!$('#tabs').tabs('exists', subtitle)) {
    //    index++;
    //    $('#tabs').tabs('add', {
    //        title: $('#tree').tree('getSelected').text,
    //        content: '<iframe frameborder="0" style="width:100%; height:100%;border:0px;"  src=' + xx + '></iframe>',
    //        closable: true
    //    });
    //} else {
    //    $('#tabs').tabs('select', subtitle);
    //}


}

function OpenDialogOrAddTab(type, action, id) {
    if (type == "dialog" && action == "add") {
        xsjs.window({
            title: "新增信息",
            url: "../WarehouseMessage/WarehouseMessageAddOrEdit",
            owdoc: window.top,
            width: 800,
            height: 680,
            modal: true
        });
    }

}

function rowDbClick(tr) {
    var id = tr.children("td:eq(0)").find(":hidden").val();
    //编辑品牌              
  
    xsjs.window({
        title: "查看消息",
        url: "../WarehouseMessage/WarehouseMessageView?id=" + id,
        owdoc: window.top,
        width: 800,
        height: 680,
        modal: true
    });
}
//如果isSingleSelect为false，则表示该表格为多选表格，则务必定义该函数。
function changeCheck() {
    var tbody = $("table[name='tblWarehouseMessage']").children("tbody");
    //全部未选中
    if (tbody.find(":checked").length == 0) {
        $("#chkAll").removeAttr("checked");
        $("#btnDelete").attr("disabled", "disabled");
    } else {
        //全部选中
        if (tbody.find(":checked").length == tbody.children("tr").length) {
            $("#chkAll").attr("checked", "checked");
        } else {
            //部分选中
            $("#chkAll").removeAttr("checked");
        }
        $("#btnDelete").removeAttr("disabled");
    }
}
