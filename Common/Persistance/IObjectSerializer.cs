namespace Code1.Common.Persistance
{
    public interface IObjectSerializer
    {
        T DeserializeObject<T>(string text);
        string SerializeObject(object value);
    }
}