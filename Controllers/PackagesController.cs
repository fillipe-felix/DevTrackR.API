using DevTrackR.API.Entities;
using DevTrackR.API.Models;
using DevTrackR.API.Repository;

using Microsoft.AspNetCore.Mvc;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace DevTrackR.API.Controllers;

/// <summary>
/// Controller para gerenciar pacotes
/// </summary>
[ApiController]
[Route("api/packages")]
public class PackagesController : ControllerBase
{
    private readonly IPackageRepository _packageRepository;
    private readonly ISendGridClient _sendGridClient;

    public PackagesController(IPackageRepository packageRepository, ISendGridClient sendGridClient)
    {
        _packageRepository = packageRepository;
        _sendGridClient = sendGridClient;
    }

    /// <summary>
    /// Busca todos os pacotes
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetAll()
    {
        var packages = _packageRepository.GetAll();

        if (!packages.Any()) return NotFound();

        return Ok(packages);
    }

    /// <summary>
    /// Busca um pacote através do código
    /// </summary>
    /// <param name="code">código de rastreio</param>
    /// <returns></returns>
    [HttpGet("{code}")]
    public IActionResult GetByCode(string code)
    {
        var package = _packageRepository.GetByCode(code);

        if (package is null) return NotFound();

        return Ok(package);
    }

    /// <summary>
    /// Adiciona um pacote
    /// </summary>
    /// <param name="model">modelo de entrada</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(AddPackageInputModel model)
    {
        var package = new Package(model.Title, model.Weight);

        _packageRepository.Add(package);

        var message = new SendGridMessage
        {
            From = new EmailAddress("skambookapp@gmail.com", "DevTrackR"),
            Subject = "Your package was dispatched",
            PlainTextContent = $"Your package with code {package.Code} was dispatched"
        };
        
        message.AddTo(model.SenderEmail, model.SenderName);

        await _sendGridClient.SendEmailAsync(message);

        return CreatedAtAction("GetByCode", new { code = package.Code }, package);
    }

    /// <summary>
    /// Atualiza o status de um pacote
    /// </summary>
    /// <param name="code">código de rastreio do pacote</param>
    /// <param name="model">modelo de entrada</param>
    /// <returns></returns>
    [HttpPost("{code}/updates")]
    public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model)
    {
        var package = _packageRepository.GetByCode(code);

        if (package is null) return NotFound();

        package.AddUpdate(model.Status, model.Delivered);
        _packageRepository.Update(package);

        return NoContent();
    }
}
