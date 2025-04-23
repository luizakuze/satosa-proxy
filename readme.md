## Satosa proxy: fluxo SAML2 ↔ login social

Esta aplicação contém um provedor de serviço (SP) ASP.NET Core que utiliza o protocolo **SAML2** para realizar autenticação com diferentes provedores de login social que utilizam **OAuth2** e **OpenID Connect (OIDC)**: Facebook e Google. 

### Ferramentas
- [Biblioteca Sustainsys (C#)](https://saml2.sustainsys.com/en/v2/)
- [Proxy SATOSA](https://github.com/IdentityPython/SATOSA)
 
### Estrutura do projeto
```bash
.
├── saml2-social                            # configurações do proxy satosa
│   ├── internal_attributes.yaml
│   ├── metadata
│   │   ├── frontend_facebook.xml
│   │   ├── frontend_google.xml
│   │   └── sp.xml
│   ├── pki
│   │   ├── frontend.crt
│   │   └── frontend.key
│   ├── plugins
│   │   ├── facebook_backend.yaml
│   │   ├── google_backend.yaml
│   │   └── mirror_frontend.yaml
│   └── proxy_conf.yaml
├── sp                                      # provedor de serviço
│   ├── appsettings.json
│   ├── AttributeMap
│   │   └── SamlUri.cs 
│   ├── Certificates
│   │   ├── mycert.crt
│   │   ├── mykey.key
│   │   └── newcert.pfx
│   ├── Controllers
│   │   ├── HomeController.cs
│   │   ├── LogoutController.cs
│   │   └── UsersController.cs
│   ├── Program.cs
│   ├── saml-csharp.csproj
│   ├── saml-csharp.sln
│   └── Views
│       ├── Home
│       │   └── Index.cshtml
│       └── Users
└──         └── Index.cshtml
```

### Como executar

1. **Clone o repositório:**

    ```bash
    git clone https://github.com/luizakuze/satosa-proxy
    cd satosa-proxy
    ```

2. **Configure os provedores de login social:**

    - **Google:**  
    Acesse o [Google Cloud Console](https://console.cloud.google.com/), crie um novo OAuth Client ID e copie o `client_id` e o `client_secret` para o arquivo ```saml2-social/plugins/google_backend.yaml```

    - **Facebook:**  
    Acesse o [Facebook for Developers](https://developers.facebook.com/), registre um novo aplicativo e insira as credenciais no arquivo ```saml2-social/plugins/facebook_backend.yaml```

3. **Execute a aplicação do SP (ASP.NET Core):**

    ```bash
    dotnet restore sp/saml-csharp.csproj
    dotnet run --project sp/saml-csharp.csproj

    ```

4. **Inicie o SATOSA com Gunicorn usando o script de execução:**

    ```bash
    chmod +x run.sh
    ./run.sh
    ```

---

### Script gerador de metadados

Caso deseje obter os metadados gerados pelo satosa para cada backend configurado, execute:

```bash
satosa-saml-metadata saml2-social/proxy_conf.yaml \
saml2-social/pki/frontend.key \
saml2-social/pki/frontend.crt \
--split-frontend \
--split-backend \
--dir saml2-social/metadata
```
