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
            if (_submissionsRepository.Get().FirstOrDefault(s => s.Id == submission.Id) == null)
            {
                return await _submissionsRepository.Create(submission);
            }
            return 0;
        }

        public async Task<ulong> UpdateSubmission(Submission submission)
        {
            if (_submissionsRepository.GetByContestAndIndex(submission.ContestId, submission.Index) != null)
            {
                return await _submissionsRepository.Update(submission);
            }
            return 0;
        }

        public async Task<ulong> DeleteSubmission(ulong id)
        {
            return await _submissionsRepository.Delete(id);
        }
    }
}
