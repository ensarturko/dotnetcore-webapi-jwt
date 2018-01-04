using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApiUndockerized.Data;
using CoreApiUndockerized.Data.Entities;
using CoreApiUndockerized.Filters;
using CoreApiUndockerized.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreApiUndockerized.Controllers
{
    [Authorize]
    [EnableCors("AnyGET")]
    [ValidateModel]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private IProductRepository _repo;
        private IMapper _mapper;
        private ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repo,
            IMapper mapper, ILogger<ProductsController> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var products = _repo.GetAllProducts();

            return Ok(_mapper.Map<IEnumerable<ProductModel>>(products));
        }

        [HttpGet("{id}", Name= "GetProduct")]
        public IActionResult Get(int id)
        {
            try
            {
                var product = _repo.GetProduct(id);

                if (product == null) return NotFound($"Product {id} was not found.");

                return Ok(_mapper.Map<IEnumerable<ProductModel>>(product)); 
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
            
        [EnableCors("Ensar")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Product model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                _repo.Add(model);

                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("GetProduct", new {id = model.Id});
                    return Created(newUri, model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving product: {ex}");
            }

            return BadRequest();
        }

        [HttpPut("{id}")] 
        public async Task<IActionResult> Put(int id, [FromBody] Product model)
        {
            try
            {
                var oldProduct = _repo.GetProduct(id);
                if (oldProduct == null) return NotFound($"Could not find a product with an ID of {id}"); 

                // Map the model to the old product
                oldProduct.Title = model.Title ?? oldProduct.Title;
                oldProduct.Price = model.Price;

                if (await _repo.SaveAllAsync())
                {
                    return Ok(oldProduct);  
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while updating product: {ex}");
            }

            return BadRequest("Couldn't update Product");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oldProduct = _repo.GetProduct(id);
                if (oldProduct == null) return NotFound($"Could not find a product with an ID of {id}");

                _repo.Delete(oldProduct);

                if (await _repo.SaveAllAsync())
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while deleting the product: {ex}");
            }

            return BadRequest("Couldn't delete Product");

        }
    }
}