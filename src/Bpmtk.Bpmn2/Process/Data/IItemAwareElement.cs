﻿using System;

namespace Bpmtk.Bpmn2
{
    public interface IItemAwareElement : IBaseElement
    {
        string Name
        {
            get;
            set;
        }

        ItemDefinition ItemSubjectRef
        {
            get;
            set;
        }

        DataState DataState
        {
            get;
            set;
        }
    }
}
