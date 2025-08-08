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
                        ForkTasks = new object[]
                        {
                            new object[]
                            {
                                new
                                {
                                    name = "get_customer_task",
                                    taskReferenceName = "get_customer_ref",
                                    type = "SIMPLE",
                                    inputParameters = new { customerId = wf.WorkflowInput.CustomerId }
                                }
                            },
                            new object[]
                            {
                                new
                                {
                                    name = "prepare_email_task",
                                    taskReferenceName = "prepare_email_ref",
                                    type = "SIMPLE",
                                    inputParameters = new { templateId = "welcome" }
                                }
                            }
                        },
                        JoinOn = new[] { "get_customer_ref", "prepare_email_ref" }
                    }
            );
        }
    }
}
