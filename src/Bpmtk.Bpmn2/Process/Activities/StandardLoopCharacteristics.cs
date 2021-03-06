﻿using System;

namespace Bpmtk.Bpmn2
{
    public class StandardLoopCharacteristics : LoopCharacteristics
    {
        public StandardLoopCharacteristics()
        {
            this.TestBefore = false;
        }

        /// <summary>
        /// A boolean Expression that controls the loop. The Activity will only loop
        //as long as this condition is true. The looping behavior MAY be
        //underspecified, meaning that the modeler can simply document the
        //condition, in which case the loop cannot be formally executed.
        /// </summary>
        public virtual Expression LoopCondition
        {
            get;
            set;
        }

        /// <summary>
        /// Flag that controls whether the loop condition is evaluated at the beginning
        // (testBefore = true) or at the end(testBefore = false) of the loop
        // iteration.
        /// </summary>
        public virtual bool TestBefore
        {
            get;
            set;
        }

        /// <summary>
        /// Serves as a cap on the number of iterations.
        /// </summary>
        public virtual int? LoopMaximum
        {
            get;
            set;
        }

        //public override void Execute(ExecutionContext executionContext)
        //{
        //    var loopCounter = executionContext.GetVariableLocal<int?>("loopCounter");
        //    if (loopCounter != null)
        //        throw new RuntimeException("The loop activity can't call execute.");

        //    if (this.LoopCondition == null 
        //            && this.LoopMaximum == null)
        //        throw new RuntimeException("Either loopCondition or loopMaximum was not specified.");

        //    if (LoopMaximum.HasValue && LoopMaximum.Value < 1)
        //        throw new RuntimeException("Invalid loopMaximum.");

        //    if (this.TestBefore)
        //    {
        //        bool canLoop = false;
        //        if(this.LoopCondition != null)
        //            canLoop = executionContext.EvaluteExpression<bool>(this.LoopCondition.Text);

        //        if(canLoop)
        //        {
        //            executionContext.SetVariableLocal("loopCounter", 0);
        //            this.Activity.Execute(executionContext);
        //        }
        //    }
        //}
    }
}
