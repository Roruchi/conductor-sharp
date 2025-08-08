using System.Collections.Generic;
using System.Linq;
using ConductorSharp.Client.Generated;
using ConductorSharp.Engine.Interface;
using ConductorSharp.Engine.Util;
using ConductorSharp.Engine.Util.Builders;

namespace ConductorSharp.Engine.Builders
{
    /// <summary>
    /// Builder for an individual fork branch containing a sequence of tasks
    /// </summary>
    public class ForkBranchBuilder<TWorkflow> : ITaskSequenceBuilder<TWorkflow>
        where TWorkflow : ITypedWorkflow
    {
        private readonly List<ITaskBuilder> _taskBuilders = new();

        public BuildContext BuildContext { get; } = new();
        public BuildConfiguration BuildConfiguration { get; }
        public WorkflowBuildItemRegistry WorkflowBuildRegistry { get; } = new();
        public IEnumerable<ConfigurationProperty> ConfigurationProperties { get; } = new List<ConfigurationProperty>();

        public ForkBranchBuilder(BuildConfiguration buildConfiguration)
        {
            BuildConfiguration = buildConfiguration;
        }

        public void AddTaskBuilderToSequence(ITaskBuilder builder)
        {
            _taskBuilders.Add(builder);
        }

        /// <summary>
        /// Builds all tasks in this fork branch
        /// </summary>
        /// <returns>Array of workflow tasks for this branch</returns>
        public WorkflowTask[] BuildBranch()
        {
            return _taskBuilders.SelectMany(builder => builder.Build()).ToArray();
        }
    }
}
