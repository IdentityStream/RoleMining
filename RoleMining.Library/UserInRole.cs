using System;
using System.Collections.Generic;

namespace RoleMining.Library
{
    public class UserInRole
    {
        public string RoleID { get; set; }
        public string UserID { get; set; }
        public override int GetHashCode()
        {
            return $"{UserID} - {RoleID}".GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is UserInRole other)
            {
                return RoleID == other.RoleID && UserID == other.UserID;
            }
            return false;
        }
    }
}