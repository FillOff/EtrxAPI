using Etrx.Domain.Models;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;

namespace Etrx.Application.Services
{
    public class SubmissionsService : ISubmissionsService
    {
        private readonly ISubmissionsRepository _submissionsRepository;

        public SubmissionsService(ISubmissionsRepository submissionsRepository)
        {
            _submissionsRepository = submissionsRepository;
        }

        public IQueryable<Submission> GetAllSubmissions()
        {
            return _submissionsRepository.Get();
        }

        public async Task<ulong> CreateSubmission(Submission submission)
        {
            return await _submissionsRepository.Create(submission);
        }

        public async Task<ulong> UpdateSubmission(Submission submission)
        {
            return await _submissionsRepository.Update(submission);
        }

        public async Task<ulong> DeleteSubmission(ulong id)
        {
            return await _submissionsRepository.Delete(id);
        }

        public IQueryable<Submission> GetSubmissionsByContestId(int contestId)
        {
            return _submissionsRepository
                .Get()
                .Where(s => s.ContestId == contestId);
        }
    }
}
