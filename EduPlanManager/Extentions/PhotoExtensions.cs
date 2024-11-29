namespace EduPlanManager.Extentions
{
    public static class PhotoExtensions
    {
        public static string ExtractPublicId(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("Image URL cannot be null or empty.", nameof(imageUrl));
            }

            var uri = new Uri(imageUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
            {
                throw new InvalidOperationException("Invalid image URL format.");
            }
            var publicIdWithVersion = segments[segments.Length - 1];
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithVersion);

            return publicId;
        }
    }
}
