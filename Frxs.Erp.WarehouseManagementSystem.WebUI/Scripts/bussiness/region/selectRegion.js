//数据源对象,原来是取自本地的js对象,后改成调用接口从数据库中取,故默认为空数组
var sysAreaDB = [];

//根据 区域ID(AreaID) 或 父级(ID ParentID) 从数据库中取地区信息
function getGetSysArea(strSearchType, id, fn) {
    $.ajax({
        url: '../Common/GetSysArea',
        type: 'get',
        data: { searchType: strSearchType, searchValue: id },
        dataType: "json",
        success: function (data) {
            fn(data);
        },
        error: function (e) {
        }
    });
}

//省市县对象
var region = {
    province: sysAreaDB,
    item: [],
    init: function (option) {
        ///<summary>省市县初始化</summary>
        ///<param type="json" name="option">
        ///<para>ddlProvince: 省份控件</para>
        ///<para>ddlCity: 市控件</para>
        ///<para>ddlCounty: 县控件</para>
        ///<para>defaultID: 默认选择项</para>
        ///<para>provinceID: 省份ID</para>
        ///<para>cityID: 市州ID</para>
        ///<para>countyID: 区县ID</para>
        ///</param>

        this.item = option;

        var _this = this;

        //市州
        option.ddlProvince.change(function () {
            _this.bindProvinceChange();
        });
        //县
        option.ddlCity.change(function () {
            _this.bindCityChange();
        });

        option.ddlProvince.append('<option value="0">全部</option>');
        getGetSysArea("ParentID", 0, function (obj) {
            $(obj).each(function () {
                option.ddlProvince.append("<option value=\"" + this.AreaID + "\">" + this.AreaName + "</option>");
            });

            //默认值
            _this.bindDefault(option);
        })
    },
    //下拉事件
    bindProvinceChange: function (fn) {
        var _this = this;
        _this.item.ddlCity.empty().append('<option value="0">全部</option>');
        _this.item.ddlCounty.empty().append('<option value="0">全部</option>');

        if (_this.item.ddlProvince.val() > 0) {
            getGetSysArea("ParentID", _this.item.ddlProvince.val(), function (obj) {
                $(obj).each(function () {
                    _this.item.ddlCity.append("<option value=\"" + this.AreaID + "\">" + this.AreaName + "</option>");
                });
                if (fn) {
                    fn();
                }
            })
        }
    },
    //地级市绑定事件
    bindCityChange: function (fn) {
        var _this = this;
        _this.item.ddlCounty.empty().append('<option value="0">全部</option>');
        if (_this.item.ddlCity.val() > 0) {
            getGetSysArea("ParentID", _this.item.ddlCity.val(), function (obj) {
                $(obj).each(function () {
                    _this.item.ddlCounty.append("<option value=\"" + this.AreaID + "\">" + this.AreaName + "</option>");
                });
                if (fn) {
                    fn();
                }
            })
        }
    },
    //绑定默认值
    bindDefault: function (option) {

        if (option.defaultID > 100000) {//精确到县
            option.countyID = option.defaultID;
            option.cityID = option.defaultID.toString().substr(2, 2);
            option.provinceID = option.defaultID.toString().substr(0, 2);
        }
        else if (option.defaultID > 1000) {//精确到市州
            option.cityID = option.defaultID;
            option.provinceID = option.defaultID.toString().substr(0, 2);
        }
        else if (option.defaultID > 10) {//精确到省份
            option.provinceID = option.defaultID;
        }

        this.bindSelect(option);
    },
    //绑定默认值到控件
    bindSelect: function (option) {
        if (option.provinceID > 0) {

            var _this = this;
            option.ddlProvince.val(option.provinceID).trigger("change");
            if (this.item.cityID > 0) {
                _this.bindProvinceChange(function () {
                    _this.item.ddlCity.val(_this.item.cityID);

                    _this.bindCityChange(function () {
                        _this.item.ddlCounty.val(_this.item.countyID);
                    });
                });
            }
        }
    }
};
$(function () {
    //初始化
    if ($("select[conRegionSelectProvince]").length > 0) {
        var rSelect = [];
        for (var i = 0; i < $("select[conRegionSelectProvince]").length; i++) {
            rSelect.push({
                ddlProvince: $("select[conRegionSelectProvince=" + (i + 1) + "]"),
                ddlCity: $("select[conRegionSelectCity=" + (i + 1) + "]"),
                ddlCounty: $("select[conRegionSelectCounty=" + (i + 1) + "]"),
                defaultID: $("select[conRegionSelectProvince=" + (i + 1) + "]").attr("defaultID") || 0,
                provinceID: $("select[conRegionSelectProvince=" + (i + 1) + "]").attr("provinceID") || 0,
                cityID: $("select[conRegionSelectCity=" + (i + 1) + "]").attr("cityID") || 0,
                countyID: $("select[conRegionSelectCounty=" + (i + 1) + "]").attr("countyID") || 0,
            });
        }

        for (var j = 0; j < rSelect.length; j++) {
            region.init(rSelect[j]);
        }
    }
});