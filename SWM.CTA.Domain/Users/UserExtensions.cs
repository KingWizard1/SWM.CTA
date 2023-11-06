using System.Collections.Generic;
using System.Linq;

namespace SWM.CTA.Domain.Users;

public record UserGenderGroupByAge(int Age, IEnumerable<UserGenderCount> GenderCounts);

public record UserGenderCount(string Gender, int Count);

public static class UserExtensions
{
    /// <summary>Joins the first and last names of the given user and returns the result.</summary>
    /// <remarks>The name will be trimmed in case either first or last name fields are empty.</remarks>
    public static string GetFullName(this User user) => $"{user.First} {user.Last}".Trim();

    /// <summary>Returns all users who are of the given age.</summary>
    public static IEnumerable<User> GetByAge(this IEnumerable<User> users, int age) => users.Where(u => u.Age == age).ToList();

    /// <summary>Groups all users by age, and provides a count for all genders within each age group.</summary>
    /// <param name="useParallelProcessing">Pass true to enable parallel processing of large data sets.</param>
    public static IEnumerable<UserGenderGroupByAge> GetGendersByAgeAscending(this IEnumerable<User> users, bool useParallelProcessing) =>
        (useParallelProcessing ? users.AsParallel() : users).GroupBy(u => u.Age)
                                                            .OrderBy(g => g.Key)
                                                            // Allocating up to 100 objects here, one for each age group.
                                                            .Select(g => new UserGenderGroupByAge(
                                                                g.Key,
                                                                g.GroupBy(u => u.Gender)
                                                                 .Select(genderGroup => new UserGenderCount(genderGroup.Key, genderGroup.Count())))
                                                            );
        
}