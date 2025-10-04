namespace GrapesTl.Controllers.IT;

[Authorize(Roles = "Super Admin,Accounts Manager,Accounts Executive")]
[Route("api/[controller]")]
[ApiController]
public class AcReportPdfController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private static byte[] TableTemplate(int numberOfColumn, string reportName, StringBuilder tableBody, string size = "A4", string dateRange = "")
    {
        var tc = new StringBuilder();

        tc.Append("<table style='width: 100%; border-collapse: collapse; font-family: Helvetica; font-size:10px;'>");
        tc.Append("<thead style='display: table-header-group;'>");
        tc.Append("<tr>");
        tc.Append($"<th colspan='{numberOfColumn}'>");
        tc.Append("<table style='width: 100%'>");
        tc.Append("<tbody>");
        tc.Append("<tr>");
        tc.Append($"<td style='text-align: left; padding-top: 10px'><img style='background-color:white; padding: 3px' src='{SD.ReportImageUrl}'/></td >");
        tc.Append($"<td style='text-align: right; font-size: 25px;padding-bottom: 10px; padding-top: 10px;'>{reportName}</td>");
        tc.Append("</tr>");
        if (string.IsNullOrWhiteSpace(dateRange) == false)
        {
            tc.Append("<tr>");
            tc.Append($"<td style='text-align: right; padding-bottom: 10px; padding-top: 10px;'>{dateRange}</td>");
            tc.Append("</tr>");
        }
        tc.Append("</tbody>");
        tc.Append("</table>");
        tc.Append("</th>");
        tc.Append("</tr>");
        tc.Append("<tr>");
        tc.Append(tableBody);
        tc.Append("</tbody>");
        tc.Append("</table>");

        var htmlToPdf = new HtmlToPdfConverter
        {
            PageHeaderHtml = "<div style='padding-top: 30px'></div>",
            PageFooterHtml = "<div class='page-footer' style='text-align: center; padding-bottom: 10px;font - family: Helvetica; font - size:10px;'>Page: <span class='page'></span></div>"
        };
        htmlToPdf.Size = size == "A4" ? PageSize.A4 : PageSize.Legal;
        htmlToPdf.Orientation = size == "A4" ? PageOrientation.Portrait : PageOrientation.Landscape;
        var pdfBytes = htmlToPdf.GeneratePdf(tc.ToString());

        return pdfBytes;
    }


    [HttpGet("New/{fromDate}/{tillDate}")]
    public async Task<IActionResult> New([FromRoute] DateTime fromDate, [FromRoute] DateTime tillDate)
    {

        try
        {
            var parameter = new DynamicParameters();
            parameter.Add("@FromDate", fromDate);
            parameter.Add("@TillDate", tillDate);

            var list = await _unitOfWork.SP_Call.List<AccountGlView>("AcDayBookSearch", parameter);
            var company = await _unitOfWork.SP_Call.OneRecord<Company>("adCompanyGetById");

            var tableBody = new StringBuilder();
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Date</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Ledger Name</th>");
            //tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Sub Ledger Name</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Particulars</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Debit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Credit Amount</th>");


            // do not delete this block of code as it is required to generate the table
            tableBody.Append("</tr></thead><tbody>");
            // end

            double totalDr = 0;
            double totalCr = 0;

            foreach (var item in list)
            {
                tableBody.Append("<tr style='border: 1px solid #000000;'>");

                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.WorkDate.ToString("dd/MMM/yyyy")}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.LedgerName}</td>");
                //tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.SubLedgerName}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.Particulars}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.Dr:N0}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.Cr:N0}</td>");
                //sb.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{(item.SellPrice):N0}</td>");

                tableBody.Append("</tr>");

                totalDr += item.Dr;
                totalCr += item.Cr;

            }

            tableBody.Append("<tr>");
            tableBody.Append($"<td colspan='3' style='border: 1px solid #000000; text-align: right; padding: 8px;'></td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalDr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalCr:N0}</td>");
            tableBody.Append("</tr>");


            var pdfBytes = TableTemplate(5, "DayBook List", tableBody);

            return File(pdfBytes, "application/pdf", "dayBook.pdf");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating PDF file: " + e.Message);
        }
    }


    [HttpGet("LedgerBook/{SearchId}/{fromDate}/{tillDate}")]
    public async Task<IActionResult> LedgerBook([FromRoute] string SearchId, [FromRoute] DateTime fromDate, [FromRoute] DateTime tillDate)
    {

        try
        {
            var parameter = new DynamicParameters();
            parameter.Add("@LedgerId", SearchId);
            parameter.Add("@FromDate", fromDate);
            parameter.Add("@TillDate", tillDate);
            var data = await _unitOfWork.SP_Call.List<AccountGlView>("AcLedgerNameGetBySearch", parameter);

            var company = await _unitOfWork.SP_Call.OneRecord<Company>("adCompanyGetById");

            var sb = new StringBuilder();


            string ledgerName = data.FirstOrDefault()?.LedgerName ?? "";
            double openingBalance = data.FirstOrDefault()?.ClosingBalance ?? 0;
            double sumDr = data.Sum(item => item.Dr);
            double sumCr = data.Sum(item => item.Cr);
            double closingBalance = openingBalance + sumDr - sumCr;


            sb.Append("<div style='font-family: Arial;'>");

            sb.Append($"<table style='width: 100%; border-collapse: collapse; font-family: Helvetica; font-size: 10px;'>");
            sb.Append($"<thead style='display: table-header-group'>");
            sb.Append($"<tr style='font-weight: bold'>");
            sb.Append($"<th colspan='1' style='text-align: left'>");
            sb.Append($"<img style='height: 100%' src='{SD.ReportImageUrl}' />");
            sb.Append($"</th>");
            sb.Append($"<th colspan='5' style='text-align: right; font-size: 16px'>");
            sb.Append($"<span>Ledger Book Report</span>");
            sb.Append($"</th>");
            sb.Append($"</tr>");

            sb.Append($"<tr style='font-weight: bold'>");
            sb.Append($"<th colspan='5' style='text-align: right'>Opening Balance</th>");
            sb.Append($"<th colspan='1' style='text-align: right'>{openingBalance:N0}</th>");
            sb.Append($"</tr>");

            sb.Append($"<tr>");
            sb.Append($"<th colspan='5' style='text-align: right'>Current Total Debit</th>");
            sb.Append($"<th colspan='1' style='text-align: right'>{sumDr:N0}</th>");
            sb.Append($"</tr>");

            sb.Append($"<tr>");
            sb.Append($"<th colspan='4' style='text-align: left; font-size: 10px'>");
            sb.Append($"<span>Account Name: Cash at Bank-Stanbic Bank USD</span>");
            sb.Append($"</th>");
            sb.Append($"<th colspan='1' style='text-align: right'>Current Total Credit</th>");
            sb.Append($"<th colspan='1' style='text-align: right'>{sumCr:N0}</th>");
            sb.Append($"</tr>");

            sb.Append($"<tr>");
            sb.Append($"<th colspan='4' style='text-align: left; font-size: 10px'>");
            sb.Append($"<span>Period: 01/Jan/2024 to 30/Sep/2024</span>");
            sb.Append($"</th>");
            sb.Append($"<th colspan='1' style='text-align: right'>Closing Balance</th>");
            sb.Append($"<th colspan='1' style='text-align: right'>{closingBalance:N0}</th>");
            sb.Append($"</tr>");

            sb.Append($"<tr>");
            sb.Append($"<th colspan='6' style='text-align: left; font-size: 10px;height:10px'>");
            sb.Append($"</th>");
            sb.Append($"</tr>");

            sb.Append($"<tr>");
            sb.Append($"<th style='width: 10%; border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Date</th>");
            sb.Append($"<th style='width: 35%; border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Particulars</th>");
            sb.Append($"<th style='width: 10%; border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Voucher No</th>");
            sb.Append($"<th style='width: 15%; border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Debit Amount</th>");
            sb.Append($"<th style='width: 15%; border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Credit Amount</th>");
            sb.Append($"<th style='width: 15%; border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Balance</th>");
            sb.Append($"</tr>");
            sb.Append($"</thead>");
            sb.Append($"<tbody>");

            foreach (var item in data)
            {
                sb.Append("<tr style='border: 1px solid #000000;'>");
                sb.Append($"<td style='border: 1px solid #000000; text-align: left; padding: 8px;'>{item.WorkDate.ToString("dd/MMM/yyyy")}</td>");
                sb.Append($"<td style='border: 1px solid #000000; text-align: left; padding: 8px;'>{item.Particulars}</td>");
                sb.Append($"<td style='border: 1px solid #000000; text-align: left; padding: 8px;'>{item.VoucherNumber}</td>");
                sb.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.Dr:N0}</td>");
                sb.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.Cr:N0}</td>");
                sb.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.ClosingBalance:N0}</td>");
                sb.Append("</tr>");
            }

            sb.Append($"<tr>");
            sb.Append($"<td colspan='6' style='text-align: left; font-size: 10px;height:10px'>");
            sb.Append($"</td>");
            sb.Append($"</tr>");

            sb.Append("<tr style='font-weight: bold'>");

            sb.Append("<td colspan='5' style='text-align:right;'>Opening Balance</td>");
            sb.Append($"<td colspan='1' style='text-align:right'>{openingBalance:N0}</td>");
            sb.Append("</tr>");

            sb.Append("<tr style='font-weight: bold'>");
            sb.Append("<td colspan='5' style='text-align:right'>Current Total Debit</td>");
            sb.Append($"<td colspan='1' style='text-align:right'>{sumDr:N0}</td>");
            sb.Append("</tr>");

            sb.Append("<tr style='font-weight: bold'>");
            sb.Append("<td colspan='5' style='text-align:right'>Current Total Credit</td>");
            sb.Append($"<td colspan='1' style='text-align:right'>{sumCr:N0}</td>");
            sb.Append("</tr>");

            sb.Append("<tr style='font-weight: bold'>");
            sb.Append("<td colspan='5' style='text-align:right'>Closing Balance</td>");
            sb.Append($"<td colspan='1' style='text-align:right'>{closingBalance:N0}</td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");

            var htmlContent = sb.ToString();

            var htmlToPdf = new HtmlToPdfConverter();
            var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);

            return File(pdfBytes, "application/pdf", "Ledger.pdf");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating PDF file: " + e.Message);
        }
    }


    [HttpGet("TrialBalance/{fromDate}/{tillDate}")]
    public async Task<IActionResult> TrialBalance([FromRoute] DateTime fromDate, [FromRoute] DateTime tillDate)
    {

        try
        {
            var parameter = new DynamicParameters();
            parameter.Add("@FromDate", fromDate);
            parameter.Add("@TillDate", tillDate);

            var list = await _unitOfWork.SP_Call.List<AccountGlView>("AcTrialBalanceSearch", parameter);

            var company = await _unitOfWork.SP_Call.OneRecord<Company>("adCompanyGetById");

            var tableBody = new StringBuilder();
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Ledger Code</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Ledger Name</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Opening Balance</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Debit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Credit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Closing Balance</th>");

            // do not delete this block of code as it is required to generate the table
            tableBody.Append("</tr></thead><tbody>");
            // end

            double openingBalance = 0;
            double totalCr = 0;
            double totalDr = 0;
            double closingBalance = 0;

            foreach (var item in list)
            {
                tableBody.Append("<tr style='border: 1px solid #000000;'>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: left; padding: 8px;'>{item.LedgerCode}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: left; padding: 8px;'>{item.LedgerName}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.OpeningBalance:N0}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.Dr:N0}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.Cr:N0}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{item.ClosingBalance:N0}</td>");
                tableBody.Append("</tr>");
            }

            openingBalance = list.Sum(a => a.OpeningBalance);
            totalDr = list.Sum(a => a.Dr);
            totalCr = list.Sum(a => a.Cr);
            closingBalance = list.Sum(a => a.ClosingBalance);

            tableBody.Append("<tr>");
            tableBody.Append($"<td colspan='1' style='border: 1px solid #000000; text-align: right; padding: 8px;'>Balance:</td>");
            tableBody.Append($"<td colspan='1' style='border: 1px solid #000000; text-align: right; padding: 8px;'></td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{openingBalance:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalDr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalCr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{closingBalance:N0}</td>");
            tableBody.Append("</tr>");

            var pdfBytes = TableTemplate(6, "Trial Balance", tableBody);

            return File(pdfBytes, "application/pdf", "trialbalance.pdf");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating PDF file: " + e.Message);
        }
    }



    [HttpGet("BankBook/{fromDate}/{tillDate}")]
    public async Task<IActionResult> BankBook([FromRoute] DateTime fromDate, [FromRoute] DateTime tillDate)
    {

        try
        {
            var parameter = new DynamicParameters();
            parameter.Add("@FromDate", fromDate);
            parameter.Add("@TillDate", tillDate);

            var list = await _unitOfWork.SP_Call.List<AccountGlView>("ActGLBankBookGetAll", parameter);

            var company = await _unitOfWork.SP_Call.OneRecord<Company>("adCompanyGetById");


            var tableBody = new StringBuilder();
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Date</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Ledger Name</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Opeing Balance</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Debit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Credit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Closing Balance</th>");

            // do not delete this block of code as it is required to generate the table
            tableBody.Append("</tr></thead><tbody>");
            // end

            double totalDr = 0;
            double totalCr = 0;

            foreach (var item in list)
            {
                tableBody.Append("<tr style='border: 1px solid #000000;'>");

                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.WorkDate.ToShortDateString()}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.LedgerName}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.OpeningBalance}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.Dr}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.Cr}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.ClosingBalance}</td>");


                tableBody.Append("</tr>");

                totalDr += item.Dr;
                totalCr += item.Cr;

            }

            tableBody.Append("<tr>");
            tableBody.Append($"<td colspan='3' style='border: 1px solid #000000; text-align: right; padding: 8px;'></td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalDr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalCr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{(totalDr - totalCr):N0}</td>");
            tableBody.Append("</tr>");

            var pdfBytes = TableTemplate(6, "BankBook", tableBody);

            return File(pdfBytes, "application/pdf", "BankBook.pdf");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating PDF file: " + e.Message);
        }
    }




    [HttpGet("CashBook/{fromDate}/{tillDate}")]
    public async Task<IActionResult> CashBook([FromRoute] DateTime fromDate, [FromRoute] DateTime tillDate)
    {

        try
        {
            var parameter = new DynamicParameters();
            parameter.Add("@FromDate", fromDate);
            parameter.Add("@TillDate", tillDate);

            var list = await _unitOfWork.SP_Call.List<AccountGlView>("ActGLBankBookGetAll", parameter);

            var company = await _unitOfWork.SP_Call.OneRecord<Company>("adCompanyGetById");


            var tableBody = new StringBuilder();
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Date</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Ledger Name</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Opening Balance</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Debit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Credit Amount</th>");
            tableBody.Append("<th style='border: 1px solid #000000; text-align: center; padding: 8px;' colspan='1'>Closing Balance</th>");

            // do not delete this block of code as it is required to generate the table
            tableBody.Append("</tr></thead><tbody>");
            // end

            double totalDr = 0;
            double totalCr = 0;

            foreach (var item in list)
            {
                tableBody.Append("<tr style='border: 1px solid #000000;'>");

                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.WorkDate.ToShortDateString()}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.LedgerName}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.OpeningBalance}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.Dr}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.Cr}</td>");
                tableBody.Append($"<td style='border: 1px solid #000000; text-align: center; padding: 8px;'>{item.ClosingBalance}</td>");


                tableBody.Append("</tr>");

                totalDr += item.Dr;
                totalCr += item.Cr;

            }

            tableBody.Append("<tr>");
            tableBody.Append($"<td colspan='2' style='border: 1px solid #000000; text-align: right; padding: 8px;'></td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalDr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{totalCr:N0}</td>");
            tableBody.Append($"<td style='border: 1px solid #000000; text-align: right; padding: 8px;'>{(totalDr - totalCr):N0}</td>");
            tableBody.Append("</tr>");

            var pdfBytes = TableTemplate(6, "CashBook", tableBody);

            return File(pdfBytes, "application/pdf", "CashBook.pdf");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating PDF file: " + e.Message);
        }
    }


}
