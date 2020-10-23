using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace _01mongodbRepository.DB.Entity
{
    public class EntityBase<Tkey>
    {

        public EntityBase()
        {
            CreateDateTime = UpdateDateTime = DateTime.Now;
        }

        /// <summary>
        /// MongoDB系统自带的主键
        /// </summary>
        [Key]
        public Tkey _id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
