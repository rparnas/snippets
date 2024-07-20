namespace Snippets;

internal record UtilityInfo(
  string Name,
  string Description,
  string Usage,
  Action<string[]> CommandLine);
