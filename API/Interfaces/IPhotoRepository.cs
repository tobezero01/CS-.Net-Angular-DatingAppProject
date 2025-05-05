using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Photo?> GetPhotoById(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="photo"></param>
    void RemovePhoto(Photo photo);
}
