$(function () {
    init();
    //grid绑定
    initGrid();
    initSubGrid();
    //grid高度改变
    gridresize();
});

function init() {
    $("#aSearch").click(function () {
        initGrid();
    });
}

function initGrid() {
    $('#main_datagrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../PlatformRate/GetShopList',          //Aajx地址
        sortName: 'ShopID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopID',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        checkOnSelect: false,
        selectOnCheck: false,
        onClickRow: function (rowIndex, rowData) {
            MainGridClickRow2(rowIndex, rowData);
            $(this).datagrid('unselectRow', rowIndex);

            //var subselect = false;
            //var rows = $('#sub_datagrid').datagrid('getRows');
            //if (rows != null && rows.length > 0) {
            //    for (var i = 0; i < rows.length; i++) {
            //        if (rowData.ShopID == rows[i].ShopID) {
            //            subselect = true;
            //            break;
            //        }
            //    }
            //}
            //if (subselect) {
            //    $(this).datagrid('selectRow', rowIndex);
            //} else {
            //    $(this).datagrid('unselectRow', rowIndex);
            //}
        },
        queryParams: {
            //查询条件
            searchType: $("#searchType").val(),
            searchKey: $("#searchKey").val(),
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: 'ShopID', field: 'ShopID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: 'WID', field: 'WID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: '门店编号', field: 'ShopCode', width: 130, align: 'center', sortable: true },
            { title: '门店名称', field: 'ShopName', width: 260, align: 'left', formatter: frxs.replaceCode },
            //{ title: '门店账号', field: 'ShopAccount', width: 180, align: 'center', formatter: frxs.replaceCode },
            //{ title: '联系人', field: 'LinkMan', width: 160, align: 'center', formatter: frxs.replaceCode },
            //{ title: '联系电话', field: 'Telephone', width: 160, align: 'center' },
            //{ title: '送货线路', field: 'LineName', width: 160, align: 'center', formatter: frxs.replaceCode },
            //{ title: '发货排序', field: 'SerialNumberStr', width: 80, align: 'center' },
            //{ title: '门店类型', field: '', width: 80, align: 'center' },
            {
                title: '门店类型', field: 'ShopType', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "加盟店";
                    }
                    else if (value == "1") {
                        return "签约店";
                    }
                    else {
                        return value;
                    }
                }
            },
            { title: '门店地址', field: 'FullAddress', width: 360, align: 'left', formatter: frxs.replaceCode },
            {
                title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
                    if (value == "0") {
                        return "<span class='freeze_text'>冻结</span>";
                    }
                    else {
                        return "正常";
                    }
                }
            }
        ]],
        toolbar: []
        , onLoadSuccess: function (data) {
            window.focus();
        }
    });
}

function initSubGrid() {
    $('#sub_datagrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        //methord: 'get',                    //提交方式
        //url: '../Shop/GetShopList',          //Aajx地址
        sortName: 'ShopID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ShopID',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        checkOnSelect: false,
        selectOnCheck: false,
        onClickRow: function (rowIndex, rowData) {
            MainGridClickRow(rowIndex, rowData);
            $(this).datagrid('unselectRow', rowIndex);
        },
        queryParams: {
            //查询条件
            searchType: $("#searchType").val(),
            searchKey: $("#searchKey").val(),
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { title: 'ShopID', field: 'ShopID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: 'WID', field: 'WID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: '门店编号', field: 'ShopCode', width: 130, align: 'center', sortable: true },
            { title: '门店名称', field: 'ShopName', width: 260, align: 'left', formatter: frxs.replaceCode },
            { title: '门店类型', field: 'ShopType', width: 130, align: 'center', hidden: true },
            { title: '门店地址', field: 'FullAddress', width: 150, align: 'left', hidden: true }
            //{ title: '门店账号', field: 'ShopAccount', width: 180, align: 'center', formatter: frxs.replaceCode },
            //{ title: '联系人', field: 'LinkMan', width: 160, align: 'center', formatter: frxs.replaceCode },
            //{ title: '联系电话', field: 'Telephone', width: 160, align: 'center' },
            //{ title: '送货线路', field: 'LineName', width: 160, align: 'center', formatter: frxs.replaceCode },
            //{ title: '发货排序', field: 'SerialNumberStr', width: 80, align: 'center' },
            //{ title: '门店类型', field: '', width: 80, align: 'center' },
            //{ title: '地址全称', field: 'FullAddress', width: 360, align: 'left', formatter: frxs.replaceCode },
            //{
            //    title: '状态', field: 'Status', width: 60, align: 'center', formatter: function (value, rec) {
            //        if (value == "0") {
            //            return "<span class='freeze_text'>冻结</span>";
            //        }
            //        else {
            //            return "正常";
            //        }
            //    }
            //}
        ]],
        toolbar: []
    });
}

function saveData() {
    window.frameElement.wapi.reloadGroups($('#sub_datagrid'));
    window.frameElement.wapi.focus();
    frxs.pageClose();
}

//提交和关闭快捷键
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData()
    }
    else if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();