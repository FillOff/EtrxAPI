using Etrx.Domain.Interfaces;
using Etrx.Domain.Interfaces.UnitOfWork;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Repositories;

namespace Etrx.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EtrxDbContext _dbContext;

    private IContestsRepository? _contestsRepository;
    private IContestTranslationsRepository? _contestTranslationsRespository;
    private IProblemsRepository? _problemsRepository;
    private IProblemTranslationsRepository? _problemTranslationsRespository;
    private IRanklistRowsRepository? _ranklistRowsRepository;
    private IProblemResultsRepository? _problemResultsRepository;
    private ISubmissionsRepository? _submissionsRepository;
    private IUsersRepository? _usersRepository;

    public UnitOfWork(EtrxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IContestsRepository Contests
    {
        get
        {
            _contestsRepository ??= new ContestsRepository(_dbContext);
            return _contestsRepository;
        }
    }

    public IContestTranslationsRepository ContestTranslations
    {
        get
        {
            _contestTranslationsRespository ??= new ContestTranslationsRepository(_dbContext);
            return _contestTranslationsRespository;
        }
    }

    public IProblemsRepository Problems
    {
        get
        {
            _problemsRepository ??= new ProblemsRepository(_dbContext);
            return _problemsRepository;
        }
    }

    public IProblemTranslationsRepository ProblemTranslations
    {
        get
        {
            _problemTranslationsRespository ??= new ProblemTranslationsRepository(_dbContext);
            return _problemTranslationsRespository;
        }
    }

    public IRanklistRowsRepository RanklistRows
    {
        get
        {
            _ranklistRowsRepository ??= new RanklistRowsRepository(_dbContext);
            return _ranklistRowsRepository;
        }
    }

    public IProblemResultsRepository ProblemResults
    {
        get
        {
            _problemResultsRepository ??= new ProblemResultsRepository(_dbContext);
            return _problemResultsRepository;
        }
    }

    public ISubmissionsRepository Submissions
    {
        get
        {
            _submissionsRepository ??= new SubmissionsRepository(_dbContext);
            return _submissionsRepository;
        }
    }

    public IUsersRepository Users
    {
        get
        {
            _usersRepository ??= new UsersRepository(_dbContext);
            return _usersRepository;
        }
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}