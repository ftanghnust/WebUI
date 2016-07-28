//全局变量
var productId;
var addOrEdit;

$(function () {
    productId = frxs.getUrlParam("productId");
    addOrEdit = frxs.getUrlParam("addOrEdit");
    $("#productId").val(productId);
    $("#AddOrEdit").val(addOrEdit);

    //加载页面按钮控制；
    if (addOrEdit == 1) {
        $("#stockUnitPriceManage").hide();
    }
    else {
        $("#stockUnitPriceManage").show();
    }

    //加载主表数据
    initProduct();

    //供应商选中事件
    eventBind();
});



function initProduct() {
    var load = frxs.loading("正在加载...");
    //编辑
    $.ajax({
        url: "../WProduct/WProductAdd",
        type: "post",
        data: {
            productId: productId,
            addOrEdit: addOrEdit
        },
        dataType: 'json',
        success: function (obj) {
            load.close();
            $("#WPrdoctForm").form("load", obj);

            //多单位加载
            loadGrid(obj.WProductUnitList);

            //建议零售单位加载
            wProductMarketUnitListInit(obj.WProductMarketUnitList, obj.MarketUnit);

            //是否可退
            productSaleBackFlagListInit(obj.ProductSaleBackFlagList, obj.SaleBackFlag);

            //状态
            wProductStatusListInit(obj.WProductStatusList, obj.WStatus);

            //默认选中grid行
            selectInit(obj.DeliveryUnitID);
        }
    });
}

//加载多单位grid列表
function loadGrid(griddata) {
    $('#grid').datagrid({
        title: '',                      //标题
        height: 150,
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        data: griddata,
        idField: 'SKU',                  //主键
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        showFooter: true,
        onClickRow: function (rowIndex, rowData) {
            $('#grid').datagrid('clearSelections'); //清除所有选中
            $('#grid').datagrid('selectRow', rowIndex);
            $("input[type='radio']")[rowIndex].checked = true;
        },
        onLoadSuccess: function () {
            var deliveryUnitId = $("#DeliveryUnitID").val();
            selectInit(deliveryUnitId);
        },
        columns: [[
              {
                  title: '选择配送单位', field: 'ProductsUnitId', align: 'left', sortable: false, width: 80, formatter: function (value, rowData, rowIndex) {
                      return '<input type="radio" name="ProductsUnitId" id="ProductsUnitId"' + rowIndex + '    value="' + rowData.ProductsUnitId + '" />';
                  }
              },
            { title: '单位', field: 'Unit', width: 40, align: 'left' },
            { title: '类型', field: 'UnitTypeName', width: 55, align: 'left' },
            { title: '包装', field: 'Spec', width: 50, align: 'left' },
            {
                title: '包装数', field: 'PackingQty', width: 50, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.00" : frxs.formatText(value);
                }
            },
            { title: '商品体积', field: 'UnitVolume', width: 55, align: 'left' },
            { title: '商品重量', field: 'UnitWeight', width: 55, align: 'left' },
            {
                title: '进价', field: 'UnitBuyPrice', width: 85, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.0000" : frxs.formatText(parseFloat(value).toFixed(4));
                }
            },
            {
                title: '配送价', field: 'UnitSalePrice', width: 85, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.0000" : frxs.formatText(parseFloat(value).toFixed(4));
                }
            },
            { title: '物流费率', field: 'UnitVendorPerc1', width: 85, align: 'left' },
            {
                title: '物流费金额', field: 'UnitVendorPerc1Money', width: 85, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.0000" : frxs.formatText(parseFloat(value).toFixed(4));
                }
            },
            { title: '仓储费率', field: 'UnitVendorPerc2', width: 85, align: 'left' },
            {
                title: '仓储费金额', field: 'UnitVendorPerc2Money', width: 85, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.0000" : frxs.formatText(parseFloat(value).toFixed(4));
                }
            },
            { title: '平台费率', field: 'UnitShopAddPerc', width: 85, align: 'left' },
            {
                title: '平台费金额', field: 'UnitShopAddPercMoney', width: 85, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.0000" : frxs.formatText(parseFloat(value).toFixed(4));
                }
            },
            {
                title: '门店积分', field: 'UnitShopPoint', width: 85, align: 'left', formatter: function (value) {
                    return value == 0 ? "0.00" : frxs.formatText(value);
                }
            }
        ]]
    });
}


//建议零售单位
function wProductMarketUnitListInit(data, selectValue) {
    $('#ddlMarketUnit').combobox({
        data: data,
        valueField: 'Value',
        textField: 'Text',
        onChange: function (value) {
            $("#MarketUnit").val(value);
        }
    });
    if (data != undefined && data.length > 0) {
        if (selectValue == "" || selectValue == undefined) {
            selectValue = data[0].Value;
        }
        $("#ddlMarketUnit").combobox('select', selectValue);
    }
}

//是否可退
function productSaleBackFlagListInit(data, selectValue) {
    $('#ddlSaleBackFlag').combobox({
        data: data,
        valueField: 'Value',
        textField: 'Text',
        onChange: function (value) {
            $("#SaleBackFlag").val(value);
        }
    });
    if (data != undefined && data.length > 0) {
        if (selectValue == "" || selectValue == undefined) {
            selectValue = data[0].Value;
        }
        $("#ddlSaleBackFlag").combobox('select', selectValue);
    }
}

//状态
function wProductStatusListInit(data, selectValue) {
    $('#ddlWStatus').combobox({
        data: data,
        valueField: 'Value',
        textField: 'Text',
        onChange: function (value) {
            $("#WStatus").val(value);
        }
    });
    if (data != undefined && data.length > 0) {
        if (selectValue == "" || selectValue == undefined) {
            selectValue = data[0].Value;
        }
        $("#ddlWStatus").combobox('select', selectValue);
    }
}

//默认选中grid行
function selectInit(deliveryUnitId) {
    var rows = $("#grid").datagrid("getRows");
    for (var i = 0; i < rows.length; i++) {
        if (rows[i].ProductsUnitId == deliveryUnitId) {
            $("#grid").datagrid("selectRow", i);
            $("input[type='radio']")[i].checked = true;
        }
    }
}

//保存
function saveData() {

    $(this).attr("disabled", "disabled");

    //表单验证操作：
    var isValidate = $("#WPrdoctForm").form("validate");
    var isValidate2 = valiDate();
    if (isValidate && isValidate2) {
        var selRow = $("#grid").datagrid("getSelected");
        $("#DeliveryUnitID").val(selRow.ProductsUnitId);
        var load = frxs.loading("正在保存...");
        var data = $("#WPrdoctForm").serialize();
        $.ajax({
            url: '../WProduct/WProductAddHandle',
            data: data,
            type: "post",
            success: function (result) {
                load.close();
                var obj = $.parseJSON(result);
                if (obj.Flag == "FAIL") {
                    $.messager.alert("提示", obj.Info, "info");
                }
                else if (obj.Flag == "ERROR") {
                    $.messager.alert("提示", obj.Info, "info");
                }
                else {
                    $.messager.alert("提示", "保存成功", "info", function () {
                        if (addOrEdit == 0) {
                            window.frameElement.wapi.$("#grid").datagrid("reload");
                            frxs.pageClose();
                        }
                        else {
                            window.parent.frameElement.wapi.$("#grid").datagrid("reload");
                            window.parent.frxs.pageClose();
                        }
                    });
                }
            }
        });
    }

    $(this).removeAttr("disabled");
}

//系数计算
function changeGrid() {
    debugger;
    var rows = $("#grid").datagrid("getRows");
    var buyPrice = $("#BuyPrice").val();
    var salePrice = $("#SalePrice").val();
    var vendorPerc1 = $("#VendorPerc1").val();
    var vendorPerc2 = $("#VendorPerc2").val();
    var shopAddPerc = $("#ShopAddPerc").val();
    var shopPoint = $("#ShopPoint").val();

    if (buyPrice == undefined || buyPrice == "") {
        $.messager.alert("提示", "进价不能为空", "info");
        $("#BuyPrice").focus();
        return;
    }
    if (salePrice == undefined || salePrice == "") {
        $.messager.alert("提示", "配送价不能为空", "info");
        $("#SalePrice").focus();
        return;
    }

    if (buyPrice != salePrice) {
        $.messager.alert("提示", "进价和配送价要相等", "info");
        $("#SalePrice").focus();
        return;
    }

    if (vendorPerc1 == undefined || vendorPerc1 == "") {
        $.messager.alert("提示", "物流费率不能为空", "info");
        $("#vendorPerc1").focus();
        return;
    }
    if (vendorPerc2 == undefined || vendorPerc2 == "") {
        $.messager.alert("提示", "仓储费率不能为空", "info");
        $("#vendorPerc2").focus();
        return;
    }
    if (ShopAddPerc == undefined || ShopAddPerc == "") {
        $.messager.alert("提示", "平台费率不能为空", "info");
        $("#ShopAddPerc").focus();
        return;
    }
    if (ShopPoint == undefined || ShopPoint == "") {
        $.messager.alert("提示", "门店积分不能为空", "info");
        $("#ShopPoint").focus();
        return;
    }
    for (var i = 0; i < rows.length; i++) {
        var packingQty = parseFloat(rows[i].PackingQty);
        var buyPriceF = parseFloat(buyPrice);
        var salePriceF = parseFloat(salePrice);
        var vendorPerc1F = parseFloat(vendorPerc1);
        var vendorPerc2F = parseFloat(vendorPerc2);
        var shopAddPercF = parseFloat(shopAddPerc);
        var shopPointF = parseFloat(shopPoint);
        var unitSalePriceF = parseFloat(rows[i].UnitSalePrice);
        rows[i].UnitBuyPrice = (buyPriceF * packingQty).toFixed(4);
        rows[i].UnitSalePrice = (salePriceF * packingQty).toFixed(4);
        rows[i].UnitVendorPerc1 = vendorPerc1F.toFixed(3);
        rows[i].UnitVendorPerc2 = vendorPerc2F.toFixed(3);
        rows[i].UnitShopAddPerc = shopAddPercF.toFixed(3);
        rows[i].UnitShopPoint = (shopPointF * packingQty).toFixed(2);
        rows[i].UnitVendorPerc1Money = (vendorPerc1F * unitSalePriceF).toFixed(2);
        rows[i].UnitVendorPerc2Money = (vendorPerc2F * unitSalePriceF).toFixed(2);
        rows[i].UnitShopAddPercMoney = (shopAddPercF * unitSalePriceF).toFixed(2);
        //更新行
        $('#grid').datagrid('updateRow', {
            index: i,
            row: rows[i]
        });
    }
    var selRow = $("#grid").datagrid("getSelected");
    selectInit(selRow.ProductsUnitId);
}


//选择供应商
function selMasterVendor() {
    var vendorCode = $("#VendorCode").val();
    frxs.dialog({
        title: "选择供应商",
        url: "../BuyOrderPre/SelectVendor?vendorCode=" + encodeURIComponent(vendorCode),
        owdoc: window.top,
        width: 850,
        height: 500
    });
}

//回填供应商
function backFillVendor(vendorId, vendorCode, vendorName) {
    $("#VendorID").val(vendorId);
    $("#VendorCode").val(vendorCode);
    $("#hidVendorCode").val(vendorCode);
    $("#VendorName").val(vendorName);
}



//验证逻辑
function valiDate() {
    var rows = $('#grid').datagrid("getSelections");
    if (rows.length != 1) {
        $.messager.alert("提示", "没有选中配送单位", "info");
        return false;
    }
    return true;
}

//取消
function cancel() {
    if (window.parent == window.top) {
        frxs.pageClose();
    }
    else {
        window.parent.frxs.pageClose();
    }
}

//主供应商设置
function eventBind() {
    $("#VendorCode").keydown(function (e) {
        if (e.keyCode == 13) {
            selMasterVendor();
        }
    });
}

//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData();//弹窗提交
    }
    else if (e.keyCode == 27) {
        //新增
        if (addOrEdit == 0) {
            window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
            frxs.pageClose();//弹窗关闭
        }
        else {
            window.parent.frameElement.wapi.focus();
            window.parent.frxs.pageClose();
        }
    }
});
window.focus();
