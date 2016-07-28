$(function () {
    $("#SendW").empty().append("<option value=''>全部</option>").append("<option value='SendW1'>周一</option>").append("<option value='SendW2'>周二</option>").append("<option value='SendW3'>周三</option>").append("<option value='SendW4'>周四</option>").append("<option value='SendW5'>周五</option>").append("<option value='SendW6'>周六</option>").append("<option value='SendW7'>周日</option>");
    
    $("#btnDelete").attr("disabled", "disabled");
    var tbody = $("table[name='tblWarehouseLine']").children("tbody");
    $("#chkAll").click(function () {
        if (tbody.children("tr").length > 0) {
            var arr = new Array();
            arr[0] = $("#btnDelete");
            checkAll(tbody, $(this), arr);
        }
    });
    //单个删除
    $("#btnDelete").click(function () {
        if (tbody.find(":checked").length > 0) {
            var idStr = "";
            tbody.children("tr").each(function () {
                if ($(this).find(":checked").length > 0) {
                    var id = $(this).children("td:eq(0)").find(":hidden").val();
                    idStr += ("'" + id + "',");
                }
            });
            if (confirm("确认删除？")) {
                $.ajax({
                    url: "../WarehouseLine/DeleteWarehouseLine",
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
            if (tbody.find(":checked").length > 1)
            {
                $.messager.alert("提示", "只能选中一条数据再编辑！", "info");
                
            }
            else
            {
                var idStr = "";
                tbody.children("tr").each(function () {
                    if ($(this).find(":checked").length > 0) {
                        var id = $(this).children("td:eq(0)").find(":hidden").val();
                        idStr = id;
                    }
                });

                //编辑品牌                
                xsjs.window({
                    title: "编辑送货线路",
                    url: "/WarehouseLine/WarehouseLineAddOrEdit?Id=" + idStr,
                    owdoc: window.top,
                    width: 550,
                    height: 360,
                    modal: true
                });
            }


        };
    });

    //编辑门店顺序
    $("#btnOrder").click(function () {
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

                //编辑门店顺序                
                xsjs.window({
                    title: "编辑门店顺序",
                    url: "../WarehouseLineShop/WarehouseLineShopAddOrEdit?Id=" + idStr,
                    owdoc: window.top,
                    width: 400,
                    height: 700,
                    modal: true
                });
            }


        };
    });
});

function rowDbClick(tr) {
    var idStr = tr.children("td:eq(0)").find(":hidden").val();
    //编辑品牌                
    xsjs.window({
        title: "编辑送货线路",
        url: "/WarehouseLine/WarehouseLineAddOrEdit?Id=" + idStr,
        owdoc: window.top,
        width: 550,
        height: 360,
        modal: true
    });
}

//如果isSingleSelect为false，则表示该表格为多选表格，则务必定义该函数。
function changeCheck() {
    var tbody = $("table[name='tblWarehouseLine']").children("tbody");
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



function OpenDialogOrAddTab(type, action, id) {
    if (type == "dialog" && action == "add") {
        xsjs.window({
            title: "新增送货线路",
            url: "/WarehouseLine/WarehouseLineAddOrEdit",
            owdoc: window.top,
            width: 550,
            height: 360,
            modal: true
        });
    }
}
