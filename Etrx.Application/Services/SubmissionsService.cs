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

        public async Task<List<Submission>> GetAllSubmissionsAsync()
        {
            return await _submissionsRepository.Get();
        }

        public async Task<List<Submission>> GetSubmissionsByContestIdAsync(int contestId)
        {
            return await _submissionsRepository.GetByContestId(contestId);
        }

        public async Task<List<string>> GetUserParticipantTypesAsync(string handle)
        {
            return await _submissionsRepository.GetUserParticipantTypes(handle);
        }

        public (int SolvedCount, List<int> Tries) GetTriesAndSolvedCountByHandleAsync(
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

        public async Task<ulong> CreateSubmissionAsync(Submission submission)
        {
            return await _submissionsRepository.Create(submission);
        }

        public async Task<ulong> UpdateSubmissionAsync(Submission submission)
        {
            return await _submissionsRepository.Update(submission);
        }

        public async Task<ulong> DeleteSubmissionAsync(ulong id)
        {
            return await _submissionsRepository.Delete(id);
        }
    }
}
