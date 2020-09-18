# How to create .Net Visual Studio Multi Projects Solution Template and deploy Nuget Package?

## Create Sample Project

* To begin with, we create our sample project by going to the folder where we keep our repositories.

```powershell

PS> mkdir Matech.Sample.Template
PS> cd Matech.Sample.Template

```

* Add some class library projects(Application,Domain,Infrastructure) and Web Api project.

```powershell

PS Matech.Sample.Template> dotnet new classlib -n Application
PS Matech.Sample.Template> dotnet new classlib -n Domain
PS Matech.Sample.Template> dotnet new classlib -n Infrastructure
PS Matech.Sample.Template> dotnet new webapi -n WebApi

```

* Add Solution item

```powershell

PS Matech.Sample.Template> dotnet new sln

```

* Add multiple C# projects to a solution `Matech.Sample.Template.sln` using a globbing pattern (Windows PowerShell only):

```powershell

PS Matech.Sample.Template> dotnet sln Matech.Sample.Template.sln add (ls -r **/*.csproj)

```

* Add multiple C# projects to a solution using a globbing pattern (Unix/Linux only):

```powershell

PS Matech.Sample.Template> dotnet sln Matech.Sample.Template.sln add **/*.csproj

```

You can check [dotnet sln](https://docs.microsoft.com/tr-tr/dotnet/core/tools/dotnet-sln).


### Matech.Sample.Template Project

![Matech.Sample.Template Project](/Project_Default.jpg "Default Project")

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

* Prerequest for Database Migrations
> If you don't have [Entity Framework Core tools reference - .NET Core CLI](https://docs.microsoft.com/tr-tr/ef/core/miscellaneous/cli/dotnet), Install

```powershell

PS Matech.Sample.Template> dotnet tool install --global dotnet-ef

```
> Add Domain reference to Infrastructure project

> Add Application and Infrastructure reference to WebApi project

> Dependencies
>> Infrastructure
>>> Microsoft.EntityFrameworkCore.SqlServer
>>>
>>> Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
>>
>>WebApi
>>> Microsoft.EntityFrameworkCore.Tools


* Add initial create for database.
> --project Infrastructure (optional if in this folder)

> --startup-project WebApi

> --output-dir Migrations

```powershell

PS Matech.Sample.Template> dotnet ef migrations add "SampleMigration" --project Infrastructure --startup-project WebApi --output-dir Migrations

```

Sample template project is finally ready :smile:

![Matech.Sample.Template Project](/Project_Final.jpg "Default Project")