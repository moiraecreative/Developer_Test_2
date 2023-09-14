using System.ComponentModel.DataAnnotations;

namespace Moirae.ViewModels
{
	public class EmailViewModel
	{
		public string Title { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }

		public string Email { get; set; }

		public string Company { get; set; }
		public string ImageLink { get; set; }

		public string Message { get; set; }

	}
}
