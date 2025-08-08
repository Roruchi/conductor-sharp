using MediatR;

namespace ConductorSharp.Engine.Model
{
    public class ForkJoinInput : IRequest<NoOutput>
    {
        public object ForkTasks { get; set; }

        public object JoinOn { get; set; }
    }

    public class ForkJoinTaskModel : TaskModel<ForkJoinInput, NoOutput> { }
}
