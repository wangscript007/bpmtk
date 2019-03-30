﻿using System;
using System.Xml.Linq;

namespace Bpmtk.Bpmn2.Parser.Handlers
{
    abstract class EventParseHandler : FlowNodeParseHandler
    {
        public EventParseHandler()
        {
            this.handlers.Add("property", new PropertyParseHandler());
        }
    }

    abstract class CatchEventParseHandler : EventParseHandler
    {
        public CatchEventParseHandler()
        {
            var keys = EventDefinitionParseHandler.Keys;
            IParseHandler handler = new EventDefinitionParseHandler();

            foreach (var key in keys)
                this.handlers.Add(key, handler);

            this.handlers.Add("dataOutput", new DataOutputParseHandler());
            this.handlers.Add("dataOutputAssociation", new DataOutputAssociationParseHandler());

            this.handlers.Add("eventDefinitionRef", new ParseHandlerAction<CatchEvent>((p, c, x) =>
            {
                var eventDefinitionRef = x.Value;
                if(eventDefinitionRef != null)
                    c.AddReferenceRequest<EventDefinition>(eventDefinitionRef,(d) => p.EventDefinitionRefs.Add(d));
            }));
        }

        protected virtual void Init(CatchEvent catchEvent, IParseContext context, XElement element)
        {
            catchEvent.ParallelMultiple = element.GetBoolean("parallelMultiple");

            base.Init(catchEvent, context, element);
        }
    }

    abstract class ThrowEventParseHandler : EventParseHandler
    {
        public ThrowEventParseHandler()
        {
            this.handlers.Add("dataInput", new DataInputParseHandler());
            this.handlers.Add("dataInputAssociation", new DataInputAssociationParseHandler());

            var keys = EventDefinitionParseHandler.Keys;
            IParseHandler handler = new EventDefinitionParseHandler();

            foreach (var key in keys)
                this.handlers.Add(key, handler);

            this.handlers.Add("eventDefinitionRef", new ParseHandlerAction<ThrowEvent>((p, c, x) =>
            {
                var eventDefinitionRef = x.Value;
                if (eventDefinitionRef != null)
                    c.AddReferenceRequest<EventDefinition>(eventDefinitionRef, (d) => p.EventDefinitionRefs.Add(d));
            }));
        }
    }

    class StartEventParseHandler : CatchEventParseHandler
    {
        public override object Create(IFlowElementsContainer parent, IParseContext context, XElement element)
        {
            var startEvent = context.BpmnFactory.CreateStartEvent();
            parent.FlowElements.Add(startEvent);

            startEvent.IsInterrupting = element.GetBoolean("isInterrupting");

            base.Init(startEvent, context, element);

            return startEvent;
        }
    }

    class EndEventParseHandler : ThrowEventParseHandler
    {
        public override object Create(IFlowElementsContainer parent, IParseContext context, XElement element)
        {
            var endEvent = context.BpmnFactory.CreateEndEvent();
            parent.FlowElements.Add(endEvent);

            base.Init(endEvent, context, element);

            return endEvent;
        }
    }
}