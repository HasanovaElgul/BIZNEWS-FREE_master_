using BIZNEWS_FREE.Models;

namespace BIZNEWS_FREE.Areas.Admin.ViewModels
{
	public class UserVM
	{
		public User User { get; set; }
		public IEnumerable<string> Roles { get; set; }
		public string ErrorMessage { get; set; }
		public List<string> UserRoles { get; internal set; }
	}
}

//IEnumerable   -  yalniz oxumaq ucun  get edib baxa bilirik
//ICollection   - add de remove ede bilerik
//IList - List  - 
