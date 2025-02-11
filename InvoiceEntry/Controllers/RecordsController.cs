using InvoiceEntry.Data;
using InvoiceEntry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceEntry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecordsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get records for a selected date
        [HttpGet("{year}/{month}")]
        public async Task<IActionResult> GetRecords(int year, int month)
        {
            var records = await _context.Records
                .Where(r => r.InvoiceNo.StartsWith($"CS{year}{month:D2}"))
                .Take(10)
                .ToListAsync();

            return Ok(records);
        }

        // ✅ Create a single record (not used for bulk insert)
        [HttpPost]
        public async Task<IActionResult> CreateRecord([FromBody] Record record)
        {
            if (record == null)
                return BadRequest("Invalid data.");

            _context.Records.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Record added successfully!" });
        }

        // ✅ Create 10 records with format CSYYYYMM01 - CSYYYYMM10
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkRecords([FromBody] string selectedDate)
        {
            if (string.IsNullOrEmpty(selectedDate))
                return BadRequest("Invalid date.");

            // Parse the selected date
            DateTime date = DateTime.Parse(selectedDate, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

            // Generate InvoiceNo prefix "CSYYYYMM"
            string invoicePrefix = $"CS{date.Year}{date.Month:D2}";

            var records = new List<Record>();

            // Generate 10 records with suffix 01 - 10
            for (int i = 1; i <= 10; i++)
            {
                records.Add(new Record
                {
                    InvoiceNo = $"{invoicePrefix}{i:D2}"  // Format "CSYYYYMM01" to "CSYYYYMM10"
                });
            }

            // Save records to the database
            await _context.Records.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "10 records added successfully!" });
        }
    }
}
