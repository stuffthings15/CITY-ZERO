using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class StringFloatPair
    {
        public string key;
        public float value;
    }

    [Serializable]
    public sealed class StringIntPair
    {
        public string key;
        public int value;
    }

    [Serializable]
    public sealed class StringStringPair
    {
        public string key;
        public string value;
    }
}
