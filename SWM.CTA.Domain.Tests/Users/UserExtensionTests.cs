using SWM.CTA.Domain.Users;

namespace SWM.CTA.Domain.Tests.Users;

public class UserExtensionTests
{
    [Theory]
    [InlineData("Jack", " Sparrow", "Jack Sparrow")]
    [InlineData("Jean-Luc  ", "Picard   ", "Jean-Luc Picard")]
    [InlineData("  Chewbacca", "", "Chewbacca")]
    [InlineData("", "  Newman", "Newman")]
    public void GetFullName_ReturnsFullNameTrimmed(string inFirst, string inLast, string expected)
    {
        // Arrange
        var user = new User() { First = inFirst, Last = inLast };

        // Act
        string actual = user.GetFullName();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetByAge_ReturnsCorrectCount()
    {
        // Arrange
        List<User> users = new()
        {
            new() { Age = 25 },
            new() { Age = 20 },
            new() { Age = 44 },
            new() { Age = 73 },
            new() { Age = 20 },
        };

        // Act
        IEnumerable<User> usersByAge = users.GetByAge(20);

        // Assert
        Assert.Equal(2, usersByAge.Count());
    }

    [Fact]
    public void GetGendersByAgeAscending_ReturnsCorrectCountAscending()
    {
        // Arrange
        List<User> users = new()
        {
            new() { Age = 35, Gender = "T" },
            new() { Age = 15, Gender = "M" },
            new() { Age = 15, Gender = "M" },
            new() { Age = 22, Gender = "F" },
            new() { Age = 22, Gender = "F" },
            new() { Age = 22, Gender = "T" },
        };
        
        // Act
        List<UserGenderGroupByAge> genderGroupsByAges = users.GetGendersByAgeAscending(false).ToList();

        // Assert
        Assert.Equal(15, genderGroupsByAges.First().Age);
        Assert.Equal(35, genderGroupsByAges.Last().Age);
        Assert.Equal(3, genderGroupsByAges.Count);
        Assert.Single(genderGroupsByAges.First().GenderCounts);
        Assert.Equal(2, genderGroupsByAges.Skip(1).First().GenderCounts.Count());
        Assert.Single(genderGroupsByAges.Last().GenderCounts);
        
    }
}