using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using SWM.CTA.Application.Users;
using SWM.CTA.Domain.Users;
using SWM.CTA.Infrastructure.Extensions;
using SWM.CTA.Infrastructure.Services;
using SWM.CTA.Presentation.ConsoleApp.AppSettings;

public static partial class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("-- Submission of SevenWestMedia Core Technical Assignment 2020-02-28 --");
        Console.WriteLine("-------------------- By Phil James for 7Plus Team ---------------------");
        Console.WriteLine();
        Console.WriteLine("The application will load its settings from ./AppSettings/appsettings.json.");
        Console.WriteLine("You can modify it to change the endpoint from which user data is retrieved.");
        Console.WriteLine();
        Console.WriteLine("Press any key to start the test.");
        Console.ReadKey();
        Console.WriteLine();

        try
        {
            // Arrange
            IConfiguration config = _LoadConfiguration();
            IServiceProvider serviceProvider = _LoadServices(config);

            IUserDataService userDataService = serviceProvider.GetRequiredService<IUserDataService>();
            ICollection<User> users = await userDataService.GetAll();

            AppSettings appSettings = serviceProvider.GetService<IOptions<AppSettings>>()?.Value 
                                      ?? throw new MissingConfigurationException(typeof(AppSettings));
            
            // Act
            _PrintUsersFullName(users, 42);
            _PrintUsersFirstNamesByAge(users, 23);
            _PrintNumberOfGendersPerAgeAscending(users, appSettings.EnableParallelProcessing);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToInformativeString());
        }
        
        // Finish
        Console.WriteLine();
        Console.WriteLine("Test finished. Press any key to quit.");
        Console.ReadKey();

    }
    
    #region Application Loading

    private static IConfiguration _LoadConfiguration()
    {
        // Load application config.
        // For now, for simplicity, our service configurations are expected to be amongst the application settings.
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("AppSettings/appsettings.json", optional: false)
                                                .AddJsonFile("AppSettings/appsettings.Local.json", optional: true);

        return builder.Build();
    }
    
    private static IServiceProvider _LoadServices(IConfiguration config)
    {
        // Create DI Service Collection.
        var services = new ServiceCollection();
        
        // Add logging
        Log.Logger = new LoggerConfiguration()
                     .WriteTo.Console()
                     .CreateLogger();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        
        // Register all applicable services and their configurations
        services.AddSingleton<IUserDataService, UserDataService>();
        services.AddServiceConfiguration<UserDataServiceSettings>(config);
        
        // Register core application configuration
        services.AddConfiguration<AppSettings>(config);

        return services.BuildServiceProvider();
    }
    
    #endregion
}


