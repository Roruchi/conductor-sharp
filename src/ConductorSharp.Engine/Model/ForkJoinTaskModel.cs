using System.Collections.Generic;
using ConductorSharp.Client.Generated;
using MediatR;

namespace ConductorSharp.Engine.Model
{
    public class ForkJoinInput : IRequest<NoOutput>
    {
        public ICollection<ICollection<WorkflowTask>> ForkTasks { get; set; }

        public ICollection<string> JoinOn { get; set; }
    }

    /// <summary>
    /// Input for builder-style Fork configuration (placeholder for builder pattern)
    /// </summary>
    public class ForkInput : IRequest<NoOutput>
    {
        // This is a placeholder - actual configuration happens through the builder pattern
    }

    public class ForkJoinTaskModel : TaskModel<ForkJoinInput, NoOutput> { }
}
