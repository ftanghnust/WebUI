$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd"));
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd"));

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
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            nKind: 2,
            SubWID: $("#SubWID").combobox('getValue'),
            S3: $('#S3').val(),
            S4: $("#S4").val() ? ($("#S4").val() + " 23:59:59") : ""
        },
        columns: [[
            { title: '公司机构', field: 'WName', width: 120, formatter: frxs.formatText },
            { title: '商品编码', field: 'ProductCode', width: 80 },
            { title: '商品名称', field: 'ProductName', width: 170, formatter: frxs.formatText },
            { title: '主条码', field: 'BarCode', width: 110 },
            { title: '商品状态', field: 'StatusName', width: 100 },
            { title: '商品分类', field: 'CategoryName', width: 120, formatter: frxs.formatText },
            { title: '采购员', field: 'BuyEmpName', width: 110, formatter: frxs.formatText },
            { title: '计量单位', field: 'Unit', width: 80 },
            {
                title: '销售数量', field: 'SaleQty', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '销售金额', field: 'SaleAmt', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '订货数量', field: 'BuyQty', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '订货金额', field: 'BuyAmt', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '缺货数量', field: 'LackQty', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '缺货金额', field: 'LackAmt', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '缺货率', field: 'LackRateStr', width: 120, align: 'right' },
            { title: '货区', field: 'ShelfAreaName', width: 120 }
        ]],
        toolbar: [
            {
                id: 'btnExport',
                text: '导出',
                iconCls: 'icon-daochu',
                handler: exportout
            }]
    });
}

//导出
function exportout() {
    var loading = window.top.frxs.loading("正在导出数据，如数据量大可能需要长一点时间...");
    var text = exportExcel();
    if (text) {
        event.preventDefault();
        var bb = self.Blob;
        saveAs(
            new bb(
                ["\ufeff" + text] //\ufeff防止utf8 bom防止中文乱码
                , { type: "html/plain;charset=utf8" }
            ), "采购员缺货分析报表_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
        );
    }
    loading.close();
}

function exportExcel() {
    var rows = $("#grid").datagrid("getRows");
    if (rows.length <= 0) {
        $.messager.alert("提示", "必须先查询得出数据才能导出。", "info");
        return false;
    }

    var trtdCode = "<tr>";
    trtdCode += "<td style='height:24px'>公司机构</td>";
    trtdCode += "<td>商品编码</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>主条码</td>";
    trtdCode += "<td>商品状态</td>";
    trtdCode += "<td>商品分类</td>";
    trtdCode += "<td>采购员</td>";
    trtdCode += "<td>计量单位</td>";
    trtdCode += "<td>销售数量</td>";
    trtdCode += "<td>销售金额</td>";
    trtdCode += "<td>订货数量</td>";
    trtdCode += "<td>订货金额</td>";
    trtdCode += "<td>缺货数量</td>";
    trtdCode += "<td>缺货金额</td>";
    trtdCode += "<td>缺货率</td>";
    trtdCode += "<td>货区</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";

        trtdCode += "<td style='height:20px'>" + rows[i].WName + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].ProductCode + "\">" + rows[i].ProductCode + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ProductName) + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].BarCode + "\">" + rows[i].BarCode + "</td>";
        trtdCode += "<td>" + rows[i].StatusName + "</td>";
        trtdCode += "<td>" + rows[i].CategoryName + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].BuyEmpName) + "</td>";
        trtdCode += "<td>" + rows[i].Unit + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].SaleQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].BuyQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BuyAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].LackQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].LackAmt + "</td>";
        trtdCode += "<td>" + rows[i].LackRateStr + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ShelfAreaName) + "</td>";

        trtdCode += "</tr>";
    }

    var dataCode = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
    dataCode = dataCode.replace("{table}", trtdCode);
    return dataCode;
}



//查询
function search() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }
    var loading = frxs.loading();
    $.ajax({
        url: '../PurchaseReport/GetListSaleStockout',
        type: 'post',
        data: {
            //查询条件
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            nKind: 4,
            SubWID: $("#SubWID").combobox('getValue'),
            S3: $('#S3').val(),
            S4: $("#S4").val() ? ($("#S4").val() + " 23:59:59") : ""
        },
        success: function (data) {
            loading.close();
            $('#grid').datagrid({ data: $.parseJSON(data) });
        }
    });
}

function resetSearch() {
    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd"));
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd"));

    $("#SubWID").combobox('setValue', '');

}

function initDDL() {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            data.unshift({ "WID": "", "WName": "-请选择-" });
            //创建控件
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#SubWID").combobox('select', data[0].WID);
        }, error: function (e) {

        }
    });
    
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

