namespace Base.Helper;

public static class EnumHelper
{
    public static int EnumIndex<T>(int value)
    {
        var i = 0;
        foreach (var v in Enum.GetValues(typeof(T)))
        {
            if ((int) v == value) return i;

            ++i;
        }

        return -1;
    }

    public static T FromString<T>(string str)
    {
        A.Ensure(Enum.IsDefined(typeof(T), str), des: $"str{str} not in {typeof(T)}");

        return (T) Enum.Parse(typeof(T), str);
    }
}