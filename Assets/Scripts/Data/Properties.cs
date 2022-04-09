using System;

namespace Koapower.KoafishTwitchBot.Data
{
    public abstract class Property { }

    public class Property<T> : Property
    {
        public T value;
    }

    [Serializable]
    public class StringProperty : Property<string>
    {
        public StringProperty() { }
        public StringProperty(string value)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class BoolProperty : Property<bool>
    {
        public BoolProperty() { }
        public BoolProperty(bool value)
        {
            this.value = value;
        }
    }
}
