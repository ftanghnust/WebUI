var frxs = {
    dateFormat: function (val) {
        /// <summary>日期格式化</summary>
        if (!val) {
            return "";
        }
        var date = null;
        if ((typeof val) == "object") {
            date = val;
        } else {
            date = new Date(val.replace(/-/g, "/"));
        }

        var year = date.getFullYear();
        var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : ("0" + (date.getMonth() + 1));
        var day = date.getDate() > 9 ? date.getDate() : ("0" + date.getDate());
        var hours = date.getHours() > 9 ? date.getHours() : ("0" + date.getHours());
        var minutes = date.getMinutes() > 9 ? date.getMinutes() : ("0" + date.getMinutes());
        var seconds = date.getSeconds() > 9 ? date.getSeconds() : ("0" + date.getSeconds());
        var dateStr = year + "-" + month + "-" + day + " " + hours + ":" + minutes;

        return dateStr;
    },
    ymdFormat: function (val) {
        /// <summary>年月日格式化，只显示 年月日,不需要时间</summary>
        if (!val) {
            return "";
        }
        var date = null;
        if ((typeof val) == "object") {
            date = val;
        } else {
            date = new Date(val.replace(/-/g, "/"));
        }

        var year = date.getFullYear();
        var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : ("0" + (date.getMonth() + 1));
        var day = date.getDate() > 9 ? date.getDate() : ("0" + date.getDate());
        //var hours = date.getHours() > 9 ? date.getHours() : ("0" + date.getHours());
        //var minutes = date.getMinutes() > 9 ? date.getMinutes() : ("0" + date.getMinutes());
        //var seconds = date.getSeconds() > 9 ? date.getSeconds() : ("0" + date.getSeconds());
        var dateStr = year + "-" + month + "-" + day;

        return dateStr;
    },
    //获取url中的参数
    getUrlParam: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
        var r = window.location.search.substr(1).match(reg); //匹配目标参数
        if (r != null) return decodeURIComponent(r[2]);
        return null; //返回参数值
    },
    //js创建guid
    newGuid: function () {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                guid += "-";
        }
        return guid;
    },
    //关闭dialog
    pageClose: function (isReload) {
        /// <summary>关闭jquery-easyui iframe弹窗</summary>
        /// <param name="isReload" type="json">是否刷新弹出此页的来源页面(默认为false)。</param>
        if (window != window.top) {
            //$(window.frameElement).closest(".tabs-panels").children(".panel")
            //$(window.frameElement).closest(".tabs-panels").children(".panel").index($(window.frameElement).closest(".panel"))
            //window.parent.$(".tabs-wrap li").eq($(window.frameElement).closest(".tabs-panels").children(".panel").index($(window.frameElement).closest(".panel")))
            //window.parent.$(".tabs-wrap li").eq($(window.frameElement).closest(".tabs-panels").children(".panel").index($(window.frameElement).closest(".panel"))).find(".tabs-close").trigger("click");
            if (typeof isReload == "boolean" && isReload == true) {
                window.frameElement.wapi.location.href = window.frameElement.wapi.location.href;
            }
            if (window.parent == window) {

            } else {
                var frame = $(window.frameElement);
                if (frame.attr("easyuiTabs") == "true") {
                    window.parent.$(".tabs-wrap li").eq($(window.frameElement).closest(".tabs-panels").children(".panel").index($(window.frameElement).closest(".panel"))).find(".tabs-close").trigger("click");
                } else {
                    try {
                        window.parent.$(window.frameElement).closest("div").window('close', true);
                        window.parent.$(window.frameElement).closest("div.window").remove();
                    } catch (e) {

                    }
                }
                if (typeof CollectGarbage == "function") {
                    CollectGarbage();
                }
            }
            //window.document.write('');
            //window.close();
        } else {
            this.window.opener = null;
            window.close();
        }
    },
    /// jquery-easyui iframe弹窗
    dialog: function (items) {
        /// <summary>jquery-easyui iframe弹窗</summary>
        /// <param name="items" type="json">
        ///     <para>items:继承easyui window属性</para>
        ///     <para>添加新属性:  winid: 窗口ID，</para>
        ///     <para>url: 页面地址,</para>
        ///     <para>isReload: 是否重新加载此页面（注：当winid不为空时此属性生效）</para>
        ///     <para>owdoc: 弹窗对象(默认值为window, 可为window、window.top、window.parent)</para>
        /// </param>
        items = $.extend({
            "width": items.url ? ($((items && items.owdoc) || window).width()) - 20 : 360,
            "height": items.url ? ($((items && items.owdoc) || window).height()) - 20 : 130,
            "collapsible": true,
            "minimizable": false,
            "maximizable": true,
            "resizable": true,
            "dialog": true,
            "modal": true,
            "owdoc": window,
            "onClose": function () {
                $(this).remove();
            }
        }, items);
        var winid = items.winid || ("winid_" + Math.random() * 10000000000000000);
        items.owdoc.$("#" + winid).remove();
        var div = items.owdoc.$("<div id='" + winid + "' style='overflow: hidden;'></div>").appendTo(items.owdoc.document.body);
        if (items.url) {
            var iframe = $("<iframe width='100%' frameborder='0' height='100%' data-easyui-window='true'/>").appendTo(div);
            iframe.load(function () {
                iframe.get(0).wapi = window;
                iframe.get(0).apidata = items.apidata || {};
                div.subpage = iframe.get(0).contentWindow;
            });
            iframe.attr("src", items.url);
            if (items.dialog) {
                div.dialog(items);
            } else {
                div.window(items);
            }
        } else {
            items.content = $("<div style='padding:6px;'>" + items.content + "</div>");
            div.dialog(items);
        }

        if (items.buttons && items.buttons.length > 0) {
            var dialogButton = div.parent().find($(".dialog-button a"));



            $(dialogButton).keyup(function (evt) {
                if (evt.keyCode == 37) {
                    if ($(this).prev().length > 0) {
                        $(this).prev().focus();
                    }
                } else if (evt.keyCode == 39) {
                    if ($(this).next().length > 0) {
                        $(this).next().focus();
                    }
                }
            }).eq(0).focus();
        }



        return div;
    },
    //easyui遮罩
    loading: function (message, closeTime) {
        var height = $(window).height();
        var left = ($(document.body).outerWidth(true) - 190) / 2;
        var top = ($(window).height() - 45) / 2;
        var guid = this.newGuid();
        var htmCode = "<div class=\"datagrid-mask\" id=\"" + guid + "\" style=\"display: block;z-index:99999999; width: 100%; height: " + height + "px;\"></div><div class=\"datagrid-mask-msg\" id=\"msg" + guid + "\" style=\"display: block;z-index:100000000; left: " + left + "px; top: " + top + "px;\">" + (message ? message : "正在处理，请稍候。。。") + "<input type='text' id='loadtxt' style='height:1px; width:1px;border:none'></div>";

        var loading = $(htmCode).appendTo("body");

        loading.close = function () {
            $("#" + guid).remove();
            $("#msg" + guid).remove();
        };

        //失去焦点达到警用快捷键
        $("#loadtxt").focus();

        if (closeTime > 0) {
            setTimeout(function () {
                $("#" + guid).remove();
                $("#msg" + guid).remove();
            }, closeTime);
        }

        return loading;
    },
    //替换尖括号
    replaceCode: function (value) {
        return value ? value.toString().replace(/</g, '&lt;').replace('/>/', '&gt;') : "";
    },
    //关闭当前打开的选项卡
    tabClose: function () {
        window.top.$('#tabs').tabs("close", window.top.$('#tabs').tabs('getTabIndex', window.top.$("#tabs").tabs("getSelected")));
    },
    //刷新父级Tab窗口的grid
    reParentTabGrid: function (gridId) {
        if (window.frameElement.tabs) {
            gridId = gridId ? gridId : "grid";
            window.frameElement.tabs.wapi.$("#" + gridId + "").datagrid("reload");
        }
    },
    //修改当前选中卡的标题和ICON
    updateTabTitle: function (title, icon) {
        window.top.$(".tabs-selected span:first").text(title);
        if (icon) {
            window.top.$(".tabs-selected span:eq(1)").removeClass();
            window.top.$(".tabs-selected span:eq(1)").addClass("tabs-icon " + icon);
        }
    },
    //数字输入左带0不删除
    onlymath: function () {
        $(".onlymath").keypress(function (evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : ""); //兼容IE和Firefox获得keyBoardEvent对象
            var key = evt.keyCode ? evt.keyCode : evt.which;//兼容IE和Firefox获得keyBoardEvent对象的键值 
            return (/[\d.]/.test(String.fromCharCode(key)));
        });
        $(".onlymath").blur(function () {
            if (isNaN($(this).val())) {
                var value = "";
                var array = $(this).val().split('');
                for (var i = 0; i < array.length; i++) {
                    if (isNaN(array[i])) {
                        break;
                        ;
                    } else {
                        value += array[i];
                    }
                }
                $(this).val(value);
            }
        });
    },
    //打开新选项卡
    openNewTab: function (title, url, icon, parentWin) {
        
        //判断title是否存在存在直接选中 否则添加
        var len = parent.$("#tabs .tabs li").length;
        for (var i = 0; i < len; i++) {
            var text = parent.$("#tabs .tabs li:eq(" + i + ")").text();
            if (text == title) {
                //alert("exit");
                parent.$("#tabs").tabs('select', i);
                return false;
            }
        }

        var content = $('<iframe scrolling="auto" frameborder="0" easyuiTabs="true" src="' + url + '" style="width:100%;height:98%;position:relative;"></iframe>');
        if (parentWin) {
            content.get(0).tabs = { wapi: parentWin };
        }

        parent.$("#tabs").tabs('add', {
            title: title,
            content: content,//'<iframe frameborder="0" scrolling="true" src="' + url + '" style="width:100%;height:98%;position:relative;"></iframe>',
            closable: true,
            icon: icon
        });

        if (typeof window.top.tabClose == "function") {
            window.top.tabClose();
        }
        return true;
    },
    //得到当前时间时分秒-不传参数为默认格式
    nowDateTime: function (formatter) {
        //nowDate 在页面初始化了系统时间
        var date = new Date(nowDate);
        var year = date.getFullYear();
        var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : ("0" + (date.getMonth() + 1));
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
    },
    //格式化日期
    dateTimeFormat: function (value, formatter) {
        return value.DataFormat(formatter);
    },
    //超出单元格文字自动隐藏
    formatText: function (value) {
        value = frxs.replaceCode(value);
        return "<table style='table-layout:fixed;width:100%;'><tr><td style='border:none;white-space:nowrap;overflow:hidden; text-overflow:ellipsis;' title='" + value + "'>" + value + "</td></tr></table>";
    },
    //替换过滤斜杠和双引号
    filterText: function (value) {
        return value.replace(/\\/g, '\\\\').replace(/"/g, '\\"');
    },
    //设置标题颜色
    setTitleColor: function (value) {
        return "<span style='color:blue'>" + value + "</span>";
    },
    //设置标题颜色
    setColumnColor: function () {
        return "color:blue";
    },
    ///序列化URL
    serializeURL2KeyVal: function (url) {
        /// <summary>序列化URL</summary>
        /// <param name="url" type="string">序列为键值对。 不填写 默认为当前地址</param>
        url = url || window.location.search;
        url = url.split("?"),
        url = url[url.length - 1];
        var parameter = url.split("&");
        var result = [];
        for (var i = 0; i < parameter.length; i++) {
            var val = parameter[i].split("=");
            result.push({ key: val[0], value: $.trim(val[1]) });
        }
        return $(result);
    },
    ///序列化URL
    serializeURL2Json: function (url) {
        /// <summary>序列化URL</summary>
        /// <param name="url" type="string">序列为键值对。 不填写 默认为当前地址</param>
        var result = {};
        this.serializeURL2KeyVal(url).each(function () {
            result[this.key] = this.value;
        });
        return result;
    }
};


$(function () {
    //初始化
    frxs.onlymath();
    //初始化只能输入数字和小数点
    $(".NumDecText").keyup(function () {
        $(this).val($(this).val().replace(/[^0-9.]/g, ''));
    }).bind("paste", function () {  //CTR+V事件处理    
        $(this).val($(this).val().replace(/[^0-9.]/g, ''));
    }).css("ime-mode", "disabled"); //CSS设置输入法不可用   

    $(".onlymath").keyup(function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    }).bind("paste", function () {  //CTR+V事件处理    
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    }).css("ime-mode", "disabled"); //CSS设置输入法不可用  
    //初始化给按钮加title
    setTimeout(function () {
        $("#btnAdd").attr("title", "【ALT+A】或【Insert】");
        $("#btnDel").attr("title", "【ALT+D】或【Delect】");
        $("#btnSave").attr("title", "【F8】");
        $("#btnEdit").attr("title", "【F7】");
        $("#btnSure").attr("title", "【F9】");
        $("#btnPost").attr("title", "【F10】");
        $("#btnClose").attr("title", "【ESC】");
    }, 2000);

});

//快捷方式方法
//快捷键说明
//添加：【ALT+A】或者【Insert】
//提交：【ALT+S】
//删除：【ALT+D】或者【Delect】
//编辑：【F7】
//保存：【F8】
//确认：【F9】
//过账：【F10】
//关闭：【ESC】
$(document).on('keydown', function (e) {

    if (e.keyCode != 13) {
        if ($(".messager-window").length > 0) {
            return false;
        }
        if ($(".datagrid-mask-msg").length > 0) {
            return false;
        }
    }


    if (e.altKey
        //&& e.keyCode == 222
        ) {
        switch (e.keyCode) {
            case 65:
                $("#btnAdd").trigger("click");//添加
                break;
            case 68:
                $("#btnDel").trigger("click");//删除
                break;
        }
    }
    else {
        switch (e.keyCode) {
            case 45: //Insert
                $("#btnAdd").trigger("click");//添加
                break;
            case 119://F8
                $("#btnSave").trigger("click");//保存
                break;
            case 46://Delect
                $("#btnDel").trigger("click");//删除
                break;
            case 118://F7
                $("#btnEdit").trigger("click");//编辑
                break;
            case 120://F9
                $("#btnSure").trigger("click");//确认
                break;
            case 121://F10
                $("#btnPost").trigger("click");//过账
                break;
            case 27://ESC
                $("#btnClose").trigger("click");//关闭
                break;
            case 13://回车
                $("#aSearch").trigger("click");//列表搜索按钮
                break;
        }
    }
});
//扩展numberbox方法，限定numberbox输入多少位数
//$('.try').numberbox('limit',{lengths:10});
$.extend($.fn.numberbox.methods, {
    limit: function (jq, param) {
        return jq.each(function () {
            var nbox = $(this);
            var nboxinput = nbox.next().children('input[class*="textbox-text validatebox-text"]');
            $(nboxinput).each(function (index, value) {
                $(value).attr('maxlength', param.lengths);
            });
        });
    }
});


$(function() {
    //服务器时间与本机时间验证
    //nowDate 在页面初始化了系统时间
    //var date = new Date("2016-07-06 17:00");
    var date = new Date(nowDate);
    var jsDate = new Date();
    if (Math.abs(jsDate - date) / 1000 > 60) {
        if (parent.$(".panel-tool-close").length <= 0) {
            window.top.$.messager.alert("提示", "本机时间与服务器时间相差[" + parseFloat(Math.abs(jsDate - date) / 1000 / 60).toFixed(0) + "]分钟。", "error", function() {
                parent.location.href = 'Home/Logout';
            });
            parent.$(".panel-tool-close").click(function() {
                parent.location.href = 'Home/Logout';
            });
        }
    }
});
