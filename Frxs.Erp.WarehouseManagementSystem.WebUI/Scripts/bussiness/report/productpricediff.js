$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S3").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S4").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
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
            S3: $("#S3").val(),
            S4: $("#S4").val(),
            S5: $("#S5").val(),
            S6: $("#S6").val(),
            nKind: 3,
            SubWID: $("#SubWID").combobox('getValue')
        },
        columns: [[
            { title: '单号', field: 'AdjId', width: 80, align: 'center' },
            { title: '公司机构', field: 'WName', width: 120 },
            { title: '单据类型', field: 'BillType', width: 100 },
            {
                title: '日期', field: 'BeginTime', width: 120, formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            {
                title: '过帐日期', field: 'PostingTime', width: 120, formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            { title: '操作员', field: 'CreateUserName', width: 100 },
            { title: '商品编码', field: 'SKU', width: 100 },
            { title: '商品名称', field: 'ProductName', width: 170 },
            { title: '商品分类1', field: 'Category1Name', width: 100 },
            { title: '商品分类2', field: 'Category2Name', width: 100 },
            { title: '商品分类3', field: 'Category3Name', width: 120 },
            { title: '计量单位', field: 'Unit', width: 80 },
            { title: '采购员', field: 'BuyEmpName', width: 100, formatter: frxs.replaceCode },
            {
                title: '旧价格', field: 'OldPrice', width: 100, align: 'right', formatter: function (value, rec) {
                    if (rec.WName == "单号合计:" || rec.WName == "总合计:") {
                        return "";
                    }
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '新价格', field: 'NewPrice', width: 100, align: 'right', formatter: function (value, rec) {
                    if (rec.WName == "单号合计:" || rec.WName == "总合计:") {
                        return "";
                    }
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '价格调整差异', field: 'DifPrice', width: 120, align: 'right', formatter: function (value, rec) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '库存数量', field: 'StoreQty', width: 100, align: 'right', formatter: function (value, rec) {
                    if (rec.WName == "单号合计:" || rec.WName == "总合计:") {
                        return "";
                    }
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '批发金额差异', field: 'DifAmt', width: 120, align: 'right', formatter: function (value, rec) {
                    return parseFloat(value).toFixed(4);
                }
            }
        ]],
        toolbar: [
            {
                id: 'btnExport',
                text: '导出',
                iconCls: 'icon-daochu',
                handler: exportoutToPage // exportout
            }]
    });
}

function exportout() {
    location.href = "../SalesReport/ExportExcelProductPriceDiff?nKind=3&S1=" + $("#S1").val() +
        "&S2=" + $("#S2").val() + "&S3=" + $("#S3").val() + "&S4=" + $("#S4").val() +
        "&SubWID=" + $("#SubWID").combobox('getValue');
}


//查询
function search() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }

    var loading = frxs.loading();
    $.ajax({
        url: '../SalesReport/GetListProductPriceDiff',
        type: 'post',
        data: {
            //查询条件
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            S3: $("#S3").val(),
            S4: $("#S4").val(),
            S5: $("#S5").val(),
            S6: $("#S6").val(),
            nKind: 3,
            SubWID: $("#SubWID").combobox('getValue')
        },
        success: function (data) {
            loading.close();
            $('#grid').datagrid({ data: $.parseJSON(data) });
        }
    });
}

function resetSearch() {
    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S3").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S4").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S5").val('');
    $("#S6").val('');
    $("#SubWID").combobox('setValue', $('#SubWID').combobox('getData')[0].WID);
}

function initDDL() {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            //data.unshift({ "WID": "", "WName": "-请选择-" });
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
            ), "批发价调整金额差异导出_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td style='height:24px'>单号</td>";
    trtdCode += "<td>公司机构</td>";
    trtdCode += "<td>单据类型</td>";
    trtdCode += "<td>日期</td>";
    trtdCode += "<td>过帐日期</td>";
    trtdCode += "<td>操作员</td>";
    trtdCode += "<td>商品编码</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>商品分类1</td>";
    trtdCode += "<td>商品分类2</td>";
    trtdCode += "<td>商品分类3</td>";
    trtdCode += "<td>计量单位</td>";
    trtdCode += "<td>采购员</td>";
    trtdCode += "<td>旧价格</td>";
    trtdCode += "<td>新价格</td>";
    trtdCode += "<td>价格调整差异</td>";
    trtdCode += "<td>库存数量</td>";
    trtdCode += "<td>批发金额差异</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";
        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].AdjId + "\">" + rows[i].AdjId + "</td>";//单号
        trtdCode += "<td>" + frxs.replaceCode(rows[i].WName) + "</td>";                                 //公司机构
        trtdCode += "<td>" + frxs.replaceCode(rows[i].BillType) + "</td>";                              //单据类型
        trtdCode += "<td>" + frxs.replaceCode(rows[i].BeginTime) + "</td>";                              //日期
        trtdCode += "<td>" + frxs.replaceCode(rows[i].PostingTime) + "</td>";                              //过帐日期
        trtdCode += "<td>" + frxs.replaceCode(rows[i].CreateUserName) + "</td>";                              //操作员

        var sku = rows[i].SKU;
        sku = (!sku) ? "" : sku;//防止sku == null时显示null
        trtdCode += "<td x:str=\"'" + sku + "\">" + sku + "</td>";                            //商品编码

        trtdCode += "<td>" + frxs.replaceCode(rows[i].ProductName) + "</td>";                              //商品名称
        trtdCode += "<td>" + frxs.replaceCode(rows[i].Category1Name) + "</td>";                              //商品分类1
        trtdCode += "<td>" + frxs.replaceCode(rows[i].Category2Name) + "</td>";                              //商品分类2
        trtdCode += "<td>" + frxs.replaceCode(rows[i].Category3Name) + "</td>";                              //商品分类3
        trtdCode += "<td>" + frxs.replaceCode(rows[i].Unit) + "</td>";                              //计量单位
        trtdCode += "<td>" + frxs.replaceCode(rows[i].BuyEmpName) + "</td>";                              //采购员

        trtdCode += (rows[i].WName == "单号合计:" || rows[i].WName == "总合计:") ? "<td></td>" : ("<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].OldPrice + "</td>");                    //旧价格
        trtdCode += (rows[i].WName == "单号合计:" || rows[i].WName == "总合计:") ? "<td></td>" : ("<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].NewPrice + "</td>");                    //新价格

        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].DifPrice + "</td>";                    //价格调整差异
        trtdCode += (rows[i].WName == "单号合计:" || rows[i].WName == "总合计:") ? "<td></td>" : ("<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].StoreQty + "</td>");                    //库存数量
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].DifAmt + "</td>";                        //批发金额差异

        trtdCode += "</tr>";
    }

    var dataCode = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
    dataCode = dataCode.replace("{table}", trtdCode);
    return dataCode;
}