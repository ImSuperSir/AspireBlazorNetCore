
dotnet add reference ..\BlazorAspire.Model\BlazorAspire.Model.csproj


 dotnet add package NewtonSoft.Json

 dotnet add package Blazored.Toast
        builder.Services.AddBlazoredToast();

        // This is in _imports.razor
        @using Blazored.Toast   
        @using Blazored.Toast.Services
        //this is in the MainLayout
        @using Blazored.Toast.Configuration

        <BlazoredToast Position="ToastPosition.TopRight"
            Timeout="5000"
            IconType="IconType.FontAwesome"
            SuccessClass="success-toast-override"
            SuccessIcon="fa fa-thumbs-up"
            ErrorIcon="fa fa-bug"
        />

//authentication
dotnet add package Microsoft.AspNetcore.Authentication.JwtBearer  --> JwtSecurityTokenHandler
        