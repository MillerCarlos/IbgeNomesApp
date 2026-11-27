using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IbgeNomesApp.Models;
using IbgeNomesApp.Services;

namespace IbgeNomesApp.ViewModels;

// Liga a tela (View) aos Services. Não conhece nenhum controle visual.
public class MainViewModel : INotifyPropertyChanged
{
    private readonly LocalidadesService _localidades;
    private readonly NomesService _nomes;

    // Contadores para ignorar respostas antigas se o usuário trocar de item rápido.
    private int _versaoNomes;
    private int _versaoMunicipios;
    private bool _inicializado;
    private bool _atualizandoMunicipios;

    public MainViewModel(LocalidadesService localidades, NomesService nomes)
    {
        _localidades = localidades;
        _nomes = nomes;
    }

    // ---------- Listas ligadas à tela ----------
    public ObservableCollection<Estado> Estados { get; } = new();
    public ObservableCollection<Municipio> Municipios { get; } = new();
    public ObservableCollection<Frequencia> Homens { get; } = new();
    public ObservableCollection<Frequencia> Mulheres { get; } = new();
    public ObservableCollection<Frequencia> Sobrenomes { get; } = new();

    // ---------- Itens selecionados ----------
    private Estado? _estadoSelecionado;
    public Estado? EstadoSelecionado
    {
        get => _estadoSelecionado;
        set
        {
            if (!SetField(ref _estadoSelecionado, value)) return;
            if (value is not null)
                _ = AoSelecionarEstadoAsync(value);
        }
    }

    private Municipio? _municipioSelecionado;
    public Municipio? MunicipioSelecionado
    {
        get => _municipioSelecionado;
        set
        {
            if (!SetField(ref _municipioSelecionado, value)) return;
            // Ignora quando a lista de municípios está sendo recarregada (valor nulo).
            if (value is not null && !_atualizandoMunicipios)
                _ = CarregarNomesAsync(value.Id, value.Nome);
        }
    }

    // ---------- Estado da tela ----------
    private bool _estaCarregando;
    public bool EstaCarregando
    {
        get => _estaCarregando;
        private set => SetField(ref _estaCarregando, value);
    }

    private string _mensagemErro = string.Empty;
    public string MensagemErro
    {
        get => _mensagemErro;
        private set
        {
            if (SetField(ref _mensagemErro, value))
                OnPropertyChanged(nameof(TemErro));
        }
    }
    public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

    private string _localidadeExibida = string.Empty;
    public string LocalidadeExibida
    {
        get => _localidadeExibida;
        private set => SetField(ref _localidadeExibida, value);
    }

    private bool _temResultados;
    public bool TemResultados
    {
        get => _temResultados;
        private set => SetField(ref _temResultados, value);
    }

    private bool _municipiosDisponiveis;
    public bool MunicipiosDisponiveis
    {
        get => _municipiosDisponiveis;
        private set => SetField(ref _municipiosDisponiveis, value);
    }

    // ---------- Ações ----------

    // Chamada quando a tela abre: carrega os estados no Picker.
    public async Task InicializarAsync()
    {
        if (_inicializado) return;
        _inicializado = true;

        EstaCarregando = true;
        MensagemErro = string.Empty;
        try
        {
            var estados = await _localidades.BuscarEstadosAsync();
            foreach (var e in estados) Estados.Add(e);
        }
        catch (ServicoException ex)
        {
            MensagemErro = ex.Message;
            _inicializado = false; // permite tentar de novo
        }
        catch (Exception)
        {
            MensagemErro = "Erro inesperado ao carregar os estados.";
            _inicializado = false;
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    // Ao escolher um estado: mostra os nomes do estado e carrega os municípios dele.
    private async Task AoSelecionarEstadoAsync(Estado estado)
    {
        await Task.WhenAll(
            CarregarNomesAsync(estado.Id, estado.Nome),
            CarregarMunicipiosAsync(estado.Id));
    }

    private async Task CarregarMunicipiosAsync(int estadoId)
    {
        int versao = ++_versaoMunicipios;

        _atualizandoMunicipios = true;
        MunicipioSelecionado = null;
        Municipios.Clear();
        MunicipiosDisponiveis = false;
        _atualizandoMunicipios = false;

        try
        {
            var lista = await _localidades.BuscarMunicipiosAsync(estadoId);
            if (versao != _versaoMunicipios) return; // resposta antiga

            _atualizandoMunicipios = true;
            foreach (var m in lista) Municipios.Add(m);
            _atualizandoMunicipios = false;
            MunicipiosDisponiveis = true;
        }
        catch (ServicoException ex)
        {
            if (versao == _versaoMunicipios) MensagemErro = ex.Message;
        }
        catch (Exception)
        {
            if (versao == _versaoMunicipios)
                MensagemErro = "Erro inesperado ao carregar os municípios.";
        }
        finally
        {
            _atualizandoMunicipios = false;
        }
    }

    // Consulta encadeada: usa o id da localidade na API de Nomes.
    private async Task CarregarNomesAsync(int idLocalidade, string nomeLocalidade)
    {
        int versao = ++_versaoNomes;

        EstaCarregando = true;
        MensagemErro = string.Empty;
        TemResultados = false;
        LimparListas();

        try
        {
            var dados = await _nomes.BuscarNomesAsync(idLocalidade);
            if (versao != _versaoNomes) return; // resposta antiga

            Preencher(Homens, dados.Homens);
            Preencher(Mulheres, dados.Mulheres);
            Preencher(Sobrenomes, dados.Sobrenomes);

            LocalidadeExibida = nomeLocalidade;
            TemResultados = true;
        }
        catch (ServicoException ex)
        {
            if (versao == _versaoNomes) MensagemErro = ex.Message;
        }
        catch (Exception)
        {
            if (versao == _versaoNomes)
                MensagemErro = "Erro inesperado ao carregar os nomes.";
        }
        finally
        {
            if (versao == _versaoNomes) EstaCarregando = false;
        }
    }

    private void LimparListas()
    {
        Homens.Clear();
        Mulheres.Clear();
        Sobrenomes.Clear();
    }

    private static void Preencher(ObservableCollection<Frequencia> destino, List<Frequencia> origem)
    {
        foreach (var item in origem) destino.Add(item);
    }

    // ---------- INotifyPropertyChanged ----------
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? nome = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));

    private bool SetField<T>(ref T campo, T valor, [CallerMemberName] string? nome = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor;
        OnPropertyChanged(nome);
        return true;
    }
}
