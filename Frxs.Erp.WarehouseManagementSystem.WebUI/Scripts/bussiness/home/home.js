var index = 0;
$('#tree').tree({
    url: '/Home/GetMenu',
    onClick: function (node) {
        if ($('#tree').tree('isLeaf', node.target)) {
            var subtitle = $('#tree').tree('getSelected').text;
            var xx = $('#tree').tree('getSelected').url;
            if (!$('#tabs').tabs('exists', subtitle)) {
                index++;
                $('#tabs').tabs('add', {
                    title: $('#tree').tree('getSelected').text,
                    content: '<iframe frameborder="0" data-easyuiTabs="true" style="width:100%; height:100%;border:0px;"  src=' + xx + '></iframe>',
                    closable: true
                });
            } else {
                $('#tabs').tabs('select', subtitle);
            }
        }
    }
});
function addTabs(subtitle, xx) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        index++;
        $('#tabs').tabs('add', {
            title: subtitle,
            content: '<iframe frameborder="0" data-easyuiTabs="true" style="width:100%; height:100%;border:0px;"  src=' + xx + '></iframe>',
            closable: true
        });
    } else {
        $('#tabs').tabs('select', subtitle);
    }
}