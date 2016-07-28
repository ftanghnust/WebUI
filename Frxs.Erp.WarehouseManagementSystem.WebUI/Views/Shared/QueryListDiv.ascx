<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Frxs.Erp.WarehouseManagementSystem.WebUI.Models.CommonQueryModel>" %>
<script type="text/javascript" src="<%=Url.Content("~/Scripts/plugin/datepicker/WdatePicker.js") %>"></script>
<%
    Frxs.Erp.WarehouseManagementSystem.WebUI.Models.CommonQueryModel model = Model.GetCommonQueryModel();
%>
<%=model.JsScript%>
<script type="text/javascript" src="<%=Url.Content("~/Scripts/bussiness/commonQuery/commonQuery.js") %>"></script>
<input type="hidden" id="hidAdvancedSearchRows" value="<%=model.AdvancedSearchRows%>" />
<%=model.OperateControlStr %>
<div name="<%=model.PageName %>" style="display: none" class="right_contain clearfix" divType="divDialog">
    <div class="search_top">
        <div class="right_top">
        </div>
        <div class="left_top">
        </div>
    </div>
    <div class="search_box">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td class="left_box">
                </td>
                <td>
                    <form>
                    <table name="tblSearch" width="100%" border="0" cellpadding="0" cellspacing="0" class="table3">
                        <tbody>
                            <%
                                var v = model.SearchTrStr;
                                 %>
                            <%=model.SearchTrStr %>
                        </tbody>
                    </table>
                    </form>
                </td>
            </tr>
        </table>
    </div>
    <div class="search_bottom">
        <div class="right_bottom">
        </div>
        <div class="left_bottom">
        </div>
    </div>
    <%
        int j = 0;
        foreach (string tableName in model.TableNameList)
        {%>
    <div class="result2">
        <%if (!string.IsNullOrEmpty(model.TableDescList[j]))
          { %>
        <div style="font-weight: bold">
            <%=model.TableDescList[j]%></div>
        <%} %>
        <table name="<%=tableName %>" class="table1" width="100%" border="0" cellpadding="0"
            cellspacing="0">
            <thead>
                <tr style="cursor: hand">
                </tr>
            </thead>
            <tbody>
            </tbody>
            <tfoot>
                <tr>
                    <th name="pagerMain" align="center" colspan="<%=model.ShowColumnCount[j]%>" class="pages">
                        <span class="pages1">每页显示
                            <select name="pageSize" onchange="this.go()">
                                <option value="5">5</option>
                                <option value="10">10</option>
                                <option value="20" selected="selected">20</option>
                                <option value="50">50</option>
                                <option value="100">100</option>
                            </select>
                            条 </span>&nbsp; 总计<span name="totalCount" class="Total">0</span>条&nbsp; <span name="pageIndex"
                                class="current">1</span>/<span name="pageCount">1</span>页&nbsp; <span class="first ">
                                    <a name="firstPage" href="#" onclick="this.go();return false;">第一页</a></span>&nbsp;
                        <span class="prev "><a name="prevPage" href="#" onclick="this.go();return false;">上一页</a></span>&nbsp;
                        <span class="next "><a name="nextPage" href="#" onclick="this.go();return false;">下一页</a></span>&nbsp;
                        <span class="last "><a name="lastPage" href="#" onclick="this.go();return false;">最后一页</a></span>&nbsp;
                        跳转至<input name="gotoPage" type="text" class="page_input" onkeydown="this.go()" />页
                    </th>
                </tr>
            </tfoot>
        </table>
    </div>
    <%
          j++;
    } %>
    <input type="hidden" id="hidTargetName" />
</div>