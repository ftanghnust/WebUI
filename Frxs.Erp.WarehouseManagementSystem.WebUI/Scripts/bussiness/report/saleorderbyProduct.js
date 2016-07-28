//页面加载
$(function () {

    //表格绑定
    initGrid();

    //下拉绑定
    initDDL();

    //供应商-采购员事件绑定
    eventBind();
    
    //grid高度改变
    gridresize();
    
    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");

});

//初始化grid
function initGrid() {
    $('#grid').datagrid({
        rownumbers: true,                   //显示行编号
        columns: [[
            { title: '商品编码', field: 'SKU', width: 80, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 160, align: 'left', formatter: frxs.formatText },
            {
                title: '过账日', field: 'Sett_Date', width: 100, align: 'center', formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            { title: '采购员', field: 'BuyEmpName', width: 60, align: 'center', formatter: frxs.formatText },
            { title: '商品分类', field: 'CategoryName', width: 160, align: 'left', formatter: frxs.formatText },
            { title: '供应商', field: 'VendorName', width: 160, align: 'left', formatter: frxs.formatText },
            {
                title: '销售数量', field: 'SaleQty', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '销售成本金额', field: 'SaleAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '销售平台费用', field: 'SalePoint', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
             //{
             //    title: '销售金额', field: 'SaleTotalAmount', width: 100, align: 'center', formatter: function (value) {
             //        value = (!value) ? 0 : value;
             //        return parseFloat(value).toFixed(4);
             //    }
             //},
                {
                    title: '销售金额', field: 'SaleAmount_SaleTotalAmount', width: 100, align: 'right', formatter: function (value,rec) {
                        value = (!rec.SaleAmount) ? 0 : rec.SaleAmount;
                        return parseFloat(value).toFixed(4);
                    }
                },
                //{
                //    title: '合计销售金额', field: 'TotalAmt', width: 150, align: 'center', formatter: function (value) {
                //        value = (!value) ? 0 : value;
                //        return parseFloat(value).toFixed(4);
                //    }
                //},
                {
                    title: '合计销售金额', field: 'SaleTotalAmount', width: 150, align: 'right', formatter: function (value) {
                        value = (!value) ? 0 : value;
                        return parseFloat(value).toFixed(4);
                    }
                },
            {
                title: '退货数量', field: 'BackQty', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '退货成本金额', field: 'BackAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            
            {
                title: '退货平台费用', field: 'BackPoint', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            //{
            //    title: '退货金额', field: 'BackTotalAmount', width: 100, align: 'center', formatter: function (value) {
            //        value = (!value) ? 0 : value;
            //        return parseFloat(value).toFixed(4);
            //    }
            //},
            {
                title: '退货金额', field: 'BackAmount_BackTotalAmount', width: 100, align: 'right', formatter: function (value, rec) {
                    value = (!rec.BackAmount) ? 0 : rec.BackAmount;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '合计退货金额', field: 'BackTotalAmount', width: 110, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '合计平台费用', field: 'TotalPoint', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '合计成本金额', field: 'TotalAmount', width: 100, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
             {
                 title: '净销售', field: '净销售', width: 100, align: 'right', formatter: function (value) {
                     return parseFloat(value).toFixed(4);
                 }
             },
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

//下拉列表
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
        }
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
            ), "供应商销售情况汇总表_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td style='height:24px'>商品编码</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>采购员</td>";
    trtdCode += "<td>商品分类</td>";
    trtdCode += "<td>过账日</td>";
    trtdCode += "<td>供应商</td>";
    trtdCode += "<td>销售数量</td>";
    trtdCode += "<td>销售成本金额</td>";
    trtdCode += "<td>销售平台费用</td>";
    trtdCode += "<td>销售金额</td>";
    trtdCode += "<td>合计销售金额</td>";

    trtdCode += "<td>退货数量</td>";
    trtdCode += "<td>退货成本金额</td>";
    trtdCode += "<td>退货平台费用</td>";
    trtdCode += "<td>退货金额</td>";
    trtdCode += "<td>合计退货金额</td>";
    trtdCode += "<td>合计平台费用</td>";
    trtdCode += "<td>合计成本金额</td>";
    //trtdCode += "<td>合计销售金额</td>";
    trtdCode += "<td>净销售</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";
        
        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].SKU + "\">" + rows[i].SKU + "</td>";//商品编码
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ProductName) + "</td>";//商品名称
        trtdCode += "<td>" + frxs.replaceCode(rows[i].BuyEmpName) + "</td>";//采购员
        trtdCode += "<td>" + frxs.replaceCode(rows[i].CategoryName) + "</td>";//商品分类
        trtdCode += "<td>" + (rows[i].Sett_Date ? frxs.dateTimeFormat(rows[i].Sett_Date, "yyyy-MM-dd") : "") + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].VendorName) + "</td>";//供应商
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].SaleQty + "</td>";//销售数量
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleAmount + "</td>";//销售成本金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SalePoint + "</td>";//销售平台费用
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleAmount + "</td>";//销售金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleTotalAmount + "</td>";//合计销售金额

        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].BackQty + "</td>";//退货数量
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BackAmount + "</td>";//退货成本金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BackPoint + "</td>";//退货平台费用
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BackAmount + "</td>";//退货金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BackTotalAmount + "</td>";//合计退货金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].TotalPoint + "</td>";//合计平台费用
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].TotalAmount + "</td>";//合计成本金额
        //trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].TotalAmt + "</td>";//合计销售金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].净销售) ? "0" : rows[i].净销售) + "</td>";//净销售

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
        url: '../SalesReport/GetSalesReportByProductList',
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        rownumbers: true,                   //显示行编号
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            //查询条件
            nKind: 5,
            SubWID: $("#SubWID").combobox('getValue'),
            S1: $("#S1").val(),//开单开始时间
            S2: $("#S2").val(),//开单截止时间
            S3: $("#S3").val(),//供应商ID
            S4: $("#S4").val(),//采购员ID
            S5: $("#S5").val(),//商品名称
            S6: $("#S6").val(),//商品编号
            S7: $('#CategoriesId1').combobox('getValue'),//商品基本一级类ID
            S8: $('#CategoriesId2').combobox('getValue'),//商品二级类ID
            S10: $("#S10").val(),
            S11: $("#S11").val() ? ($("#S11").val() + " 23:59:59") : ""
        }
    });
}

function resetSearch() {
    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
  
    $("#S3").attr("value", "");
    $("#S4").attr("value", "");
    
    $("#S5").val('');
    $("#S6").val('');
    
    $('#CategoriesId1').combobox('setValue', '');
    $('#CategoriesId2').combobox('setValue', '');
    $('#CategoriesId3').combobox('setValue', '');
    
    $("#VendorCode").attr("value", "");
    $("#VendorName").attr("value", "");
    $("#BuyEmpName").attr("value", "");
    
    $("#S10").val('');
    $("#S11").val('');

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
    $("#S4").val(empId);
    $("#BuyEmpName").val(empName);
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

    $("#BuyEmpName").keydown(function (e) {
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
            var parseobj = JSON.parse(obj);
            if (parseobj.total == 1) {
                $("#S3").val(parseobj.rows[0].VendorID);
                $("#VendorCode").val(parseobj.rows[0].VendorCode);
                $("#VendorName").val(parseobj.rows[0].VendorName);
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
            EmpName: $("#BuyEmpName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var parseobj = JSON.parse(obj);
            if (parseobj.total == 1) {
                $("#S4").val(parseobj.rows[0].EmpID);
                $("#BuyEmpName").val(parseobj.rows[0].EmpName);
            } else {
                selBuyEmp();
            }
        }
    });
}


