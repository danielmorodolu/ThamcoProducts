using System;

namespace ProductService.ProductRepository;

//Interface for getting products

public interface IProductService{

    Task<IEnumerable<ProductDto>> GetProductsAsync();
}