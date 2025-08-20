using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace BedRockExtension.UI
{
    internal sealed class BedrockChatContent : RemoteUserControl
    {
        public BedrockChatContent(BedrockChatViewModel viewModel) : base(viewModel) { }
    }
}
