# Monitor Ativos B3

![.NET CI](https://github.com/gav1ao/monitor-ativos-b3/actions/workflows/dotnet.yml/badge.svg)

<div align="center">
    <img src="https://img.shields.io/badge/C%23-12.0-blue" alt="C# Version">
    <img src="https://img.shields.io/badge/.NET-8.0-blueviolet" alt=".NET Version">
</div>

<div align="center">
  <h3 align="center">Monitor Ativos B3</h3>
  <p align="center">
    Uma aplicação para monitoramento automático de ativos negociados na B3.
    <br />
    <a href="https://github.com/gav1ao/monitor-ativos-b3"><strong>Explore a documentação »</strong></a>
    <br />
    <br />
    <a href="https://github.com/gav1ao/monitor-ativos-b3/issues">Reportar Bug</a>
    ·
    <a href="https://github.com/gav1ao/mmonitor-ativos-b3/issues">Solicitar Feature</a>
  </p>
</div>

---

## Sobre o Projeto

Monitoramento automatizado de ativos da B3, com alertas por e-mail e integração com a BraveAPI.

## Funcionalidades

- Consulta automática de preços de ativos da B3
- Envio de alertas por e-mail quando o preço atinge valores de referência

## Serviços utilizados

- [BraveAPI](https://brapi.dev/): Utilizado para obter informações dos ativos da B3, principalmente o preço;
- [MailTrap](https://mailtrap.io/): Utilizado para envio de e-mails via SMTP;

## Como rodar localmente

1. Clone o repositório:
   ```bash
   git clone https://github.com/gav1ao/monitor-ativos-b3.git
   cd monitor-ativos-b3
   ```

2. Configure o arquivo `appsettings.json` com seus dados de e-mail e BraveAPI.

   > **Dica:** Utilize o arquivo `appsettings.sample.json` como referência para criar o seu `appsettings.json`.

   > **Importante:** Não se esqueça de preencher corretamente os tokens de acesso da BraveAPI e as credenciais de e-mail antes

3. Execute o projeto:
   ```bash
   dotnet run --project MonitorAtivosB3 <TICKER> <PRECO_VENDA> <PRECO_COMPRA>
    ```

## Testes

Para rodar os testes unitários:
```bash
dotnet test
```

## Docker

Para rodar via Docker:
```bash
docker build -t monitor-ativos-b3 .
docker run \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -v $(pwd)/appsettings.json:/app/appsettings.json \
  monitor-ativos-b3 <TICKER> <PRECO_VENDA> <PRECO_COMPRA>
```

Exemplo:
```bash
docker run \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -v $(pwd)/appsettings.json:/app/appsettings.json \
  monitor-ativos-b3 PETR4 40 20
```

## Docker Compose

Você pode rodar o projeto facilmente usando Docker Compose.
Exemplo de uso:

1. Ajuste os argumentos em `command` no arquivo `docker-compose.yml` conforme necessário.
2. Execute o serviço:
   ```bash
   docker-compose up --build
   ```
3. O serviço irá utilizar o arquivo `appsettings.json` da raiz do projeto para configuração.

O arquivo `docker-compose.yml` já está configurado para montar o arquivo de configuração e passar os argumentos necessários

## Variáveis de ambiente

Configure as variáveis de ambiente conforme necessário ou utilize o `appsettings.json` para:

- BraveAPI (BaseUrl, Token)
- E-mail (Server, Port, Username, Password, EnableSsl, EmailRemetente, EmailDestinatario)
- Monitoramento (FrequenciaMonitoramentoPrecosMinutos)

## Contribuição

Pull requests são bem-vindos! Para grandes mudanças, abra uma issue primeiro para discutir o que você gostaria de modificar.

Depois:

1. Faça um fork do projeto.
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFeature`).
3. Commit suas mudanças (`git commit -m 'Adiciona NovaFeature'`).
4. Faça um push para a branch (`git push origin feature/NovaFeature`).
5. Abra um Pull Request.

---

Feito com ❤️ (e muito café) por Vítor Gavião - [@vigal_jan](https://x.com/vigal_jan) - vitor.gaviao@protonmail.com
