using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Practice_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Practice_WebApi.Controllers
{
    public class EmployeeController : ApiController
    {
        private NorthwindContext _Context = new NorthwindContext();

        // GET api/<controller>
        [EnableCors(origins: "https://localhost:44364", headers: "*", methods: "*")]
        [HttpGet]
        public List<Employees> GetAllEmployee()
        {
            return _Context.Employees.ToList();
        }

        // GET api/<controller>/5
        [EnableCors(origins: "https://localhost:44364", headers: "*", methods: "*")]
        public Employees Get(int id)
        {
            return _Context.Employees.ToList().FirstOrDefault(x => x.EmployeeID == id);
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {

        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}