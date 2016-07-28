
function MainGridClickRow(rowIndex, rowData) {
    var indexTag = $('#sub_datagrid').datagrid('getRowIndex', rowData);
    if (indexTag >= 0) {
        DeleteData($('#sub_datagrid'), indexTag);
    } else {
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