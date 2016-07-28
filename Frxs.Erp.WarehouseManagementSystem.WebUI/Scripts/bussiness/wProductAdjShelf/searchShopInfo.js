
var shopCode;

$(function () {

    shopCode = frxs.getUrlParam("ShopCode");
    $("#ShopCode").val(shopCode);
    $("#ShopCode").focus();

    //加载数据
    loadgrid();
    //高度自适应
    gridresize();

    //事件绑定
    eventBind();

    productCodeKeypress();
});

function loadgrid() {
    $('#shopGrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../SaleFee/GetShopModelPageData',          //Aajx地址
        sortName: 'ShopCode',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopCode',                  //主键
        pageSize: 30,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        singleSelect: true,                 //单选
        onClickRow: function (rowIndex) {
        },
        onDblClickRow: function (index, rowData) {

            window.frameElement.wapi.backFillShopInfo(rowData);
            frxs.pageClose();
        },
        queryParams: {
            //查询条件
            ShopCode: $("#ShopCode").val(),
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'WID', field: 'WID', hidden: true },
            { title: 'ShopID', field: 'ShopID', hidden: true },
            { title: '门店编码', field: 'ShopCode', width: 200, align: 'center' },
            { title: '门店名称', field: 'ShopName', width: 200, align: 'center' },
            { title: '门店地址', field: 'Address', width: 200 }

        ]]
    });
}

//事件绑定
function eventBind() {
    //绑定上下键事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        var index;
        //向下
        if (ev.keyCode == 40) {
            if ($("#shopGrid").datagrid("getRows").length <= 0) {
                return false;
            }
            var selRow = $("#shopGrid").datagrid("getSelected");
            if (selRow != null) {
                index = $("#shopGrid").datagrid("getRowIndex", $("#shopGrid").datagrid("getSelected"));
                index = index + 1;
            } else {
                index = 0;
            }
            if (index > $("#shopGrid").datagrid("getRows").length - 1) {
                index = 0;
            }

            $('#shopGrid').datagrid('clearSelections');
            $("#shopGrid").datagrid("selectRow", index);
        }

        //向上
        if (ev.keyCode == 38) {
            if ($("#shopGrid").datagrid("getRows").length <= 0) {
                return false;
            }
            var selRow = $("#shopGrid").datagrid("getSelected");
            if (selRow != null) {
                index = $("#shopGrid").datagrid("getRowIndex", $("#shopGrid").datagrid("getSelected"));
                index = index - 1;
            } else {
                index = $("#shopGrid").datagrid("getRows").length - 1;
            }
            if (index < 0) {
                index = $("#shopGrid").datagrid("getRows").length - 1;
            }
            $('#shopGrid').datagrid('clearSelections');
            $("#shopGrid").datagrid("selectRow", index);
        }

        //回车事件
        if (ev.keyCode == 13) {
            var selRow = $("#shopGrid").datagrid("getSelected");
            if (selRow != null) {
                window.frameElement.wapi.backFillShopInfo(selRow);
                frxs.pageClose();
            } else {
                //$.messager.alert("提示", "没有选中行！", "info");
            }
        }

        //ESC按键
        if (ev.keyCode == 27) {
            var parentRow = window.frameElement.wapi.$("#shopGrid").datagrid("getSelected");
            var parentIndex = window.frameElement.wapi.$("#shopGrid").datagrid("getRowIndex", parentRow);
            window.frameElement.wapi.onClickCell(parentIndex, "ShopCode");

            frxs.pageClose();
        }

    };
}

//回车事件
function productCodeKeypress() {
    $("#ShopCode").keypress(function (e) {
        if (e.keyCode == 13) {
            loadgrid();
        }
    });

}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 10);
    $('#shopGrid').datagrid('resize', {
        width: $(window).width() - 5,
        height: h
    });
}
