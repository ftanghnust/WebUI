
$(function () {
    InitLeftMenu();
    tabClose();
    tabCloseEven();
    
    ////clockon();  //时钟
    //$('#tabs').tabs('add', {
    //    title: '欢迎',
    //    //content: createFrame('home/welcome'),
    //    //content: '<H1 style="margin:10px;color:red">HELLO WORLD！！<BR /><BR />GOOD GOOD STUDY, DAY DAY UP！！</H1>'
    //});
    $('#tabs').tabs('add', {
        title: '目录导航',
        content: createFrame('home/Direction'),
       
        selected: true,
    });

    atuoTheme();

    $("#tabs").tabs({
        onSelect: function () {
            $("#tabs").tabs("getSelected").find("iframe").focus();//当前tab下获得焦点
        }
    });
});

//得到GUID
function guid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
};

//初始化左侧
function InitLeftMenu() {

    $("#nav").accordion({ animate: true });

    $.ajax({
        url: "../Home/GetMenu",
        type: "get",
        success:function (obj) {
            $($.parseJSON(obj)).each(function () {
                var menulist = '';
                menulist += '<ul>';
                
                $(this.children).each(function (i) {
                    menulist += '<li><div><a ref="' + guid() + '" href="#" rel="' + this.url + '" ><span class="icon icon-page" >&nbsp;</span><span class="nav">' + this.text + '</span></a></div></li> ';
                    //menulist += '<li><div><a ref="' + guid() + '" href="#" rel="../Vendor/EasyuiVendorList" ><span class="icon icon-add" >&nbsp;</span><span class="nav">' + this.text + '</span></a></div></li> ';
                    
                });
                menulist += '</ul>';

                $('#nav').accordion('add', {
                    title: this.text,
                    content: menulist,
                    iconCls: 'icon icon-page2'
                });

            });
            

            //选中第一个
            var panels = $('#nav').accordion('panels');
            var t = panels[0].panel('options').title;
            $('#nav').accordion('select', t);


            $('.easyui-accordion li a').click(function () {
                var tabTitle = $(this).children('.nav').text();

                var url = $(this).attr("rel");
                var menuid = $(this).attr("ref");
                //var icon = getIcon(menuid, icon);
                var icon = "icon icon-page";

                addTab(tabTitle, url, icon);
                $('.easyui-accordion li div').removeClass("selected");
                $(this).parent().addClass("selected");
            }).hover(function () {
                $(this).parent().addClass("hover");
            }, function () {
                $(this).parent().removeClass("hover");
            });

        }
    });

    

    

    
}
//获取左侧导航的图标
function getIcon(menuid) {
    var icon = 'icon ';
    $.each(_menus.menus, function(i, n) {
        $.each(n.menus, function(j, o) {
            if (o.menuid == menuid) {
                icon += o.icon;
            }
        });
    });

    return icon;
}

function addTab(subtitle, url, icon, win) {
    if (!$('#tabs').tabs('exists', subtitle)) {

        var content = $('<iframe scrolling="auto" frameborder="0" easyuiTabs="true" src="' + url + '" style="width:100%;height:98%;"></iframe>');

        if (win) {
            content.get(0).tabs.wapi = win;
        }

        $('#tabs').tabs('add', {
            title: subtitle,
            content: content,
            closable: true,
            icon: icon
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        //$('#mm-tabupdate').click();
    }
    tabClose();
}

function createFrame(url) {
    var s = '<iframe scrolling="auto" frameborder="0" src="' + url + '" style="width:100%;height:99%;"></iframe>';
    return s;
}

function tabClose() {
    /*双击关闭TAB选项卡*/
    $(".tabs-inner").dblclick(function() {
        var subtitle = $(this).children(".tabs-closable").text();
        $('#tabs').tabs('close', subtitle);
    });
    /*为选项卡绑定右键*/
    $(".tabs-inner").bind('contextmenu', function (e) {
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        var subtitle = $(this).children(".tabs-closable").text();

        $('#mm').data("currtab", subtitle);
        $('#tabs').tabs('select', subtitle);
        return false;
    });
}
//绑定右键菜单事件
function tabCloseEven() {
    //刷新
    $('#mm-tabupdate').click(function() {
        var currTab = $('#tabs').tabs('getSelected');
        var url = $(currTab.panel('options').content).attr('src');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url)
            }
        });
    });
    //关闭当前
    $('#mm-tabclose').click(function() {
        var currtabTitle = $('#mm').data("currtab");
        $('#tabs').tabs('close', currtabTitle);
    });
    //全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            var t = $(n).text();
            $('#tabs').tabs('close', t);
        });
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        $('#mm-tabcloseright').click();
        $('#mm-tabcloseleft').click();
    });
    //关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length == 0) {
            //msgShow('系统提示','后边没有啦~~','error');
            //alert('后边没有啦~~');
            return false;
        }
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });
    //关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () { 
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length == 0) {
            //alert('到头了，前边没有啦~~');
            return false;
        }
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });

    //退出
    $("#mm-exit").click(function() {
        $('#mm').menu('hide');
    });
}
//本地时钟
function clockon() {
    var now = new Date();
    var year = now.getFullYear(); //getFullYear getYear
    var month = now.getMonth();
    var date = now.getDate();
    var day = now.getDay();
    var hour = now.getHours();
    var minu = now.getMinutes();
    var sec = now.getSeconds();
    var week;
    month = month + 1;
    if (month < 10) month = "0" + month;
    if (date < 10) date = "0" + date;
    if (hour < 10) hour = "0" + hour;
    if (minu < 10) minu = "0" + minu;
    if (sec < 10) sec = "0" + sec;
    var arrWeek = new Array("星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六");
    week = arrWeek[day];
    var time = "";
    time = year + "年" + month + "月" + date + "日" + " " + hour + ":" + minu + ":" + sec + " " + week;

    $("#bgclock").html(time);

    var timer = setTimeout("clockon()", 1000);
}



/**************************绑定导航栏目**********************************************/
var _menus = {
    "menus": [
        {
            "menuid": "1",
            "icon": "icon-role",
            "menuname": "management",
            "menus": [
                { "menuid": "dcfee5be-651e-4aac-8968-ce127e457454", "menuname": "datagrid", "icon": "icon-set", "url": 'orguser/index' },
                //{ "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b9", "menuname": "product", "icon": "icon-set", "url": 'product/index' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b1", "menuname": "accordion", "icon": "icon-set", "url": 'control/accordion' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b2", "menuname": "combobox", "icon": "icon-set", "url": 'control/combobox' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b3", "menuname": "dialog", "icon": "icon-set", "url": 'control/dialog' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b4", "menuname": "messager", "icon": "icon-set", "url": 'control/messager' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b5", "menuname": "tree", "icon": "icon-set", "url": 'control/tree' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b6", "menuname": "combotree", "icon": "icon-set", "url": 'control/combotree' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b8", "menuname": "slider", "icon": "icon-set", "url": 'control/slider' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e456718", "menuname": "tabs", "icon": "icon-set", "url": 'control/tabs' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e456728", "menuname": "input", "icon": "icon-set", "url": 'control/input' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e456738", "menuname": "treegrid", "icon": "icon-set", "url": 'control/treegrid' }
            ]
        }
        //,{
     //    "menuid": "2", "icon": "icon-set", "menuname": "组织管理",
     //    "menus": [{ "menuid": "21", "menuname": "角色管理", "icon": "icon-log", "url": '' },
     //            { "menuid": "22", "menuname": "角色权限", "icon": "icon-database", "url": '' }
     //    ]
     //}
    ]
};

function loginOut() {
    $.messager.confirm('提示', "确定退出系统！", function (res) {
        if (res) {
            location.href = 'Home/Logout';
        }
    });
}


//下载APP
function downloadApp() {
    var htmCode = $("#appHtml").html();
    frxs.dialog({
        title: "客户端程序下载",
        content: htmCode,
        width: 725,
        height: 520,
        modal: true,
        minimizable: false,
        maximizable: false
    });
}
