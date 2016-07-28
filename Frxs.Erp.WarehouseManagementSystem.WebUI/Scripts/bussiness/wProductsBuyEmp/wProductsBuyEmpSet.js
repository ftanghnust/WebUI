var productId = "";

$(function () {
    productId = frxs.getUrlParam("Id");
    init();
    //grid绑定
    initGrid();
    //下拉绑定
    //initDDL();
    //grid高度改变
    gridresize();
});

function init() {

}

function initGrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: '../WProductsBuyEmp/GetBuyEmpInfo',          //Aajx地址
        sortName: 'EmpID',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'EmpID',                  //主键
        pageSize: 20,                       //每页条数
        pageList: [20, 50, 100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分

        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('clearSelections'); //清除所有选中
            $('#grid').datagrid('selectRow', rowIndex);
            $("input[type='radio']")[rowIndex].checked = true;
        },
        onDblClickRow: function (index, rowData) {
            saveData();
        },
        queryParams: {
            //查询条件
            //EmpName: $.trim($("#EmpName").val())
        },
        onLoadSuccess: function (data) {
            $('#grid').datagrid('clearSelections');
            window.focus();
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            {
                title: '选择', field: 'EmpID', align: 'left', sortable: false, width: 80, formatter: function (value, rowData, rowIndex) {
                    return '<input type="radio" name="EmpID" id="EmpID"' + rowIndex + '    value="' + rowData.EmpID + '" />';
                }
            },
            //{ title: 'EmpID', field: 'EmpID', hidden: true, width: 80 },
            { title: '采购员', field: 'EmpName', width: 100, formatter: frxs.formatText }
            //,{ title: '手机号码', field: 'UserMobile', width: 580 }
        ]]
    });
}



function saveData() {
    var rows = $('#grid').datagrid("getSelections");
    if (rows.length > 1) {
        $.messager.alert("提示", "只能选中一条！", "info");
        window.focus();
    } else if (rows.length == 0) {
        $.messager.alert("提示", "没有选中记录！", "info");
        window.focus();
    } else {
        var empIds = "";
        for (var i = 0; i < rows.length; i++) {
            var empId = rows[i].EmpID;
            empIds += empId + ",";
        }
        if (empIds != "") {
            empIds = empIds.substring(0, empIds.length - 1);
        } else {
            window.top.$.messager.alert("提示", "只能设置一个采购员！", "info");
            return;
        }
        window.top.$.messager.confirm("操作提示", "确认要设置采购员【" + rows[0].EmpName + "】吗？", function (isdata) {
            if (isdata) {
                var loading = frxs.loading("正在加载中，请稍后...");
                $.ajax({
                    url: "../WProductsBuyEmp/WProductsBuyEmpListSet",
                    type: "post",
                    data: { productIds: productId, ids: empIds },
                    dataType: 'json',
                    success: function (obj) {
                        loading.close();
                        window.top.$.messager.alert("提示", obj.Info, "info");
                        if (obj.Flag == "SUCCESS") {
                            window.frameElement.wapi.reload();
                            window.frameElement.wapi.focus();
                            frxs.pageClose();
                        }
                    }
                });
            }
        });
    }
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - 0);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 5,
        height: h
    });
}

//提交和关闭快捷键
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData();
    }
    else if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();