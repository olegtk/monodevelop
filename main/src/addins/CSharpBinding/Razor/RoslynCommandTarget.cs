//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//

using System;
using Microsoft.CodeAnalysis.Editor.Shared.Utilities;
using Microsoft.CodeAnalysis.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using MonoDevelop.Ide.Composition;

namespace Microsoft.VisualStudio.Platform
{
    public class RoslynCommandTarget
    {
        internal IEditorCommandHandlerService CurrentHandlers { get; set; }

        internal ITextBuffer _languageBuffer;
        internal ITextView _textView;

        static RoslynCommandTarget()
        {
            var defaultForegroundThreadData = ForegroundThreadData.CreateDefault(
                defaultKind: ForegroundThreadDataKind.ForcedByPackageInitialize);
            ForegroundThreadAffinitizedObject.CurrentForegroundThreadData = defaultForegroundThreadData;
        }

        private RoslynCommandTarget(ITextView textView, ITextBuffer languageBuffer)
        {
            var commandHandlerServiceFactory = CompositionManager.GetExportedValue<IEditorCommandHandlerServiceFactory>();

            if (commandHandlerServiceFactory != null)
            {
                CurrentHandlers = commandHandlerServiceFactory.GetService(textView, languageBuffer);
            }

            _languageBuffer = languageBuffer;
            _textView = textView;
        }

        public static RoslynCommandTarget FromViewAndBuffer(ITextView textView, ITextBuffer languageBuffer)
        {
            return languageBuffer.Properties.GetOrCreateSingletonProperty<RoslynCommandTarget>(() => new RoslynCommandTarget(textView, languageBuffer));
        }

        public void ExecuteTypeCharacter(char typedChar, Action lastHandler)
        {
            CurrentHandlers?.Execute ((view, buffer) => new TypeCharCommandArgs (_textView, _languageBuffer, typedChar),
				nextCommandHandler: lastHandler);
        }

        public void ExecuteTab (Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new TabKeyCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteBackspace(Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new BackspaceKeyCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteDelete(Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new DeleteKeyCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteReturn(Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new ReturnKeyCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteUp (Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new UpKeyCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteDown (Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new DownKeyCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteUncommentBlock(Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new UncommentSelectionCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteCommentBlock(Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new CommentSelectionCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }

        public void ExecuteInvokeCompletionList (Action executeNextCommandTarget)
        {
            CurrentHandlers.Execute ((view, buffer) => new InvokeCompletionListCommandArgs (_textView, _languageBuffer),
                nextCommandHandler: executeNextCommandTarget);
        }
    }
}