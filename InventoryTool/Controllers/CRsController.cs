﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryTool.Models;
using InventoryTool.ViewModels;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Globalization;

namespace InventoryTool.Controllers
{
    public class CRsController : Controller
    {
        private InventoryToolContext db = new InventoryToolContext();

        // GET: CRs
        [Authorize(Roles = "PhantomView")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string fleetString, string unitString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "WA number" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "VIN number" : "";
            ViewBag.ObligorSortParm = String.IsNullOrEmpty(sortOrder) ? "Client name" : "";
            ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "Status" : "";
            ViewBag.FleetSortParm = String.IsNullOrEmpty(sortOrder) ? "Fleet Number" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.fleetFilter = fleetString;
            ViewBag.UnitSortParm = unitString;

            var crs = from s in db.CRs
                            select s;
            crs = crs.Where(s => !s.Status.Equals("none"));
            crs = crs.Where(s => !s.Status.Equals("Canceled"));
            crs = crs.Where(s => !s.Status.Equals("Closed"));

            if (!String.IsNullOrEmpty(searchString))
            {
                crs = crs.Where(s => s.WAnumber.ToString().Contains(searchString)
                                       || s.VINnumber.ToString().Contains(searchString)
                                       || s.Clientname.ToString().Contains(searchString)
                                       || s.Status.ToString().Contains(searchString));
            }
            if (!String.IsNullOrEmpty(fleetString))
                crs = crs.Where(s => s.FleetNumber.ToString().Contains(fleetString));
            if (!String.IsNullOrEmpty(unitString))
                crs = crs.Where(s => s.UnitNumber.ToString().Contains(unitString));

            switch (sortOrder)
            {
                case "WA number":
                    crs = crs.OrderByDescending(s => s.WAnumber);
                    break;
                case "Fleet Number":
                    crs = crs.OrderBy(s => s.FleetNumber);
                    break;
                case "VIN number":
                    crs = crs.OrderBy(s => s.VINnumber);
                    break;
                case "Client name":
                    crs = crs.OrderBy(s => s.Clientname);
                    break;
                case "Status":
                    crs = crs.OrderBy(s => s.Status);
                    break;
                default:  // ID ascending 
                    crs = crs.OrderBy(s => s.crID);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(crs.ToPagedList(pageNumber, pageSize));

        }
        // GET: CRs
        [Authorize(Roles = "APhantomView")]
        public ActionResult APlist(string sortOrder, string currentFilter, string searchString, string fleetString, string unitString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "WA number" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "VIN number" : "";
            ViewBag.ObligorSortParm = String.IsNullOrEmpty(sortOrder) ? "Client name" : "";
            ViewBag.FleetSortParm = String.IsNullOrEmpty(sortOrder) ? "Fleet Number" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.fleetFilter = fleetString;
            ViewBag.UnitSortParm = unitString;

            var crs = from s in db.CRs
                      select s;
            crs = crs.Where(s => s.Status.Equals("Approved"));
            if (!String.IsNullOrEmpty(searchString))
            {
                crs = crs.Where(s => s.WAnumber.ToString().Contains(searchString)
                                       || s.VINnumber.ToString().Contains(searchString)
                                       || s.Clientname.ToString().Contains(searchString)
                                       );
            }
            if (!String.IsNullOrEmpty(fleetString))
                crs = crs.Where(s => s.FleetNumber.ToString().Contains(fleetString));
            if (!String.IsNullOrEmpty(unitString))
                crs = crs.Where(s => s.UnitNumber.ToString().Contains(unitString));

            switch (sortOrder)
            {
                case "WA number":
                    crs = crs.OrderByDescending(s => s.WAnumber);
                    break;
                case "Fleet Number":
                    crs = crs.OrderBy(s => s.FleetNumber);
                    break;
                case "VIN number":
                    crs = crs.OrderBy(s => s.VINnumber);
                    break;
                case "Client name":
                    crs = crs.OrderBy(s => s.Clientname);
                    break;
                default:  // ID ascending 
                    crs = crs.OrderBy(s => s.crID);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(crs.ToPagedList(pageNumber, pageSize));

        }
        // GET: CRs
        [Authorize(Roles = "PhantomView")]
        public ActionResult Historic( int? page, int id, string vin, int screen)
        {
            page = 1;
            //string sortOrder, string currentFilter, string searchString,
            //ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "WA number" : "";
            //ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "VIN number" : "";
            //ViewBag.ObligorSortParm = String.IsNullOrEmpty(sortOrder) ? "Client name" : "";
            ViewBag.cr = id;
            ViewBag.screen = screen;
            ViewBag.vin = vin;

            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}

            //ViewBag.CurrentFilter = searchString;

            var crs = from s in db.CRs
                      select s;
            //if (!String.IsNullOrEmpty(searchString))
            //{
                crs = crs.Where(s => s.VINnumber.Contains(vin) && !s.Status.Equals("none")).OrderBy(s => s.crID);
            //}
            //else {
            //    crs = crs.Where(s => s.VINnumber.Contains(vin) && !s.Status.Equals("none"));
            //}
            //switch (sortOrder)
            //{
            //    case "WA number":
            //        crs = crs.OrderByDescending(s => s.WAnumber);
            //        break;
            //    case "VIN number":
            //        crs = crs.OrderBy(s => s.VINnumber);
            //        break;
            //    case "Client name":
            //        crs = crs.OrderBy(s => s.Clientname);
            //        break;
            //    default:  // ID ascending 
            //        crs = crs.OrderBy(s => s.crID);
            //        break;
            //}

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(crs.ToPagedList(pageNumber, pageSize));

        }
        // GET: CRs/Details/5
        public ActionResult Details(int? id, int? screen)
        {
            ViewBag.screen = screen;
            ViewBag.id = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var crdetail = from s in db.CRdetails
                           where s.IDCR.ToString().Contains(id.ToString())
                           select s;
            return View(crdetail.ToList());
        }
        // GET: CRs/Details/5
        [Authorize(Roles = "APhantomView")]
        public ActionResult APDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var crdetail = from s in db.CRdetails
                           where s.IDCR.ToString().Contains(id.ToString())
                           select s;
            return View(crdetail.ToList());
        }

        // GET: CRs 
        public ActionResult General(string sortOrder, string currentFilter, string searchString, DateTime? from, DateTime? to, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.WASortParm = String.IsNullOrEmpty(sortOrder) ? "WA number" : "";
            ViewBag.VINSortParm = String.IsNullOrEmpty(sortOrder) ? "VIN number" : "";
            ViewBag.ClientSortParm = String.IsNullOrEmpty(sortOrder) ? "Client name" : "";
            ViewBag.SupplierSortParm = String.IsNullOrEmpty(sortOrder) ? "Supplier" : "";
            ViewBag.AtaSortParm = String.IsNullOrEmpty(sortOrder) ? "Ata code" : "";
            ViewBag.CreatedbySortParm = String.IsNullOrEmpty(sortOrder) ? "Created By" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CurrentFrom = from;
            ViewBag.CurrentTo = to;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var crs = from a in db.CRs
                      join c in db.CRdetails on a.crID equals c.IDCR
                      join s in db.Suppliers on a.Supplier equals s.SupplierID
                      where a.Status != "none"
                      select new { a.crID, a.WAnumber, a.FleetNumber, a.UnitNumber, a.VINnumber,a.Servicedate,a.Odometer,a.Status,a.Supplier,a.Suppliername, s.StoreNumber
                      ,a.Clientname,a.Subtotal,a.IVA, a.Total, c.ID, c.IDCR, c.Quantity,c.Atacode,c.Description
                      ,c.Requested,c.Authorized,c.CreateDate,c.CreatedBy, a.OkedBy
                      ,a.Invoicenumber, a.Invoicedate,a.Amountpaid,a.Paymentdate,a.MaintenanceComments,a.ApComments, a.LegalName};

            //public int crID { get; set; }
            //public string WAnumber { get; set; }
            //public string VINnumber { get; set; }
            //public DateTime Servicedate { get; set; }
            //public int Odometer { get; set; }
            //public string Status { get; set; }
            //public int Supplier { get; set; }
            //public string Suppliername { get; set; }
            //public string Clientname { get; set; }
            //public decimal Subtotal { get; set; }
            //public decimal IVA { get; set; }
            //public decimal Total { get; set; }
            //public int ID { get; set; }
            //public int IDCR { get; set; }
            //public int Quantity { get; set; }
            //public int Atacode { get; set; }
            //public string Description { get; set; }
            //public decimal Requested { get; set; }
            //public decimal Authorized { get; set; }
            //public DateTime CreateDate { get; set; }
            //public string CreatedBy { get; set; }
            //public string OkedBy { get; set; }
            //public string Invoicenumber { get; set; }
            //public DateTime Invoicedate { get; set; }
            //public double Amountpaid { get; set; }
            //public DateTime Paymentdate { get; set; }
            //public string MaintenanceComments { get; set; }
            //public string ApComments { get; set; }

            if (!String.IsNullOrEmpty(searchString))
            {
                crs = crs.Where(s => s.WAnumber.ToString().Contains(searchString)
                                       || s.VINnumber.ToString().Contains(searchString)
                                       || s.Clientname.ToString().Contains(searchString)
                                       || s.Suppliername.ToString().Contains(searchString)
                                       || s.Atacode.ToString().Contains(searchString)
                                       || s.CreatedBy.ToString().Contains(searchString)
                                         || s.Status.ToString().Contains(searchString));
                                     
            }
            if(from != null && to !=null)
            {
                var last = to.HasValue ? to.Value.AddDays(1) : to;
                crs = crs.Where(s => s.Servicedate >= from && s.Servicedate < last);
            }
            switch (sortOrder)
            {
                case "WA number":
                    crs = crs.OrderByDescending(s => s.WAnumber);
                    break;
                case "VIN number":
                    crs = crs.OrderBy(s => s.VINnumber);
                    break;
                case "Client name":
                    crs = crs.OrderBy(s => s.Clientname);
                    break;
                case "Supplier":
                    crs = crs.OrderBy(s => s.Suppliername);
                    break;
                case "Ata code":
                    crs = crs.OrderBy(s => s.Atacode);
                    break;
                case "Created By":
                    crs = crs.OrderBy(s => s.CreatedBy);
                    break;
                case "Status":
                    crs = crs.OrderBy(s => s.Status);
                    break;
                case "Date":
                    crs = crs.OrderByDescending(s => s.Servicedate);
                    break;
                default:  // ID ascending 
                    crs = crs.OrderBy(s => s.crID);
                    break;
            }
            List<General> gral = new List<Models.General>();
            
            foreach(var item in crs)
            {
                General ag = new General();
                ag.crID = item.crID;
                ag.WAnumber = item.WAnumber;
                ag.FleetNumber = item.FleetNumber;
                ag.UnitNumber = item.UnitNumber;
                ag.VINnumber = item.VINnumber;
                ag.Servicedate = item.Servicedate;
                ag.Odometer = item.Odometer;
                ag.Status = item.Status;
                ag.Supplier = item.Supplier;
                ag.Suppliername = item.Suppliername;
                ag.Clientname = item.Clientname;
                ag.Subtotal = item.Subtotal;
                ag.IVA = item.IVA;
                ag.Total = item.Total;
                ag.ID = item.ID;
                ag.IDCR = item.IDCR;
                ag.Quantity = item.Quantity;
                ag.Atacode = item.Atacode;
                ag.Description = item.Description;
                ag.Requested = item.Requested;
                ag.Authorized = item.Authorized;
                ag.CreateDate = item.CreateDate;
                ag.CreatedBy = item.CreatedBy;
                ag.OkedBy = item.OkedBy;
                ag.Invoicenumber = item.Invoicenumber;
                ag.Invoicedate = item.Invoicedate;
                ag.Amountpaid = item.Amountpaid;
                ag.Paymentdate = item.Paymentdate;
                ag.MaintenanceComments = item.MaintenanceComments;
                ag.ApComments = item.ApComments;
                ag.storenumber = item.StoreNumber;
                ag.LegalName = item.LegalName;
                gral.Add(ag);

            }

            //gral = (General)crs.ToList();

            int pageSize = 200;
            int pageNumber = (page ?? 1);
            return View(gral.ToPagedList(pageNumber, pageSize));

        }

        // GET: CRs/ClosedReport
        public ActionResult ClosedReport()
        {
            var crs = from a in db.CRs
                      where a.ClosedReport == "False" 
                      select a;
            crs = crs.Where(s => s.Status.Equals("Closed"));

         List <Closed> closed = new List<Models.Closed>();

            foreach (var item in crs)
            {
                Closed ag = new Closed();
                ag.SupplierCode = item.Supplier;
                if (item.Invoicenumber.Length > 2)
                {
                    ag.Serial = item.Invoicenumber.Substring(0, 2);
                    ag.Invoice = item.Invoicenumber.Substring(2);
                }
                ag.InvoiceDate = item.Invoicedate;
                ag.DueDate = item.Invoicedate;
                ag.Total = item.Total;
                ag.Currency = 1;
                ag.ExchangeRate = 1;
                ag.FleetUnit = item.FleetNumber + "_" + item.UnitNumber;
                ag.Description = "Maintenance";
                ag.CRnumber = item.WAnumber;
                ag.Docto = "99";
                ag.Related = "99";
                ag.SerialDocRelated = ag.Invoice;
                ag.DocRelated = ag.Invoice;

                closed.Add(ag);
            }

            return View(closed.ToList());

        }

        public ActionResult CRsClosed()
        {
            //var crs = from a in db.CRs
            //          where a.ClosedReport == "False"
            //          select new
            //          {
            //              a.Invoicenumber,
            //              a.Supplier,
            //              a.Invoicedate,
            //              a.Subtotal,
            //              a.WAnumber,
            //              a.Status
            //          };
            //crs = crs.Where(s => s.Status.Equals("Closed"));
            var crs = from a in db.CRs
                      where a.ClosedReport == "False"
                      select new
                      {
                          a.Invoicenumber,
                          a.Supplier,
                          a.Invoicedate,
                          a.Subtotal,
                          a.WAnumber,
                          a.Status
                      };
            crs = crs.Where(s => s.Status.Equals("Closed"));
            List<CRsClosed> CRclosed = new List<Models.CRsClosed>();
            foreach (var item in crs)
            {
                CRsClosed ag = new CRsClosed();
                ag.Invoicenumber = item.Invoicenumber;
                ag.Supplier = item.Supplier;
                ag.Invoicedate = item.Invoicedate;
                ag.NotAplicable = 0;
                ag.CurrencyNumber = 1;
                ag.ExchangeRate = 1;
                ag.Concept = "Maintenance";
                ag.NotAplicable2 = "";
                ag.NotAplicable3 = "";
                ag.DeliverDate = "";
                ag.ExpirationDate = "";
                ag.Subtotal = item.Subtotal;
                ag.NotAplicable4 = 0;
                ag.NotAplicable5 = 0;
                ag.NotAplicable6 = 0;
                ag.NotAplicable7 = 0;
                ag.TaxScheme = 1;
                ag.ItemCode = 7;
                ag.NotAplicable8 = 0;
                ag.IEPS = 0;
                ag.Tax2 = 0;
                ag.Tax3 = 0;
                ag.IVA = 16;
                ag.CRNumber = item.WAnumber;
                CRclosed.Add(ag);
            }
            return View(CRclosed);
        }

        public ActionResult ApMaintenance()
        {
            var crs = from c in db.CRs
                      join h in db.CRs_hs on c.crID equals h.crID
                      join f in db.Fleets on c.VINnumber equals f.VinNumber
                      select new
                      {
                          c.WAnumber,
                          c.VINnumber,
                          f.LogNumber,
                          c.Status,
                          hStatus = h.Status,
                          h.Modified,
                          c.Supplier,
                          c.Suppliername,
                          c.Subtotal,
                          c.IVA,
                          c.Total,
                          c.Invoicenumber,
                          c.Invoicedate,
                          c.FleetNumber,
                          c.UnitNumber,
                      };
            crs = crs.Where(s => s.Status.Equals("Closed"));
            crs = crs.Where(s => s.hStatus.Equals("Closed"));
            crs = crs.Where(s => !s.Suppliername.Equals("Treka"));

            DateTime time = DateTime.Now;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            // Return the week of our adjusted day
            int week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            List<APMaintenance> aPMaintenance = new List<Models.APMaintenance>();
            foreach (var item in crs)
            {
                APMaintenance ag = new APMaintenance();
                ag.JournalSource = "payables";
                ag.CorpCode = 501;
                ag.FleetNumber = item.FleetNumber;
                ag.UnitNumber = item.UnitNumber;
                ag.ProcessDate = item.Modified.ToString("MMM-yy");
                ag.CRNumber = item.WAnumber;
                ag.PartyName = item.Supplier.ToString() + item.Suppliername;
                ag.TransactionDate = item.Invoicedate;
                ag.CurrencyCode = "MXN";
                ag.TCInvoice = 1;
                ag.TCPayment = 1;
                ag.ExternalDocLocator = item.Invoicenumber;
                ag.NET = item.Subtotal;
                ag.charge = true;
                ag.InternalDocumentLocator = "Pha" + DateTime.Now.Year.ToString() + week.ToString();
                ag.DAN = item.LogNumber;
                ag.EnteredDrSUM = item.Total;
                ag.EnteredCrSUM = 0;
                ag.NET = item.Total;
                ag.AccountedDrSUM = item.Total;
                ag.AccountedCrSUM = 0;
                ag.JournalCategory = "Maintenance";
                ag.Bankaccount = "NA";
                aPMaintenance.Add(ag);
            }
            return View(aPMaintenance.Distinct());
        }

        public ActionResult ArMaintenance()
        {
            var crs = from c in db.CRs
                      join h in db.CRs_hs on c.crID equals h.crID
                      join f in db.Fleets on c.VINnumber equals f.VinNumber
                      select new
                      {
                          c.WAnumber,
                          c.VINnumber,
                          f.LogNumber,
                          c.Status,
                          hStatus = h.Status,
                          h.Modified,
                          c.Supplier,
                          c.Suppliername,
                          c.Subtotal,
                          c.IVA,
                          c.Total,
                          c.Invoicenumber,
                          c.Invoicedate,
                          c.FleetNumber,
                          c.UnitNumber,
                      };
            crs = crs.Where(s => s.Status.Equals("Closed"));
            crs = crs.Where(s => s.hStatus.Equals("Closed"));
            crs = crs.Where(s => !s.Suppliername.Equals("Treka"));

            DateTime time = DateTime.Now;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            // Return the week of our adjusted day
            int week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            List<APMaintenance> aPMaintenance = new List<Models.APMaintenance>();
            foreach (var item in crs)
            {
                APMaintenance ag = new APMaintenance();
                ag.JournalSource = "recivables";
                ag.CorpCode = 501;
                ag.FleetNumber = item.FleetNumber;
                ag.UnitNumber = item.UnitNumber;
                ag.ProcessDate = item.Modified.ToString("MMM-yy");
                ag.CRNumber = item.WAnumber;
                ag.PartyName = item.Supplier.ToString() + item.Suppliername;
                ag.TransactionDate = item.Invoicedate;
                ag.CurrencyCode = "MXN";
                ag.TCInvoice = 1;
                ag.TCPayment = 1;
                ag.ExternalDocLocator = item.Invoicenumber;
                ag.NET = item.Subtotal;
                ag.charge = true;
                ag.InternalDocumentLocator = "Pha" + DateTime.Now.Year.ToString() + week.ToString();
                ag.DAN = item.LogNumber;
                ag.EnteredDrSUM = 0;
                ag.EnteredCrSUM = item.Total;
                ag.NET = item.Total;
                ag.AccountedDrSUM = 0;
                ag.AccountedCrSUM = item.Total;
                aPMaintenance.Add(ag);
            }
            return View(aPMaintenance.Distinct());
        }

        // GET: CRs/Create
        [Authorize(Roles = "PhantomCreate")]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CR cR = db.CRs.Find(id);

            newCR request = new newCR();
            request.cr = cR;
            request.crdetail = new CRdetail();

            var acodes = db.Atacodes.ToList();
            foreach (Acode item in acodes)
            {
                item.Description = item.code.ToString() + "-" + item.Description;
            }

            ViewBag.Listcodes = acodes;

            var crdetails = from s in db.CRdetails
                            select s;
            crdetails = crdetails.Where(s => s.IDCR.ToString().Contains(cR.crID.ToString()));

            ViewData["CRdetails"] = crdetails.ToList();

            var crs = from s in db.CRs
                      select s;
            crs = crs.Where(s => s.VINnumber.ToString().Contains(cR.VINnumber.ToString()) && s.Odometer > 0);
            crs = crs.OrderByDescending(s => s.crID);
            var crshistoric = new CR();
            if (crs.Count() >0)
            { crshistoric = crs.First(); }
           
            ViewData["CRhistoric"] = crshistoric;


            Ssupplier supplier = db.Suppliers.Find(cR.Supplier);
            if (supplier == null)
            { supplier = new Ssupplier(); }
            ViewData["supplier"] = supplier;

            if (cR == null)
            {
                return HttpNotFound();
            }

            return View(request);
        }

        // POST: CRs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PhantomCreate")]
        public ActionResult Create(CR cR)
        {
            if (ModelState.IsValid)
            {
                var crdetail = from s in db.CRdetails
                               where s.IDCR.ToString().Contains(cR.crID.ToString())
                               select s;
                decimal subtotal = 0.0m; 
                foreach(CRdetail item in crdetail)
                {
                    subtotal = subtotal + item.Authorized;
                }
                cR.Status = "Pending Aproval";
                cR.ClosedReport = "False";
                cR.Subtotal = subtotal;
                cR.IVA = subtotal * 0.16m;
                cR.Total = subtotal * 1.16m;
                cR.ModifiedDate = DateTime.Now;
                //db.CRs.Add(cR);
                db.Entry(cR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit",new { id=cR.crID});
            }

            return View(cR);
        }

        // GET: CRs/Edit/5
        [Authorize(Roles = "PhantomEdit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CR cR = db.CRs.Find(id);

            newCR request = new newCR();
            request.cr = cR;
            request.crdetail = new CRdetail();
          
            var acodes = db.Atacodes.ToList();
            foreach(Acode item in acodes)
            {
                item.Description = item.code.ToString() + "-" + item.Description;
            }

            ViewBag.Listcodes = acodes;

            var crdetails = from s in db.CRdetails
                            select s;
            crdetails = crdetails.Where(s => s.IDCR.ToString().Contains(cR.crID.ToString()));

            ViewData["CRdetails"] = crdetails.ToList();

            var crs = from s in db.CRs
                        select s;
            crs = crs.Where(s => s.crID.ToString().Contains(cR.crID.ToString()));
            crs = crs.OrderByDescending(s => s.crID);
            var crshistoric = crs.First();
            if (crshistoric == null)
            { crshistoric = new CR(); }
            ViewData["CRhistoric"] = crshistoric;

            
             Ssupplier supplier = db.Suppliers.Find(cR.Supplier);
            if (supplier == null)
            { supplier = new Ssupplier(); }
            ViewData["supplier"] = supplier;

            if (cR == null)
            {
                return HttpNotFound();
            }

            return View(request);
        }

        // POST: CRs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PhantomEdit")]
        public ActionResult Edit(CR cR)
        {
            if (ModelState.IsValid)
            {
                var crdetail = from s in db.CRdetails
                               where s.IDCR.ToString().Contains(cR.crID.ToString())
                               select s;
                decimal subtotal = 0.0m;
                foreach (CRdetail item in crdetail)
                {
                    subtotal = subtotal + item.Authorized;
                }
                cR.Status = "Pending Aproval";
                cR.Subtotal = subtotal;
                cR.IVA = subtotal * 0.16m;
                cR.Total = subtotal * 1.16m;
                cR.ModifiedDate = DateTime.Now;
                db.Entry(cR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = cR.crID });
            }
            return View(cR);
        }

        public ActionResult Supplier(int? id, int screen)
        {
            return RedirectToAction("Index", "Ssuppliers", new { id = id, screen = screen });
        }
        // GET: CRs/Delete/5
        [Authorize(Roles = "PhantomEdit")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CR cR = db.CRs.Find(id);
            if (cR == null)
            {
                return HttpNotFound();
            }
            return View(cR);
        }

        // POST: CRs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PhantomEdit")]
        public ActionResult DeleteConfirmed(int id, string comments)
        {
            CR cR = db.CRs.Find(id);
            cR.MaintenanceComments = comments;
            cR.Status = "Canceled";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CRdetails/DeleteCode/5
        public ActionResult DeleteCode(int? id, int? cr, int? screen)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CRdetail cRdetail = db.CRdetails.Find(id);
            
            if (cRdetail == null)
            {
                return HttpNotFound();
            }
            db.CRdetails.Remove(cRdetail);
            db.SaveChanges();
            if (screen == 2)
            { return RedirectToAction("Edit", "CRs", new { id = cr }); }
            else { return RedirectToAction("Create", "CRs", new { id = cr}); }
        }

        // GET: CRs/Approve/5
        [Authorize(Roles = "PhantomEdit")]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CR cR = db.CRs.Find(id);
            if (cR == null)
            {
                return HttpNotFound();
            }
            return View(cR);
        }

        [Authorize(Roles = "PhantomView")]
        public ActionResult Mail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CR cR = db.CRs.Find(id);
            var crdetails = from s in db.CRdetails
                            select s;
            crdetails = crdetails.Where(s => s.IDCR.ToString().Contains(cR.crID.ToString()));

            ViewData["CRdetails"] = crdetails.ToList();
            if (cR == null)
            {
                return HttpNotFound();
            }
            return View(cR);
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PhantomEdit")]
        public ActionResult ApproveConfirmed(CR cR)
        {
            cR.Status = "Approved";
            cR.ModifiedDate = DateTime.Now;
            db.Entry(cR).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Mail",new { id = cR.crID });
        }

        // GET: CRs/Close/5
        [Authorize(Roles = "APhantomView")]
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CR cR = db.CRs.Find(id);
            if (cR == null)
            {
                return HttpNotFound();
            }
            return View(cR);
        }

        [Authorize(Roles = "APhantomEdit")]
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed(CR cR)
        {
            cR.Status = "Closed";
            cR.ModifiedDate = DateTime.Now;
            db.Entry(cR).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("APlist");
        }

        //POST: CRs/ExportData
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportData()
        {
            GridView gv = new GridView();
            var crs = from a in db.CRs
                      join c in db.CRdetails on a.crID equals c.IDCR
                      join s in db.Suppliers on a.Supplier equals s.SupplierID
                      where a.Status != "none"
                      select new
                      {
                          a.crID,
                          a.WAnumber,
                          a.VINnumber,
                          a.FleetNumber,
                          a.UnitNumber,
                          a.Servicedate,
                          a.Odometer,
                          a.Status,
                          a.Supplier,
                          a.Suppliername,
                          s.StoreNumber,
                          a.LegalName
                      ,
                          a.Clientname,
                          a.Subtotal,
                          a.IVA,
                          a.Total,
                          c.ID,
                          c.IDCR,
                          c.Quantity,
                          c.Atacode,
                          c.Description
                      ,
                          c.Requested,
                          c.Authorized,
                          c.CreateDate,
                          c.CreatedBy,
                          a.OkedBy
                      ,
                          a.Invoicenumber,
                          a.Amountpaid,
                          a.Paymentdate,
                          a.MaintenanceComments,
                          a.ApComments

                      };
            gv.DataSource = crs.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=CrsList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("General");
        }

        public ActionResult ExportClosedCRs()
        {
            GridView gv = new GridView();
            var crs = from a in db.CRs
                      where a.ClosedReport == "False"
                      select new
                      {
                          a.Invoicenumber,
                          a.Supplier,
                          a.Invoicedate,
                          a.Subtotal,
                          a.WAnumber,
                          a.Status

                      };
            crs = crs.Where(s => s.Status.Equals("Closed"));
            List<CRsClosed> CRclosed = new List<Models.CRsClosed>();
            foreach (var item in crs)
            {
                CRsClosed ag = new CRsClosed();
                ag.Invoicenumber = item.Invoicenumber;
                ag.Supplier = item.Supplier;
                ag.Invoicedate = item.Invoicedate;
                ag.NotAplicable = 0;
                ag.CurrencyNumber = 1;
                ag.ExchangeRate = 1;
                ag.Concept = "Maintenance";
                ag.NotAplicable2 = "";
                ag.NotAplicable3 = "";
                ag.DeliverDate = "";
                ag.ExpirationDate = "";
                ag.Subtotal = item.Subtotal;
                ag.NotAplicable4 = 0;
                ag.NotAplicable5 = 0;
                ag.NotAplicable6 = 0;
                ag.NotAplicable7 = 0;
                ag.TaxScheme = 1;
                ag.ItemCode = 7;
                ag.NotAplicable8 = 0;
                ag.IEPS = 0;
                ag.Tax2 = 0;
                ag.Tax3 = 0;
                ag.IVA = 16;
                ag.CRNumber = item.WAnumber;
                CRclosed.Add(ag);
                //db.Entry(item).State = EntityState.Modified;
            }
            gv.DataSource = CRclosed;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=CRsClosedList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            //db.SaveChanges();

            return RedirectToAction("CRsClosed");
        }

        public ActionResult ExportApMaintenanceReport()
        {
            GridView gv = new GridView();
            var crs = from c in db.CRs
                      join h in db.CRs_hs on c.crID equals h.crID
                      join f in db.Fleets on c.VINnumber equals f.VinNumber
                      select new
                      {
                          c.WAnumber,
                          c.VINnumber,
                          f.LogNumber,
                          c.Status,
                          hStatus = h.Status,
                          h.Modified,
                          c.Supplier,
                          c.Suppliername,
                          c.Subtotal,
                          c.IVA,
                          c.Total,
                          c.Invoicenumber,
                          c.Invoicedate,
                          c.FleetNumber,
                          c.UnitNumber,
                      };
            crs = crs.Where(s => s.Status.Equals("Closed"));
            crs = crs.Where(s => s.hStatus.Equals("Closed"));
            crs = crs.Where(s => !s.Suppliername.Equals("Treka"));

            DateTime time = DateTime.Now;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            // Return the week of our adjusted day
            int week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            List<APMaintenance> aPMaintenance = new List<Models.APMaintenance>();
            foreach (var item in crs)
            {
                APMaintenance ag = new APMaintenance();
                ag.JournalSource = "payables";
                ag.CorpCode = 501;
                ag.FleetNumber = item.FleetNumber;
                ag.UnitNumber = item.UnitNumber;
                ag.ProcessDate = item.Modified.ToString("MMM-yy");
                ag.CRNumber = item.WAnumber;
                ag.PartyName = item.Supplier.ToString() + item.Suppliername;
                ag.TransactionDate = item.Invoicedate;
                ag.CurrencyCode = "MXN";
                ag.TCInvoice = 1;
                ag.TCPayment = 1;
                ag.ExternalDocLocator = item.Invoicenumber;
                ag.NET = item.Subtotal;
                ag.charge = true;
                ag.InternalDocumentLocator = "Pha" + DateTime.Now.Year.ToString() + week.ToString();
                ag.DAN = item.LogNumber;
                ag.EnteredDrSUM = item.Total;
                ag.EnteredCrSUM = 0;
                ag.NET = item.Total;
                ag.AccountedDrSUM = item.Total;
                ag.AccountedCrSUM = 0;
                ag.JournalCategory = "Maintenance";
                ag.Bankaccount = "NA";
                aPMaintenance.Add(ag);
            }

            gv.DataSource = aPMaintenance.Distinct();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=APmaintenance.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            db.SaveChanges();

            return RedirectToAction("ApMaintenance");
        }

        public ActionResult ExportArMaintenanceReport()
        {
            GridView gv = new GridView();
            var crs = from c in db.CRs
                      join h in db.CRs_hs on c.crID equals h.crID
                      join f in db.Fleets on c.VINnumber equals f.VinNumber
                      select new
                      {
                          c.WAnumber,
                          c.VINnumber,
                          f.LogNumber,
                          c.Status,
                          hStatus = h.Status,
                          h.Modified,
                          c.Supplier,
                          c.Suppliername,
                          c.Subtotal,
                          c.IVA,
                          c.Total,
                          c.Invoicenumber,
                          c.Invoicedate,
                          c.FleetNumber,
                          c.UnitNumber,
                      };
            crs = crs.Where(s => s.Status.Equals("Closed"));
            crs = crs.Where(s => s.hStatus.Equals("Closed"));
            crs = crs.Where(s => !s.Suppliername.Equals("Treka"));

            DateTime time = DateTime.Now;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            // Return the week of our adjusted day
            int week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            List<APMaintenance> aPMaintenance = new List<Models.APMaintenance>();
            foreach (var item in crs)
            {
                APMaintenance ag = new APMaintenance();
                ag.JournalSource = "payables";
                ag.CorpCode = 501;
                ag.FleetNumber = item.FleetNumber;
                ag.UnitNumber = item.UnitNumber;
                ag.ProcessDate = item.Modified.ToString("MMM-yy");
                ag.CRNumber = item.WAnumber;
                ag.PartyName = item.Supplier.ToString() + item.Suppliername;
                ag.TransactionDate = item.Invoicedate;
                ag.CurrencyCode = "MXN";
                ag.TCInvoice = 1;
                ag.TCPayment = 1;
                ag.ExternalDocLocator = item.Invoicenumber;
                ag.NET = item.Subtotal;
                ag.charge = true;
                ag.InternalDocumentLocator = "Pha" + DateTime.Now.Year.ToString() + week.ToString();
                ag.DAN = item.LogNumber;
                ag.EnteredDrSUM = 0;
                ag.EnteredCrSUM = item.Total;
                ag.NET = item.Total;
                ag.AccountedDrSUM = 0;
                ag.AccountedCrSUM = item.Total;
                aPMaintenance.Add(ag);
            }

            gv.DataSource = aPMaintenance.Distinct();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ARmaintenance.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            db.SaveChanges();

            return RedirectToAction("ApMaintenance");
        }

        //POST: CRs/ExportData
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportClosed()
        {
            GridView gv = new GridView();

            var crs = from a in db.CRs
                      where a.ClosedReport == "False"
                      select a;
            crs = crs.Where(s => s.Status.Equals("Closed"));

            List<Closed> closed = new List<Models.Closed>();

            foreach (var item in crs)
            {
                Closed ag = new Closed();
                ag.SupplierCode = item.Supplier;
                if (item.Invoicenumber.Length > 2)
                {
                    ag.Serial = item.Invoicenumber.Substring(0, 2);
                    ag.Invoice = item.Invoicenumber.Substring(2);
                }
                ag.InvoiceDate = item.Invoicedate;
                ag.DueDate = item.Invoicedate;
                ag.Total = item.Total;
                ag.Currency = 1;
                ag.ExchangeRate = 1;
                ag.FleetUnit = item.FleetNumber + "_" + item.UnitNumber;
                ag.Description = "Maintenance";
                ag.CRnumber = item.WAnumber;
                ag.Docto = "99";
                ag.Related = "99";
                ag.SerialDocRelated = ag.Invoice;
                ag.DocRelated = ag.Invoice;
                closed.Add(ag);

                item.ClosedReport = "True";
                db.Entry(item).State = EntityState.Modified;
            }

            gv.DataSource = closed;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=CRsClosedList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

           
            db.SaveChanges();

            return RedirectToAction("Closed");
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
