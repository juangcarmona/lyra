using System.Text.RegularExpressions;

public static class FileNameSanitizer
{
    public static string SanitizeFileName(string fileName, int maxLength = 255)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Filename cannot be empty or whitespace.", nameof(fileName));

        // 1. Remove invalid characters for any file system
        string sanitized = Regex.Replace(fileName, @"[<>:""/\\|?*\x00-\x1F]", "_");

        // 2. Remove problematic characters specific to some operating systems
        sanitized = Regex.Replace(sanitized, @"[\p{C}]", ""); // Invisible control characters
        sanitized = sanitized.Replace("..", "_"); // Avoid problems with relative paths
        sanitized = sanitized.Replace("~", "_"); // Avoid problems on UNIX

        // 3. Replace multiple spaces with a single underscore
        sanitized = Regex.Replace(sanitized, @"\s+", "_");

        // 4. Remove any leading or trailing dots to avoid problems with Unix and Windows
        sanitized = sanitized.Trim('.');

        // 5. Limit the maximum length to that allowed by common file systems
        if (sanitized.Length > maxLength)
            sanitized = sanitized.Substring(0, maxLength);

        // 6. Convert to lowercase to avoid case sensitivity issues on some systems
        return sanitized.ToLower();
    }
}