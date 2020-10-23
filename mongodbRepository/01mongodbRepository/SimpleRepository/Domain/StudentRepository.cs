using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _01mongodbRepository.DB.Entity;

namespace _01mongodbRepository.DB.Domain
{
    public class StudentRepository : BaseRepository<Student,Guid>, IStudentRepository
    {
        public StudentRepository(IMongoBookDBContext context) : base(context)
        {
        }
    }
}
