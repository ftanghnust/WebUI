using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Platform.Utility.Json;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var userName = "";
            var warehouseName = "";
            if (UserIdentity != null)
            {
                userName = UserIdentity.UserName;
                
            }
            if (CurrentWarehouse!=null)
            {
                warehouseName = CurrentWarehouse.WarehouseName;
            }
            ViewBag.UserName = userName;
            ViewBag.WarehouseName = warehouseName;
            return View();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            //登出
            return Redirect(Frxs.ServiceCenter.SSO.Client.AuthorizationManager.Instance.QuitServerUrl);
        }

        /// <summary>
        /// 欢迎
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
        {
            return View();
        }

        /// <summary>
        /// 使用说明
        /// </summary>
        /// <returns></returns>
        public ActionResult Direction()
        {
            return View();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Main()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 获取左侧菜单JSON
        /// </summary>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult GetMenu()
        {
            //获取当前登录用户拥有的所有菜单权限
            var menuLists = AuthorizationManager.Instance.GetMenus().Select(o => new Node()
            {
                children = null,
                id = o.MenuID,
                text = o.MenuName,
                url = o.MenuUrl,
                parentid = o.ParentMenuID
            }).ToList();

            //保存根节点
            List<Node> roots = new List<Node>();

            //循环加载根节点的子节点集合
            foreach (var item in menuLists.Where(o => o.parentid == 0))
            {
                //当前节点信息
                var curtItem = new Node()
                {
                    id = item.id,
                    text = item.text,
                    url = item.url,
                    children = new List<Node>()
                };
                //递归出所有节点的子节点的子节点
                this.LoopToAppendChildren(menuLists, curtItem);
                //添加根节点
                roots.Add(curtItem);
            }

            //输出树表JSON格式
            return Content(roots.ToJson());
        }

        /// <summary>
        /// 递归出当前节点子节点
        /// </summary>
        /// <param name="allNode"></param>
        /// <param name="curNode"></param>
        private void LoopToAppendChildren(List<Node> allNode, Node curNode)
        {
            var subItems = allNode.Where(o => o.parentid == curNode.id).ToList();
            curNode.children = new List<Node>();
            curNode.children.AddRange(subItems);
            foreach (var subItem in subItems)
            {
                LoopToAppendChildren(allNode, subItem);
            }
        }

        /// <summary>
        /// 菜单对象
        /// </summary>
        private class Node
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string text { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int parentid { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<Node> children { get; set; }

        }
    }
}
