using System;
namespace TaskVault.API.Services;
using TaskVault.API.Data;

public interface ISearchService
{

}

public class SearchService : ISearchService
{

    private readonly DbContext _db;

    public SearchService(DbContext db)
    {
     
        _db = db;
    
    }

}
