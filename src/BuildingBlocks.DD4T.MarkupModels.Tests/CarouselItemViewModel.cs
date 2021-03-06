﻿using System.Collections.Generic;
using BuildingBlocks.DD4T.MarkupModels.ExtensionMethods;

namespace BuildingBlocks.DD4T.MarkupModels.Tests
{
    [TridionViewModel(SchemaTitle = "Carousel Item", ParentLinkType = ParentLinkType.Nested, ParentLinkFieldName = "list")]
    public class CarouselItemViewModel
    {
        [TextField("heading")]
        public string Heading { get; set; }

        [TextField("summary")]
        public string Summary { get; set; }

        [MultimediaUrl("image")]
        public string ImageUrl { get; set; }
    }
}