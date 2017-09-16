using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MySimpleWebApi.Controllers
{
    /// <summary>
    /// Guess...
    /// </summary>
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly SimpleDataAccess _dataAccess;

        /// <summary>
        /// Default constructor (duh)
        /// </summary>
        public ValuesController()
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            _dataAccess = new SimpleDataAccess(connectionString);
        }

        /// <summary>
        /// Returns all values stored
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _dataAccess.GetAllValues();
        }

        /// <summary>
        /// Get value by Id
        /// </summary>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return _dataAccess.GetValueById(id);
        }

        /// <summary>
        /// Save new value
        /// </summary>
        /// <returns>the value's Id</returns>
        [HttpPost]
        public int Post([FromBody]string value)
        {
            return _dataAccess.AddNewValue(value);
        }

        /// <summary>
        /// Save value with predefined Id
        /// </summary>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            var simpleValue = new SimpleValue{Id = id, Value = value};
            _dataAccess.AddNewValue(simpleValue);
        }

        /// <summary>
        /// Delete value by id
        /// </summary>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _dataAccess.DeleteById(id);
        }
    }
}
