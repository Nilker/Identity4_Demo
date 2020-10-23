using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _01mongodbRepository.DB.Entity
{
    public class Student : EntityBase<Guid>
    {
        public string SName { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }
    }
}
