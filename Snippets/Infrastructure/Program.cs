namespace Snippets;

internal static class Program
{
  public static void Main(string[] args)
  {
    var utilities = new List<UtilityInfo>
    {
      RelativeSearch.RelativeSearchUtils.Info,
    };

    try
    {
      var utilityName = args.FirstOrDefault();
      var utilityArgs = utilityName is null ? Array.Empty<string>() : args.Skip(1).ToArray();
      var utility = utilities.FirstOrDefault(x => x.Name == utilityName);
      if (utility is not null)
      {
        utility.CommandLine(utilityArgs);
      }
      else
      {
        throw new UsageException();
      }
    }
    catch (UsageException)
    {
      WriteUsages(utilities);
    }
  }

  static void WriteUsages(List<UtilityInfo> utilities)
  {
    var lines = new List<string>
    {
      "Usages:",
      string.Empty,
    };

    foreach (var utility in utilities)
    {
      lines.AddRange([
        $@"  {utility.Name}",
        $@"  {utility.Description}",
        $@"  {utility.Usage}",
        string.Empty,
        ]);
    }

    foreach (var line in lines)
    {
      Console.WriteLine(line);
    }
  }
}
