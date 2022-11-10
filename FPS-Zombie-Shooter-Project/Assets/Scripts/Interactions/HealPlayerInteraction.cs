
public sealed class HealPlayerInteraction : Interaction
{
    protected override void OnInteracted()
    {
        Destroy(gameObject);
    }
}