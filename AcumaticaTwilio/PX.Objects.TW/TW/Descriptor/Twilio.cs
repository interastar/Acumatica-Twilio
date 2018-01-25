using System;
using System.Collections.Specialized;
using System.Linq;
using PX.Data;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PX.Objects.TW
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// NameValueCollection extension that transforms the collection into a string with Uri encoded values
        /// for an HTTP Request
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection collection, string startChar = "?")
        {
            var array = (from key in collection.AllKeys
                         from value in collection.GetValues(key)
                         select string.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value))).ToArray();

            return startChar + string.Join("&", array);
        }
    }

    public class TwilioNotification
    {
        private static string TwimletBase = "http://twimlets.com/message";

        public string Origin { get; set; }

        /// <summary>
        /// Initializes the Twilio client with credentials
        /// </summary>
        /// <param name="sid">Your Account SID from twilio.com/console</param>
        /// <param name="token">Your Auth Token from twilio.com/console</param>
        public TwilioNotification(string sid, string token)
        {
            TwilioClient.Init(sid, token);
        }

        /// <summary>
        /// Builds Twimlet URL to the message twimlet to generate TwiML for a voice message
        /// </summary>
        /// <param name="messages">Text to say</param>
        /// <returns></returns>
        public static string MessageUrl(params string[] messages)
        {
            var messageCollection = new NameValueCollection();

            for (int i = 0; i < messages.Length; i++)
            {
                messageCollection.Add("Message[" + i + "]", messages[i]);
            }

            return TwimletBase + messageCollection.ToQueryString();
        }

        /// <summary>
        /// Sends an SMS message to the specified number
        /// </summary>
        /// <param name="number">Destination number</param>
        /// <param name="message">Message text</param>
        public void SendSMS(string number, string message)
        {
            try
            {
                var msg = MessageResource.Create(
                    to: new PhoneNumber(number),
                    from: new PhoneNumber(Origin),
                    body: message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Makes a call to specified number and says the message
        /// </summary>
        /// <param name="number">Destination number</param>
        /// <param name="message">Message text</param>
        public void SendCall(string number, string message)
        {
            try
            {
                var msg = CallResource.Create(
                to: new PhoneNumber(number),
                from: new PhoneNumber(Origin),
                url: new Uri(MessageUrl(message)));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}