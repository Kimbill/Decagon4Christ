using Decagon4Christ.Data.Repositories.Interfaces;
using Decagon4Christ.Model;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decagon4Christ.Data.Repositories.Implementations
{
    internal class EmailRepository : IEmailRepository
    {
        private readonly AppDbContext _dbContext;

        public EmailRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEmails(List<EmailTemplate  > emails)
        {
            _dbContext.EmailTemplates.AddRange(emails);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<EmailTemplate>> GetEmails()
        {
            return await _dbContext.EmailTemplates.ToListAsync();
        }

        public async Task SaveDataFromExcelToDatabase(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using ExcelPackage package = new ExcelPackage(fileInfo);

            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet != null)
            {
                int rows = worksheet.Dimension.Rows;

                for (int row = 2; row <= rows; row++) // Assuming the first row contains headers
                {
                    // Replace the following logic with your actual database model and context logic
                    var email = new EmailTemplate(); // Replace Email with your actual model name
                    email.Subject = worksheet.Cells[row, 1].Value?.ToString(); // Assuming the email is in the first column
                    email.Body = worksheet.Cells[row, 2].Value?.ToString();

                    _dbContext.EmailTemplates.Add(email);
                }

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("No worksheet found in the Excel file.");
            }
        }
    }
}
