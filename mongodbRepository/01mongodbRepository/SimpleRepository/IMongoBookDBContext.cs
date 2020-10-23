using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace _01mongodbRepository.DB
{
    public interface IMongoBookDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
