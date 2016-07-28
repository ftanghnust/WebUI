
var orderid;
var shpoid;

$(function () {
    orderid = frxs.getUrlParam("OrderID");
    shpoid = frxs.getUrlParam("ShopID");
});


//对货
function packCheck(){
    window.frameElement.wapi.closeSonOpenCheckDialog(orderid, shpoid);
}

//装箱
function packStart() {
    window.frameElement.wapi.closeSonOpenPackDialog(orderid, shpoid);
}
