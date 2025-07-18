﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DbOperation.Models;

public partial class CarFeaturesList
{
    public int featureId { get; set; }

    public string featureName { get; set; }

    public string featureDisplayName { get; set; }

    public string featureCategory { get; set; }

    public string featureSubCategory { get; set; }

    public string featureDescription { get; set; }

    public string featureImportanceLevel { get; set; }

    public string typicalFoundIn { get; set; }

    public bool? affectsResaleValue { get; set; }

    public bool? isStandardFeature { get; set; }

    public bool? isActiveFeature { get; set; }

    public string featureIcon { get; set; }

    public virtual ICollection<CarListingFeatures> CarListingFeatures { get; set; } = new List<CarListingFeatures>();
}