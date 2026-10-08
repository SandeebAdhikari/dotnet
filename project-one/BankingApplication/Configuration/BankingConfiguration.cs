namespace BankingApp.Configuration
{
    internal sealed record BankingConfiguration(
        string ConnectionString,
        string AdminUsername,
        string AdminPassword)
    {
        public static BankingConfiguration Load()
        {
            LoadEnvironmentFile();

            return new BankingConfiguration(
                Require("BANKING_DB_CONNECTION"),
                Require("BANKING_ADMIN_USERNAME"),
                Require("BANKING_ADMIN_PASSWORD"));
        }

        public static string LoadConnectionString()
        {
            LoadEnvironmentFile();

            return Require("BANKING_DB_CONNECTION");
        }

        private static void LoadEnvironmentFile()
        {
            string environmentFilePath = Path.Combine(
                AppContext.BaseDirectory,
                ".env");

            DotNetEnv.Env
                .NoClobber()
                .TraversePath()
                .Load(environmentFilePath);
        }

        private static string Require(string variableName)
        {
            string? value = Environment.GetEnvironmentVariable(
                variableName);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Required setting {variableName} was not found in .env or the process environment.");
            }

            return value;
        }
    }
}
