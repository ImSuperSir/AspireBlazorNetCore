dotnet add package Swashbuckle.AspNetCore


dotnet add reference ..\BlazorAspire.Model\BlazorAspire.Model.csproj
dotnet add reference ..\BlazorAspire.Database\BlazorAspire.Database.csproj


dotnet add package Microsoft.EntityFrameworkCore.SqlServer

/*Para que genere el codigo de los controllers*/


dotnet tool install -g dotnet-aspnet-codegenerator
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

/*Generando el controller*/
dotnet aspnet-codegenerator controller -name ProductController -outDir Controllers -async -api -dc AppDbContext

/*Para el uso de docker con la instancia de sql*/
$sa_password = ""         /*ESto se usar en powershell, solo para no estar repitiendo el password y mantenerlo in memory*/
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -p 1433:1433 -d -v sqlvolume:/var/opt/mssql --rm --name BlazorAspireSample mcr.microsoft.com/mssql/server:2022-latest



dotnet add package Microsoft.AspNetcore.Authentication.JwtBearer