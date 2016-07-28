$(function () {
    $("#btnDelete").attr("disabled", "disabled");
    var tbody = $("table[name='tblShopGroup']").children("tbody");
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
                    idStr += (id + ",");
                }
            });
            if (confirm("确认删除？")) {
                $.ajax({
                    url: "../ShopGroup/DeleteShopGroup",
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

                window.top.addTabs('编辑门店分组', '../ShopGroup/ShopGroupAddOrEdit?Id=' + idStr);

                ////编辑品牌                
                //xsjs.window({
                //    title: "编辑门店分组",
                //    url: "/ShopGroup/ShopGroupAddOrEdit?Id=" + idStr,
                //    owdoc: window.top,
                //    width: 825,
                //    height: 800,
                //    modal: true
                //});
            }


        };
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

                //编辑品牌                
                xsjs.window({
                    title: "查看门店分组",
                    url: "/ShopGroup/ShopGroupView?Id=" + idStr,
                    owdoc: window.top,
                    width: 825,
                    height: 800,
                    modal: true
                });
            }


        };
    });

});

//如果isSingleSelect为false，则表示该表格为多选表格，则务必定义该函数。
function changeCheck() {
    var tbody = $("table[name='tblShopGroup']").children("tbody");
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

function rowDbClick(tr) {
    var id = tr.children("td:eq(0)").find(":hidden").val();

    window.top.addTabs('编辑门店分组', '../ShopGroup/ShopGroupAddOrEdit?Id=' + id);    
}


function OpenDialogOrAddTab(type, action, id) {
    if (type == "dialog" && action == "add") {       
        window.top.addTabs('新增', '../ShopGroup/ShopGroupAddOrEdit');
    }
}