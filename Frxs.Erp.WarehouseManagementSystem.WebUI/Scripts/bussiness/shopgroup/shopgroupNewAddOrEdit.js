//全局变量
var GroupID;

$(function () {
    

    //加载主表数据
    init();

    //加载详情数据
    loadgrid();

    //高度自适应
    gridresize();

    //初始化添加一行
    //addGridRow();
});

//加载主表数据
function init() {
    GroupID = frxs.getUrlParam("ID");
    if (GroupID) {
        //编辑
        $.ajax({
            url: "../ShopGroup/ShopGroupInfo",
            type: "post",
            data: { id: GroupID },
            dataType: 'json',
            success: function (obj) {
                $('#ShopGroupForm').form('load', obj);

                loadgrid(obj.List);

                //高度自适应
                gridresize();
            }
        });
    } else {
       
    }
}


function loadgrid(griddata) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        data: griddata,
        //url: '../Shop/GetShopList',          //Aajx地址
        //sortName: 'ShopID',                 //排序字段
        //sortOrder: 'asc',                  //排序方式
        idField: 'ID',                  //主键
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
            $('#grid').datagrid('clearSelections');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onLoadSuccess: function () {
            $('#grid').datagrid('clearSelections');
        },
        queryParams: {
            //查询条件
           
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: '门店编号', field: 'ShopCode', width: 130, align: 'center', sortable: true },
            { title: '门店名称', field: 'ShopName', width: 260, align: 'center', formatter: frxs.replaceCode },            
            { title: '地址全称', field: 'FullAddress', width: 360, align: 'center', formatter: frxs.replaceCode }            
        ]],
        hideColumn: 'ShopID',
        toolbar: [{
            id:'btnAdd',
            text: '添加',
            iconCls: 'icon-add',
            handler: addGridRow
        }, '-', {
            id: 'btnDel',
            text: '删除',
            iconCls: 'icon-remove',
            handler: del
        }]
    });
}

function addGridRow() {
    var thisdlg = frxs.dialog({
        title: "选择门店",
        url: "../ShopGroup/EasyuiSearchShop",
        owdoc: window.top,
        width: 850,
        height: 500,
        buttons: [{
            text: '<div title=【Alt+S】>添加选中</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.selAddRows();
                thisdlg.dialog("close");
            }
        }, {
            text: '<div title=【Alt+S】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                thisdlg.dialog("close");
            }
        }]
    });
}

function del() {
    var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));
    $('#grid').datagrid('endEdit', index);

    var is = "";
    $(".datagrid-body:first table tr").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            is = i + "," + is;
        }
    });

    var rows = $('#grid').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < is.split(',').length; j++) {
        var ci = is.split(',')[j];
        if (ci) {
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#grid').datagrid('getRowIndex', copyRows[i]);
        $('#grid').datagrid('deleteRow', ind);
    }
}

//数据回填
function backFillShop(shop) {

    var rows = $('#grid').datagrid('getRows');
    var flag = true;
    for (var i = 0; i < rows.length; i++) {

        if (rows[i].ShopCode == shop.ShopCode)
        {
            flag = false;
        }
    }
    if (flag) {
        //更新行
        $('#grid').datagrid('appendRow', {
            ShopCode: shop.ShopCode,
            ShopID: shop.ShopID,
            ShopName: shop.ShopName,
            FullAddress: shop.FullAddress
        });
    }
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});


//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 120);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}

function saveData() {

    var validate = $("#OrderForm").form('validate');
    if (!validate) {
        return false;
    }
    if ($("input[name='GroupCode']").val() == "") {
        $.messager.alert("提示", "门店群组编号不能为空！", "info");       
        $("input[name='GroupCode']")[0].focus();
        return;
    }
    if ($("input[name='GroupName']").val() == "") {
        $.messager.alert("提示", "群组名称不能为空！", "info");       
        $("input[name='GroupName']")[0].focus();
        return;
    }
    //$('#grid').datagrid('endEdit', editIndex);
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        
            jsonStr += "{";
            jsonStr += "\"ShopID\":\"" + rows[i].ShopID + "\"";            
            jsonStr += "},";
            count++;
        
    }
    jsonStr = jsonStr.substring(0, jsonStr.length - 1);
    jsonStr += "]";
    if (count == 0) {
        $.messager.alert("提示", "门店数据不能为空！", "info");
        return false;
    }
    var GroupID = 0;
    if (!$("#GroupID").val()=="")
    {
        GroupID = $("#GroupID").val();
    }

    var loading = frxs.loading("正在加载中，请稍后...");
    var jsonBuyOrder = "{";
    jsonBuyOrder += "\"GroupID\":'" + GroupID + "',";
    jsonBuyOrder += "\"GroupName\":'" + $("#GroupName").val() + "',";    
    jsonBuyOrder += "\"GroupCode\":'" + $("#GroupCode").val() + "',";   
    jsonBuyOrder += "\"Remark\":'" + $("#Remark").val() + "'";
    jsonBuyOrder += "}";

    $.ajax({
        url: "../ShopGroup/ShopGroupAddSave",
        type: "post",
        data: { jsonData: jsonBuyOrder, jsonDetails: jsonStr },
        dataType: "json",
        success: function (result) {
            loading.close();
            //result = jQuery.parseJSON(result);
            if (result.Flag == "SUCCESS") {
                var data = jQuery.parseJSON(result.Data);
                $.messager.alert("提示", "保存成功", "info", function () {
                    window.frameElement.tabs.wapi.$('#grid').datagrid("reload");
                    frxs.tabClose();
                });
                
            }
            else {
                var data = jQuery.parseJSON(result.Data);
                $.messager.alert("提示", result.Info, "info");
            }
        }
    });
}