
namespace Etrx.Domain.Interfaces.Services
{
    public interface ILastUpdateTimeService
    {
        DateTime GetLastUpdateTime(string tableName);
        void UpdateLastUpdateTime(string tableName, DateTime currentTime);
    }
}