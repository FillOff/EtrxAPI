using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Etrx.Persistence.Interfaces;
using Etrx.Application.Interfaces;
using AutoMapper;
using Etrx.Core.Contracts.Problems;

namespace Etrx.Application.Services
{
    public class ProblemsService : IProblemsService
    {
        private readonly IProblemsRepository _problemsRepository;
        private readonly IMapper _mapper;

        public ProblemsService(
            IProblemsRepository problemsRepository,
            IMapper mapper)
        {
            _problemsRepository = problemsRepository;
            _mapper = mapper;
        }

        public async Task<List<ProblemResponseDto>> GetAllProblemsAsync()
        {
            var problems = await _problemsRepository.GetAll()
                .ToListAsync();
            var response = _mapper.Map<List<ProblemResponseDto>>(problems);

            return response;
        }

        public async Task<ProblemResponseDto?> GetProblemByContestIdAndIndexAsync(
            int contestId,
            string index)
        {
            var problem = await _problemsRepository.GetByKey(new { contestId, index });
            var response = _mapper.Map<ProblemResponseDto>(problem);

            return response;
        }

        public async Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId)
        {
            var problems = await _problemsRepository.GetByContestId(contestId)
                .ToListAsync();
            var response = _mapper.Map<List<ProblemResponseDto>>(problems);

            return response;
        }

        public ProblemWithPropsResponseDto GetProblemsByPageWithSortAndFilterTagsAsync(
            GetSortProblemRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.SortField) ||
                !typeof(Problem).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception($"Invalid field: SortField");
            }

            if (dto.Page <= 0)
            {
                throw new Exception($"Invalid field: Page");
            }

            if (dto.PageSize <= 0)
            {
                throw new Exception($"Invalid field: PageSize");
            }

            var problems = _problemsRepository.GetAll();
            string order = dto.SortOrder == true ? "asc" : "desc";

            if (dto.Tags != null)
            {
                var tagsFilter = dto.Tags.Split(';');
                problems = problems.Where(p => tagsFilter.All(tag => p.Tags!.Contains(tag)));
            }

            if (dto.Indexes != null)
            {
                var indexesFilter = dto.Indexes.Split(";");
                problems = problems.Where(p => indexesFilter.Contains(p.Index));
            }

            if (dto.ProblemName != null)
            {
                problems = problems.Where(p => p.Name.Contains(dto.ProblemName));
            }
            
            problems = problems
                .Where(p => p.Rating >= dto.MinRating && p.Rating <= dto.MaxRating)
                .Where(p => p.Points >= dto.MinPoints && p.Points <= dto.MaxPoints);

            int pageCount = problems.Count() % dto.PageSize == 0
                ? problems.Count() / dto.PageSize
                : problems.Count() / dto.PageSize + 1;

            problems = problems
                .OrderBy($"{dto.SortField} {order}")
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize);

            ProblemWithPropsResponseDto response = new ProblemWithPropsResponseDto
            (
                Problems: _mapper.Map<List<ProblemResponseDto>>(problems),
                Properties: typeof(ProblemResponseDto).GetProperties().Select(p => p.Name).ToArray()!,
                PageCount: pageCount
            );

            return response;
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            return await _problemsRepository.GetAllTags()
                .ToListAsync();
        }

        public async Task<List<string>> GetAllIndexesAsync()
        {
            return await _problemsRepository.GetAllIndexes()
                .ToListAsync();
        }

        public async Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId)
        {
            return await _problemsRepository.GetIndexesByContestId(contestId)
                .ToListAsync();
        }
    }
}
