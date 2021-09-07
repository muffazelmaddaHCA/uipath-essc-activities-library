using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using eSSCParallon.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace eSSCParallon.Activities
{
    [LocalizedDisplayName(nameof(Resources.PALoadNTLM_DisplayName))]
    [LocalizedDescription(nameof(Resources.PALoadNTLM_Description))]
    public class PALoadNTLM : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_InUserID_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_InUserID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> InUserID { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_InPassword_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_InPassword_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> InPassword { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_InURL_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_InURL_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> InURL { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_InDomain_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_InDomain_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> InDomain { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_InData_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_InData_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> InData { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_OutResponse_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_OutResponse_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> OutResponse { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_OutSatusCD_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_OutSatusCD_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> OutSatusCD { get; set; }

        [LocalizedDisplayName(nameof(Resources.PALoadNTLM_OutMessage_DisplayName))]
        [LocalizedDescription(nameof(Resources.PALoadNTLM_OutMessage_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> OutMessage { get; set; }

        #endregion


        #region Constructors

        public PALoadNTLM()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (InUserID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InUserID)));
            if (InPassword == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InPassword)));
            if (InURL == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InURL)));
            if (InDomain == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InDomain)));
            if (InData == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InData)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var inuserid = InUserID.Get(context);
            var inpassword = InPassword.Get(context);
            var inurl = InURL.Get(context);
            var indomain = InDomain.Get(context);
            var indata = InData.Get(context);

            string returnResponse = "";
            string returnStatus = "";
            string returnMessage = "";

            var credentialCache = new CredentialCache { { new Uri(inurl), "NTLM", new NetworkCredential(inuserid, inpassword, indomain) } };
            var handler = new HttpClientHandler { Credentials = credentialCache };
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response = null;
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = await client.PostAsync(inurl, new StringContent(indata, Encoding.UTF8, "application/json"));
                    var contents = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        returnResponse = "SUCCESS";
                        returnStatus = ((int)response.StatusCode).ToString();
                        returnMessage = contents.ToString();
                    }
                    else
                    {
                        returnResponse = "FAILURE";
                        returnStatus = ((int)response.StatusCode).ToString();
                        returnMessage = contents.ToString();
                    }
                }
                catch (Exception ex)
                {
                    returnResponse = "FAILURE";
                    returnStatus = "9999";
                    returnMessage = ex.ToString();
                }

                // Outputs
                return (ctx) =>
                {
                    OutResponse.Set(ctx, returnResponse);
                    OutSatusCD.Set(ctx, returnStatus);
                    OutMessage.Set(ctx, returnMessage);
                };
            }
        }

        #endregion
    }
}

