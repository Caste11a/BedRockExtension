using System.Runtime.Serialization;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;
using BedRockExtension.Services;

namespace BedRockExtension.UI
{

    [DataContract]
    internal sealed class BedrockChatViewModel : NotifyPropertyChangedObject
    {
        private readonly VisualStudioExtensibility _ext;

        public BedrockChatViewModel(VisualStudioExtensibility ext)
        {
            _ext = ext;

            SendCommand = new AsyncCommand(async (parameter, ctx, ct) =>
            {
                var svc = new BedrockService(Region, Profile);
                var text = await svc.InvokeTextAsync(ModelId, Prompt ?? string.Empty, ct);
                Output = text;
                RaiseNotifyPropertyChangedEvent(nameof(Output));
            });

            InsertCommand = new AsyncCommand(async (parameter, ctx, ct) =>
            {
                var textView = await _ext.Editor().GetActiveTextViewAsync(ctx, ct);
                if (textView is null || string.IsNullOrEmpty(Output)) return;

                await _ext.Editor().EditAsync(batch =>
                {
                    // 選択範囲を置換（未選択ならキャレット位置に挿入）
                    textView.Document.AsEditable(batch).Replace(textView.Selection.Extent, Output);
                }, ct);
            });

            ClearCommand = new AsyncCommand((parameter, ctx, ct) =>
            {
                Output = string.Empty;
                RaiseNotifyPropertyChangedEvent(nameof(Output));
                return Task.CompletedTask;
            });
        }

        [DataMember] public string? Prompt { get; set; }
        [DataMember] public string ModelId { get; set; } = "amazon.titan-text-express-v1";
        [DataMember] public string Region { get; set; } = "ap-northeast-1";
        [DataMember] public string Profile { get; set; } = "bedrock";
        [DataMember] public string? Output { get; private set; }

        [DataMember] public IAsyncCommand SendCommand { get; }
        [DataMember] public IAsyncCommand InsertCommand { get; }
        [DataMember] public IAsyncCommand ClearCommand { get; }
    }

}



