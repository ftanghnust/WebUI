$(function () {
   
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();

    
    //高度自适应
    gridresize();

    //事件绑定
    eventBind();

    productCodeKeypress();
});

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../Shop/GetShopList',          //Aajx地址
        sortName: 'ShopID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopID',                  //主键
        pageSize: 20,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
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
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        onDblClickRow: function (index, rowData) {

            window.frameElement.wapi.backFillShop(rowData);
            frxs.pageClose();
        },
        queryParams: {            
            //查询条件
            ShopCode: $("#ShopCode").val(),
            ShopName: $("#ShopName").val(),
            ShopAccount: $("#ShopAccount").val(),
            LinkMan: $("#LinkMan").val(),
            LineID: $("#LineID").combobox("getValue"),
            Status: $("#Status").combobox("getValue")
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '门店编号', field: 'ShopCode', width: 100, align: 'center', sortable: true },
            { title: '门店名称', field: 'ShopName', width: 240, align: 'left', formatter: frxs.replaceCode },            
            { title: '送货线路', field: 'LineName', width: 140, align: 'center', formatter: frxs.replaceCode },            
            { title: '地址全称', field: 'FullAddress', width: 260, align: 'left', formatter: frxs.replaceCode }            
        ]]
    });
}



//查询
function search() {
    initGrid();
}

function resetSearch() {
    $("#ShopCode").attr("value", "");
    $("#ShopName").attr("value", "");
    $("#ShopAccount").attr("value", "");
    $("#LinkMan").attr("value", "");

    $('#LineID').combobox('setValue', '');
    $('#Status').combobox('setValue', '');
}

//添加多条记录
function selAddRows() {    
    var rows = $("#grid").datagrid("getSelections");
    for (var i = 0; i < rows.length; i++) {
        
        window.frameElement.wapi.backFillShop(rows[i]);
    }
}

function initDDL() {
    $.ajax({
        url: '../Common/GetWarehouseLineList',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "LineID": "", "LineName": "-请选择-" });
            //创建控件
            $("#LineID").combobox({
                data: data,                       //数据源
                valueField: "LineID",       //id列
                textField: "LineName"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
}

    function eventBind() {

        //绑定上下键事件
        document.onkeydown = function (e) {
            var ev = document.all ? window.event : e;
            var index;
            //向下
            if (ev.keyCode == 40) {
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
                var selRow = $("#grid").datagrid("getSelected");
                if (selRow != null) {
                    window.frameElement.wapi.backFillProduct(selRow);
                    frxs.pageClose();
                } else {
                    //$.messager.alert("提示", "没有选中行！", "info");
                }
            }

            //ESC按键
            if (ev.keyCode == 27) {        
                frxs.pageClose();
            }

        };
    }


    //回车事件
    function productCodeKeypress() {
        $("#ShopCode,#ShopName,#ShopAccount,#LinkMan").keypress(function (e) {
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
        var h = ($(window).height() - $("fieldset").height()-21);
        $('#grid').datagrid('resize', {
            width:$(window).width()-10,
            height: h
        });
    }

    //快捷键在弹出页面里面出发事件
    $(document).on('keydown', function (e) {
        if (e.altKey && e.keyCode == 83) {
            selAddRows();//弹窗提交
            window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
            frxs.pageClose();//弹窗关闭


        }
        else if (e.keyCode == 27) {

            window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
            frxs.pageClose();//弹窗关闭


        }
    });
    window.focus();