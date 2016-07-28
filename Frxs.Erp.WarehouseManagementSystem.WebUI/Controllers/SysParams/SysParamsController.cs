/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com $time$
 * *******************************************************/
using System.Linq;
using System.Web.Mvc;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.SysParams
{
    /// <summary>
    /// 业务参数设置
    /// </summary>
    public class SysParamsController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520119)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520119)]
        public ActionResult GetList()
        {
            var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWarehouseSysParamsGetRequest()
                    {
                        WID = this.CurrentWarehouse.Parent.WarehouseId
                    });

            //调用远程数据失败
            if (null == resp || null == resp.Data)
            {
                Content("[]");
            }

            //获取数据成功 ->json
            return Content(resp.Data.ToJson());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramCode"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeButtonFiter(520119, 52011901)]
        public ActionResult Update(string paramCode)
        {

            var model = new Models.SysParamModel();

            var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWarehouseSysParamsGetRequest()
                    {
                        WID = this.CurrentWarehouse.Parent.WarehouseId,
                        ParamCode = paramCode
                    });
            if (null == resp || null == resp.Data || !resp.Data.Any())
            {
                return View(model);
            }

            //获取远程数据成功，返回远程数据
            model.ParamCode = resp.Data[0].ParamCode;
            model.ParamValue = resp.Data[0].ParamValue;
            model.ParamName = resp.Data[0].ParamName;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeButtonFiter(520119, 52011901, ContentType = ContentType.JSON, Flag = AuthorizeFlag.Content)]
        public ActionResult Update(Models.SysParamModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ErrorResult(this.GetValidateErrorMsg());
            }

            var resp = this.ErpProductSdkClient.Execute(new FrxsErpProductWarehouseSysParamsUpdateRequest()
                    {
                        WID = this.CurrentWarehouse.Parent.WarehouseId,
                        ParamCode = model.ParamCode,
                        ParamValue = model.ParamValue
                    });

            //远程调用失败
            if (null == resp)
            {
                return this.ErrorResult("调用远程接口错误，请刷新重试");
            }

            //调用成功
            return resp.Flag == 0 ? this.SuccessResult("更新参数成功") : this.ErrorResult(resp.Info);
        }
    }
}
