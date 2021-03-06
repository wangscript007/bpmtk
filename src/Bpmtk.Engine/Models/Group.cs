﻿using System;
using System.Collections.Generic;

namespace Bpmtk.Engine.Models
{
    public class Group
    {
        public virtual string Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string ConcurrencyStamp
        {
            get;
            set;
        }

        public virtual DateTime Created
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual ICollection<UserGroup> Users
        {
            get;
            set;
        }
    }
}
