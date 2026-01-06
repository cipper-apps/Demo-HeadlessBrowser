using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessBrowserAnalyzer
{
    /// <summary>
    /// Data model for page response information
    /// </summary>
    public class PageResponse
    {
        public string HtmlContent { get; set; } = string.Empty;
        public Dictionary<string, string> ResponseHeaders { get; set; } = new();
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Handles Playwright browser automation and HTML extraction
    /// </summary>
    public class BrowserAutomation
    {
        private IBrowser? _browser;
        private IPlaywright? _playwright;

        /// <summary>
        /// Initializes the Playwright browser instance
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                Console.WriteLine("Initializing Playwright...");
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });
                Console.WriteLine("Playwright initialized successfully.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize Playwright. Make sure Playwright browsers are installed.", ex);
            }
        }

        /// <summary>
        /// Navigates to a URL and extracts the full rendered HTML content and response headers
        /// </summary>
        /// <param name="url">The URL to navigate to</param>
        /// <returns>PageResponse object containing HTML content and response headers</returns>
        public async Task<PageResponse> ExtractPageDataAsync(string url)
        {
            if (_browser == null)
            {
                throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");
            }

            IPage? page = null;
            try
            {
                Console.WriteLine($"Navigating to {url}...");
                
                page = await _browser.NewPageAsync();
                try
                {
                    IResponse? response = null;

                    // Navigate and capture response
                    response = await page.GotoAsync(url, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                        Timeout = 30000 // 30 seconds timeout
                    });

                    Console.WriteLine("Page loaded successfully. Extracting data...");
                    
                    // Get the full HTML content
                    string htmlContent = await page.ContentAsync();
                    
                    // Extract response headers
                    var pageResponse = new PageResponse
                    {
                        HtmlContent = htmlContent,
                        StatusCode = response?.Status ?? 0,
                        StatusMessage = response?.StatusText ?? "Unknown"
                    };

                    // Extract all response headers
                    if (response != null)
                    {
                        var headers = await response.AllHeadersAsync();
                        foreach (var header in headers)
                        {
                            pageResponse.ResponseHeaders[header.Key] = header.Value;
                        }
                    }
                    
                    return pageResponse;
                }
                finally
                {
                    if (page != null)
                    {
                        await page.CloseAsync();
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new InvalidOperationException($"Timeout: Page did not load within 30 seconds. URL: {url}");
            }
            catch (PlaywrightException ex)
            {
                throw new InvalidOperationException($"Browser error while accessing {url}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while extracting data from {url}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Closes the browser instance and cleans up resources
        /// </summary>
        public async Task CloseAsync()
        {
            try
            {
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    Console.WriteLine("Browser closed successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing browser: {ex.Message}");
            }
            finally
            {
                _playwright?.Dispose();
            }
        }
    }
}
