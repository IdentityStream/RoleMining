namespace RoleMining.Library
{
    /// <summary>
    /// UserAccess object
    /// </summary>
    public class UserAccess
	{
		public string UserID { get; set; }
		public string RoleID { get; set; }
		public string AccessLevelID { get; set; }
		public bool IsExtraAccess { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is UserAccess other)
            {
                return UserID == other.UserID && RoleID == other.RoleID && AccessLevelID == other.AccessLevelID && IsExtraAccess == other.IsExtraAccess;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return $"{UserID} - {RoleID} - {AccessLevelID}".GetHashCode();
        }
    }
}