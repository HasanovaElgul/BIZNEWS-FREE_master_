namespace BIZNEWS_FREE.Helpers
{
    public static class FileHelper
    {
        public static async  Task<string> SaveFileAsync(IFormFile file, string WebRootPath)
        {
            string path = Path.Combine("/uploads/", Guid.NewGuid() + file.FileName);
            using FileStream fileStream = new(WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return path;

        }

    }
}
//eksteysen metoda cevirmek ucun this istifade olunur