$(function () {

    //grid绑定
    initGrid();

    initDll();

    //grid高度改变
    gridresize();



    document.onkeydown = function (e) {
        if (!e) e = window.event;//火狐中是 window.event
        if ((e.keyCode || e.which) == 13) {
            $("#aSearch").click();
        }
    }
});

function initGrid() {

    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../ShopScheduling/GetShopSchedulingList',                //Aajx地址
        sortOrder: 'desc',                  //排序方式
        idField: 'ShopID',                  //主键
        //  pageSize: 30,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
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
        onDblClickRow: function (index, rowData) {

        },
        queryParams: {
            //查询条件
            SearchDate: $("#SearchDate").val(),
            LineId: $("#LineId").combobox("getValue"),
            ShopType: $("#ShopType").combobox("getValue"),
            IsOrder: $("#IsOrder").combobox("getValue")
        },
        frozenColumns: [[
            //冻结列
          { field: 'ck', checkbox: true }, //选择
          { title: '订单编号', field: 'OrderId', width: 120, formatter: frxs.formatText, align: 'center' },
          { title: '门店编号', field: 'ShopCode', width: 100, align: 'center' }
        ]],
        columns: [[
            { title: 'LineID', field: 'LineID', hidden: true },
            { title: 'ShopID', field: 'ShopID', hidden: true },
            { title: '门店名称', field: 'ShopName', width: 260, },
            { title: '配送线路', field: 'LineName', width: 100 },
            { title: '配送周期', field: 'DeliverWeek', width: 260, },
            { title: '门店状态', field: 'StatusStr', width: 100, align: 'center' },

              {
                  field: 'opt',
                  title: '操作',
                  align: 'center',
                  width: 120,
                  formatter: function (value, rec) {
                      var str = "";
                      str += "<a class='rowbtn' onclick=\"addShopScheduling()\">添加订单</a>";
                      return str;
                  }
              },
               { title: '门店类型', field: 'ShopTypeStr', width: 100, align: 'center' },
            { title: '门店地址', field: 'Address', width: 260, formatter: frxs.formatText },
              { title: '联系电话', field: 'Telephone', width: 100, align: 'center' },
            { title: '联系人', field: 'LinkMan', width: 100, align: 'center' }
        ]],
        toolbar: [

        ]
    });
}

//新增按钮事件
function addShopScheduling() {
    frxs.openNewTab("添加门店订单", "../WarehouseOrder/WarehouseOrderAddOrEdit?isPd=true", "icon-add");
}

//查询
function search() {
    initGrid();
}

//重置
function resetSearch() {
    //   $("#SearchDate").form("clear");
    $('#LineId').combobox('setValue', '');
    $('#ShopType').combobox('setValue', '');
    $('#IsOrder').combobox('setValue', '');
}

function initDll() {
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
            $("#LineId").combobox({
                data: data,             //数据源
                valueField: "LineID",       //id列
                textField: "LineName"      //value列
            });
            $("#LineId").combobox('select', data[0].LineID);
        }, error: function (e) {
            debugger;
        }
    });

    $("#SearchDate").val(getNowFormatDate());
}

function getNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = date.getFullYear() + "年" + month + "月" + strDate + "日";
    //   + " " + date.getHours() + seperator2 + date.getMinutes()
    //    + seperator2 + date.getSeconds();
    return currentdate;
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 21);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 11,
        height: h
    });
}


