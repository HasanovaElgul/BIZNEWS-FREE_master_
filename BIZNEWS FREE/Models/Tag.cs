namespace BIZNEWS_FREE.Models
{
	public class Tag
	{
		public int Id { get; set; }
		public string TagName { get; set; }

		public List<ArticleTag> ArticleTags { get; set; }
	}
}



