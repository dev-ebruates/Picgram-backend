namespace Domain.Extensions;

public static class EnumExtension
{
  public static string GetEnumDescription(this Enum value)
  {
    if (value == null)
      return string.Empty;

    FieldInfo field = value.GetType().GetField(value.ToString());

    DescriptionAttribute attribute =
        field.GetCustomAttribute<DescriptionAttribute>();

    return attribute == null ? value.ToString() : attribute.Description;
  }
}
