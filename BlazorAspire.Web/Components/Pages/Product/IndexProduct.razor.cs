using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAspire.Model;
using BlazorAspire.Model.Entities;
using BlazorAspire.Web.Components.BaseComponents;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace BlazorAspire.Web.Components.Pages.Product;

public partial class IndexProduct
{
    [Inject]
    public ApiClient _ApiClient { get; set; }

    public List<ProductModel> ProductModels { get; set; }

    public AppModal Modal { get; set; }

    public int DeleteId { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {

        await base.OnInitializedAsync();
        await LoadProducts();
    }

    protected async Task LoadProducts()
    {
        var res = await _ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/Product");
        if (res != null && res.IsSuccess)
        {
            ProductModels = JsonConvert.DeserializeObject<List<ProductModel>>(res.Data.ToString());
        }
    }

    protected async Task HandleDelete()
    {
        var res = await _ApiClient.DeleteAsync<BaseResponseModel>($"/api/Product/{DeleteId}");
        if(res != null && res.IsSuccess)
        {
            ToastService.ShowSuccess("Product deleted successfully.");
            await LoadProducts();
            Modal.Close();
        }
    }

}
