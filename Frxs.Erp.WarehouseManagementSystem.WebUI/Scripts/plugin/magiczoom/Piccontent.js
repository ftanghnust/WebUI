$(function () {
    var t = $(".number input#text_box");
    $(".number .add").click(function() {
        t.val(parseInt(t.val()) + 1);
        if (t.val() <= 0) {
            t++;
        }
    });
    $(".number .min").click(function () {
        t.val(parseInt(t.val()) - 1);
        if (t.val() <= 0) {
            t.val(0);
        }
    });
    //颜色选择效果
    $("ul li").click(function () {
        $(this).addClass("").siblings().removeClass("");
    });
    //放大镜效果
    $(".detail-box .detail-pic dl dt").hover(function() {
        var _index = $(this).index();
        var P_src = $(".detail-box .detail-main .detail-main-left ul li").eq(_index).find('img').attr('src');
        $(".detail-box .detail-main .detail-main-left ul li").eq(_index).fadeIn().siblings().fadeOut();
        $(".detail-box .bimg img").attr("src", P_src);
    });
    $(".detail-box .detail-main .detail-main-left").hover(function () {
        $(".move").show();
        $(" .bimg").show();
    }, function () {
        $(".move").hide();
        $(".bimg").hide();
    });
    $(".detail-box .detail-main .detail-main-left").mousemove(function (e) {
        var x = e.clientX;//获取鼠标x值
        var y = e.clientY;//获取鼠标y值
        //console.log(x+":"+y);在console插件里面显示出来
        //获取大盒子的左偏移量
        var l = $(".detail-box .detail-main .detail-main-left").offset().left;
        //获取大盒子的上偏移量
        var t = $(".detail-box .detail-main .detail-main-left").offset().top;
        //获取移动方块的宽度的一半
        var w = $(".detail-box .detail-main .detail-main-left .move").width() / 2;
        //获取移动方块的高度的一半
        var h = $(".detail-box .detail-main .detail-main-left .move").height() / 2;
        //计算移动方块的left值
        var _left = x - l - w;
        //计算移动方块的top值
        var _top = y - t - h;
        //alert(_left);
        //控制left的范围
        if (_left < 0) {
            _left = 0;
        } else if (_left > $(".hide").width() - 2 * w - 2) {
            _left = $(".hide").width() - 2 * w - 2;
        }
        //控制top的范围
        if (_top < 0) {
            _top = 0;
        } else if (_top > $(".hide").height() - 2 * h - 2) {
            _top = $(".hide").height() - 2 * h - 2;
        }
        //让小方块跟随鼠标移动
        $(".move").css({ left: _left, top: _top });

        //小方块可移动的最大高度
        var b_top = $(".hide").height() - 2 * h - 2;
        //小方块可移动的最大宽度
        var b_left = $(".hide").width() - 2 * w - 2;
        //小方块的位置与最大位移量的比例
        var a = _left / b_left;
        var b = _top / b_top;
        var big_left = ($(".detail-box .bimg img").width() - $(".detail-box .bimg").width()) * a;
        var big_top = ($(".detail-box .bimg img").height() - $(".detail-box .bimg").height()) * b;
        $(".detail-box .bimg img").css({ left: -big_left, top: -big_top });
    });

});

$(".color ul").find("li").click(function () {
    $("li.red").removeClass("red");
    $(this).addClass("red");
});
