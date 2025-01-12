using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using ProductService.ProductRepository;



namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        
        private readonly ILogger<ProductController> _logger;
        private IProductService _productService;
     

        

        public ProductController(ILogger<ProductController> logger, IProductService productSerivce)
        {
            _logger = logger;
            _productService = productSerivce;
        }

        //get products 

        [HttpGet("Products")]
        [Authorize]
        public async Task<IActionResult> Products()

        {
            IEnumerable<ProductDto> products = null!;

            try{

                products = await _productService.GetProductsAsync();

            }
            catch{

                _logger.LogWarning("failure to access undercutters service ");
                products= Array.Empty<ProductDto>();

            }

            return Ok(products.ToList());

        }
       

        }
    
}