# Quick Start Guide

## Prerequisites
- .NET 10 SDK installed
- Internet connection for package downloads

## Setup (First Time Only)

### 1. Install Playwright Browsers
```bash
dotnet tool install microsoft.playwright.cli -g
playwright install
```

### 2. Build the Project
```bash
cd HeadlessBrowserAnalyzer
dotnet build
```

## Running the Application

```bash
dotnet run
```

## Example Usage

```
╔════════════════════════════════════════╗
║   Headless Browser Analyzer (.NET 10)  ║
╚════════════════════════════════════════╝

Enter a URL (or 'exit' to quit): github.com
Navigating to https://github.com/...
Page loaded successfully. Extracting HTML...

================================================================================
HEADLESS BROWSER ANALYSIS RESULTS
================================================================================
[... results displayed ...]

Save full HTML to file? (y/n): n

Enter a URL (or 'exit' to quit): exit
Thank you for using Headless Browser Analyzer!
```

## Tips

- **Omit protocol**: Just type `example.com` (https:// is added automatically)
- **Save HTML**: Answer 'y' to save the full page HTML to a timestamped file
- **Multiple URLs**: Analyze multiple URLs in one session
- **Exit**: Type `exit` to close the application

## System Requirements

| Component | Requirement |
|-----------|-------------|
| OS | Windows, macOS, or Linux |
| Runtime | .NET 10 |
| RAM | 512 MB minimum |
| Disk | 500 MB for browser binaries |
| Network | Required for page access |

## Troubleshooting

### "Playwright not initialized" Error
Run the setup steps again:
```bash
playwright install
```

### "Timeout" Error
The page took longer than 30 seconds to load. This is normal for slow/complex sites.

### "Invalid URL" Error
Make sure your URL is properly formatted (without spaces or special characters).

## Application Output

The application displays:
1. **Page Title** - The `<title>` tag content
2. **HTML Content** - First 500 characters of rendered HTML
3. **CSS Files** - External stylesheet references
4. **Inline Styles** - Count and preview of `<style>` tags
5. **JavaScript Files** - External script references  
6. **Inline Scripts** - Count and preview of inline `<script>` tags

## For More Information

See `README.md` for complete documentation and feature details.
