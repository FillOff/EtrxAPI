namespace Etrx.Application.Repositories.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IContestsRepository Contests { get; }
    IContestTranslationsRepository ContestTranslations { get; }
    IProblemsRepository Problems { get; }
    IProblemTranslationsRepository ProblemTranslations { get; }
    IRanklistRowsRepository RanklistRows { get; }
    IProblemResultsRepository ProblemResults { get; }
    ISubmissionsRepository Submissions { get; }
    IUsersRepository Users { get; }
    ITagsRepository Tags { get; }

    Task SaveAsync();
}