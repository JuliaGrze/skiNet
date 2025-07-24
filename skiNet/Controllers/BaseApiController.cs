using API.RequestHelpers;
using CORE.Entities;
using CORE.Interfaces;
using CORE.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        //zwraca encje z bazdy danych odpowiednio zpaginowana
        protected async Task<ActionResult> CreatePageResult<T>(IGenericRepository<T> repository, 
            ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
        {
            var items = await repository.ListAsyncWIithSpec(spec);
            var count = await repository.CountAsync(spec);

            var pagination = new Pagination<T>(pageIndex, pageSize, count, items);

            return Ok(pagination);
        }

        //zwraca dto danych odpowiednio zpaginowana
        protected async Task<ActionResult> CreatePageResult<T, TDto>(IGenericRepository<T> repository,
            ISpecification<T> spec, int pageIndex, int pageSize, Func<T, TDto> toDto) 
            where T : BaseEntity, IDtoConvertiable
        {
            var items = await repository.ListAsyncWIithSpec(spec);
            var count = await repository.CountAsync(spec);

            var dtoItems = items.Select(toDto).ToList();

            var pagination = new Pagination<TDto>(pageIndex, pageSize, count, dtoItems);

            return Ok(pagination);
        }
    }
}
