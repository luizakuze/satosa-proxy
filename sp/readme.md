# Autenticação com SAML2 em C#  

 
Esta aplicação ASP.NET Core implementa um provedor de serviço (SP) que autentica usuários via protocolo SAML2, utilizando a biblioteca Sustainsys.Saml2.

### Sumário
 
  - [Estrutura do Projeto](#estrutura-do-projeto)
  - [Instalação](#instala%C3%A7%C3%A3o)
  - [Preparando ambiente para novo contexto](#preparando-ambiente-para-novo-contexto)
      - [Gerar novos certificados](#1-gerar-novos-certificados)
      - [Configurações no arquivo `appsettings.json`](#2-configurações-do-arquivo-appsettingsjson)

## Estrutura do Provedor de Serviço

```bash
.
├── AttributeMap                # Mapeamento de atributos SAML  
│   └── SamlUri.cs              
├── Certificates                 # Certificados para assinatura e encriptação das asserções SAML 
│   ├── mycert.crt               # Certificado público
│   ├── mycert.key               # Chave privada correspondente ao certificado
│   └── newcert.pfx              # Certificado + chave combinados em formato PFX (usado pelo SP)
├── Controllers                  # Lógica dos endpoints da aplicação
│   ├── HomeController.cs        
│   ├── LogoutController.cs       
│   └── UsersController.cs        
├── Views                        # Páginas HTML renderizadas pelos controllers
│   ├── Home
│   │   └── Index.cshtml            
│   └── Users
│       └── Index.cshtml        
├── appsettings.json             # Configurações da aplicação (logs, ambiente e variáveis)
├── saml-csharp.csproj           # Arquivo de configuração do projeto C# (.NET)
├── Program.cs                   # Ponto de entrada da aplicação e configuração do serviço 
├── metadata-sp.xml              # Metadado do SP
└── readme.md                    
```
 

## Preparando ambiente para novo contexto

Caso deseje apenas testar a aplicação, esta etapa não é obrigatória.  


### 1. Gerar novos certificados  
 
> 📝 Embora o repositório já inclua certificados prontos, é recomendável gerar os seus próprios.

   1. **Remover os certificados atuais do diretório `Certificates`**:

      ```bash
      rm Certificates/*
      ```

   2. **Gerar novos certificados**:

      Os comandos abaixo geram os certificados utilizados para assinar e encriptar as asserções SAML, e os colocam no diretório `Certificates`.

      ```bash
      # Criar a chave privada
      openssl genrsa -out Certificates/mykey.key 2048

      # Criar o certificado a partir da chave
      openssl req -new -key Certificates/mykey.key -out Certificates/mycert.csr
      openssl x509 -req -days 365 -in Certificates/mycert.csr -signkey Certificates/mykey.key -out Certificates/mycert.crt

      # Gerar o arquivo .pfx (contendo chave + certificado)
      openssl pkcs12 -export \
      -out Certificates/newcert.pfx \
      -inkey Certificates/mykey.key \
      -in Certificates/mycert.crt \
      -certfile Certificates/mycert.crt
      ```

      > 📝 É possível utilizar o mesmo certificado `.pfx` para **encriptação** e **assinatura** das asserções. Entretanto, você pode repetir esse processo para gerar certificados separados.


### 2. Configurações do arquivo `appsettings.json`

Essas configurações são lidas dinamicamente pela aplicação para configurar os certificados e informações do SP no arquivo `Program.cs`. Abra o arquivo `appsettings.json` e configure conforme o contexto desejado:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Saml": {
    "Sp": {
      "Fqdn": "sp-csharp-local",       // FQDN do Service Provider
      "Port": "5001",                  // Porta onde o SP será exposto
      "Certificates": {
        "Encryption": "newcert.pfx",   // Nome do certificado para encriptação
        "Signing": "newcert.pfx"       // Nome do certificado para assinatura
      }
    }
  }
}
```

- `Logging`: Define os níveis de log da aplicação. Não é necessário alterar para o uso padrão.
- `AllowedHosts`: Define os domínios permitidos. Usar `"*"` permite todos, o que é comum em ambiente local.
- `Saml.Sp.Fqdn`: Define o Fully Qualified Domain Name (ex: `sp-csharp-local`) usado pelo SP.
- `Saml.Sp.Port`: Define a porta usada pelo SP (ex: `5001`).
- `Saml.Sp.Certificates.Encryption`: Nome do arquivo `.pfx` que será usado para **encriptação das asserções SAML**.
- `Saml.Sp.Certificates.Signing`: Nome do arquivo `.pfx` usado para **assinatura das asserções SAML**.

> 💡 No seu contexto, podem ser necessárias alterações no `Fqdn`, `Port` ou nos nomes dos certificados caso utilize arquivos diferentes de `newcert.pfx`.

 
Pronto! Agora o ambiente está preparado para uso com certificados personalizados e configuração flexível via `appsettings.json`.

