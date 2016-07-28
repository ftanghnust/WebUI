var dialogWidth = 800;
var dialogHeight = 600;

$(function () {
    init();
    //grid绑定
    initGrid();
    //下拉绑定
    initDDL();
    //grid高度改变
    gridresize();
});

function init() {
    $("#searchform").submit(function () {
        //var validate = $("#searchform").form('validate');
        //if (validate == false) {
        //    return false;
        //}
        if ($("#BeginTime").val() == "" || $("#EndTime").val() == "") {
            $.messager.alert("提示", "请填写开始时间和结束时间查询！", "info");
            return false;
        }
        initGrid();
        return false;
    });
    //$("#aSearch").click(function () {
    //    initGrid();
    //});
    //重置按钮事件
    $("#aReset").click(function () {
        $("#searchform").form("clear");
        $("#MenuID").combobox("setValue", null);
    });
}

function initGrid() {
    
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../OperatorLog/GetOperatorLogList',          //Aajx地址
        sortName: 'CreateTime',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'ID',                  //主键
        pageSize: 50,                       //每页条数
        pageList: [50, 100,200,400],//可以设置每页记录条数的列表 
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
            OperatorName: $.trim($("#OperatorName").val()),
            MenuID: $("#MenuID").combobox("getValue"),
            BeginTime: $("#BeginTime").val(),
            EndTime: $("#EndTime").val()
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            //{ field: 'ck', checkbox: false }, //选择
            { title: 'ID', field: 'ID', width: 130, align: 'center', hidden: true },
            { title: '操作时间', field: 'CreateTime', align: 'center', width: 200 },
            { title: '模块', field: 'MenuName', width: 150, align: 'left' },
            { title: '操作内容', field: 'Remark', width: 280, align: 'left', formatter: frxs.formatText },
            { title: '用户名', field: 'OperatorName', width: 150, align: 'center' },
            { title: 'IP地址', field: 'IPAddress', width: 150, align: 'center' },
        ]]
        //,toolbar: vartoolbar
    });
}

function reload() {
    $("#grid").datagrid("reload");
}


function initDDL() {
    $.ajax({
        url: '../OperatorLog/GetOperatorLogMenu',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            data.unshift({ "MenuID": null, "MenuName": "-请选择-" });
            //创建控件
            $("#MenuID").combobox({
                data: data,             //数据源
                valueField: "MenuID",       //id列
                textField: "MenuName"      //value列
            });
            $("#MenuID").combobox("setValue", null);
        }, error: function (e) {

        }
    });
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height()-22);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}