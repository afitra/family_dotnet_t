using familyMart.Model;

namespace familyMart.Interface;

public interface IS3Repository
{
    Task<string> UploadPhotoToS3(UploadS3Data data);
}