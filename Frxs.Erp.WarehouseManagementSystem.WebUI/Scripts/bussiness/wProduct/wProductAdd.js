

$(function () {

    //修改商品价格体系
    $("#btnAddProductUnitPrice").click(function () {
        AddProductUnitPrice();
    });

    initProductUnitChecked();

    //提交按钮事件
    $("#aSubmit").click(function () {
        $(this).attr("disabled", "disabled");
        //easyUI表单校验
        var isValidate = $("form[action='WProductAddHandle']").form("validate");
        //var isValidate = $("#frmProduct").form("validate");
        //alert(isValidate);
        var isValidate2 = valiDate();
        if (isValidate && isValidate2) {
            //提交表单
            submitForm("WProductAddHandle", null, null, false, callbackResponse);
        }
        $(this).removeAttr("disabled");
    });
});


function callbackResponse(obj, seq) {
    var optionType = $("#AddOrEdit").val();
    if (obj.Flag == "SUCCESS") {
        if (optionType == "0") {
            window.top.$.messager.alert("提示", obj.Info, "info", function () {
                
                //var url = "../WProduct/WProductEdit?productId=" + eval("(" + obj.Data + ")").Data;
                //var productname = $("#ProductObject_Products_ProductName").val();
                //var title = "编辑仓库商品-" + productname;
                //window.top.addTabs(title, url);
                //frxs.pageClose();
            });
        }
        else {
            //编辑
            window.top.$.messager.alert("提示", obj.Info, "info", function () {
                //刷新主界面的tab。调用查询按钮
                if (window.parent.frameElement.tabs && window.parent.frameElement.tabs.wapi) {
                    xsjs.tabs.loadList({
                        win: window.parent.frameElement.tabs.wapi, //页A
                        isSelect: true //是否选中页面
                    });
                }
                else {
                    //查看tabs上是否有电商商品库tab，如果有刷新该tab
                    var tab = window.parent.parent.$("#tabs").tabs("getTab", "ERP商品库");
                    if (tab != null) {
                        xsjs.tabs.loadList({
                            win: tab.find("iframe").get(0).contentWindow, //页A
                            isSelect: true //是否选中页面
                        });
                    }
                }
                //关闭界面
                window.parent.frxs.pageClose();
            });
        }
    } else {
        window.top.$.messager.alert("提示", obj.Info, "info");
    }
}


//初始化选择默认配送单位
function initProductUnitChecked() {
    //默认选择的单位选中
    $("input[name='ck']").each(function () {
        if ($(this).val() == $("#DeliveryUnitID").val()) {
            $(this).prop("checked", true);
        }
    });
}



function valiDate() {
    return true;
}


function AddProductUnitPrice() {
    var buyPrice = $("#BuyPrice").val();
    var marketPrice = $("#MarketPrice").val();
    var salePrice = $("#SalePrice").val();
    var vendorPerc1 = $("#VendorPerc1").val();
    var vendorPerc2 = $("#VendorPerc2").val();
    var shopAddPerc = $("#ShopAddPerc").val();
    var shopPoint = $("#ShopPoint").val();

    $("#tblUnit tbody tr").each(function () {
        var packingQty = $(this).find("td:eq(4)").text();
        $(this).find("td:eq(7)").text(packingQty * buyPrice);
        $(this).find("td:eq(8)").text(packingQty * marketPrice);
        $(this).find("td:eq(9)").text(packingQty * salePrice);
        $(this).find("td:eq(10)").text(vendorPerc1);
        $(this).find("td:eq(11)").text(packingQty * vendorPerc1 * salePrice * 0.01);
        $(this).find("td:eq(12)").text(vendorPerc2);
        $(this).find("td:eq(13)").text(packingQty * vendorPerc2 * salePrice * 0.01);
        $(this).find("td:eq(14)").text(shopAddPerc);
        $(this).find("td:eq(15)").text(packingQty * shopAddPerc * salePrice * 0.01);
        $(this).find("td:eq(16)").text(shopPoint * packingQty);
    });
}


//改变单位
function changeUnit(obj) {
    $("#DeliveryUnitID").val($(obj).val());
}
