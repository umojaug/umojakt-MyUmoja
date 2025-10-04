using Newtonsoft.Json;
using System.Collections.Generic;

namespace GrapesTl.Models;

public record AcJournals
{
    [JsonProperty("journals")]
    public List<Journal> Journals { get; init; }
}

public record Journal
{
    //[JsonProperty("trId")]
    //public string TrId { get; set; }
    [JsonProperty("ledgerNameCode")]
    public string LedgerNameCode { get; set; }
    [JsonProperty("particulars")]
    public string Particulars { get; set; }
    [JsonProperty("dr")]
    public double Dr { get; set; }
    [JsonProperty("cr")]
    public double Cr { get; set; }
}
