namespace MySuperDuperPetProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            string appS = "appsettings.json";
            if (args.Length > 0)
            {
                appS = args[0];
            }
            return Host.CreateDefaultBuilder(args)
                  .ConfigureAppConfiguration((builderContext, config) =>
                  {
                      config.SetBasePath(Directory.GetCurrentDirectory());
                      config.AddJsonFile(appS, optional: false, reloadOnChange: false);
                      config.AddEnvironmentVariables();
                  })
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.ConfigureKestrel((context, options) =>
                      {
                          options.Limits.MaxRequestBodySize = 737_280_000;
                      })
                      .UseStartup<Startup>();
                  }).ConfigureLogging(logging =>
                  {
                      logging.ClearProviders();
                      logging.AddConsole();
                      logging.SetMinimumLevel(LogLevel.Trace);
                  });
        }
    }
}
