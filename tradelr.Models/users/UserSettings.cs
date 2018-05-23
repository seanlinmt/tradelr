using System;

namespace tradelr.Models.users
{
    [Flags]
    public enum UserSettings
    {
        NONE = 0x0,
        PANEL_ACTIVITY_ACTIVE = 0x1, // 0001
        PASSWORD_RESET = 0X2, // 0010
        POST_TO_FACEBOOK = 0x4, // 0100
        POST_TO_GOOGLE = 0x8, // 1000
        POST_TO_BLOGGER = 0x10, // 1 0000
        POST_TO_TWITTER = 0x20, // 10 0000
        PANEL_TWITTER_ACTIVE = 0x40, // 100 0000
        PANEL_GETSTARTED_ACTIVE = 0x80, // 1000 0000
        PANEL_FACEBOOK_ACTIVE = 0x100,
        METRIC_VIEW = 0x200,
        //POST_TO_ETSY = 0x400,
        POST_TO_FACEBOOK_OWN = 0x800, // post to own facebook stream
        CREATE_ALBUM_FACEBOOK_OWN = 0x1000, // create photo album on own facebook stream
        POST_TO_EBAY = 0x2000,
        POST_TO_WORDPRESS = 0x4000,
        POST_TO_TUMBLR = 0x8000,
        POST_TO_TRADEME = 0X10000
    }

    [Flags]
    public enum UserSettingsMask
    {
        ACTIVITY_MASK = 0xfe3e,
        POSTTO_MASK = 0x01c1
    }

    public static class UserSettingsHelper
    {
        public static int ToIndex(this UserSettings paneltype)
        {
            var index = 0;
            switch (paneltype)
            {
                case UserSettings.PANEL_ACTIVITY_ACTIVE:
                    index = 1;
                    break;
                case UserSettings.PANEL_TWITTER_ACTIVE:
                    index = 2;
                    break;
                case UserSettings.PANEL_FACEBOOK_ACTIVE:
                    index = 3;
                    break;
                default:
                    break;
            }
            return index;
        }

    }

}
