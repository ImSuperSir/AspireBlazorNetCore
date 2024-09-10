using System;
using System.Threading.Tasks;
using BlazorAspire.Model;
using BlazorAspire.Model.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorAspire.Web.Components.Pages.Product;

public partial class CreateProduct
{
    public ProductModel Model { get; set; } = new();

    [Inject]
    private ApiClient ApiClient { get; set; }
    
    [Inject]
    private IToastService ToastService { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    public async Task Submit()
    {
        var res = await ApiClient.PostAsync<BaseResponseModel, ProductModel>("/api/Product", Model);
        if(res != null && res.IsSuccess)
        {
            ToastService.ShowSuccess("Product created successfully.");
            NavigationManager.NavigateTo("/product");
            // Redirect to index page
        }
    }




}
