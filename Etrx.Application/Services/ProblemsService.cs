using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Models;
using Etrx.Domain.Interfaces.Services;
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

        public IQueryable<Problem> GetAllProblems()
        {
            return _problemsRepository.Get();
        }

        public Problem? GetProblemByContestIdAndIndex(int contestId, string index)
        {
            return _problemsRepository.GetByContestIdAndIndex(contestId, index);
        }

        public IQueryable<Problem> GetProblemsByContestId(int contestId)
        {
            return _problemsRepository.Get().Where(p => p.ContestId == contestId);
        }

        public async Task<int> CreateProblem(Problem problem)
        {
            return await _problemsRepository.Create(problem);
        }

        public async Task<int> UpdateProblem(Problem problem)
        {
            return await _problemsRepository.Update(problem);
        }

        public async Task<int> DeleteProblem(int id)
        {
            return await _problemsRepository.Delete(id);
        }

        public List<string?>? GetAllTags()
        {
            var problems = _problemsRepository.Get().ToList();

            var tags = problems
                .SelectMany(problem => problem.Tags)
                .Distinct()
                .ToList();

            return tags;
        }

        public (IQueryable<Problem> Problems, int PageCount) GetProblemsByPageWithSortAndFilterTags(int page, int pageSize, string? tags, string sortField, bool sortOrder)
        {
            var problems = _problemsRepository.Get();

            string order = sortOrder == true ? "asc" : "desc";

            problems = problems.OrderBy($"{sortField} {order}");

            if (tags != null)
            {
                var tagsFilter = tags.Split(';');
                problems = problems.Where(p => tagsFilter.All(tag => p.Tags!.Contains(tag)));
            }

            int pageCount = problems.Count() % pageSize == 0
                ? problems.Count() / pageSize
                : problems.Count() / pageSize + 1;

            problems = problems.Skip((page - 1) * pageSize).Take(pageSize);

            return (problems, pageCount);
        }
        public string[]? GetProblemsIndexesByContestId(int contestId)
        {
            var problems = _problemsRepository.GetByContestId(contestId);

            if (problems == null) 
                return null;

            return problems
                .Select(p => p.Index)
                .ToArray();
        }
    }
}
