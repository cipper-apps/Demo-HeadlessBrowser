using System;
using System.Text;

namespace HeadlessBrowserAnalyzer
{
    /// <summary>
    /// Formats and displays the extracted page resources
    /// </summary>
    public class ResultsFormatter
    {
        /// <summary>
        /// Displays the extracted resources in a formatted manner
        /// </summary>
        /// <param name="url">The URL that was analyzed</param>
        /// <param name="resources">The extracted resources</param>
        public static void DisplayResults(string url, PageResources resources)
        {
            var output = new StringBuilder();
            
            output.AppendLine("\n" + new string('=', 80));
            output.AppendLine("HEADLESS BROWSER ANALYSIS RESULTS");
            output.AppendLine(new string('=', 80));
            
            output.AppendLine($"\nURL: {url}");
            output.AppendLine($"Page Title: {resources.PageTitle}");
            output.AppendLine($"Status: {resources.StatusCode} {resources.StatusMessage}");
            
            // Response Headers
            output.AppendLine("\n" + new string('-', 80));
            output.AppendLine("HTTP RESPONSE HEADERS");
            output.AppendLine(new string('-', 80));
            if (resources.ResponseHeaders.Count > 0)
            {
                foreach (var header in resources.ResponseHeaders.OrderBy(h => h.Key))
                {
                    output.AppendLine($"{header.Key}: {header.Value}");
                }
            }
            else
            {
                output.AppendLine("No response headers found.");
            }
            
            // CSS Summary
            output.AppendLine("\n" + new string('-', 80));
            output.AppendLine("CSS RESOURCES SUMMARY");
            output.AppendLine(new string('-', 80));
            int totalCssResources = resources.CssFiles.Count + resources.InlineStyles.Count;
            output.AppendLine($"Total CSS Resources: {totalCssResources} (External: {resources.CssFiles.Count}, Inline: {resources.InlineStyles.Count})");
            
            if (resources.CssFiles.Count > 0)
            {
                output.AppendLine($"\nExternal CSS Files ({resources.CssFiles.Count}):");
                for (int i = 0; i < resources.CssFiles.Count; i++)
                {
                    output.AppendLine($"  {i + 1}. {resources.CssFiles[i]}");
                }
            }
            
            if (resources.InlineStyles.Count > 0)
            {
                output.AppendLine($"\nInline Styles ({resources.InlineStyles.Count} <style> tags)");
            }
            
            // JavaScript Summary
            output.AppendLine("\n" + new string('-', 80));
            output.AppendLine("JAVASCRIPT RESOURCES SUMMARY");
            output.AppendLine(new string('-', 80));
            int totalJsResources = resources.JavaScriptFiles.Count + resources.InlineScripts.Count;
            output.AppendLine($"Total JavaScript Resources: {totalJsResources} (External: {resources.JavaScriptFiles.Count}, Inline: {resources.InlineScripts.Count})");
            
            if (resources.JavaScriptFiles.Count > 0)
            {
                output.AppendLine($"\nExternal JavaScript Files ({resources.JavaScriptFiles.Count}):");
                for (int i = 0; i < resources.JavaScriptFiles.Count; i++)
                {
                    output.AppendLine($"  {i + 1}. {resources.JavaScriptFiles[i]}");
                }
            }
            
            if (resources.InlineScripts.Count > 0)
            {
                output.AppendLine($"\nInline Scripts ({resources.InlineScripts.Count} <script> tags)");
            }
            
            output.AppendLine("\n" + new string('=', 80));
            output.AppendLine("END OF ANALYSIS");
            output.AppendLine(new string('=', 80) + "\n");
            
            Console.WriteLine(output.ToString());
        }

        /// <summary>
        /// Optionally save full HTML to file
        /// </summary>
        /// <param name="filePath">Path to save the HTML file</param>
        /// <param name="htmlContent">The HTML content to save</param>
        public static void SaveHtmlToFile(string filePath, string htmlContent)
        {
            try
            {
                System.IO.File.WriteAllText(filePath, htmlContent);
                Console.WriteLine($"Full HTML content saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving HTML file: {ex.Message}");
            }
        }
    }
}
