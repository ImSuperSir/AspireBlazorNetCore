using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BlazorAspire.Model.Entities;

namespace BlazorAspire.BL;


public interface IProductService
{

    Task<List<ProductModel>> GetProductsAsync();
    Task<ProductModel> GetProductByIdAsync(int id);
    Task<ProductModel> CreateProductAsync(ProductModel model);
    // Task UpdateProductAsync(ProductModel model);
    Task<bool> ProductModelExistsAsync(int id);
    Task UpdateProductAsync(ProductModel productModel);
    Task DeleteProductAsync(int id);
}
public class ProductService(IProductRepository productRepository) : IProductService
{

    public async Task<List<ProductModel>> GetProductsAsync()
    {
        return await productRepository.GetProductsAsync();
    }

    public async Task<ProductModel> GetProductByIdAsync(int id)
    {
        return await productRepository.GetProductByIdAsync(id);
    }

    public async Task<ProductModel> CreateProductAsync(ProductModel productModel)
    {
        return await productRepository.CreateProductAsync(productModel);
    }

    public async Task<bool> ProductModelExistsAsync(int id)
    {
        return await productRepository.ProductModelExistsAsync(id);
    }

    public async Task UpdateProductAsync(ProductModel productModel)
    {
        await productRepository.UpdateProductAsync(productModel);
    }

    public async Task DeleteProductAsync(int id)
    {
        await productRepository.DeleteProductAsync(id);
    }
}
