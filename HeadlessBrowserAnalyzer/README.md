# Headless Browser Analyzer (.NET 10)

A modern .NET 10 console application that uses Playwright to visit user-provided URLs, execute JavaScript, and extract a complete summary of HTML content, CSS file dependencies, and JavaScript file dependencies.

## Features

✅ **Interactive URL Input** - Accept URLs from console with validation and normalization  
✅ **Headless Browser Automation** - Launch Chromium browser using Playwright  
✅ **Full HTML Extraction** - Extract complete rendered HTML content including JavaScript-executed DOM  
✅ **Resource Detection** - Identify all CSS file references (external and inline styles)  
✅ **JavaScript Analysis** - Identify all JavaScript file references (external and inline scripts)  
✅ **Formatted Output** - Clear, structured console display of results  
✅ **Error Handling** - Graceful handling of network errors, invalid URLs, and timeouts  
✅ **.NET 10 Native** - Built on latest .NET runtime with modern C# features  

## Architecture

The application is organized into modular components:

### Components

1. **UrlValidator** (`UrlValidator.cs`)
   - Validates URL format
   - Normalizes URLs (adds https:// if scheme missing)
   - Handles edge cases gracefully

2. **BrowserAutomation** (`BrowserAutomation.cs`)
   - Initializes Playwright Chromium browser
   - Navigates to URLs with network idle wait condition
   - Extracts full rendered HTML content
   - Handles timeouts and browser errors

3. **HtmlParser** (`HtmlParser.cs`)
   - Parses HTML using AngleSharp
   - Extracts CSS file references from `<link rel="stylesheet">` tags
   - Extracts JavaScript file references from `<script src="">` tags
   - Identifies inline styles and scripts
   - Provides structured `PageResources` data model

4. **ResultsFormatter** (`ResultsFormatter.cs`)
   - Formats extracted data for console display
   - Organizes results with clear sections
   - Optional: Save full HTML to file
   - Handles large content gracefully

5. **Program** (`Program.cs`)
   - Main application orchestration
   - User input loop
   - Error handling and user feedback
   - Resource cleanup

## Project Structure

```
HeadlessBrowserAnalyzer/
├── Program.cs                      # Main entry point
├── UrlValidator.cs                 # URL validation logic
├── BrowserAutomation.cs            # Playwright automation
├── HtmlParser.cs                   # HTML parsing and resource extraction
├── ResultsFormatter.cs             # Results display formatting
├── HeadlessBrowserAnalyzer.csproj  # Project configuration
└── bin/
    └── Debug/
        └── net10.0/               # Build output
```

## Requirements

- **.NET 10 Runtime**
- **Playwright Browsers** (Chromium, Firefox, WebKit) - installed via CLI
- **Windows, macOS, or Linux** - Playwright and .NET 10 are cross-platform

## NuGet Dependencies

- `Microsoft.Playwright` (v1.57.0+) - Headless browser automation
- `AngleSharp` (v1.4.0+) - HTML parsing and DOM querying

## Installation & Setup

### 1. Create/Open the Project

The project has already been created in the `HeadlessBrowserAnalyzer/` directory.

### 2. Install Playwright Browsers

First, install the Playwright CLI tool:

```bash
dotnet tool install microsoft.playwright.cli -g
```

Then install browsers:

```bash
playwright install
```

### 3. Build the Application

```bash
cd HeadlessBrowserAnalyzer
dotnet build
```

## Running the Application

Start the application with:

```bash
dotnet run
```

### Usage Example

```
╔════════════════════════════════════════╗
║   Headless Browser Analyzer (.NET 10)  ║
╚════════════════════════════════════════╝

Enter a URL (or 'exit' to quit): example.com
Navigating to https://example.com/...
Page loaded successfully. Extracting HTML...

================================================================================
HEADLESS BROWSER ANALYSIS RESULTS
================================================================================

URL: https://example.com/
Page Title: Example Domain

...
[Detailed analysis results]
...

Save full HTML to file? (y/n): n

Enter a URL (or 'exit' to quit): exit
Thank you for using Headless Browser Analyzer!
Browser closed successfully.
```

## Features in Detail

### URL Validation

The application automatically:
- Validates URL format
- Adds `https://` scheme if missing
- Rejects invalid URLs with clear error messages

### Browser Automation

The application:
- Launches headless Chromium browser (no UI)
- Waits for network idle (all resources loaded)
- Supports 30-second timeout per page
- Executes all JavaScript before extraction

### Resource Extraction

Identifies:
- **External CSS Files**: All `<link rel="stylesheet">` references
- **Inline Styles**: All `<style>` tags with CSS content
- **External JavaScript**: All `<script src="">` references
- **Inline Scripts**: All `<script>` tags with JavaScript code

### Results Display

Clear formatted output including:
- Page URL and title
- HTML content size and snippet
- Complete list of CSS file references
- Count of inline styles with previews
- Complete list of JavaScript file references
- Count of inline scripts with previews

### Error Handling

Graceful error handling for:
- Invalid URLs (format validation)
- Network timeouts (30-second limit)
- Browser errors (invalid pages, access denied)
- Parsing errors (malformed HTML)
- File operations (HTML export)

## Code Examples

### Using UrlValidator

```csharp
if (UrlValidator.ValidateAndNormalizeUrl(userInput, out string validatedUrl))
{
    Console.WriteLine($"Valid URL: {validatedUrl}");
}
```

### Using BrowserAutomation

```csharp
var browser = new BrowserAutomation();
await browser.InitializeAsync();
string html = await browser.ExtractHtmlAsync("https://example.com");
await browser.CloseAsync();
```

### Using HtmlParser

```csharp
PageResources resources = HtmlParser.ParseHtmlContent(htmlContent);
Console.WriteLine($"CSS Files: {resources.CssFiles.Count}");
Console.WriteLine($"JS Files: {resources.JavaScriptFiles.Count}");
```

### Using ResultsFormatter

```csharp
ResultsFormatter.DisplayResults(url, resources);
ResultsFormatter.SaveHtmlToFile("page.html", resources.HtmlContent);
```

## Performance Considerations

- **Browser Cleanup**: Browser closes after each URL visit for resource efficiency
- **Network Idle Wait**: Ensures all dynamic content is loaded
- **HTML Parsing**: AngleSharp efficiently parses without rendering
- **Memory**: Full HTML content is kept in memory; consider for very large pages

## Supported URL Schemes

- ✅ `http://` - Standard HTTP
- ✅ `https://` - Secure HTTPS (default)
- ✅ No scheme - Automatically defaults to `https://`
- ❌ `ftp://`, `file://`, other schemes not supported

## Known Limitations

1. **WebKit Browser**: On macOS 12 and older, WebKit browser download may fail (not critical - Chromium works)
2. **Page-Specific JavaScript**: Dynamic content loaded after network idle might not be captured
3. **Authentication**: Does not support pages requiring authentication
4. **Large Pages**: Very large HTML pages may take time to parse

## Troubleshooting

### "Playwright not initialized" Error
Ensure Playwright browsers are installed:
```bash
playwright install
```

### Browser Download Failures
This can happen with slow internet connections. Retry or check:
```bash
playwright install chromium
```

### Timeout Errors
Some pages take longer to load. The current timeout is 30 seconds in `BrowserAutomation.cs`:
```csharp
Timeout = 30000 // 30 seconds timeout
```

## Future Enhancements

Potential improvements:
- Support for authenticated pages (cookies, login)
- Export to JSON/CSV format
- Parallel URL processing
- Custom browser settings (viewport, user-agent)
- Screenshot capture of pages
- Web performance metrics
- JavaScript execution tracking

## License

This project is provided as-is for educational and demonstration purposes.

## Technical Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Language | C# | Latest (.NET 10) |
| Framework | .NET | 10.0 |
| Browser Automation | Playwright | 1.57.0+ |
| HTML Parser | AngleSharp | 1.4.0+ |
| Platform | Cross-platform | Windows, macOS, Linux |

## Author Notes

This application demonstrates:
- Modern .NET 10 async/await patterns
- Headless browser automation with Playwright
- DOM parsing with AngleSharp
- Clean modular architecture
- Comprehensive error handling
- User-friendly CLI design
