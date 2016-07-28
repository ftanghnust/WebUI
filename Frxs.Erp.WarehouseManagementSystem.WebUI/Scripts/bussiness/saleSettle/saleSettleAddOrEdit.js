
var SettleId = "";
var type = "";
$(function () {
    //初始门店结算单
    initSaleSettle();
});

//加载数据
function initSaleSettle() {

    SettleId = frxs.getUrlParam("SettleId");
    type = frxs.getUrlParam("Type");

    //绑定结算方式
    initSettleType();

    eventBind();

    if (SettleId) {
        var load = frxs.loading("正在加载中，请稍后...");
        $.ajax({
            url: "../SaleSettle/GetSaleSettleInfoNew",
            type: "post",
            data: { SettleId: SettleId },
            dataType: 'json',
            success: function (obj) {
                load.close();
                if (obj.Flag == "SUCCESS") {
                    var saleSettle = obj.Data.SaleSettle;
                    var saleSettleDetailList = $.parseJSON(obj.Data.SaleSettleDetailList);
                    //格式化日期
                    if (saleSettle != null) {
                        obj.CreateTime = frxs.dateFormat(saleSettle.CreateTime);
                        obj.ConfTime = frxs.dateFormat(saleSettle.ConfTime);
                        obj.PostingTime = frxs.dateFormat(saleSettle.PostingTime);
                        $("#formAdd").form("load", saleSettle);
                        $("#hidShopCode").val(saleSettle.ShopCode);
                        $('#SettleType').combobox('setValue', saleSettle.SettleType);
                        //明细数据加载
                        loadgrid(saleSettleDetailList);
                        totalCalculate();
                        //高度自适应
                        gridresize();
                        if (saleSettle.Status == 0) {//录单
                            if (type == "edit") {
                                SetPermission('edit');
                            } else {
                                SetPermission('query');
                            }
                        }
                        else if (saleSettle.Status == 1) {//确认
                            SetPermission('sure');
                        }
                        else {
                            SetPermission('posting');
                        }
                    }
                    else {
                        $.messager.alert("提示", "编辑读取数据失败,请重新读取！", "info");
                        return;
                    }
                }
                else {
                    $.messager.alert("提示", "编辑读取数据失败,请重新读取！", "info");
                    return;
                }
            }
        });
    } else {

        //初始化主仓库
        InitParentWarehores();
        //加载数据
        loadgrid();
        //高度自适应
        gridresize();
        SetPermission('add');
    }


}

//实现对DataGird控件的绑定操作
function loadgrid(griddata) {
    $('#gridDetail').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式 
        data: griddata,
        idField: 'BillDetailsID',                  //主键
        fit: false,                         //分页在最下面
        pagination: false,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: true,                     //奇偶行是否区分
        showFooter: true,
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#gridDetail').datagrid('clearSelections');
            $('#gridDetail').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            SettleId: SettleId
        },
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true },
            { title: '单据编号', field: 'BillID', width: 120 }
        ]],
        columns: [[
             { title: '账单日期', field: 'BillDate', width: 120, formatter: frxs.ymdFormat, align: 'center' },
             {
                 // 单据类型(0:销售订单;1:销售退货单;2:销售费用单) 
                 title: '单据类型', field: 'BillType', width: 120, align: 'left', formatter: function (value, rowData) {
                     var billTypeName = "";
                     if (rowData.BillType == 0) {
                         billTypeName = "销售订单";
                     }
                     else if (rowData.BillType == 1) {
                         billTypeName = "销售退货单";
                     }
                     else if (rowData.BillType == 2) {
                         billTypeName = "销售费用单";
                     }
                     else {
                         billTypeName = rowData.BillType;
                     }
                     return billTypeName;
                 }
             },
            { title: '二级项目名称', field: 'FeeName', width: 120 },
            { title: '金额', field: 'BillAmt', width: 180 },
            { title: '平台费', field: 'BillAddAmt', width: 180 },
            { title: '小计', field: 'BillPayAmt', width: 180 }
        ]],
        toolbar: [{
            text: '添加',
            id: 'btnAdd',
            iconCls: 'icon-add',
            handler: addGridRow
        }, {
            text: '删除',
            id: 'btnDel',
            iconCls: 'icon-remove',
            handler: del
        }]
    });
};


//总额计算
function totalCalculate() {
    var rows = $("#gridDetail").datagrid("getRows");
    var totalBillAmt = 0.0000;
    var totalBillAddAmt = 0.0000;
    var totalBillPayAmt = 0.0000;
    for (var i = 0; i < rows.length; i++) {
        var billAmt = parseFloat(rows[i].BillAmt);
        var billAddAmt = parseFloat(rows[i].BillAddAmt);
        var billPayAmt = parseFloat(rows[i].BillPayAmt);
        totalBillAmt += billAmt;
        totalBillAddAmt += billAddAmt;
        totalBillPayAmt += billPayAmt;
    }
    var billAmt1 = "金额总计:" + parseFloat(totalBillAmt).toFixed(2);
    var billAddAmt1 = "平台费总计:" + parseFloat(totalBillAddAmt).toFixed(2);
    var billPayAmt1 = "小计总计:" + parseFloat(totalBillPayAmt).toFixed(2);
    $('#gridDetail').datagrid('reloadFooter', [
        {
            "BillAmt": billAmt1,
            "BillAddAmt": billAddAmt1,
            "BillPayAmt": billPayAmt1
        }
    ]);
}


//门店事件绑定
function eventBind() {
    $("#ShopCode").keydown(function (e) {
        if (e.keyCode == 13) {
            eventShop();
        }
    });

    $("#ShopName").keydown(function (e) {
        if (e.keyCode == 13) {
            eventShop();
        }
    });
}

//退货门店 回车事件
function eventShop() {
    $.ajax({
        url: "../Common/GetShopInfo",
        type: "post",
        data: {
            shopCode: $("#ShopCode").val(),
            //shopName: $("#ShopName").val(),
            page: 1,
            rows: 200
        },
        success: function (obj) {
            obj = JSON.parse(obj);
            if (obj.total == 1) {
                var rows = $('#gridDetail').datagrid("getRows");
                var shopId = obj.rows[0].ShopID;
                var shopCode = obj.rows[0].ShopCode;
                var shopName = obj.rows[0].ShopName;
                var shopType = obj.rows[0].ShopType;
                var bankAccountName = obj.rows[0].BankAccountName;
                var bankType = obj.rows[0].BankType;
                var bankAccount = obj.rows[0].BankAccount;
                var creditAmt = obj.rows[0].CreditAmt;
                var settleType = obj.rows[0].SettleType;

                if ($("#ShopID").val() != "" && $("#ShopID").val() != obj.rows[0].ShopID) {
                    if (rows.length > 0 && rows[0].BillID) {
                        $.messager.confirm("提示", "确认更换退货门店吗?（将清除结算单明细信息）", function (r) {
                            if (r) {
                                if (rows) {

                                    $('#grid').datagrid('loadData', { total: 0, rows: [] });
                                    totalCalculate();
                                    $("#ShopID").val(shopId);
                                    $("#ShopCode").val(shopCode);
                                    $("#hidShopCode").val(shopCode);
                                    $("#ShopName").val(shopName);
                                    $("#ShopType").val(shopType);
                                    $('#SettleType').combobox('setValue', settleType);
                                    $("#BankAccountName").val(bankAccountName);
                                    $("#BankType").val(bankType);
                                    $("#BankAccount").val(bankAccount);
                                    $("#CreditAmt").val(creditAmt);

                                }
                            } else {
                                return false;
                            }
                        });
                    } else {
                        $("#ShopID").val(shopId);
                        $("#ShopCode").val(shopCode);
                        $("#hidShopCode").val(shopCode);
                        $("#ShopName").val(shopName);
                        $("#ShopType").val(shopType);
                        $('#SettleType').combobox('setValue', settleType);
                        $("#BankAccountName").val(bankAccountName);
                        $("#BankType").val(bankType);
                        $("#BankAccount").val(bankAccount);
                        $("#CreditAmt").val(creditAmt);
                    }
                } else {
                    $("#ShopID").val(shopId);
                    $("#ShopCode").val(shopCode);
                    $("#hidShopCode").val(shopCode);
                    $("#ShopName").val(shopName);
                    $("#ShopType").val(shopType);
                    $('#SettleType').combobox('setValue', settleType);
                    $("#BankAccountName").val(bankAccountName);
                    $("#BankType").val(bankType);
                    $("#BankAccount").val(bankAccount);
                    $("#CreditAmt").val(creditAmt);
                }
            } else {
                selShop();
            }
        }
    });
}



//选择门店
function selShop() {
    var shopCode = $("#ShopCode").val();
    var thisdlg = frxs.dialog({
        title: "选择门店",
        url: "../SaleSettle/SelectShop?shopCode=" + encodeURIComponent(shopCode),//+ "&shopName=" + encodeURIComponent(shopName)
        owdoc: window.top,
        width: 850,
        height: 500,
        buttons: [{
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

//回填门店
function saleSettleFillShop(rowData) {
    var shopId = rowData.ShopID;
    var shopCode = rowData.ShopCode;
    var shopName = rowData.ShopName;
    var shopType = rowData.ShopType;
    var bankAccountName = rowData.BankAccountName;
    var bankType = rowData.BankType;
    var bankAccount = rowData.BankAccount;
    var creditAmt = rowData.CreditAmt;
    var settleType = rowData.SettleType;
    var rows = $('#gridDetail').datagrid("getRows");
    if ($("#ShopID").val() != "" && $("#ShopID").val() != shopId) {
        if (rows.length > 0 && rows[0].BillID) {
            $.messager.confirm("提示", "确认更换门店吗?（将清除结算单明细信息）", function (r) {
                if (r) {
                    if (rows) {

                        $('#gridDetail').datagrid('loadData', { total: 0, rows: [] });
                        totalCalculate();
                        $("#ShopID").val(shopId);
                        $("#ShopCode").val(shopCode);
                        $("#hidShopCode").val(shopCode);
                        $("#ShopName").val(shopName);
                        $("#ShopType").val(shopType);
                        $('#SettleType').combobox('setValue', settleType);
                        $("#BankAccountName").val(bankAccountName);
                        $("#BankType").val(bankType);
                        $("#BankAccount").val(bankAccount);
                        $("#CreditAmt").val(creditAmt);
                    }
                } else {
                    return false;
                }
            });
        } else {
            $("#ShopID").val(shopId);
            $("#ShopCode").val(shopCode);
            $("#hidShopCode").val(shopCode);
            $("#ShopName").val(shopName);
            $("#ShopType").val(shopType);
            $('#SettleType').combobox('setValue', settleType);
            $("#BankAccountName").val(bankAccountName);
            $("#BankType").val(bankType);
            $("#BankAccount").val(bankAccount);
            $("#CreditAmt").val(creditAmt);
        }
    } else {
        $("#ShopID").val(shopId);
        $("#ShopCode").val(shopCode);
        $("#hidShopCode").val(shopCode);
        $("#ShopName").val(shopName);
        $("#ShopType").val(shopType);
        $('#SettleType').combobox('setValue', settleType);
        $("#BankAccountName").val(bankAccountName);
        $("#BankType").val(bankType);
        $("#BankAccount").val(bankAccount);
        $("#CreditAmt").val(creditAmt);
    }

}


//添加
function addGridRow() {
    var shopId = $("#ShopID").val();
    if (!shopId) {
        $.messager.alert("提示", "没有选择门店,不能添加数据！", "info");
        return;
    }
    var thisdlg = frxs.dialog({
        title: "门店费用单选取",
        url: "/SaleSettle/SelectSaleSettleDetail?ShopID=" + shopId,
        owdoc: window.top,
        width: 800,
        height: 650,
        buttons: [{
            text: '<div title=【ALT+S】>保存</div>',
            iconCls: 'icon-ok',
            handler: function () {
                thisdlg.subpage.selectSaleSettleDetailsData();
            }
        }, {
            text: '<div title=【ESC】>关闭</div>',
            iconCls: 'icon-cancel',
            handler: function () {
                window.focus();
                thisdlg.dialog("close");
            }
        }]
    });
}

//设置门店结算单明细数据
function setSaleSettleDetailsData(rows) {
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (!rowBillIdExist(row["BillDetailsID"])) {
            $('#gridDetail').datagrid('appendRow', row);
        }
    }
    totalCalculate();
}


//判断是否有重复数据，有重复数据去重(判断单据是否相同,单据明细是否相同)
function rowBillIdExist(billDetailsId) {
    var rows = $('#gridDetail').datagrid("getRows");
    for (var i = 0, l = rows.length; i < l; i++) {
        var row = rows[i];
        if (row["BillDetailsID"] == billDetailsId) {
            return true;
        }
    }
    return false;
}


//内存中删除
function del() {
    var ids = "";
    $(".datagrid-body:first table tr").each(function (i) {
        if ($(this).hasClass("datagrid-row-selected")) {
            ids = i + "," + ids;
        }
    });

    var rows = $('#gridDetail').datagrid("getRows");
    var copyRows = [];
    for (var j = 0; j < ids.split(',').length; j++) {
        var ci = ids.split(',')[j];
        if (ci) {
            copyRows.push(rows[ci]);
        }
    }
    for (var i = 0; i < copyRows.length; i++) {
        var ind = $('#gridDetail').datagrid('getRowIndex', copyRows[i]);
        $('#gridDetail').datagrid('deleteRow', ind);
    }

    var newrows = $('#gridDetail').datagrid("getRows");
    totalCalculate(newrows);
}


//保存
function saveData() {
    var validate = $("#formAdd").form('validate');
    if (!validate) {
        return false;
    }

    if ($("#hidShopCode").val() != $("#ShopCode").val()) {
        $.messager.alert("提示", "结算门店编码有误，请重新选择！", "info");
        return false;
    }

    var rows = $('#gridDetail').datagrid('getRows');
    var jsonDetails = JSON.stringify(rows);

    if (rows.length == 0) {
        $.messager.alert("提示", "明细数据不能为空！", "info");
        return false;
    }

    var loading = frxs.loading("正在加载中，请稍后...");
    var jsonSaleSettleserialize = $("#formAdd").serialize();//$("#formAdd").serializeArray();
    var jsonSaleSettle = JSON.stringify(frxs.serializeURL2Json(jsonSaleSettleserialize));
    $.ajax({
        url: "../SaleSettle/SaleSettleAddOrEditeHandle",
        type: "post",
        data: { jsonData: jsonSaleSettle, jsonDetails: jsonDetails },
        dataType: "json",
        success: function (result) {
            loading.close();
            //result = jQuery.parseJSON(result);
            if (result.Flag == "SUCCESS") {
                if ($("#SettleID").val() != "") {
                    $.messager.alert("提示", "编辑成功", "info");
                } else {
                    var data = jQuery.parseJSON(result.Data);
                    SettleId = result.Data.SettleID;
                    $("#SettleID").val(result.Data.SettleID);
                    $("#CreateUserName").val(result.Data.UserName);
                    $("#CreateTime").val(result.Data.Time);
                    $.messager.alert("提示", "添加成功", "info");
                }

                frxs.updateTabTitle("查看门店结算单" + result.Data.SettleID, "icon-search");

                SetPermission("save");
            } else {
                $.messager.alert("提示", result.Info, "info");
            }

        }
    });
}



function InitParentWarehores() {
    $.ajax({
        url: '../SaleSettle/InitParentWarehores',
        type: 'get',
        data: {},
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            $("#WName").val(data.WName);
            $("#WCode").val(data.WCode);
            $("#WID").val(data.WID);
        }, error: function (e) {

        }
    });
}

//加载仓库编号和结算方式下拉列表
function initSettleType() {
    $.ajax({
        url: '../Common/GetVendorDllInfo',
        type: 'get',
        data: {
            dictType: "ShopSettleType"
        },
        success: function (data) {
            //在第一个Item加上请选择
            data = $.parseJSON(data);
            if (data.length > 1) {
                data.unshift({ "DictValue": "", "DictLabel": "-全部-" });
            }
            //创建控件
            $("#SettleType").combobox({
                data: data,             //数据源
                valueField: "DictValue",       //id列
                textField: "DictLabel"      //value列
            });
            $("#SettleType").combobox('select', data[0].DictValue);
        }, error: function (e) {
            debugger;
        }
    });
};


// 门店结算单 添加
function add() {
    location.href = '../SaleSettle/SaleSettleAddOrEdit';
    frxs.updateTabTitle("添加门店结算单", "icon-add");
}

//门店结算单 编辑
function edit() {
    SetPermission("edit");
    frxs.updateTabTitle("编辑门店结算单" + $("#SettleID").val(), "icon-edit");
}


//门店结算单 确认
function sure() {
    var status = $("#Status").combobox('getValue');
    if (status != "0") {
        $.messager.alert("提示", "录单状态才能确认！", "info");
        return;
    }
    var settleId = $("#SettleID").val();
    if (settleId != "") {
        $.ajax({
            url: "../SaleSettle/SaleSettleSureOrNo",
            type: "post",
            dataType: "json",
            data: {
                settleIds: settleId,
                status: 1,
                issure: 1 //确认
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "确认成功！", "info");
                        $("#Status").combobox('setValue', '1');
                        $("#ConfUserName").val(result.Data.UserName);
                        $("#ConfTime").val(result.Data.Time);
                        SetPermission("sure");
                    } else {
                        $.messager.alert("提示", result.Info, "info");
                    }
                }
            },
            error: function (request, textStatus, errThrown) {
                if (textStatus) {
                    $.messager.alert("提示", textStatus, "info");
                } else if (errThrown) {
                    $.messager.alert("提示", errThrown, "info");
                } else {
                    $.messager.alert("提示", "出现错误", "info");
                }
            }
        });
    }
}

//门店结算单 反确认
function noSure() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能反确认！", "info");
        return;
    }
    var settleId = $("#SettleID").val();
    if (settleId != "") {
        $.ajax({
            url: "../SaleSettle/SaleSettleSureOrNo",
            type: "get",
            dataType: "json",
            data: {
                settleIds: settleId,
                status: 0,
                issure: 0 //反确认
            },
            success: function (result) {
                if (result != undefined && result.Info != undefined) {
                    if (result.Flag == "SUCCESS") {
                        $.messager.alert("提示", "反确认成功！", "info");
                        $("#Status").combobox('setValue', '0');
                        $("#ConfUserName").val("");
                        $("#ConfTime").val("");
                        SetPermission("nosure");
                    } else {
                        $.messager.alert("提示", result.Info, "info");
                    }
                }
            },
            error: function (request, textStatus, errThrown) {
                if (textStatus) {
                    $.messager.alert("提示", textStatus, "info");
                } else if (errThrown) {
                    $.messager.alert("提示", errThrown, "info");
                } else {
                    $.messager.alert("提示", "出现错误", "info");
                }
            }
        });
    }
}

//门店结算单 过账
function posting() {
    var status = $("#Status").combobox('getValue');
    if (status != "1") {
        $.messager.alert("提示", "确认状态才能过账！", "info");
        return;
    }

    var settleId = $("#SettleID").val();
    if (settleId != "") {
        $.messager.confirm("提示", "是否过账单据?", function (r) {
            if (r) {
                $.ajax({
                    url: "../SaleSettle/SaleSettlePost",
                    type: "get",
                    dataType: "json",
                    data: {
                        settleIds: settleId
                    },
                    success: function (result) {
                        if (result != undefined && result.Info != undefined) {
                            if (result.Flag == "SUCCESS") {
                                $.messager.alert("提示", "过账成功！", "info");
                                $("#Status").combobox('setValue', '2');
                                $("#PostingUserName").val(result.Data.UserName);
                                $("#PostingTime").val(result.Data.Time);
                                SetPermission("posting");
                            } else {
                                $.messager.alert("提示", result.Info, "info");
                            }
                        }
                    },
                    error: function (request, textStatus, errThrown) {
                        if (textStatus) {
                            $.messager.alert("提示", textStatus, "info");
                        } else if (errThrown) {
                            $.messager.alert("提示", errThrown, "info");
                        } else {
                            $.messager.alert("提示", "出现错误", "info");
                        }
                    }
                });
            }
        });
    }
}



//设置Button权限
function SetPermission(type) {
    switch (type) {
        case "add":
            $("#btnAdd2").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#ShopCode").removeAttr("disabled");
            $("#btnSelShop").removeAttr("disabled");
            break;
        case "edit":
            $("#btnAdd2").linkbutton('disable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('disable');
            $("#btnSave").linkbutton('enable');
            $("#btnAdd").linkbutton('enable');
            $("#btnDel").linkbutton('enable');
            $("#ShopCode").removeAttr("disabled");
            $("#btnSelShop").removeAttr("disabled");
            break;
        case "query":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#ShopCode").attr({ "disabled": "disabled" });
            $("#btnSelShop").attr({ "disabled": "disabled" });
            break;
        case "nosure":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#ShopCode").attr({ "disabled": "disabled" });
            $("#btnSelShop").attr({ "disabled": "disabled" });
            break;
        case "sure":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('enable');
            $("#btnPost").linkbutton('enable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#ShopCode").attr({ "disabled": "disabled" });
            $("#btnSelShop").attr({ "disabled": "disabled" });
            break;
        case "posting":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('disable');
            $("#btnSure").linkbutton('disable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#ShopCode").attr({ "disabled": "disabled" });
            $("#btnSelShop").attr({ "disabled": "disabled" });
            break;
        case "save":
            $("#btnAdd2").linkbutton('enable');
            $("#btnEdit").linkbutton('enable');
            $("#btnSure").linkbutton('enable');
            $("#reSure").linkbutton('disable');
            $("#btnPost").linkbutton('disable');
            $("#exportBtn").linkbutton('enable');
            $("#btnSave").linkbutton('disable');
            $("#btnAdd").linkbutton('disable');
            $("#btnDel").linkbutton('disable');
            $("#ShopCode").removeAttr("disabled");
            $("#btnSelShop").removeAttr("disabled");
            break;
    }
}


//导出
function exportData() {
    //site_common.ShowProgressBar();
    location.href = "../SaleSettle/DataExportItems?SettleID=" + SettleId;
    //site_common.HideProgressBar();
}


//打印门店结算单
function print() {
    var settleId = $("#SettleID").val();
    if (settleId != "") {
        var thisdlg = frxs.dialog({
            title: "打印门店结算单",
            url: "../FastReportTemplets/Aspx/PrintSaleSettleIn6a.aspx?SettleID=" + settleId,
            owdoc: window.top,
            width: 765,
            height: 600
        });
    } else {
        $.messager.alert("提示", "结算单号不能为空！", "info");
    }
}

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("fieldset").height() - $("#formAdd").height() - 21);
    $('#gridDetail').datagrid('resize', {
        width: $(window).width() - 10,
        height: h
    });
}

