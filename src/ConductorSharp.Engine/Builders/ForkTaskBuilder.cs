using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ConductorSharp.Client.Generated;
using ConductorSharp.Engine.Interface;
using ConductorSharp.Engine.Model;
using ConductorSharp.Engine.Util.Builders;

namespace ConductorSharp.Engine.Builders
{
    /// <summary>
    /// Extension methods for adding Fork tasks with builder pattern
    /// </summary>
    public static class ForkTaskExtensions
    {
        /// <summary>
        /// Adds a Fork task using builder pattern for configuring branches
        /// </summary>
        /// <typeparam name="TWorkflow">The workflow type</typeparam>
        /// <param name="builder">The sequence builder</param>
        /// <param name="reference">Expression pointing to the fork task model property</param>
        /// <param name="configureFork">Action to configure the fork branches</param>
        /// <returns>Task options builder for additional configuration</returns>
        public static ITaskOptionsBuilder Fork<TWorkflow>(
            this ITaskSequenceBuilder<TWorkflow> builder,
            Expression<Func<TWorkflow, ForkJoinTaskModel>> reference,
            Action<IForkBuilder<TWorkflow>> configureFork
        )
            where TWorkflow : ITypedWorkflow
        {
            // Create a dummy input expression since we're using builder pattern
            Expression<Func<TWorkflow, ForkInput>> inputExpression = wf => new ForkInput();

            var taskBuilder = new ForkTaskBuilder<TWorkflow>(reference.Body, inputExpression.Body, builder.BuildConfiguration);
            builder.AddTaskBuilderToSequence(taskBuilder);

            // Configure the fork branches
            configureFork(taskBuilder);

            return taskBuilder;
        }
    }

    /// <summary>
    /// Builder for Fork tasks that manages multiple parallel branches
    /// </summary>
    public class ForkTaskBuilder<TWorkflow> : BaseTaskBuilder<ForkInput, NoOutput>, IForkBuilder<TWorkflow>
        where TWorkflow : ITypedWorkflow
    {
        private readonly List<ForkBranchBuilder<TWorkflow>> _branches = new();
        private readonly List<string> _joinOnTasks = new();

        public ForkTaskBuilder(Expression taskExpression, Expression inputExpression, BuildConfiguration buildConfiguration)
            : base(taskExpression, inputExpression, buildConfiguration) { }

        /// <summary>
        /// Adds a new fork branch
        /// </summary>
        /// <param name="configureBranch">Action to configure tasks within this fork branch</param>
        public void AddFork(Action<ITaskSequenceBuilder<TWorkflow>> configureBranch)
        {
            var branchBuilder = new ForkBranchBuilder<TWorkflow>(_buildConfiguration);
            configureBranch(branchBuilder);

            _branches.Add(branchBuilder);

            // Collect task reference names for JoinOn property
            var branchTasks = branchBuilder.BuildBranch();
            foreach (var task in branchTasks)
            {
                if (!string.IsNullOrEmpty(task.TaskReferenceName))
                {
                    _joinOnTasks.Add(task.TaskReferenceName);
                }
            }
        }

        /// <summary>
        /// Builds the FORK_JOIN and JOIN tasks
        /// </summary>
        /// <returns>Array containing the fork and join workflow tasks</returns>
        public override WorkflowTask[] Build()
        {
            var forkTaskName = $"FORK_{_taskRefferenceName}";
            var joinTaskName = $"JOIN_{_taskRefferenceName}";

            // Build fork tasks from all branches
            var forkTasks = _branches.Select(branch => branch.BuildBranch()).ToArray();

            return new[]
            {
                new WorkflowTask
                {
                    Name = forkTaskName,
                    TaskReferenceName = forkTaskName,
                    WorkflowTaskType = WorkflowTaskType.FORK_JOIN,
                    Type = WorkflowTaskType.FORK_JOIN.ToString(),
                    ForkTasks = forkTasks,
                    Optional = _additionalParameters?.Optional == true
                },
                new WorkflowTask
                {
                    Name = joinTaskName,
                    TaskReferenceName = joinTaskName,
                    WorkflowTaskType = WorkflowTaskType.JOIN,
                    Type = WorkflowTaskType.JOIN.ToString(),
                    JoinOn = _joinOnTasks.ToArray()
                }
            };
        }
    }
}
