$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

    //查询按钮事件
    $("#aSearch").click(function () {
        search();
    });
    //重置按钮事件
    $("#aReset").click(function () {
        resetSearch();
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
        url: '../Vendor/VendorList',          //Aajx地址
        sortName: 'VendorID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'VendorCode',                  //主键
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
        onDblClickCell: function (rowIndex) {
            ToView();
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        queryParams: {
            //查询条件
            VendorCode: $("#VendorCode").val(),
            VendorName: $("#VendorName").val(),
            VendorTypeID: $("#VendorTypeID").combobox('getValue'),
            LinkMan: $("#LinkMan").val(),
            SettleTimeType: $("#SettleTimeType").combobox('getValue'),
            Status: $('#Status').combobox('getValue'),
            PaymentDateType: $("#PaymentDateType").combobox('getValue')            
        },
        frozenColumns: [[
           //冻结列
           { field: 'ck', checkbox: true }, //选择
           { title: '供应商编码', field: 'VendorCode', width: 110, align: 'center', formatter: frxs.replaceCode },
        ]],
        columns: [[

            { title: '名称', field: 'VendorName', width: 260, formatter: frxs.replaceCode },
            { title: '简称', field: 'VendorShortName', width: 180, formatter: frxs.replaceCode },
            { title: '供应商分类', field: 'VendorTypeName', width: 140, align: 'center', formatter: frxs.replaceCode },
            { title: '结算方式', field: 'SettleTimeTypeName', width: 100, align: 'center', formatter: frxs.replaceCode },
            { title: '账期', field: 'PaymentDateTypeName', width: 100, align: 'center', formatter: frxs.replaceCode },
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            },
            { title: '联系人', field: 'LinkMan', width: 100, align: 'center', formatter: frxs.replaceCode },
            { title: '电话', field: 'Telephone', width: 150, align: 'center', formatter: frxs.replaceCode },
            { title: '地址', field: 'FullAddress', width: 360, align: 'left', formatter: frxs.formatText }
        ]],
        toolbar: [{
            id: 'btnReload',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                //实现刷新栏目中的数据
                $("#grid").datagrid("reload");
            }
        }, '-',
        {
            id: 'btnReload',
            text: '查看',
            iconCls: 'icon-search',
            handler: ToView
        }, '-',
        {
            id: 'btnSetVendorProduct',
            text: '商品设置',
            iconCls: 'icon-edit',
            handler: setVendorProduct
        }]
    });
}

//add 罗靖 设置商品供应关系
function setVendorProduct() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        if (rows[0].Status == "0") {
            $.messager.alert("警告", "该供应商已被冻结！", "warning");
            return;
        }
        var thisdlg = frxs.dialog({
            title: "供应商商品设置",
            url: "../VerdorProducts/VendorProductsList?vendorId=" + rows[0].VendorID + "&vendorName=" + rows[0].VendorName,
            owdoc: window.top,
            width: 880,
            height: 660,
            buttons: [{
                text: '<div title=【ESC】>关闭</div>',
                iconCls: 'icon-cancel',
                handler: function () {
                    window.focus();
                    thisdlg.dialog("close");
                }
            }]
        });
    }
}

//回调函数，添加商品供应商关系 add 罗靖
function addProductsVendorList(data) {
    $.ajax({
        url: "../ProductsVendor/AddProductsVendorListHandle",
        type: "post",
        dataType: "json",
        data: data,
        success: function (result) {
            if (result.Flag == "SUCCESS") {
                $.messager.alert('提示', "商品设置成功", "info");
            }
            else {
                $.messager.alert('提示', result.Info, "info");
            }
        },
        error: function (request, textStatus, errThrown) {
            if (textStatus) {
                $.messager.alert('错误', textStatus, 'warning');
            } else if (errThrown) {
                $.messager.alert('错误', errThrown, 'warning');
            } else {
                $.messager.alert('错误', "商品设置失败", 'warning');
            }
        }
    });
}


//重置
function resetSearch() {
    $("#VendorCode").val('');
    $("#VendorName").val('');
    $("#VendorTypeID").combobox('setValue', '');
    $('#LinkMan').val('');
    $('#SettleTimeType').combobox('setValue', '');
    $('#Status').combobox('setValue', '');
    $('#PaymentDateType').combobox('setValue', '');
}


//编辑供应商
function ToViewVendor(vendorID) {
    var thisdlg = frxs.dialog({
        title: "查询",
        url: "../Vendor/AddOrEditNew?id=" + vendorID,
        owdoc: window.top,
        width: 580,
        height: 460,
        buttons: [{
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                thisdlg.dialog("close");
            }
        }]
    });
}

//编辑按钮事件
function ToView() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        ToViewVendor(rows[0].VendorID);
    }
}



function initDDL() {

    $.ajax({
        url: '../Common/GetVendorTypes',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "VendorTypeID": "", "VendorTypeName": "-请选择-" });
            //创建控件
            $("#VendorTypeID").combobox({
                data: data,                       //数据源
                valueField: "VendorTypeID",       //id列
                textField: "VendorTypeName"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        dataType: 'json',
        async: false,
        data: { dictType: "VendorSettleTimeType" },
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#SettleTimeType").combobox({
                data: data,                       //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        dataType: 'json',
        async: false,
        data: { dictType: "PaymentDateType" },
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#PaymentDateType").combobox({
                data: data,                       //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"       //value列
            });
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