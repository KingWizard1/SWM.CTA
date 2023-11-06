using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SWM.CTA.Application.Users;
using SWM.CTA.Domain.Users;
using SWM.CTA.Infrastructure.Extensions;
using SWM.CTA.Infrastructure.Services;

public static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("-- Submission of SevenWestMedia Core Technical Assignment 2020-02-28 --");
        Console.WriteLine("-------------------- By Phil James for 7Plus Team ---------------------");
        Console.WriteLine();
        Console.WriteLine("Press any key to start the test.");
        Console.ReadKey();
        Console.WriteLine();

        try
        {
            // Arrange
            IConfiguration config = _LoadConfiguration();
            IServiceProvider serviceProvider = _LoadServices(config);

            // Act
            await _PerformAssignmentTasks(serviceProvider);

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
        services.ValidateAndAddServiceOptions<UserDataServiceSettings>(config);

        return services.BuildServiceProvider();
    }

    private static async Task _PerformAssignmentTasks(IServiceProvider serviceProvider)
    {

        IUserDataService userDataService = serviceProvider.GetRequiredService<IUserDataService>();

        ICollection<User> users = await userDataService.GetAll();
        
        // 1. Output User Id 42's Full Name
        long idToGet = 42;
        User? user42 = users.SingleOrDefault(u => u.Id == idToGet);
        if (user42 != null)
            Console.WriteLine($"1. The full name for user with id '{idToGet}' is '{user42.First} {user42.Last}'");
        else
            Console.WriteLine($"1. Cannot print the full name for user with id '{idToGet}': user does not exist");

        Console.WriteLine();
        
        // 2. All user's first names, comma seperated, who are 23 years old
        int ageToGet = 23;
        List<User> usersByAge = users.Where(u => u.Age == ageToGet).ToList();
        if (usersByAge.Count > 0)
        {
            Console.WriteLine($"2. All first names for user's who are {ageToGet} years old, comma seperated:");
            Console.WriteLine(string.Join(", ", usersByAge.Select(u => u.First)));
            Console.WriteLine();            
        }
        else
            Console.WriteLine($"2. Cannot print first names for user's who are {ageToGet} years old: there aren't any.");
        

        // 3. The number of genders per Age, displayed from youngest to oldest.
        var userGendersPerAgeAscending = users.GroupBy(u => u.Age)
                                              .OrderBy(g => g.Key)
                                              .Select(g => new
                                              {
                                                  Age = g.Key,
                                                  GenderCounts = g.GroupBy(u => u.Gender)
                                                                  .Select(genderGroup => new
                                                                  {
                                                                      Gender = genderGroup.Key,
                                                                      Count = genderGroup.Count()
                                                                  })
                                              });

        Console.WriteLine("3. The number of genders per age, displayed from youngest to oldest:");
        foreach (var ageGroup in userGendersPerAgeAscending)
            Console.WriteLine($"Age: {ageGroup.Age}\t\t{string.Join(", ", ageGroup.GenderCounts.Select(gc => $"\"{gc.Gender}\": {gc.Count}"))}");

    }

}


