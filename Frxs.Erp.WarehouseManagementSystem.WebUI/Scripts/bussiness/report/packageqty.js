$(function () {
    //grid绑定
    initGrid();

    //grid高度改变
    gridresize();

    //$("#ShippingBeginDate").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    //$("#ShippingEndDate").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
});

function initGrid() {
    $('#grid').datagrid({
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            //查询条件
            ShippingBeginDate: $("#ShippingBeginDate").val(),
            ShippingEndDate: $("#ShippingEndDate").val(),
            ShippingUserName: $.trim($("#ShippingUserName").val())
        },
        columns: [[
            { title: '司机名称', field: 'ShippingUserName', width: 120 ,formatter:frxs.replaceCode},
            { title: '门店名称', field: 'ShopName', width: 220, formatter: frxs.replaceCode },
            {
                title: '配送日期', field: 'ShippingBeginDate', width: 120,  align: 'center', formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            { title: '订单编号', field: 'OrderId', width: 160 },
            { title: '周转箱数', field: 'Package1Qty', width: 100, align: 'center' }]],
        toolbar: [
            {
                id: 'btnExport',
                text: '导出',
                iconCls: 'icon-daochu',
                handler: exportoutToPage
            }]
    });
}

//查询
function search() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }

    var loading = frxs.loading();
    $.ajax({
        url: '../SalesReport/GetListPackageQtyData',
        type: 'post',
        data: {
            //查询条件
            ShippingBeginDate: $("#ShippingBeginDate").val() ? ($("#ShippingBeginDate").val() + " 00:00") : "",
            ShippingEndDate: $("#ShippingEndDate").val() ? ($("#ShippingEndDate").val() + " 23:59") : "",
            ShippingUserName: $.trim($("#ShippingUserName").val())
        },
        success: function (data) {
            loading.close();
            $('#grid').datagrid({ data: $.parseJSON(data) });
        }
    });
}

function resetSearch() {
    $("#ShippingBeginDate").val('');
    $("#ShippingEndDate").val('');
    $("#ShippingUserName").val('');
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});


//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}

//导出 新代码
function exportoutToPage() {
    var loading = window.top.frxs.loading("正在导出数据，如数据量大可能需要长一点时间...");
    var text = exportExcel();
    if (text) {
        event.preventDefault();
        var bb = self.Blob;
        saveAs(
            new bb(
                ["\ufeff" + text] //\ufeff防止utf8 bom防止中文乱码
                , { type: "html/plain;charset=utf8" }
            ), "周转箱统计导出_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
        );
    }
    loading.close();
}

//客户端导出Excel
function exportExcel() {
    var rows = $("#grid").datagrid("getRows");
    if (rows.length <= 0) {
        $.messager.alert("提示", "必须先查询得出数据才能导出。", "info");
        return false;
    }

    var trtdCode = "<tr>";
    trtdCode += "<td style='height:24px'>司机名称</td>";
    trtdCode += "<td>门店名称</td>";
    trtdCode += "<td>配送日期</td>";
    trtdCode += "<td>订单编号</td>";
    trtdCode += "<td>周转箱数</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";
        
        trtdCode += "<td style='height:20px'>" + frxs.replaceCode(rows[i].ShippingUserName) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ShopName) + "</td>";
        trtdCode += "<td>" + ((rows[i].ShopName == "合计" || rows[i].ShopName == "总计") ? "" : (frxs.dateTimeFormat(rows[i].ShippingBeginDate, "yyyy-MM-dd"))) + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].OrderId + "\">" + rows[i].OrderId + "</td>";
        trtdCode += "<td>" + rows[i].Package1Qty + "</td>";
        trtdCode += "</tr>";
    }

    var dataCode = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
    dataCode = dataCode.replace("{table}", trtdCode);
    return dataCode;
}