using Twilio.AspNet.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Providers;

namespace UmbracoSite;

public class SmsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTwilioClient();
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<SmsWorkflow>();
    }
}