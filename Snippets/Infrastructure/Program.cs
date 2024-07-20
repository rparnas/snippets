using System.Runtime.InteropServices;

namespace Snippets;

internal static class Program
{
  public static void Main(string[] args)
  {
    var utilities = new List<UtilityInfo>
    {
      RelativeSearch.RelativeSearchUtil.Info,
      VideoRuler.VideoRulerUtil.Info,
    };

    try
    {
      var utilityName = args.FirstOrDefault();
      var utilityArgs = utilityName is null ? [] : args.Skip(1).ToArray();
      var utility = utilities.FirstOrDefault(x => x.Name == utilityName);
      if (utility is null)
      {
        throw new UsageException();
      }
      else if (!GetIsPlatformSupported(utility.Platforms))
      {
        Console.WriteLine($@"The utility '{utility.Name}' is not supported on this platform");
      }
      else
      {
        utility.CommandLine(utilityArgs);
      }
    }
    catch (UsageException)
    {
      WriteUsages(utilities);
    }
  }

  static bool GetIsPlatformSupported(SupportedPlatforms platforms)
  {
    return 
      (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && platforms.HasFlag(SupportedPlatforms.Windows)) ||
      (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && platforms.HasFlag(SupportedPlatforms.Linux)) ||
      (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && platforms.HasFlag(SupportedPlatforms.MacOS));
  }

  static void WriteUsages(List<UtilityInfo> utilities)
  {
    static string GetPlatformString(SupportedPlatforms platforms)
    {
      if (platforms == SupportedPlatforms.All)
        return nameof(SupportedPlatforms.All);

      var supportedPlatforms = new[] { SupportedPlatforms.Windows, SupportedPlatforms.Linux, SupportedPlatforms.MacOS }
        .Where(platform => platforms.HasFlag(platform))
        .Select(platform => platform.ToString())
        .ToArray();

      var count = supportedPlatforms.Length;

      return
        count == 0 ? nameof(SupportedPlatforms.None) :
        count == 1 ? supportedPlatforms.Single() :
        count == 2 ? $@"{supportedPlatforms[0]} or {supportedPlatforms[1]}" :
        string.Join(", ", supportedPlatforms, 0, supportedPlatforms.Length - 1) + $", or {supportedPlatforms.Last()}";
    }

    var lines = new List<string>
    {
      "# snippets",
      "A collection of small algorithms and mini-projects",
      string.Empty,
      "## Available Utilities:",
      string.Empty,
    };

    foreach (var utility in utilities)
    {
      lines.AddRange([
        $@"### {utility.Name}",
        $@"{utility.Description}",
        $@"Platform(s): {GetPlatformString(utility.Platforms)}",
        $@"Usage: snippets {utility.Name} {utility.Usage}",
        string.Empty,
        ]);
    }

    foreach (var line in lines)
    {
      Console.WriteLine(line);
    }
  }
}
