﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bpmtk.Engine.Bpmn2;
using Bpmtk.Engine.Models;
using Bpmtk.Engine.Storage;
using Bpmtk.Engine.Utils;

namespace Bpmtk.Engine.Repository
{
    public class DeploymentManager : IDeploymentManager
    {
        /// <summary>
        /// BPMN Object Model cached by DeploymentId.
        /// </summary>
        private static readonly ConcurrentDictionary<int, BpmnModel> modelCache = new ConcurrentDictionary<int, BpmnModel>();

        private readonly IDbSession session;
        private readonly Context context;

        public DeploymentManager(Context context)
        {
            this.context = context;
            this.session = context.DbSession;
        }

        //public virtual IQueryable<Deployment> Deployments => this.session.Deployments;

        //public virtual IQueryable<ProcessDefinition> ProcessDefinitions => this.session.ProcessDefinitions;

        public virtual IDeploymentBuilder CreateDeploymentBuilder()
        {
            return new DeploymentBuilder(context, this);
        }

        public virtual Deployment Find(int deploymentId)
            => this.session.Find<Deployment>(deploymentId);

        IDeployment IDeploymentManager.Find(int deploymentId)
            => this.Find(deploymentId);

        IProcessDefinition IDeploymentManager.FindProcessDefinitionById(int processDefinitionId)
            => this.FindProcessDefinitionById(processDefinitionId);

        IProcessDefinition IDeploymentManager.FindProcessDefinitionByKey(string processDefinitionKey)
            => this.FindProcessDefinitionByKey(processDefinitionKey);

        public virtual ProcessDefinition FindProcessDefinitionById(int processDefinitionId)
            => this.session.Find<ProcessDefinition>(processDefinitionId);

        public virtual Task<ProcessDefinition> FindProcessDefinitionByIdAsync(int processDefinitionId)
            => this.session.FindAsync<ProcessDefinition>(processDefinitionId);

        public virtual ProcessDefinition FindProcessDefinitionByKey(string processDefinitionKey)
        {
            var query = this.session.ProcessDefinitions
                .Where(x => x.Key == processDefinitionKey)
                .OrderByDescending(x => x.Version)
                .Take(1);

            return query.SingleOrDefault();
        }

        public virtual Task<ProcessDefinition> FindProcessDefinitionByKeyAsync(string processDefinitionKey)
        {
            var query = this.session.ProcessDefinitions
                .Where(x => x.Key == processDefinitionKey)
                .OrderByDescending(x => x.Version)
                .Take(1);

            return this.session.QuerySingleAsync(query);
        }

        public virtual IList<EventSubscription> GetEventSubscriptions(int processDefintionId)
        {
            var query = this.session.EventSubscriptions
                .Where(x => x.ProcessDefinition.Id == processDefintionId);

            return query.ToList();
        }

        public virtual EventSubscription FindEventSubscriptionByMessage(string messageName)
        {
            if (string.IsNullOrEmpty(messageName))
                throw new ArgumentException("message", nameof(messageName));

            var query = this.session.EventSubscriptions
                .Where(x => x.EventType == "message"
                    && x.EventName == messageName
                  );

            return query.SingleOrDefault();
        }

        public virtual IList<ScheduledJob> GetScheduledJobs(int processDefintionId)
        {
            var query = this.session.ScheduledJobs
                .Where(x => x.ProcessDefinition.Id == processDefintionId);

            return query.ToList();
        }

        public virtual BpmnModel GetBpmnModel(int deploymentId)
        {
            BpmnModel model = null;
            if (!modelCache.ContainsKey(deploymentId))
            {
                var query = this.session.Deployments.Where(x => x.Id == deploymentId)
                    .Select(x => x.Model.Value);

                var bytes = query.SingleOrDefault();
                model = BpmnModel.FromBytes(bytes);
            }

            return modelCache.GetOrAdd(deploymentId, (id) => model);
        }

        public virtual async Task AddIdentityLinksAsync(int processDefinitionId, params IdentityLink[] identityLinks)
        {
            if(identityLinks != null && identityLinks.Length > 0)
            {
                //Check if identity-link already exists.
                var procDef = await this.FindProcessDefinitionByIdAsync(processDefinitionId);

                var date = Utils.Clock.Now;
                foreach (var item in identityLinks)
                {
                    item.Created = date;
                    item.ProcessDefinition = procDef;
                    //procDef.IdentityLinks.Add(item);
                }

                await this.session.SaveRangeAsync(identityLinks);
                await this.session.FlushAsync();
            }
        }

        public virtual Task<IList<IdentityLink>> GetIdentityLinksAsync(int processDefintionId)
        {
            var query = this.session.IdentityLinks.Where(x => x.ProcessDefinition.Id == processDefintionId);

            return this.session.QueryMultipleAsync(query);
        }

        public virtual async Task RemoveIdentityLinksAsync(params long[] identityLinkIds)
        {
            if(identityLinkIds != null && identityLinkIds.Length > 0)
            {
                var query = this.session.IdentityLinks.Where(x => identityLinkIds.Contains(x.Id));
                var items = query.ToList();

                this.session.RemoveRange(items);

                await this.session.FlushAsync();
            }
        }

        public virtual DeploymentQuery CreateDeploymentQuery() => new DeploymentQuery(this.session);

        public virtual ProcessDefinitionQuery CreateDefinitionQuery() => new ProcessDefinitionQuery(this.context);

        public virtual ProcessDefinition InactivateProcessDefinition(int processDefinitionId,
            string comment = null)
        {
            var procDef = this.FindProcessDefinitionById(processDefinitionId);
            if (procDef == null)
                throw new ObjectNotFoundException(nameof(ProcessDefinition));

            procDef.State = ProcessDefinitionState.Inactive;

            var item = new Comment();
            item.ProcessDefinition = procDef;
            item.Body = comment;
            item.Created = Clock.Now;
            item.UserId = this.context.UserId;

            this.session.Save(item);
            this.session.Flush();

            return procDef;
        }

        public virtual ProcessDefinition ActivateProcessDefinition(int processDefinitionId, string comment = null)
        {
            var procDef = this.FindProcessDefinitionById(processDefinitionId);
            if (procDef == null)
                throw new ObjectNotFoundException(nameof(ProcessDefinition));

            procDef.State = ProcessDefinitionState.Active;

            var item = new Comment();
            item.ProcessDefinition = procDef;
            item.Body = comment;
            item.Created = Clock.Now;
            item.UserId = this.context.UserId;

            this.session.Save(item);
            this.session.Flush();

            return procDef;
        }

        IProcessDefinition IDeploymentManager.ActivateProcessDefinition(int processDefinitionId, string comment)
            => this.ActivateProcessDefinition(processDefinitionId, comment);

        IProcessDefinition IDeploymentManager.InactivateProcessDefinition(int processDefinitionId, string comment)
            => this.InactivateProcessDefinition(processDefinitionId, comment);

        public virtual async Task<IProcessDefinition> InactivateProcessDefinitionAsync(int processDefinitionId,
            string comment = null)
        {
            var procDef = this.FindProcessDefinitionById(processDefinitionId);
            if (procDef == null)
                throw new ObjectNotFoundException(nameof(ProcessDefinition));

            procDef.State = ProcessDefinitionState.Inactive;

            var item = new Comment();
            item.ProcessDefinition = procDef;
            item.Body = comment;
            item.Created = Clock.Now;
            item.UserId = this.context.UserId;

            await this.session.SaveAsync(item);
            await this.session.FlushAsync();

            return procDef;
        }

        public virtual async Task<IProcessDefinition> ActivateProcessDefinitionAsync(int processDefinitionId, string comment = null)
        {
            var procDef = this.FindProcessDefinitionById(processDefinitionId);
            if (procDef == null)
                throw new ObjectNotFoundException(nameof(ProcessDefinition));

            procDef.State = ProcessDefinitionState.Active;

            var item = new Comment();
            item.ProcessDefinition = procDef;
            item.Body = comment;
            item.Created = Clock.Now;
            item.UserId = this.context.UserId;

            await this.session.SaveAsync(item);
            await this.session.FlushAsync();

            return procDef;
        }

        public virtual IList<Comment> GetCommentsForProcessDefinition(int processDefinitionId)
        {
            var query = this.session.Query<Comment>()
                .Where(x => x.ProcessDefinition.Id == processDefinitionId)
                .OrderByDescending(x => x.Created);

            return query.ToList();
        }

        IProcessDefinitionQuery IDeploymentManager.CreateDefinitionQuery()
            => this.CreateDefinitionQuery();

        IDeploymentQuery IDeploymentManager.CreateDeploymentQuery()
            => this.CreateDeploymentQuery();

        public virtual Task<byte[]> GetBpmnModelContentAsync(int deploymentId)
        {
            var query = this.session.Deployments.Where(x => x.Id == deploymentId)
                .Select(x => x.Model.Value);

            return this.session.QuerySingleAsync(query);
        }
    }
}
