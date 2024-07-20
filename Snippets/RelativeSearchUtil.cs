namespace Snippets.RelativeSearch;

internal static class RelativeSearchUtils
{
  public static readonly UtilityInfo Info = new UtilityInfo(
    Name:        "relative-search",
    Description: "Given a sequence of bytes and a file, return addresses in that file matching bytes in that sequence (or where each value is offset by the same amount).",
    Usage:       "<file> <byte1> <byte2>...",
    CommandLine: CommandLine);

  public static void CommandLine(string[] args)
  {
    if (args.Length < 3 || !File.Exists(args[0]) || args.Skip(1).Any(arg => !byte.TryParse(arg, out var b)))
    {
      throw new UsageException();
    }

    var bytes = File.ReadAllBytes(args[0]);

    var pattern = new byte?[args.Length - 1];
    for (var i = 1; i < args.Length; i++)
    {
      pattern[i - 1] = args[i] == "*" ? null : byte.Parse(args[i]);
    }

    Console.WriteLine("Searching...");
    Console.WriteLine();

    var found = RelativeSearch(bytes, pattern);
    foreach (var address in found)
    {
      Console.WriteLine($@"0x{found:x2}");
    }

    Console.WriteLine();
    Console.WriteLine("Done.");
  }

  public static List<long> RelativeSearch(byte[] bytes, byte?[] pattern)
  {
    var ret = new List<long>();

    for (var i = 0L; i < bytes.LongLength - pattern.Length + 1; i++)
    {
      var patternByteZero = pattern[0];
      var zero = bytes[i];
      var success = true;

      for (var j = 1; j < pattern.Length; j++)
      {
        var patternByte = pattern[j];
        if (patternByte is null)
        {
          continue;
        }

        var val = bytes[i + j];
        if ((zero - patternByteZero) != (val - patternByte))
        {
          success = false;
          break;
        }
      }

      if (success)
      {
        ret.Add(i);
      }
    }

    return ret;
  }
}
