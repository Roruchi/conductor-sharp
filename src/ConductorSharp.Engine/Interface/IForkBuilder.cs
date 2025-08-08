using System;
using ConductorSharp.Engine.Interface;

namespace ConductorSharp.Engine.Interface
{
    /// <summary>
    /// Interface for building fork branches in a ForkJoin task
    /// </summary>
    public interface IForkBuilder<TWorkflow>
        where TWorkflow : ITypedWorkflow
    {
        /// <summary>
        /// Adds a new fork branch with tasks configured via the taskBuilder action
        /// </summary>
        /// <param name="configureBranch">Action to configure tasks within this fork branch</param>
        void AddFork(Action<ITaskSequenceBuilder<TWorkflow>> configureBranch);
    }
}
