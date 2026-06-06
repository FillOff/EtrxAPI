using Etrx.Domain.Models.ParsingModels.Dl;

namespace Etrx.Application.Interfaces.Api;

public interface IDlApiService
{
    Task<List<DlUser>> GetDlUsersAsync();
}
