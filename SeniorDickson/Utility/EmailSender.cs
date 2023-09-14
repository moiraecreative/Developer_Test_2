using Moirae.ViewModels;
using System.Net.Mail;
using System.Runtime.InteropServices;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Core.Services;

namespace Moirae.Utility
{
	public static class EmailSender
	{
		public static void SendEmail(MailMessage message)
		{
			using (var client = new SmtpClient())
			{
				client.Host = "84.18.208.150";
				client.Credentials = new System.Net.NetworkCredential("test@moiraelive.co.uk", "Password1234*");
				client.Send(message);
			}
		}
	}
}
