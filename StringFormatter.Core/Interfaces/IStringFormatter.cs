namespace StringFormatter.Core.Interfaces
{
    public interface IStringFormatter
    {
        string Format(string template, object target);
    }
}
