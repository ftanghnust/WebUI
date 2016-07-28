//全局变量
var editID;
//切换提示
var comboVal;


$(function () {
    //下拉绑定
    initDDL();

    //加载主表数据
    init();
});

//加载数据
function init() {
    editID = frxs.getUrlParam("EditID");
    if (editID) {
        //编辑
        $.ajax({
            url: "../BatchRecommend/GetBatchRecommendInfo",
            type: "post",
            data: { EditID: editID },
            dataType: 'json',
            success: function (obj) {
                //格式化日期
                obj.SaleEdit.EditDate = frxs.dateFormat(obj.SaleEdit.EditDate);
                obj.SaleEdit.CreateTime = frxs.dateFormat(obj.SaleEdit.CreateTime);
                obj.SaleEdit.ConfTime = frxs.dateFormat(obj.SaleEdit.ConfTime);
                obj.SaleEdit.PostingTime = frxs.dateFormat(obj.SaleEdit.PostingTime);

                $('#OrderForm').form('load', obj.SaleEdit);

                loadgrid(obj.Details);

                loadOrder(obj.Order);

                totalCalculate();

                //高度自适应
                gridresize();

                if (obj.SaleEdit.Status == 0) {
                    SetPermission('query');
                }
                else if (obj.SaleEdit.Status == 1) {
                    SetPermission('sure');
                }
                else if (obj.SaleEdit.Status == 2) {
                    SetPermission('posting');
                }

                var rows = $("#grid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    var index = $('#grid').datagrid('getRowIndex', rows[i]);

                    rows[i].SaleQty = parseFloat(rows[i].SaleQty).toFixed(2);
                    rows[i].UnitQty = parseFloat(rows[i].UnitQty).toFixed(2);
                    rows[i].SalePackingQty = parseFloat(rows[i].SalePackingQty).toFixed(2);
                    rows[i].Stock = parseFloat(rows[i].Stock).toFixed(2);
                    rows[i].OnTheWay = parseFloat(rows[i].OnTheWay).toFixed(2);
                    rows[i].Available = parseFloat(rows[i].Available).toFixed(2);
                    rows[i].ShopAddPerc = parseFloat(rows[i].ShopAddPerc).toFixed(2);
                    rows[i].SalePrice = parseFloat(rows[i].SalePrice).toFixed(4);
                    rows[i].SubAmt = parseFloat(rows[i].SubAmt * (1 + parseFloat(rows[i].ShopAddPerc))).toFixed(4);

                    rows[i].MinSalePrice = rows[i].UnitPrice;
                    //更新行
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: rows[i]
                    });
                }

                //仓库不可编辑
                $("#SubWID").combobox("disable");

                totalCalculate();
            }
        });
    } else {
        //添加
        $("#Status").combobox('setValue', '0');

        loadgrid();

        loadOrder();

        //高度自适应
        gridresize();

        //初始化添加一行
        addGridRow();

        SetPermission('add');


        $("#EditDate").val(frxs.nowDateTime());
    }
}

//加载商品数据
function loadgrid(griddata) {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        data: griddata,
        idField: 'SKU',                  //主键
        //pageSize: 2000,                       //每页条数
        //pageList: [5, 10, 1000, 2000],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        showFooter: true,
        onClickRow: function (rowIndex) {
            //$('#grid').datagrid('unselectAll');
            $('#grid').datagrid('clearSelections'); //清除所有选中
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onClickCell: onClickCell,
        onLoadSuccess: function () {
            //
        },
        //rowStyler: function (index, row) {
        //    if (row.IsAppend == 1) {
        //        return 'background-color:#6293BB;color:#fff;font-weight:bold;';
        //    }
        //},
        queryParams: {

        },
        frozenColumns: [[
            { field: 'ck', checkbox: true }, //选择
            {
                title: frxs.setTitleColor('商品编码'), field: 'SKU', width: 100, editor: { type: 'text', options: {} }
            }
            //冻结列
        ]],
        columns: [[

            { title: '商品名称', field: 'ProductName', width: 180, formatter: frxs.replaceCode },
            //{
            //    title: '推荐', field: 'IsAppendStr', width: 40, align: 'center',
            //    formatter: function (value, rec) {
            //        var str = "";
            //        switch (rec.IsAppend) {
            //            case 0:
            //                str += "否";
            //                break;
            //            case 1:
            //                str += "是";
            //                break;
            //        }
            //        return str;
            //    }
            //},
            {
                title: '单位', field: 'SaleUnit', width: 80, align: 'center', editor: {
                    type: 'combobox',
                    options: {
                        //url: '../Common/GetProductUnit?ProductID=2927',
                        //valueField: 'PackingQty',
                        valueField: 'PackingQty',
                        textField: 'UnitName',
                        editable: false,
                        panelHeight: 'auto',
                        onChange: function (value) {
                            //设置为Text

                            if (!isNaN(value)) {
                               
                                var row = $("#grid").datagrid("getSelected");
                                var index = $("#grid").datagrid("getRowIndex", row);
                                //赋值包装数
                                row.SalePackingQty = value;

                                //计算数量=数量*包装数
                                row.UnitQty = parseFloat(row.SalePackingQty * row.SaleQty).toFixed(2);

                                //下拉框设置
                                var text = $(this).combobox("getText");
                                row.SaleUnit = text;
                                if (text == row.Unit) {
                                    row.SalePrice = row.MinSalePrice;
                                } else {
                                    row.SalePrice = row.MaxSalePrice;
                                }
                                
                                var sumPrice = row.SaleQty * row.SalePrice;
                                sumPrice = sumPrice * (parseFloat(1 + parseFloat(row.ShopAddPerc)));
                                row.SubAmt = parseFloat(sumPrice).toFixed(4);


                                $(".datagrid-row .combo").prev().combobox("hidePanel");
                                //更新行
                                $('#grid').datagrid('updateRow', {
                                    index: index,
                                    row: row
                                });
                                //更新总数
                                totalCalculate();
                                

                            }
                        }
                    }
                }
            },
            {
                title: frxs.setTitleColor('数量'), field: 'SaleQty', width: 80, align: 'right', editor: {
                    type: 'numberbox',
                    options: {
                        precision: 2,
                        min: 0
                    }
                }
            },

            { title: '包装数', field: 'SalePackingQty', width: 80, align: 'right' },
            {
                title: '总数量', field: 'UnitQty', width: 160, align: 'right', formatter: function (value) {
                    value = value ? value : "0";
                    return isNaN(value) ? value : parseFloat(value).toFixed(2);
                }
            },
            { title: '库存数量', field: 'Stock', width: 80, align: 'right' },
            { title: '在途数量', field: 'OnTheWay', width: 80, align: 'right' },
            { title: '可用数量', field: 'Available', width: 80, align: 'right' },
            {
                title: '配送价', field: 'SalePrice', width: 100, align: 'right', formatter: function (value) {
                    return value ? parseFloat(value).toFixed(4):"";
                }
            },
            {
                title: '平台费率', field: 'ShopAddPerc', width: 70, align: 'right'
            },
            { title: '金额', field: 'SubAmt', width: 160, align: 'right',formatter:function (value) {
                value = value ? value : "0";
                return isNaN(value) ? value : parseFloat(value).toFixed(4);
            } },
            { title: '国际条码', field: 'BarCode', width: 160 },
            {
                title: frxs.setTitleColor('备注'), field: 'Remark', width: 200, editor: 'text', formatter: frxs.formatText
            },
            { title: 'ProductId', field: 'ProductId', hidden: true, width: 80 },   //商品ID
            { title: 'MaxOrderQty', field: 'MaxOrderQty', hidden: true, width: 100 },
            { title: 'BasePoint', field: 'BasePoint', hidden: true, width: 100 },
            { title: 'VendorPerc1', field: 'VendorPerc1', hidden: true, width: 100 },
            { title: 'VendorPerc2', field: 'VendorPerc2', hidden: true, width: 100 },
            
            { title: 'Unit', field: 'Unit', hidden: true, width: 80 },
            { title: 'UnitPrice', field: 'UnitPrice', hidden: true, width: 80 },    //商品最小单位价格

            { title: 'MinSalePrice', field: 'MinSalePrice', hidden: true },
            { title: 'MaxSalePrice', field: 'MaxSalePrice', hidden: true }
        ]],
        //hideColumn: 'STAFF_ID',
        toolbar: [{
            text: '添加',
            id: 'btnAdd',
            iconCls: 'icon-add',
            handler: addGridRow
        }, '-', {
            text: '删除',
            id: 'btnDel',
            iconCls: 'icon-remove',
            handler: del
        }]
    });


};

//加载订单数据
function loadOrder(griddata) {
    $('#gridOrder').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        data: griddata,
        sortName: 'OrderDate',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'OrderId',                  //主键
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#gridOrder').datagrid('clearSelections');
            $('#gridOrder').datagrid('selectRow', rowIndex);
        },
        frozenColumns: [[
             { field: 'ck', checkbox: true },
            { title: '订单编号', field: 'OrderId', width: 120 },
            //冻结列
        ]],
        columns: [[

            { title: '下单时间', field: 'OrderDate', width: 120, formatter: frxs.dateFormat },
            { title: '门店类型', field: 'ShopTypeName', width: 100, align: 'center' },
            { title: '门店编号', field: 'ShopCode', width: 100 },
            { title: '门店名称', field: 'ShopName', width: 190 },
            { title: '预计配送日期', field: 'SendDate', width: 120, formatter: frxs.dateFormat },
            { title: '订单金额', field: 'PayAmount', width: 100, align: 'right' },
            { title: '配送线路', field: 'LineName', width: 140 },
            { title: '状态', field: 'StatusName', width: 100, align: 'center' },
            { title: '门店ID', field: 'ShopID', width: 80, hidden: true },
            { title: '线路ID', field: 'LineID', width: 80, hidden: true },
            { title: '状态', field: 'Status', width: 80, hidden: true }
        ]],
        toolbar: [
            {
                id: 'btnOrderAdd',
                text: '添加',
                iconCls: 'icon-add',
                handler: orderAdd
            },
            {
                text: '删除',
                id: 'delOrderBtn',
                iconCls: 'icon-remove',
                handler: orderDel
            }
        ]
    });
}

//选择订单
function orderAdd() {
    var thisdlg = frxs.dialog({
        title: "选择订单",
        url: "../BatchRecommend/OrderSelect",
        owdoc: window.top,
        width: 1040,
        height: 600,
        buttons: [{
            // title:"【ALT+S】",
            text: '<div title=【ALT+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.addOrder();
            }
        }, {

            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();//设置焦点在当前页面
                thisdlg.dialog("close");
            }
        }]
    });





}

//订单新增回调
function reloadOrders(grid) {
    var rows = grid.datagrid('getSelections');
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (!rowOrderExist(row["OrderId"])) {
            $('#gridOrder').datagrid('appendRow', row);
        }
    }
}

//判断订单列表是否存在该订单
function rowOrderExist(id) {
    var rows = $('#gridOrder').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["OrderId"] == id) {
            return true;
        }
    }
    return false;
}

//订单删除
function orderDel() {
    var rows = $('#gridOrder').datagrid("getSelections");
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridOrder').datagrid('getRowIndex', copyRows[i]);
        $('#gridOrder').datagrid('deleteRow', index);
    }
    $('#gridOrder').datagrid('clearSelections');
}


//查询历史  打开
function showHistory() {
    var temp = getCookie("WarehouseOrder");
    var obj = JSON.parse(temp);
    var htmltr = "<table><thead><th>单号</th><th>供应商编号-名称</th><th>操作</th></thead>";
    $.each(obj, function () {
        htmltr = htmltr + "<tr><td width=\"100px\" height=\"24\" style=\"cursor:pointer\" onclick=\"loadData(this)\">" + this.ID + "</td><td width=\"300px\" style=\"text-align:left;\">" + this.VendorCode + "-" + this.VendorName + "</td><td><a href=\"#\" onclick=\"removeHistory(this)\">移除</a></td></tr>";
    });

    htmltr = htmltr + "</table>";
    $("#history").empty().append(htmltr);
    $(".history").toggle(200);
}

// 查询历史 记录单击
function loadData(obj) {
    var orderid = $(obj).text();
    window.location.href = "../WarehouseOrder/WarehouseOrderAddOrEdit?OrderID=" + orderid;

    frxs.updateTabTitle("查看门店订单" + orderid, "icon-search");
}

//查询历史 删除
function removeHistory(obj) {
    var orderid = $(obj).parents("tr").find("td:eq(0)").text();
    var temp = getCookie("WarehouseOrder");
    var objs = JSON.parse(temp);

    var result = [];
    for (var i = 0; i < objs.length; i++) {
        if (objs[i].ID != orderid) {
            result.push(objs[i]);
        }
    }
    delCookie("WarehouseOrder");
    if (result.length != 0) {
        addCookie("WarehouseOrder", JSON.stringify(result), 0);
        $(obj).parents("tr").remove();
    } else {
        $(obj).parents("tr").remove();
        $("#history").find("table").remove();
    }
}

//获取cookies
function getCookie(cookie_name) {
    var allcookies = document.cookie;
    var cookie_pos = allcookies.indexOf(cookie_name);   //索引的长度  
    // 如果找到了索引，就代表cookie存在，  
    // 反之，就说明不存在。  
    if (cookie_pos != -1) {
        // 把cookie_pos放在值的开始，只要给值加1即可。  
        cookie_pos += cookie_name.length + 1;      //这里我自己试过，容易出问题，所以请大家参考的时候自己好好研究一下。。。  
        var cookie_end = allcookies.indexOf(";", cookie_pos);
        if (cookie_end == -1) {
            cookie_end = allcookies.length;
        }
        var value = allcookies.substring(cookie_pos, cookie_end); //这里就可以得到你想要的cookie的值了。。。  
    }
    return value;
}

//删除cookies
function delCookie(name) {//为了删除指定名称的cookie，可以将其过期时间设定为一个过去的时间 
    var date = new Date();
    date.setTime(date.getTime() - 10000);
    document.cookie = name + "=a; expires=" + date.toGMTString() + ";path=/";
}

//新增cookies
function addCookie(objName, objValue, objHours) {//添加cookie 
    var str = objName + "=" + objValue;
    if (objHours > 0) {//为0时不设定过期时间，浏览器关闭时cookie自动消失 
        var date = new Date();
        var ms = objHours * 3600 * 1000;
        date.setTime(date.getTime() + ms);
        str += "; expires=" + date.toGMTString();
    }
    document.cookie = str + ";path=/";
}


//商品 添加
function addGridRow() {
    var ciLen = $('#grid').datagrid('getRows').length - 1;
    var lastRow = $('#grid').datagrid('getRows')[ciLen];

    if (lastRow && lastRow.SKU == "") {

        $('#grid').datagrid('clearSelections');
        onClickCell($('#grid').datagrid('getRows').length - 1, "SKU");

        return false;
    } else {
        var index = $('#grid').datagrid('getRows').length;
        $("#grid").datagrid("insertRow", {
            index: index + 1,
            row: { SKU: "", ProductName: "", SaleUnit: "", SaleQty: "", SalePackingQty: "", UnitQty: "", SalePrice: "", Stock: "", OnTheWay: "", Available: "", ShopAddPerc: "", SubAmt: "", Remark: "", BarCode: "" }
            //row: { SKU: "", ProductName: "", SaleUnit: "", SaleQty: "0.00", SalePackingQty: "0", UnitQty: "0.00", SalePrice: "0.0000", Stock: "0.00", OnTheWay: "0.00", Available: "0.00", ShopAddPerc: "0.00", SubAmt: "0.0000", Remark: "", BarCode: "" }
        });
        //onClickCell($('#grid').datagrid('getRows').length - 1, "SKU");
        return true;
    }
}

//商品 删除
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
            if (rows[ci].IsAppend == 0) {
                $.messager.alert("提示", " 该商品不能删除！！", "info");       //非强配商品不能删除
                return false;
            }
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#grid').datagrid('getRowIndex', copyRows[i]);
        $('#grid').datagrid('deleteRow', ind);
    }
    totalCalculate();
}

//保存
function saveData() {
    var validate = $("#OrderForm").form('validate');
    if (!validate) {
        return false;
    }
    
    if ($("#SubWID").combobox("getValue") == "") {
        $.messager.alert("提示", "请选择仓库！", "info");
        return false;
    }

    $('#grid').datagrid('endEdit', editIndex);
    var loading = frxs.loading("正在加载中，请稍后...");
    var rows = $('#grid').datagrid('getRows');
    var jsonStr = "[";    //商品数据
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            if (rows[i].SaleQty == 0) {
                loading.close();
                $.messager.alert("提示", "商品【" + rows[i].ProductName + "】数量不能为零！", "info");
                return false;
            }

            jsonStr += "{";
            jsonStr += "\"BarCode\":\"" + rows[i].BarCode + "\",";
            jsonStr += "\"SalePackingQty\":\"" + rows[i].SalePackingQty + "\",";
            jsonStr += "\"SalePrice\":\"" + rows[i].SalePrice + "\",";
            jsonStr += "\"SaleQty\":\"" + rows[i].SaleQty + "\",";
            jsonStr += "\"SaleUnit\":\"" + rows[i].SaleUnit + "\",";
            jsonStr += "\"ProductName\":\"" + rows[i].ProductName + "\",";
            jsonStr += "\"Remark\":\"" + frxs.filterText(rows[i].Remark ? rows[i].Remark : "") + "\",";
            jsonStr += "\"SKU\":\"" + rows[i].SKU + "\",";
            jsonStr += "\"SubAmt\":\"" + rows[i].SubAmt + "\",";
            //jsonStr += "\"IsAppend\":\"" + rows[i].IsAppend + "\",";
            jsonStr += "\"UnitPrice\":\"" + rows[i].UnitPrice + "\",";
            jsonStr += "\"Unit\":\"" + rows[i].Unit + "\",";
            jsonStr += "\"ShopAddPerc\":\"" + rows[i].ShopAddPerc + "\",";
            jsonStr += "\"BasePoint\":\"" + rows[i].BasePoint + "\",";
            jsonStr += "\"VendorPerc1\":\"" + rows[i].VendorPerc1 + "\",";
            jsonStr += "\"VendorPerc2\":\"" + rows[i].VendorPerc2 + "\",";
            jsonStr += "\"ProductId\":\"" + rows[i].ProductId + "\"";
            jsonStr += "},";
            count++;
        }
    }
    jsonStr = jsonStr.substring(0, jsonStr.length - 1);
    jsonStr += "]";
    if (count == 0) {
        loading.close();
        $.messager.alert("提示", "商品数据不能为空！", "info");
        return false;
    }
    var orderRows = $('#gridOrder').datagrid('getRows');
    var jsonOrderStr = "[";    //订单数据
    var countOrder = 0;
    for (var i = 0; i < orderRows.length; i++) {
        jsonOrderStr += "{";
        jsonOrderStr += "\"OrderID\":\"" + orderRows[i].OrderId + "\",";
        jsonOrderStr += "\"OrderDate\":\"" + orderRows[i].OrderDate + "\",";
        jsonOrderStr += "\"SendDate\":\"" + orderRows[i].SendDate + "\",";
        jsonOrderStr += "\"ShopID\":\"" + orderRows[i].ShopID + "\",";
        jsonOrderStr += "\"ShopCode\":\"" + orderRows[i].ShopCode + "\",";
        jsonOrderStr += "\"ShopName\":\"" + orderRows[i].ShopName + "\"";
        jsonOrderStr += "},";
        countOrder++;
    }
    jsonOrderStr = jsonOrderStr.substring(0, jsonOrderStr.length - 1);
    jsonOrderStr += "]";
    if (countOrder == 0) {
        loading.close();
        $.messager.alert("提示", "订单数据不能为空！", "info");
        return false;
    }
    var jsonEditOrder = "{";  //批量改单数据
    jsonEditOrder += "\"EditID\":\"" + $("#EditID").val() + "\",";
    jsonEditOrder += "\"EditDate\":\"" + $("#EditDate").val() + "\",";
    jsonEditOrder += "\"SubWID\":\"" + $("#SubWID").combobox('getValue') + "\",";
    jsonEditOrder += "\"Status\":\"" + $("#Status").combobox('getValue') + "\",";
    jsonEditOrder += "\"Remark\":\"" + frxs.filterText($("#Remark").val()) + "\"";
    jsonEditOrder += "}";
    $.ajax({
        url: "../BatchRecommend/BatchRecommendAddOrEditeHandle",
        type: "post",
        data: { jsonEdit: jsonEditOrder, jsonProducts: jsonStr, jsonOrders: jsonOrderStr },
        dataType: "json",
        success: function (result) {
            loading.close();
            if (result.Flag == "SUCCESS") {
                if ($("#EditID").val() != "") {
                    $.messager.alert("提示", "编辑成功", "info");
                } else {
                    $("#EditID").val(result.Data.EditID);
                    $("#CreateUserName").val(result.Data.UserName);
                    $("#CreateTime").val(result.Data.Time);
                    $.messager.alert("提示", "添加成功", "info");
                }
                frxs.updateTabTitle("查看商品批量推荐订单" + result.Data.EditID, "icon-search");

                SetPermission("save");

                //仓库不可编辑
                $("#SubWID").combobox("disable");
            } else {
                $.messager.alert("提示", result.Info, "info");
            }
        }
    });
}


//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 185);
    $('#grid').datagrid('resize', {
        width: $(window).width() - 12,
        height: h
    });

    $('#gridOrder').datagrid('resize', {
        width: $(window).width() - 12,
        height: h
    });
}

//下拉控件数据初始化
function initDDL() {
    $.ajax({
        url: '../Common/GetWCList',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "WID": "", "WName": "-请选择-" });
            }
            //创建控件
            $("#SubWID").combobox({
                data: data,             //数据源
                valueField: "WID",       //id列
                textField: "WName",     //value列
                onSelect: function (rec) {
                    if (comboVal > 0) {
                        var msgdlg = frxs.dialog({
                            title: "提示",
                            content: "切换仓库将清空已添加的商品数据。确定切换？",
                            width: 300,
                            height: 130,
                            modal: true,
                            minimizable: false,
                            maximizable: false,
                            buttons: [{
                                text: '&nbsp;确 定&nbsp;',
                                handler: function () {
                                    comboVal = rec.WID;
                                    msgdlg.dialog("close");
                                    $('#grid').datagrid('loadData', { total: 0, rows: [] });
                                    addGridRow();
                                }
                            }, {
                                text: '&nbsp;取 消&nbsp;',
                                handler: function () {
                                    $("#SubWID").combobox('setValue', comboVal);
                                    msgdlg.dialog("close");
                                }
                            }],
                            onClose: function () {
                                $("#SubWID").combobox('setValue', comboVal);
                            }
                        });
                    }
                }
            });
            $("#SubWID").combobox('select', data[0].WID);
            comboVal = data[0].WID;
        }, error: function (e) {

        }
    });
}

$.extend($.fn.datagrid.methods, {
    //编辑单元格事件
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }

            $(this).datagrid('beginEdit', param.index);

            $('#grid').datagrid('clearSelections'); //清除所有选中
            $('#grid').datagrid('selectRow', param.index);


            var ed = $(this).datagrid('getEditor', param);
            if (ed && ed.type && ed.type == "combobox") {

                //ed.target.combobox('getValue')
                //绑定单位字段
                var productId = $("#grid").datagrid("getSelected").ProductId;
                if (productId > 0) {
                    var url = '../Common/GetSaleProductUnit?ProductID=' + productId;
                    ed.target.combobox('reload', url); //联动下拉列表重载
                }
            }
            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
                    if (ed.field == "SaleQty") {
                        obj.attr("maxlength", "10");
                        obj.keyup(function () {
                            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                        }).bind("paste", function () {  //CTR+V事件处理    
                            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                        }).css("ime-mode", "disabled"); //CSS设置输入法不可用   
                    }
                    objEvent(obj, ed.field, param.index);
                } else {
                    objEvent($(ed.target), ed.field, param.index);
                }
            }

            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});

var valueinput = "";
//绑定对象事件
function objEvent(obj, field, index) {

    obj.focus();

    setTimeout(function () {
        obj.select();
    }, 100);
    valueinput = $(obj).val();

    if (field == "SaleQty") {
        obj.bind("change", function () {
            changeBuyQtyValue($(this).val());
        });
    }

    if (field == "SalePrice") {
        obj.bind("change", function () {
            changeBuyPriceValue($(this).val());
        });
    }


    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "SKU") {
                if ($(this).val() == "") {
                    showDialog("");
                    $(this).blur();
                } else if ($(this).val() != valueinput) {
                    showDialog($(this).val());
                    $(this).blur();
                    //$.messager.alert("提示", "弹窗,带入参数" + $(this).val());
                } else {
                    nextControl();
                }
            } else {
                if (field == "SaleQty") {
                    var row = $('#grid').datagrid('getSelected');
                    var index = $("#grid").datagrid("getRowIndex", row);
                    var temp = parseFloat($(this).val()) * parseFloat(row.SalePackingQty);
                    if (temp > 999999) {
                        row.SaleQty = "0.00";
                        $('#grid').datagrid('updateRow', {
                            index: index,
                            row: row
                        });
                        $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                        return false;
                    }
                    obj.change();
                }
                if (field == "SalePrice") {
                    obj.change();
                }
                nextControl();
            }
        }

        //向下
        if (e.keyCode == 40) {
            if (field == "SaleQty") {
                var row = $('#grid').datagrid('getSelected');
                var index = $("#grid").datagrid("getRowIndex", row);
                var temp = parseFloat($(this).val()) * parseFloat(row.SalePackingQty);
                if (temp > 999999) {
                    row.SaleQty = "0.00";
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                    return false;
                }
                obj.change();
            }
            if (field == "SalePrice") {
                obj.change();
            }
            var selected = $('#grid').datagrid('getSelected');
            var index = $('#grid').datagrid('getRowIndex', selected);
            var countIndex = $('#grid').datagrid('getRows');
            $('#grid').datagrid('clearSelections');

            if (index + 1 < countIndex.length) {
                onClickCell(index + 1, field);
            } else {
                onClickCell(0, field);
            }
        }

        //向下
        if (e.keyCode == 38) {
            if (field == "SaleQty") {
                var row = $('#grid').datagrid('getSelected');
                var index = $("#grid").datagrid("getRowIndex", row);
                var temp = parseFloat($(this).val()) * parseFloat(row.SalePackingQty);
                if (temp > 999999) {
                    row.SaleQty = "0.00";
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, field); });
                    return false;
                }
                obj.change();
            }
            if (field == "SalePrice") {
                obj.change();
            }
            var selected = $('#grid').datagrid('getSelected');
            var index = $('#grid').datagrid('getRowIndex', selected);
            var countIndex = $('#grid').datagrid('getRows');
            $('#grid').datagrid('clearSelections');

            if (index > 0) {
                onClickCell(index - 1, field);
            } else {
                onClickCell(countIndex.length - 1, field);
            }
        }
    });

    //为SKU绑定移开变回事件
    if ($(obj).parents('td:eq(1)').attr("field") == "SKU") {
        obj.bind("blur", function () {
            $(this).val(valueinput);
        });
    }
}

//金额计算 并且更新grid的行数据
function changeBuyQtyValue(SaleQty) {
    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    //判断购买数量不能大于限购数
    var SalePackingQty = row.SalePackingQty;
    if (row.MaxOrderQty && SaleQty) {
        if (parseFloat(SaleQty) > parseFloat(row.MaxOrderQty)) {
            SaleQty = parseFloat(row.MaxOrderQty); /// parseFloat(SalePackingQty)
        }
    }
    //var SalePackingQty = row.SalePackingQty;
    var SalePrice = row.SalePrice;
    var ShopAddPerc = row.ShopAddPerc;

    row.UnitQty = parseFloat(parseFloat(SaleQty) * parseFloat(SalePackingQty)).toFixed(2);
    if (row.UnitQty > 999999) {
        row.SaleQty = "0.00";
        row.UnitQty = "0.00";
        $('#grid').datagrid('updateRow', {
            index: index,
            row: row
        });
        $.messager.alert("提示", "总数量不能超过6位！", "info", function () { onClickCell(index, 'SaleQty'); });
        return false;
    }
    row.SaleQty = parseFloat(SaleQty).toFixed(2);
    var temp = parseFloat(1 + parseFloat(ShopAddPerc)) * parseFloat(SalePrice);
    row.SubAmt = parseFloat(parseFloat(SaleQty) * temp).toFixed(4);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.SaleQty)) {
        row.SaleQty = "0.00";
    }
    if (isNaN(row.SubAmt)) {
        row.SubAmt = "0.0000";
    }

    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    totalCalculate();
}


//总额计算
function totalCalculate() {
    var rows = $("#grid").datagrid("getRows");
    var totalSubAmt = 0.0000;
    var totalUnitQty = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].SKU) {
            var unitQty = parseFloat(rows[i].UnitQty);
            var subAmt = parseFloat(rows[i].SubAmt);
            totalUnitQty += unitQty;
            totalSubAmt += subAmt;
        }
    }

    $("#PayAmount").val(parseFloat(totalSubAmt).toFixed(4));
    $('#grid').datagrid('reloadFooter', [
       { "UnitQty": "数量总计：" + parseFloat(totalUnitQty).toFixed(2), "SubAmt": "金额总计：" + parseFloat(totalSubAmt).toFixed(4) }
    ]);

}


//金额计算 并且更新grid的行数据
function changeBuyPriceValue(SalePrice) {

    var row = $('#grid').datagrid('getSelected');
    var index = $("#grid").datagrid("getRowIndex", row);

    //判断采购价不能大于OldPrice价格
    //if (row.OldBuyPrice && SalePrice) {
    //    if (parseFloat(SalePrice) > parseFloat(row.OldBuyPrice)) {
    //        SalePrice = parseFloat(row.OldBuyPrice);
    //    }
    //}


    var SalePackingQty = row.SalePackingQty;
    var SaleQty = row.SaleQty;

    row.SalePrice = parseFloat(SalePrice).toFixed(4);
    row.UnitQty = parseFloat(parseFloat(SaleQty) * parseFloat(SalePackingQty)).toFixed(2);
    row.SaleQty = parseFloat(SaleQty).toFixed(2);
    row.SubAmt = parseFloat(parseFloat(SaleQty) * parseFloat(SalePrice)).toFixed(2);

    if (isNaN(row.UnitQty)) {
        row.UnitQty = "0.00";
    }
    if (isNaN(row.SaleQty)) {
        row.SaleQty = "0.00";
    }
    if (isNaN(row.SubAmt)) {
        row.SubAmt = "0.0000";
    }

    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });

    totalCalculate();
}


//去下一个可以编辑框
function nextControl() {
    var selected = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', selected);
    var currentField = "SKU";

    //编辑字段
    var fields = "SKU,SaleQty".split(',');
    for (var i = 0; i < fields.length; i++) {
        var ciField = fields[i];
        if (ciField == editfield) {
            currentField = fields[i + 1];
            if (currentField == undefined) {
                currentField = "SKU";
                index = index + 1;
                $('#grid').datagrid('clearSelections');
                $('#grid').datagrid('getRowIndex', index);
            }
        }
    }
    
    //单位-下拉框回车处理
    if (editfield == "SaleUnit") {
        $('#grid').datagrid('updateRow', {
            index: index,
            row: selected
        });
        currentField = "SaleQty";
    }

    //Remark回车到下一行
    if (editfield == "Remark") {
        currentField = "SKU";
        index = index + 1;
    }

    var len = $("#grid").datagrid("getRows").length;
    if (len == index) {
        //插入行
        if (addGridRow()) {
            onClickCell(index, currentField);
        }
    } else {
        onClickCell(index, currentField);
    }
}

//点击编辑
var editIndex = undefined;
var editfield = undefined;
function endEditing() {
    if (editIndex == undefined) {
        return true;
    }
    if ($('#grid').datagrid('validateRow', editIndex)) {
        $('#grid').datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

//单击单元格事件
function onClickCell(index, field) {

    //判断当前是否需要添加-根据添加按钮是否禁用来判断
    if ($("#btnAdd").hasClass("l-btn-disabled") == false) {

        editfield = field;

        if (endEditing()) {
            $('#grid').datagrid('selectRow', index)
                .datagrid('editCell', { index: index, field: field });
            editIndex = index;
        }
    }
}


//选择商品资料
function showDialog(parm) {
    var subWID = $("#SubWID").combobox('getValue');
    if (subWID) {
        
        var type = "SKU";
        //判断需要查询的类型
        var onebyte = parm.substring(0, 1);
        if (isNaN(onebyte) && onebyte != "赠") {
            type = "ProductName";
        }

        $.ajax({
            url: "../Common/GetSaleProductStockXGInfo",
            type: "get",
            dataType: "json",
            data: {
                SubWID: subWID,
                Value: parm,
                Type: type,
                SKULikeSearch: true
            },
            success: function (obj) {
                if (obj && obj.total == 1) {
                    backFillProduct(obj.rows[0]);
                } else {
                    frxs.dialog({
                        title: "选择商品资料",
                        url: "../BatchRecommend/SearchSaleProductStockXG?productCode=" + parm + "&subWID=" + subWID,
                        owdoc: window.top,
                        width: 850,
                        height: 500
                    });
                }
            }
        });
    } else {
        $.messager.alert("提示", "请先选择仓库信息", "info");
    }
}

//回填商品信息
function backFillProduct(product) {
    //判断是否重复
    var rep = false;
    var rows = $("#grid").datagrid("getRows");
    var index = $('#grid').datagrid('getRowIndex', $('#grid').datagrid('getSelected'));

    //如果选中的是相同的数据 过滤
    //if ($('#grid').datagrid('getSelected').SKU == product.SKU) {
    //    return;
    //}

    for (var i = 0; i < rows.length; i++) {

        if (rows[i].SKU == product.SKU && index != i) {
            rep = true;
            break;
        }
    }
    if (rep) {
        //var msgdlg = frxs.dialog({
        //    title: "提示",
        //    content: "存在重复数据。是否放弃操作？",
        //    width: 300,
        //    height: 130,
        //    modal: true,
        //    buttons: [{
        //        text: '放弃',
        //        iconCls: 'icon-no',
        //        handler: function () {
        //            msgdlg.dialog("close");
        //        }
        //    }, {
        //        text: '添加',
        //        iconCls: 'icon-add',
        //        handler: function () {
        //            backFillProduct2(product);
        //            msgdlg.dialog("close");
        //        }
        //    }]
        //});

        //$.messager.confirm("提示", "存在重复数据。是否继续？", function(r) {
        //    if(r) {
        //        backFillProduct2(product);
        //    }
        //});
        $.messager.alert("提示", "存在重复数据,不能添加", "info");
    } else {
        backFillProduct2(product);
    }


}

//数据回填
function backFillProduct2(product) {
    var row = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', row);

    $('#grid').datagrid('endEdit', index);

    row.SKU = product.SKU;
    row.ProductName = product.ProductName;
    row.SaleUnit = product.BigUnit;
    row.SalePackingQty = parseFloat(product.BigPackingQty).toFixed(2);
    row.SalePrice = parseFloat(product.BigSalePrice).toFixed(4);
    row.BarCode = product.BarCode;
    row.ProductId = product.ProductId;
    row.UnitPrice = product.SalePrice;
    row.Unit = product.Unit;

    row.ShopAddPerc = parseFloat(product.ShopAddPerc).toFixed(2);
    row.Stock = parseFloat(product.Stock).toFixed(2);
    row.OnTheWay = parseFloat(product.OnTheWay).toFixed(2);
    row.Available = parseFloat(product.Available).toFixed(2);

    //row.IsAppend = 1;  //默认为强配商品
    row.MaxOrderQty = product.MaxOrderQty;
    //row.BigShopPoint = product.BigShopPoint;
    row.BasePoint = product.BasePoint;
    row.VendorPerc1 = product.VendorPerc1;
    row.VendorPerc2 = product.VendorPerc2;
    

    row.MinSalePrice = product.SalePrice;
    row.MaxSalePrice = product.BigSalePrice;

    row.SaleQty = '0.00';
    row.UnitQty = '0.00';
    row.SubAmt = '0.0000';


    //更新行
    $('#grid').datagrid('updateRow', {
        index: index,
        row: row
    });


    onClickCell(index, "SaleQty");
}



//订单 添加
function add() {
    location.href = '../BatchRecommend/BatchRecommendAddOrEdit';
    frxs.updateTabTitle("添加商品批量推荐单", "icon-add");
}

//订单 编辑
function edit() {
    SetPermission("edit");

    frxs.updateTabTitle("编辑商品批量推荐单" + $("#EditID").val(), "icon-edit");
}

//订单状态 确认
function sure() {
    var status = $("#Status").combobox('getValue');
    if (status != "0") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return false;
    }
    var editID = $("#EditID").val();
    if (editID != "") {
        $.ajax({
            url: "../BatchRecommend/BatchRecommendChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                editids: editID,
                status: 1
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "确认成功！", "info");
                        $("#Status").combobox('setValue', '1');
                        $("#ConfUserName").val(result.Data.UserName);
                        $("#ConfTime").val(result.Data.Time);
                        SetPermission("sure");
                    } else {
                        $.messager.alert("提示", result.Info, "info");
                    }
                }
            },
            error: function (request, textStatus, errThrown) {
                if (textStatus) {
                    $.messager.alert("提示", textStatus, "info");
                } else if (errThrown) {
                    $.messager.alert("提示", errThrown, "info");
                } else {
                    $.messager.alert("提示", "出现错误", "info");
                }
            }
        });
    }
}

//订单状态 反确认
function noSure() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
        return false;
    }
    var editID = $("#EditID").val();
    if (editID != "") {
        $.ajax({
            url: "../BatchRecommend/BatchRecommendChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                editids: editID,
                status: 0
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "反确认成功！", "info");
                        $("#Status").combobox('setValue', '0');
                        $("#ConfUserName").val("");
                        $("#ConfTime").val("");
                        SetPermission("nosure");
                    } else {
                        $.messager.alert("提示", result.Info, "info");
                    }
                }
            },
            error: function (request, textStatus, errThrown) {
                if (textStatus) {
                    $.messager.alert("提示", textStatus, "info");
                } else if (errThrown) {
                    $.messager.alert("提示", errThrown, "info");
                } else {
                    $.messager.alert("提示", "出现错误", "info");
                }
            }
        });
    }
}

//订单状态 过账
function posting() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return false;
    }
    var editID = $("#EditID").val();
    if (editID != "") {
        //禁用按钮
        var loading = frxs.loading("正在处理中，请稍后...");
        $.ajax({
            url: "../BatchRecommend/BatchRecommendChangeStatus",
            type: "get",
            dataType: "json",
            data: {
                editids: editID,
                status: 2
            },
            success: function (result) {
                loading.close();
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "过账成功！", "info");
                        $("#Status").combobox('setValue', '2');
                        $("#PostingUserName").val(result.Data.UserName);
                        $("#PostingTime").val(result.Data.Time);
                        SetPermission("posting");
                    } else {
                        $.messager.alert("提示", result.Info, "info");
                    }
                }
            },
            error: function (request, textStatus, errThrown) {
                if (textStatus) {
                    $.messager.alert("提示", textStatus, "info");
                } else if (errThrown) {
                    $.messager.alert("提示", errThrown, "info");
                } else {
                    $.messager.alert("提示", "出现错误", "info");
                }
            }
        });
    }
}



//订单 导出
//function exportData() {
//    var orderid = $("#OrderId").val();
//    if (orderid != "") {
//        location.href = "../WarehouseOrder/DataExport?OrderID=" + orderid;
//    } else {
//        $.messager.alert("提示", "订单号为空！", "info");
//    }
//}

//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#addBtn").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnOrderAdd").linkbutton('enable');
            $("#delOrderBtn").linkbutton('enable');
            break;
        case "edit":
            $("#addBtn").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#btnOrderAdd").linkbutton('enable');
            $("#delOrderBtn").linkbutton('enable');
            break;
        case "query":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnOrderAdd").linkbutton('disable');
            $("#delOrderBtn").linkbutton('disable');
            break;
        case "nosure":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnOrderAdd").linkbutton('disable');
            $("#delOrderBtn").linkbutton('disable');
            break;
        case "sure":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnOrderAdd").linkbutton('disable');
            $("#delOrderBtn").linkbutton('disable');
            break;
        case "posting":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnOrderAdd").linkbutton('disable');
            $("#delOrderBtn").linkbutton('disable');
            break;
        case "save":
            $("#addBtn").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#noSureBtn").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#btnOrderAdd").linkbutton('disable');
            $("#delOrderBtn").linkbutton('disable');
            break;
    }

}
