using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace _01mongodbRepository.DB
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task Create(TEntity obj);
        void Delete(string id);
        void Update(TEntity obj);
        Task<TEntity> Get(string id);
        Task<IEnumerable<TEntity>> Get();
    }
}
