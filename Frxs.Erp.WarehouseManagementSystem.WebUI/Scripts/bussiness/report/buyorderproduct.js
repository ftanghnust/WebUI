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

    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S5").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S6").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
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
           { title: '采购员姓名', field: 'EmpName', width: 120, align: 'center', formatter: frxs.formatText },
            { title: '单号', field: 'BuyId', width: 130, align: 'left' },
            { title: '供应商名称', field: 'VendorName', width: 410, align: 'left', formatter: frxs.formatText },
            
            {
                title: '数量', field: 'Qty', width: 130, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '配送金额', field: 'Amount', width: 150, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '录单日期', field: 'OrderDate', width: 130 },
            {
                title: '含税金额', field: 'FaxAmt', width: 130, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            }
        ]],
        toolbar: [
           {
               id: 'btnExport',
               text: '导出',
               iconCls: 'icon-daochu',
               handler: exportoutToPage//exportout
           }]
    });
}

function exportout() {
    location.href = "../PurchaseReport/ExportExcelPurchaseReport1?nKind=1&S1=" + $("#S1").val() +
        "&S2=" + $("#S2").val() +
        "&S3=" + $("#S3").val() +
        "&S4=" + $("#S4").val() +
        "&S5=" + $("#S5").val() +
        "&S6=" + $("#S6").val() +
        "&SubWID=" + $("#SubWID").combobox('getValue');
}


//查询
function search() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }

    $('#grid').datagrid({
        url: '../PurchaseReport/GetBuyOrderProductList',
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
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
            nKind: 1,
            //查询条件
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            S3: $("#S3").val(),
            S4: $("#S4").val(),
            S5: $("#S5").val(),
            S6: $("#S6").val(),
            SubWID: $("#SubWID").combobox('getValue')
        }
    });
}

function resetSearch() {
    $("#S1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S5").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#S6").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#S3").attr("value", "");
    $("#S4").attr("value", "");
    $("#VendorCode").attr("value", "");
    $("#VendorName").attr("value", "");
    $("#BuyEmpName").attr("value", "");
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
function backFillBuyEmp(empID, empName) {
    $("#S4").val(empID);
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
            EmpName: $("#BuyEmpName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            var obj = JSON.parse(obj);
            if (obj.total == 1) {
                $("#S4").val(obj.rows[0].EmpID);
                $("#BuyEmpName").val(obj.rows[0].EmpName);
            } else {
                selBuyEmp();
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
            ), "采购商品汇总查询导出_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td style='height:24px'>采购员姓名</td>";
    trtdCode += "<td>单号</td>";
    trtdCode += "<td>供应商名称</td>";
    trtdCode += "<td>数量</td>";
    trtdCode += "<td>配送金额</td>";
    trtdCode += "<td>录单日期</td>";
    trtdCode += "<td>含税金额</td>";

    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";

        trtdCode += "<td style='height:20px'>" + frxs.replaceCode(rows[i].EmpName) + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].BuyId + "\">" + rows[i].BuyId + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].VendorName) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + ((!rows[i].Qty) ? "0" : rows[i].Qty) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].Amount) ? "0" : rows[i].Amount) + "</td>";
        //避免导出空行为0
        trtdCode += "<td>" + (rows[i].VendorName == "小计" ? "" : rows[i].OrderDate) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + ((!rows[i].FaxAmt) ? "0" : rows[i].FaxAmt) + "</td>";

        trtdCode += "</tr>";
    }

    var dataCode = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
    dataCode = dataCode.replace("{table}", trtdCode);
    return dataCode;
}