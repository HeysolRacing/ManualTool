﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using InventoryTool.Models;
using System.Security.Claims;
using System.Data.Entity.Validation;

namespace ContosoUniversity.Controllers
{
    public class TransactionLogsController : Controller
    {
        private InventoryToolContext db = new InventoryToolContext();

        // GET: TransactionLogs
        //public ActionResult Index()
        //{
        //    return View(db.TransactionLogs.ToList());
        //}

        //[HttpPost]
        [Authorize(Roles ="RiskView")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Created Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var transaction = from t in db.TransactionLogs
                              select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                transaction = transaction.Where(s => s.RequestStatus.Contains(searchString) );
            }
            switch (sortOrder)
            {
                case "name_desc":
                    transaction = transaction.OrderByDescending(s => s.RequestStatus);
                    break;
                case "Date":
                    transaction = transaction.OrderBy(s => s.Created);
                    break;
                case "date_desc":
                    transaction = transaction.OrderByDescending(s => s.Created);
                    break;
                default:  // Name ascending 
                    transaction = transaction.OrderBy(s => s.QuotationID);
                    break;
            }

            int pageSize = 200;
            int pageNumber = (page ?? 1);
            return View(transaction.ToPagedList(pageNumber, pageSize));
        }

        // GET: TransactionLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionLog transactionLog = db.TransactionLogs.Find(id);
            if (transactionLog == null)
            {
                return HttpNotFound();
            }
            return View(transactionLog);
        }

        // GET: TransactionLogs/Create
        [Authorize(Roles = "RiskCreate")]
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        // POST: TransactionLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]   
        [Authorize(Roles = "RiskCreate")]
        public ActionResult Create([Bind(Include = "ID,FleetNumber,QuotationID,CreditLineInitial,QuotationAmount")] TransactionLog transactionLog)
        {
            if (ModelState.IsValid)
            {
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
                transactionLog.CreatedBy = userIdValue;
                transactionLog.Created = DateTime.Now;
                var risk = from s in db.Risks
                            select s;
                    risk = risk.Where(s =>  s.FleetNumber.ToString().Equals(transactionLog.FleetNumber.ToString()));

                if (!risk.Any())
                {
                    return RedirectToAction("Error");
                }
                var risks = from s in db.Risks
                            select s;
                string parent = risk.ToList()[0].IdParentName.ToString();
                risks = risks.Where(s => s.IdParentName.ToString().Equals(parent));

                decimal CreditLine = risk.ToList()[0].CreditLine;
                decimal OutstandingBalance = risk.ToList()[0].OutstandingBalance;
                decimal WorkProgress = risk.ToList()[0].WorkProgress;
                decimal InFlight = risk.ToList()[0].InFlight;
                decimal sum = risk.ToList()[0].Sum;

                transactionLog.CreditLineInitial = CreditLine;
                transactionLog.OutstandingBalance = OutstandingBalance;
                transactionLog.WorkProgress = WorkProgress;
                transactionLog.InFlight = InFlight;
                transactionLog.Sum = sum;

                if (transactionLog.QuotationAmount < (CreditLine-(OutstandingBalance + InFlight + WorkProgress)) && DateTime.Now < risk.ToList()[0].ExpirationDate)
                {
                    transactionLog.RequestStatus = "approved";
                    foreach (Risk item in risks)
                    {
                        item.InFlight = item.InFlight + transactionLog.QuotationAmount;
                    }
                    TryUpdateModel(risks);
                }
                else { transactionLog.RequestStatus = "rejected"; }
               
                db.TransactionLogs.Add(transactionLog);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

                }
                    return RedirectToAction("RiskDetails","Risks", new { id = transactionLog.FleetNumber });
            }

            return View(transactionLog);
        }

        // GET: TransactionLogs/Edit/5
        [Authorize(Roles = "RiskEdit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionLog transactionLog = db.TransactionLogs.Find(id);
            if (transactionLog == null)
            {
                return HttpNotFound();
            }
            return View(transactionLog);
        }

        // POST: TransactionLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public ActionResult Edit([Bind(Include = "ID,FleetNumber,QuotationID,CreditLineInitial,QuotationAmount,CreatedBy,Created")] TransactionLog transactionLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactionLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transactionLog);
        }

        // GET: TransactionLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionLog transactionLog = db.TransactionLogs.Find(id);
            if (transactionLog == null)
            {
                return HttpNotFound();
            }
            return View(transactionLog);
        }

        // POST: TransactionLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TransactionLog transactionLog = db.TransactionLogs.Find(id);
            db.TransactionLogs.Remove(transactionLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //POST: RiskLists/ExportData
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportData()
        {
            GridView gv = new GridView();
            gv.DataSource = db.TransactionLogs.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Requestlist.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
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
