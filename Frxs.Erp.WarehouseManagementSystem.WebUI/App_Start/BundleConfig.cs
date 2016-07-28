using System.Web;
using System.Web.Optimization;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            //easyui的javascript
            bundles.Add(new ScriptBundle("~/Content/easyui").Include(
                        "~/Content/easyui-1.4.3/jquery-1.7.1.js",
                        "~/Content/easyui-1.4.3/jquery.cookie.js",
                        "~/Content/easyui-1.4.3/jquery.easyui.js",
                        "~/Content/js/layout.js",
                        "~/Scripts/common/frxsCommon.js",
                        "~/Scripts/common/common.js",
                        "~/Scripts/plugin/datepicker/WdatePicker.js",
                        "~/Scripts/plugin/jquery.easyui.extend.js",
                        "~/Scripts/plugin/jquery.form.js",
                        "~/Content/easyui-1.4.3/locale/easyui-lang-zh_CN.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/css/base.css",
                        "~/Content/easyui-1.4.3/themes/icon.css"));

        }
    }
}