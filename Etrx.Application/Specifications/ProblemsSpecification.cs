using Etrx.Domain.Expressions;
using Etrx.Domain.Models;
using Etrx.Application.Dtos.Problems;
using LinqKit;
using System.Linq.Expressions;

namespace Etrx.Application.Specifications;

public class ProblemsSpecification : BaseSpecification<Problem>
{
    public ProblemsSpecification(GetSortProblemRequestDto dto)
    {
        var predicate = PredicateBuilder.New<Problem>(true);
        var f = dto.Filters;

        if (!string.IsNullOrWhiteSpace(dto.ProblemName))
        {
            predicate = predicate.And(p => p.ProblemTranslations.Any(
                pt => pt.LanguageCode == dto.Lang &&
                pt.Name.Contains(dto.ProblemName)));
        }

        if (f.AvailableTags != null && f.AvailableTags.Any())
        {
            if (dto.IsOnly)
            {
                predicate = predicate.And(p =>
                    p.Tags.Count == f.AvailableTags.Count() &&
                    p.Tags.All(t => f.AvailableTags.Contains(t)));
            }
            else
            {
                predicate = predicate.And(p => f.AvailableTags.All(tag => p.Tags.Contains(tag)));
            }
        }

        if (f.AvailableIndexes != null && f.AvailableIndexes.Any())
        {
            predicate = predicate.And(p => f.AvailableIndexes.Contains(p.Index));
        }

        if (f.AvailableDivisions != null && f.AvailableDivisions.Any())
        {
            predicate = predicate.And(p =>
                p.Contest != null &&
                !string.IsNullOrEmpty(p.Contest.Division) &&
                f.AvailableDivisions.Contains(p.Contest.Division)
            );
        }

        if (f.AvailableRanks != null && f.AvailableRanks.Any())
        {
            var rankPredicate = RankExpressions.GetPredicate(f.AvailableRanks.ToList());
            predicate = predicate.And(rankPredicate.Expand());
        }

        if (f.MinRating.HasValue)
            predicate = predicate.And(p => p.Rating >= f.MinRating.Value);
        if (f.MaxRating.HasValue)
            predicate = predicate.And(p => p.Rating <= f.MaxRating.Value);

        if (f.MinPoints.HasValue)
            predicate = predicate.And(p => p.Points >= f.MinPoints.Value);
        if (f.MaxPoints.HasValue)
            predicate = predicate.And(p => p.Points <= f.MaxPoints.Value);

        if (f.MinSolved.HasValue)
            predicate = predicate.And(p => p.SolvedCount >= f.MinSolved.Value);
        if (f.MaxSolved.HasValue)
            predicate = predicate.And(p => p.SolvedCount <= f.MaxSolved.Value);

        var difficultyExpr = ProblemExpressions.DifficultyExpression;

        if (f.MinDifficulty.HasValue)
            predicate = predicate.And(p => difficultyExpr.Invoke(p) >= f.MinDifficulty.Value);
        if (f.MaxDifficulty.HasValue)
            predicate = predicate.And(p => difficultyExpr.Invoke(p) <= f.MaxDifficulty.Value);

        FilterCondition = predicate;

        bool isAsc = dto.Sorting.SortOrder.ToLower() == "asc";

        switch (dto.Sorting.SortField.ToLowerInvariant())
        {
            case "name":
                if (isAsc) OrderBy = p => p.ProblemTranslations.FirstOrDefault(t => t.LanguageCode == dto.Lang)!.Name;
                else OrderByDescending = p => p.ProblemTranslations.FirstOrDefault(t => t.LanguageCode == dto.Lang)!.Name;
                break;

            case "starttime":
                if (isAsc) OrderBy = p => p.Contest.StartTime;
                else OrderByDescending = p => p.Contest.StartTime;
                break;

            case "difficulty":
                var convertedDifficultyExpr = Expression.Lambda<Func<Problem, object>>(
                    Expression.Convert(difficultyExpr.Body, typeof(object)),
                    difficultyExpr.Parameters);

                if (isAsc) OrderBy = convertedDifficultyExpr;
                else OrderByDescending = convertedDifficultyExpr;
                break;

            case "ranks":
            case "rating":
                if (isAsc) OrderBy = p => p.Rating;
                else OrderByDescending = p => p.Rating;
                break;

            case "points":
                if (isAsc) OrderBy = p => p.Points;
                else OrderByDescending = p => p.Points;
                break;

            case "solvedcount":
                if (isAsc) OrderBy = p => p.SolvedCount;
                else OrderByDescending = p => p.SolvedCount;
                break;

            case "index":
                if (isAsc) OrderBy = p => p.Index;
                else OrderByDescending = p => p.Index;
                break;

            case "contestid":
            default:
                if (isAsc) OrderBy = p => p.ContestId;
                else OrderByDescending = p => p.ContestId;
                break;
        }
    }
}