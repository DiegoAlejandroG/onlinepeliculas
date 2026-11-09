using api.DTOs;
using api.Entidades;
using api.Servicios;
using api.Utilidades;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/productoras")]
    public class ProductorasController : ControllerBase
    {
        private const string cacheTag = "productoras";
        private const string contenedor = "productoras";

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public ProductorasController(
            ApplicationDbContext context,
            IMapper mapper,
            IOutputCacheStore outputCacheStore,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<ProductoraDTO>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Productoras.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            return await queryable
                .OrderBy(productora => productora.Nombre)
                .Paginar(paginacion)
                .ProjectTo<ProductoraDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObtenerProductoraPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<ProductoraDTO>> Get(int id)
        {
            var productora = await context.Productoras
                .ProjectTo<ProductoraDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(productora => productora.Id == id);

            if (productora is null)
            {
                return NotFound();
            }

            return productora;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductoraCreacionDTO productoraCreacionDTO)
        {
            var productora = mapper.Map<Productora>(productoraCreacionDTO);

            if (productoraCreacionDTO.Logo is not null)
            {
                productora.Logo = await almacenadorArchivos.Almacenar(contenedor, productoraCreacionDTO.Logo);
            }

            context.Add(productora);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            var productoraDTO = mapper.Map<ProductoraDTO>(productora);
            return CreatedAtRoute("ObtenerProductoraPorId", new { id = productora.Id }, productoraDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] ProductoraCreacionDTO productoraCreacionDTO)
        {
            var productoraDB = await context.Productoras.FirstOrDefaultAsync(productora => productora.Id == id);

            if (productoraDB is null)
            {
                return NotFound();
            }

            mapper.Map(productoraCreacionDTO, productoraDB);

            if (productoraCreacionDTO.Logo is not null)
            {
                productoraDB.Logo = await almacenadorArchivos.Editar(
                    productoraDB.Logo,
                    contenedor,
                    productoraCreacionDTO.Logo);
            }

            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var productora = await context.Productoras.FirstOrDefaultAsync(productora => productora.Id == id);

            if (productora is null)
            {
                return NotFound();
            }

            context.Productoras.Remove(productora);
            await context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(productora.Logo))
            {
                await almacenadorArchivos.Borrar(productora.Logo, contenedor);
            }

            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }
    }
}