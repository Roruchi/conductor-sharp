using ConductorSharp.Engine.Tests.Samples.Tasks;

namespace ConductorSharp.Engine.Tests.Samples.Workflows
{
    /// <summary>
    /// Updated ForkJoinWorkflow showing migration from object-style to builder-style
    /// </summary>
    public class ForkJoinWorkflowNewStyle : Workflow<ForkJoinWorkflowNewStyle, ForkJoinWorkflowInput, ForkJoinWorkflowOutput>
    {
        public ForkJoinTaskModel ForkJoinTask { get; set; }

        // Required task properties
        public CustomerGetV1 GetCustomer { get; set; }
        public EmailPrepareV1 PrepareEmail { get; set; }

        public ForkJoinWorkflowNewStyle(WorkflowDefinitionBuilder<ForkJoinWorkflowNewStyle, ForkJoinWorkflowInput, ForkJoinWorkflowOutput> builder)
            : base(builder) { }

        public override void BuildDefinition()
        {
            // Using the new Fork builder pattern - cleaner and more maintainable
            _builder.Fork(
                wf => wf.ForkJoinTask,
                forkBuilder =>
                {
                    // First branch: Get customer information
                    forkBuilder.AddFork(taskBuilder =>
                    {
                        taskBuilder.AddTask(wf => wf.GetCustomer, wf => new CustomerGetV1Input { CustomerId = wf.WorkflowInput.CustomerId });
                    });

                    // Second branch: Prepare email template
                    forkBuilder.AddFork(taskBuilder =>
                    {
                        taskBuilder.AddTask(
                            wf => wf.PrepareEmail,
                            wf => new EmailPrepareV1Input { Name = "Default Name", Address = "Default Address" }
                        );
                    });
                }
            );
        }
    }

    /// <summary>
    /// Example showing multiple tasks within a single fork branch
    /// </summary>
    public class MultiBranchForkWorkflow : Workflow<MultiBranchForkWorkflow, ForkJoinWorkflowInput, ForkJoinWorkflowOutput>
    {
        public ForkJoinTaskModel ParallelProcessing { get; set; }

        // Task properties for sequential processing within branches
        public CustomerGetV1 GetCustomer { get; set; }
        public EmailPrepareV1 PrepareEmail { get; set; }
        public EmailPrepareV1 FinalizeEmail { get; set; }

        public MultiBranchForkWorkflow(WorkflowDefinitionBuilder<MultiBranchForkWorkflow, ForkJoinWorkflowInput, ForkJoinWorkflowOutput> builder)
            : base(builder) { }

        public override void BuildDefinition()
        {
            _builder.Fork(
                wf => wf.ParallelProcessing,
                forkBuilder =>
                {
                    // Branch 1: Simple customer retrieval
                    forkBuilder.AddFork(taskBuilder =>
                    {
                        taskBuilder.AddTask(wf => wf.GetCustomer, wf => new CustomerGetV1Input { CustomerId = wf.WorkflowInput.CustomerId });
                    });

                    // Branch 2: Sequential email processing (multiple tasks in one branch)
                    forkBuilder.AddFork(taskBuilder =>
                    {
                        taskBuilder.AddTask(
                            wf => wf.PrepareEmail,
                            wf => new EmailPrepareV1Input { Name = "Preparing...", Address = "temp@example.com" }
                        );

                        taskBuilder.AddTask(
                            wf => wf.FinalizeEmail,
                            wf => new EmailPrepareV1Input { Name = wf.PrepareEmail.Output.EmailBody, Address = "final@example.com" }
                        );
                    });
                }
            );
        }
    }
}
