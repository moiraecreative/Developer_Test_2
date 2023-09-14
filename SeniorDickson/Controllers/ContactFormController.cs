using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Moirae.ViewModels;
using Umbraco.Cms.Web.Common.Filters;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using Razor.Templating.Core;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
using System.Reflection.Metadata.Ecma335;
using Moirae.Utility;
using Umbraco.Cms.Core.Events;
using Microsoft.CodeAnalysis.CSharp;
using Org.BouncyCastle.Crypto.IO;

namespace Moirae.Controllers
{
	public class ContactFormController : SurfaceController
	{
		IContentService _contentService;
		IUmbracoContextFactory _contextFactory;

		public ContactFormController(
			IUmbracoContextAccessor umbracoContextAccessor,
			IUmbracoDatabaseFactory databaseFactory,
			ServiceContext services,
			AppCaches appCaches,
			IProfilingLogger profilingLogger,
			IPublishedUrlProvider publishedUrlProvider,
			IContentService contentService,
			IUmbracoContextFactory contextFactory)
			: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
			_contentService = contentService;
			_contextFactory = contextFactory;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ValidateUmbracoFormRouteString]
		public async Task<IActionResult> Submit(ContactFormViewModel model)
		{
			#region Verify Model

			// This checks whether the model is valid, it will return the same pae with the errors if there is an issue.
			// Temp data is for quick debugging.

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

				foreach (var error in errors)
				{
					TempData["Errors"] = error;
				}
				return CurrentUmbracoPage();
			}

			#endregion

			#region Front End Access

			// In here we access the umbraco front end to get some variables to put into emails.
			// This includes logo, email sender and in house recipients of new notifications.

			string imageLink = string.Empty;
			string sender = "noreply@moirae.co.uk";
			Contact contactForm = null;
			List<string> notificationRecipients = new List<string>();

			using (var umbracoContextReference = _contextFactory.EnsureUmbracoContext())
			{
				IPublishedContent content = umbracoContextReference.UmbracoContext.Content.GetAtRoot().FirstOrDefault();

				if (content != null)
				{
					Master master = content as Master;
					sender = (!String.IsNullOrEmpty(master.NotificationSender)) ? master.NotificationSender : sender;
					imageLink = master.EmailLogo.Url(mode: UrlMode.Absolute);

					foreach (Recipient recipient in master.NotificationList.Children())
					{
						notificationRecipients.Add(recipient.Email);
					}

					contactForm = master.ContactForm as Contact;
				}
			}
            #endregion

            #region File Handling 

            string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            string file = string.Empty;

            if (model.File != null)
            {
                string fileName = model.File.FileName;
                string uniqueFileName = GetUniqueFileName(fileName);

                file = "/uploads/" + uniqueFileName;
                filePath = System.IO.Path.Combine(filePath, uniqueFileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.File.CopyToAsync(stream);
                }
            }

            #endregion

            #region Client Notification

            //This notification will be sent to the recipients set in the recipient list.
            //If there are none, it will send to moirae support instead.
            try
			{
				using (MailMessage message = new MailMessage())
				{
					message.IsBodyHtml = true;
					message.Sender = new MailAddress(sender);
					EmailViewModel emailModel = new EmailViewModel()
					{
						Title = "Contact Form Submission",
						Name = model.Name,
						Email = model.Email,
						Company = model.Company,
						Message = model.Message,
						Phone = model.Phone,
						ImageLink = imageLink,
					};

					if (notificationRecipients.Count() > 0)
					{
						foreach (var recipient in notificationRecipients)
						{
							message.To.Add(new MailAddress(recipient));
						}
					}
					else
					{
						message.Subject = "ALERT - SENIOR DICKSON";
						message.Priority = MailPriority.High;
						emailModel.Title = "No Notification Recipients Set for Forms";
						message.To.Add(new MailAddress("support@moirae.co.uk"));
					}

					message.Subject = emailModel.Title;

					if (!String.IsNullOrEmpty(file))
					{
						try
						{
							using (var ms = new MemoryStream())
							{
								model.File.CopyTo(ms);
								var fileBytes = ms.ToArray();
								Attachment att = new Attachment(new MemoryStream(fileBytes), model.File.FileName);
								message.Attachments.Add(att);
							}
						} catch (Exception ex)
						{
							Console.Error.WriteLine("Failed to attach File: " + ex.Message);
						}
						
					}
					message.From = new MailAddress(sender);
					message.Body = await RazorTemplateEngine.RenderAsync("Notification.cshtml", emailModel);
					EmailSender.SendEmail(message);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Failed to send Client Notification: " + ex.Message);
			}
			#endregion

			#region User Notification
			try
			{
				using (MailMessage message = new MailMessage())
				{
					message.IsBodyHtml = true;
					message.Sender = new MailAddress(sender);
					EmailViewModel emailModel = new EmailViewModel()
					{
						Title = "Thanks for your submission",
						Name = model.Name,
						ImageLink = imageLink
					};
					message.From = new MailAddress(sender);
					message.To.Add(new MailAddress(model.Email));
					message.Subject = emailModel.Title;
					message.Body = await RazorTemplateEngine.RenderAsync("Email.cshtml", emailModel);
					EmailSender.SendEmail(message);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Failed to send User Notification: " + ex.Message);
			}
			#endregion

			#region Capture in Umbraco

			try
			{
				var submission = _contentService.Create(model.Name + " | Form Submission", contactForm.Id, "formSubmission", -1);
				submission.SetValue("sender", model.Name);
				submission.SetValue("phoneNumber", model.Phone);
				submission.SetValue("email", model.Email);
				submission.SetValue("company", model.Company);
				submission.SetValue("message", model.Message);
				submission.SetValue("file", file);
				_contentService.SaveAndPublish(submission);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Failed to capture in back office: " + ex.Message);
				TempData["Errors"] = "Something went wrong :(";
				return CurrentUmbracoPage();
			}

			#endregion

			return Redirect("/thank-you");
		}

        private string GetUniqueFileName(string fileName)
        {
            string uniqueIdentifier = Guid.NewGuid().ToString().Substring(0, 8); // Generate a unique identifier
            string extension = System.IO.Path.GetExtension(fileName); // Get the file extension
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName); // Get the file name without extension

            // Combine the unique identifier, file name, and extension to create a new unique file name
            string uniqueFileName = $"{fileNameWithoutExtension}_{uniqueIdentifier}{extension}";

            return uniqueFileName;
        }

    }
}
