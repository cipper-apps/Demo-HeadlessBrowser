using System;
using System.Collections.Generic;
using System.Globalization;

namespace HeadlessBrowserAnalyzer
{
    public enum CacheStatus
    {
        Hit,
        Miss,
        Revalidated,
        Stale,
        Bypass,
        Error,
        Unknown
    }

    public static class CacheStatusDetector
    {
        public static CacheStatus Detect(Dictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
            {
                return CacheStatus.Unknown;
            }

            var lookup = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
            var detectors = new Func<Dictionary<string, string>, CacheStatus>[]
            {
                DetectCloudflareCacheStatus,
                DetectCacheStatusHeader,
                DetectCacheHitsHeader,
                DetectXCacheHeader,
                DetectStackPathCacheStatus,
                DetectCacheControlBypass,
                DetectAgeHeader
            };

            foreach (var detector in detectors)
            {
                var status = detector(lookup);
                if (status != CacheStatus.Unknown)
                {
                    return status;
                }
            }

            return CacheStatus.Unknown;
        }

        private static CacheStatus DetectCloudflareCacheStatus(Dictionary<string, string> headers)
        {
            return TryGetHeaderValue(headers, "CF-Cache-Status", out var cfCacheStatus)
                ? MapCloudflareCacheStatus(cfCacheStatus)
                : CacheStatus.Unknown;
        }

        private static CacheStatus DetectCacheStatusHeader(Dictionary<string, string> headers)
        {
            if (!TryGetHeaderValue(headers, "X-Cache-Status", out var cacheStatusValue))
            {
                return CacheStatus.Unknown;
            }

            var normalized = cacheStatusValue.Trim().ToLowerInvariant();
            return normalized switch
            {
                "hit" => CacheStatus.Hit,
                "miss" => CacheStatus.Miss,
                "revalidated" => CacheStatus.Revalidated,
                "stale" => CacheStatus.Stale,
                "uncacheable" => CacheStatus.Bypass,
                _ => CacheStatus.Unknown
            };
        }

        private static CacheStatus DetectCacheHitsHeader(Dictionary<string, string> headers)
        {
            if (TryGetHeaderValue(headers, "X-Cache-Hits", out var cacheHitsValue)
                && int.TryParse(cacheHitsValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var hits)
                && hits > 0)
            {
                return CacheStatus.Hit;
            }

            return CacheStatus.Unknown;
        }

        private static CacheStatus DetectXCacheHeader(Dictionary<string, string> headers)
        {
            if (!TryGetHeaderValue(headers, "X-Cache", out var xCacheValue))
            {
                return CacheStatus.Unknown;
            }

            var normalized = xCacheValue.Trim();

            var cloudfrontResult = MapCloudFrontCacheStatus(normalized);
            if (cloudfrontResult != CacheStatus.Unknown)
            {
                return cloudfrontResult;
            }

            var akamaiResult = MapAkamaiCacheStatus(normalized);
            if (akamaiResult != CacheStatus.Unknown)
            {
                return akamaiResult;
            }

            if (ContainsToken(normalized, "HIT"))
            {
                return CacheStatus.Hit;
            }

            if (ContainsToken(normalized, "MISS"))
            {
                return CacheStatus.Miss;
            }

            if (ContainsToken(normalized, "REFRESH"))
            {
                return CacheStatus.Revalidated;
            }

            if (ContainsToken(normalized, "DENIED") || ContainsToken(normalized, "NOCACHE"))
            {
                return CacheStatus.Bypass;
            }

            return CacheStatus.Unknown;
        }

        private static CacheStatus DetectStackPathCacheStatus(Dictionary<string, string> headers)
        {
            return TryGetHeaderValue(headers, "X-HW", out var xHwValue)
                && xHwValue.Contains(".c", StringComparison.OrdinalIgnoreCase)
                ? CacheStatus.Hit
                : CacheStatus.Unknown;
        }

        private static CacheStatus DetectCacheControlBypass(Dictionary<string, string> headers)
        {
            return TryGetHeaderValue(headers, "Cache-Control", out var cacheControlValue)
                && (ContainsToken(cacheControlValue, "no-store") || ContainsToken(cacheControlValue, "private"))
                ? CacheStatus.Bypass
                : CacheStatus.Unknown;
        }

        private static CacheStatus DetectAgeHeader(Dictionary<string, string> headers)
        {
            if (TryGetHeaderValue(headers, "Age", out var ageValue)
                && int.TryParse(ageValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var age))
            {
                return age > 0 ? CacheStatus.Hit : CacheStatus.Miss;
            }

            return CacheStatus.Unknown;
        }

        private static CacheStatus MapCloudflareCacheStatus(string value)
        {
            var normalized = value.Trim().ToUpperInvariant();
            return normalized switch
            {
                "HIT" => CacheStatus.Hit,
                "MISS" => CacheStatus.Miss,
                "REVALIDATED" => CacheStatus.Revalidated,
                "EXPIRED" => CacheStatus.Miss,
                "STALE" => CacheStatus.Stale,
                "BYPASS" => CacheStatus.Bypass,
                "DYNAMIC" => CacheStatus.Bypass,
                "UPDATING" => CacheStatus.Hit,
                "NONE" => CacheStatus.Unknown,
                "UNKNOWN" => CacheStatus.Unknown,
                _ => CacheStatus.Unknown
            };
        }

        private static CacheStatus MapCloudFrontCacheStatus(string value)
        {
            if (value.Contains("RefreshHit from cloudfront", StringComparison.OrdinalIgnoreCase))
            {
                return CacheStatus.Revalidated;
            }

            if (value.Contains("Hit from cloudfront", StringComparison.OrdinalIgnoreCase))
            {
                return CacheStatus.Hit;
            }

            if (value.Contains("Miss from cloudfront", StringComparison.OrdinalIgnoreCase))
            {
                return CacheStatus.Miss;
            }

            if (value.Contains("Error from cloudfront", StringComparison.OrdinalIgnoreCase))
            {
                return CacheStatus.Error;
            }

            return CacheStatus.Unknown;
        }

        private static CacheStatus MapAkamaiCacheStatus(string value)
        {
            var normalized = value.Trim().ToUpperInvariant();
            return normalized switch
            {
                "TCP_HIT" => CacheStatus.Hit,
                "TCP_MEM_HIT" => CacheStatus.Hit,
                "TCP_IMS_HIT" => CacheStatus.Revalidated,
                "TCP_REFRESH_HIT" => CacheStatus.Revalidated,
                "TCP_MISS" => CacheStatus.Miss,
                "TCP_REFRESH_MISS" => CacheStatus.Miss,
                "TCP_DENIED" => CacheStatus.Bypass,
                "TCP_NEGATIVE_HIT" => CacheStatus.Hit,
                "TCP_REMOTE_HIT" => CacheStatus.Hit,
                "PRIVATE_NOSTORE" => CacheStatus.Bypass,
                "CONFIG_NOCACHE" => CacheStatus.Bypass,
                _ => CacheStatus.Unknown
            };
        }

        private static bool TryGetHeaderValue(Dictionary<string, string> headers, string key, out string value)
        {
            if (headers.TryGetValue(key, out var rawValue) && !string.IsNullOrWhiteSpace(rawValue))
            {
                value = rawValue;
                return true;
            }

            value = string.Empty;
            return false;
        }

        private static bool ContainsToken(string value, string token)
        {
            return value.Contains(token, StringComparison.OrdinalIgnoreCase);
        }
    }
}
