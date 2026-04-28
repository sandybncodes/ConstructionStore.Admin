using System.Globalization;

namespace ConstructionStore.Admin.Services;

/// <summary>
/// Utility methods for building variant display names, matching the logic used
/// in the ConstructionStore.Web storefront (variantUtils.js / buildVariantSuffix).
/// </summary>
public static class VariantUtils
{
    private static readonly HashSet<string> DimensionExact = new(StringComparer.OrdinalIgnoreCase)
    {
        "inaltime", "lungime", "grosime", "greutate", "latime"
    };

    private static bool IsDimensionAttr(string name)
    {
        var lower = (name ?? string.Empty).ToLowerInvariant();
        return DimensionExact.Contains(lower) || lower.StartsWith("greu");
    }

    /// <summary>
    /// Formats a numeric value removing unnecessary trailing zeros.
    /// 100.00 → "100", 10.50 → "10.5", 10.56 → "10.56"
    /// Matches JS: parseFloat(n).toString()
    /// </summary>
    private static string FmtNum(decimal? n)
    {
        if (n == null) return string.Empty;
        return n.Value.ToString("G29", CultureInfo.InvariantCulture);
    }

    private static string RawVal(VariantAttributeValueModel a) =>
        a.ValueNumeric != null ? FmtNum(a.ValueNumeric) : (a.ValueText ?? string.Empty);

    private static bool HasVal(VariantAttributeValueModel a) =>
        a.ValueNumeric != null || !string.IsNullOrWhiteSpace(a.ValueText);

    private static string FmtPrimary(VariantAttributeValueModel a)
    {
        var val = RawVal(a);
        return string.IsNullOrWhiteSpace(a.Unit) ? val : $"{val} {a.Unit}";
    }

    /// <summary>
    /// Builds a compact dimension/weight suffix for a variant, matching the Web storefront rules:
    /// - LUNGIME + LATIME [+ GROSIME] → "200x100 cm" or "200x100x5 cm"
    /// - INALTIME + LATIME [+ GROSIME] → "100x50 cm" or "100x50x5 cm"
    /// - INALTIME primary              → "100 cm, 5 kg"
    /// - GREUTATE primary              → "5 kg, 100 cm"
    /// - Densitatea materialului appended last, comma-separated
    /// </summary>
    public static string BuildVariantSuffix(ProductVariantModel v)
    {
        var dims = v.Attributes.Where(a => IsDimensionAttr(a.AttributeName)).ToList();
        var density = v.Attributes.FirstOrDefault(a => a.AttributeName.ToLowerInvariant().Contains("densitat"));

        string AppendDensity(string suffix)
        {
            if (density == null || !HasVal(density)) return suffix;
            var d = FmtPrimary(density);
            return string.IsNullOrEmpty(suffix) ? d : $"{suffix}, {d}";
        }

        if (dims.Count == 0) return AppendDensity(string.Empty);

        var lungime  = dims.FirstOrDefault(a => a.AttributeName.Equals("lungime",  StringComparison.OrdinalIgnoreCase));
        var latime   = dims.FirstOrDefault(a => a.AttributeName.Equals("latime",   StringComparison.OrdinalIgnoreCase));
        var grosime  = dims.FirstOrDefault(a => a.AttributeName.Equals("grosime",  StringComparison.OrdinalIgnoreCase));
        var inaltime = dims.FirstOrDefault(a => a.AttributeName.Equals("inaltime", StringComparison.OrdinalIgnoreCase));
        var greutate = dims.FirstOrDefault(a => a.AttributeName.ToLowerInvariant().StartsWith("greu"));

        // LUNGIME + LATIME [+ GROSIME] → cross-product block
        if (lungime != null && latime != null)
        {
            var crossParts = new List<VariantAttributeValueModel> { lungime, latime };
            var inCross = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "lungime", "latime" };
            if (grosime != null && HasVal(grosime))
            {
                crossParts.Add(grosime);
                inCross.Add("grosime");
            }
            var unit = lungime.Unit ?? string.Empty;
            var crossStr = string.Join("x", crossParts.Select(a => RawVal(a)))
                           + (string.IsNullOrEmpty(unit) ? string.Empty : $" {unit}");
            var otherStr = string.Join(", ", dims
                .Where(a => !inCross.Contains(a.AttributeName))
                .Where(HasVal)
                .Select(FmtPrimary));
            return AppendDensity(string.IsNullOrEmpty(otherStr) ? crossStr : $"{crossStr}, {otherStr}");
        }

        // INALTIME + LATIME [+ GROSIME] → cross-product block
        if (inaltime != null && latime != null)
        {
            var crossParts = new List<VariantAttributeValueModel> { inaltime, latime };
            var inCross = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inaltime", "latime" };
            if (grosime != null && HasVal(grosime))
            {
                crossParts.Add(grosime);
                inCross.Add("grosime");
            }
            var unit = inaltime.Unit ?? string.Empty;
            var crossStr = string.Join("x", crossParts.Select(a => RawVal(a)))
                           + (string.IsNullOrEmpty(unit) ? string.Empty : $" {unit}");
            var otherStr = string.Join(", ", dims
                .Where(a => !inCross.Contains(a.AttributeName))
                .Where(HasVal)
                .Select(FmtPrimary));
            return AppendDensity(string.IsNullOrEmpty(otherStr) ? crossStr : $"{crossStr}, {otherStr}");
        }

        // INALTIME primary
        if (inaltime != null)
        {
            var otherStr = string.Join(", ", dims
                .Where(a => !ReferenceEquals(a, inaltime))
                .Where(HasVal)
                .Select(FmtPrimary));
            return AppendDensity(string.IsNullOrEmpty(otherStr)
                ? FmtPrimary(inaltime)
                : $"{FmtPrimary(inaltime)}, {otherStr}");
        }

        // GREUTATE primary
        if (greutate != null)
        {
            var otherStr = string.Join(", ", dims
                .Where(a => !ReferenceEquals(a, greutate))
                .Where(HasVal)
                .Select(FmtPrimary));
            return AppendDensity(string.IsNullOrEmpty(otherStr)
                ? FmtPrimary(greutate)
                : $"{FmtPrimary(greutate)}, {otherStr}");
        }

        // Fallback: all as value+unit comma-separated
        return AppendDensity(string.Join(", ", dims.Where(HasVal).Select(FmtPrimary)));
    }

    /// <summary>
    /// Returns the full display name for a product variant in the format used by the Web storefront:
    /// "Product Name 200x100 cm" (no dash, space separator, Web-format suffix).
    /// </summary>
    public static string GetVariantDisplayName(ProductModel p, ProductVariantModel v)
    {
        var suffix = BuildVariantSuffix(v);
        return string.IsNullOrEmpty(suffix) ? p.Name : $"{p.Name} {suffix}";
    }
}
