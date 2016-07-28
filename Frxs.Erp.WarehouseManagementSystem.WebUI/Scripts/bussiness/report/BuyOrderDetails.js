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

    $("#CreateTime1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#CreateTime2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");
    $("#PostingTime1").val(frxs.nowDateTime("yyyy-MM-dd") + " 00:00");
    $("#PostingTime2").val(frxs.nowDateTime("yyyy-MM-dd") + " 23:59");

});

function initGrid() {
    $('#grid').datagrid({
        rownumbers: true,                   //显示行编号
        columns: [[
            { title: '用户单号', field: '用户单号', width: 150, align: 'left' },
            { title: '过账日', field: '过账日', width: 150, align: 'center' },
            { title: '日期', field: '日期', width: 80, align: 'center' },
            {
                title: '账期', field: '账期', width: 80, align: 'center', formatter: function (value) {
                    return value ? frxs.dateTimeFormat(value, "yyyy-MM-dd") : "";
                }
            },
            { title: '供应商', field: '供应商', width: 100, align: 'left', formatter: frxs.formatText },
            { title: '供应商编码', field: '供应商编码', width: 80, align: 'left', formatter: frxs.formatText },
            { title: '仓库', field: '仓库', width: 100, align: 'left' },
            { title: '开单人', field: '开单人', width: 100, align: 'left' },
            { title: '备注', field: '备注', width: 200, align: 'left', formatter: frxs.formatText },
            { title: '商品名称', field: '商品名称', width: 150, align: 'left', formatter: frxs.formatText },
            { title: '商品编码', field: '商品编码', width: 80, align: 'left', formatter: frxs.formatText },
            { title: '商品单位', field: '商品单位', width: 100, align: 'center' },
            { title: '规格', field: '规格', width: 100, align: 'left' },
            { title: '条码', field: '条码', width: 100, align: 'left' },
            {
                title: '包装数', field: '包装数', width: 100, align: 'right'
            },
            {
                title: '数量', field: '数量', width: 100, align: 'right',
                formatter: function (value) { return parseFloat(value).toFixed(4); }
            },
            {
                title: '散数', field: '散数', width: 100, align: 'right',
                formatter: function (value) { return parseFloat(value).toFixed(4); }
            },
            {
                title: '数量合计', field: '数量合计', width: 100, align: 'right',
                formatter: function (value) { return parseFloat(value).toFixed(4); }
            },
            {
                title: '不含税单价', field: '不含税单价', width: 100, align: 'center'
            },
            {
                title: '含税单价', field: '含税单价', width: 120, align: 'right'
            },
            {
                title: '不含税金额', field: '不含税金额', width: 120, align: 'right',
                formatter: function (value) { return parseFloat(value).toFixed(4); }
            },
            {
                title: '含税金额', field: '含税金额', width: 120, align: 'right',
                formatter: function (value) { return parseFloat(value).toFixed(4); }
            },
            { title: '品牌', field: '品牌', width: 120, align: 'left', formatter: frxs.formatText },
            //{ title: '主供应商', field: '主供应商', width: 80, align: 'left', formatter: frxs.formatText },
            { title: '商品分类', field: '商品分类', width: 80, align: 'left' },
            { title: '单位', field: '单位', width: 80, align: 'center' }
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
            ), "采购收货明细报表_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
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
    trtdCode += "<td style='height:24px'>用户单号</td>";
    trtdCode += "<td>过账日</td>";
    trtdCode += "<td>日期</td>";
    trtdCode += "<td>账期</td>";
    trtdCode += "<td>供应商</td>";
    trtdCode += "<td>供应商编码</td>";
    trtdCode += "<td>仓库</td>";
    trtdCode += "<td>开单人</td>";
    trtdCode += "<td>备注</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>商品编码</td>";
    trtdCode += "<td>商品单位</td>";
    trtdCode += "<td>规格</td>";
    trtdCode += "<td>条码</td>";
    trtdCode += "<td>包装数</td>";
    trtdCode += "<td>数量</td>";
    trtdCode += "<td>散数</td>";
    trtdCode += "<td>数量合计</td>";
    trtdCode += "<td>不含税单价</td>";
    trtdCode += "<td>含税单价</td>";
    trtdCode += "<td>不含税金额</td>";
    trtdCode += "<td>含税金额</td>";
    trtdCode += "<td>品牌</td>";
    //trtdCode += "<td>主供应商</td>";
    trtdCode += "<td>商品分类</td>";
    trtdCode += "<td>单位</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        
        trtdCode += "<tr>";
        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].用户单号 + "\">" + frxs.replaceCode(rows[i].用户单号) + "</td>";
        trtdCode += "<td>" + (rows[i].用户单号 == "合计" ? "" : rows[i].过账日) + "</td>";
        trtdCode += "<td>" + (rows[i].用户单号 == "合计" ? "" : rows[i].日期 + "</td>");
        trtdCode += "<td>" + (rows[i].账期 ? frxs.dateTimeFormat(rows[i].账期, "yyyy-MM-dd") : "") + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].供应商) + "</td>";
        trtdCode += rows[i].用户单号 == "合计" ? "<td></td>" : ("<td x:str=\"'" + rows[i].供应商编码 + "\">" + frxs.replaceCode(rows[i].供应商编码) + "</td>");
        trtdCode += "<td>" + frxs.replaceCode(rows[i].仓库) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].开单人) + "</td>";
        trtdCode += "<td width='380'>" + frxs.replaceCode(rows[i].备注) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].商品名称) + "</td>";
        trtdCode += rows[i].用户单号 == "合计" ? "<td></td>" : ("<td x:str=\"'" + rows[i].商品编码 + "\">" + rows[i].商品编码 + "</td>");
        trtdCode += "<td>" + (rows[i].用户单号 == "合计" ? "" : rows[i].商品单位 + "</td>");
        trtdCode += "<td>" + (rows[i].用户单号 == "合计" ? "" : rows[i].规格 + "</td>");
        trtdCode += rows[i].用户单号 == "合计" ? "<td></td>" : ("<td x:str=\"'" + rows[i].条码 + "\">" + rows[i].条码 + "</td>");
        trtdCode += rows[i].用户单号 == "合计" ? "<td></td>" : ("<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].包装数 + "</td>");
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].数量 + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].散数 + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].数量合计 + "</td>";
        trtdCode += rows[i].用户单号 == "合计" ? "<td></td>" : ("<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].不含税单价 + "</td>");
        trtdCode += rows[i].用户单号 == "合计" ? "<td></td>" : ("<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].含税单价 + "</td>");
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].不含税金额 + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].含税金额 + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].品牌) + "</td>";
        //trtdCode += "<td>" + frxs.replaceCode(rows[i].主供应商) + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].商品分类) + "</td>";
        trtdCode += "<td>" + (rows[i].用户单号 == "合计" ? "" : rows[i].单位) + "</td>";
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
        url: '../BuyOrderDetailsReport/GetList',
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
            CategoryId1: $('#CategoriesId1').combobox('getValue'),
            CategoryId2: $('#CategoriesId2').combobox('getValue'),
            CategoryId3: $('#CategoriesId3').combobox('getValue'),
            CreateTime1: $('#CreateTime1').val(),
            CreateTime2: $('#CreateTime2').val(),
            PostingTime1: $('#PostingTime1').val(),
            PostingTime2: $('#PostingTime2').val(),
            SKU: $('#SKU').val(),
            ProductName: $('#ProductName').val(),
            CreateUserName: $('#CreateUserName').val(),
            VendorCode: $('#VendorCode').val(),
            VendorName: $('#VendorName').val(),
            SettDateStart: $('#SettDateStart').val(),
            SettDateEnd: $('#SettDateEnd').val()
        }
    });
}




function resetSearch() {
    $("#SubWID").combobox('setValue', '');
    $('#CategoriesId1').combobox('setValue', '');
    $('#CategoriesId2').combobox('setValue', '');
    $('#CategoriesId3').combobox('setValue', '');

    $('#CreateTime1').val('');
    $('#CreateTime2').val('');
    $('#PostingTime1').val('');
    $('#PostingTime2').val('');

    $("#VendorCode").val('');
    $("#VendorName").val('');
    $("#VendorID").val('');

    $("#CreateUserName").val('');
    $("#SKU").val('');
    $("#ProductName").val('');
    
    $('#SettDateStart').val('');
    $("#SettDateEnd").val('');
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
    var h = ($(window).height() - $("fieldset").height() - 30);
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
    $("#VendorID").val(vendorId);
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
                $("#VendorID").val(obj.rows[0].VendorID);
                $("#VendorCode").val(obj.rows[0].VendorCode);
                $("#VendorName").val(obj.rows[0].VendorName);
            } else {
                selVendor();
            }
        }
    });
}
