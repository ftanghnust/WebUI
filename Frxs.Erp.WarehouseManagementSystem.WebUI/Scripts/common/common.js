
//js Format
String.prototype.Format = function () {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g, function (m, i, o, n) { return args[i]; });
};

//重写toFixed方法  
Number.prototype.toFixed = function (d) {
    var s = this + "";
    if (!d) d = 0;
    if (s.indexOf(".") == -1) s += ".";
    s += new Array(d + 1).join("0");
    if (new RegExp("^(-|\\+)?(\\d+(\\.\\d{0," + (d + 1) + "})?)\\d*$").test(s)) {
        var s = "0" + RegExp.$2, pm = RegExp.$1, a = RegExp.$3.length, b = true;
        if (a == d + 2) {
            a = s.match(/\d/g);
            if (parseInt(a[a.length - 1]) > 4) {
                for (var i = a.length - 2; i >= 0; i--) {
                    a[i] = parseInt(a[i]) + 1;
                    if (a[i] == 10) {
                        a[i] = 0;
                        b = i != 1;
                    } else break;
                }
            }
            s = a.join("").replace(new RegExp("(\\d+)(\\d{" + d + "})\\d$"), "$1.$2");

        }
        if (b) s = s.substr(1);
        return (pm + s).replace(/\.$/, "");
    }
    return this + "";

};

String.prototype.DataFormat = function (formatter) {
    /// <summary>日期格式化</summary>
    var val = this;
    if (!val) {
        return "";
    }
    var date = new Date(val.replace(/-/g, "/"));

    var year = date.getFullYear();
    var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : +("0") + (date.getMonth() + 1);
    var day = date.getDate() > 9 ? date.getDate() : ("0" + date.getDate());
    var hours = date.getHours() > 9 ? date.getHours() : ("0" + date.getHours());
    var minutes = date.getMinutes() > 9 ? date.getMinutes() : ("0" + date.getMinutes());
    var seconds = date.getSeconds() > 9 ? date.getSeconds() : ("0" + date.getSeconds());
    var dateStr = year + "-" + month + "-" + day + " " + hours + ":" + minutes;
    if (formatter) {
        formatter = formatter.replace(/yyyy/g, year);
        formatter = formatter.replace(/YYYY/g, year);
        formatter = formatter.replace(/MM/g, month);
        formatter = formatter.replace(/dd/g, day);
        formatter = formatter.replace(/DD/g, day);
        formatter = formatter.replace(/hh/g, hours);
        formatter = formatter.replace(/HH/g, hours);
        formatter = formatter.replace(/mm/g, minutes);
        formatter = formatter.replace(/ss/g, seconds);
        formatter = formatter.replace(/SS/g, seconds);
        dateStr = formatter;
    }

    return dateStr;

};

/**
*提交表单
*@param actionName 动作类型
*@param callbackFunction 回调函数
*@param seq 表单序列号
*@param isClose 是否关闭
*@param callbackResponse 回调函数(不管成功或失败都会调用)
*/
function submitForm(actionName, callbackFunction, seq, isClose, callbackResponse) {
    var form = null;

    form = $("form[action$='" + actionName + "']");


    var option = {
        dataType: "json",
        resetForm: false,
        success: function (result) {
            //如果回写对象存在
            if (result.Data) {
                if (callbackFunction != undefined && typeof callbackFunction == "function") {
                    //业务单ID赋值
                    callbackFunction(result.Data, seq);
                }
            }

            if (callbackResponse != undefined && typeof callbackResponse == "function") {
                callbackResponse(result, seq);
            }
            else {
                $.messager.alert("提示", result.Info, "info");
            }
        },
        error: function (result) {
            $.messager.alert("提示", result.responseText, "info");
        },
        complete: function (xhr, ts) {
        }
    };
    form.ajaxSubmit(option);
}
