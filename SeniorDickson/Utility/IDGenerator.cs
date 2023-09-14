using Moirae.ViewModels;
using System.Net.Mail;
using System.Runtime.InteropServices;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Core.Services;

namespace Moirae.Utility
{
	public static class IDGenerator
	{
		public static string GenerateID()
		{
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var pass = string.Empty;
			Random rnd = new Random();

			for (int i = 0; i < 10; i++)
			{
				var c = chars[rnd.Next(chars.Length)];
				pass = pass + c;
			}

			return pass;
		}
	}
}