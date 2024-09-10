using System.Threading.Tasks;
using BlazorAspire.BL;
using BlazorAspire.Model;
using BlazorAspire.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace BlazorAspire.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController(IProductService productService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<BaseResponseModel>> GetProducts()
        {
            var proucts = await productService.GetProductsAsync();
            return Ok(new BaseResponseModel { IsSuccess = true, Data = proucts });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseModel>> GetProductById(int id)
        {
            var prouctModel = await productService.GetProductByIdAsync(id);
            if(prouctModel == null)
            {
                return Ok(new BaseResponseModel { IsSuccess = false, ErrorMessage = "Product not found." });
            }
            return Ok(new BaseResponseModel { IsSuccess = true, Data = prouctModel });
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseModel>> CreateProduct(ProductModel model)
        {
            await productService.CreateProductAsync(model);
            return Ok(new BaseResponseModel { IsSuccess = true });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponseModel>> UpdateProduct(int id, ProductModel productModel)
        {
            if(id != productModel.Id && !await productService.ProductModelExistsAsync(id))
            {
                return Ok(new BaseResponseModel { IsSuccess = false, ErrorMessage = "Product not found, Bad Request." });
            }
            await productService.UpdateProductAsync(productModel);
            return Ok(new BaseResponseModel { IsSuccess = true });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponseModel>> DeleteProduct(int id)
        {
            if(!await productService.ProductModelExistsAsync(id))
            {
                return Ok(new BaseResponseModel { IsSuccess = false, ErrorMessage = "Product not found, Bad Request." });
            }
            await productService.DeleteProductAsync(id);
            return Ok(new BaseResponseModel { IsSuccess = true });
        }

    }
}
