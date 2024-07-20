namespace Snippets;

internal record UtilityInfo(
  string Name,
  string Description,
  string Usage,
  SupportedPlatforms Platforms,
  Action<string[]> CommandLine);
