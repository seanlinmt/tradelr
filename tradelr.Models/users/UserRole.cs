using System;

namespace tradelr.Models.users
{
    // stored as name in databas because even if bit value change, name still stays the same
    [Flags]
    public enum UserRole : int
    {
        // bits
        NONE = 0,
        USER = 0x2, // normal users incl. store users
        CREATOR = 0x4,
        UNVERIFIED = 0x8,
        GOD = 0x10,

        // combinations
        ADMIN = CREATOR | USER, // 6
        TENTATIVE = ADMIN | UNVERIFIED, // 14
        SEAN = ADMIN | GOD  // 22

    }
}
