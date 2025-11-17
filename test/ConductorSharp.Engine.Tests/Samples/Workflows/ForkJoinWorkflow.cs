using System.Collections.Generic;
using ConductorSharp.Client.Generated;

namespace ConductorSharp.Engine.Tests.Samples.Workflows
{
    public class ForkJoinWorkflowInput : WorkflowInput<ForkJoinWorkflowOutput>
    {
        public int CustomerId { get; set; }
    }

    public class ForkJoinWorkflowOutput : WorkflowOutput
    {
        public object CustomerData { get; set; }
        public object EmailData { get; set; }
    }

    public class ForkJoinWorkflow : Workflow<ForkJoinWorkflow, ForkJoinWorkflowInput, ForkJoinWorkflowOutput>
    {
        public ForkJoinTaskModel ForkJoinTask { get; set; }

        public ForkJoinWorkflow(WorkflowDefinitionBuilder<ForkJoinWorkflow, ForkJoinWorkflowInput, ForkJoinWorkflowOutput> builder)
            : base(builder) { }

        public override void BuildDefinition()
        {
            _builder.AddTask(
                wf => wf.ForkJoinTask,
                wf =>
                    new ForkJoinInput()
                    {
                        ForkTasks = new[]
                        {
                            new WorkflowTask[]
                            {
                                new()
                                {
                                    Name = "get_customer_task",
                                    TaskReferenceName = "get_customer_ref",
                                    Type = "SIMPLE",
                                    WorkflowTaskType = WorkflowTaskType.SIMPLE,
                                    InputParameters = new Dictionary<string, object> { { "customerId", wf.WorkflowInput.CustomerId } }
                                }
                            },
                            new WorkflowTask[]
                            {
                                new()
                                {
                                    Name = "prepare_email_task",
                                    TaskReferenceName = "prepare_email_ref",
                                    Type = "SIMPLE",
                                    WorkflowTaskType = WorkflowTaskType.SIMPLE,
                                    InputParameters = new Dictionary<string, object> { { "templateId", "welcome" } }
                                }
                            }
                        },
                        JoinOn = new[] { "get_customer_ref", "prepare_email_ref" }
                    }
            );
        }
    }
}
