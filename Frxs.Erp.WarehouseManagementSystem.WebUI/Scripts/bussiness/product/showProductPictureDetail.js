function setImg(obj) {
    var smallImg = $(obj).find("img").attr("src");
    $("#showBigImg .img-box #bigImg").attr("src", smallImg);
}


//快捷键在弹出页面里面出发事件
$(document).on('keydown', function (e) {
    if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();