using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeadlessBrowserAnalyzer
{
    /// <summary>
    /// Data model for extracted page resources
    /// </summary>
    public class PageResources
    {
        public string HtmlContent { get; set; } = string.Empty;
        public string PageTitle { get; set; } = string.Empty;
        public Dictionary<string, string> ResponseHeaders { get; set; } = new();
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public CacheStatus CacheStatus { get; set; } = CacheStatus.Unknown;
        public List<string> CssFiles { get; set; } = new();
        public List<string> InlineStyles { get; set; } = new();
        public List<string> JavaScriptFiles { get; set; } = new();
        public List<string> InlineScripts { get; set; } = new();
    }

    /// <summary>
    /// Parses HTML content and extracts CSS and JavaScript dependencies
    /// </summary>
    public class HtmlParser
    {
        /// <summary>
        /// Parses HTML content and extracts resources
        /// </summary>
        /// <param name="htmlContent">The HTML content to parse</param>
        /// <returns>PageResources object containing extracted data</returns>
    /// <summary>
    /// Parses HTML content and extracts resources
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse</param>
    /// <param name="responseHeaders">HTTP response headers</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="statusMessage">HTTP status message</param>
    /// <returns>PageResources object containing extracted data</returns>
    public static PageResources ParseHtmlContent(string htmlContent, Dictionary<string, string>? responseHeaders = null, int statusCode = 0, string statusMessage = "")
    {
        var resources = new PageResources 
        { 
            HtmlContent = htmlContent,
            ResponseHeaders = responseHeaders ?? new Dictionary<string, string>(),
            StatusCode = statusCode,
            StatusMessage = statusMessage
        };

        try
        {
            var context = BrowsingContext.New();
            var document = context.OpenAsync(req => req.Content(htmlContent)).Result;
            
            try
            {
                // Extract page title
                var titleElement = document.QuerySelector("title");
                if (titleElement != null)
                {
                    resources.PageTitle = titleElement.TextContent?.Trim() ?? "No title";
                }

                // Extract CSS files from <link rel="stylesheet"> tags
                ExtractCssFiles(document, resources);

                // Extract JavaScript files from <script src=""> tags
                ExtractJavaScriptFiles(document, resources);

                // Extract inline styles
                ExtractInlineStyles(document, resources);

                // Extract inline scripts
                ExtractInlineScripts(document, resources);
            }
            finally
            {
                document?.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error parsing HTML: {ex.Message}");
        }

        resources.CacheStatus = CacheStatusDetector.Detect(resources.ResponseHeaders);

        return resources;
    }

        /// <summary>
        /// Extracts CSS file references from link tags
        /// </summary>
        private static void ExtractCssFiles(IDocument document, PageResources resources)
        {
            var linkElements = document.QuerySelectorAll("link[rel='stylesheet'], link[rel='Stylesheet']");
            
            foreach (var linkElement in linkElements)
            {
                var href = linkElement.GetAttribute("href");
                if (!string.IsNullOrWhiteSpace(href))
                {
                    resources.CssFiles.Add(href);
                }
            }

            // Remove duplicates
            resources.CssFiles = resources.CssFiles.Distinct().ToList();
        }

        /// <summary>
        /// Extracts JavaScript file references from script tags with src attribute
        /// </summary>
        private static void ExtractJavaScriptFiles(IDocument document, PageResources resources)
        {
            var scriptElements = document.QuerySelectorAll("script[src]");
            
            foreach (var scriptElement in scriptElements)
            {
                var src = scriptElement.GetAttribute("src");
                if (!string.IsNullOrWhiteSpace(src))
                {
                    resources.JavaScriptFiles.Add(src);
                }
            }

            // Remove duplicates
            resources.JavaScriptFiles = resources.JavaScriptFiles.Distinct().ToList();
        }

        /// <summary>
        /// Extracts inline style tags
        /// </summary>
        private static void ExtractInlineStyles(IDocument document, PageResources resources)
        {
            var styleElements = document.QuerySelectorAll("style");
            
            foreach (var styleElement in styleElements)
            {
                var content = styleElement.TextContent;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    // Store first 100 characters of each inline style for reference
                    var snippet = content.Substring(0, Math.Min(100, content.Length)).Replace("\n", " ").Trim();
                    resources.InlineStyles.Add($"[{snippet}...]");
                }
            }
        }

        /// <summary>
        /// Extracts inline script tags
        /// </summary>
        private static void ExtractInlineScripts(IDocument document, PageResources resources)
        {
            var scriptElements = document.QuerySelectorAll("script:not([src])");
            
            foreach (var scriptElement in scriptElements)
            {
                var content = scriptElement.TextContent;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    // Store first 100 characters of each inline script for reference
                    var snippet = content.Substring(0, Math.Min(100, content.Length)).Replace("\n", " ").Trim();
                    resources.InlineScripts.Add($"[{snippet}...]");
                }
            }
        }
    }
}
