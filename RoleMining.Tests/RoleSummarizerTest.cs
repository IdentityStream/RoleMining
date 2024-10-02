namespace RoleMining.Tests;

using FluentAssertions;
using RoleMining.Library.Algorithms;
using RoleMining.Library.Classes;
using System.Collections.Generic;
using Xunit;

public class RoleSummarizerTest
{
    [Fact]
    public void Should_not_throw_when_inputs_are_valid()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1",  AccessID = "Read" },
            new UserAccess { UserID = "4",  AccessID = "Write"}
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "A" },
            new UserInRole { UserID = "2", RoleID = "A" },
            new UserInRole { UserID = "3", RoleID = "A" },
            new UserInRole { UserID = "4", RoleID = "A" }
        };

        var act = () => RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);
        act.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetNullOrEmptyValues()
    {
        yield return new object[] { null,
                                    null,
                                    typeof(ArgumentNullException) };
        yield return new object[] { new List<UserAccess> { new() { UserID = "", AccessID = "" } },
                                    null,
                                    typeof(ArgumentException) };
        yield return new object[] { null,
                                    new List<UserInRole> { new() { UserID = "", RoleID = "" } },
                                    typeof(ArgumentNullException) };
        yield return new object[] { new List<UserAccess> { new() { UserID = "", AccessID = "" } },
                                    new List<UserInRole> { new() { UserID = "", RoleID = "" } },
                                    typeof(ArgumentException) };
    }
    [Theory]
    [MemberData(nameof(GetNullOrEmptyValues))]
    public void Should_throw_when_inputs_are_null_or_empty(List<UserAccess> userAccesses, List<UserInRole> userInRoles, Type typeOfException)
    {
        // Act
        Action act = () => RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);

        // Assert
        if (typeOfException == typeof(ArgumentNullException))
        {
            act.Should().Throw<ArgumentNullException>();
        }
        else if (typeOfException == typeof(ArgumentException))
        {
            act.Should().Throw<ArgumentException>();
        }
        else
        {
            throw new InvalidOperationException("Unexpected exception type");
        }
    }

    [Fact]
    public void Should_return_correct_values_for_given_few_inputs()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"},
            new UserAccess { UserID = "4", AccessID = "Write" }
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "A" },
            new UserInRole { UserID = "2", RoleID = "A" },
            new UserInRole { UserID = "3", RoleID = "A" },
            new UserInRole { UserID = "4", RoleID = "A" }
        };

        var result = RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);

        result.Should()
            .HaveCount(2)
            .And
            .Contain(result => result.RoleID == "A" && result.AccessID == "Read" && result.Ratio == 0.25)
            .And
            .Contain(result => result.RoleID == "A" && result.AccessID == "Write" && result.Ratio == 0.25);
    }



    [Fact]
    public void Should_return_correct_values_for_input_with_duplicates()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read" },
            new UserAccess { UserID = "2", AccessID = "Read" },
            new UserAccess { UserID = "3", AccessID = "Write" },
            new UserAccess { UserID = "3", AccessID = "Write" },
            new UserAccess { UserID = "3", AccessID = "Write" },
            new UserAccess { UserID = "4", AccessID = "Write" },
            new UserAccess { UserID = "4", AccessID = "Read" },
            new UserAccess { UserID = "4", AccessID = "Read" }
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "Admin" },
            new UserInRole { UserID = "1", RoleID = "Admin" },
            new UserInRole { UserID = "1", RoleID = "Admin" },
            new UserInRole { UserID = "1", RoleID = "Admin" },
            new UserInRole { UserID = "2", RoleID = "Admin" },
            new UserInRole { UserID = "2", RoleID = "User" },
            new UserInRole { UserID = "2", RoleID = "User" },
            new UserInRole { UserID = "3", RoleID = "User" },
            new UserInRole { UserID = "4", RoleID = "User" },
            new UserInRole { UserID = "4", RoleID = "Admin" },
            new UserInRole { UserID = "5", RoleID = "Admin" },
            new UserInRole { UserID = "5", RoleID = "Admin" },
            new UserInRole { UserID = "5", RoleID = "Admin" },
            new UserInRole { UserID = "5", RoleID = "Admin" },
            new UserInRole { UserID = "6", RoleID = "User" }
        };

        var result = RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);

        result.Should()
            .Contain(result => result.RoleID == "Admin" && result.AccessID == "Read" && result.Ratio == 0.75)
            .And
            .Contain(result => result.RoleID == "User" && result.AccessID == "Write" && result.Ratio == 0.5);
    }

    [Fact]
    public void Should_return_correct_values_for_1000_inputs()
    {
        var userInRoles = new List<UserInRole>();
        var userAccesses = new List<UserAccess>();

        // Simulate 100 users, each having multiple roles
        for (int i = 1; i <= 1000; i++)
        {
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = "A" });
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = "B" });

            // Simulate extra accesses for half of the users
            if (i % 2 == 0)
            {
                userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = "execute" }); // Extra access
            }
        }

        // Run the role mining algorithm on this large dataset
        var result = RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);

        // Only two roles, and one extra access: 2x1. Also verify that users who were supposed to have extra access actually appear in the results
        result.Should()
            .HaveCount(2)
            .And
            .Contain(r => userAccesses.Any(user => r.AccessID == user.AccessID && r.RoleID != null));
    }

    [Fact]
    public void Should_return_correct_values_for_input_with_multiple_roles()
    {
        var userAccesses = new List<UserAccess>
    {
        new UserAccess { UserID = "1", AccessID = "Read"},
        new UserAccess { UserID = "3", AccessID = "Read"},
        new UserAccess { UserID = "5", AccessID = "Read"},
        new UserAccess { UserID = "7", AccessID = "Read"},
    };

        var userInRoles = new List<UserInRole>
    {
        new UserInRole { UserID = "1", RoleID = "A" },
        new UserInRole { UserID = "2", RoleID = "A" },
        new UserInRole { UserID = "3", RoleID = "B" },
        new UserInRole { UserID = "4", RoleID = "B" },
        new UserInRole { UserID = "5", RoleID = "C" },
        new UserInRole { UserID = "6", RoleID = "C" },
        new UserInRole { UserID = "7", RoleID = "D" },
        new UserInRole { UserID = "8", RoleID = "D" }
    };

        var result = RoleSummarizer.SummarizeRoles(userAccesses, userInRoles);

        result.Should()
            .HaveCount(4)
            .And
            .Contain(r => r.RoleID == "A" && r.AccessID == "Read" && r.Ratio == 0.5)
            .And
            .Contain(r => r.RoleID == "B" && r.AccessID == "Read" && r.Ratio == 0.5)
            .And
            .Contain(r => r.RoleID == "C" && r.AccessID == "Read" && r.Ratio == 0.5)
            .And
            .Contain(r => r.RoleID == "D" && r.AccessID == "Read" && r.Ratio == 0.5);
    }
}