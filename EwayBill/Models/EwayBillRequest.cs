using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace EwayBill.Models
{
    public class EwayBillRequest
    {
        [Required(ErrorMessage = "SupplyType is required")]
        public string SupplyType { get; set; }

        [Required(ErrorMessage = "SubSupplyType is required")]
        public string SubSupplyType { get; set; }

        [Required(ErrorMessage = "DocType is required")]
        public string DocType { get; set; }

        [Required(ErrorMessage = "DocNo is required")]
        [StringLength(16, ErrorMessage = "DocNo should not exceed 16 characters")]
        public string DocNo { get; set; }

        [Required(ErrorMessage = "DocDate is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        public string DocDate { get; set; }

        [Required(ErrorMessage = "FromGstin is required")]
        [RegularExpression(@"^([0]{1}[1-9]{1}|[1-9]{1}[0-9]{1})[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid FromGstin format")]
        public string FromGstin { get; set; }

        [Required(ErrorMessage = "FromTrdName is required")]
        public string FromTrdName { get; set; }

        [Required(ErrorMessage = "FromAddr1 is required")]
        public string FromAddr1 { get; set; }

        [Required(ErrorMessage = "FromAddr2 is required")]
        public string FromAddr2 { get; set; }

        [Required(ErrorMessage = "FromPlace is required")]
        public string FromPlace { get; set; }

        [Required(ErrorMessage = "ActFromStateCode is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ActToStateCode must be a positive number")]
        public int ActFromStateCode { get; set; }

        [Required(ErrorMessage = "FromPincode is required")]
        [Range(100000, 999999, ErrorMessage = "Invalid FromPincode")]
        public int FromPincode { get; set; }

        [Required(ErrorMessage = "FromStateCode is required")]
        [Range(1, int.MaxValue, ErrorMessage = "FromStateCode must be a positive number")]
        public int FromStateCode { get; set; }

        [Required(ErrorMessage = "ToGstin is required")]
        [RegularExpression(@"^([0]{1}[1-9]{1}|[1-9]{1}[0-9]{1})[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid ToGstin format")]
        public string ToGstin { get; set; }

        [Required(ErrorMessage = "ToTrdName is required")]
        public string ToTrdName { get; set; }

        [Required(ErrorMessage = "ToAddr1 is required")]
        public string ToAddr1 { get; set; }

        [Required(ErrorMessage = "ToAddr2 is required")]
        public string ToAddr2 { get; set; }

        [Required(ErrorMessage = "ToPlace is required")]
        public string ToPlace { get; set; }

        [Required(ErrorMessage = "ToPincode is required")]
        [Range(100000, 999999, ErrorMessage = "Invalid ToPincode")]
        public int ToPincode { get; set; }

        [Required(ErrorMessage = "ActToStateCode is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ActToStateCode must be a positive number")]
        public int ActToStateCode { get; set; }

        [Required(ErrorMessage = "ToStateCode is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ToStateCode must be a positive number")]
        public int ToStateCode { get; set; }

        [Required(ErrorMessage = "TransactionType is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TransactionType must be a positive number")]
        public int TransactionType { get; set; }

        [Required(ErrorMessage = "DispatchFromGSTIN is required")]
        [RegularExpression(@"^([0]{1}[1-9]{1}|[1-9]{1}[0-9]{1})[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid DispatchFromGSTIN format")]
        public string DispatchFromGSTIN { get; set; }

        [Required(ErrorMessage = "DispatchFromTradeName is required")]
        public string DispatchFromTradeName { get; set; }

        [Required(ErrorMessage = "ShipToGSTIN is required")]
        [RegularExpression(@"^([0]{1}[1-9]{1}|[1-9]{1}[0-9]{1})[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid ShipToGSTIN format")]
        public string ShipToGSTIN { get; set; }

        [Required(ErrorMessage = "ShipToTradeName is required")]
        public string ShipToTradeName { get; set; }

        [Required(ErrorMessage = "TotalValue is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TotalValue must be greater than 0")]
        public int TotalValue { get; set; }

        [Required(ErrorMessage = "CgstValue is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Invalid CgstValue")]
        public decimal CgstValue { get; set; }

        [Required(ErrorMessage = "SgstValue is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Invalid SgstValue")]
        public decimal SgstValue { get; set; }

        [Required(ErrorMessage = "IgstValue is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid IgstValue")]
        public int IgstValue { get; set; }

        [Required(ErrorMessage = "CessValue is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Invalid CessValue")]
        public decimal CessValue { get; set; }

        [Required(ErrorMessage = "CessNonAdvolValue is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid CessNonAdvolValue")]
        public int CessNonAdvolValue { get; set; }

        [Required(ErrorMessage = "TotInvValue is required")]
        [Range(1, double.MaxValue, ErrorMessage = "TotInvValue must be greater than 0")]
        public decimal TotInvValue { get; set; }

        [Required(ErrorMessage = "TransMode is required")]
        public string TransMode { get; set; }

        [Required(ErrorMessage = "TransDistance is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid TransDistance")]
        public string TransDistance { get; set; }

        [Required(ErrorMessage = "TransporterId is required")]
        public string TransporterId { get; set; }

        [Required(ErrorMessage = "TransDocNo is required")]
        public string TransDocNo { get; set; }

        [Required(ErrorMessage = "VehicleNo is required")]
        public string VehicleNo { get; set; }

        [Required(ErrorMessage = "VehicleType is required")]
        public string VehicleType { get; set; }

        [Required(ErrorMessage = "ItemList is required")]
        public List<EwayBillItem> ItemList { get; set; }
        public EwayBillRequest()
        {
            ItemList = new List<EwayBillItem>();
        }

        public List<SelectListItem> DocumentTypes { get; set; }
    }

    public class EwayBillItem
    {
        [Required(ErrorMessage = "ProductName is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "ProductDesc is required")]
        public string ProductDesc { get; set; }

        [Required(ErrorMessage = "HsnCode is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid HsnCode")]
        public int HsnCode { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "QtyUnit is required")]
        public string QtyUnit { get; set; }

        [Required(ErrorMessage = "TaxableAmount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TaxableAmount must be greater than 0")]
        public int TaxableAmount { get; set; }

        [Required(ErrorMessage = "CgstRate is required")]
        [Range(1, double.MaxValue, ErrorMessage = "CgstRate must be greater than 0")]
        public decimal CgstRate { get; set; }

        [Required(ErrorMessage = "SgstRate is required")]
        [Range(1, double.MaxValue, ErrorMessage = "SgstRate must be greater than 0")]
        public decimal SgstRate { get; set; }

        [Required(ErrorMessage = "IgstRate is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid IgstRate")]
        public int IgstRate { get; set; }

        [Required(ErrorMessage = "CessRate is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid CessRate")]
        public int CessRate { get; set; }
    }

    public class EwayBillResponse
    {
        public string status_cd { get; set; }
        public string status_desc { get; set; }
        public ErrorResponse Error { get; set; }
        public EwayBillData Data { get; set; }
        public Header Header { get; set; }
        public List<EwayBillData> DataResponse { get; set; }
        public EwayBillRequest RequestData { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string ErrorCd { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
    }

    public class EwayBillData
    {
        public long ewbNo { get; set; }
        public long ewayBillNo { get; set; }
        public string EwayBillDate { get; set; }
        public string GenMode { get; set; }
        public string UserGstin { get; set; }
        public string SupplyType { get; set; }
        public string SubSupplyType { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string DocDate { get; set; }
        public string FromGstin { get; set; }
        public string FromTrdName { get; set; }
        public string FromAddr1 { get; set; }
        public string FromAddr2 { get; set; }
        public string FromPlace { get; set; }
        public int? FromPincode { get; set; }
        public int? FromStateCode { get; set; }
        public string ToGstin { get; set; }
        public string ToTrdName { get; set; }
        public string ToAddr1 { get; set; }
        public string ToAddr2 { get; set; }
        public string ToPlace { get; set; }
        public int? ToPincode { get; set; }
        public int? ToStateCode { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? TotInvValue { get; set; }
        public string TransMode { get; set; }
        public string TransDistance { get; set; }
        public decimal? CgstValue { get; set; }
        public decimal? SgstValue { get; set; }
        public decimal? IgstValue { get; set; }
        public decimal? CessValue { get; set; }
        public string TransporterId { get; set; }
        public string TransDocNo { get; set; }
        public string VehicleNo { get; set; }
        public string TransporterName { get; set; }
        public string Status { get; set; }
        public int? ActualDist { get; set; }
        public int? NoValidDays { get; set; }
        public string ValidUpto { get; set; }
        public int? ExtendedTimes { get; set; }
        public string RejectStatus { get; set; }
        public string VehicleType { get; set; }
        public int? ActFromStateCode { get; set; }
        public int? ActToStateCode { get; set; }
        public int? TransactionType { get; set; }
        public decimal? OtherValue { get; set; }
        public decimal? CessNonAdvolValue { get; set; }
        public string DispatchFromGSTIN { get; set; }
        public string DispatchFromTradeName { get; set; }
        public string ShipToGSTIN { get; set; }
        public string ShipToTradeName { get; set; }
        public List<Item> ItemList { get; set; }
        public List<Vehicle> VehiclListDetails { get; set; }
    }

    public class VehicleDetail
    {
        public string UpdMode { get; set; }
        public string FromPlace { get; set; }
        public int FromState { get; set; }
        public int TripshtNo { get; set; }
        public string UserGSTINTransin { get; set; }
        public string EnteredDate { get; set; }
        public string TransMode { get; set; }
        public string TransDocNo { get; set; }
        public string TransDocDate { get; set; }
        public string GroupNo { get; set; }
    }

    public class HeaderResponse
    {
        public string Gstin { get; set; }
        public string ClientId { get; set; }
        public string IpAddress { get; set; }
        public string ClientSecret { get; set; }
    }

    public class Item
    {
        public int ItemNo { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public int HsnCode { get; set; }
        public decimal Quantity { get; set; }
        public string QtyUnit { get; set; }
        public decimal CgstRate { get; set; }
        public decimal SgstRate { get; set; }
        public decimal IgstRate { get; set; }
        public decimal CessRate { get; set; }
        public decimal CessNonAdvol { get; set; }
        public decimal TaxableAmount { get; set; }
    }

    public class Vehicle
    {
        public string UpdMode { get; set; }
        public string VehicleNo { get; set; }
        public string FromPlace { get; set; }
        public int FromState { get; set; }
        public int TripshtNo { get; set; }
        public string UserGSTINTransin { get; set; }
        public string EnteredDate { get; set; }
        public string TransMode { get; set; }
        public string TransDocNo { get; set; }
        public string TransDocDate { get; set; }
        public string GroupNo { get; set; }
    }

    public class Header
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string gstin { get; set; }
        public string ip_address { get; set; }
    }

    public class AuthenticationResponse
    {
        public string status_cd { get; set; }
        public string status_desc { get; set; }
        public ErrorDetails Error { get; set; }
        public HeaderDetails Header { get; set; }
        public string Data { get; set; }
    }

    public class ErrorDetails
    {
        public string Message { get; set; }
        public string ErrorCd { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
    }

    public class HeaderDetails
    {
        public string Gstin { get; set; }
        public string ClientId { get; set; }
        public string IpAddress { get; set; }
        public string Username { get; set; }
        public string Txn { get; set; }
    }

    public class AuthenticationRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }

    public class EwayBillViewGenerateData
    {
        public EwayBillRequest ParentData { get; set; }
        public List<Item> ItemList { get; set; }  // Child table data

        // Constructor to initialize lists
        public EwayBillViewGenerateData()
        {
            ItemList = new List<Item>();
        }
    }

    public class DocumentType
    {
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
    }
}