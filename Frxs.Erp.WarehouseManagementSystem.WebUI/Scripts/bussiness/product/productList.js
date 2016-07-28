$(function () {
    //重置按钮事件
    $("#aReset").click(function () {
        resetSearch();
    });
});

function initGrid(addProduct, vartoolbar) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../ProductList/GetProductList',          //Aajx地址
        sortName: 'Sku',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        pageSize: 30,                      //每页条数
        pageList: [10, 30, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        // singleSelect: true, //设置为单选
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onDblClickRow: function (index) {
            if (addProduct) {
                add(index);
            }
        },
        queryParams: {
            ProductName: $.trim($("#ProductName").val()),
            Sku: $.trim($("#Sku").val()),
            BarCode: $.trim($("#BarCode").val()),
            CategoriesId1: $('#CategoriesId1').combobox('getValue'),
            CategoriesId2: $('#CategoriesId2').combobox('getValue'),
            CategoriesId3: $('#CategoriesId3').combobox('getValue')
        },
        frozenColumns: [[
            //冻结列
        ]],
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
            gridresize();
        },
        columns: [[
              { field: 'ck', checkbox: true },
              { title: '商品编码', field: 'SKU', width: 130, align: 'center' },
              {
                  title: '商品名称', field: 'ProductName', width: 280, formatter: function (value, rowData) {
                      var str = "<a name=\"aproductName\" style='cursor:pointer;color:#0066cc' onclick=\"ShowProductPictureDetail('{0}')\">{1}</>".Format(rowData.ProductId, frxs.formatText(rowData.ProductName));
                      return str;
                  }
              },
              {
                  title: '基本分类', field: 'CategoryName', width: 280, align: 'left', formatter: function (value, rowData, rowIndex) {
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
                      return categoryName;
                  }
              },
            { title: '国际条码', field: 'BarCode', width: 160 },
            { title: '库存单位', field: 'Unit', width: 100, align: 'center' }
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


//加入按钮事件
function add() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    } else {
        var dlg = frxs.dialog({
            title: "加入商品",
            url: "../WProduct/WProductAddNew?productId=" + rows[0].ProductId + "&addOrEdit=0",
            owdoc: window.top,
            width: 850,
            height: 600
        });

    }
}

//批量添加事件
function addStatus() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
    }
    else {
        var idStr = "";
        for (var i = 0; i < rows.length; i++) {
            idStr += ("" + rows[i].ProductId + ",");
        }
        window.top.$.messager.confirm("操作提示", "确认要添加仓库商品？", function (isdata) {
            if (isdata) {
                $.ajax({
                    url: "../WProduct/WProductStatus",
                    type: "post",
                    dataType: "json",
                    data: {
                        prodcutids: idStr
                    },
                    success: function (result) {
                        if (data.IsSuccess == true) {
                            window.top.$.messager.alert("提示", data.Message, "info");
                            //实现刷新栏目中的数据
                            $("#grid").datagrid("reload");
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


//重置
function resetSearch() {
    $("#ProductName").val('');
    $("#Sku").val('');
    $("#BarCode").val('');
    $('#CategoriesId1').combobox('setValue', '');
    $('#CategoriesId2').combobox('setValue', '');
    $('#CategoriesId3').combobox('setValue', '');
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});


//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 20);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}
