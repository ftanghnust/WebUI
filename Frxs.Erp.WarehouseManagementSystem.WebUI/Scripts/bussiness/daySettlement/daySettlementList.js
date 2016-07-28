$(function () {
    
    //仓库初始化
    ddlInit();
    
    //重置按钮事件
    $("#aReset").click(function () {
        $("#SerchTime").val('');
        var data = $('#WID').combobox('getData');
        $("#WID").combobox('select', data[0].WID);
    });

});

//搜索
function search() {
    //实现刷新栏目中的数据
    initGrid();
}

//初始化查询
function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../DaySettlement/DaySettlementListData',          //Aajx地址
        idField: 'ID',                  //主键
        pageSize: 30,                       //每页条数
        pageList: [10, 30, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
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
        onDblClickCell: function () {
            var selRow = $("#grid").datagrid("getSelected");
            if (selRow.ID) {
                var settleDate = selRow.SettleDate.DataFormat("yyyy-MM-dd");
                frxs.openNewTab("查看日结算详细" + settleDate, "../DaySettlement/DaySettlementDetail?Id=" + selRow.ID, "icon-search");
            }
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        queryParams: {
            SubWID: $("#WID").combobox("getValue"),
            SerchTime: $("#SerchTime").val(),
            StockName: $("#WID").combobox("getText")
        },
        frozenColumns: [[
             {
                 title: '过帐日', field: 'SettleDate', width: 100, align: 'center', formatter: function (value, rec) {
                     if (value) {
                         return value.DataFormat("yyyy-MM-dd");
                     }
                 }
             },
            { title: '执行仓库', field: 'WorkStockName', width: 180 }
        ]],
        columns: [[
            { title: '是否执行', field: 'IsWork', width: 80, align: 'center', },
            { title: '执行方式', field: 'StatusName', width: 120 },
            {
                title: '执行时间', field: 'ModifyTime', width: 120, formatter: function (value) {
                    if (value) {
                        return value.DataFormat("yyyy-MM-dd HH:mm");
                    }
                }
            },
            {
                title: '操作', field: 'Opt', width: 180, align: 'center', formatter: function (value, rec) {
                    var code = "<a class='rowbtn' onclick=\"execute('"+rec.SettleDate+"')\">手动执行</a>";
                    if (rec.ID) {
                        code = "<a class='rowbtn' onclick=\"look('" + rec.ID + "','" + rec.SettleDate + "')\">查看</a>";
                        //暂时不需要改功能
                        //code += "<a class='rowbtn' onclick=\"againExecute('" + rec.SettleDate + "','" + rec.WID + "','" + rec.SubWID + "')\">重新计算</a>";
                        code += "<a class='rowbtn' onclick=\"importData('" + rec.ID + "')\">导出</a>";
                    }

                    return code;
                }
            },
            {
                title: '期初库存数量', field: 'BeginQty', width: 120, hidden: false, align: 'right', formatter: function (value) {
                return parseFloat(value).toFixed(2);
                }
            },
            {
                title: '期初库存金额', field: 'BeginAmt', width: 120, hidden: false, align: 'right', formatter: function (value) {
                return parseFloat(value).toFixed(4);
            } },
            {
                title: '采购数量', field: 'BuyQty', width: 120, hidden: false, align: 'right', formatter: function (value) {
                return parseFloat(value).toFixed(2);
            } },
            {
                title: '采购金额', field: 'BuyAmt', width: 120, hidden: false, align: 'right', formatter: function (value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '采购退货数量', field: 'BuyBackQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '采购退货金额', field: 'BuyBackAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '销售数量', field: 'SaleQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '销售金额', field: 'SaleAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '销售退货数量', field: 'SaleBackQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '销售退货金额', field: 'SaleBackAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '盘盈数量', field: 'AdjGainQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '盘盈金额', field: 'AjgGginAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '盘亏数量', field: 'AdjLossQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '盘亏金额', field: 'AjgLosssAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '库存数量', field: 'StockQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '库存金额', field: 'StockAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '期末库存数量', field: 'EndQty', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '期末库存金额', field: 'EndStockAmt', width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } },
            { title: '差异数量', field: 'EndDiffQty' , width: 120, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(2);
            } },
            { title: '差异金额', field: 'EndDiffStockAmt', width: 160, hidden: false,align: 'right',formatter:function(value) {
                return parseFloat(value).toFixed(4);
            } }
        ]]
    });
}


//查看
function look(id,settleDate) {
    if (id) {
        settleDate = settleDate.DataFormat("yyyy-MM-dd");
        frxs.openNewTab("查看日结算详细" + settleDate, "../DaySettlement/DaySettlementDetail?Id=" + id, "icon-search");
    }
}

//执行
function execute(settleDate) {
    $.messager.confirm("提示", "确定执行[" + frxs.dateTimeFormat(settleDate, "yyyy-MM-dd") + "]日结？", function (r) {
        if (r) {
            var load = frxs.loading();
            $.ajax({
                url: "../DaySettlement/DaySettlementExecute",
                type: "post",
                data: {
                    subWId: $("#WID").combobox("getValue"),
                    settleDate: settleDate
                },
                dataType: 'json',
                success: function (obj) {
                    load.close();
                    if (obj.Flag == "SUCCESS") {
                        $.messager.alert("提示", "执行成功", "info");
                        $("#grid").datagrid("reload");
                    } else {
                        $.messager.alert("提示", "执行失败：" + obj.Info, "info");
                    }
                }
            });
        }
    });
    
}


//导出
function importData(id) {
    var load = frxs.loading("正在导出，请稍候...");
    $.ajax({
        url: "../DaySettlement/DaySettlementDetilData",
        type: "post",
        data: {
            page: 1,
            rows: 100000,
            SKU: '',
            ProductName: '',
            RefSet_ID: id
        },
        dataType: 'json',
        success: function (data) {
            load.close();
            exportExcel(data);
        }
    });
}


//客户端导出Excel
function exportExcel(data) {
    var rows = data.rows;
    //var settCurrentSum = data.settCurrentSum;
    var settTotalSum = data.settTotalSum;
    
    var trtdCode = "<tr>";
    trtdCode += "<td style='height:24px'>商品编码</td>";
    trtdCode += "<td>商品名称</td>";
    trtdCode += "<td>库存单位</td>";
    trtdCode += "<td>期初库存数量</td>";
    trtdCode += "<td>期初库存金额</td>";
    trtdCode += "<td>采购数量</td>";
    trtdCode += "<td>采购金额</td>";
    trtdCode += "<td>采购退货数量</td>";
    trtdCode += "<td>采购退货金额</td>";
    trtdCode += "<td>销售数量</td>";
    trtdCode += "<td>销售金额</td>";
    trtdCode += "<td>销售退货数量</td>";
    trtdCode += "<td>销售退货金额</td>";
    trtdCode += "<td>盘盈数量</td>";
    trtdCode += "<td>盘盈金额</td>";
    trtdCode += "<td>盘亏数量</td>";
    trtdCode += "<td>盘亏金额</td>";
    trtdCode += "<td>库存数量</td>";
    trtdCode += "<td>库存金额</td>";
    trtdCode += "<td>期末库存数量</td>";
    trtdCode += "<td>期末库存金额</td>";
    trtdCode += "<td>差异数量</td>";
    trtdCode += "<td>差异金额</td>";
    trtdCode += "</tr>";

    for (var i = 0; i < rows.length; i++) {
        trtdCode += "<tr>";

        trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].SKU + "\">" + rows[i].SKU + "</td>";
        trtdCode += "<td>" + frxs.replaceCode(rows[i].ProductName) + "</td>";
        trtdCode += "<td>" + rows[i].Unit + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].BeginQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BeginAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].BuyQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BuyAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].BuyBackQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].BuyBackAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].SaleQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].SaleBackQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].SaleBackAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].AdjGainQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].AjgGginAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].AdjLossQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].AjgLosssAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].StockQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].StockAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].EndQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].EndStockAmt + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].EndDiffQty + "</td>";
        trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + rows[i].EndDiffStockAmt + "</td>";
        trtdCode += "</tr>";
    }

    //总计
    trtdCode += "<tr>";
    trtdCode += "<td style='height:20px'>总计：</td>";
    trtdCode += "<td></td>";
    trtdCode += "<td></td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumBeginQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumBeginAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumBuyQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumBuyAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumBuyBackQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumBuyBackAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumSaleQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumSaleAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumSaleBackQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumSaleBackAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumAdjGainQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumAjgGginAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumAdjLossQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumAjgLosssAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumStockQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumStockAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumEndQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumEndStockAmt + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + settTotalSum.SumEndDiffQty + "</td>";
    trtdCode += "<td style='mso-number-format:\"#,##0.0000\";'>" + settTotalSum.SumEndDiffStockAmt + "</td>";
    trtdCode += "</tr>";

    var text = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
    text = text.replace("{table}", trtdCode);
    if (text) {

        var settleDate = $("#grid").datagrid("getSelected").SettleDate.DataFormat("yyyy-MM-dd");

        event.preventDefault();
        var bb = self.Blob;
        saveAs(
            new bb(
                ["\ufeff" + text] //\ufeff防止utf8 bom防止中文乱码
                , { type: "html/plain;charset=utf8" }
            ), "日结算单详细（" + settleDate + "）.xls"
        );
    }
}



//重新计算
function againExecute(settleDate,wid ,subWid) {
    var load = frxs.loading();
    $.ajax({
        url: "../DaySettlement/DaySettlementAgainExecute",
        type: "post",
        data: {
            settleDate: settleDate,
            wid: wid,
            subWid: subWid,
        },
        dataType: 'json',
        success: function (obj) {
            load.close();
            if (obj.Flag == "SUCCESS") {
                $.messager.alert("提示", "执行成功", "info");
                $("#grid").datagrid("reload");
            } else {
                $.messager.alert("提示", "执行失败：" + obj.Info, "info");
            }
        }
    });
}

//仓库初始化
function ddlInit() {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            //创建控件
            $("#WID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#WID").combobox('select', data[0].WID);
            
            //grid绑定
            initGrid();
            //grid高度改变
            gridresize();
        }
    });
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 20);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}