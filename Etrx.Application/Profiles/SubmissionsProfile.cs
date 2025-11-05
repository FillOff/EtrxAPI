using AutoMapper;
using Etrx.Domain.Dtos.Submissions;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles;

public class SubmissionsProfile : Profile
{
    public SubmissionsProfile()
    {
        CreateMap<Submission, GetUserContestProtocolResponseDto>();
    }
}
