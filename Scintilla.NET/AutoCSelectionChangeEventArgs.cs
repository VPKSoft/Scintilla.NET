﻿using System;

namespace ScintillaNET
{
    /// <summary>
    /// Provides data for the <see cref="Scintilla.AutoCSelectionChange" /> event.
    /// </summary>
    public class AutoCSelectionChangeEventArgs : EventArgs
    {
        private readonly Scintilla scintilla;
        private readonly IntPtr textPtr;
        private readonly int bytePosition;
        private int? position;
        private string text;

        /// <summary>
        /// Gets the list type of the user list or 0 for an autocompletion.
        /// </summary>
        /// <returns>The list type of the user list or 0 for an autocompletion.</returns>
        public int ListType { get; private set; }

        /// <summary>
        /// Gets the position where the list was displayed at.
        /// </summary>
        /// <returns>The zero-based document position where the list was displayed.</returns>
        public int Position
        {
            get
            {
                this.position = this.position ?? this.scintilla.Lines.ByteToCharPosition(this.bytePosition);

                return (int)this.position;
            }
        }

        /// <summary>
        /// Gets the text of the selected autocompletion item.
        /// </summary>
        /// <returns>The selected autocompletion item text.</returns>
        public unsafe string Text
        {
            get
            {
                if (this.text == null)
                {
                    int len = 0;
                    while (((byte*)this.textPtr)[len] != 0)
                        len++;

                    this.text = Helpers.GetString(this.textPtr, len, this.scintilla.Encoding);
                }

                return this.text;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCSelectionChangeEventArgs" /> class.
        /// </summary>
        /// <param name="scintilla">The <see cref="Scintilla" /> control that generated this event.</param>
        /// <param name="text">A pointer to the selected autocompletion text.</param>
        /// <param name="bytePosition">The zero-based byte position within the document where the list was displayed.</param>
        /// <param name="listType">The list type of the user list, or 0 for an autocompletion.</param>
        public AutoCSelectionChangeEventArgs(Scintilla scintilla, IntPtr text, int bytePosition, int listType)
        {
            this.scintilla = scintilla;
            this.textPtr = text;
            this.bytePosition = bytePosition;
            ListType = listType;
        }
    }
}
