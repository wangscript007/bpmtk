﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bpmtk.Engine.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bpmtk.Engine.WebApi.Controllers
{
    /// <summary>
    /// ProcessDefinition APIs
    /// </summary>
    [Route("api/process-definitions")]
    [ApiController]
    public class ProcessDefinitionController : ControllerBase
    {
        private readonly IContext context;

        public ProcessDefinitionController(IContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Filter ProcessDefinitions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<ProcessDefinitionModel>> Get()
        {
            var query = this.context.DeploymentManager.ProcessDefinitions;
            var q = query.GroupBy(x => x.Key)
                .Select(x => new
                {
                    key = x.Key,
                    version = x.Max(y => y.Version)
                });

            var q2 = from item in query
                     join b in q on new
                     {
                         key = item.Key,
                         version = item.Version
                     } equals b
                     select item;

            var data = q2.Select(x => ProcessDefinitionModel.Create(x)).ToArray();

            return data;
        }

        /// <summary>
        /// Find the specified ProcessDefinition.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessDefinitionModel>> Get(int id)
        {
            var item = await this.context.DeploymentManager.FindProcessDefinitionByIdAsync(id);
            if (item != null)
                return ProcessDefinitionModel.Create(item);

            return this.NotFound();
        }

        /// <summary>
        /// Gets IdentityLinks of ProcessDefinition.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/identity-links")]
        public ActionResult<IEnumerable<IdentityLinkModel>> GetIdentityLinks(int id)
        {
            var q = this.context.DbSession.IdentityLinks
                .Where(x => x.ProcessDefinition.Id == id)
                .Select(x => new IdentityLinkModel
                {
                    Id = x.Id,
                    UserId = x.User.Id,
                    UserName = x.User.UserName,
                    GroupId = x.Group.Id,
                    GroupName = x.Group.Name,
                    Type = x.Type
                }).ToArray();
            
            return q;
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}