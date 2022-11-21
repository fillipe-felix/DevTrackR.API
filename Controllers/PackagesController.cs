using DevTrackR.API.Entities;
using DevTrackR.API.Models;
using DevTrackR.API.Persistence;

using Microsoft.AspNetCore.Mvc;

namespace DevTrackR.API.Controllers;

/// <summary>
/// Controller para gerenciar pacotes
/// </summary>
[ApiController]
[Route("api/packages")]
public class PackagesController : ControllerBase
{
    private readonly DevTrackRContext _context;

    public PackagesController(DevTrackRContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca todos os pacotes
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetAll()
    {
        var packages = _context.Packages;

        if (!packages.Any())
        {
            return NotFound();
        }
        
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
        var package = _context.Packages.SingleOrDefault(p => p.Code == code);

        if (package is null)
        {
            return NotFound();
        }

        return Ok(package);
    }
    
    /// <summary>
    /// Adiciona um pacote
    /// </summary>
    /// <param name="model">modelo de entrada</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Post(AddPackageInputModel model)
    {
        var package = new Package(model.Title, model.Weight);

        _context.Packages.Add(package);
        
        return CreatedAtAction("GetByCode", new { code = package.Code}, package);
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
        var package = _context.Packages.SingleOrDefault(p => p.Code == code);

        if (package is null)
        {
            return NotFound();
        }

        package.AddUpdate(model.Status, model.Delivered);
        
        return NoContent();
    }
}
