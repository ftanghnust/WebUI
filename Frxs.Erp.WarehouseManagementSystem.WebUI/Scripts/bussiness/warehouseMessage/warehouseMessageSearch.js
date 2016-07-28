
var messageId = "";

$(function () {

    messageId = frxs.getUrlParam("ID");

    //下拉绑定
    initDDL();

    init();

    initMessageShopsGrid();

    gridresize();
});


//grid高度改变
function gridresize() {


    $('#messageShopGrid').datagrid('resize', {
        width: "350px",
        height: "170px"
    });
}



//加载数据
function init() {

    if (messageId) {
        $.ajax({
            url: "../WarehouseMessage/WarehouseMessageAddOrEdit",
            type: "post",
            data: { ID: messageId },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);
                editor.html(obj.Message); //预加载内容
                editor.readonly(true);  //设计控件为只读

                if (obj.IsFirst == 1) {
                    $("#IsFirst").prop("checked", true);
                } else {
                    $("#IsFirst").prop("checked", false);
                }
             
                if (obj.RangType == 1) {
                    $("#shopGroupView").show();
                    $("#rangType1").attr("disabled", true);
                }
                else {
                    $("#shopGroupView").hide();
                    $("#rangType3").attr("disabled", true);
                  
                }

            }
        });
    } else
    {
        $("#shopGroupView").hide();
    }
}

function initMessageShopsGrid() {
    $('#messageShopGrid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WarehouseMessage/GetMessageShopGroupData',//Aajx地址
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
        checkOnSelect: false,
        selectOnCheck: false,
        onClickRow: function (rowIndex, rowData) {
          
        },
        queryParams: {
            WarehouseMessageID: messageId
        },
        columns: [[
        { title: '仓库ID', field: 'WID', width: 100, align: 'left', hidden: true },
        { title: '门店分组ID', field: 'GroupID', width: 100, hidden: true },
        { title: '门店群组编码', field: 'GroupCode', width: 100, align: 'left' },
        { title: '门店分组', field: 'GroupName', width: 260, align: 'left' }
        ]]
    });
}



//下拉绑定
function initDDL() {
    $.ajax({
        url: '../Common/GetMessageType',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#MessageType").combobox({
                data: data,                       //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"       //value列
            });
        }, error: function (e) {
            $.messager.alert('提示', "加载消息下拉框出错！", 'info');
        }
    });

}