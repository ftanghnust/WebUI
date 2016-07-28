$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();
 
    //grid高度改变
    gridresize();
    //select下拉框自适应高度    
    $('.easyui-combobox').combobox({
        panelHeight: 'auto'
    });

    $("#SarteTime").val(frxs.nowDateTime("yyyy-MM-dd"));
    $("#EndTime").val(frxs.nowDateTime("yyyy-MM-dd"));
});



function initGrid() {
    $('#grid').datagrid({
        rownumbers: true,                   //显示行编号
        columns: [[
            { title: '单号', field: 'cVouchID', width: 130, align: 'left' },
            { title: '时间', field: 'dVouchDate', width: 160, align: 'left' },
            { title: '供应商', field: 'cVenName', width: 400, align: 'left' },
            {
                title: '金额', field: 'iAmount', width: 150, align: 'right', formatter: function (value) {
                    return parseFloat(value).toFixed(4);
                }
            },          
            {
                title: '商品金额', field: 'cAmout', width: 130, align: 'right', formatter: function (value, rec) {
                    return parseFloat(rec.cAmout).toFixed(4);
                }
            }
            ,
            {
                title: '物流金额', field: 'lCost', width: 130, align: 'right', formatter: function (value, rec) {
                    return parseFloat(rec.lCost).toFixed(4);
                }
            }
             ,
            {
                title: '仓储金额', field: 'wCost', width: 130, align: 'right', formatter: function (value, rec) {
                    return parseFloat(rec.wCost).toFixed(4);
                }
            }
        ]]
    });
}




//查询
function search() {
    var validate = $("#myForm").form('validate');
    if (!validate) {
        return false;
    }

    $('#grid').datagrid({
        url: '../SyncReport/GetSyncReportData',
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
        showFooter: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            totalCalculate();
        },
        queryParams: {
            //查询条件
            SarteTime: $("#SarteTime").val(),
            EndTime: $("#EndTime").val(),
            Status: $("#Status").combobox("getValue"),
            SyncTableName: 0           
        }
    });
}

function resetSearch() {
    $("#SarteTime").val(frxs.nowDateTime("yyyy-MM-dd"));
    $("#EndTime").val(frxs.nowDateTime("yyyy-MM-dd"));
   
}

//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totaliAmount = 0.0000;
    var totalcAmout = 0.0000;
    var totalwCost = 0.0000;
    var totallCost = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var iAmount = parseFloat(rows[i].iAmount);
        totaliAmount += iAmount;

        var cAmout = parseFloat(rows[i].cAmout);
        totalcAmout += cAmout;

        var wCost = parseFloat(rows[i].wCost);
        totalwCost += wCost;

        var lCost = parseFloat(rows[i].lCost);
        totallCost += lCost;
    }


    $('#grid').datagrid('reloadFooter', [
       { "cCusName": "<div style='width:100%;text-align:right'>合计：</div>", "iAmount": parseFloat(totaliAmount).toFixed(4), "cAmout": parseFloat(totalcAmout).toFixed(4), "lCost": parseFloat(totallCost).toFixed(4), "wCost": parseFloat(totalwCost).toFixed(4) }

    ]);
}

function initDDL() {
   
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
