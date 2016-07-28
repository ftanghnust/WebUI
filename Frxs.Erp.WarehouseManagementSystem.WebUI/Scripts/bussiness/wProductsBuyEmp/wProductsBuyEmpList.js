$(function () {
    init();
    //grid绑定
    initGrid();
    //下拉绑定
    //initDDL();
    //grid高度改变
    gridresize();
});

function init() {
    $("#searchform").submit(function () {
        initGrid();
        return false;
    });
    $("#aReset").click(function () {
        $("#searchform").form("clear");
        $("#CategoriesId1").combobox("setValue", '');
        $("#CategoriesId2").combobox("setValue", '');
        $("#CategoriesId3").combobox("setValue", '');
        $("#HasBuyEmp").combobox("setValue", '');
    });
}

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../WProductsBuyEmp/GetWProductsBuyEmpList',          //Aajx地址
        //sortName: 'SKU',                 //排序字段
        //sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
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
        onDblClickRow: function (index) {
            
        },
        queryParams: {
            ProductName: $.trim($("#ProductName").val()),
            SKU: $.trim($("#Sku").val()),
            BuyEmpName: $.trim($("#BuyEmpName").val()),
            CategoriesId1: $('#CategoriesId1').combobox('getValue'),
            CategoriesId2: $('#CategoriesId2').combobox('getValue'),
            CategoriesId3: $('#CategoriesId3').combobox('getValue'),
            HasBuyEmp: $('#HasBuyEmp').combobox('getValue'),
            WStatus:1
        },
        columns: [[
            { field: 'ck', checkbox: true },
            { title: 'WProductId', field: 'WProductId', hidden: true },
            { title: '商品编码', field: 'SKU', width: 80, align: 'center' },
            { title: '商品名称', field: 'ProductName', width: 250, formatter: frxs.formatText },
            { title: '采购员', field: 'BuyEmpName', width: 250, formatter: frxs.formatText },
            { title: '国际条码', field: 'BarCode', width: 120, align: 'center' }
            , {
                title: '基本分类', field: 'CategoryName', width: 300, align: 'left', formatter: function (value, rowData) {
                    var categoryName = "";
                    if (rowData.CategoryName1 != null && rowData.CategoryName1 != "") {
                        categoryName = rowData.CategoryName1;
                    }
                    if (rowData.CategoryName2 != null && rowData.CategoryName2 != "") {
                        categoryName = categoryName + ">>" + rowData.CategoryName2;
                    }
                    if (rowData.CategoryName3 != null && rowData.CategoryName3 != "") {
                        categoryName = categoryName + ">>" + rowData.CategoryName3;
                    }
                    return frxs.formatText(categoryName);
                }
            }
        ]],
        toolbar: vartoolbar
    });
}

function add() {
    var rows = $('#grid').datagrid('getSelections');
    //if (rows.length > 1) {
    //    $.messager.alert("提示", "只能选中一条！", "info");
    //    return;
    //} else if (rows.length == 0) {
    //    $.messager.alert("提示", "没有选中记录！", "info");
    //    return;
    //} else {
    //    addEmp(rows[0].WProductId);
    //}
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var productIds = "";
    for (var i = 0; i < rows.length; i++) {
        var productId = rows[i].WProductId;
        productIds += productId + ",";
    }
    if (productIds != "") {
        productIds = productIds.substring(0, productIds.length - 1);
    }
    addEmp(productIds);
}

//设置采购员
function addEmp(productId) {
    //alert(productId);
    var thisdlg = frxs.dialog({
        title: "设置采购员",
        url: "../WProductsBuyEmp/WProductsBuyEmpSet?Id=" + productId,
        owdoc: window.top,
        width: 400,
        height: 300,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                //$("#grid").datagrid("reload");
                //thisdlg.dialog("close");
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                //$("#grid").datagrid("reload");
                thisdlg.dialog("close");
            }
        }]
    });
}

function reload() {
    $("#grid").datagrid("reload");
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

