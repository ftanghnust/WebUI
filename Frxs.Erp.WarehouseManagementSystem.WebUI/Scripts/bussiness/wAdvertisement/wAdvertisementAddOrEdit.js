var dialogWidth = 850;
var dialogHeight = 600;
var advertisementId = "";

$(function () {
    //下拉绑定
    initDDL();
    init();
    initImgUpload();
    initProductGrid();
});

//加载数据
function init() {
    advertisementId = frxs.getUrlParam("Id");
    if (advertisementId) {
        $.ajax({
            url: "../Wadvertisement/GetWAdvertisement",
            type: "post",
            data: { id: advertisementId },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);
                //alert(obj.ImagesSrc);
                //$("#upimg").attr("src", obj.ImagesSrc);
                window.focus();
            }
        });
    } else {
        initRegionData();
    }
}

function initImgUpload() {
    var state = 'pending';
    var uploader = WebUploader.create({
        auto: false,
        // swf文件路径
        swf: '/Scripts/plugin/webuploader/uploader.swf',
        // 文件接收服务端。
        server: '/Data/upload_ajax.ashx',
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: '#picker',
        formData: {     //上传图片时附带的参数
            savemethod: "SaveWadvertisementImages"
        },
        // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
        resize: false,
        // 只允许选择图片文件。
        accept: {
            title: 'Images',
            extensions: 'gif,jpg,jpeg,bmp,png',
            mimeTypes: 'image/*'
        }
        , duplicate: false
        // 上传文件个数
        //,fileNumLimit: 1
        //,fileSizeLimit: 200 * 1024 * 1024    // 200 M     上传文件总大小
        , fileSingleSizeLimit: 2 * 1024 * 1024    // 2 M    单个文件大小
    });
    uploader.on('fileQueued', function (file) {
        // 创建缩略图
        uploader.makeThumb(file, function (error, src) {
            if (error) {
                //$img.replaceWith('<span>不能预览</span>');
                alert("该图片不能预览！")
                return;
            }
            //alert(src);
            $("#upimg").attr('src', src);
        }, 200, 200);
    });
    uploader.on('beforeFileQueued', function (file) {
        //var uploadedFiles = uploader.getFiles();
        //if (uploadedFiles != null) {
        //    for (var i = 0; i < uploadedFiles.length; i++) {
        //        uploader.removeFile(uploadedFiles[i]);
        //    }
        //    alert(uploader.getFiles().length);
        //}
        uploader.reset();//每次只上传最后1张图片
    });
    uploader.on('all', function (type) {
        if (type === 'startUpload') {
            state = 'uploading';
        } else if (type === 'stopUpload') {
            state = 'paused';
        } else if (type === 'uploadFinished') {
            state = 'done';
        }
        if (state === 'uploading') {
            $('#ctlBtn').text('暂停上传');
        } else {
            $('#ctlBtn').text('开始上传');
        }
    });
    uploader.on('uploadSuccess', function (file, response) {//上传成功事件
        if (response.status == 1) {
            //alert(file.name);
            var filePath = response.filePath;
            //alert(filePath);
            $('#ImagesSrc').val(filePath);
            //alert($('#ImagesSrc').val());
            //alert("上传图片成功！");
            $.messager.alert("提示", "上传图片成功！", "info");
        } else {
            $.messager.alert("提示", response.msg, "info");
            uploader.reset();//每次只上传最后1张图片
        }
    });
    $('#ctlBtn').on('click', function () {
        if (state === 'uploading') {
            uploader.stop();
        } else {
            uploader.upload();
        }
        return false;
    });
}

//初始化区域下拉菜单
function initRegionData(obj) {
    //if (obj) {
    //    region.init({
    //        ddlProvince: $("#ddlProvince"),
    //        ddlCity: $("#ddlCity"),
    //        ddlCounty: $("#ddlCountry"),
    //        provinceID: obj.ProvinceID,
    //        cityID: obj.CityID,
    //        countyID: obj.RegionID
    //    });
    //} else {
    //    region.init({
    //        ddlProvince: $("#ddlProvince"),
    //        ddlCity: $("#ddlCity"),
    //        ddlCounty: $("#ddlCountry")
    //    });
    //}
}

//保存数据
function saveData() {
    if (advertisementId) {
        addData();
    } else {
        editData();
    }
    //var validate = $("#formAdd").form('validate');
    //if (validate == false) {
    //    return false;
    //} else {
    //    //if ($('#ImagesSrc').val() == "") {
    //    //    $.messager.alert("提示", "请上传图片！", "info");
    //    //    return false;
    //    //}else if ($("#AdvertisementProduct").val() == "") {
    //    //    $.messager.alert("提示", "请添加商品！", "info");
    //    //    return false;
    //    //}
    //    if ($("#AdvertisementProduct").val() == "") {
    //        $.messager.alert("提示", "请添加商品！", "info");
    //        return false;
    //    }
    //    var data = $("#formAdd").serialize();
    //    $.ajax({
    //        url: "../Wadvertisement/WAdvertisementHandle",
    //        type: "post",
    //        data: data,
    //        dataType: 'json',
    //        success: function (obj) {
    //            //alert(obj.Info);
    //            $.messager.alert("提示", obj.Info, "info");
    //            window.frameElement.wapi.reload();
    //            frxs.pageClose();
    //        }
    //    });
    //}
}

function addData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        //if ($('#ImagesSrc').val() == "") {
        //    $.messager.alert("提示", "请上传图片！", "info");
        //    return false;
        //}else if ($("#AdvertisementProduct").val() == "") {
        //    $.messager.alert("提示", "请添加商品！", "info");
        //    return false;
        //}
        if ($("#AdvertisementProduct").val() == "") {
            $.messager.alert("提示", "请添加商品！", "info");
            return false;
        }
        var data = $("#formAdd").serialize();
        var load = window.top.frxs.loading();
        $.ajax({
            url: "../Wadvertisement/AddWAdvertisement",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                load.close();
                //alert(obj.Info);
                $.messager.alert("提示", obj.Info, "info");
                window.frameElement.wapi.reload();
                window.frameElement.wapi.focus();
                frxs.pageClose();
            }
        });
    }
}

function editData() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    } else {
        //if ($('#ImagesSrc').val() == "") {
        //    $.messager.alert("提示", "请上传图片！", "info");
        //    return false;
        //}else if ($("#AdvertisementProduct").val() == "") {
        //    $.messager.alert("提示", "请添加商品！", "info");
        //    return false;
        //}
        if ($("#AdvertisementProduct").val() == "") {
            $.messager.alert("提示", "请添加商品！", "info");
            return false;
        }
        var data = $("#formAdd").serialize();
        var load = window.top.frxs.loading();
        $.ajax({
            url: "../Wadvertisement/EditWAdvertisement",
            type: "post",
            data: data,
            dataType: 'json',
            success: function (obj) {
                load.close();
                //alert(obj.Info);
                $.messager.alert("提示", obj.Info, "info");
                window.frameElement.wapi.reload();
                window.frameElement.wapi.focus();
                frxs.pageClose();
            }
        });
    }
}

//下拉绑定
function initDDL() {

}

function initProductGrid() {
    //alert(advertisementId);
    var ajaxUrl = "";
    if (advertisementId) {
        ajaxUrl = '../Wadvertisement/GetWadvertisementProductList';
        //alert(advertisementId);
    }
    //alert(ajaxUrl);
    $('#gridProduct').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'get',                    //提交方式
        url: ajaxUrl,          //Aajx地址
        sortName: 'ProductId',                 //排序字段
        sortOrder: 'asc',                  //排序方式
        idField: 'ProductId',                  //主键
        width: 600,
        height: 250,
        pageSize: 100,                       //每页条数
        pageList: [100],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex, rowData) {
            $('#gridProduct').datagrid('clearSelections');
            $('#gridProduct').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            //查询条件
            AdvertisementID: advertisementId
        },
        frozenColumns: [[
            //冻结列
        ]],
        columns: [[
            { field: 'ck', checkbox: true }, //选择
            { title: 'ProductId', field: 'ProductId', width: 100, align: 'center', sortable: true, hidden: true },
            { title: '商品编码', field: 'SKU', width: 100, align: 'center' },
            { title: '名称', field: 'ProductName', width: 130, align: 'left' },
            { title: '配送单位', field: 'SaleUnit', width: 100, align: 'left' },
            { title: '包装数', field: 'PackingQty', width: 80, align: 'center' },
            { title: '配送价格', field: 'SalePrice', width: 80, align: 'right' }
        ]],
        toolbar: [
        //{
        //    id: 'btnReload',
        //    text: '刷新',
        //    iconCls: 'icon-reload',
        //    handler: function () {
        //        //实现刷新栏目中的数据
        //        $("#grid").datagrid("reload");
        //    }
        //},
        {
            id: 'btnAdd',
            text: '新增',
            iconCls: 'icon-add',
            handler: addProduct
        }
        , {
            id: 'btnDelete',
            text: '删除',
            iconCls: 'icon-remove',
            handler: delCheckProduct
        }
        //,{
        //    id: 'btnDelete',
        //    text: '清除',
        //    iconCls: 'icon-cancel',
        //    handler: clearProduct
        //}
        ],
        onLoadSuccess: function (data) {
            setAdvertisementProduct();
        }
    });

    $("#btnAddProduct").click(function () { addProduct() });
    $("#btnDelProduct").click(function () { delCheckProduct() });
    $("#btnClearProduct").click(function () { clearProduct() });
}

function setAdvertisementProduct() {
    var value = "";
    var rows = $('#gridProduct').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        //alert(row["ProductId"]);
        value += row["ProductId"] + ",";
    }
    if (value != "") {
        value = value.substr(0, value.length - 1);
    }
    $("#AdvertisementProduct").val(value);
    //alert($("#AdvertisementProduct").val());
}

function delProduct(index) {
    var row = $('#gridProduct').datagrid('getRows')[index];
    var copyRows = [];
    copyRows.push(row);
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridProduct').datagrid('getRowIndex', copyRows[i]);
        $('#gridProduct').datagrid('deleteRow', index);
    }
    $("#gridProduct").datagrid("reload");
    setAdvertisementProduct();
}

function addProduct() {
    var thisdlg = frxs.dialog({
        title: "增加商品到推荐",
        url: "../Wadvertisement/WadvertisementProduct",
        owdoc: window.top,
        width: dialogWidth,
        height: dialogHeight,
        buttons: [{
            text: '提交',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.saveData();
                //$("#grid").datagrid("reload");
                //thisdlg.dialog("close");
            }
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                //$("#grid").datagrid("reload");
                thisdlg.dialog("close");
            }
        }]
    });
}

function delCheckProduct() {
    var rows = $('#gridProduct').datagrid("getSelections");
    var copyRows = [];
    for (var j = 0; j < rows.length; j++) {
        copyRows.push(rows[j]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var index = $('#gridProduct').datagrid('getRowIndex', copyRows[i]);
        $('#gridProduct').datagrid('deleteRow', index);
    }
    setAdvertisementProduct();
}

function clearProduct() {
    $('#gridProduct').datagrid('loadData', { total: 0, rows: [] });
    setAdvertisementProduct();
}

function reload(grid) {
    var rows = grid.datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i]
        if (!rowExist(row["ProductId"])) {
            $('#gridProduct').datagrid('appendRow', row);
        }
    }
    setAdvertisementProduct();
}

function rowExist(id) {
    var rows = $('#gridProduct').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["ProductId"] == id) {
            return true;
        }
    }
    return false;
}

//提交和关闭快捷键
$(document).on('keydown', function (e) {
    if (e.altKey && e.keyCode == 83) {
        saveData()
    }
    else if (e.keyCode == 27) {
        window.frameElement.wapi.focus();//当前窗体的母页面获取焦点为了当关闭窗体后继续相应快捷键
        frxs.pageClose();//弹窗关闭
    }
});
window.focus();