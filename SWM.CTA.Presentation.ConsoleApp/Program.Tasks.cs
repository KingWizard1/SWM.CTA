using System.Diagnostics;
using SWM.CTA.Domain.Users;

public static partial class Program
{
    #region Assignment Tasks

    private static void _PrintUsersFullName(IEnumerable<User> users, long userId)
    {
        // 1. Output User 42's Full Name
        User? user = users.FirstOrDefault(u => u.Id == userId);  // Would normally use SingleOrDefault here
        if (user != null)
            Console.WriteLine($"1. The full name for user with id '{userId}' is '{user.GetFullName()}'");
        else
            Console.WriteLine($"1. Cannot print the full name for user with id '{userId}': user does not exist");
    }

    private static void _PrintUsersFirstNamesByAge(IEnumerable<User> users, int age)
    {
        // 2. All user's first names, comma seperated, who are 23 years old
        int ageToGet = 23;
        ICollection<User> usersByAge = users.GetByAge(ageToGet).ToList();
        if (usersByAge.Count > 0)
        {
            Console.WriteLine($"2. All first names for user's who are {ageToGet} years old, comma seperated:");
            Console.WriteLine(string.Join(", ", usersByAge.Select(u => u.First)));
        }
        else
            Console.WriteLine($"2. Cannot print first names for user's who are {ageToGet} years old: there aren't any.");
    }

    private static void _PrintNumberOfGendersPerAgeAscending(IEnumerable<User> users, bool useParallelProcessing)
    {
        // 3. The number of genders per Age, displayed from youngest to oldest.
        Console.WriteLine($"3. The number of genders per age, sorted by youngest to oldest, {(useParallelProcessing ? "using AsParallel()" : "without using AsParallel()")}:");

        var sw = Stopwatch.StartNew();
        IEnumerable<UserGenderGroupByAge> userGendersPerAgeAscending = users.GetGendersByAgeAscending(useParallelProcessing);
        sw.Stop();
        
        foreach (UserGenderGroupByAge ageGroup in userGendersPerAgeAscending)
            Console.WriteLine($"Age: {ageGroup.Age}\t\t{string.Join(", ", ageGroup.GenderCounts.Select(gc => $"\"{gc.Gender}\": {gc.Count}"))}");
        
        Console.WriteLine($"{(useParallelProcessing ? string.Empty : "Non-")}Parallel operation time: {sw.Elapsed}");
    }
    
    #endregion

}


