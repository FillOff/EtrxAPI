using Etrx.Domain.Models.ParsingModels.Dl;

namespace Etrx.Domain.Interfaces.Services;

public interface IDlApiService
{
    Task<List<DlUser>> GetDlUsersAsync();
}