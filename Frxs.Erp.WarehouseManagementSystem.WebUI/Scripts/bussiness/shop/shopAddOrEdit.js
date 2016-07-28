$(function () {

    initMap(); //创建和初始化地图

    var markerold = new BMap.Marker(new BMap.Point($("#Longitude").val(), $("#Latitude").val()));  // 创建标注
    map.addOverlay(markerold);

    $("input.textbox-text").addClass('jump');
    $('input:first').focus();//定义第一个input获取焦点
    var $inp = $('.jump');       //定义要依次跳转的IPNUT 都给一个.jump样式
    $inp.bind('keydown', function (e) {
        var key = e.which;
        if (key == 13) {
            e.preventDefault();
            var nxtIdx = $inp.index(this) + 1;
            $(".jump:eq(" + nxtIdx + ")").focus();
        }
    });


    //提交按钮事件
    $("#aSubmit").click(function () {
        $(this).attr("disabled", "disabled");

        var fulladdress = $("#ddlProvince").find("option:selected").text() + $("#ddlCity").find("option:selected").text() + $("#ddlCountry").find("option:selected").text() + $("#Address").val();
      
        $("#FullAddress").val(fulladdress);
        $("#ProvinceID").val($("#ddlProvince").val());
        $("#CityID").val($("#ddlCity").val());
        $("#RegionID").val($("#ddlCountry").val());
        //easyUI表单校验
        var isValidate = $("form[action='ShopHandle']").form("validate");

        if (isValidate) {
            //提交表单
            submitForm("ShopHandle", null, null, false, callbackResponse);
        }
        $(this).removeAttr("disabled");
    });

    //初始化区域下拉菜单
    region.init({
        ddlProvince: $("#ddlProvince"),
        ddlCity: $("#ddlCity"),
        ddlCounty: $("#ddlCountry"),
        provinceID: $("#ProvinceID").val(),
        cityID: $("#CityID").val(),
        countyID: $("#RegionID").val()
    });
})

function saveData() {
  //  $(this).attr("disabled", "disabled");
   // window.top.load();
    var fulladdress = $("#ddlProvince").find("option:selected").text() + $("#ddlCity").find("option:selected").text() + $("#ddlCountry").find("option:selected").text() + $("#Address").val();

    $("#FullAddress").val(fulladdress);
    $("#ProvinceID").val($("#ddlProvince").val());
    $("#CityID").val($("#ddlCity").val());
    $("#RegionID").val($("#ddlCountry").val());
    //easyUI表单校验
    var isValidate = $("form[action='ShopHandle']").form("validate");

    if (isValidate) {
        //提交表单
        submitForm("ShopHandle", null, null, false, callbackResponse);
    }
    $(this).removeAttr("disabled");
}

//提交后回调
function callbackResponse(obj, seq) {
    if (obj.Flag == "SUCCESS") {
        $.messager.alert("提示", "保存成功", "info");
        if (window != window.top && window.frameElement.wapi) {
            window.frameElement.wapi.$("#grid").datagrid("reload");
            window.frameElement.wapi.focus();
            frxs.pageClose();
        }
    } else {
        window.top.$.messager.alert("提示", obj.Info, "info")
        
    }
}

//创建和初始化地图函数：
function initMap() {
    createMap(); //创建地图
    setMapEvent(); //设置地图事件
    addMapControl(); //向地图添加控件
}

//创建地图函数：
function createMap() {
    var map = new BMap.Map("dituContent"); //在百度地图容器中创建一个地图
    var point = new BMap.Point(112.979353, 28.213478); //定义一个中心点坐标
    map.centerAndZoom(point, 12); //设定地图的中心点和坐标并将地图显示在地图容器中
    window.map = map; //将map变量存储在全局
    map.addEventListener("click", showInfo);
}

//地图事件设置函数：
function setMapEvent() {
    map.enableDragging(); //启用地图拖拽事件，默认启用(可不写)
    map.enableScrollWheelZoom(); //启用地图滚轮放大缩小
    map.enableDoubleClickZoom(); //启用鼠标双击放大，默认启用(可不写)
    map.enableKeyboard(); //启用键盘上下左右键移动地图
}

//地图控件添加函数：
function addMapControl() {
    //向地图中添加缩放控件
    var ctrl_nav = new BMap.NavigationControl({ anchor: BMAP_ANCHOR_TOP_LEFT, type: BMAP_NAVIGATION_CONTROL_LARGE });
    map.addControl(ctrl_nav);
    //向地图中添加缩略图控件
    var ctrl_ove = new BMap.OverviewMapControl({ anchor: BMAP_ANCHOR_BOTTOM_RIGHT, isOpen: 1 });
    map.addControl(ctrl_ove);
    //向地图中添加比例尺控件
    var ctrl_sca = new BMap.ScaleControl({ anchor: BMAP_ANCHOR_BOTTOM_LEFT });
    map.addControl(ctrl_sca);
}

function showInfo(e) {
    map.clearOverlays();  
    $("#Longitude").val(e.point.lng);
    $("#Latitude").val(e.point.lat);   
    var marker1 = new BMap.Marker(new BMap.Point(e.point.lng, e.point.lat));  // 创建标注
    map.addOverlay(marker1);
}

function addressChange() {
    map.clearOverlays();
    var provinceName = $("#ddlProvince option:selected").text();
    var cityName = $("#ddlCity option:selected").text();
    var countyName = $("#ddlCounty option:selected").text();
    var city = provinceName + cityName + countyName;
    //var city = $("#ddlRegions1").find("option:selected").text() + $("#ddlRegions2").find("option:selected").text() + $("#ddlRegions3").find("option:selected").text();
    var address = $("#Address").val();
    // 创建地址解析器实例
    var myGeo = new BMap.Geocoder();
    // 将地址解析结果显示在地图上,并调整地图视野
    myGeo.getPoint(city + address, function (point) {
        if (point) {
            map.centerAndZoom(point, 16);
            map.addOverlay(new BMap.Marker(point));           
            $("#Longitude").val(point.lng);
            $("#Latitude").val(point.lat);          
        }
    }, city);
}
//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData();//弹窗提交

    }
    else if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭


    }
});
window.focus();