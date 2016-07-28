$(function () {
   
    GetShelfAreaList();
    $("#btnDelete").attr("disabled", "disabled");
    var tbody = $("table[name='tblWarehouseEmpShelf']").children("tbody");
    $("#chkAll").click(function () {
        if (tbody.children("tr").length > 0) {
            var arr = new Array();
            arr[0] = $("#btnDelete");
            checkAll(tbody, $(this), arr);
        }
    });
    

    //单个编辑
    $("#btnEdit").click(function () {
        if (tbody.find(":checked").length > 0) {
            if (tbody.find(":checked").length > 1)
            {
                alert("只能选中一条数据再编辑！");
            }
            else
            {
                var idStr = "";
                var name = "";
                var userAccount = "";
                tbody.children("tr").each(function () {
                    if ($(this).find(":checked").length > 0) {
                        var id = $(this).children("td:eq(0)").find(":hidden").val();
                        name=$(this).children("td:eq(2)").text()
                        userAccount=$(this).children("td:eq(3)").text()
                        idStr = id;
                    }
                });

                //编辑品牌                
                xsjs.window({
                    title: "编辑送货员管理货位",
                    url: "/WarehouseEmpShelf/WarehouseEmpShelfEdit?Id=" + idStr + "&name=" + name + "&userAccount=" + userAccount,
                    owdoc: window.top,
                    width: 650,
                    height: 600,
                    modal: true
                });
            }


        };
    });

    
});

function rowDbClick(tr) {
    var idStr = tr.children("td:eq(0)").find(":hidden").val();
    var name = tr.children("td:eq(2)").text();
    var userAccount = tr.children("td:eq(3)").text();
    //编辑品牌                
    xsjs.window({
        title: "编辑送货员管理货位",
        url: "/WarehouseEmpShelf/WarehouseEmpShelfEdit?Id=" + idStr + "&name=" + name + "&userAccount=" + userAccount,
        owdoc: window.top,
        width: 650,
        height: 600,
        modal: true
    });
}

//如果isSingleSelect为false，则表示该表格为多选表格，则务必定义该函数。
function changeCheck() {
    var tbody = $("table[name='tblWarehouseEmpShelf']").children("tbody");
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

function GetShelfAreaList() {
    var ShelfAreaID = $("select[name='ShelfAreaID']");
    $.ajax({
        url: '../ShelfArea/GetShelfAreaSelectList',
        type: 'get',
        success: function (obj) {
            var list = JSON.parse(obj);
            ShelfAreaID.empty();
            ShelfAreaID.append("<option value=''>全部</option>");
            for (var i = 0; i < list.length; i++) {
                ShelfAreaID.append("<option value='" + list[i].ShelfAreaID + "'>" + list[i].ShelfAreaName + "</option>");
            }
        },
        error: function () {
        }
    });
}
