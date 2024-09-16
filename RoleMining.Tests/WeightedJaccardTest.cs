namespace RoleMining.Tests;

using RoleMining.Library;
using RoleMining.Library.Classes;
using System.Collections.Generic;
using Xunit;

public class WeightedJaccardTest
{

    [Fact]
    public void RunCode()
    {
        var userAccesses = new List<UserAccess>
        {
            new UserAccess { UserID = "1", AccessID = "Read", },
            new UserAccess { UserID = "2", AccessID = "Write",},
            new UserAccess { UserID = "3", AccessID = "Read", },
            new UserAccess { UserID = "4", AccessID = "Write",},
            new UserAccess { UserID = "5", AccessID = "Read", },
            new UserAccess { UserID = "6", AccessID = "Write",},
            new UserAccess { UserID = "7", AccessID = "Read", },
            new UserAccess { UserID = "8", AccessID = "Write",},
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
        var jaccard = RoleMining.JaccardIndices(userAccesses, userInRoles);
        Assert.Equal(1, 1);
    }

    [Fact]
    public void TestJaccardIndices()
    {
        // Arrange
        var userAccesses = new List<UserAccess>
            {
                new UserAccess { UserID = "1", AccessID = "Read" },
                new UserAccess { UserID = "2", AccessID = "Write"},
                //new UserAccess { UserID = "3", RoleID = "B", AccessID = "Read", IsExtraAccess = false },
                new UserAccess { UserID = "4", AccessID = "Delete"}
            };

        var userInRoles = new List<UserInRole>
            {
                new UserInRole { UserID = "1", RoleID = "A" },
                new UserInRole { UserID = "2", RoleID = "A" },
                new UserInRole { UserID = "3", RoleID = "B" },
                new UserInRole { UserID = "4", RoleID = "B" }
            };

        // Act
        var result = RoleMining.JaccardIndices(userAccesses, userInRoles);

        // Assert
        var user1Role1Access1 = result.FirstOrDefault(j => j.RoleID == "A" && j.AccessID == "Read");
        var user2Role1Access2 = result.FirstOrDefault(j => j.RoleID == "A" && j.AccessID == "Write");
        // var user3Role2Access1 = result.FirstOrDefault(j => j.RoleID == "B" && j.AccessID == "Read");
        var user4Role2Access3 = result.FirstOrDefault(j => j.RoleID == "B" && j.AccessID == "Delete");

        Assert.NotNull(user1Role1Access1);
        Assert.Equal(0.5, user1Role1Access1.JaccardIndex);

        Assert.NotNull(user2Role1Access2);
        Assert.Equal(0.5, user2Role1Access2.JaccardIndex);

        Assert.NotNull(user4Role2Access3);
        Assert.Equal(0.5, user4Role2Access3.JaccardIndex);
    }

    [Fact]
    public void TestWeightedJaccardIndex_LowAmountOfExtraAccess()
    {
        // Test Case: 10 users in the role, 1 user with the access
        int amountOfUsersInRole = 10;
        int amountOfUsersWithAccess = 1;

        // Expected penalty: 10 - 1 / 10 = 0.9, 1 - 0.9 = 0.1
        // Expected reward: log(1+1) = log(2)
        // Expected weight = 0.1 * log(2) = 0.1 * 0.693 = 0.0693

        var expectedWeight = 0.1 * Math.Log10(2);

        // Call the method
        var actualWeight = RoleMining.CalculateJaccardWeight(amountOfUsersInRole, amountOfUsersWithAccess);

        // Assert that the calculated weight matches the expected weight
        Assert.Equal(expectedWeight, actualWeight, precision: 4); // Precision to 4 decimal places
    }

    [Fact]
    public void TestWeightedJaccardIndex_HighAmountOfExtraAccess()
    {
        // Test Case: 10 users in the role, 9 users with the access
        int amountOfUsersInRole = 10;
        int amountOfUsersWithAccess = 9;

        // Expected penalty: 10 - 9 / 10 = 0.1, 1 - 0.1 = 0.9
        // Expected reward: log(9+1) = log(10)
        // Expected weight = 0.9 * log10(10) = 0.9 * 1 = 0.9

        var expectedWeight = 0.9 * Math.Log10(10);

        // Call the method
        var actualWeight = RoleMining.CalculateJaccardWeight(amountOfUsersInRole, amountOfUsersWithAccess);

        // Assert that the calculated weight matches the expected weight
        Assert.Equal(expectedWeight, actualWeight, precision: 4); // Precision to 4 decimal places
    }
}