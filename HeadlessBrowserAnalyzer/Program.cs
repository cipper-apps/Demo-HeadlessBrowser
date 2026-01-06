using HeadlessBrowserAnalyzer;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║   Headless Browser Analyzer (.NET 10)  ║");
        Console.WriteLine("╚════════════════════════════════════════╝\n");

        var browser = new BrowserAutomation();

        try
        {
            // Initialize browser
            await browser.InitializeAsync();

            // Main application loop
            bool continueSession = true;
            while (continueSession)
            {
                // Get URL from user
                Console.Write("\nEnter a URL (or 'exit' to quit): ");
                string? urlInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(urlInput) || urlInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    continueSession = false;
                    break;
                }

                // Validate and normalize URL
                if (!UrlValidator.ValidateAndNormalizeUrl(urlInput, out string validatedUrl))
                {
                    continue;
                }

                try
                {
                    // Extract page data using Playwright
                    PageResponse pageResponse = await browser.ExtractPageDataAsync(validatedUrl);

                    // Parse HTML and extract resources
                    PageResources resources = HtmlParser.ParseHtmlContent(
                        pageResponse.HtmlContent,
                        pageResponse.ResponseHeaders,
                        pageResponse.StatusCode,
                        pageResponse.StatusMessage
                    );

                    // Display formatted results
                    ResultsFormatter.DisplayResults(validatedUrl, resources);

                    // Ask user if they want to save full HTML
                    Console.Write("\nSave full HTML to file? (y/n): ");
                    string? saveChoice = Console.ReadLine();
                    if (saveChoice?.Equals("y", StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        string fileName = $"page_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                        ResultsFormatter.SaveHtmlToFile(fileName, pageResponse.HtmlContent);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"\n❌ Error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Details: {ex.InnerException.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Unexpected error: {ex.Message}");
                }
            }

            Console.WriteLine("\nThank you for using Headless Browser Analyzer!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n❌ Initialization Error: {ex.Message}");
            Console.WriteLine("Make sure Playwright browsers are installed by running:");
            Console.WriteLine("  pwsh bin/Debug/net10.0/playwright.ps1 install");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Fatal Error: {ex.Message}");
            Environment.Exit(1);
        }
        finally
        {
            // Clean up browser resources
            await browser.CloseAsync();
        }
    }
}
