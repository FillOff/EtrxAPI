using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Models;
using Etrx.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Application.Services
{
    public class ProblemsService : IProblemsService
    {
        private readonly IProblemsRepository _problemsRepository;

        public ProblemsService(IProblemsRepository problemsRepository)
        {
            _problemsRepository = problemsRepository;
        }

        public async Task<List<Problem>> GetAllProblemsAsync()
        {
            return await _problemsRepository.Get();
        }

        public async Task<Problem?> GetProblemByContestIdAndIndexAsync(
            int contestId, 
            string index)
        {
            return await _problemsRepository.GetByContestIdAndIndex(contestId, index);
        }

        public async Task<List<Problem>> GetProblemsByContestIdAsync(int contestId)
        {
            return await _problemsRepository.GetByContestId(contestId);
        }

        public async Task<(List<Problem> Problems, int PageCount)> GetProblemsByPageWithSortAndFilterTagsAsync(
            int page,
            int pageSize,
            string? tags,
            string? indexes,
            string? problemName,
            string sortField,
            bool sortOrder,
            int minRating,
            int maxRating,
            double minPoints,
            double maxPoints)
        {
            var problems = (await _problemsRepository.Get()).AsQueryable();
            string order = sortOrder == true ? "asc" : "desc";

            if (tags != null)
            {
                var tagsFilter = tags.Split(';');
                problems = problems.Where(p => tagsFilter.All(tag => p.Tags!.Contains(tag)));
            }

            if (indexes != null)
            {
                var indexesFilter = indexes.Split(";");
                problems = problems.Where(p => indexesFilter.Contains(p.Index));
            }

            if (problemName != null)
            {
                problems = problems.Where(p => p.Name.Contains(problemName));
            }

            problems = problems
                .Where(p => p.Rating >= minRating && p.Rating <= maxRating)
                .Where(p => p.Points >= minPoints && p.Points <= maxPoints);

            int pageCount = problems.Count() % pageSize == 0
                ? problems.Count() / pageSize
                : problems.Count() / pageSize + 1;

            problems = problems
                .OrderBy($"{sortField} {order}")
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return (problems.ToList(), pageCount);
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            return await _problemsRepository.GetAllTags();
        }

        public async Task<List<string>> GetAllIndexesAsync()
        {
            return await _problemsRepository.GetAllIndexes();
        }

        public async Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId)
        {
            return await _problemsRepository.GetIndexesByContestId(contestId);
        }

        public async Task<int> CreateProblemAsync(Problem problem)
        {
            return await _problemsRepository.Create(problem);
        }

        public async Task<int> UpdateProblemAsync(Problem problem)
        {
            return await _problemsRepository.Update(problem);
        }

        public async Task<int> DeleteProblemAsync(int id)
        {
            return await _problemsRepository.Delete(id);
        }
    }
}
