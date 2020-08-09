using Entities;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface ISituationReportsService
    {
        Task<SituationReport> GenerateReport(SituationReport situationReport);
    }
}
