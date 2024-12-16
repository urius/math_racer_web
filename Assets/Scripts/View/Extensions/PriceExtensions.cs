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
                : ToGoldView(Mathf.Abs(priceInt));
        }
        public static string ToPriceView2(this int priceInt)
        {
            return priceInt > 0
                ? $"{Constants.TextSpriteCash} {priceInt}"
                : ToGoldView2(Mathf.Abs(priceInt));
        }

        public static string ToGoldView(this int priceInt)
        {
            return priceInt.ToGoldView(Constants.TextCrystalLiteBlueColor);
        }

        public static string ToGoldView(this int priceInt, string color)
        {
            return $"{RichTextHelper.FormatColor(priceInt, color)} {Constants.TextSpriteCrystal}";
        }

        public static string ToGoldView2(this int priceInt)
        {
            return $"{Constants.TextSpriteCrystal} {priceInt}";
        }
    }
}