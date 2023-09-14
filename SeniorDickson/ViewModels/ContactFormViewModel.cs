using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moirae.ViewModels
{
	public class ContactFormViewModel
	{
		[Required]
		[Display(Name = "Name*")]
		public string Name { get; set; }

		[Required]
		[Phone]
		[Display(Name = "Phone Number*")]
		public string Phone { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email*")]
		public string Email { get; set; }

		public string? Company { get; set; }

		[Display(Name = "Supporting File")]
		public IFormFile? File { get; set; }

		[Required]
		[Display(Name = "Message*")]
		public string Message { get; set; }
	}

}