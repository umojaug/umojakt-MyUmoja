using System;

namespace GrapesTl.Models;

public class AccountGlView
{
    public string GlId { get; set; }
    public int TrnsId { get; set; }
    public DateTime WorkDate { get; set; }
    public string VoucherType { get; set; }
    public string VoucherNumber { get; set; }
    public string TransType { get; set; }
    public string Particulars { get; set; }
    public bool IsReverse { get; set; }
    public string LedgerCode { get; set; }
    public string LedgerName { get; set; }
    public string SubLedgerName { get; set; }
    public double Dr { get; set; }
    public double Cr { get; set; }
    //public double Balance { get; set; }
    //public double CrBalance { get; set; }
    //public double DrBalance { get; set; }
    public double OpeningBalance { get; set; }
    public double ClosingBalance { get; set; }

}
