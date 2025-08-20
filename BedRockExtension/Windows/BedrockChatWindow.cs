using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using BedRockExtension.UI;

namespace BedRockExtension.Windows
{
    [VisualStudioContribution]
    public class BedrockChatWindow : ToolWindow
    {
        private readonly BedrockChatContent _content;

        public BedrockChatWindow(VisualStudioExtensibility extensibility) : base(extensibility)
        {
            Title = "Bedrock Chat";
            _content = new BedrockChatContent(new BedrockChatViewModel(extensibility));
        }

        public override ToolWindowConfiguration ToolWindowConfiguration => new()
        {
            //Placement = ToolWindowPlacement.Right
            // DockDirection / Toolbar など必要に応じて追加
            
        };

        public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
            => Task.FromResult<IRemoteUserControl>(_content);

        protected override void Dispose(bool disposing)
        {
            if (disposing) _content?.Dispose();
            base.Dispose(disposing);
        }
    }
}
