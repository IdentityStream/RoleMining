namespace RoleMining.Tests;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RoleMining.Library;
using RoleMining.Library.Algorithms;
using RoleMining.Library.Classes;
using System;
using System.Collections.Generic;
using Xunit;

public class JaccardIndexTest
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAccessInRoleRecommender _jaccardIndex;

    // Constructor to set up the service provider
    public JaccardIndexTest()
    {
        // Configure the DI container
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRoleMining(); // Use your extension method to add services
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _jaccardIndex = _serviceProvider.GetService<IAccessInRoleRecommender>();
    }

    [Fact]
    public void Should_not_throw_when_inputs_are_valid()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "2", AccessID = "Write" },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "4", AccessID = "Write" },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "6", AccessID = "Write" },
            new UserAccess { UserID = "7", AccessID = "Read"  },
            new UserAccess { UserID = "8", AccessID = "Write" }
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
        var act = new Action(() => _jaccardIndex.CalculateScores(userAccesses, userInRoles));
        act.Should().NotThrow();
    }

    public static IEnumerable<object?[]> GetNullOrEmptyValues()
    {
        yield return new object?[] { null,
                                     null,
                                     typeof(ArgumentNullException) };
        yield return new object?[] { new List<UserAccess>(),
                                     new List<UserInRole>(),
                                     typeof(ArgumentException) };
        yield return new object?[] { new List<UserAccess> { new() { UserID = "", AccessID = "" } },
                                     null,
                                     typeof(ArgumentException) };
        yield return new object?[] { null,
                                     new List<UserInRole> { new() { UserID = "", RoleID = "" } },
                                     typeof(ArgumentNullException) };
        yield return new object?[] { new List<UserAccess> { new() { UserID = "", AccessID = "" } },
                                     new List<UserInRole> { new() { UserID = "", RoleID = "" } },
                                     typeof(ArgumentException) };
    }
    [Theory]
    [MemberData(nameof(GetNullOrEmptyValues))]
    public void Should_throw_when_inputs_are_null_or_empty(List<UserAccess> userAccesses, List<UserInRole> userInRoles, Type typeOfException)
    {
        try
        {
            _jaccardIndex.CalculateScores(userAccesses, userInRoles);
        }
        catch (Exception e)
        {
            e.Should().BeOfType(typeOfException);
        }
    }

    [Fact]
    public void Should_return_correct_JaccardIndex_for_given_few_inputs()
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

        var result = _jaccardIndex.CalculateScores(userAccesses, userInRoles);

        result.Should()
            .HaveCount(2)
            .And
            .Contain(result => result.RoleID == "A" && result.AccessID == "Read" && result.AccessToRoleScore == 0.25)
            .And
            .Contain(result => result.RoleID == "A" && result.AccessID == "Write" && result.AccessToRoleScore == 0.25)
            ;
    }

    [Fact]
    public void Should_return_correct_amount_for_100000_inputs()
    {
        var userInRoles = new List<UserInRole>();
        var userAccesses = new List<UserAccess>();

        var iterations = 100000;

        // Generate 10,000 users, all in role "A"
        for (int i = 1; i <= iterations; i++)
        {
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = $"{i}" });
        }

        // Assign 10,000 users access to "read"
        for (int i = 1; i <= iterations; i++)
        {
            userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = $"{i}" });
        }

        var result = _jaccardIndex.CalculateScores(userAccesses, userInRoles);
        result.Should().HaveCount(iterations);
    }

    [Fact]
    public void Should_return_perfect_JaccardIndex_when_all_users_have_access_and_role()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "7", AccessID = "Read"  },
        };

        var userInRoles = new List<UserInRole>
        {
            new UserInRole { UserID = "1", RoleID = "A" },
            new UserInRole { UserID = "3", RoleID = "A" },
            new UserInRole { UserID = "5", RoleID = "A" },
            new UserInRole { UserID = "7", RoleID = "A" },

        };
        var result = _jaccardIndex.CalculateScores(userAccesses, userInRoles);
        result.Should().HaveCount(1);
        result[0].AccessToRoleScore.Should().Be(1.0);
    }

    [Fact]
    public void Should_return_correct_amount_for_input_with_multiple_roles()
    {
        var userInRoles = new List<UserInRole>();
        var accessInRole = new List<AccessInRole>();
        var userAccesses = new List<UserAccess>();

        var iterations = 10000;

        // Simulate 100 users, each having multiple roles
        for (int i = 1; i <= iterations; i++)
        {
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = $"{i % 2}" });
            userInRoles.Add(new UserInRole { UserID = i.ToString(), RoleID = $"{i % 5}" });

            // Simulate extra accesses for half of the users
            if (i % 2 == 0)
            {
                userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = "execute" }); // Extra access
            }
        }

        var result = _jaccardIndex.CalculateScores(userAccesses, userInRoles);
        result.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Should_return_correct_JaccardIndex_for_multiple_roles()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "2", AccessID = "Read"  },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "4", AccessID = "Read"  },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "6", AccessID = "Read"  },
            // Total 6 users with access
            // 3 with role A and B
            // 2 with just role A
            // 1 with just role B
        };

        var userInRoles = new List<UserInRole>
        {
            // Role A
            new UserInRole { UserID = "1",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "2",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "3",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "A4", RoleID = "A" },
            new UserInRole { UserID = "5",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "6",  RoleID = "A" }, // Has access
            new UserInRole { UserID = "A7", RoleID = "A" },
            new UserInRole { UserID = "A8", RoleID = "A" },
            new UserInRole { UserID = "A9", RoleID = "A" },
            // 9 in role, 5 with access

            // Role B
            new UserInRole { UserID = "1",  RoleID = "B" }, // Has access
            new UserInRole { UserID = "2",  RoleID = "B" }, // Has access 
            new UserInRole { UserID = "3",  RoleID = "B" }, // Has access
            new UserInRole { UserID = "4",  RoleID = "B" }, // Has access
            new UserInRole { UserID = "B5", RoleID = "B" },
            new UserInRole { UserID = "B6", RoleID = "B" },
            new UserInRole { UserID = "B7", RoleID = "B" },
            new UserInRole { UserID = "B8", RoleID = "B" },
            new UserInRole { UserID = "B9", RoleID = "B" },
            new UserInRole { UserID ="B10", RoleID = "B" },
            new UserInRole { UserID ="B11", RoleID = "B" },
            new UserInRole { UserID ="B12", RoleID = "B" },
            new UserInRole { UserID ="B13", RoleID = "B" },
            new UserInRole { UserID ="B14", RoleID = "B" },
            // 14 in role, 4 with access
        };

        // Act
        var result = _jaccardIndex.CalculateScores(userAccesses, userInRoles);

        // Assert
        result.Should()
            .HaveCount(2)
            .And
            .Contain(r => r.RoleID == "A" && r.AccessID == "Read" && r.AccessToRoleScore == 0.5)
            .And
            .Contain(r => r.RoleID == "B" && r.AccessID == "Read" && r.AccessToRoleScore == 0.25);
    }
}
