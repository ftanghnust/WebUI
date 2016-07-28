var dialogWidth = 850;
var dialogHeight = 600;
var vendorTypeId = "";

$(function () {
    //下拉绑定
    initDDL();
    init();
});

function initDDL() {

}

function init() {
    vendorTypeId = frxs.getUrlParam("Id");
    if (vendorTypeId) {
        $.ajax({
            url: "../VendorType/GetVendorType",
            type: "post",
            data: { id: vendorTypeId },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);
            }
        });
    } else {
        initRegionData();
    }
}

//保存数据
function saveData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        var data = $("#formAdd").serialize();
        $.ajax({
            url: "../VendorType/SaveVendorType",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                alert(obj.Info);
                //$.messager.alert("提示", obj.Info, "info");
                //$("#grid").datagrid("reload");
                window.frameElement.wapi.reload();
                frxs.pageClose();
            }
        });
    }
}

