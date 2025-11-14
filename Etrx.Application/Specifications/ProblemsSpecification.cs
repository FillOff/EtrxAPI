using Etrx.Application.Queries;
using Etrx.Domain.Models;
using LinqKit;
using System.Linq.Expressions;

namespace Etrx.Application.Specifications;

public class ProblemsSpecification : BaseSpecification<Problem>
{
    public ProblemsSpecification(ProblemQueryParameters parameters)
    {
        var predicate = PredicateBuilder.New<Problem>(true);

        // Filtering

        if (!string.IsNullOrEmpty(parameters.Tags))
        {
            var tagsFilter = parameters.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (tagsFilter.Length > 0)
            {
                if (parameters.IsOnly)
                {
                    predicate = predicate.And(p => p.Tags.Count == tagsFilter.Length && p.Tags.All(t => tagsFilter.Contains(t)));
                }
                else
                {
                    predicate = predicate.And(p => tagsFilter.All(tag => p.Tags.Contains(tag)));
                }
            }
        }

        if (!string.IsNullOrEmpty(parameters.Indexes))
        {
            var indexesFilter = parameters.Indexes.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (indexesFilter.Length > 0)
            {
                predicate = predicate.And(p => indexesFilter.Contains(p.Index));
            }
        }

        if (!string.IsNullOrEmpty(parameters.ProblemName))
        {
            predicate = predicate.And(p => p.ProblemTranslations.Any(
                pt => pt.LanguageCode == parameters.Lang &&
                pt.Name.Contains(parameters.ProblemName)));
        }

        predicate = predicate.And(p => p.Rating >= parameters.MinRating && p.Rating <= parameters.MaxRating);
        predicate = predicate.And(p => p.Points >= parameters.MinPoints && p.Points <= parameters.MaxPoints);

        var difficultyExpr = ProblemExpressions.DifficultyExpression;
        predicate = predicate.And(p =>
            difficultyExpr.Invoke(p) >= parameters.MinDifficulty &&
            difficultyExpr.Invoke(p) <= parameters.MaxDifficulty);

        FilterCondition = predicate;

        // Sorting

        bool isAscending = parameters.Sorting.SortOrder == true;

        switch (parameters.Sorting.SortField.ToLowerInvariant())
        {
            case "name":
                if (isAscending) OrderBy = p => p.ProblemTranslations.FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name;
                else OrderByDescending = p => p.ProblemTranslations.FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name;
                break;

            case "starttime":
                if (isAscending) OrderBy = p => p.Contest.StartTime;
                else OrderByDescending = p => p.Contest.StartTime;
                break;

            case "difficulty":
                var convertedDifficultyExpr = Expression.Lambda<Func<Problem, object>>(
                    Expression.Convert(difficultyExpr.Body, typeof(object)),
                    difficultyExpr.Parameters);

                if (isAscending) OrderBy = convertedDifficultyExpr;
                else OrderByDescending = convertedDifficultyExpr;
                break;

            case "rating":
                if (isAscending) OrderBy = p => p.Rating;
                else OrderByDescending = p => p.Rating;
                break;

            case "points":
                if (isAscending) OrderBy = p => p.Points;
                else OrderByDescending = p => p.Points;
                break;

            case "solvedcount":
                if (isAscending) OrderBy = p => p.SolvedCount;
                else OrderByDescending = p => p.SolvedCount;
                break;

            case "index":
                if (isAscending) OrderBy = p => p.Index;
                else OrderByDescending = p => p.Index;
                break;

            case "contestid":
            default:
                if (isAscending) OrderBy = p => p.ContestId;
                else OrderByDescending = p => p.ContestId;
                break;
        }
    }
}