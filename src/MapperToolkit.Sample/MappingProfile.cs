using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MapperToolkit.Sample;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //GenerateAllMapper<ArtistDto, Artist>();
        //GenerateAllMapper<TracksDto, Tracks>();
        //GenerateAllMapper<CopyrightDto, Copyright>();
        //GenerateAllMapper<ImageDto, Image>();
        //GenerateAllMapper<ExternalIdsDto, ExternalIds>();
        //GenerateAllMapper<ExternalUrlsDto, ExternalUrls>();
        //GenerateAllMapper<ItemDto, Item>().Map(src => src.Name.Substring(1, 2), dest => dest.Name);

        GenerateAllTransoform<ItemDto>("Shared", "Item11")

           .Map(src => src.Name.Substring(1, 2), "Name");

    }

}


