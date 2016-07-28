$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //供应商-采购员事件绑定
    eventBind();

    //grid高度改变
    gridresize();
    //select下拉框自适应高度    
    $('.easyui-combobox').combobox({
        panelHeight: 'auto'
    });

});


function initGrid() {
    $('#grid').datagrid({
        rownumbers: true,                   //显示行编号
        columns: [[
            { title: '公司机构', field: 'WAllName', width: 130, align: 'left' },
            { title: '商品分类', field: 'CategoryName', width: 150, align: 'left', formatter: frxs.formatText },
            { title: '商品编码', field: 'ProductCode', width: 100, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 200, formatter: frxs.formatText },
            { title: '条码', field: 'Barcode', width: 150, align: 'center' },
            { title: '规格', field: 'Spec', width: 80, align: 'center' },
            { title: '计量单位', field: 'Unit', width: 80, align: 'center' },
            {
                title: '库存数量', field: 'StockQty', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '库存单价', field: 'Price', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '库存金额', field: 'Amount', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '建议零售价', field: 'SalePrice', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '零售金额', field: 'SaleAmount', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '主供应商', field: 'VendorName', width: 200, align: 'left', formatter: frxs.formatText },
            {
                title: '采购员', field: 'BuyEmpName', width: 150, align: 'center', formatter: frxs.formatText
            },
            {
                title: '销售未过账数量', field: 'NoStoreQty', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '实时库存数量', field: 'StoreQty', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '配送价', field: 'WholeSalePrice', width: 100, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '配送金额', field: 'WholeSaleAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '总库存金额', field: 'StockTotalAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (isNaN(value) || (!value)) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '总零售金额', field: 'SaleTotalAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '总配送金额', field: 'WholeSaleTotalAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '商品状态', field: 'Status', width: 100, align: 'center' },
            { title: '最后销售日期', field: 'LastSaleDate', width: 120 },
            {
                title: '上周销售数量', field: 'WeekQty', width: 120, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '上周销售金额', field: 'WeekAmount', width: 120, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '上月销售数量', field: 'MonthQty', width: 120, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '上月销售金额', field: 'MonthAmount', width: 120, align: 'right', formatter: function (value) {
                    value = (isNaN(value)||(!value)) ? 0 : value;
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
            ), "采购员库存查询报表_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td>商品分类</td>";
    trtdCode += "<td>商品编码</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>条码</td>";
    trtdCode += "<td>规格</td>";
    trtdCode += "<td>计量单位</td>";
    trtdCode += "<td>库存数量</td>";
    trtdCode += "<td>库存单价</td>";
    trtdCode += "<td>库存金额</td>";
    trtdCode += "<td>建议零售价</td>";
    trtdCode += "<td>零售金额</td>";
    trtdCode += "<td>主供应商</td>";
    trtdCode += "<td>采购员</td>";
    trtdCode += "<td>销售未过账数量</td>";
    trtdCode += "<td>实时库存数量</td>";
    trtdCode += "<td>配送价</td>";
    trtdCode += "<td>配送金额</td>";
    trtdCode += "<td>总库存金额</td>";
    trtdCode += "<td>总零售金额</td>";
    trtdCode += "<td>总配送金额</td>";
    trtdCode += "<td>商品状态</td>";
    trtdCode += "<td>最后销售日期</td>";
    trtdCode += "<td>上周销售数量</td>";
    trtdCode += "<td>上周销售金额</td>";
    trtdCode += "<td>上月销售数量</td>";
    trtdCode += "<td>上月销售金额</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";

        trtdCode += "<td style='height:20px'>" + frxs.replaceCode(rows[i].WAllName) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].CategoryName) + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].ProductCode + "\">" + rows[i].ProductCode + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ProductName) + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].Barcode + "\">" + rows[i].Barcode + "</td>";
        trtdCode += "<td>" + rows[i].Spec + "</td>";
        trtdCode += "<td>" + rows[i].Unit + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].StockQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].Price + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].Amount + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SalePrice + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleAmount + "</td>";
        trtdCode += "<td width='380'>" + frxs.replaceCode(rows[i].VendorName) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].BuyEmpName) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].NoStoreQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].StoreQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].WholeSalePrice + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].WholeSaleAmount + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].StockTotalAmount + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleTotalAmount + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].WholeSaleTotalAmount + "</td>";
        trtdCode += "<td>" + rows[i].Status + "</td>";
        trtdCode += "<td>" + rows[i].LastSaleDate + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].WeekQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].WeekAmount + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].MonthQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].MonthAmount + "</td>";

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

    $('#grid').datagrid({
        url: '../StoreReport/GetBuyerStockCheckList',
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页

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
            nKind: 4,
            SubWID: $("#SubWID").combobox('getValue'),
            S1: $('#CategoriesId1').combobox('getValue'),
            S2: $('#CategoriesId2').combobox('getValue'),
            S3: $('#CategoriesId3').combobox('getValue'),
            S4: $('#S4').val(),
            S5: $('#S5').val(),
            S6: $('#S6').val(),
            S7: $('#S7').val(),
            S8: $('#S8').combobox('getValue')
        }
    });
}

function resetSearch() {
    $("#SubWID").combobox('setValue', '');
    $('#CategoriesId1').combobox('setValue', '');
    $('#CategoriesId2').combobox('setValue', '');
    $('#CategoriesId3').combobox('setValue', '');
    $('#S4').val('');
    $("#VendorCode").val('');
    $("#VendorName").val('');
    $('#S5').val('');
    $('#S6').val('');
    $('#S7').val('');
    $('#S8').combobox('setValue', '');

    $("#EmpName").val('');
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

function selVendor() {
    var vendorCode = $("#VendorCode").val();
    var vendorName = $("#VendorName").val();
    frxs.dialog({
        title: "选择供应商",
        url: "../BuyOrderPre/SelectVendor?vendorCode=" + encodeURIComponent(vendorCode) + "&vendorName=" + encodeURIComponent(vendorName),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填供应商
function backFillVendor(vendorId, vendorCode, vendorName) {
    $("#S3").val(vendorId);
    $("#VendorCode").val(vendorCode);
    $("#VendorName").val(vendorName);
}


function selBuyEmp() {
    frxs.dialog({
        title: "选择采购员",
        url: "../BuyOrderPre/SelectBuyEmp?buyEmpName=" + encodeURIComponent($("#EmpName").val()),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填采购员
function backFillBuyEmp(empID, empName) {
    $("#S7").val(empID);
    $("#EmpName").val(empName);
}


//供应商-采购员事件绑定
function eventBind() {
    $("#VendorCode").keydown(function (e) {
        if (e.keyCode == 13) {
            eventVendorCodeName();
        }
    });

    $("#VendorName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventVendorCodeName();
        }
    });
    
    $("#EmpName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventBuyEmp();
        }
    });

}

//供应商名称Code
function eventVendorCodeName() {
    $.ajax({
        url: "../Common/GetVendorInfo",
        type: "post",
        data: {
            VendorCode: $("#VendorCode").val(),
            VendorName: $("#VendorName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#S3").val(obj.rows[0].VendorID);
                $("#VendorCode").val(obj.rows[0].VendorCode);
                $("#VendorName").val(obj.rows[0].VendorName);
            } else {
                selVendor();
            }
        }
    });
}

//采购员
function eventBuyEmp() {
    $.ajax({
        url: "../Common/GetBuyEmpInfo",
        type: "post",
        data: {
            EmpName: $("#EmpName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#S7").val(obj.rows[0].EmpID);
                $("#EmpName").val(obj.rows[0].EmpName);
            } else {
                selBuyEmp();
            }
        }
    });
}