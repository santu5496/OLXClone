public class CarVariantViewModel
{
    public int variantId { get; set; }
    public string variantName { get; set; }
    public string variantTrimLevel { get; set; }
    public decimal? variantLaunchPrice { get; set; }
    public decimal? variantCurrentPrice { get; set; }
    public bool? isActiveVariant { get; set; }
    public DateTime? variantCreatedDate { get; set; }
    public string variantFeatures { get; set; }

    public int modelId { get; set; }
    public string modelName { get; set; }

    public int brandId { get; set; }
    public string brandName { get; set; }
}
