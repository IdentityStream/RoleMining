namespace RoleMining.Tests;

using RoleMining.Library;
using RoleMining.Library.Algorithms;
using RoleMining.Library.Classes;
using System.Collections.Generic;
using Xunit;

public class RoleSummarizerTest : IAlgorithmTest
{
    [Fact]
    public void RunCode()
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

        var result = RoleMining.MineRoles(userAccesses, userInRoles);
        Assert.Equal(1, 1);
    }

    [Fact]
    public void TestNullOrEmptyValues()
    {
        var userAccesses = new List<UserAccess>();
        var userInRoles = new List<UserInRole>();

        var exception1 = Assert.Throws<ArgumentException>(() => RoleMining.MineRoles(userAccesses, userInRoles));
        Assert.Equal("userAccesses", exception1.ParamName);

        var exception2 = Assert.Throws<ArgumentNullException>(() => RoleMining.MineRoles(null, null));
        Assert.Equal("userAccesses", exception2.ParamName);
    }

    [Fact]
    public void TestBasicFunctionality()
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

        var result = RoleMining.MineRoles(userAccesses, userInRoles);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Read" && r.Ratio == 0.25);
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Write" && r.Ratio == 0.25);
    }



    [Fact]
    public void TestDuplicateRemoval()
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

        var result = RoleMining.MineRoles(userAccesses, userInRoles);
        // We want Role A with Read and Write
        // We want Role B with Read and Write

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, r => r.RoleID == "Admin" && r.AccessID == "Read" && r.Ratio == 0.75);
        Assert.Contains(result, r => r.RoleID == "User" && r.AccessID == "Write" && r.Ratio == 0.5);
    }

    [Fact]
    public void TestLargeData()
    {
        var userInRoles = new List<UserInRole>();
        var accessInRole = new List<AccessInRole>();
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
        var result = RoleMining.MineRoles(userAccesses, userInRoles);

        // Expecting 2 role - extra access relations (Only two roles, and one extra access: 2x1)
        Assert.Equal(2, result.Count);

        // Verify that users who were supposed to have extra access actually appear in the results
        foreach (var user in userAccesses)
        {
            Assert.Contains(result, r => r.AccessID == user.AccessID && r.RoleID != null);
        }

        // Verify that none of the standard accesses (read, write) appear in the extra accesses list
        Assert.DoesNotContain(result, r => r.AccessID == "read" || r.AccessID == "write");
    }

    [Fact]
    public void TestMultipleRoles()
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

        var result = RoleMining.MineRoles(userAccesses, userInRoles);

        Assert.Equal(4, result.Count);
        Assert.Contains(result, r => r.RoleID == "A" && r.AccessID == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.RoleID == "B" && r.AccessID == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.RoleID == "C" && r.AccessID == "Read" && r.Ratio == 0.5);
        Assert.Contains(result, r => r.RoleID == "D" && r.AccessID == "Read" && r.Ratio == 0.5);
    }


}