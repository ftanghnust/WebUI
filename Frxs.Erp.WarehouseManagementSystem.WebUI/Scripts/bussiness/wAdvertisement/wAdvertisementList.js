var dialogWidth = 800;
//var dialogHeight = 600;
var dialogHeight = 450;

$(function () {
    //grid绑定
    initGrid();

    //下拉绑定
    initDDL();

    //grid高度改变
    gridresize();
});

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../Wadvertisement/GetWAdvertisementList',          //Aajx地址
        sortName: 'AdvertisementID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'AdvertisementID',                  //主键
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
           // edit(rowIndex);
        },
        queryParams: {
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'AdvertisementID', field: 'AdvertisementID', width: 130, align: 'center', sortable: true, hidden: true },
            { title: '名称', field: 'AdvertisementName', width: 260 },
            //{ title: '位置', field: 'AdvertisementPosition', width: 180, formatter: function (value, rec) {
            //    if (value == "1") {
            //        return "轮播广告";
            //    }
            //    else if (value == "2") {
            //        return "底部广告";
            //    }
            //    else if (value == "3") {
            //        return "橱窗";
            //    } else {
            //        return value;
            //    }
            //} },
            { title: '位置', field: 'Sort', width: 150, align: 'center' },
            {
                field: 'opt',
                title: '操作',
                align: 'center',
                width: 200,
                formatter: function (value, rec) {
                    var str = "";
                    str += "<a class='rowbtn' onclick=\"javascript:edit('" + rec.AdvertisementID + "')\">编辑</a>";
                    str += "<a class='rowbtn' onclick=\"javascript:del('" + rec.AdvertisementID + "')\">删除</a>";
                    return str;
                }
            }
        ]],
        toolbar: vartoolbar
        //toolbar: [{
        //    id: 'btnReload',
        //    text: '刷新',
        //    iconCls: 'icon-reload',
        //    handler: function () {
        //        //实现刷新栏目中的数据
        //        $("#grid").datagrid("reload");
        //    }
        //}, '-',
        //{
        //    id: 'btnAdd',
        //    text: '新增',
        //    iconCls: 'icon-add',
        //    handler: add
        //},
        //{
        //    id: 'btnDelete',
        //    text: '删除',
        //    iconCls: 'icon-remove',
        //    handler: delCheck
        //}
        //]
    });
}

function reload() {
    $("#grid").datagrid("reload");
}

//新增按钮事件
function add() {
    var thisdlg = frxs.dialog({
        title: "新增橱窗推荐",
        url: "../Wadvertisement/WAdvertisementAddOrEdit",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
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

function editCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        return;
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    } else {
        edit(rows[0].AdvertisementID);
        $("#grid").datagrid('clearSelections');
    }
}

function edit(id) {
    var thisdlg = frxs.dialog({
        title: "编辑",
        url: "../Wadvertisement/WAdvertisementAddOrEdit?Id=" + id,
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                $("#grid").datagrid("reload");
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                thisdlg.dialog("close");
            }
        }]
    });
}

function delCheck() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return;
    }
    var idStr = "";
    for (var i = 0; i < rows.length; i++) {
        idStr += ("" + rows[i].AdvertisementID + ",");
    }
    if (idStr != "") {
        idStr = idStr.substr(0, idStr.length - 1);
    }
    
        $.messager.confirm("提示", "确认删除？", function (r) {
            if (r) {
                $.ajax({
                    url: "../WAdvertisement/DeleteWAdvertisement",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: idStr
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            $.messager.alert("提示", result.Info, "info");
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                                $("#grid").datagrid('clearSelections');
                            }
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            alert(textStatus);
                        } else if (errThrown) {
                            alert(errThrown);
                        } else {
                            alert("出现错误");
                        }
                    }
                });
            }
    })
}

function del(id) {
   
        $.messager.confirm("提示", "确认删除？", function (r) {
            if (r) {
                $.ajax({
                    url: "../WAdvertisement/DeleteWAdvertisement",
                    type: "get",
                    dataType: "json",
                    data: {
                        ids: id
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            $.messager.alert("提示", result.Info, "info");
                            if (result.Flag == "SUCCESS") {
                                $("#grid").datagrid("reload");
                            }
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            alert(textStatus);
                        } else if (errThrown) {
                            alert(errThrown);
                        } else {
                            alert("出现错误");
                        }
                    }
                });
            }
    })
}


function initDDL() {
    
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 4);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}