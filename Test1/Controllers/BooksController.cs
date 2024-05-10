using Microsoft.AspNetCore.Mvc;
using Test1.DTOs;
using Test1.Repositories;

namespace Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly IBookRepository _bookRepository;
        private readonly IPublishingHousesRepository _publishingHousesRepository;
        
        public BooksController(IBookRepository bookRepository, IPublishingHousesRepository publishingHousesRepository)
        {
            _bookRepository = bookRepository;
            _publishingHousesRepository = publishingHousesRepository;
        }
        
        [HttpGet("{id:int}/editions")]
        public async Task<IActionResult> GetBookEditions(int id)
        {
            if (!await _bookRepository.DoesBookExist(id))
                return NotFound($"Animal with given ID - {id} doesn't exist");

            if (!await _bookRepository.DoesBookHaveEditions(id))
                return NotFound($"Book with given ID - {id} doesn't have any editions");
            
            var editions = await _bookRepository.GetBookEditions(id);
            
            return Ok(editions);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookWithPublisherDto addBookWithPublisherDto)
        {
            if (!await _publishingHousesRepository.DoesPublishingHouseExist(addBookWithPublisherDto.PublishingHouseId))
                return NotFound($@"Publishing house with given ID -
                                  {addBookWithPublisherDto.PublishingHouseId} doesn't exist");
            
            await _bookRepository.AddBookWithPublishingHouse(addBookWithPublisherDto);
            
            return Created(Request.Path.Value ?? "api/books", addBookWithPublisherDto);
        }
    }
}
