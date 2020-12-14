using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ServerSidePagination.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

       [HttpPost]
        public ActionResult LoadData()
        {
            //Get Parameters

            //Get Start (paging start index) and length(no of records)
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            //Get Sort columns value
            string sortDirection = Request["order[0][dir]"];

            //Find search columns
            IList<Customer> customers = new List<Customer>();

            using(ServerSideTestEntities dc = new ServerSideTestEntities())
            {
                customers = dc.Customers.ToList();
                int totalrows = customers.Count;
                //Searching
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customers = customers.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) || x.City.ToLower().Contains(searchValue.ToLower()) || x.Email.ToLower().Contains(searchValue.ToLower())).ToList<Customer>();
                }

                int totalrowsafterfiltering = customers.Count;
                customers = sortDirection == "asc" ? customers.OrderBy(c => c.Name).ToList() : customers.OrderByDescending(c => c.Name).ToList();

                customers = customers.Skip(start).Take(length).ToList<Customer>();

                return Json(new { data = customers, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);
            }
          }
    }
}