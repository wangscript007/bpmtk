using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xunit;
//using NHibernate;
//using NHibernate.Cfg;
using Bpmtk.Engine.Models;
using Bpmtk.Engine.Runtime;
//using NHibernate.Tool.hbm2ddl;
using Xunit.Abstractions;
using Bpmtk.Engine;
using Bpmtk.Engine.Repository;
using Microsoft.Extensions.DependencyInjection;
using Jint;
using Jint.Native;
using Jint.Runtime.References;
using System.Threading.Tasks;

namespace Bpmtk.Engine.Tests
{
    public class ProcessEngineBuilderUnitTest : BpmtkTestCase
    {
        public ProcessEngineBuilderUnitTest(ITestOutputHelper output) : base(output)
        {
        }

        

        [Fact] public async Task Execute()
        {
            await this.DeployBpmnModel("Bpmtk.Engine.Tests.Resources.sequential_flow.bpmn.xml");

            var pi = await this.runtimeManager.StartProcessByKeyAsync("Process_0cyms8o");

            this.Commit();

            //var data = rs.GetBpmnModelData(5);

            //var variables = new Dictionary<string, object>();
            //variables.Add("days", 6);

            //var es = context.GetService<IRuntimeService>();

            //var query = es.CreateProcessInstanceQuery();
            //var items = query
            //    .List(10);
            //var count = query.Count();

            //var processInstance = es.StartProcessByKey("leaveRequest", variables);

            //foreach(var name in names)
            //{
            //    cfg.AddAssembly()
            //}
            //.AddFile()
            //cfg = cfg.AddClass(typeof(ExecutionObject))
            //   .AddClass(typeof(ProcessInstance))
            //   .AddClass(typeof(ActivityInstance))
            //   .AddClass(typeof(Token));

            
            //var factory = cfg.BuildSessionFactory();
            //var session = factory.WithOptions().Interceptor(new XUnitSqlCaptureInterceptor(this.output))
            //    .OpenSession();

            //var list = session.Query<ProcessDefinition>().Where(x => x.DeploymentId == 1).ToList();
            //var query = session.CreateQuery("from InstanceIdentityLink where proc_inst_id=1");
            //var list = query.List<InstanceIdentityLink>();
        }

        
    }
}
