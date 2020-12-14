using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Mvc;

using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Data.Entity;
using DataTables;
using Database = DataTables.Database;

namespace ServerSidePagination.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult LoadData()
        {
            //Get Parameters
            
            //Get Start (paging start index) and length(no of records)
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Get Sort columns value
            var sortColumn = Request.Form.GetValues("columns["+Request.Form.GetValues("order[0][column]").FirstOrDefault()+"][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            //Find search columns
            var searchName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var searchEmail = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;

            using(MyDataEntities dc = new MyDataEntities())
            {
                var v = (from a in dc.Customers select a);

                //Searching
                if (!string.IsNullOrEmpty(searchName))
                {
                    v = v.Where(a => a.Name.Contains(searchName));
                }
                if(!string.IsNullOrEmpty(searchEmail))
                {
                    v = v.Where(a => a.Email.Contains(searchEmail));
                }
                //Sorting
                if(!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new {draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data}, JsonRequestBehavior.AllowGet);
            }

            


          }
        [HttpPost]
        public ActionResult DataHandlerEditor()
        {   
        }
    }
}