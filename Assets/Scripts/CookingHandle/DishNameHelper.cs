using System.Collections.Generic;

public static class DishNameHelper
{
    private static readonly Dictionary<BowlState, string> DishNames =
        new Dictionary<BowlState, string>
    {
        // ===== COMBO =====
        { BowlState.ComTrung, "Egg Rice" },
        { BowlState.ComSuon, "Grilled Pork Chop Rice" },
        { BowlState.TrungSuon, "Egg & Pork Rib" },

        { BowlState.ComGa, "Chicken Rice" },
        { BowlState.ComThitKho, "Braised Pork Rice" },
        { BowlState.ComCa, "Fish Rice" },
        { BowlState.ComDacBiet, "Special Rice" },

        // ===== SINGLE =====
        { BowlState.Com, "Rice" },
        { BowlState.Trung, "Egg" },
        { BowlState.Suon, "Pork Rib" },

        { BowlState.Ca, "Fish" },
        { BowlState.CaChua, "Tomato" },
        { BowlState.BongCai, "Broccoli" },
        { BowlState.ThitKho, "Braised Pork" },
        { BowlState.Ga, "Chicken" },
        { BowlState.So, "Clam" },
        { BowlState.TomHum, "Lobster" }
    };

    public static string GetDishName(BowlState state)
    {
        if (DishNames.TryGetValue(state, out var name))
            return name;

        return "Unknown Dish";
    }
}
