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
    var lines = new List<string>
    {
      "Available Utilities:",
      string.Empty,
    };

    foreach (var utility in utilities)
    {
      lines.AddRange([
        $@"  ----{utility.Name}----",
        $@"  {utility.Description}",
        $@"  Usage: snippets {utility.Name} {utility.Usage}",
        string.Empty,
        ]);
    }

    foreach (var line in lines)
    {
      Console.WriteLine(line);
    }
  }
}
