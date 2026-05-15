using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkHub.Application.Common.Models;

namespace TalkHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 10 * 1024 * 1024;

    public MediaController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload-avatar")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<string>.FailureResponse(400, "Không có file nào được chọn."));
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(ApiResponse<string>.FailureResponse(400, "Kích thước file không được vượt quá 10MB."));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse<string>.FailureResponse(400, "Định dạng file không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG, WEBP."));
        }

        try
        {
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "avatars");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/avatars/{fileName}";
            return Ok(ApiResponse<string>.SuccessResponse(url, "Tải lên thành công."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.FailureResponse(500, $"Lỗi hệ thống: {ex.Message}"));
        }
    }
}
