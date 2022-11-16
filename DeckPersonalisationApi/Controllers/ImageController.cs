﻿using DeckPersonalisationApi.Exceptions;
using DeckPersonalisationApi.Middleware.JwtRole;
using DeckPersonalisationApi.Model;
using DeckPersonalisationApi.Model.Dto.External.GET;
using DeckPersonalisationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeckPersonalisationApi.Controllers;

[ApiController]
[Route("images")]
public class ImageController : Controller
{
    private ImageService _service;
    private JwtService _jwt;
    private UserService _user;

    public ImageController(ImageService service, JwtService jwt, UserService user)
    {
        _service = service;
        _jwt = jwt;
        _user = user;
    }

    [HttpGet("{id}")]
    public IActionResult GetImage(string id)
    {
        SavedImage? image = _service.GetImage(id);

        if (image == null)
            return new NotFoundResult();

        string path = _service.GetFullFilePath(image);
        Stream stream = System.IO.File.OpenRead(path);
        return new FileStreamResult(stream, "image/jpg");
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetImagesFromUser()
    {
        UserJwtDto? dto = _jwt.DecodeToken(Request);

        if (dto == null)
            throw new UnauthorisedException("User does not exist");

        User? user = _user.GetUserById(dto.Id);
        
        if (user == null)
            throw new UnauthorisedException("User does not exist");

        return new OkObjectResult(_service.GetImagesByUser(user).Select(x => x.Id).ToList());
    }

    [HttpPost]
    [Authorize]
    [JwtRoleReject(Permissions.FromApiToken)]
    public IActionResult PostImage(IFormFile file)
    {
        UserJwtDto dto = _jwt.DecodeToken(Request)!;

        return new OkObjectResult(
            new TokenGetDto(_service.CreateImage(file.OpenReadStream(), file.FileName, dto.Id).Id));
    }
}