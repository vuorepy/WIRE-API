using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wire.DTO.Document;
using Wire.Models;
using Wire.Routes;
using Wire.Services;

namespace Wire.Controllers
{
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;

        public DocumentController(IDocumentService documentService, IMapper mapper)
        {
            _documentService = documentService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route(ApiRoutes.Document.GetDocuments)]
        public async Task<IActionResult> GetDocuments()
        {
            var documents = await _documentService.GetAllDocumentsAsync();

            if (documents == null)
            {
                return NotFound();
            }

            var documentDtos = _mapper.Map<IEnumerable<GetDocumentDto>>(documents);
            return Ok(documentDtos);
        }

        [HttpGet]
        [Route(ApiRoutes.Document.GetDocument)]
        public async Task<IActionResult> GetDocument(string id)
        {
            var document = await _documentService.GetDocumentAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            var documentDto = _mapper.Map<GetDocumentDto>(document);
            return Ok(documentDto);
        }

        [HttpPost]
        [Route(ApiRoutes.Document.CreateDocument)]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDto createDocumentDto)
        {
            var document = _mapper.Map<Document>(createDocumentDto);
            var createdDocument = await _documentService.CreateDocumentAsync(document);

            if (createdDocument == null)
            {
                return BadRequest();
            }

            var createdDocumentDto = _mapper.Map<GetDocumentDto>(createdDocument);
            return Created(ApiRoutes.Document.GetDocument.Replace("{id}", createdDocumentDto.Id.ToString()), createdDocumentDto);
        }

        [HttpPut]
        [Route(ApiRoutes.Document.UpdateDocument)]
        public async Task<IActionResult> UpdateDocument(string id, [FromBody] UpdateDocumentDto updateDocumentDto)
        {
            var document = _mapper.Map<Document>(updateDocumentDto);
            var updatedDocument = await _documentService.UpdateDocumentAsync(id, document);

            if (updatedDocument == null)
            {
                return BadRequest();
            }

            var updatedDocumentDto = _mapper.Map<GetDocumentDto>(updatedDocument);
            return Ok(updatedDocumentDto);
        }

        [HttpDelete]
        [Route(ApiRoutes.Document.DeleteDocument)]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            var isDeleted = await _documentService.RemoveDocumentAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}