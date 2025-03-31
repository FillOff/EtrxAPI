namespace Etrx.Application.Interfaces;

public interface ILastUpdateTimeService
{
    DateTime GetLastUpdateTime(string tableName);
    void UpdateLastUpdateTime(string tableName, DateTime currentTime);
}