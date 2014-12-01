#region

using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using Twilio.Mvc;
using Twilio.TwiML;

#endregion

namespace Twilio3.ApiControllers
{
    public class ConferenceController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Call(TwilioRequest voiceRequest)
        //  api/conference/call
        {
            var twilioResponse = new TwilioResponse();

            var callUrl= WebConfigurationManager.AppSettings.Get("callUrl");
            var userUrl= WebConfigurationManager.AppSettings.Get("userUrl");

            twilioResponse.BeginGather(new {action = userUrl, finishOnKey = "#", method = "POST"});

            twilioResponse.Say(@"
                Welcome to the Prayer Line of Fellowship Mission Church. Please press 1 at any time to join the Prayer Line. 
Our Congregation is centered between the historic Fort Hill neighborhood and directly behind Roxbury Community College.
We are steps away from the Orange Line Roxbury Crossing T Station. Our direct address is 85 Centre Street, Roxbury, 02119.

                  
                Press 3 to join as a prayer coordinator",
                new {voice = "woman", numOfDigits = 1});
            //Press 2 to join as speaker,

            twilioResponse.EndGather();

            twilioResponse.Say(@"
                We did not receive a response", new { voice = "woman" });

            twilioResponse.Redirect(callUrl);

            return CreateResponseMessage(twilioResponse);
        }

        [HttpPost]
        public HttpResponseMessage User(VoiceRequest voiceRequest)
        {
            var twilioResponse = new TwilioResponse();

            switch (voiceRequest.Digits)
            {
                case "1":
                    twilioResponse.DialConference("myRoom", new {muted = true, startConferenceOnEnter = false});
                    break;
                case "2":
                    twilioResponse.DialConference("myRoom", new {startConferenceOnEnter = false});
                    break;
                case "3":
                    twilioResponse.DialConference("myRoom", new { startConferenceOnEnter = true, endConferenceOnExit = true, muted = false });
                    break;
                //case "3":
                //   twilioResponse.DialConference("myRoom", new { startConferenceOnEnter = true, endConferenceOnExit = true, muted = false });
                //    break;
            }

            return CreateResponseMessage(twilioResponse);
        }

        private HttpResponseMessage CreateResponseMessage(TwilioResponse twilioResponse)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(twilioResponse.ToString(), Encoding.UTF8, "text/xml")
            };
        }
    }
}