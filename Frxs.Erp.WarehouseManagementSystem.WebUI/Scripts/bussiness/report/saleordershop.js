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


    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
});


function GetDateStr(AddDayCount) {
    var dd = new Date();
    dd.setDate(dd.getDate() + AddDayCount);//获取AddDayCount天后的日期 
    var y = dd.getFullYear();
    var m = dd.getMonth() + 1;//获取当前月份的日期 
    var d = dd.getDate();
    return y + "-" + m + "-" + d;
}

function initGrid() {
    $('#grid').datagrid({
        rownumbers: true,                   //显示行编号
        columns: [[
            { title: '门店编号', field: 'ShopCode', width: 130, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 300 },
            {
                title: '过账日', field: 'Sett_Date', width: 100, formatter: function (value) { return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : ""; }
            },
            {
                title: '销售数量', field: 'SaleQty', width: 100, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '销售成本金额', field: 'SaleAmount', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '销售平台费用', field: 'SalePoint', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
             {
                 title: '销售金额', field: 'SaleAmount_1', width: 120, align: 'right', formatter: function (value, rec) {
                     value = rec.SaleAmount ? rec.SaleAmount : 0;
                     return parseFloat(value).toFixed(4);
                 }
             },
             {
                 title: '合计销售金额', field: 'SaleTotalAmount', width: 130, align: 'right', formatter: function (value) {
                     value = value ? value : 0;
                     return parseFloat(value).toFixed(4);
                 }
             },
             {
                 title: '门店积分', field: '门店积分', width: 130, align: 'right', formatter: function (value) {
                     value = value ? value : 0;
                     return parseFloat(value).toFixed(2);
                 }
             },
            {
                title: '退货数量', field: 'BackQty', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '退货成本金额', field: 'BackAmount', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '退货平台费用', field: 'BackPoint', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '退货金额', field: 'BackAmount_1', width: 120, align: 'right', formatter: function (value,rec) {
                    value = rec.BackAmount ? rec.BackAmount : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
             {
                 title: '合计退货金额', field: 'BackTotalAmount', width: 130, align: 'right', formatter: function (value) {
                     value = value ? value : 0;
                     return parseFloat(value).toFixed(4);
                 }
             },
            {
                title: '合计平台费用', field: 'TotalPoint', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '合计成本金额', field: 'TotalAmount', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },
            {
                title: '净销售', field: '净销售', width: 120, align: 'right', formatter: function (value) {
                    value = value ? value : 0;
                    return parseFloat(value).toFixed(4);
                }
            },            
            //{
            //    title: '合计销售金额', field: 'TotalAmt', width: 110, align: 'right', formatter: function (value) {
            //        return parseFloat(value).toFixed(4);
            //    }
            //},
            //{
            //    title: '合计销售金额', field: '合计销售', width: 110, align: 'right', formatter: function (value) {
            //        value = value ? value : 0;
            //        return parseFloat(value).toFixed(4);
            //    }
            //},
            //{
            //    title: '合计退货金额', field: '退货合计', width: 110, align: 'right', formatter: function (value) {
            //        value = value ? value : 0;
            //        return parseFloat(value).toFixed(4);
            //    }
            //},
            
            {
                title: '线路名称', field: 'LineName', width: 130
            }

        ]],
        toolbar: [
           {
               id: 'btnExport',
               text: '导出',
               iconCls: 'icon-daochu',
               handler: exportoutToPage //exportout
           }]
    });
}

function exportout() {
    location.href = "../SalesReport/ExportExcelSalesReport1?nKind=1&S1=" + $("#S1").val() +
        "&S2=" + $("#S2").val() +
        "&S3=" + $("#S3").val() +
        "&S4=" + $("#S4").val() +
        "&S5=" + $("#S5").val() +
        "&S6=" + $("#S6").combobox('getValue');
}


//查询
function search() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }
    $('#grid').datagrid({
        url: '../SalesReport/GetGetCustomerSaleReportList',//(1)客户销售情况汇总表
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
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            S3: $("#S3").val(),
            S4: $("#S4").val(),
            S5: $("#S5").val(),
            S6: $("#S6").combobox('getValue'),
            nKind: 1,
            SubWID: $("#SubWID").combobox('getValue'),
            S10: $("#S10").val(),
            S11: $("#S11").val() ? ($("#S11").val() + " 23:59:59") : ""
        }
    });
}

function resetSearch() {
    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S3").attr("value", "");

    $("#S4").val('');
    $("#S5").val('');
    $("#S6").combobox('setValue', '');

    $("#ShopCode").attr("value", "");
    $("#ShopName").attr("value", "");
    
    $("#S10").val('');
    $("#S11").val('');

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
        }
    });

    $.ajax({
        url: '../Common/GetWarehouseLineList',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data.unshift({ "LineID": "", "LineName": "-请选择-" });
            //创建控件
            $("#S6").combobox({
                data: data,                       //数据源
                valueField: "LineID",       //id列
                textField: "LineName"       //value列
            });
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

function selShop() {
    var shopCode = $("#ShopCode").val();
    var shopName = $("#ShopName").val();
    frxs.dialog({
        title: "选择门店",
        url: "../SaleBackPre/SelectShop?shopCode=" + encodeURIComponent(shopCode) + "&shopName=" + encodeURIComponent(shopName),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填门店
function backFillShop(shopID, shopCode, shopName) {
    $("#S3").val(shopID);
    $("#ShopCode").val(shopCode);
    $("#ShopName").val(shopName);
}


//供门店 回车事件绑定
function eventBind() {
    $("#ShopCode").keydown(function (e) {
        if (e.keyCode == 13) {
            eventShop();
        }
    });

    $("#ShopName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventShop();
        }
    });
}

//门店 回车事件
function eventShop() {
    $.ajax({
        url: "../Common/GetShopInfo",
        type: "post",
        data: {
            shopCode: $("#ShopCode").val(),
            shopName: $("#ShopName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#S3").val(obj.rows[0].ShopID);
                $("#ShopCode").val(obj.rows[0].ShopCode);
                $("#ShopName").val(obj.rows[0].ShopName);
            } else {
                selShop();
            }
        }
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
            ), "客户销售情况汇总查询导出_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td style='height:24px'>门店编号</td>";
    trtdCode += "<td>门店名称</td>";
    trtdCode += "<td>过账日</td>";
    trtdCode += "<td>销售数量</td>";
    trtdCode += "<td>销售成本金额</td>";
    trtdCode += "<td>销售平台费用</td>";
    trtdCode += "<td>销售金额</td>";
    trtdCode += "<td>合计销售金额</td>";
    trtdCode += "<td>门店积分</td>";

    trtdCode += "<td>退货数量</td>";
    trtdCode += "<td>退货成本金额</td>";
    trtdCode += "<td>退货平台费用</td>";
    trtdCode += "<td>退货金额</td>";
    trtdCode += "<td>合计退货金额</td>";
    trtdCode += "<td>合计平台费用</td>";
    trtdCode += "<td>合计成本金额</td>";  
    //trtdCode += "<td>合计销售</td>";  
    trtdCode += "<td>净销售</td>";    
    trtdCode += "<td>线路名称</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";

        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].ShopCode + "\">" + rows[i].ShopCode + "</td>";//门店编号
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ShopName) + "</td>";//门店名称
        trtdCode += "<td>" + (rows[i].Sett_Date ? frxs.dateTimeFormat(rows[i].Sett_Date, "yyyy-MM-dd") : "") + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + ((!rows[i].SaleQty) ? "0" : rows[i].SaleQty) + "</td>";                    //销售数量 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].SaleAmount) ? "0" : rows[i].SaleAmount) + "</td>";            //销售成本金额 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].SalePoint) ? "0" : rows[i].SalePoint) + "</td>";              //销售平台费用 浏览器中显示0为空，导出时统一为0
        //****trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].SaleTotalAmount) ? "0" : rows[i].SaleTotalAmount) + "</td>";  //销售金额 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].SaleAmount) ? "0" : rows[i].SaleAmount) + "</td>";  //销售金额=销售成本金额 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].SaleTotalAmount) ? "0" : rows[i].SaleTotalAmount) + "</td>";//合计销售金额
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + ((!rows[i].门店积分) ? "0" : rows[i].门店积分) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + ((!rows[i].BackQty) ? "0" : rows[i].BackQty) + "</td>";                    //退货数量 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].BackAmount) ? "0" : rows[i].BackAmount) + "</td>";            //退货成本金额 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].BackPoint) ? "0" : rows[i].BackPoint) + "</td>";              //退货平台费用 浏览器中显示0为空，导出时统一为0
        //***trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].BackTotalAmount) ? "0" : rows[i].BackTotalAmount) + "</td>";  //退货金额 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].BackAmount) ? "0" : rows[i].BackAmount) + "</td>";  //退货金额=退货成本金额 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].BackTotalAmount) ? "0" : rows[i].BackTotalAmount) + "</td>";//合计退货金额
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].TotalPoint) ? "0" : rows[i].TotalPoint) + "</td>";            //合计平台费用 浏览器中显示0为空，导出时统一为0
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].TotalAmount) ? "0" : rows[i].TotalAmount) + "</td>";          //合计成本金额 浏览器中显示0为空，导出时统一为0
        //trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].TotalAmt) ? "0" : rows[i].TotalAmt) + "</td>";                //合计销售金额 浏览器中显示0为空，导出时统一为0
        //trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].合计销售) ? "0" : rows[i].合计销售) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].净销售) ? "0" : rows[i].净销售) + "</td>";//净销售
        trtdCode += "<td>" + frxs.replaceCode(rows[i].LineName) + "</td>";                                                                      //线路名称

        trtdCode += "</tr>";
    }

    var dataCode = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
    dataCode = dataCode.replace("{table}", trtdCode);
    return dataCode;
}