using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace App1.Controllers
{
    public class QueryController : Controller
    {
        // GET: Query
        public ActionResult Index(string selectQueryString = "SELECT * FROM Workers")
        {
            BureauContext db = new BureauContext();
            var data = Query.DynamicListFromSql(db, selectQueryString, new Dictionary<string, object>());
            var grid = new WebGrid(data);
            ViewBag.Grid = grid;
            return View();
        }
    }
}