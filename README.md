# IbgeNomesApp

Aplicativo .NET MAUI que consome duas APIs públicas do IBGE em cadeia e mostra os nomes mais comuns de homens e mulheres, e os sobrenomes mais comuns, em cada estado ou município brasileiro (Censo 2022).

## Integrantes

- Miller Carlos
- Vinícius Lemes Cazelato

## Como funciona

1. Ao abrir, o app busca os estados na API de Localidades e preenche o dropdown.
2. Ao escolher um estado, o app busca os municípios dele e preenche o segundo dropdown.
3. Ao escolher um estado ou município, o app usa o `id` da localidade para consultar a API de Nomes e exibe as listas de nomes masculinos, femininos e sobrenomes, ordenadas do mais frequente para o menos frequente (top 10 de cada).

## APIs utilizadas

- Localidades: `https://servicodados.ibge.gov.br/api/v1/localidades/estados` e `/estados/{id}/municipios`
- Nomes (Censo 2022): `https://servicodados.ibge.gov.br/api/v3/nomes/2022/localidade/{id_localidade}`

## Arquitetura em camadas

- **Models/**: classes que mapeiam o JSON (`Estado`, `Municipio`, `Frequencia`, `NomesLocalidade`).
- **Services/**: acesso HTTP e tratamento de erros (`IbgeApiClient`, `LocalidadesService`, `NomesService`).
- **ViewModels/**: liga a tela aos Services (`MainViewModel`).
- **Tela (MainPage.xaml)**: `Picker`, `ActivityIndicator` e `CollectionView`.

## Tratamento de erros

O app mostra mensagens claras para: sem internet, tempo de resposta esgotado, API fora do ar, localidade sem dados e resposta em formato inesperado.

## Como executar

Pré-requisitos:

- Visual Studio 2022 ou mais recente, com a carga de trabalho **Desenvolvimento de interface do usuário multiplataforma .NET (.NET MAUI)**
- SDK do .NET 11
- Emulador Android configurado (ou dispositivo físico)

Passos:

1. Clone o repositório: `git clone https://github.com/MillerCarlos/IbgeNomesApp.git`
2. Abra o arquivo `IbgeNomesApp.slnx` no Visual Studio.
3. Selecione o emulador Android como destino.
4. Pressione **F5**.

O emulador precisa de acesso à internet.

## Observações

- A API de Nomes pode não retornar sobrenomes para todas as localidades. Nesse caso, a tela mostra um aviso.

## Vídeo de demonstração

LINK DO VÍDEO AQUI
