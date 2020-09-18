# How to create .Net Visual Studio Multi Projects Solution Template and deploy Nuget Package?

## Create Sample Project

To begin with, we create our sample project by going to the folder where we keep our repositories. I am using [dotnet CLI](https://docs.microsoft.com/tr-tr/dotnet/core/tools/) with powershell in Windows 10 but you can use different terminals and OS (MACOS, Linux etc.) 

```powershell
PS> mkdir Matech.Sample.Template
PS> cd Matech.Sample.Template
```

Add some class library projects (Application, Domain, Infrastructure) and Web Api project.

```powershell
PS Matech.Sample.Template> dotnet new classlib -n Application
PS Matech.Sample.Template> dotnet new classlib -n Domain
PS Matech.Sample.Template> dotnet new classlib -n Infrastructure
PS Matech.Sample.Template> dotnet new webapi -n WebApi
```

Add Solution item

```powershell
PS Matech.Sample.Template> dotnet new sln
```

Add multiple C# projects to a solution `Matech.Sample.Template.sln` using a globbing pattern (Windows PowerShell only):

```powershell
PS Matech.Sample.Template> dotnet sln Matech.Sample.Template.sln add (ls -r **/*.csproj)
```

Add multiple C# projects to a solution using a globbing pattern (Unix/Linux only):

```powershell
PS Matech.Sample.Template> dotnet sln Matech.Sample.Template.sln add **/*.csproj
```

You can check [dotnet sln](https://docs.microsoft.com/tr-tr/dotnet/core/tools/dotnet-sln).


### Matech.Sample.Template Project

![Matech.Sample.Template Project](Project_Default.jpg "Default Project")

Let's go to the folder where we created our project and add new classes to our project.

`Domain` will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer. If you interested in Clean Architecture, you can check my public open source [solution template for creating a ASP.NET Core Web Api following the principles of Clean Architecture](https://github.com/iayti/CleanArchitecture).

```csharp
namespace Domain.Entities
{
    using System.Collections.Generic;

    public class City 
    {
        public City()
        {
            Districts = new List<District>();
        }

        public int Id { get; set; }

        public string Name { get; set; }


        public IList<District> Districts { get; set; }
    }
}
```

```csharp
namespace Domain.Entities
{
    public class District 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

    }
}
```

`Infrastructure` layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

```csharp
namespace Infrastructure
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=<SQLSERVER>; Database=Sample.TemplateDB; User Id = <YOUR_ID>; Password=<YOUR_PASSWORD>; MultipleActiveResultSets=true;");
        }
    }
}
```

### Prerequest for Database Migrations
* If you don't have [Entity Framework Core tools reference - .NET Core CLI](https://docs.microsoft.com/tr-tr/ef/core/miscellaneous/cli/dotnet), Install

```powershell
PS Matech.Sample.Template> dotnet tool install --global dotnet-ef
```
* Add Domain reference to Infrastructure project

* Add Application and Infrastructure reference to WebApi project

* Dependencies
    * Infrastructure
        * Microsoft.EntityFrameworkCore.SqlServer
        * Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
    * WebApi
        * Microsoft.EntityFrameworkCore.Tools


Add initial create for database.
* `--project Infrastructure` (optional if in this folder)

* `--startup-project WebApi`

* `--output-dir Migrations`

```powershell
PS Matech.Sample.Template> dotnet ef migrations add "SampleMigration" --project Infrastructure --startup-project WebApi --output-dir Migrations
```

Sample template project is finally ready :smile:

![Matech.Sample.Template Project](Project_Final.jpg "Default Project")

### Adding .template.config and testing locally.

The example below demonstrates the file and folder structure of using a template.json to create a template pack. Check the [microsoft docs](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates).

```
Matech.Sample.Template
|___.template.config
    |___template.json
|___Application
|___Domain
|___Infrastructure
|___WebApi
|___Matech.Sample.Template.sln
```

Configure your `template.json` and that's it :smile:. 

```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "ilker Ayti", // author name
  "classifications": [], // Web/MVC/WebApi
  "name": "Matech Sample Template",
  "identity": "Matech.Sample.Template", // Unique identity for Nuget
  "groupIdentity": "Matech.Sample.Template",
  "shortName": "mst", // short name for dotnet new mat creating new project using this template
  "tags": {
    "language": "C#",
    "type": "project"
  },
  "sourceName": "MatechSample",
  "preferNameDirectory": true,
  "sources": [
    {
      "source": "./",
      "target": "./",
      "exclude": [
        "**/README.md",
        "**/nuget.config",
        "**/LICENSE",
        "**/.gitattributes",
        "**/.gitignore",
        "**/*.nupkg",
        "**/[Bb]in/**",
        "**/[Oo]bj/**",
        ".template.config/**/*",
        ".vs/**/*",
        "**/*.filelist",
        "**/*.user",
        "**/*.lock.json",
        "**/.git/**",
        "**/.github/**",
        "*.nuspec",
        "**/node_modules/**"
      ]
    }
  ]
}
```

Let's try our template locally.

```powershell
PS > dotnet new --install <Project Folder Path>\Matech.Sample.Template
```

![Matech.Sample.Template Project](Dotnet_CLI_Local_Template.jpg "Default Project")

Create new project using our local template.

```powershell
PS> mkdir Test_Local_Sample_Template
PS> cd Test_Local_Sample_Template
PS Test_Local_Sample_Template> dotnet new mst
```

Project was created :smile:

![Test.Sample.Project](Test_Sample_Project.jpg "Sample Project")

Check the project and if anything is wrong, change the settings you made and install template locally for testing again.

Uninstall Template

```powershell
PS> dotnet new --uninstall
PS> dotnet new -u <Project Folder Path>\Matech.Sample.Template
```

## Configure Nuget Deployment