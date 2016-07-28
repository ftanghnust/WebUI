
var buyid;

$(function () {
    buyid = frxs.getUrlParam("BuyID");
});


//A4纸打印无价格
function printA4No() {
    window.frameElement.wapi.closeSonOpenPrintA4No(buyid);
}

//A4纸打印有价格
function printA4Yes() {
    window.frameElement.wapi.closeSonOpenPrintA4Yes(buyid);
}

//三联纸打印无价格
function printThreeNo() {
    window.frameElement.wapi.closeSonOpenPrintThreeNo(buyid);
}

//三联纸打印有价格
function printThreeYes() {
    window.frameElement.wapi.closeSonOpenPrintThreeYes(buyid);
}
