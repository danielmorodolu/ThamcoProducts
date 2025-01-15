# **README for Product Service Container**

## **Product Service**

### **Overview**

The Product Service container manages product-related data. It allows for retrieving product information, searching for products, and updating product details. The service can be accessed by other containers like the Profile Service to fetch user-related product data and can make outbound API calls to external services for product-related information.

This service is deployed to Azure and can be accessed via the following URL:
- **Product Service URL:** [https://product-thamco-dueuc8hug6b2g7cr.uksouth-01.azurewebsites.net/api/Product/Products](https://product-thamco-dueuc8hug6b2g7cr.uksouth-01.azurewebsites.net/api/Product/Products)

### **Features**
- **Product Management:** Retrieve, add, update, and delete products in the catalog.
- **Product Search:** Allows searching for products based on criteria such as name, category, and price.
- **Resilience:** Implements Polly for retry and circuit breaker patterns to handle service failures.
- **API Documentation:** Automatically generated using Swagger (via Swashbuckle).

### **Technologies**
- **ASP.NET Core:** Framework for building the web API and managing product-related endpoints.
- **Polly:** Used for handling retries and circuit breakers to ensure the service remains resilient.
- **Swagger (Swashbuckle):** Generates API documentation for endpoints.
- **xUnit / Moq:** Used for unit testing the controller actions and services.
- **HttpClient:** For making outbound HTTP calls to fetch product-related data from external services.
- **Azure Web App:** For cloud deployment.

### **Endpoints**

The following API endpoints are available in the Product Service:
- **GET /Products:** Retrieves a list of all products.
- **GET /Products/{id}:** Retrieves product details for a specific product by id.
- **POST /Products:** Adds a new product to the catalog (admin access required).
- **PUT /Products/{id}:** Updates an existing product by id.
- **DELETE /Products/{id}:** Deletes a product from the catalog.
- **GET /Products/Search:** Allows searching for products based on query parameters like name, category, price.

### **Data Model**

The Product Service uses the following data model:
- **Product:** Attributes include Id, Name, Category, Price, Description, and StockAvailability.

### **Setup and Configuration**

#### **Prerequisites**
- **.NET 9.0 SDK or later.**
- **SQL Server or SQLite for data persistence.**

#### **Configuration**
- Set up external API URLs (if applicable) and authentication credentials in `appsettings.json`.

#### **Running the Service**
1. **Clone the repository:**
git clone https://github.com/danielmorodolu/ThamcoProducts.git
cd ThamcoProducts

2.	Install dependencies:
    	dotnet restore
3. Run application
        dotnet run
Testing

Run unit tests using the following command:
dotnet test