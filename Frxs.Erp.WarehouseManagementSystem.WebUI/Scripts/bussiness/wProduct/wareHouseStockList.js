//商品仓库价格列表
var productId;
$(function () {
    productId = frxs.getUrlParam("ProductId");

    //grid绑定
    initGrid();

    //grid高度改变
    gridresize();
});

//初始化查询
function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WProduct/GetWareHouseStockList?productId=' + productId,          //Aajx地址
        sortName: 'WName',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        queryParams: {
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: '子仓库名称', field: 'SubWarehouseName', width: 160, align: 'left' },
            { title: '库存', field: 'StockQty', width: 160, align: 'left' }
        ]]
    });
}

//grid高度改变
function gridresize() {
    var h = ($(window).height() - 2);
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
