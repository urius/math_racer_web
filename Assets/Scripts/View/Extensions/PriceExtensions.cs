using Data;
using UnityEngine;
using View.Helpers;

namespace View.Extensions
{
    public static class PriceExtensions
    {
        public static string ToPriceView(this int priceInt)
        {
            return priceInt > 0
                ? $"{RichTextHelper.FormatGreen(priceInt)} {Constants.TextSpriteCash}"
                : $" {RichTextHelper.FormatColor(Mathf.Abs(priceInt), Constants.TextCrystalLiteBlueColor)} {Constants.TextSpriteCrystal}";
        }
    }
}