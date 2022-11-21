namespace Qsi.Tests.Utilities;

public static class StringUtility
{
    public static ulong CalculateHash(string value)
    {
        ulong hashedValue = 3074457345618258791ul;

        foreach (var t in value)
        {
            hashedValue += t;
            hashedValue *= 3074457345618258799ul;
        }

        return hashedValue;
    }
}
