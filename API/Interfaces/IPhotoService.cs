using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IPhotoService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="publicId"></param>
    /// <returns></returns>
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
