
//门店编号
var ShopID;

//选取门店要结算单列表
$(function () {
    ShopID = frxs.getUrlParam("ShopID");

    loadGrid();

    //grid高度改变
    gridresize();

    //事件绑定
    eventBind();

});



function loadGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../SaleSettle/GetSaleSettleDetailList?ShopID=' + ShopID,         //Aajx地址
        idField: 'BillDetailsID',                  //主键
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: true,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            ShopID: ShopID
        },
        frozenColumns: [[
           { field: 'ck', checkbox: true },   //选择
           { title: '单据编号', field: 'BillID', width: 90 }
        ]],
        columns: [[
            { title: '帐单日期', field: 'BillDate', width: 100, formatter: frxs.dateFormat, align: 'center' },
            {
                // 单据类型(0:销售订单;1:销售退货单;2:销售费用单) 
                title: '单据类型', field: 'BillType', width: 70, align: 'left', formatter: function (value, rowData) {
                    var billTypeName = "";
                    if (rowData.BillType == 0) {
                        billTypeName = "销售订单";
                    }
                    else if (rowData.BillType == 1) {
                        billTypeName = "销售退货单";
                    }
                    else if (rowData.BillType == 2) {
                        billTypeName = "门店费用单";
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
            //, { title: '单据明细编号', field: 'BillDetailsID' }
        ]],
        toolbar: [{
            id: 'btnReload',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                //实现刷新栏目中的数据
                $("#grid").datagrid("reload");
            }
        }]
    });
}


//选取门店费用产生单据数据
function selectSaleSettleDetailsData() {
    var rows = $('#grid').datagrid('getSelections');
    window.frameElement.wapi.setSaleSettleDetailsData(rows);
    window.frameElement.wapi.focus();
    frxs.pageClose();
}

//事件绑定
function eventBind() {
    //绑定上下键事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        var index;
        //向下
        var selRow;
        if (ev.keyCode == 40) {
            //移除光标
            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            selRow = $("#grid").datagrid("getSelected");
            if (selRow != null) {
                index = $("#grid").datagrid("getRowIndex", $("#grid").datagrid("getSelected"));
                index = index + 1;
            } else {
                index = 0;
            }
            if (index > $("#grid").datagrid("getRows").length - 1) {
                index = 0;
            }
            $('#grid').datagrid('clearSelections');
            $("#grid").datagrid("selectRow", index);
        }

        //向上
        if (ev.keyCode == 38) {
            //移除光标
            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            selRow = $("#grid").datagrid("getSelected");
            if (selRow != null) {
                index = $("#grid").datagrid("getRowIndex", $("#grid").datagrid("getSelected"));
                index = index - 1;
            } else {
                index = $("#grid").datagrid("getRows").length - 1;
            }
            if (index < 0) {
                index = $("#grid").datagrid("getRows").length - 1;
            }
            $('#grid').datagrid('clearSelections');
            $("#grid").datagrid("selectRow", index);
        }

        //回车事件
        if (ev.keyCode == 13) {
            //判断是否有弹窗
            if ($("body").text().indexOf("没有选中行") < 0) {
                
                //获取焦点元素
                selectSaleSettleDetailsData();
            }
        }
        return true;
    };
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
    if (e.altKey && e.keyCode == 83) {
        selectSaleSettleDetailsData();
    }
    if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();

