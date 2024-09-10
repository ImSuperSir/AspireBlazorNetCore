using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAspire.Database.Data;
using BlazorAspire.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorAspire.BL;


public interface IProductRepository
{
    Task<List<ProductModel>> GetProductsAsync();
    Task<ProductModel> GetProductByIdAsync(int id);
    Task<ProductModel> CreateProductAsync(ProductModel productModel);
    Task<bool> ProductModelExistsAsync(int id);
    Task UpdateProductAsync(ProductModel productModel);
    Task DeleteProductAsync(int id);
}
public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<List<ProductModel>> GetProductsAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task<ProductModel> GetProductByIdAsync(int id)
    {
       return await context.Products.FirstOrDefaultAsync( x => x.Id == id);
    }
    public async Task<ProductModel> CreateProductAsync(ProductModel productModel)
    {
        context.Products.Add(productModel);
        await context.SaveChangesAsync();
        return productModel;
    }

    public Task<bool> ProductModelExistsAsync(int id)
    {
        return context.Products.AnyAsync(e => e.Id == id);
    }

    public async Task UpdateProductAsync(ProductModel productModel)
    {
        context.Entry(productModel).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}
