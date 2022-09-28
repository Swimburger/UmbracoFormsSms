using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;

namespace UmbracoSite;

public class SmsWorkflow : WorkflowType
{
    private readonly ILogger<SmsWorkflow> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;

    [Setting("From", Description = "The Twilio Phone Number the SMS is sent from.", View = "TextField")]
    public string FromPhoneNumber { get; set; }

    [Setting("To", Description = "The Phone Number the SMS is sent to.", View = "TextField")]
    public string ToPhoneNumber { get; set; }

    [Setting("Message", Description = "The body of the text message.", View = "TextField")]
    public string SmsBody { get; set; }


    public SmsWorkflow(
        ILogger<SmsWorkflow> logger, 
        IServiceScopeFactory serviceScopeFactory
    )
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;

        this.Id = new Guid("9e44e413-afa0-4a1e-a40d-7d10e7c7f2b5");
        this.Name = "SMS";
        this.Description = "This workflow will send an SMS.";
        this.Icon = "icon-chat-active";
        this.Group = "Communication";
    }

    public override WorkflowExecutionStatus Execute(WorkflowExecutionContext context)
    {
        using(var scope = serviceScopeFactory.CreateScope()){
            var twilioClient = scope.ServiceProvider.GetRequiredService<ITwilioRestClient>();
            MessageResource.Create(
                from: new PhoneNumber(FromPhoneNumber),
                to: new PhoneNumber(ToPhoneNumber),
                body: SmsBody,
                client: twilioClient
            );
        }
        
        context.Record.State = FormState.Approved;
        return WorkflowExecutionStatus.Completed;
    }

    public override List<Exception> ValidateSettings()
    {
        return new List<Exception>();
    }
}