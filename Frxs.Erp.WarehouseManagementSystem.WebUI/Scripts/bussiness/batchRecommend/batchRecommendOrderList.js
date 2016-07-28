$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();
});

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WarehouseOrder/GetWarehouseOrderList',          //Aajx地址
        //sortName: 'OrderDate',                 //排序字段
        //sortOrder: 'desc',                  //排序方式
        idField: 'OrderId',                  //主键
        pageSize: 0,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        //onClickRow: function (rowIndex) {
        //    $('#grid').datagrid('clearSelections');
        //    $('#grid').datagrid('selectRow', rowIndex);
        //},
        //onDblClickRow: function () {
        //    var rows = $('#grid').datagrid('getSelections');
        //    frxs.openNewTab("查看门店订单" + rows[0].OrderId, "../WarehouseOrder/WarehouseOrderAddOrEdit?OrderID=" + rows[0].OrderId, "icon-search");
        //},
        queryParams: {
            //查询条件
            OrderId: $("#OrderId").val(),
            Status: 101,  //101
            ShopCode: $("#ShopCode").val(),
            ShopName: $("#ShopName").val(),
            LineID: $("#LineName").combobox("getValue"),
            SendDateBegin: $("#SendStartDate").val(),
            SendDateEnd: $("#SendEndDate").val(),
            OrderDateBegin: $("#OrderStartDate").val(),
            OrderDateEnd: $("#OrderEndDate").val()
        },
        frozenColumns: [[
              { field: 'ck', checkbox: true },
            { title: '订单编号', field: 'OrderId', width: 120, align: 'center' }
            //冻结列
        ]],
        columns: [[

            { title: '下单时间', field: 'OrderDate', width: 110, formatter: frxs.dateFormat, align: 'center' },
             { title: '状态', field: 'StatusName', width: 100, align: 'center' },
            //{ title: '确认时间', field: 'ConfDate', width: 120, formatter: frxs.dateFormat },
            { title: '门店类型', field: 'ShopTypeName', width: 100, align: 'center' },
            { title: '门店编号', field: 'ShopCode', width: 100, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 190 },
            {
                title: '预计配送日期', field: 'SendDate', width: 110, formatter: function (value, rec) {
                    if (value) {
                        return value.DataFormat("yyyy-MM-dd");
                    }
                }, align: 'center'
            },
            //{ title: '缺货率', field: 'StockOutRate', width: 70, align: 'right' },
            { title: '订单金额', field: 'PayAmount', width: 100, align: 'right' },
            { title: '配送线路', field: 'LineName', width: 100 },
            //{ title: '配送日期', field: 'ShippingBeginDate', width: 120, formatter: frxs.dateFormat },
            //{ title: '待装区编号', field: 'StationNumber', width: 80, align: 'center' },

            { title: '门店ID', field: 'ShopID', width: 80, hidden: true },
            { title: '线路ID', field: 'LineID', width: 80, hidden: true },
            { title: '状态', field: 'Status', width: 80, hidden: true }
        ]]
    });
}


//添加订单
function addOrder() {
    window.frameElement.wapi.reloadOrders($('#grid'));
    window.frameElement.wapi.focus();//确定添加后在关闭弹窗之前让父窗体获得当前焦点，为进一步实现快捷键
    frxs.pageClose();
}

//查询
function search() {
    $('#grid').datagrid('clearSelections');
    initGrid();
}

//重置
function resetSearch() {
    $("#searchform").form("clear");

    var data1 = $('#LineName').combobox('getData');  //赋默认值
    $("#LineName ").combobox('setValue', data1[0].LineID);

    var data2 = $('#WName').combobox('getData');  //赋默认值
    $("#WName ").combobox('setValue', data2[0].WID);

    $('#Status').combobox('setValue', '');
    $('#ShopType').combobox('setValue', '');
    $('#OrderType').combobox('setValue', '');
}


//下拉控件数据初始化
function initDDL() {
    $.ajax({
        url: '../Common/GetWarehouseLineList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "LineID": "", "LineName": "-全部-" });
            }
            //创建控件
            $("#LineName").combobox({
                data: data,             //数据源
                valueField: "LineID",       //id列
                textField: "LineName"      //value列
            });
            $("#LineName").combobox('select', data[0].LineID);
        }, error: function (e) {
            debugger;
        }
    });

    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#WName").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#WName").combobox('select', data[0].WID);
        }, error: function (e) {
            debugger;
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



//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        addOrder();//弹窗提交

    }
    else if (e.keyCode == 27) {

        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭


    }
});
window.focus();