using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decagon4Christ.Model.Enums
{
    public enum Squad
    {
        Squad1 = 1,
        Squad2 = 2,
        Squad3 = 3,
        Squad4 = 4,
        Squad5 = 5,
        Squad6 = 6,
        Squad7 = 7,
        Squad8 = 8,
        Squad9 = 9,
        Squad10 = 10,
        Squad11 = 11,
        Squad12 = 12,
        Squad13 = 13,
        Squad14 = 14,
        Squad15 = 15,
        Squad16 = 16,
        Squad17 = 17,
        Squad18 = 18,
        Squad19 = 19,
        Squad20 = 20,
        Squad21 = 21,
        Squad22 = 22,
        Squad23 = 23,
        Squad24 = 24,
        Squad25 = 25,
    }

    public enum UserRole
    {
        Admin,
        Regular
    }

    public enum EmailPurpose
    {
        [Description("Registration")]
        Registration,

        [Description("PasswordReset")]
        PasswordReset,

        [Description("Newsletter")]
        Newsletter
    }

    public enum UserAction
    {
        Registration,
        PasswordReset,
        Newsletter
    }
}
