$(function () {
    //自动切换主题
    atuoTheme();
});


//更换主题
function changeTheme(themeName) {
    var themeeasy = $('#themeeasy');
    var url = themeeasy.attr('href');

    var href = url.substring(0, url.indexOf('themes')) + 'themes/' + themeName + '/easyui.css';
    themeeasy.attr('href', href);
    
    var iframe = $('iframe');
    if (iframe.length > 0) {
        for (var i = 0; i < iframe.length; i++) {
            var ifr = iframe[i];
            $(ifr).contents().find('#themeeasy').attr('href', href);
        }
    }

    $.cookie('easyuiThemeName', themeName, {
        expires: 7,
        path: '/'
    });
}


//自动切换主题
function atuoTheme() {
    if ($.cookie('easyuiThemeName')) {
        changeTheme($.cookie('easyuiThemeName'));
    }
}
