using Decagon4Christ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decagon4Christ.Data.Repositories.Interfaces
{
    public interface IEmailRepository
    {
        Task AddEmails(List<EmailTemplate> emails);
        Task<List<EmailTemplate>> GetEmails();
        Task SaveDataFromExcelToDatabase(string filePath);
    }
}
