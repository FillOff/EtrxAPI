using Etrx.Domain.Models.ParsingModels.Dl;

namespace Etrx.Application.Interfaces;

public interface IDlApiService
{
    Task<List<DlUser>> GetDlUsersAsync();
}