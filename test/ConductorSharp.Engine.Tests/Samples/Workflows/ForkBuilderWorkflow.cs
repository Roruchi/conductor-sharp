using ConductorSharp.Engine.Tests.Samples.Tasks;

namespace ConductorSharp.Engine.Tests.Samples.Workflows
{
    public class ForkBuilderWorkflowInput : WorkflowInput<ForkBuilderWorkflowOutput>
    {
        public int CustomerId { get; set; }
    }

    public class ForkBuilderWorkflowOutput : WorkflowOutput
    {
        public object CustomerData { get; set; }
        public object EmailData { get; set; }
    }

    /// <summary>
    /// Test workflow demonstrating the new Fork builder pattern
    /// </summary>
    public class ForkBuilderWorkflow : Workflow<ForkBuilderWorkflow, ForkBuilderWorkflowInput, ForkBuilderWorkflowOutput>
    {
        public ForkJoinTaskModel ParallelTasks { get; set; }

        public ForkBuilderWorkflow(WorkflowDefinitionBuilder<ForkBuilderWorkflow, ForkBuilderWorkflowInput, ForkBuilderWorkflowOutput> builder)
            : base(builder) { }

        public override void BuildDefinition()
        {
            // Using the new Fork builder pattern
            _builder.Fork(
                wf => wf.ParallelTasks,
                forkBuilder =>
                {
                    // First branch: Get customer information
                    forkBuilder.AddFork(taskBuilder =>
                    {
                        taskBuilder.AddTask(wf => wf.CustomerGet, wf => new CustomerGetV1Input { CustomerId = wf.WorkflowInput.CustomerId });
                    });

                    // Second branch: Prepare email template
                    forkBuilder.AddFork(taskBuilder =>
                    {
                        taskBuilder.AddTask(
                            wf => wf.EmailPrepare,
                            wf => new EmailPrepareV1Input { Name = "Default Name", Address = "Default Address" }
                        );
                    });
                }
            );
        }

        // Task properties for the workflow
        public CustomerGetV1 CustomerGet { get; set; }
        public EmailPrepareV1 EmailPrepare { get; set; }
    }
}
