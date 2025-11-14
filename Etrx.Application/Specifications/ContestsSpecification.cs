using Etrx.Application.Queries;
using Etrx.Domain.Models;
using LinqKit;

namespace Etrx.Application.Specifications;

public class ContestsSpecification : BaseSpecification<Contest>
{
    public ContestsSpecification(ContestQueryParameters parameters)
    {
        var predicate = PredicateBuilder.New<Contest>(true);
        predicate = predicate.And(c => c.Phase != "BEFORE");

        if (parameters.Gym != null)
        {
            predicate = predicate.And(c => c.Gym == parameters.Gym);
        }
        FilterCondition = predicate;

        bool isAscending = parameters.Sorting.SortOrder == true;

        switch (parameters.Sorting.SortField.ToLowerInvariant())
        {
            case "name":
                if (isAscending) OrderBy = c => c.ContestTranslations.FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name;
                else OrderByDescending = c => c.ContestTranslations.FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name;
                break;

            case "starttime":
                if (isAscending) OrderBy = c => c.StartTime;
                else OrderByDescending = c => c.StartTime;
                break;

            case "durationseconds":
                if (isAscending) OrderBy = c => c.DurationSeconds;
                else OrderByDescending = c => c.DurationSeconds;
                break;

            case "relativetimeseconds":
                if (isAscending) OrderBy = c => c.RelativeTimeSeconds;
                else OrderByDescending = c => c.RelativeTimeSeconds;
                break;

            case "gym":
                if (isAscending) OrderBy = c => c.Gym;
                else OrderByDescending = c => c.Gym;
                break;

            case "iscontestloaded":
                if (isAscending) OrderBy = c => c.IsContestLoaded;
                else OrderByDescending = c => c.IsContestLoaded;
                break;

            case "contestid":
            default:
                if (isAscending) OrderBy = c => c.ContestId;
                else OrderByDescending = c => c.ContestId;
                break;
        }
    }
}
