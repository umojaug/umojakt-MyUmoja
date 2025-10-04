namespace GrapesTl.Models;

public class Employee : EmployeeCommon
{
    public double BasicSalary { get; set; }
    public double HousingAllowance { get; set; }
    public double TransportAllowance { get; set; }
    public double LunchAllowance { get; set; }
    public double Gratuity { get; set; }
    public string Nhima { get; set; }
    public string Nrc { get; set; }
}