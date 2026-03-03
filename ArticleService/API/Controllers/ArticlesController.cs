using ArticleService.Application.DTO;
using ArticleService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController(IArticleService service) : ControllerBase
    {
        
        private const string CreateArticleRoute = "create-article";
        private const string GetArticleByIdRoute = "get-article-by-id/{id:guid}";
        private const string GetAllArticlesRoute = "get-all-articles";
        private const string UpdateArticleRoute = "update-article/{id:guid}";
        private const string DeleteArticleRoute = "delete-article/{id:guid}";
        
        
        
        
        [HttpPost]
        [Route(CreateArticleRoute)]
        public async Task<IActionResult> Create([FromBody] CreateArticleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var article = await service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById),
                    new { id = article.Id, continent = article.Continent },
                    article);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpGet]
        [Route(GetArticleByIdRoute)]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] string? continent)
        {
            try
            {
                var article = await service.GetByIdAsync(id, continent);
                return article is null ? NotFound() : Ok(article);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpGet]
        [Route(GetAllArticlesRoute)]
        public async Task<IActionResult> GetAll([FromQuery] string? continent)
        {
            try
            {
                var articles = await service.GetAllAsync(continent);
                return Ok(articles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpPut]
        [Route(UpdateArticleRoute)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDto dto,
            [FromQuery] string? continent)
        {
            try
            {
                var updated = await service.UpdateAsync(id, dto, continent);
                return updated is null ? NotFound() : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpDelete]
        [Route(DeleteArticleRoute)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] string? continent)
        {
            try
            {
                var deleted = await service.DeleteAsync(id, continent);
                return deleted ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
