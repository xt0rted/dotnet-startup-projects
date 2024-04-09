namespace StartupProjects;

public class ConsoleConverter : WriteOnlyJsonConverter<IConsole>
{
    public override void Write(VerifyJsonWriter writer, IConsole value)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        var errorOutput = value.Error.ToString();
        var output = value.Out.ToString();

        writer.WriteStartObject();
        writer.WriteMember(value, value.IsErrorRedirected, nameof(value.IsErrorRedirected));
        writer.WriteMember(value, value.IsInputRedirected, nameof(value.IsInputRedirected));
        writer.WriteMember(value, value.IsOutputRedirected, nameof(value.IsOutputRedirected));

        if (!string.IsNullOrEmpty(errorOutput))
        {
            writer.WriteMember(value, errorOutput, nameof(value.Error));
        }

        if (!string.IsNullOrEmpty(output))
        {
            writer.WriteMember(value, output, nameof(value.Out));
        }

        writer.WriteEndObject();
    }
}
