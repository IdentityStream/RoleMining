# RoleMining.Library

A C# library for mining roles based on extra access lists.

## Features (For now)
- **MineRoles** method that inputs a list of UserAccess and UserInRole objects and outputs a list of RatioAccessLevel objects.
- **UserAccess** and **UserInRole** classes for input.
- **RatioAccessLevel** class for output.

## Syntax
**MineRoles**
```csharp
	RoleMining.MineRoles(IEnumerable<UserAccess> userAccesses, IEnumerable<UserInRole> userInRoles)
	// Return type is RatioAccessLevel
```

Classes
```csharp
    public class UserAccess
	{
		public string UserID { get; set; }
		public string RoleID { get; set; }
		public string AccessID { get; set; }
		public bool IsExtraAccess { get; set; }
	}

    public class UserInRole
	{
		public string RoleID { get; set; }
		public string UserID { get; set; }
	}
	
    public class RatioAccessLevel
	{
		public string RoleID { get; set; }
		public string AccessID { get; set; }
		public double Ratio { get; set; }
		public int UsersWithAccessAsExtra { get; set; }
		public int TotalUsers { get; set; }
	}
```