using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BlazorAspire.Model;
using BlazorAspire.Model.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;

namespace BlazorAspire.Web.Components.Pages.Product;

public partial class UpdateProduct : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    private ApiClient ApiClient { get; set; }

    [Inject]
    NavigationManager NavigationManager { get; set; }

    [Inject]
    IToastService ToastService { get; set; }



    public ProductModel Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"/api/Product/{Id}");
        if (res != null && res.IsSuccess)
        {
            Model = JsonConvert.DeserializeObject<ProductModel>(res.Data.ToString());
           ///NavigationManager.NavigateTo("/product");
        }
    }

    public async Task Submit()
    {
        var res = await ApiClient.PutAsync<BaseResponseModel, ProductModel>($"/api/Product/{Id}", Model);
        if (res != null && res.IsSuccess)
        {
            ToastService.ShowSuccess("Product updated successfully.");
            NavigationManager.NavigateTo("/product");
        }
    }
}
