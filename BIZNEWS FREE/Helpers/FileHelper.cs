namespace BIZNEWS_FREE.Helpers
{    /// <summary>
/// 
/// bu bir file yuklemek sistemidir
/// </summary>
    public static class FileHelper
    {              /// <summary>
    /// 
    /// Bu bir file yukleme metodudur asinxrondu
    /// </summary>
    /// <param name="file">iformFile tipinden nese</param>
    /// <param name="WebRootPath"></param>
    /// <param name="folderName">folderin adi</param>
    /// <returns></returns>
        public static async  Task<string> SaveFileAsync(this IFormFile file, string WebRootPath, string folderName)
        {
            if (!Directory.Exists(WebRootPath + folderName))
            { 
              Directory.CreateDirectory(WebRootPath + folderName);
            }
              
            string path = Path.Combine(folderName, Guid.NewGuid() + file.FileName);
            using FileStream fileStream = new(WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return path;

        }

    }
}
//eksteysen metoda cevirmek ucun this istifade olunur