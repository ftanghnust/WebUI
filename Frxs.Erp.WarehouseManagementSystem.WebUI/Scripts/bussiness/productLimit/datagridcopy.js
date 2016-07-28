
function MainGridClickRow(rowIndex, rowData) {
    //var indexTag = $('#sub_datagrid').datagrid('getRowIndex', rowData);
    //if (indexTag >= 0) {
    //    DeleteData($('#sub_datagrid'), indexTag);
    //} else {
    //    AddData($('#sub_datagrid'), rowData);
    //}

    var subselect = false;
    var rows = $('#sub_datagrid').datagrid('getRows');
    if (rows != null && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (rowData.ProductId == rows[i].ProductId) {
                DeleteData($('#sub_datagrid'), $('#sub_datagrid').datagrid('getRowIndex', rows[i]));
                subselect = true;
                //break;
            }
        }
    }
    if (!subselect) {
        AddData($('#sub_datagrid'), rowData);
    }
}

function MainGridClickRow2(rowIndex, rowData) {
    //var indexTag = $('#sub_datagrid').datagrid('getRowIndex', rowData);
    //if (indexTag >= 0) {
    //    DeleteData($('#sub_datagrid'), indexTag);
    //} else {
    //    AddData($('#sub_datagrid'), rowData);
    //}

    var subselect = false;
    var rows = $('#sub_datagrid').datagrid('getRows');
    if (rows != null && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (rowData.GroupID == rows[i].GroupID) {
                DeleteData($('#sub_datagrid'), $('#sub_datagrid').datagrid('getRowIndex', rows[i]));
                subselect = true;
                //break;
            }
        }
    }
    if (!subselect) {
        AddData($('#sub_datagrid'), rowData);
    }
}

function SubGridClickRow(rowIndex, rowData) {
    var indexTag = $('#sub_datagrid').datagrid('getRowIndex', rowData);
    DeleteData($('#sub_datagrid'), indexTag);
}

//添加数据
function AddData(grid, rowdata) {
    grid.datagrid('appendRow', rowdata);

}

//删除数据
function DeleteData(grid, rowIndex) {
    grid.datagrid('deleteRow', rowIndex);
}