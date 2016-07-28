$(function () {
    //grid绑定-写在cshtml页面了

    //仓库初始化
    ddlInit();

    //grid高度改变
    gridresize();

    //查询按钮事件
    $("#aSearch").click(function () {
        search();
    });

    //重置按钮事件
    $("#aReset").click(function () {
        $("#searchForm").form("clear");
        $("#WID").combobox("setValue", '');
        $("#Status").combobox("setValue", '');
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
        url: '../Again/AaginList',          //Aajx地址
        //sortName: 'AdjID',                 //排序字段
        //sortOrder: 'asc',                  //排序方式
        idField: 'AppID',                  //主键
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
        onDblClickCell: function () {
            var selRow = $("#grid").datagrid("getSelected");
            frxs.openNewTab("查看返配申请单" + selRow.AppID, "../Again/AgainApplyAddOrEdit?action=view&appId=" + selRow.AppID, "icon-search");
        },
        onLoadSuccess:function () {
            $('#grid').datagrid('clearSelections');
        },
        queryParams: {
            AppType: 0,
            //查询条件
            AppID: $("#AppID").val(),
            Status: $("#Status").combobox("getValue"),
            SubWID: $("#WID").combobox("getValue"),
            AppDateStart: $("#AppDateStart").val(),
            AppDateEnd: $("#AppDateEnd").val(),
            PostingTimeStart: $("#PostingTimeStart").val(),
            PostingTimeEnd: $("#PostingTimeEnd").val()
        },
        frozenColumns: [[
            { field: 'ck', checkbox: true }, //选择
            {
                title: '返配单号', field: 'AppID', width: 140, align: 'center'
            }
        ]]
        ,
        columns: [[

            { title: '仓库名称', field: 'WName', width: 160,formatter:function(value,rec){
                return "【" + rec.SubWCode + "】" + rec.WName + "_" + rec.SubWName;
                }
            },
            { title: '录单人员', field: 'CreateUserName', width: 120, align: 'center' },
            { title: '录单时间', field: 'AppDate', width: 120, formatter: frxs.dateFormat, align: 'center' },
              {
                  title: '状态', field: 'Status', width: 80, align: 'center', formatter: function (value, rec) {
                      if (value == "0") {
                          return "录单";
                      } else if (value == "1") {
                          return "确认";
                      } else {
                          return "过账";
                      }
                  }
              },
            { title: '过账人员', field: 'PostingUserName', width: 120, align: 'center' },
            { title: '过账时间', field: 'PostingTime', width: 140, formatter: frxs.dateFormat, align: 'center' },
            { title: '备注', field: 'Remark', width: 180, formatter: frxs.formatText }
          
        ]],
        toolbar: toolbarArray
    });
}

//添加
function add() {
    frxs.openNewTab("添加返配申请单", "../Again/AgainApplyAddOrEdit", "icon-add", window);
}

//查看
function look() {
    var selRow = $("#grid").datagrid("getSelections");
    if (selRow.length == 0) {
        $.messager.alert("提示", "没有选中记录", "info");
        return;
    }
    if (selRow.length > 1) {
        $.messager.alert("提示", "只能选择一行", "info");
        return;
    }
    frxs.openNewTab("查看返配申请单" + selRow[0].AppID, "../Again/AgainApplyAddOrEdit?action=view&appId=" + selRow[0].AppID, "icon-search");
}

//仓库初始化
function ddlInit() {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 0) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#WID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName"      //value列
            });
            $("#WID").combobox('select', data[0].WID);
        }, error: function (e) {
            debugger;
        }
    });
}


//确认
function confirmOrder() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var appIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 0) {
            appIds += rows[i].AppID + ",";
        }
    }

    if (appIds == "") {
        $.messager.alert("提示", "选中的记录中没有可确认的数据！", "info");
        return false;
    }
    appIds = appIds.substring(0, appIds.length - 1);
    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../Again/AgainConfirmOrReconfirm",
        type: "get",
        dataType: "json",
        data: {
            appIds: appIds,
            type: 1,
            menuType:0
        },
        success: function (result) {
            loading.close();
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    //实现刷新栏目中的数据
                    $.messager.alert("提示", "确认成功", "info");
                    $("#grid").datagrid("reload");
                } else {
                    $.messager.alert("提示", result.Info, "info");
                }
            }
        }
    });
}

//反确认
function noConfirmOrder() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var appIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 1) {
            appIds += rows[i].AppID + ",";
        }
    }

    if (appIds == "") {
        $.messager.alert("提示", "选中的记录中没有可反确认的数据！", "info");
        return false;
    }

    appIds = appIds.substring(0, appIds.length - 1);
    //遮罩层
    var loading = window.top.frxs.loading();
    $.ajax({
        url: "../Again/AgainConfirmOrReconfirm",
        type: "get",
        dataType: "json",
        data: {
            appIds: appIds,
            type: 0,
            menuType:0
        },
        success: function (result) {
            loading.close();
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    //实现刷新栏目中的数据
                    $.messager.alert("提示", "反确认成功", "info", function () {
                        $("#grid").datagrid("reload");
                    });
                }
            }
        }
    });

}

//过账
function goEffect() {
    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var appIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status == 1) {
            appIds += rows[i].AppID + ",";
        }
    }

    if (appIds == "") {
        $.messager.alert("提示", "选中的记录中没有可过账的数据！", "info");
        return false;
    }

    appIds = appIds.substring(0, appIds.length - 1);
    $.messager.confirm("提示", "确定过账选中单据？", function (r) {
        if (r) {
            //遮罩层
            var loading = window.top.frxs.loading();
            $.ajax({
                url: "../Again/AgainPosting",
                type: "get",
                dataType: "json",
                data: {
                    appIds: appIds,
                    menuType: 0
                },
                success: function (result) {
                    loading.close();
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {
                            //实现刷新栏目中的数据
                            $.messager.alert("提示", "过账成功", "info", function() {
                                $("#grid").datagrid("reload");
                            });
                        } else {
                            $.messager.alert("提示", result.Info, "info");
                        }
                    }
                }
            });
        }
    });
}


//删除
function del() {

    var rows = $('#grid').datagrid('getSelections');
    if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        return false;
    }
    var appIds = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].Status != 0) {
            $.messager.alert("提示", "错误：非录单状态不能删除！", "info");
            return false;
        }
        appIds += rows[i].AppID + ",";
    }

    appIds = appIds.substring(0, appIds.length - 1);


    $.messager.confirm("提示", "确认删除？", function (r) {
        if (r) {
            //遮罩层
            var loading = window.top.frxs.loading();
            $.ajax({
                url: "../Again/AgainDel",
                type: "get",
                dataType: "json",
                data: {
                    appIds: appIds
                },
                success: function (result) {
                    loading.close();
                    if (result != undefined && result.Info != undefined) {
                        if (result.Flag == "SUCCESS") {
                            //实现刷新栏目中的数据
                            $('#grid').datagrid('clearSelections'); //清除所有选中
                            $("#grid").datagrid("reload");
                            $.messager.alert("提示", "删除成功", "info");
                        } else {
                            $.messager.alert("提示", "失败："+result.Info, "info");
                        }
                    }
                }
            });
        }
    });

}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 24);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}