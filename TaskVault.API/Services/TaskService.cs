using System;
using TaskVault.API.Data;

namespace TaskVault.API.Services;

public interface ITaskService
{

}

public class TaskService : ITaskService
{
    private readonly DbContext _db;

    public TaskService(DbContext db)
    {
     
        _db = db;
    
    }

}
