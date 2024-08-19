using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ScriptController : ControllerBase
{
    [HttpGet("chat-widget.js")]
    public IActionResult GetChatWidgetScript()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "chat-widget.js");
        var fileContent = System.IO.File.ReadAllText(filePath);
        return Content(fileContent, "application/javascript");
    }
}