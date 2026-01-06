# Plan: .NET 10 Headless Browser Analyzer

A .NET 10 console application that uses Playwright (Microsoft's modern headless browser library) to visit user-provided URLs, execute JavaScript, and extract a complete summary of HTML content, CSS file dependencies, and JavaScript file dependencies.

## Steps

1. **Create .NET 10 console project** with project file configured for .NET 10, NuGet packages added for Playwright and AngleSharp.

2. **Build URL input & validation module** accepting console input, validating URL format, and handling edge cases.

3. **Implement Playwright browser automation** launching headless Chromium, navigating to URL with network idle wait condition, and extracting full rendered HTML.

4. **Create HTML parsing & extraction engine** using AngleSharp to query DOM for `<link rel="stylesheet">`, `<script src="">` tags, extract href/src attributes, and identify embedded styles/scripts.

5. **Build results formatter & display** organizing extracted data (HTML summary, CSS file list, JS file list) and outputting to console with clear formatting.

6. **Add error handling & user feedback** including network errors, invalid URLs, timeout handling, and progress messages.

## Further Considerations

1. **Browser cleanup strategy**: Should the app close the browser after each URL visit, or keep it open for multiple sequential queries? → Recommended: Close after each visit for resource efficiency.

2. **HTML content depth**: Display full HTML or summarized version (first N lines/characters)? → Recommended: Display full HTML to stdout or optionally save to file.

3. **Filtering CSS/JS**: Should the summary include inline styles/scripts and third-party CDN files, or only external file references? → Recommended: List all (both inline and external) with clear categorization.

## Technical Architecture

### Recommended Library: Playwright for .NET

**Why Playwright:**
- Microsoft-backed with official support
- .NET 10 compatible
- Multi-browser support (Chromium, Firefox, WebKit)
- Executes JavaScript (unlike static parsers)
- Modern, actively maintained, production-ready
- Cross-platform support

### Key NuGet Packages

```
Microsoft.Playwright          // Headless browser automation
AngleSharp                     // HTML parsing and DOM querying
```

### Architecture Flow

```
Console Input (URL)
    ↓
URL Validation
    ↓
Launch Headless Browser (Playwright)
    ↓
Navigate to URL
    ↓
Wait for Page Load (Network Idle)
    ↓
Extract Full HTML Content
    ↓
Parse with AngleSharp
    ↓
Extract CSS References
    ├── <link rel="stylesheet">
    ├── @import statements
    └── inline <style> tags
    ↓
Extract JavaScript References
    ├── <script src="">
    ├── Inline scripts
    └── Dynamic script tags
    ↓
Format & Display Results
    ├── HTML summary
    ├── CSS file list
    ├── JavaScript file list
    └── Embedded styles/scripts info
```

### Implementation Components

1. **User Input Module**
   - Accept URL from console
   - Validate URL format
   - Handle invalid input gracefully

2. **Browser Automation Module** (Playwright)
   - Initialize browser instance
   - Create new page/context
   - Navigate to URL with wait conditions
   - Extract rendered HTML
   - Handle timeouts and network errors

3. **HTML Parsing Module** (AngleSharp)
   - Parse HTML content
   - Query for stylesheet links: `link[rel='stylesheet']`
   - Query for script tags: `script[src]`
   - Extract href/src attributes
   - Identify inline styles and scripts

4. **Results Formatter & Reporter**
   - Organize extracted data
   - Format for console display
   - Optional: Export to file (JSON/CSV)
   - Clear, structured output

## Success Criteria

- ✅ Accepts user input for URLs via console
- ✅ Launches headless browser and navigates to URL
- ✅ Extracts full HTML content
- ✅ Identifies all CSS file references (external and inline)
- ✅ Identifies all JavaScript file references (external and inline)
- ✅ Displays formatted summary to user
- ✅ Handles errors gracefully (invalid URLs, network issues, timeouts)
- ✅ Runs on .NET 10 runtime
