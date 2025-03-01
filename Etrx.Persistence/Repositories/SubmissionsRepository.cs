using Etrx.Domain.Models;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace Etrx.Persistence.Repositories
{
    public class SubmissionsRepository : ISubmissionsRepository
    {
        private readonly EtrxDbContext _context;

        public SubmissionsRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public async Task<List<Submission>> Get()
        {
            var submissions = await  _context.Submissions
                .AsNoTracking()
                .ToListAsync();

            return submissions;
        }

        public async Task<Submission?> GetById(ulong id)
        {
            var submission = await _context.Submissions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            return submission;
        }

        public async Task<List<Submission>> GetByContestId(int contestId)
        {
            var submissions = await _context.Submissions
                .AsNoTracking()
                .Where(s => s.ContestId == contestId)
                .ToListAsync();

            return submissions;
        }

        public async Task<List<string>> GetUserParticipantTypes(string handle)
        {
            var participantTypes = await _context.Submissions
                .AsNoTracking()
                .Where(s => s.Handle == handle)
                .Select(s => s.ParticipantType)
                .Distinct()
                .ToListAsync();

            return participantTypes;
        }

        public async Task<List<Submission>> GetByHandle(string handle)
        {
            var submissions = await _context.Submissions
                .AsNoTracking()
                .Where(s => s.Handle == handle)
                .ToListAsync();

            return submissions;
        }

        public async Task<ulong> Create(Submission submission)
        {
            await _context.Submissions.AddAsync(submission);
            await _context.SaveChangesAsync();

            return submission.Id;
        }

        public async Task InsertOrUpdateAsync(List<Submission> submissions)
        {
            await _context.BulkInsertOrUpdateAsync(submissions);
        }

        public async Task<ulong> Update(Submission submission)
        {
            await _context.Submissions
                .Where(s => s.Id == submission.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(s => s.ContestId, submission.ContestId)
                    .SetProperty(s => s.CreationTimeSeconds, submission.CreationTimeSeconds)
                    .SetProperty(s => s.RelativeTimeSeconds, submission.RelativeTimeSeconds)
                    .SetProperty(s => s.ParticipantType, submission.ParticipantType)
                    .SetProperty(s => s.ProgrammingLanguage, submission.ProgrammingLanguage)
                    .SetProperty(s => s.Verdict, submission.Verdict)
                    .SetProperty(s => s.Testset, submission.Testset)
                    .SetProperty(s => s.PassedTestCount, submission.PassedTestCount)
                    .SetProperty(s => s.TimeConsumedMillis, submission.TimeConsumedMillis)
                    .SetProperty(s => s.MemoryConsumedBytes, submission.MemoryConsumedBytes)
                    .SetProperty(s => s.Index, submission.Index)
                );

            return submission.Id;
        }

        public async Task<ulong> Delete(ulong id)
        {
            await _context.Submissions
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
