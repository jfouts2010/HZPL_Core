namespace ScriptableObjects.Gameplay.Units
{
    /// <summary>
    /// A composition line after the battalion ID has been resolved to concrete battalion data.
    /// </summary>
    public readonly struct ResolvedBattalionComposition
    {
        public BattalionData Battalion { get; }
        public int Count { get; }

        public ResolvedBattalionComposition(BattalionData battalion, int count)
        {
            Battalion = battalion;
            Count = count;
        }
    }
}
