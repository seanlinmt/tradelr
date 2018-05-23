using System;
using System.Text;
using tradelr.Library.geo;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class user
    {
        partial void OnCreated()
        {
            created = DateTime.UtcNow;
            lastLogin = DateTime.UtcNow;
            settings = (int)UserSettings.NONE;
        }
    }
}
