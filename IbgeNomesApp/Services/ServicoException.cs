namespace IbgeNomesApp.Services;

// Erro "nosso", com mensagem pronta para mostrar ao usuário na tela.
public class ServicoException : Exception
{
    public ServicoException(string mensagem) : base(mensagem)
    {
    }
}