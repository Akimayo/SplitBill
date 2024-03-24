using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SplitBill.Models;

internal static class StreamReaderExtensions
{
#pragma warning disable CS8601 // Possible null reference assignment.
    readonly static FieldInfo charPosField = typeof(StreamReader).GetField("_charPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    readonly static FieldInfo charLenField = typeof(StreamReader).GetField("_charLen", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    readonly static FieldInfo charBufferField = typeof(StreamReader).GetField("_charBuffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#pragma warning restore CS8601 // Possible null reference assignment.

    /// <summary>
    /// Gets the actual position of the <see cref="StreamReader"/>.
    /// </summary>
    /// <remarks>
    /// From: <see href="https://stackoverflow.com/a/59442732"/>
    /// </remarks>
    /// <param name="reader">The <see cref="StreamReader"/> to find the position of</param>
    /// <returns>Actual position of stream</returns>
    internal static long ActualPosition(this StreamReader reader)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8605 // Unboxing a possibly null value.
        var charBuffer = (char[])charBufferField.GetValue(reader);
        var charLen = (int)charLenField.GetValue(reader);
        var charPos = (int)charPosField.GetValue(reader);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8605 // Unboxing a possibly null value.

        return reader.BaseStream.Position - reader.CurrentEncoding.GetByteCount(charBuffer, charPos, charLen - charPos);
    }
}
