namespace MonitorAtivosB3.Excecoes;

public class ParametrosInvalidosException : Exception
{
    string? message;

    public ParametrosInvalidosException()
    {
    }

    public ParametrosInvalidosException(string? message) : base(message)
    {
        this.message = message;
    }
}
