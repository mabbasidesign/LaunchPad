using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace LaunchPad.Extensions
{
    /// <summary>
    /// Extension methods for handling EF Core concurrency exceptions (DbUpdateConcurrencyException).
    /// </summary>
    public static class ConcurrencyExceptionHandler
    {
        /// <summary>
        /// Handles DbUpdateConcurrencyException by returning a ProblemDetails response.
        /// This occurs when multiple users try to update the same record simultaneously.
        /// </summary>
        public static void HandleConcurrencyException(
            this HttpContext context,
            DbUpdateConcurrencyException ex,
            ILogger logger)
        {
            logger.LogWarning(
                ex,
                "Concurrency conflict: Record has been modified by another user. Affected entities: {AffectedEntities}",
                string.Join(", ", ex.Entries.Select(e => e.Entity.GetType().Name)));

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new
            {
                type = "https://httpwg.org/specs/rfc7231.html#status.409",
                title = "Conflict",
                status = 409,
                detail = "The record has been modified by another user. Please refresh and try again.",
                instance = context.Request.Path.Value
            };

            context.Response.WriteAsJsonAsync(problemDetails, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// Checks if DbUpdateConcurrencyException indicates a specific entity conflict.
        /// </summary>
        public static bool HasConflictFor<T>(this DbUpdateConcurrencyException ex) where T : class
        {
            return ex.Entries.Any(e => e.Entity is T);
        }

        /// <summary>
        /// Reloads a single entry to reflect current database values after a concurrency conflict.
        /// Useful for retry scenarios.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static async Task ReloadAsync(this DbUpdateConcurrencyException ex, int entryIndex = 0)
        {
            if (ex.Entries.Count > entryIndex)
            {
                await ex.Entries[entryIndex].ReloadAsync();
            }
        }
    }
}
