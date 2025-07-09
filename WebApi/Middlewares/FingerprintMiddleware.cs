using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Middlewares
{
    public class FingerprintOptions
    {
        public int RateLimitThreshold { get; set; } = 100;
        public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromMinutes(15);
        public string[] WhitelistedIps { get; set; } = Array.Empty<string>();
        public string[] KnownMaliciousIps { get; set; } = Array.Empty<string>();
        public bool EnablePatternDetection { get; set; } = true;
        public bool EnableThreatIntelligence { get; set; } = true;
        public bool EnableBotDetection { get; set; } = true;
    }

    public class FingerprintMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FingerprintMiddleware> _logger;
        private readonly FingerprintOptions _options;

        public FingerprintMiddleware(
            RequestDelegate next, 
            IMemoryCache cache,
            ILogger<FingerprintMiddleware> logger,
            IOptions<FingerprintOptions>? options = null)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _options = options?.Value ?? new FingerprintOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            
            // Skip fingerprinting for whitelisted IPs
            if (_options.WhitelistedIps.Contains(ip))
            {
                await _next(context);
                return;
            }

            var fingerprint = GenerateFingerprint(context);
            
            if (await IsSuspiciousAsync(fingerprint, context))
            {
                _logger.LogWarning("Suspicious request detected with fingerprint: {fingerprint}, IP: {ip}", 
                    fingerprint.Substring(0, 8) + "...", ip);
                
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Request blocked: rate limit exceeded.");
                return;
            }

            await _next(context);
        }

        private string GenerateFingerprint(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var acceptLang = context.Request.Headers["Accept-Language"].ToString();
            var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();
            
            // Include path in fingerprint to track specific endpoints being targeted
            var path = context.Request.Path.ToString();

            // Create a more robust fingerprint with SHA256
            var raw = $"{ip}:{userAgent}:{acceptLang}:{acceptEncoding}:{path}";
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToBase64String(hashBytes);
        }

        private async Task<bool> IsSuspiciousAsync(string fingerprint, HttpContext context)
        {
            var cacheKey = $"fingerprint_{fingerprint}";
            
            if (!_cache.TryGetValue<int>(cacheKey, out var requestCount))
            {
                requestCount = 0;
            }
            
            requestCount++;
            
            // Set or update the cache
            _cache.Set(cacheKey, requestCount, _options.RateLimitWindow);
            
            // Check against configured threshold
            if (requestCount > _options.RateLimitThreshold)
            {
                return true;
            }

            // Pattern detection for suspicious behavior
            if (_options.EnablePatternDetection && DetectSuspiciousPatterns(context))
            {
                _logger.LogWarning("Suspicious pattern detected from IP: {ip}", 
                    context.Connection.RemoteIpAddress);
                return true;
            }

            // Integration with threat intelligence
            if ( _options.EnableThreatIntelligence && (await CheckThreatIntelligenceAsync(context)))
            {
                _logger.LogWarning("IP found in threat intelligence database: {ip}", 
                    context.Connection.RemoteIpAddress);
                return true;
            }

            // Check against known bot signatures
            if (_options.EnableBotDetection && MatchesBotSignature(context))
            {
                _logger.LogWarning("Request matches known bot signature: {userAgent}", 
                    context.Request.Headers.UserAgent);
                return true;
            }
            
            return false;
        }

        private bool DetectSuspiciousPatterns(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path.ToString();
            var method = context.Request.Method;
            var requestKey = $"request_pattern_{ip}";
            
            // Track sequential requests from same IP
            if (!_cache.TryGetValue<List<(string path, string method, DateTime time)>>(
                requestKey, out var recentRequests))
            {
                recentRequests = new List<(string, string, DateTime)>();
            }
            
            // Add current request to history
            recentRequests.Add((path, method, DateTime.UtcNow));
            
            // Keep only recent requests (last 5 minutes)
            recentRequests = recentRequests
                .Where(r => r.time > DateTime.UtcNow.AddMinutes(-5))
                .ToList();
            
            _cache.Set(requestKey, recentRequests, TimeSpan.FromMinutes(10));
            
            // Check for rapid scanning patterns (many different endpoints in short time)
            if (recentRequests.Count >= 10)
            {
                var uniquePaths = recentRequests.Select(r => r.path).Distinct().Count();
                if (uniquePaths >= 8) // 80% of requests to different endpoints
                {
                    return true;
                }
            }
            
            // Check for repeated failed auth attempts
            var authFailKey = $"auth_fail_{ip}";
            if (path.Contains("/login", StringComparison.OrdinalIgnoreCase) && 
                method == HttpMethods.Post && context.Response.StatusCode == 401)
            {
                int failCount = _cache.GetOrCreate(authFailKey, e => {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return 1;
                });
                
                _cache.Set(authFailKey, failCount + 1, TimeSpan.FromHours(1));
                
                if (failCount >= 5) // 5 failed attempts
                {
                    return true;
                }
            }
            
            return false;
        }

        private async Task<bool> CheckThreatIntelligenceAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip)) return false;
            
            // Cache check results to avoid repeated API calls
            var cacheKey = $"threat_intel_{ip}";
            if (_cache.TryGetValue<bool>(cacheKey, out var isKnownThreat))
            {
                return isKnownThreat;
            }
            
            // For a real implementation, you would call a threat intelligence API here
            // Example with placeholder async call:
            bool isThreatListed = await CheckExternalThreatDatabase(ip);
            
            // Cache the result for an hour
            _cache.Set(cacheKey, isThreatListed, TimeSpan.FromHours(1));
            
            return isThreatListed;
        }

        // Placeholder method - replace with actual threat intel API
        private async Task<bool> CheckExternalThreatDatabase(string ip)
        {
            // In a real implementation, you would call an external API like:
            // - AbuseIPDB
            // - IPQualityScore
            // - VirusTotal
            
            // Sample implementation (replace with actual API call)
            await Task.Delay(1); // Just to keep method async
            
            // For demo purposes, consider local threat list
            string[] knownBadIps = _options.KnownMaliciousIps ?? Array.Empty<string>();
            return knownBadIps.Contains(ip);
        }

        private bool MatchesBotSignature(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString().ToLowerInvariant();
            
            // Check for common bad bot signatures
            string[] knownBadBots = {
                "zgrab", "masscan", "nmap", "semrush", "rogerbot",
                "dotbot", "ahrefsbot", "mj12bot", "scraper", "crawler",
                "spider", "curl", "python-requests"
            };
            
            if (knownBadBots.Any(bot => userAgent.Contains(bot)))
            {
                return true;
            }
            
            // Check for UA/header inconsistencies that suggest spoofing
            var acceptHeader = context.Request.Headers["Accept"].ToString();
            var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
            
            // Browser request without standard headers
            if (userAgent.Contains("chrome") || userAgent.Contains("firefox") || 
                userAgent.Contains("safari") || userAgent.Contains("edge"))
            {
                if (string.IsNullOrEmpty(acceptHeader) || string.IsNullOrEmpty(acceptLanguage))
                {
                    return true; // Suspicious - browsers always send these headers
                }
            }
            
            return false;
        }
    }
}
