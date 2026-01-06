using System;

namespace HeadlessBrowserAnalyzer
{
    /// <summary>
    /// Handles URL input validation and normalization
    /// </summary>
    public class UrlValidator
    {
        /// <summary>
        /// Validates and normalizes a URL string
        /// </summary>
        /// <param name="urlInput">Raw URL input from user</param>
        /// <param name="validatedUrl">Output parameter with the validated URL</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateAndNormalizeUrl(string urlInput, out string validatedUrl)
        {
            validatedUrl = string.Empty;

            // Check for null or empty input
            if (string.IsNullOrWhiteSpace(urlInput))
            {
                Console.WriteLine("Error: URL cannot be empty.");
                return false;
            }

            // Trim whitespace
            urlInput = urlInput.Trim();

            // Add scheme if missing
            if (!urlInput.StartsWith("http://") && !urlInput.StartsWith("https://"))
            {
                urlInput = "https://" + urlInput;
            }

            // Try to parse as Uri
            if (!Uri.TryCreate(urlInput, UriKind.Absolute, out Uri? uri))
            {
                Console.WriteLine($"Error: Invalid URL format: {urlInput}");
                return false;
            }

            // Validate that it's http or https
            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                Console.WriteLine($"Error: Only HTTP and HTTPS URLs are supported. Got: {uri.Scheme}");
                return false;
            }

            validatedUrl = uri.AbsoluteUri;
            return true;
        }
    }
}
