using Etrx.Domain.Models;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using System;

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

        public List<string> GetUserParticipantTypeList(string handle)
        {
            return _submissionsRepository
                .Get()
                .Where(s => s.Handle == handle)
                .Select(s => s.ParticipantType)
                .Distinct()
                .ToList();
        }

        public (int SolvedCount, int[] Tries) GetTriesAndSolvedCountByHandle(string handle, IQueryable<Submission> userSubmissions, string[] indexes)
        {
            int solvedCount = 0;
            int[] tries = new int[indexes.Length];
            int i = 0;

            foreach (var index in indexes)
            {
                var indexSubmissions = userSubmissions.Where(s => s.Index == index);

                int tryCount = indexSubmissions.Count();

                if (indexSubmissions.Any(s => s.Verdict == "Ok"))
                {
                    solvedCount++;
                    tries[i] = tryCount;
                }
                else
                {
                    tries[i] = -tryCount;
                }
                i++;
            }

            return (solvedCount, tries);
        }
    }
}
