namespace EntityFramework.Configuration;

internal static class EnvFile
{
    public static void Load(string fileName = ".env")
    {
        string? path = FindInParentDirectories(Directory.GetCurrentDirectory(), fileName)
            ?? FindInParentDirectories(AppContext.BaseDirectory, fileName);

        if (path is null)
        {
            return;
        }

        foreach (string line in File.ReadLines(path))
        {
            string value = line.Trim();

            if (value.Length == 0 || value.StartsWith('#'))
            {
                continue;
            }

            int separatorIndex = value.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            string key = value[..separatorIndex].Trim();
            string environmentValue = value[(separatorIndex + 1)..].Trim();

            if (environmentValue.Length >= 2 &&
                ((environmentValue[0] == '"' && environmentValue[^1] == '"') ||
                 (environmentValue[0] == '\'' && environmentValue[^1] == '\'')))
            {
                environmentValue = environmentValue[1..^1];
            }

            // Values supplied by the shell or deployment environment take precedence.
            if (Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, environmentValue);
            }
        }
    }

    private static string? FindInParentDirectories(string startDirectory, string fileName)
    {
        DirectoryInfo? directory = new(startDirectory);

        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
