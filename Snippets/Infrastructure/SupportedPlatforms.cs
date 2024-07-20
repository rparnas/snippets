namespace Snippets;

[Flags]
public enum SupportedPlatforms
{
  None    = 0,
  Windows = 1,
  Linux   = 2,
  MacOS   = 4,
  All     = Windows | Linux | MacOS
}
