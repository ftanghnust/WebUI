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
        columns: [[
            { title: '单号', field: 'BillId', width: 130, align: 'center' },
            { title: '公司机构', field: 'WName', width: 160, align: 'left' },
            {
                title: '过账日', field: 'Sett_Date', width: 100, align: 'center', formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            { title: '开单日期', field: 'FeeDate', width: 100, align: 'center' },
            { title: '门店编号', field: 'ShopCode', width: 120, align: 'left' },
            { title: '门店名称', field: 'ShopName', width: 300, align: 'left', formatter: frxs.formatText },
            { title: '项目名称', field: 'FeeName', width: 100, align: 'center', formatter: frxs.formatText },
            {
                title: '金额', field: 'FeeAmt', width: 150, align: 'right', formatter: function (value) {
                    value = (!value) ? 0 : value;
                    return parseFloat(value).toFixed(4);
                }
            },
            { title: '录单人员', field: 'CreateUserName', width: 100, align: 'center', formatter: frxs.formatText },
            { title: '确认人员', field: 'ConfUserName', width: 100, align: 'center', formatter: frxs.formatText },
            { title: '过帐时间', field: 'PostingTime', width: 100, align: 'left' },
            { title: '过帐人员', field: 'ConfUserName', width: 100, align: 'center' },
            
            { title: '备注', field: 'Remark', width: 150, align: 'left',formatter:frxs.formatText }
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
            ), "门店费用单导出_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td>公司机构</td>";
    trtdCode += "<td>过账日</td>";
    trtdCode += "<td>开单日期</td>";
    trtdCode += "<td>门店编号</td>";
    trtdCode += "<td>门店名称</td>";
    trtdCode += "<td>项目名称</td>";
    trtdCode += "<td>金额</td>";
    trtdCode += "<td>录单人员</td>";
    trtdCode += "<td>确认人员</td>";
    trtdCode += "<td>过帐时间</td>";
    trtdCode += "<td>过帐人员</td>";
    trtdCode += "<td>备注</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";

        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].BillId + "\">" + rows[i].BillId + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].WName) + "</td>";
        trtdCode += "<td>" + (rows[i].Sett_Date ? frxs.dateTimeFormat(rows[i].Sett_Date, "yyyy-MM-dd") : "") + "</td>";
        trtdCode += "<td>" + rows[i].FeeDate + "</td>";
        trtdCode += "<td x:str=\"'" + rows[i].ShopCode + "\">" + rows[i].ShopCode + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ShopName) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].FeeName) + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].FeeAmt + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].CreateUserName) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ConfUserName) + "</td>";
        trtdCode += "<td>" + rows[i].PostingTime + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ConfUserName) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].Remark) + "</td>";
        
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
        url: '../SalesReport/GetCustomerExpSumList',
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
            //查询条件
            S1: $("#S1").val(),
            S2: $("#S2").val(),
            S3: $("#S3").val(),
            nKind: 2,
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

    $("#ShopCode").attr("value", "");
    $("#ShopName").attr("value", "");

    $("#S10").val("");
    $("#S11").val("");
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