$(function () {
    //重置按钮事件
    $("#aReset").click(function () {
        resetSearch();
    });
});


function initGrid(editwproduct, vartoolbar) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../WProduct/GetWProductList',          //Aajx地址
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
            if (editwproduct) {
                edit(index);
            }
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            gridresize();
        },
        queryParams: {
            ProductName: $("#ProductName").val(),
            SKU: $("#Sku").val(),
            BarCode: $("#BarCode").val(),
            VendorName: $("#VendorName").val(),
            CategoriesId1: $('#CategoriesId1').combobox('getValue'),
            CategoriesId2: $('#CategoriesId2').combobox('getValue'),
            CategoriesId3: $('#CategoriesId3').combobox('getValue'),
            //AddedVendor: $('#AddedVendor').combobox('getValue'),
            WStatus: $('#WStatus').combobox('getValue')
        },
        frozenColumns: [[
            { field: 'ck', checkbox: true },
            { title: '商品编码', field: 'SKU', width: 80, align: 'center' },
            {
                title: '商品名称', field: 'ProductName', width: 240, formatter: function (value, rowData) {
                    var str = "<a name=\"aproductName\" style='cursor:pointer;color:#0066cc' onclick=\"ShowProductPictureDetail('{0}')\">{1}</>".Format(rowData.ProductId, frxs.formatText(rowData.ProductName));
                    return str;
                }

            },
            {
                title: '基本分类', field: 'CategoryName', width: 240, align: 'left', formatter: function (value, rowData) {
                    var categoryName = "";
                    if (rowData.CategoryName1 != null && rowData.CategoryName1 != "") {
                        categoryName = rowData.CategoryName1;
                    }
                    if (rowData.CategoryName1 != null && rowData.CategoryName1 != "") {
                        categoryName = categoryName + ">>" + rowData.CategoryName2;
                    }
                    if (rowData.CategoryName1 != null && rowData.CategoryName1 != "") {
                        categoryName = categoryName + ">>" + rowData.CategoryName3;
                    }
                    return frxs.formatText(categoryName);
                }
            },
            {
                //仓库商品状态(0:已移除1:正常;2:淘汰;3:冻结;) ;淘汰商品和冻结商品不能销售;加入或重新加入时为正常；移除后再加入时为正常
                title: '状态', field: 'WStatus', width: 60, align: 'center', formatter: function (value, rowData) {
                    switch (value) {
                        case 0:
                            return "<span class='red'>已移除</span>";
                        case 1:
                            return rowData.WStatusStr;
                        case 2:
                            return "<span class='red'>淘汰</span>";
                        case 3:
                            return "<span class='red'>冻结</span>";
                        default:
                            return rowData.WStatusStr;
                    }
                }
            }
        ]],
        columns: [[
            { title: '货位号', field: 'ShelfCode', width: 80, align: 'center' },
            {
                title: '配送价', field: 'Price', width: 100, align: 'right', formatter: function (value) {
                    value ? value : 0;
                    return parseFloat(value).toFixed(4);
            } },
            { title: '库存单位', field: 'Unit', width: 60, align: 'center' },
            { title: '配送单位', field: 'BigUnit', width: 60, align: 'center' },
            { title: '规格', field: 'Attributes', width: 100, align: 'center', formatter: frxs.formatText },
            { title: '配送包装数', field: 'BigPackingQty', width: 100, align: 'right' },
            { title: '主供应商', field: 'VendorName', width: 220, formatter: frxs.formatText },
            {
                title: '库存数量', field: 'StockQty', width: 120, align: 'right', formatter: function (value, rowData) {
                    //显示是否有查看库存数量的权限
                    var stockQty = rowData.StockQty;
                    if (editwproduct) {
                        stockQty = "<a name=\"awarehousePrice\" style='cursor:pointer;color:#0066cc ' onclick=\"showWarehouseStock({0})\">{1}</>".Format(rowData.ProductId, stockQty);
                    }
                    return stockQty;
                }
            },
            { title: '国际条码', field: 'BarCode', width: 120, align: 'center' }
        ]],
        toolbar: vartoolbar
    });
}

//查看商品主图
function ShowProductPictureDetail(productId) {
    var thisdlg = frxs.dialog({
        title: "查看商品主图",
        url: "../ProductList/ShowProductPictureDetail?productId=" + productId,
        owdoc: window.top,
        height: 620,
        width: 850,
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

//编辑按钮事件
function edit() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        var dlg = frxs.dialog({
            title: "编辑仓库商品",
            url: "../WProduct/WProductEdit?productId=" + rows[0].ProductId + "&addOrEdit=1",
            owdoc: window.top,
            width: 810,
            height: 675,
        });
    }
}


//批量删除按钮事件
function del() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    }
    else {
        var idStr = "";
        for (var i = 0; i < rows.length; i++) {
            idStr += ("" + rows[i].ProductId + ",");
        }
        window.top.$.messager.confirm("操作提示", "确认要移除仓库商品？", function (isdata) {
            if (isdata) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WProduct/DeletWProduct",
                    type: "post",
                    dataType: "json",
                    data: {
                        prodcutids: idStr
                    },
                    complete: function () {
                        loading.close();
                    },
                    success: function (data) {
                        if (data.Flag == "SUCCESS") {
                            window.top.$.messager.alert("提示", data.Info, "info", function () {
                                $("#grid").datagrid("reload");
                            });
                        }
                        else {
                            window.top.$.messager.alert("提示", data.Message, "info");
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            window.top.$.messager.alert("提示", textStatus, "info");
                        } else if (errThrown) {
                            window.top.$.messager.alert("提示", errThrown, "info");
                        } else {
                            window.top.$.messager.alert("提示", "出现错误", "info");
                        }
                    }
                });
            }
        });
    }
}

//查看子仓库库存
function showWarehouseStock(productId) {
    var thisdlg = frxs.dialog({
        title: "查看子仓库库存",
        url: "../WProduct/WareHouseStockList?ProductId=" + productId,
        owdoc: window.top,
        height: 400,
        width: 450,
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


//重置
function resetSearch() {
    $("#ProductName").val('');
    $("#Sku").val('');
    $("#BarCode").val('');
    $("#VendorName").val('');
    $('#CategoriesId1').combobox('setValue', '');
    $('#CategoriesId2').combobox('setValue', '');
    $('#CategoriesId3').combobox('setValue', '');
    //$('#AddedVendor').combobox('setValue', '');
    $('#WStatus').combobox('setValue', '');
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

