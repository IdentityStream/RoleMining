using System;
using System.Collections.Generic;
using System.Text;

namespace RoleMining.Library
{
    public class Jaccard
    {
        public double JaccardIndex { get; set; }
        public double WeightedJaccardIndex { get; set; }
        public string RoleID { get; set; }
        public string AccessID { get; set; }
        public int UsersWithAccess { get; set; }
        public int UsersWithoutAccess { get; set; }
    }
}
