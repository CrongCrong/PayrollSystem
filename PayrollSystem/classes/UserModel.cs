using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem.classes
{
    public static class UserModel
    {
        public static string Name { get; set; }

        public static string Username { get; set; }

        public static string Password { get; set; }

        public static bool isAdmin { get; set; }

        public static bool isViewing { get; set; }

        public static string PIN { get; set; }
    }
}
