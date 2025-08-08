using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConductorSharp.Client.Generated;
using ConductorSharp.Engine.Interface;
using ConductorSharp.Engine.Model;
using ConductorSharp.Engine.Util.Builders;

namespace ConductorSharp.Engine.Builders
{
    public static class ForkJoinTaskExtensions
    {
        public static ITaskOptionsBuilder AddTask<TWorkflow>(
            this ITaskSequenceBuilder<TWorkflow> builder,
            Expression<Func<TWorkflow, ForkJoinTaskModel>> refference,
            Expression<Func<TWorkflow, ForkJoinInput>> input
        )
            where TWorkflow : ITypedWorkflow
        {
            var taskBuilder = new ForkJoinTaskBuilder(refference.Body, input.Body, builder.BuildConfiguration);
            builder.AddTaskBuilderToSequence(taskBuilder);
            return taskBuilder;
        }
    }

    public class ForkJoinTaskBuilder(Expression taskExpression, Expression inputExpression, BuildConfiguration buildConfiguration)
        : BaseTaskBuilder<ForkJoinInput, NoOutput>(taskExpression, inputExpression, buildConfiguration)
    {
        public override WorkflowTask[] Build()
        {
            var forkTaskName = $"FORK_{_taskRefferenceName}";
            var joinTaskName = $"JOIN_{_taskRefferenceName}";

            // Extract forkTasks and joinOn from the input parameters
            var inputParametersDict = _inputParameters.ToObject<IDictionary<string, object>>();

            // Convert the anonymous objects to proper WorkflowTask objects
            ICollection<ICollection<WorkflowTask>> forkTasks = null;
            ICollection<string> joinOn = null;

            if (inputParametersDict?.ContainsKey("forkTasks") == true)
            {
                var forkTasksJson = _inputParameters["forkTasks"];
                forkTasks = forkTasksJson?.ToObject<ICollection<ICollection<WorkflowTask>>>();
            }

            if (inputParametersDict?.ContainsKey("joinOn") == true)
            {
                joinOn = _inputParameters["joinOn"]?.ToObject<ICollection<string>>();
            }

            return
            [
                new()
                {
                    Name = forkTaskName,
                    TaskReferenceName = forkTaskName,
                    WorkflowTaskType = WorkflowTaskType.FORK_JOIN,
                    Type = WorkflowTaskType.FORK_JOIN.ToString(),
                    ForkTasks = forkTasks,
                    InputParameters = inputParametersDict,
                    Optional = _additionalParameters?.Optional == true
                },
                new()
                {
                    Name = joinTaskName,
                    TaskReferenceName = joinTaskName,
                    WorkflowTaskType = WorkflowTaskType.JOIN,
                    Type = WorkflowTaskType.JOIN.ToString(),
                    JoinOn = joinOn
                }
            ];
        }
    }
}
