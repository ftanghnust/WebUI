

$(function () {
    
    //初始化控件
    initControl();

    //加载数据
    initGrid();
    //高度自适应
    gridresize();

    //事件绑定
    eventBind();
    
});



//初始化控件
function initControl() {
    var vendorCode = frxs.getUrlParam("vendorCode");
    var vendorName = frxs.getUrlParam("vendorName");
    $("#VendorCode").val(vendorCode);
    //$("#VendorCode").val("0000430");
    
    $("#VendorName").val(vendorName);
    $("#VendorCode").focus();
}


function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../Common/GetVendorInfo',          //Aajx地址
        sortName: 'VendorID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'VendorID',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        singleSelect: true,                 //单选
        //设置点击行为单选，点击行中的复选框为多选
        //checkOnSelect: true,
        //selectOnCheck: true,
        //onClickRow: function (rowIndex) {
        //    $('#grid').datagrid('clearSelections');
        //    $('#grid').datagrid('selectRow', rowIndex);
        //},

        onDblClickRow: function (index, rowData) {
            var vendorId = rowData.VendorID;
            var vendorCode = rowData.VendorCode;
            var vendorName = rowData.VendorName;

            window.frameElement.wapi.backFillVendor(vendorId, vendorCode, vendorName);
            frxs.pageClose();

        },
        queryParams: {
            //查询条件
            VendorCode: $.trim($("#VendorCode").val()),
            VendorName: $.trim($("#VendorName").val()),
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '供应商ID', field: 'VendorID', hidden: true, width: 80, align: 'center' },
            { title: '供应商编码', field: 'VendorCode', width: 100, align: 'center' },
            { title: '供应商名称', field: 'VendorName', width: 580 }
        ]]
    });
}

//查询
function loadgrid() {
    initGrid();
}

//事件绑定
function eventBind() {
    //绑定上下键事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        var index;
        //向下
        if (ev.keyCode == 40) {
            //移除光标
            $("#VendorName").blur();
            $("#VendorCode").blur();
            
            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            var selRow = $("#grid").datagrid("getSelected");
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
            $("#VendorName").blur();
            $("#VendorCode").blur();
            
            if ($("#grid").datagrid("getRows").length <= 0) {
                return false;
            }
            var selRow = $("#grid").datagrid("getSelected");
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
                var focusObj = $(document.activeElement);
                //判断焦点是不是在文本框上面，是：搜索 不是:选中并回填
                if (focusObj.attr("type") == "text") {
                    initGrid();
                } else {
                    var selRow = $("#grid").datagrid("getSelected");
                    if (selRow != null) {
                        var vendorId = selRow.VendorID;
                        var vendorCode = selRow.VendorCode;
                        var vendorName = selRow.VendorName;

                        window.frameElement.wapi.backFillVendor(vendorId, vendorCode, vendorName);
                        frxs.pageClose();
                    } else {
                        $.messager.alert("提示", "没有选中行！", "info");
                    }
                }
            }

        }
    };
}



//清空数据
function resetData() {
    $("#searchForm").form("clear");
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 10);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 5,
        height: h
    });
}
