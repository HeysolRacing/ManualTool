﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using PagedList;
using InventoryTool.Models;
using System.Security.Claims;

namespace ContosoUniversity.Controllers
{
    public class RisksController : Controller
    {
        private InventoryToolContext db = new InventoryToolContext();

        //GET: Risks
        [Authorize(Roles = "RiskView")]
        public ActionResult Index()
        {
            return View(db.Risks.ToList());
        }

        [Authorize(Roles ="RiskView")]
        public ViewResult List(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Economic Group" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Fleet Number" : "";
            ViewBag.ObligorSortParm = String.IsNullOrEmpty(sortOrder) ? "Obligor" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var risks = from s in db.Risks
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                risks = risks.Where(s => s.EconomicGroup.ToString().Contains(searchString)
                                       || s.FleetNumber.ToString().Contains(searchString)
                                       || s.Obligor.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Economic Group":
                    risks = risks.OrderByDescending(s => s.EconomicGroup);
                    break;
                case "Fleet Number":
                    risks = risks.OrderBy(s => s.FleetNumber);
                    break;
                case "Obligor":
                    risks = risks.OrderBy(s => s.Obligor);
                    break;
                default:  // ID ascending 
                    risks = risks.OrderBy(s => s.EconomicGroup);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(risks.ToPagedList(pageNumber, pageSize));
        }

        // GET: Risks/Details/5
        [Authorize(Roles ="RiskView")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Risk risk = db.Risks.Find(id);
            if (risk == null)
            {
                return HttpNotFound();
            }
            return View(risk);
        }

        // GET: Risks/Details/5
        public ActionResult RiskDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var logs = from l in db.TransactionLogs select l;
            if (!String.IsNullOrEmpty(id.ToString()))
            {
                logs = logs.Where(l => l.FleetNumber.ToString().Contains(id.ToString()));
                logs = logs.OrderByDescending(s => s.Created);
            }
            return View(logs.ToList());
        }

        // GET: Risks/Create
        [Authorize(Roles ="RiskCreate")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Risks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EconomicGroup,ParentName,FleetNumber,CreditLine,Currency,ExchangeRate,Obligor,ExpirationDate,OutstandingBalance,WorkProgress,InFlight,Sum,IdEconomicGroup,IdParentName")] Risk risk)
        {
            if (ModelState.IsValid)
            {
                risk.Created = DateTime.Now;
                var userIdValue = Environment.UserName;
                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    // the principal identity is a claims identity.
                    // now we need to find the NameIdentifier claim
                    var userIdClaim = claimsIdentity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.Name);

                    if (userIdClaim != null)
                    {
                        userIdValue = userIdClaim.Value;
                    }
                }
                risk.CreatedBy = userIdValue;
                db.Risks.Add(risk);
                db.SaveChanges();
                return RedirectToAction("List");
            }

            return View(risk);
        }

        // GET: Risks/Check
        public ActionResult Check()
        {
            return RedirectToAction("Create","TransactionLogs");
        }

        // GET: Risks/Edit/5
        [Authorize(Roles ="RiskEdit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Risk risk = db.Risks.Find(id);
            if (risk == null)
            {
                return HttpNotFound();
            }
            return View(risk);
        }

        // POST: Risks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EconomicGroup,ParentName,FleetNumber,CreditLine,Currency,ExchangeRate,Obligor,ExpirationDate,OutstandingBalance,WorkProgress,InFlight,Sum,IdEconomicGroup,IdParentName")] Risk risk)
        {
            if (ModelState.IsValid)
            {
                risk.Created = DateTime.Now;
                var userIdValue = Environment.UserName;


                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    // the principal identity is a claims identity.
                    // now we need to find the NameIdentifier claim
                    var userIdClaim = claimsIdentity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.Name);

                    if (userIdClaim != null)
                    {
                        userIdValue = userIdClaim.Value;
                    }
                }
                risk.CreatedBy = userIdValue;
                db.Entry(risk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List");
            }
            return View(risk);
        }

        // GET: Risks/Delete/5
        [Authorize(Roles ="RiskDelete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Risk risk = db.Risks.Find(id);
            if (risk == null)
            {
                return HttpNotFound();
            }
            return View(risk);
        }

        // POST: Risks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Risk risk = db.Risks.Find(id);
            db.Risks.Remove(risk);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        //POST: RiskLists/ExportData
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportData()
        {
            GridView gv = new GridView();
            gv.DataSource = db.Risks.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Risklist.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
