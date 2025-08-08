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

    public class ForkJoinTaskModel : TaskModel<ForkJoinInput, NoOutput> { }
}
