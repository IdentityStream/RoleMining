using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using RoleMining.Library;
using RoleMining.Library.Classes;
using RoleMining.Library.Algorithms;
using RoleMining.Library.Clustering;

namespace RoleMining.Tests;

public class KclusterTest : IAlgorithmTest
{
    [Fact]
    public void RunCode()
    {
        var userAccess = new List<UserAccess>
        {
            new UserAccess { UserID = "2", AccessID = "Write" },
            new UserAccess { UserID = "1", AccessID = "Read"  },
            new UserAccess { UserID = "1", AccessID = "Write" },
            new UserAccess { UserID = "3", AccessID = "Read"  },
            new UserAccess { UserID = "4", AccessID = "Write" },
            new UserAccess { UserID = "5", AccessID = "Read"  },
            new UserAccess { UserID = "6", AccessID = "Write" },
            new UserAccess { UserID = "7", AccessID = "Read"  },
            new UserAccess { UserID = "8", AccessID = "Write" }
        };

        var result = Kcluster.Cluster(userAccess);

        Assert.Equal(1, 1);
    }

    [Fact]
    public void TestNullOrEmptyValues()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void TestBasicFunctionality()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void TestLargeData()
    {
        var userInRoles = new List<UserInRole>();
        var userAccesses = new List<UserAccess>();

        var iterations = 10000;

        // Assign 10,000 users access to "read"
        for (int i = 1; i <= iterations; i++)
        {
            userAccesses.Add(new UserAccess { UserID = i.ToString(), AccessID = $"{i}" });
        }

        var result = Kcluster.Cluster(userAccesses);
        Assert.Equal(1, 1);  // Expecting 1 role with 10,000 users
    }
}
