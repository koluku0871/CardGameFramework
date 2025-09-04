using System;

public static class TimeUtil
{

    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    // DateTime‚©‚çUnixTime‚Ö•ÏŠ·
    public static long GetUnixTime(DateTime dateTime)
    {
        return (long)(dateTime - UnixEpoch).TotalSeconds;
    }

    // UnixTime‚©‚çDateTime‚Ö•ÏŠ·
    public static DateTime GetDateTime(long unixTime)
    {
        return UnixEpoch.AddSeconds(unixTime);
    }
}