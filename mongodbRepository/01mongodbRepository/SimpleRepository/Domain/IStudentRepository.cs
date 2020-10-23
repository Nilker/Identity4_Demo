using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _01mongodbRepository.DB.Entity;

namespace _01mongodbRepository.DB.Domain
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
    }
}
