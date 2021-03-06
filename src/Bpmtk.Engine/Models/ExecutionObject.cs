﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bpmtk.Engine.Runtime;
using Bpmtk.Engine.Variables;

namespace Bpmtk.Engine.Models
{
    public abstract class ExecutionObject : IExecutionObject
    {
        public virtual long Id
        {
            get;
            set;
        }

        public virtual ExecutionState State
        {
            get;
            set;
        }

        public virtual bool IsEnded
        {
            get => this.State == ExecutionState.Completed || this.State == ExecutionState.Terminated || this.State == ExecutionState.Aborted;
        }

        //public abstract IEnumerable<IVariable> Variables
        //{
        //    get;
        //}

        public virtual ICollection<IdentityLink> IdentityLinks
        {
            get;
            set;
        }

        //public virtual Variable GetVariableInstance(string name)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException(nameof(name));

        //    var items = this.Variables;
        //    return items.Where(x => x.Name == name).SingleOrDefault();
        //}

        //public virtual bool GetVariable(string name, out object value)
        //{
        //    return null;
        //}
        public abstract IReadOnlyList<IVariable> VariableInstances
        {
            get;
        }

        public abstract object GetVariable(string name);

        public abstract void SetVariable(string name, object value);

        public virtual string Name
        {
            get;
            set;
        }

        public virtual DateTime Created
        {
            get;
            set;
        }

        public virtual DateTime? StartTime
        {
            get;
            set;
        }

        public virtual DateTime LastStateTime
        {
            get;
            set;
        }

        public virtual DateTime Modified
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual string ConcurrencyStamp
        {
            get;
            set;
        }

        IDictionary<string, object> IExecutionObject.Variables
        {
            get
            {
                var map = new Dictionary<string, object>();

                var items = this.VariableInstances;
                foreach(var item in items)
                {
                    map.Add(item.Name, item.GetValue());
                }

                return map;
            }
        }

        //public virtual void Suspend()
        //{

        //}

        //public virtual void Resume()
        //{

        //}

        //public abstract void Terminate(IContext context, string endReason = null);
    }

    
}
