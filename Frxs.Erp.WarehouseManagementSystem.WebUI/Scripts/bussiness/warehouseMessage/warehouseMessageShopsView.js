
$(function () {
   
    $("#aSearch").click(function () {
        initShopsGrid();
    });

    //grid绑定
    initShopsGrid();
    //grid绑定

    //grid高度改变
    gridresize();
});

function initShopsGrid() {

    $('#shopGrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WarehouseMessage/GetShopGroupModelPageData',                //Aajx地址
        sortName: 'GroupID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'GroupID',                  //主键
        pageSize: 2000,                       //每页条数
        //pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        onClickRow: function (rowIndex, rowData) {
            ShopGridClickRow(rowIndex, rowData);
            $('#shopGrid').datagrid('clearSelections');
        },
        queryParams: {
            //查询条件
            GroupName: $("#searchGroupName").val()
        },
        columns: [[
        { title: '仓库ID', field: 'WID', width: 100, align: 'left', hidden: true },
        { title: '门店分组ID', field: 'GroupID', width: 100, hidden: true },
        { title: '门店群组编码', field: 'GroupCode', width: 100, align: 'left' },
        { title: '门店群组名称', field: 'GroupName', width: 200, align: 'left' }

        ]],
    });
}

function ShopGridClickRow(rowIndex, rowData) {
    debugger
    var rep = false;
    var indexTag;
    var rows = $("#messageShopGrid").datagrid("getRows");
    for (var i = 0; i < rows.length; i++) {

        if (rows[i].GroupCode == rowData.GroupCode) {
            rep = true;
            indexTag = $('#messageShopGrid').datagrid('getRowIndex', rows[i]);
            break;
        }
    }
    if (rep) {
        DeleteData($('#messageShopGrid'), indexTag);
    } else {
        AddData($('#messageShopGrid'), rowData);
    }
}

function MessageShopGridClickRow(rowIndex, rowData) {
  
    var indexTag = $('#messageShopGrid').datagrid('getRowIndex', rowData);
   
    DeleteData($('#messageShopGrid'), indexTag);
}

//添加数据
function AddData(grid, rowdata) {
    grid.datagrid('appendRow', rowdata);
  
}

//删除数据
function DeleteData(grid, rowIndex) {
    grid.datagrid('deleteRow', rowIndex);

}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {

    $('#shopGrid').datagrid('resize', {
        width: "350px",
        height: "170px"
    });

    $('#messageShopGrid').datagrid('resize', {
        width: "350px",
        height: "170px"
    });
}


