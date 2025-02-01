namespace Application.Features.Pictures;

public class SavePictureCommand : IRequest<Response<SavePictureCommandResponse>>
{
  public IFormFile File { get; set; }

  public class SavePictureCommandHandler : IRequestHandler<SavePictureCommand, Response<SavePictureCommandResponse>>
  {
    public async Task<Response<SavePictureCommandResponse>> Handle(SavePictureCommand request, CancellationToken cancellationToken)
    {
      try
      {
        if (request.File == null || request.File.Length == 0)
          return Response<SavePictureCommandResponse>.CreateErrorResponse("File is null or empty");

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "../picture");
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
        var filePath = Path.Combine(uploads, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await request.File.CopyToAsync(stream);
        }
        var fileUrl = $"{fileName}";
        return Response<SavePictureCommandResponse>.CreateSuccessResponse(new SavePictureCommandResponse(fileUrl));
      }
      catch (Exception ex)
      {
        return Response<SavePictureCommandResponse>.CreateErrorResponse(ex.Message);
      }
    }
  }
}

public class SavePictureCommandResponse
{
  public string Url { get; set; }

  public SavePictureCommandResponse(string url)
  {
    Url = url;
  }
}