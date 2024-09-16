# RoleMining.Library

A C# library for mining roles from a RBAC system. The library aims to provide different algorithms to reduce the number of extra accesses given to users in a RBAC system, this by recommending accesses to be made standard in a role, or by recommending a new role to be created.

## Features (For now)
- **MineRoles** method that inputs a list of UserAccess and UserInRole objects and outputs a list of RatioAccessLevel objects.
- **JaccardIndices** method that inputs a list of UserAccess and UserInRole objects and outputs a list of JaccardIndex objects. This is useful for getting an overview of which extra accesses fit each role. Also contains the (homemade) Weighted Jaccard Index.
- **UserAccess** and **UserInRole** classes for input.
- **RatioAccessLevel** class for output.

## Syntax
**MineRoles**
```csharp
	RoleMining.MineRoles(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
	// Return type is RatioAccessLevel
```

**JaccardIndices**
```csharp
	RoleMining.JaccardIndices(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
	// Return type is JaccardIndex
```

Types
```csharp
    public class UserAccess
	{
		public string UserID { get; set; }
		public string RoleID { get; set; }
		public string AccessLevelID { get; set; }
		public bool IsExtraAccess { get; set; }
	}

    public class UserInRole
	{
		public string RoleID { get; set; }
		public string UserID { get; set; }
	}
	
    public class RatioAccessLevel
	{
		public string Role { get; set; }
		public string Access { get; set; }
		public double Ratio { get; set; }
		public int UsersWithAccessAsExtra { get; set; }
		public int TotalUsers { get; set; }
	}

	public class JaccardIndex
	{
        public double JaccardIndex { get; set; }
        public double WeightedJaccardIndex { get; set; }
        public string RoleID { get; set; }
        public string AccessID { get; set; }
        public int UsersWithAccess { get; set; }
        public int UsersWithoutAccess { get; set; }
	}
```