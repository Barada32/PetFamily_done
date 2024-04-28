﻿using CSharpFunctionalExtensions;
using PetFamily.Application.Features.SocialMedias;
using PetFamily.Domain.Common;
using PetFamily.Domain.Entities;
using PetFamily.Domain.ValueObjects;

namespace PetFamily.Application.Features.Volunteers.CreateSocialMedia;

public class CreateSocialMediaService
{
    private readonly ISocialMediaRepository _socialMediaRepository;
    private readonly IVolunteerRepository _volunteerRepository;

    public CreateSocialMediaService(
        ISocialMediaRepository socialMediaRepository, 
        IVolunteerRepository volunteerRepository)
    {
        _socialMediaRepository = socialMediaRepository;
        _volunteerRepository = volunteerRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateSocialMediaRequest request, CancellationToken ct)
    {
        var volunteer = await _volunteerRepository.GetById(request.VolunteerId, ct);
        if (volunteer.IsFailure)
            return volunteer.Error;

        var social = Social.Create(request.Social).Value;
        var socialMedia = SocialMedia.Create(request.Link, social);
        
        if (socialMedia.IsFailure)
            return socialMedia.Error;

        volunteer.Value.PublishSocialMedia(socialMedia.Value);

        return await _volunteerRepository.Save(volunteer.Value, ct);
    }
}