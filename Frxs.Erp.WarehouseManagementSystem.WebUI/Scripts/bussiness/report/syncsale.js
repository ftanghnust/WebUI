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
            { title: '门店客户', field: 'cCusName', width: 400, align: 'left' },
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
                title: '提点金额', field: 'pCost', width: 130, align: 'right', formatter: function (value, rec) {
                    return parseFloat(rec.pCost).toFixed(4);
                }
            }
        ]]
    });
}

function syncbutton() {

    var SarteTime = $("#SarteTime").val();
    var EndTime = $("#EndTime").val();

    if (SarteTime != EndTime)
    {
        $.messager.alert("提示", "执行同步数据开始时间必须等于截止时间！", "info");
        return false;
    }

    $.messager.confirm("提示", "确定同步数据？", function (r) {
        if (r) {
            //禁用按钮
            var loading = window.top.frxs.loading("正在处理中，请稍后...");
            $.ajax({
                url: "../SyncReport/SyncButtonDate",
                type: "get",
                dataType: "json",
                data: {
                    SarteTime: SarteTime,
                    EndTime:EndTime,
                    SyncTableName: $("#Status").combobox("getValue")
                },
                success: function (result) {
                    loading.close();
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {
                            $.messager.alert("提示", "同步数据成功！", "info");
                            search();
                        } else {
                            $.messager.alert("提示", result.Info, "info");
                        }
                    }
                },
                error: function (request, textStatus, errThrown) {
                    loading.close();
                    if (textStatus) {
                        $.messager.alert("提示", textStatus, "info");
                    } else if (errThrown) {
                        $.messager.alert("提示", errThrown, "info");
                    } else {
                        $.messager.alert("提示", "出现错误", "info");
                    }
                }
            });
        }
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
            SyncTableName: 1           
        }
    });
}

function resetSearch() {
    $("#SarteTime").val(frxs.nowDateTime("yyyy-MM-dd"));
    $("#EndTime").val(frxs.nowDateTime("yyyy-MM-dd"));
   
}

function initDDL() {
   
}

//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totaliAmount = 0.0000;
    var totalcAmout = 0.0000;
    var totalpCost = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var iAmount = parseFloat(rows[i].iAmount);
        totaliAmount += iAmount;

        var cAmout = parseFloat(rows[i].cAmout);
        totalcAmout += cAmout;

        var pCost = parseFloat(rows[i].pCost);
        totalpCost += pCost;
    }

   
    $('#grid').datagrid('reloadFooter', [
       { "cCusName": "<div style='width:100%;text-align:right'>合计：</div>", "iAmount": parseFloat(totaliAmount).toFixed(4), "cAmout": parseFloat(totalcAmout).toFixed(4), "pCost": parseFloat(totalpCost).toFixed(4) }
      
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
