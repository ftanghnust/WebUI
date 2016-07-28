$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();
    
    //采购员事件绑定
    eventBind();

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
            S3: $('#S3').val(),
            S4: $('#S4').val(),
            S5: $('#S5').val(),
            S6: $('#CategoriesId1').combobox('getValue'),
            S7: $('#CategoriesId2').combobox('getValue'),
            S8: $('#CategoriesId3').combobox('getValue'),
            nKind: 2,
            SubWID: $("#SubWID").combobox('getValue'),
            S9: $('#S9').val(),
            S10: $("#S10").val() ? ($("#S10").val() + " 23:59:59") : ""
        },
        columns: [[
            { title: '单号', field: 'AdjId', width: 120 },
            { title: '商品分类', field: 'CategoryName', width: 150, align: 'left', formatter: frxs.formatText },
            { title: '商品编码', field: 'SKU', width: 100, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 200, formatter: frxs.formatText },
            {
                title: '过账日', field: 'Sett_Date', width: 100, align: 'center', formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            {
                title: '配送价格', field: 'SalePrice', width: 100, formatter: function (value) {
                    return value == "" ? "" : parseFloat(value).toFixed(4);
                }
            },
            { title: '开单日期', field: 'AdjDate', width: 100, formatter: frxs.ymdFormat },
            { title: '过账时间', field: 'PostingTime', width: 100, formatter: frxs.dateFormat },
            { title: '采购员', field: 'EmpName', width: 110 },
            {
                title: '盘盈数量', field: 'AdjQtyW', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '盘盈金额', field: 'AdjAmtW', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '盘亏数量', field: 'AdjQtyF', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '盘亏金额', field: 'AdjAmtF', width: 120, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            }
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
            ), "盘赢盘亏统计报表_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td style='height:24px'>单号</td>";
    trtdCode += "<td>商品分类</td>";
    trtdCode += "<td>商品编码</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>过账日</td>";
    trtdCode += "<td>配送价格</td>";
    trtdCode += "<td>开单日期</td>";
    trtdCode += "<td>过账时间</td>";
    trtdCode += "<td>采购员</td>";
    trtdCode += "<td>盘盈数量</td>";
    trtdCode += "<td>盘盈金额</td>";
    trtdCode += "<td>盘亏数量</td>";
    trtdCode += "<td>盘亏金额</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";
        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].AdjId + "\">" + rows[i].AdjId + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].CategoryName) + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].SKU + "\">" + rows[i].SKU + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ProductName) + "</td>";
        trtdCode += "<td>" + (rows[i].Sett_Date ? frxs.dateTimeFormat(rows[i].Sett_Date, "yyyy-MM-dd") : "") + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].SalePrice) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].AdjDate) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].PostingTime) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].EmpName) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].AdjQtyW + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].AdjAmtW + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].AdjQtyF + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].AdjAmtF + "</td>";

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
        url: '../StoreReport/GetListStockadj',
        type: 'post',
        data: {
            //查询条件
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            S3: $('#S3').val(),
            S4: $('#S4').val(),
            S5: $('#S5').val(),
            S6: $('#CategoriesId1').combobox('getValue'),
            S7: $('#CategoriesId2').combobox('getValue'),
            S8: $('#CategoriesId3').combobox('getValue'),
            nKind: 2,
            SubWID: $("#SubWID").combobox('getValue'),
            S9: $('#S9').val(),
            S10: $("#S10").val() ? ($("#S10").val() + " 23:59:59") : ""
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
    $("#S3").val("");
    $("#S4").val("");
    $("#S5").attr("value", "");
    
    $('#CategoriesId1').combobox('setValue', '');
    $('#CategoriesId2').combobox('setValue', '');
    $('#CategoriesId3').combobox('setValue', '');
    $("#BuyEmpName").attr("value", "");
    $("#SubWID").combobox('setValue', '');

    $("#S9").val("");
    $("#S10").val("");
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

function selBuyEmp() {
    frxs.dialog({
        title: "选择采购员",
        url: "../BuyOrderPre/SelectBuyEmp?buyEmpName=" + encodeURIComponent($("#BuyEmpName").val()),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填采购员
function backFillBuyEmp(empId, empName) {
    $("#S5").val(empId);
    $("#BuyEmpName").val(empName);
}


//供应商-采购员事件绑定
function eventBind() {
    $("#BuyEmpName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventBuyEmp();
        }
    });
}

//采购员
function eventBuyEmp() {
    $.ajax({
        url: "../Common/GetBuyEmpInfo",
        type: "post",
        data: {
            EmpName: $("#BuyEmpName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var parseobj = JSON.parse(obj);
            if (parseobj.total == 1) {
                $("#S5").val(parseobj.rows[0].EmpID);
                $("#BuyEmpName").val(parseobj.rows[0].EmpName);
            } else {
                selBuyEmp();
            }
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

