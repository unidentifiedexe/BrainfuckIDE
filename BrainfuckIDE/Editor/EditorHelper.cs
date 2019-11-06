﻿#nullable enable

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Editor
{
    static class EditorHelper
    {
        static public void Swap(this TextDocument document, int xStart, int xLength, int yStart, int yLength)
        {
            if (xStart > yStart)
            {
                (xStart, yStart) = (yStart, xStart);
                (xLength, yLength) = (yLength, xLength);
            }

            if (xStart + xLength > yStart)
                throw new ArgumentException();

            var xText = document.GetText(xStart, xLength);
            var yText = document.GetText(yStart, yLength);

            var intervalText = document.GetText(xStart + xLength, yStart - (xStart + xLength));

            var newTest = yText + intervalText + xText;

            var offsetChangeMap = new OffsetChangeMap()
            {
                new OffsetChangeMapEntry(yStart,yLength,xLength),
                new OffsetChangeMapEntry(xStart,xLength,yLength),
            };

            document.Replace(xStart, newTest.Length, newTest, offsetChangeMap);

        }

        static public bool IsSwapDocumentChangedEvent(this DocumentChangeEventArgs e)
        {
            return TryCreateSwapInfo(e, out _);
        }

        static public SwapInfo? TryCreateSwapInfo(this DocumentChangeEventArgs e)
        {
            return SwapInfo.CreateFrom(e);
        }
        static public bool TryCreateSwapInfo(this DocumentChangeEventArgs e, out SwapInfo? info)
        {
            info = SwapInfo.CreateFrom(e);
            return info != null;
        }


    }


    public class SwapInfo
    {
        private readonly SwapNode _lower;
        private readonly SwapNode _upper;

        static public SwapInfo? CreateFrom(DocumentChangeEventArgs e)
        {
            if (e is null) return null;

            if (e.RemovalLength != e.InsertionLength) return null;

            if (e.OffsetChangeMap.Count != 2) return null;

            var x = e.OffsetChangeMap[1];
            var y = e.OffsetChangeMap[0];

            if (x.Offset > y.Offset)
                (x, y) = (y, x);

            if (x.InsertionLength != y.RemovalLength) return null;
            if (y.InsertionLength != x.RemovalLength) return null;


            var xOldText = e.RemovedText.Text.Substring(0, x.RemovalLength);
            var xNewText = e.InsertedText.Text.Substring(e.InsertionLength - x.RemovalLength);
            if (xOldText != xNewText)
                return null;

            var yOldText = e.RemovedText.Text.Substring(e.RemovalLength - y.RemovalLength);
            var yNewText = e.InsertedText.Text.Substring(0, y.RemovalLength);
            if (yOldText != yNewText)
                return null;

            var xNode = new SwapNode(x.Offset, xNewText);
            var yNode = new SwapNode(y.Offset, yNewText);

            return new SwapInfo(xNode, yNode);
        }
        private SwapInfo(SwapNode lower, SwapNode upper)
        {
            _lower = lower;
            _upper = upper;
        }



        public SwapNode X => _lower;
        public SwapNode Y => _upper;


        public int GetNewOffset(int offset)
        {
            if (offset < _lower.OldOffset) return offset;
            else if (offset < _lower.OldEndOffset) return offset + (_upper.OldEndOffset - _lower.OldEndOffset);
            else if (offset < _upper.OldOffset) return offset + (_upper.Length - _lower.Length);
            else if (offset < _upper.OldEndOffset) return offset - (_upper.OldOffset - _lower.OldOffset);
            else return offset;
        }



        public struct SwapNode
        {
            public int OldOffset { get; }
            public string? Text { get; }

            public int Length => Text?.Length ?? 0;

            public int OldEndOffset => OldOffset + Length;


            public SwapNode(int oldOffset, string text)
            {
                OldOffset = oldOffset;
                Text = text;
            }

            public bool IsEmpty => Text is null;

            /// <summary> 変換前において、指定領域がこの領域に含まれるかどうか </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public bool WasIn(int offset)
            {
                return OldOffset <= offset && offset < OldOffset + Length;
            }

        }

    }


}
