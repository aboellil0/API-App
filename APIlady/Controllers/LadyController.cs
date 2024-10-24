using APIlady.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIlady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class LadyController : ControllerBase
    {
        LadyContext context;
        public LadyController(LadyContext context) {
            this.context = context;
        }

        [HttpGet]
        public IActionResult DisplayAllDipartmint()
        {
            var DeptToDisplay = context.Department.ToList();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(DeptToDisplay);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetDepartmentById(int id)
        {
            var dept = context.Department.FirstOrDefault(x => x.Id == id);
            return Ok(dept);
        }


        [HttpGet("{name}")]
        public IActionResult GetDepartmentById(string name)
        {
            var dept = context.Department.Where(x => x.Name.Contains(name)).ToList();
            return Ok(dept);
        }


        [HttpPost]
        public IActionResult AddDepartment(Department dept)
        {
            context.Department.Add(dept);
            context.SaveChanges();
            //return Created($"https://localhost:7241/api/Lady/{dept.Id}",dept);
            return CreatedAtAction("GetDepartmentById",new {Id = dept.Id }, dept);
        }


        [HttpPut("{deptId:int}")]
        public IActionResult UpdateDepartment(int deptId, Department dept)
        {
            var UpdatedDept = context.Department.FirstOrDefault(d => d.Id == deptId);
            if (UpdatedDept == null)
            {
                return NotFound();
            }
            else
            {
                UpdatedDept.Name = dept.Name;
                UpdatedDept.MangerName = dept.MangerName;
                context.SaveChanges();
                return Ok(UpdatedDept);
            }
        }
        [HttpPatch]
        public IActionResult PatchDepartment(int deptId,string name)
        {
            var UpdatedDept = context.Department.FirstOrDefault(d => d.Id == deptId);
            if (UpdatedDept == null)
            {
                return NotFound();
            }
            else
            {
                UpdatedDept.Name = name;
                context.SaveChanges();
                return Ok(UpdatedDept);
            }
        }
    }
}
