namespace BIZNEWS_FREE.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int MyProperty { get; set; }
        public List<Article> Article { get; set; }
    }
}
