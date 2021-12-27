using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Identity;
using ProductManagementApi.Models;
using ProductManagementApi.Models.DataTransferObjects;
using ProductManagementApi.Repository.Interfaces;
using ProductManagementApi.Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    //
    public class ProductController : BaseController
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = _productRepository.GetAll();  
             if(products.Count() == 0) return Response<string>(data: null, message: "No Products", ApiResponseCode.NOTFOUND);

            return ApiResponse(data: products);
        }

        // GET: api/<controller>
        [HttpGet("disabled")]
        [Authorize(Roles=Identity.Roles.Admin)]
        public IActionResult GetDisabled()
        {           
            var products = _productRepository.GetAll(x=>x.Disabled == true);
            if (products.Count() == 0) return Response<string>(data: null, message: "No Disabled Products", ApiResponseCode.NOTFOUND);

            return ApiResponse(data: products);
        }


        // GET: api/<controller>
        [HttpGet("totalPrices")]
        public IActionResult GetTotalPrices()
        {
            var lastWeek = DateTime.Now.Date.AddDays(-7);
            var productPrices = _productRepository.CalculateAllProductPrices(lastWeek);
            if (productPrices.Items.Count() == 0) return Response<string>(data: null, message: "No Products Added", ApiResponseCode.NOTFOUND);

            return ApiResponse(data: productPrices);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null || product.Disabled) return Response<string>(data: null, message: "No Product with that Id was found", ApiResponseCode.NOTFOUND);

            return ApiResponse(data: product);
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = Identity.Roles.Admin)]
        public async Task<IActionResult> Post([FromBody]ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return Response<string>(data: null, message: "", ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());
            }
            try
            {
                var product = new Product
                { Id=Guid.NewGuid(),
                  Name=productDto.Name,
                  Price=productDto.Price,
                  Disabled=false
                };
                _productRepository.Create(product);
                return ApiResponse(product);
            }
            catch (Exception ex)
            {

                return Response<string>(data: null, message: "", ApiResponseCode.INVALID_REQUEST, errors: ex.Message);
            }

        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Roles = Identity.Roles.Admin)]
        public async Task<IActionResult> Put(Guid id, [FromBody]ProductEditDto productDto)
        {
            var product = await _productRepository.GetById(id);
            if (product == null) 
                return Response<string>(data: null, message: "No Product with that Id was found", ApiResponseCode.NOTFOUND);
            product.Name = product.Name != productDto.Name ? productDto.Name : product.Name;
            product.Price = product.Price != productDto.Price ? productDto.Price : product.Price;

            _productRepository.Edit(product);
            return ApiResponse(data: "Edited "+product.Name);
        }


        // PUT api/<controller>/5/disable
        [HttpPut("{id}/disable")]
        public async Task<IActionResult> Disable(Guid id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
                return Response<string>(data: null, message: "No Product with that Id was found", ApiResponseCode.NOTFOUND);
            _productRepository.Disable(product);

            return ApiResponse(data: "disabled "+product.Name);
        }

        //// PUT api/<controller>/5/enable
        //[HttpPut("{id}/enable")]
        //public void Enable(int id)
        //{
        //}

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Identity.Roles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
                return Response<string>(data: null, message: "No Product with that Id was found", ApiResponseCode.NOTFOUND);

            _productRepository.Delete(product);
            return ApiResponse(data: "deleted "+product.Name);
        }
    }
}
