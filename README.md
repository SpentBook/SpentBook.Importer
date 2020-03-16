# File Importer

## Depend�ncias

https://www.nuget.org/packages/morelinq/

## Entity Framework (Migrations)

Esse projeto utiliza o Migration com as seguintes caracter�sticas:

* Usando um `DbContext` que est� fora do projeto principal (startup project). Isso pode acontecer facilmente quando usamos, por exemplo, uma arquitetura como `Clean Architerure`
* Separa��o dos artefatos de migra��o em outro assembly (outro projeto). A ideia � n�o poluir nenhuma camada com as classes do migration.
* Cria��o de um mecanismo que suporte m�ltiplos banco de dados usando o Migration.

Links importantes:

* https://github.com/bricelam/Sample-SplitMigrations
* https://github.com/ivanpaulovich/clean-architecture-manga
* https://garywoodfine.com/using-ef-core-in-a-separate-class-library-project/

### Passo a passo para adicionar uma nova migra��o em um determinado banco de dados

� importante executar os comandos na raiz do `.csproj` inicial da aplica��o (startup project).

1. Atualize o `appsettings.json` do projeto principal (console, web). Localize a configura��o `AppSettings:DataBaseName` e altere para: `MySql` ou `SqlServer`.

```
cd src/SpentBook.Importer.Bradesco
vim appsettings.json
```

2. Criar uma migra��o quando usamos o MySql (`AppSettings:DataBaseName=Mysql`)


```
cd src/SpentBook.Importer.Bradesco
./Migrations/AddMysql
```

Ou chame o comando nativo diretamente:

```
dotnet ef migrations add "NOME_MIGRACAO" --project ../SpentBook.Migrations.MySql
```

3. Criar uma migra��o quando usamos o SqlServer (`AppSettings:DataBaseName=SqlServer`)

```
cd src/SpentBook.Importer.Bradesco
./Migrations/AddSqlServer
```

Ou chame o comando nativo diretamente:

```
dotnet ef migrations add "NOME_MIGRACAO" --project ../SpentBook.Migrations.SqlServer
```

4. Atualizar o banco de dados (ambos)

```
cd src/SpentBook.Importer.Bradesco
./Migrations/Update
```

Ou chame o comando nativo diretamente:

```
dotnet ef database update
```

### Como configurar esse mecanismo em outro projeto

1. Adicione a tool do entity framework no projeto principal (startup-project):

```
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

2. No mesmo projeto, adicione o pacote de design do EF:

```
dotnet add package Microsoft.EntityFrameworkCore.Design --version 3.1.2
```

3. Separa o `DbContext` do projeto principal, por exemplo, crie o projeto "MyProject.Infra" e mova o `DbContext` para l�

4. No projeto principal, adicione a refer�ncia do projeto "MyProject.Infra" e crie uma classe que vai "descobrir" qual `DbContext` deve ser usado, no caso, o que movemos para o projeto de infra:

```csharp
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var appSettings = new AppSettings();
        configuration.GetSection("AppSettings").Bind(appSettings);
            
        var options = new DbContextOptionsBuilder<DatabaseContext>();
        DatabaseExtensions.ConfigureDbContextOptions(configuration, appSettings, options);

        return new DatabaseContext(options.Options);
    }
}
```

5. No projeto de infra, adicione as refer�ncias do EF e dos banco de dados que deseja usar com o EF:

```
dotnet add package Microsoft.EntityFrameworkCore --version 3.1.2
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 3.1.2
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 3.1.1
```

6. No projeto de infra, crie uma extension para o `ServiceCollection` e adicione o `DbContext` do banco de dados que est� configurado no `appsettings.json`:

```csharp
public static class DatabaseExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>((provider, options) =>
        {
            var appSettings = provider.GetRequiredService<IOptions<AppSettings>>();
            ConfigureDbContextOptions(configuration, appSettings.Value, options);
        });

        services.AddTransient<ITransactionRepository, TransactionRepository>();

        return services;
    }

    public static void ConfigureDbContextOptions(IConfiguration configuration, AppSettings appSettings, DbContextOptionsBuilder options)
    {
        switch (appSettings.DataBaseName)
        {
            case "MySql":
                options.UseMySql(
                    connectionString: configuration.GetConnectionString("DatabaseMySql"),
                    mySqlOptionsAction: opt => opt.MigrationsAssembly(appSettings.MigrationAssemblyMySql)
                );
                break;
            default:
                options.UseSqlServer(
                    connectionString: configuration.GetConnectionString("DatabaseSqlServer"),
                    sqlServerOptionsAction: opt => opt.MigrationsAssembly(appSettings.MigrationAssemblySqlServer)
                );
                break;
        }
    }
}
```

7. Cria o projeto `SpentBook.Migrations.MySql` e `SpentBook.Migrations.SqlServer` e adicione, em ambos, a refer�ncia ao projeto de infra. Esses projetos v�o conter as classes do migration.

8. Adicione as refer�ncias dos projetos `SpentBook.Migrations.MySql` e `SpentBook.Migrations.SqlServer` ao projeto principal.

9. Adicione no m�todo `Startup.cs.Configure()` a chamada do m�todo `Migrate()` para for�ar as migra��es sempre que executar a aplica��o pela primeira vez.

```csharp
public void Configure(ServiceProvider serviceProvider)
{
    var dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}
```

ou garanta que todas as migra��es criadas foram aplicadas com o comando: 

```
dotnet ef database update
```

# Openshift install

oc new-app 'dotnet:3.1~https://github.com/SpentBook/SpentBook.Importer.Bradesco.git' \
    --name=importer-ofx \
    --context-dir src \
    --build-env DOTNET_STARTUP_PROJECT=SpentBook.Importer.Bradesco/SpentBook.Importer.Bradesco.csproj \
    --build-env DOTNET_CONFIGURATION=Release