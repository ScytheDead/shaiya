namespace Imgeneus.World.SelectionScreen
{
    public interface ISelectionScreenFactory
    {
        /// <summary>
        /// Creates instance of selection screen for each client.
        /// </summary>
        public ISelectionScreenManager CreateSelectionManager(WorldClient client);
    }
}
