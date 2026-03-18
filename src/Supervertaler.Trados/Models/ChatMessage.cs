using System;
using System.Collections.Generic;

namespace Supervertaler.Trados.Models
{
    public enum ChatRole
    {
        User,
        Assistant,
        System
    }

    public class ChatMessage
    {
        public ChatRole Role { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// Optional display-only override. When set, the chat bubble shows this text instead of
        /// <see cref="Content"/>. <see cref="Content"/> is always sent to the AI unchanged.
        /// Used to show a short summary (e.g. "[source document — 47 segments]") in place of a
        /// large {{PROJECT}} expansion so the chat history stays readable.
        /// </summary>
        public string DisplayContent { get; set; }

        /// <summary>
        /// Optional image attachments. Null means text-only (most messages).
        /// </summary>
        public List<ImageAttachment> Images { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>True if this message has one or more image attachments.</summary>
        public bool HasImages => Images != null && Images.Count > 0;
    }

    /// <summary>
    /// An image attached to a chat message for vision-enabled AI models.
    /// </summary>
    public class ImageAttachment
    {
        /// <summary>Raw image bytes (PNG or JPEG).</summary>
        public byte[] Data { get; set; }

        /// <summary>MIME type, e.g. "image/png", "image/jpeg".</summary>
        public string MimeType { get; set; }

        /// <summary>Display name for the thumbnail strip.</summary>
        public string FileName { get; set; }

        /// <summary>Original image width in pixels (for layout).</summary>
        public int Width { get; set; }

        /// <summary>Original image height in pixels (for layout).</summary>
        public int Height { get; set; }
    }

    /// <summary>
    /// Event args for chat send — carries both text and optional image attachments.
    /// </summary>
    public class ChatSendEventArgs : EventArgs
    {
        public string Text { get; set; }
        public List<ImageAttachment> Images { get; set; }

        /// <summary>
        /// Optional display-only text for the user bubble. When set, the bubble shows this
        /// instead of <see cref="Text"/>. <see cref="Text"/> is always sent to the AI.
        /// </summary>
        public string DisplayText { get; set; }
    }
}
