var dialogWidth = 850;
var dialogHeight = 600;
var Id = "";
var stockId = "";

$(function () {
    initDDL();
    init();
    gridresize();
});

function initDDL() {
    $('#AdjQty').numberbox({
        onChange: function (newValue, oldValue) {
            changeAdjQty();
        }
    });
}

function init() {
    Id = frxs.getUrlParam("Id");
    stockId = frxs.getUrlParam("stockId");
    if (Id) {
        $("#btnSelectProduct").attr("disabled", "disabled");
        $("#SKU").attr("readonly", "readonly");
        var loading = frxs.loading("正在加载中，请稍后...");
        var id = Id;
        $.ajax({
            url: "../StockAdjDetail/GetStockAdjDetail",
            type: "post",
            data: { id: id },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);
                loading.close();
            }
        });
    }
}

function saveData(adjId) {
    var adjQtyNum = $('#AdjQty').numberbox('getValue');
    if ($("#ProductId").val() == "") {
        window.top.$.messager.alert("提示", "请选择商品！", "info", function () {
            $("#SKU").focus();
        });
        return;
    }
    else if ($.trim(adjQtyNum) == "") {
        window.top.$.messager.alert("提示", "请填写数量！", "info", function () {
            $("#AdjQty").siblings("span.textbox").find("input.textbox-text").focus();
        });
        return;
    }

    $("#AdjID").val(adjId);
    changeAdjQty();
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var loading = window.top.frxs.loading("正在保存中，请稍后...");
        var data = $("#formAdd").serialize();
        $.ajax({
            url: "../StockAdjDetail/SaveStockAdjDetail",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                loading.close();
                if (obj.Flag == "SUCCESS") {
                    window.top.$.messager.alert("提示", obj.Info, "info", function () {
                        window.frameElement.wapi.reloadStockAdjDetail();
                        window.frameElement.wapi.focus();
                        frxs.pageClose();
                    });
                }
                else {
                    window.top.$.messager.alert("提示", obj.Info, "info");
                }
            }
        });
    }
}

function EnterPress(e) { //传入 event
    var e = e || window.event;
    if (e.keyCode == 13) {
        getProduct();
    }

}

function changeAdjQty() {
    var adjQtyNum = $('#AdjQty').numberbox('getValue');
    var adjQty = adjQtyNum;
    var adjPackingQty = $("#AdjPackingQty").val();
    var buyPrice = $("#BuyPrice").val();
    var unitQty = parseFloat(adjQty) * parseFloat(adjPackingQty);
    var adjAmt = (unitQty * parseFloat(buyPrice)).toFixed(4);//保留4位小数
    if (!isNaN(unitQty)) {
        $("#UnitQty").val(unitQty);
    }
    else {
        $("#UnitQty").val('');
    }
    if (!isNaN(adjAmt)) {
        $("#AdjAmt").val(adjAmt);
    }
    else {
        $("#AdjAmt").val('');
    }
    //$("#Remark").focus();
    $('#AdjQty').textbox('textbox').focus();
}

function getProduct() {
    var sku = $.trim($("#SKU").val());
    $.ajax({
        url: "../StockAdjDetail/GetProductBySku",
        type: "get",
        dataType: "json",
        data: { sku: sku },
        success: function (result) {
            if (result != null && result != "") {
                reloadProduct(result);
                $("#AdjQty").siblings("span.textbox").find("input.textbox-text").focus();
                return false;
            } else {
                window.top.$.messager.alert("提示", "没有编号为" + sku + "的商品！", "info", function () {
                    clearProduct();
                    $("#SKU").focus();
                    return false;
                });
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
    return true;
}

//打开商品选择的弹窗
function selectProduct() {
    var subWid = window.frameElement.wapi.$("#SubWID").combobox("getValue");
    var thisdlg = frxs.dialog({
        title: "选择商品",
        url: "../StockAdjDetail/StockAdjDetailProducts?subWid="+subWid,
        width: dialogWidth + 80,
        height: dialogHeight,
        owdoc: window.top,
        buttons: [{
            text: '<div title=【Alt+S】>提交</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

function clearProduct() {
    $("#ProductId").val("");
    $("#SKU").val("");
    $("#ProductName").val("");
    $("#BarCode").val("");
    $("#AdjUnit").val("");//单位
    //$("#AdjPackingQty").val("");//包装数
    $("#AdjPackingQty").val(1);//包装数
    $('#AdjQty').numberbox('setValue', '');
    $("#BuyPrice").val("");//?采购单价
    $("#UnitQty").val("");//总数量=数量*包装数
    $("#AdjAmt").val("");//金额=总数量* 采购单价
    $("#StockQty").val("");//?库存
    $("#SalePrice").val("");//?配送单价
    $("#VendorID").val("");
    $("#VendorCode").val("");
    $("#VendorName").val("");
    $("#CategoryId1").val("");
    $("#CategoryId2").val("");
    $("#CategoryId3").val("");
    $("#CategoryId1Name").val("");
    $("#CategoryId2Name").val("");
    $("#CategoryId3Name").val("");
}

//重新加载数据
function reloadProduct(row) {
    $("#ProductId").val(row.ProductId);
    $("#SKU").val(row.SKU);
    $("#ProductName").val(row.ProductName);
    $("#BarCode").val(row.BarCode);
    $("#AdjUnit").val(row.Unit);//单位
    //$("#AdjPackingQty").val(row.BigPackingQty);//包装数
    // $("#AdjQty").val("");//AdjQty
    $('#AdjQty').numberbox('setValue', '');
    //$("#BuyPrice").val(row.BuyPrice);//?采购单价
    $("#BuyPrice").val(row.UnitBuyPrice);//?采购单价
    $("#UnitQty").val('');//总数量=数量*包装数
    $("#AdjAmt").val('');//金额=总数量* 采购单价
    $("#StockQty").val(row.StockQty);//?库存
    $("#SalePrice").val(row.UnitSalePrice);//?配送单价
    $("#VendorID").val(row.VendorID);
    $("#VendorCode").val(row.VendorCode);
    $("#VendorName").val(row.VendorName);
    $("#CategoryId1").val(row.CategoryId1);
    $("#CategoryId2").val(row.CategoryId2);
    $("#CategoryId3").val(row.CategoryId3);
    $("#CategoryId1Name").val(row.CategoryName1);
    $("#CategoryId2Name").val(row.CategoryName2);
    $("#CategoryId3Name").val(row.CategoryName3);
    changeAdjQty();
}

function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - 185);
    var h2 = h - 32;
    $("#grid").height(h);
    $("#easyuitabs").tabs({
        height: h,
    });
    $("#gridStockAdjDetail").datagrid({
        height: h2,
    });
    $("#gridGroup").datagrid({
        height: h2,
    });
};

//提交和关闭快捷键
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        //$("#Remark").focus();
        $('#AdjQty').textbox('textbox').focus();
        saveData(stockId);
    }
    else if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();
