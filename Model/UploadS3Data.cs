namespace familyMart.Model;

public class UploadS3Data
{
    public Stream FileStream { get; set; }
    public string FileName { get; set; }
    public string AccesKey { get; set; }
    public string SecretKey { get; set; }
    public string ServiceUrl { get; set; }
    public string BucketName { get; set; }
}