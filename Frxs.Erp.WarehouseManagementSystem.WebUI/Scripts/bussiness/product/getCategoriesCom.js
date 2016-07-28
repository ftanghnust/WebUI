//获取 三级 基本分类 列表

$(function () {
    GetOneCategories();
});


//获取一级基本分类
function GetOneCategories() {
    $.ajax({
        url: '../Common/GetChirldCategories',
        type: 'post',
        dataType: "json",
        data: { pcategoryId: null },
        success: function (data) {
            data.unshift({ "CategoryId": "", "Name": "-请选择-" });
            $("#CategoriesId1").combobox({
                onChange: function (newValue) {
                    $('#CategoriesId2').combobox("clear");
                    $("#CategoriesId2").combobox({
                        data: []
                    });
                    $('#CategoriesId3').combobox("clear");
                    $("#CategoriesId3").combobox({
                        data: []
                    });
                    if (newValue != "") {
                        getTwoCategories(newValue);
                    }
                },
                data: data,                       //数据源
                valueField: "CategoryId",       //id列
                textField: "Name"       //text列
            });
        },
        error: function (a, b) {
            var v = a;
        }
    });
}

//获取二级基本分类
function getTwoCategories(parentCategoryId) {
    $.ajax({
        url: '../Common/GetChirldCategories',
        type: 'post',
        dataType: "json",
        data: { pcategoryId: parentCategoryId },
        success: function (data) {

            //在第一个Item加上请选择
            data.unshift({ "CategoryId": "", "Name": "-请选择-" });
            //创建控件
            $("#CategoriesId2").combobox({
                onChange: function (newValue) {
                    $('#CategoriesId3').combobox("clear");
                    $("#CategoriesId3").combobox({
                        data: []
                    });
                    if (newValue != "") {
                        getThreeCategories(newValue);
                    }
                },
                data: data,                       //数据源
                valueField: "CategoryId",       //id列
                textField: "Name"       //text列
            });
        },
        error: function () {
        }
    });
}

//获取三级基本分类
function getThreeCategories(parentCategoryId) {
    $.ajax({
        url: '../Common/GetChirldCategories',
        type: 'post',
        dataType: "json",
        data: { pcategoryId: parentCategoryId },
        success: function (data) {
            //在第一个Item加上请选择
            data.unshift({ "CategoryId": "", "Name": "-请选择-" });
            //创建控件
            $("#CategoriesId3").combobox({
                data: data,                       //数据源
                valueField: "CategoryId",       //id列
                textField: "Name"       //text列
            });
        },
        error: function () {
        }
    });
}
