using Etrx.Domain.Models;
using System.Linq.Dynamic.Core;
using Etrx.Persistence.Interfaces;
using Etrx.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Etrx.Core.Contracts.Submissions;
using AutoMapper;

namespace Etrx.Application.Services
{
    public class SubmissionsService : ISubmissionsService
    {
        private readonly ISubmissionsRepository _submissionsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IProblemsRepository _problemsRepository;
        private readonly IMapper _mapper;

        public SubmissionsService(
            ISubmissionsRepository submissionsRepository,
            IUsersRepository usersRepository,
            IProblemsRepository problemsRepository,
            IMapper mapper)
        {
            _submissionsRepository = submissionsRepository;
            _usersRepository = usersRepository;
            _problemsRepository = problemsRepository;
            _mapper = mapper;
        }

        public async Task<List<SubmissionsResponseDto>> GetAllSubmissionsAsync()
        {
            var submissions =  await _submissionsRepository.GetAll()
                .ToListAsync();
            var response = _mapper.Map<List<SubmissionsResponseDto>>(submissions);

            return response;
        }

        public async Task<List<SubmissionsResponseDto>> GetSubmissionsByContestIdAsync(int contestId)
        {
            var submissions = await _submissionsRepository.GetByContestId(contestId).ToListAsync();
            var response = _mapper.Map<List<SubmissionsResponseDto>>(submissions);

            return response;
        }

        public async Task<List<string>> GetUserParticipantTypesAsync(string handle)
        {
            return await _submissionsRepository.GetUserParticipantTypes(handle).ToListAsync();
        }

        public async Task<SubmissionsWithProblemIndexesResponseDto> GetSubmissionsWithSortAsync(
            int contestId, 
            GetSubmissionRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.SortField) ||
                !typeof(SubmissionsResponseDto).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception($"Invalid field: SortField");
            }

            string order = dto.SortOrder == true ? "asc" : "desc";

            var submissions = _submissionsRepository.GetByContestId(contestId);

            var handles = submissions
                .Select(s => s.Handle)
                .Distinct()
                .ToArray();

            List<User> users = [];
            foreach ( var handle in handles)
            {
                users.Add((await _usersRepository.GetByKey(handle))!);
            }

            List<SubmissionsResponseDto> submissionsResponses = [];

            List<string>? indexes;
            if (dto.AllIndexes)
            {
                indexes = await _problemsRepository
                    .GetIndexesByContestId(contestId)
                    .ToListAsync();
            }
            else
            {
                indexes = submissions
                    .Select(s => s.Index)
                    .Distinct()
                    .ToList();
            }

            foreach (var handle in handles)
            {
                var userSubmissions = submissions.Where(s => s.Handle == handle).ToList();
                List<string> types = await GetUserParticipantTypesAsync(handle);
                foreach (var type in types)
                {
                    var typeSubmissions = userSubmissions.Where(s => s.ParticipantType == type).ToList();

                    var (solvedCount, tries) = GetTriesAndSolvedCountByHandleAsync(typeSubmissions, indexes);
                    var user = users.FirstOrDefault(u => u!.Handle.Equals(handle, StringComparison.CurrentCultureIgnoreCase))!;

                    var submissionResponse = new SubmissionsResponseDto(handle, user.FirstName, user.LastName, user.City, user.Organization,
                        user.Grade, solvedCount, type, tries);

                    submissionsResponses.Add(submissionResponse);
                }
            }

            if (dto.FilterByParticipantType != "ALL")
            {
                submissionsResponses = submissionsResponses
                    .Where(s => s.ParticipantType == dto.FilterByParticipantType)
                    .ToList();
            }

            submissionsResponses = submissionsResponses
                .AsQueryable()
                .OrderBy($"{dto.SortField} {order}")
                .ToList();

            var response = new SubmissionsWithProblemIndexesResponseDto(
                Submissions: submissionsResponses,
                ProblemIndexes: indexes,
                Properties: typeof(SubmissionsResponseDto).GetProperties().Select(p => p.Name).ToList());

            return response;
        }

        private (int SolvedCount, List<int> Tries) GetTriesAndSolvedCountByHandleAsync(
            List<Submission> userSubmissions,
            List<string> indexes)
        {
            int solvedCount = 0;
            List<int> tries = indexes
                .Select(index =>
                {
                    var submissions = userSubmissions.Where(s => s.Index == index).ToList();
                    int tryCount = submissions.Count;
                    bool isSolved = submissions.Any(s => s.Verdict == "OK");

                    if (isSolved) solvedCount++;

                    return isSolved ? tryCount : -tryCount;
                })
                .ToList();

            return (solvedCount, tries);
        }
    }
}
