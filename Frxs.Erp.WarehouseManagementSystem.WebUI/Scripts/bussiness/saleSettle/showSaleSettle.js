
//结算编号
var SettleID;

//销售结算单列表
$(function () {
    SettleID = frxs.getUrlParam("settleId");
    //加载初始化数据
    initData();

    //grid高度改变
    gridresize();

});


//加载仓库编号和结算方式下拉列表
function initData() {
    var load = frxs.loading("正在加载...");
    $.ajax({
        url: '../SaleSettle/GetSaleSettleInfo',
        type: 'post',
        dataType: 'json',
        data: {
            SettleID: SettleID
        },
        success: function (obj) {
            load.close();
            $("#ShowSaleSettleForm").form("load", obj.Data.SaleSettle);
            $("#SettleAmt").val(obj.Data.SaleSettle.SettleAmt.toFixed(2));
            //明细数据加载
            loadGrid(obj.Data.SaleSettleDetailList);
            totalCalculate(obj.Data.SaleSettleDetailList);

        }, error: function (e) {
            debugger;
        }
    });
};

function loadGrid(griddata) {
    $('#grid').datagrid({
        title: '',                      //标题
        width: 780,
        height: 400,
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        data: griddata,
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: true,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        showFooter: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: '单据编号', field: 'BillID', width: 80 },
            { title: '帐单日期', field: 'BillDate', width: 80 },
            {
                // 单据类型(0:销售订单;1:销售退货单;2:销售费用单) 
                title: '单据类型', field: 'BillType', width: 60, align: 'left', formatter: function (value, rowData) {
                    var billTypeName = "";
                    if (rowData.BillType == 0) {
                        billTypeName = "销售订单";
                    }
                    else if (rowData.BillType == 1) {
                        billTypeName = "销售退货单";
                    }
                    else if (rowData.BillType == 2) {
                        billTypeName = "销售费用单";
                    }
                    else {
                        billTypeName = rowData.BillType;
                    }
                    return billTypeName;
                }
            },
            { title: '二级项目名称', field: 'FeeName', width: 80 },
            { title: '金额', field: 'BillAmt', width: 120 },
            { title: '平台费', field: 'BillAddAmt', width: 120 },
            { title: '小计', field: 'BillPayAmt', width: 120 }
        ]]
    });
}


//总额计算
function totalCalculate(rows) {
    var totalBillAmt = 0.0000;
    var totalBillAddAmt = 0.0000;
    var totalBillPayAmt = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var billAmt = parseFloat(rows[i].BillAmt);
        var billAddAmt = parseFloat(rows[i].BillAddAmt);
        var billPayAmt = parseFloat(rows[i].BillPayAmt);
        totalBillAmt += billAmt;
        totalBillAddAmt += billAddAmt;
        totalBillPayAmt += billPayAmt;
    }
    var billAmt1 = "金额总计:" + parseFloat(totalBillAmt).toFixed(4);
    var billAddAmt1 = "平台费总计:" + parseFloat(totalBillAddAmt).toFixed(4);
    var billPayAmt1 = "小计总计:" + parseFloat(totalBillPayAmt).toFixed(4);
    $('#grid').datagrid('reloadFooter', [
        {
            "BillAmt": billAmt1,
            "BillAddAmt": billAddAmt1,
            "BillPayAmt": billPayAmt1
        }
    ]);
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

//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();

