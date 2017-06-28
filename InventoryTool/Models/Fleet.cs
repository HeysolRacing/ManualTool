﻿using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryTool.Models
{
    public class Fleet
    {
        [Key]
        public int FleetID { get; set; }

        public int LogNumber { get; set; }

        public int CorpCode { get; set; }

        [Display(Name = "Fleet Number")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 1)]
        public string FleetNumber { get; set; }

        [Display(Name = "Unit Number")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 1)]
        public string UnitNumber { get; set; }

        [Display(Name = "Vin Number")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(30, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 10)]
        public string VinNumber { get; set; }

        [Display(Name = "Contract Type")]
        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(15, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 1)]
        public string ContractType { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(15, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 1)]
        public string Make { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(50, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 1)]
        public string ModelCar { get; set; }

        public int ModelYear { get; set; }

        public decimal BookValue { get; set; }

        public decimal CapCost { get; set; }

        [Display(Name = "Inservice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Inservice_date { get; set; }
        
        [Display(Name = "Inservice Process")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Inservice_process { get; set; }

        [Display(Name = "Original Inservice")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Original_Inservice { get; set; }

        [Display(Name = "Original Process")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Original_Process { get; set; }

        [Display(Name = "OffRoad")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
       // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Offroad_date { get; set; }

        [Display(Name = "OffRoad Process")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Offroad_process { get; set; }

        [Display(Name = "Sold")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Sold_date { get; set; }

        [Display(Name = "Sold Process")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Sold_process { get; set; }

        public FleetCancelUnit FleetCancelUnit { get; set; }

        public int? Amort_Term { get; set; }

        public int? Leased_Months_Billed { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
       // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> End_date { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        [StringLength(10, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 1)]
        public string ScontrNumber { get; set; }

        public decimal? Amort { get; set; }

        [Required(AllowEmptyStrings = true)]
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        public string  LicenseNumber { get; set; }

        public string State { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal Roe { get; set; }

        public string DealerName { get; set; }

        public decimal Insurance { get; set; }

        public decimal Secdep { get; set; }

        public string DepartmentCode { get; set; }

        public decimal Residual_Amount { get; set; }

        public string Level_1 { get; set; }

        public string Level_2 { get; set; }

        public string Level_3 { get; set; }

        public string Level_4 { get; set; }

        public string Level_5 { get; set; }

        public string Level_6 { get; set; }

        [Required(ErrorMessage = "You must enter {0}")]
        public string Level_Unit { get; set; }

        public string TTL { get; set; }

        public string OutletCode { get; set; }

        public string OutletName { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public Driven Driven { get; set; }

        public string Currency { get; set; }
        //public string GetStringTypeInserviceDate  En caso de querer realizar validaciones o comparativas
        //{
        //    get { return Inservice_date != null ? Inservice_date.Value.ToShortDateString() : DateTime.MinValue.ToShortDateString(); }
        //}
        [Display(Name = "Customer Reference")]
        [Required(ErrorMessage = "You must enter {0}")]
        public string CostumerReference { get; set; }
        public string ClientUnit { get; set; }
        public int VRN { get; set; }
        [Required(ErrorMessage = "You must enter {0}")]
        public string DriverName { get; set; }
        [Required(ErrorMessage = "You must enter {0}")]
        public string DriverLastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int ZIP { get; set; }
    }
}