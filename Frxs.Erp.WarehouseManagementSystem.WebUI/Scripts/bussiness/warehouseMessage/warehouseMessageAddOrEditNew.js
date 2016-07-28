var messageId = "";

$(function () {

    messageId = frxs.getUrlParam("ID");

    //下拉绑定
    initDDL();

    init();

    initMessageShopsGrid();
});

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
                editor.html(obj.Message);//预加载内容
              
                if (obj.RangType == 1) {
                    $("#shopGroupView").show();
                }
                else {
                    $("#shopGroupView").hide();
                }

                if (obj.IsFirst == 1) {

                    $("#IsFirst").prop("checked", true);
                } else
                {
                    $("#IsFirst").prop("checked", false);

                }
            }
        });
    } else {
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
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex, rowData) {
            MessageShopGridClickRow(rowIndex, rowData);
            $('#messageShopGrid').datagrid('clearSelections');
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

//保存数据
function saveData() {

    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var data = $("#formAdd").serialize();
        var jsonOrder = "{";
        if ($("#MessageType").combobox('getValue') == "") {
            $.messager.alert('提示', "消息类型为必选项！", 'info');
            return false;
        }
        if ($("#Title").val() == "") {
            $.messager.alert('提示', "标题为必填项！", 'info');
            return false;
        }
        if ($("#Message").val() == "") {
            $.messager.alert('提示', "内容为必填项！", 'info');
            return false;
        }
        if ($("#BeginTime").val() == "") {
            $.messager.alert('提示', "发布起始时间为必填项！", 'info');
            return false;
        }
        if ($("#EndTime").val() == "") {
            $.messager.alert('提示', "发布结束时间为必填项！", 'info');
            return false;
        }

        jsonOrder += "\"MessageType\":'" + $("#MessageType").combobox('getValue') + "',";
        jsonOrder += "\"ID\":'" + $("#ID").val() + "',";
        jsonOrder += "\"Title\":'" + $("#Title").val() + "',";
        jsonOrder += "\"Message\":'" + $("#Message").val() + "',";
        jsonOrder += "\"BeginTime\":'" + $("#BeginTime").val() + "',";
        jsonOrder += "\"EndTime\":'" + $("#EndTime").val() + "',";
        jsonOrder += "\"Status\":'" + $("#Status").val() + "',";
      
        if ($("#IsFirst").attr("checked") == "checked") {
            jsonOrder += "\"IsFirst\":'1',";
        } else {
            jsonOrder += "\"IsFirst\":'0',";
        }
        jsonOrder += "\"RangType\":'" + $('input[name="RangType"]:checked').val() + "'";
        jsonOrder += "}";

        var rows = $('#messageShopGrid').datagrid('getRows');
        var jsonStr = "[";
        for (var i = 0; i < rows.length; i++) {
            jsonStr += "{";
            jsonStr += "\"WID\":\"" + rows[i].WID + "\",";
            jsonStr += "\"GroupID\":\"" + rows[i].GroupID + "\",";
            jsonStr += "\"GroupCode\":\"" + rows[i].GroupCode + "\",";
            jsonStr += "\"GroupName\":\"" + rows[i].GroupName + "\"";
            jsonStr += "},";
        }
        if (jsonStr.length > 1) {
            jsonStr = jsonStr.substring(0, jsonStr.length - 1);
        }
        jsonStr += "]";
        var loading = frxs.loading("正在处理中，请稍后...");
        $.ajax({
            url: "../WarehouseMessage/WarehouseMessageHandle",
            type: "post",
            data: {
                jsonData: jsonOrder, jsonDetails: jsonStr
            },
            dataType: 'json',
            success: function (obj) {
                loading.close();

                if (obj.Flag != "SUCCESS") {
                    $.messager.alert('提示', obj.Info, 'info');

                } else {
                    $.messager.alert('提示', "保存成功", 'info', function () {
                        window.frameElement.wapi.$("#grid").datagrid("reload");
                        frxs.pageClose();
                    });
                }
            }, error: function (e) {
                loading.close();
                $.messager.alert('提示', "操作失败！", 'info');
            }
        });
    }
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