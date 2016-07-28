
var vendorId = "";

$(function () {
    //下拉绑定
    initDDL();

    initBind();

});

//加载数据
function initBind() {
    vendorId = frxs.getUrlParam("id");
    if (vendorId) {
        $.ajax({
            url: "../Vendor/GetVendor",
            type: "post",
            data: { id: vendorId },
            dataType: 'json',
            success: function (obj) {
                initRegionData(obj);
                $('#formAdd').form('load', obj);
                if (obj.VendorCode) {
                }
            }
        });
    } else {
        initRegionData();
    }
}

//初始化区域下拉菜单
function initRegionData(obj) {
    if (obj) {
        region.init({
            ddlProvince: $("#ddlProvince"),
            ddlCity: $("#ddlCity"),
            ddlCounty: $("#ddlCountry"),
            provinceID: obj.ProvinceID,
            cityID: obj.CityID,
            countyID: obj.RegionID
        });
    } else {
        region.init({
            ddlProvince: $("#ddlProvince"),
            ddlCity: $("#ddlCity"),
            ddlCounty: $("#ddlCountry")
        });
    }
}

//下拉绑定
function initDDL() {
    $.ajax({
        url: '../Common/GetVendorTypes',
        type: 'get',
        dataType: 'json',
        async: false,
        data: {},
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "VendorTypeID": "", "VendorTypeName": "-请选择-" });
            //创建控件
            $("#VendorTypeID").combobox({
                data: data,                       //数据源
                valueField: "VendorTypeID",       //id列
                textField: "VendorTypeName"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        dataType: 'json',
        async: false,
        data: { dictType: "VendorSettleTimeType" },
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#SettleTimeType").combobox({
                data: data,                       //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        dataType: 'json',
        async: false,
        data: { dictType: "VendorLevel" },
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#CreditLevel").combobox({
                data: data,                       //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        dataType: 'json',
        async: false,
        data: { dictType: "PaymentDateType" },
        success: function (data) {
            //data = $.parseJSON(data);
            //在第一个Item加上请选择
            data.unshift({ "DictValue": "", "DictLabel": "-请选择-" });
            //创建控件
            $("#PaymentDateType").combobox({
                data: data,                       //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"       //value列
            });
        }, error: function (e) {
            debugger;
        }
    });
}