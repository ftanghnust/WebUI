$.extend($.fn.validatebox.defaults.rules, {
    maxLenFormat: {
        validator: function (value, param) {
            var len = $.trim(value).length;
            if (param) {
                this.message = '输入值长度必须等于{0}！'.replace(new RegExp(
                    "\\{" + 0 + "\\}", "g"), param[0]);
            }
            return len == param[0];
        },
        message: '输入值长度必须等于{0}！'
    },
    telFormat: {
        validator: function (value) {
            if (value.length > 0) {
                return /^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$/.test(value) || /^\d{10}$/.test(value) || /^\d{11}$/.test(value) || /^\d{12}$/.test(value) || /^(13|14|15|18)[0-9]{9}$/.test(value);
            } else {
                return true;
            }
        },
        message: '电话格式错误！'
    },
    mobileFormat: {
        validator: function (value) {
            if (value.length > 0) {
                return /^1\d{10}$/.test(value);
            } else {
                return true;
            }
        },
        message: '手机号格式错误！'
    },
    numberFormat: {
        validator: function (value) {
            if (value.length > 0) {
                return /^(\d+)$/.test(value);
            } else {
                return true;
            }
        },
        message: '必须为正整数！'
    },
    floatFormat: {
        validator: function (value) {
            if (value.length > 0) {
                return /^\d+(\.\d+)?$/.test(value);
            } else {
                return true;
            }
        },
        message: '必须为正浮点数！'
    },

    phoneRex: {
        validator: function (value) {
            var rex = /^1[3-8]+\d{9}$/;
            //var rex=/^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
            //区号：前面一个0，后面跟2-3位数字 ： 0\d{2,3}
            //电话号码：7-8位数字： \d{7,8
            //分机号：一般都是3位数字： \d{3,}
            //这样连接起来就是验证电话的正则表达式了：/^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/		 
            var rex2 = /^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
            if (rex.test(value) || rex2.test(value)) {
                // alert('t'+value);
                return true;
            } else {
                //alert('false '+value);
                return false;
            }

        },
        message: '请输入正确电话或手机格式'
    },
    chineseLength: {
        //汉子长度验证@data_options="validType:['chineseLength[10]']"
        validator: function (value, param) {
            var len = $.trim(value).replace(/[^\x00-\xff]/g, "01").length;
            if (len > param[0] * 2) {
                return false;
            }
            return true;
        },
        message: '必须在{0}个汉字以内'
    },
    numberLength: {
        //数字字符验证@data_options="validType:['numberLength[10]']"
        validator: function (value, param) {
            var rex = /^(\d+)$/;
            var len = $.trim(value).length;
            if (len > param[0]) {
                return false;
            } else if (!rex.test($.trim(value))) {
                return false;
            }
            return true;
        },
        message: '必须为{0}个字符内的数字型字符串！'
    },
    trim: {
        //去除空格验证@data_options="validType:['trim']"
        validator: function (value, param) {
            if ($.trim(value) == "") {
                return false;
            }
            return true;
        },
        message: '该输入项不能全部为空格'
    },
    riskStr: {
        //是否包含特殊字符
        validator: function (value, param) {
            var t = /[`~!@#$%^&<>?{},;]/im;
            if (t.test(value)) {
                return false;
            }

            return true;
        },
        message: '该输入项不允许输入特殊字符'
    }
});