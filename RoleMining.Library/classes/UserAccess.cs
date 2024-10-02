namespace RoleMining.Library.Classes
{
    /// <summary>
    /// UserAccess object
    /// </summary>
    public class UserAccess
	{
        /// <summary>
        /// Unique identifier for each user.
        /// </summary>
		public string UserID { get; set; }

        /// <summary>
        /// The access level, something that is unique to each access and access level. Example: access: "Github", access level: "Admin". The access parameter would then be "Github - Admin".
        /// </summary>
		public string AccessID { get; set; }


        // FUNCTIONS USED BY HASHSET TO COMPARE OBJECTS

        /// <summary>
        /// Returns the hash code for the UserAccess object, combination of UserID, RoleID and AccessID.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return $"{UserID} - {AccessID}".GetHashCode();
        }

        /// <summary>
        /// Function to check if this UserAccess object is equal to another object.
        /// </summary>
        /// <param name="obj">The <seealso cref="UserAccess">UserAccess</seealso> object to be compared with.</param>
        /// <returns>A bool representing if they are equal or not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is UserAccess other)
            {
                return UserID == other.UserID && AccessID == other.AccessID;
            }
            return false;
        }
    }
}