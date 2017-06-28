﻿using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InventoryTool.Models;
using System.Data.Entity;
using System.Security.Claims;

namespace InventoryTool.Controllers
{
    public class ExchangeRatesController : Controller
    {
        private InventoryToolContext db = new InventoryToolContext();

        [Authorize(Roles = "ExchangeView")]
        public ActionResult Index()
        {
            return View(db.ExchangeRates.ToList());
        }

        [Authorize(Roles = "ExchangeCreate")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "ExchangeEdit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExchangeRate exchangeRate = db.ExchangeRates.Find(id);
            if (exchangeRate == null)
            {
                return HttpNotFound();
            }
            return View(exchangeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExchangeRateID,Exchange,Exchangedate,Created,CreatedBy")] ExchangeRate exchangeRate)
        {
            if (ModelState.IsValid)
            {
                var CurrentDay = DateTime.Now;
                var exchanges = db.ExchangeRates.ToList();
                var exchange = exchanges.Find(e => e.Exchangedate.ToString("yyyy-MM-dd") == CurrentDay.ToString("yyyy-MM-dd"));

                if (exchange == null)
                {
                    exchangeRate.Exchangedate = DateTime.Now;
                    exchangeRate.Created = DateTime.Now;
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
                    exchangeRate.CreatedBy = userIdValue;
                    db.ExchangeRates.Add(exchangeRate);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(exchangeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExchangeRateID,Exchange,Exchangedate,Created,CreatedBy")] ExchangeRate exchangeRate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exchangeRate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exchangeRate);
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
