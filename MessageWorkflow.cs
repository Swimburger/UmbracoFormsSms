using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace UmbracoSite;

public class MessageWorkflow : WorkflowType
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    [Setting("From", Description = "The Twilio Phone Number the message is sent from.", View = "DropdownList")]
    public string FromPhoneNumber { get; set; }

    [Setting("To", Description = "The phone number the message is sent to.", View = "TextField")]
    public string ToPhoneNumber { get; set; }

    [Setting("Message", Description = "The body of the text message.", View = "TextField")]
    public string MessageBody { get; set; }

    [Setting("Media URL", Description = "URL of the media file to sent as an MMS.", View = "TextField")]
    public string MediaUrl { get; set; }

    public MessageWorkflow(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;

        this.Id = new Guid("9e44e413-afa0-4a1e-a40d-7d10e7c7f2b5");
        this.Name = "SMS";
        this.Description = "This workflow will send an SMS or MMS.";
        this.Icon = "icon-chat-active";
        this.Group = "Communication";
    }

    public override Dictionary<string, Setting> Settings()
    {
        var settings = base.Settings();

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var twilioClient = scope.ServiceProvider.GetRequiredService<ITwilioRestClient>();
            var incomingPhoneNumbers = IncomingPhoneNumberResource.Read(client: twilioClient);
            var preValues = string.Join(",", incomingPhoneNumbers.Select(p => p.PhoneNumber.ToString()));
            settings[nameof(FromPhoneNumber)].PreValues = preValues;
        }
        return settings;
    }

    public override WorkflowExecutionStatus Execute(WorkflowExecutionContext context)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var twilioClient = scope.ServiceProvider.GetRequiredService<ITwilioRestClient>();
            var messageOptions = new CreateMessageOptions(to: new PhoneNumber(ToPhoneNumber))
            {
                From = new PhoneNumber(FromPhoneNumber)
            };

            if (string.IsNullOrEmpty(MessageBody) == false)
            {
                messageOptions.Body = MessageBody;
            }

            if (string.IsNullOrEmpty(MediaUrl) == false)
            {
                messageOptions.MediaUrl = new List<Uri> {new Uri(MediaUrl)};
            }

            MessageResource.Create(messageOptions, client: twilioClient);
        }

        context.Record.State = FormState.Approved;
        return WorkflowExecutionStatus.Completed;
    }

    public override List<Exception> ValidateSettings()
    {
        return new List<Exception>();
    }
}