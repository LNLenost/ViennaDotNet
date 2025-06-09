namespace ViennaDotNet.ApiServer.Types.Catalog;

public sealed record RecipesCatalog(
    RecipesCatalog.CraftingRecipe[] crafting,
    RecipesCatalog.SmeltingRecipe[] smelting
)
{
    public sealed record CraftingRecipe(
        string id,
        string category,
        string duration,
        CraftingRecipe.Ingredient[] ingredients,
        CraftingRecipe.Output output,
        CraftingRecipe.ReturnItem[] returnItems,
        bool deprecated
    )
    {
        public sealed record Ingredient(
            string[] items,
            int quantity
        );

        public sealed record Output(
            string itemId,
            int quantity
        );

        public sealed record ReturnItem(
            string id,
            int amount
        );
    }

    public sealed record SmeltingRecipe(
        string id,
        int heatRequired,
        string inputItemId,
        SmeltingRecipe.Output output,
        SmeltingRecipe.ReturnItem[] returnItems,
        bool deprecated
    )
    {
        public sealed record Output(
            string itemId,
            int quantity
        );

        public sealed record ReturnItem(
            string id,
            int amount
        );
    }
}
