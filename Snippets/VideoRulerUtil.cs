using System.Runtime.InteropServices;

namespace Snippets.VideoRuler;

internal static class VideoRulerUtil
{
  public static readonly UtilityInfo Info = new(
    Name:        "video-ruler",
    Description: "Given a directory, sums the length of video files in that directory and sub-directories.",
    Usage:       "<directory>",
    Platforms:   SupportedPlatforms.Windows,
    CommandLine: CommandLine);

  static readonly HashSet<string> Containers =
  [
    ".AVI", 
    ".MKV",
    ".MOV",
    ".MP4",
    ".MPEG",
    ".MPG",
    ".MTS",
    ".VOB",
    ".WMV" 
  ];

  public static void CommandLine(string[] args)
  {
    if (args.Length != 1 || !Directory.Exists(args[0]))
    {
      Console.WriteLine("Usage: video-ruler <directory>");
      return;
    }

    var rootDir = new DirectoryInfo(args[0]);
    var allFiles = GetFiles(rootDir);
    var videoFiles = allFiles
      .Where(file => Containers.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
      .ToArray();

    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      throw new PlatformNotSupportedException();
    }

    var shellProgID = "Shell.Application";

    var shellType = Type.GetTypeFromProgID(shellProgID) ?? 
      throw new InvalidOperationException($@"The {shellProgID} ProgID is not registered on this system.");

    var shell = Activator.CreateInstance(shellType) ?? 
      throw new InvalidOperationException($@"Failed to create an instance of {shellProgID}.");

    var videoFileLengths = videoFiles
      .Select(file => new VideoFileInfoWithLength(file, GetLength(file, shell)))
      .ToArray();

    var totalMinutes = 0.0;
    foreach (var file in videoFileLengths)
    {
      var fileName = file.File.FullName
        .Replace(rootDir.FullName, string.Empty)
        .TrimStart('\\');

      var fileMinutes = file.Length.TotalMinutes;

      Console.WriteLine($@"{fileName}: {fileMinutes:F2}");

      totalMinutes += fileMinutes;
    }

    Console.WriteLine($@"{videoFileLengths.Length} files, {totalMinutes:F2} minutes");
  }

  /// <summary>Returns all files in the given directory (and subdirectories).</summary>
  static List<FileInfo> GetFiles(DirectoryInfo directory)
  {
    var ret = new List<FileInfo>();

    foreach (var file in directory.GetFiles())
    {
      ret.Add(file);
    }

    foreach (var subdirectory in directory.GetDirectories())
    {
      ret.AddRange(GetFiles(subdirectory));
    }

    return ret;
  }

  /// <summary>Returns the length of the given video file.</summary>
  static TimeSpan GetLength(FileInfo file, dynamic shell)
  {
    var folder = shell.NameSpace(file.DirectoryName);
    var folderItem = folder.ParseName(file.Name);
    return TimeSpan.Parse(folder.GetDetailsOf(folderItem, 27));
  }

  record VideoFileInfoWithLength(
    FileInfo File,
    TimeSpan Length);
}
