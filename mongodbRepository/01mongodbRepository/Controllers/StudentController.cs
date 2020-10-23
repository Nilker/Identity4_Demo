using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _01mongodbRepository.DB.Domain;
using _01mongodbRepository.DB.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _01mongodbRepository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Student>>> Get()
        {
            var products = await _studentRepository.Get();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> Get(string id)
        {
            var product = await _studentRepository.Get(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Student model)
        {
            await _studentRepository.Create(model);
            return Ok();
        }
    }
}
